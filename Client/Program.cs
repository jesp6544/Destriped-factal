using System;
using System.Drawing.Imaging;
using System.Net;
using System.Threading;
using Core;
using Core.Models;

namespace Client
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="-test or --t">Run the program in test mode, just output a render in the run folder</param>
        /// <param name="-master or --m">The Ip address to auto connect to</param>
        static void Main(string[] args)
        {
            var inputArgs = GetValuesFromArgs(args);
            
            if (inputArgs.testMode)
            {
                var testJob = new Job
                {
                    Height = 500,
                    Width = 500,
                    XMax = 1,
                    XMin = -3,
                    YMax = 2,
                    YMin = -2
                };
                var render = new Render();
                var result = render.GetMandlebrot(testJob);
                result.Save("test_output.png", ImageFormat.Png);
            }
            else
            {
                SpawnFarmingThread(inputArgs.ipAddress);
            }
            Console.WriteLine("Hello World!");
        }

        static (IPAddress ipAddress, bool testMode) GetValuesFromArgs(string[] args)
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var testMode = false;
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg == "-test" || arg == "--t")
                {
                    testMode = true;
                }

                if ((arg == "--m" || arg == "-master") && args.Length > i +1)
                {
                    var validIp = IPAddress.TryParse(args[i + 1], out var passedAddress);
                    if (validIp)
                    {
                        ipAddress = passedAddress;
                    }
                    else
                    {
                        Console.WriteLine("invalid IP given after --m or -master");
                        throw new InvalidOperationException();
                    }
                }
            }

            return (ipAddress, testMode);
        }
        
        static void SpawnFarmingThread(IPAddress ipAddress)
        {
            var fc = new FarmCommunicator();
            var t = new Thread(() => fc.Start(ipAddress));
            t.Start();
        }
    }
}