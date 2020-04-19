using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Core;
using Core.Models;
using Newtonsoft.Json;

namespace Client
{
    public class FarmCommunicator
    {
        private Render _render;
        public FarmCommunicator()
        {
            _render = new Render();
        }
        public void Start(IPAddress ipAddress)
        {
            var connection = EstablishConnectionToMaster(ipAddress);
            PerformHandshakeWithMaster(connection);
            while (true)
            {
                var job = AwaitJob(connection);
                var result = PerformJob(job);
                SendResultBack(connection, result);
            }
        }


        private Socket EstablishConnectionToMaster(IPAddress ipAddress)
        {
            var connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            connection.Connect(ipAddress, 9001);
            return connection;
        }

        private void PerformHandshakeWithMaster(Socket connection)
        {
            Console.WriteLine("FirstContact");
            var msg = Environment.ProcessorCount.ToString();
            var msgbyte = Encoding.ASCII.GetBytes(msg);
            connection.Send(msgbyte);
        }

        private Job AwaitJob(Socket connection)
        {
            var bytes = new byte[1024];

            connection.Receive(bytes);
            Console.WriteLine("Got a job");
            var jobString = Encoding.ASCII.GetString(bytes);
            var job = JsonConvert.DeserializeObject<Job>(jobString);
            Console.WriteLine("value: {0} - {1},", "xmin", job.XMin);
            Console.WriteLine("value: {0} - {1},", "xmax", job.XMax);
            Console.WriteLine("value: {0} - {1},", "ymin", job.YMin);
            Console.WriteLine("value: {0} - {1},", "ymax", job.YMax);
            Console.WriteLine("value: {0} - {1},", "height", job.Height);
            Console.WriteLine("value: {0} - {1},", "weidth", job.Width);
            bytes = null; // mem cleanup
            return job;
        }

        private Bitmap PerformJob(Job job)
        {
            return _render.GetMandlebrot(job);
        }
        
        private void SendResultBack(Socket connection, Bitmap result)
        {
            var piece = TurnBitmapIntoPiece(result);
            var pieceString = JsonConvert.SerializeObject(piece);    //Converts the job Object to a json string
            var pieceByte = Encoding.ASCII.GetBytes(pieceString);    //Converts that string to bytes
            Console.WriteLine("sending piece");
            connection.Send(pieceByte);
            Console.WriteLine("Done sending piece");
            piece = null;
        }

        private Piece TurnBitmapIntoPiece(Bitmap bitmap)
        {
            var piece = new Piece();
            Console.WriteLine("Started sending");
            var pix = new Pixel();
            var width = bitmap.Width;
            for (var i = 0; i < bitmap.Width; i++)
            {
                for (var j = 0; j < bitmap.Height; j++)
                {
                    pix.Placement = (uint)((width * j) + i);
                    pix.Color = bitmap.GetPixel(i, j);
                    Console.WriteLine($"now adding pix to piece, placement: {pix.Placement}, Color: {pix.Color}");
                    // Console.WriteLine(result.GetPixel(i, j) + "");
                    piece.Pixels.Add(pix);
                }
            }

            return piece;
        }
    }
}
