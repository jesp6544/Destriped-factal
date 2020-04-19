using System;
using System.Linq;
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
                    Height = 320,
                    Width = 320,
                    XMax = -1.3,
                    XMin = -2.1,
                    YMax = 1.3,
                    YMin = -1.3
                };
                var render = new Render();
                var result = render.GetMandlebrot(testJob);
                result.Save("test");
            }
            Console.WriteLine("Hello World!");
        }
    }
}