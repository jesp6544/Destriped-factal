using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Core.Models;
using Newtonsoft.Json;

namespace Server
{
    public class MasterCommunicator
    {
        private List<Worker> _renderFarm = new List<Worker>();
        private int _renderHeight = 500;
        private int _renderWidth = 500;
        
        public void Connect()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var localEndPoint = new IPEndPoint(IPAddress.Any, 9001);

            socket.Bind(localEndPoint);
            socket.Listen(100);
            
            while (true)    // Stopping token
            {
                var clientSocket = socket.Accept();
                Console.WriteLine("A new client connected");
                var bytes = new byte[1024];
                clientSocket.Receive(bytes);

                var temp = new Worker
                {
                    Ip = localEndPoint.Address, //Not the right address
                    Socket = clientSocket,
                    Threads = Convert.ToUInt16(Encoding.ASCII.GetString(bytes))
                };
                
                _renderFarm.Add(temp);
                var clientThread = new Thread(() =>
                {
                    Resive(temp);
                });
                
                clientThread.Start();
            }
        }
        
        private void Resive(Worker worker)
        {
            try
            {
                var bytes = new byte[1024];
                while (true)
                {
                    worker.Socket.Receive(bytes);
                    var pieceString = Encoding.ASCII.GetString(bytes);
                    var piece = JsonConvert.DeserializeObject<Piece>(pieceString);
                    var bitmap = new Bitmap(_renderWidth, _renderHeight);
                    foreach (var i in piece.Pixels)
                    {
                        int y = Convert.ToInt32(i.Placement) % _renderHeight;
                        int x = Convert.ToInt32(i.Placement) - _renderWidth * y;
                        bitmap.SetPixel(x, y, i.Color);
                    }
                    //string pieceString = Encoding.ASCII.GetString(bytes);
                    //Piece piece = JsonConvert.DeserializeObject<Piece>(pieceString);
                    //foreach (var i in ClientDatagridview)
                    //{

                    //}
                    
                    //if (running == true) {	//If there still are jobs to be done, send the worker a new one
                    //    Send(worker);
                    //}
                }
            }
            catch (Exception)
            {
                //TODO: Make a popup that askes to remove the tread or retry
                Thread.CurrentThread.Abort();
            }
        }
    }
}