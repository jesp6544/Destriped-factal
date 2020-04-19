using System;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Threading;
using Core;
using Core.Models;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("test"))
            {
                var testJob = new Job
                {
                    Height = 500,
                    Width = 500,
                    XMax = -1.3,
                    XMin = -2.1,
                    YMax = 1.3,
                    YMin = -1.3
                };
                var render = new Render();
                var result = render.GetMandlebrot(testJob);
                result.Save("test.png", ImageFormat.Png);
            }
            else
            {
                var ip = GetIpAddress(args);
                SpawnFarmingThread(ip);
            }
            Console.WriteLine("Hello World!");
        }

        static IPAddress GetIpAddress(string[] args)
        {
            // TODO allow input or args
            return IPAddress.Parse("127.0.0.1");
        }
        
        static void SpawnFarmingThread(IPAddress ipAddress)
        {
            var bla = new FarmCommunicator();
            var t = new Thread(() => bla.Start(ipAddress));
            t.Start();
        }
    }
}