using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Core.Models;

namespace Core
{
    public class ThreadedRender : IRender
    {
        private int _threadCounter = 0;
        private int _loops = 1000;
        private int _threads = 24;
        private List<(Guid id, Thread thread)> _renderThreads = new List<(Guid id, Thread thread)>();
        private ConcurrentBag<(int x, int y, Color color)> _rawResult = new ConcurrentBag<(int, int, Color)>();
        
        public Bitmap GetMandlebrot(Job job)
        {
            
            //MessageBox.Show(xmin + "\n" + xmax + "\n" + ymin + "\n" + ymax + "\n");
            // Holds all of the possible colors
            //Color[] cs = new Color[256];

            // Creates the Bitmap we draw to
            var b = new Bitmap(job.Width, job.Height);
            var intigralX = (job.XMax - job.XMin) / b.Width;
            var intigralY = (job.YMax - job.YMin) / b.Height;
            double x, y = 0.0;

            int s, z = 0;
            
            x = job.XMin;

            for (s = 1; s < b.Width; s++)
            {
                y = job.YMin;
                for (z = 1; z < b.Height; z++)
                {
                    StartRenderThread(x, y, s, z, b.Height, intigralX);
                    y += intigralY;
                }
                
                // for (z = 1; z < b.Height; z++)
                // {
                //     StartRenderThread(x, y, s, z);
                //     y += intigralY;
                // }
                x += intigralX;
            }

            b = PutPictureTogether(b);
            
            Console.WriteLine("Done render");
            return b;
        }

        private Bitmap PutPictureTogether(Bitmap b)
        {
            foreach (var result in _rawResult)
            {
                b.SetPixel(result.x, result.y, result.color);
            }

            return b;
        }

        private void StartRenderThread(double x, double y, int s, int z, int height, double intigralY)
        {
            var id = Guid.NewGuid();
            Console.WriteLine(_renderThreads.Count);
            while (_threadCounter >= _threads)
            {
                Thread.Sleep(10);
            }
            
            var t = new Thread(() =>
            {
                try
                {
                    var result = Runner(x, y, s, z, height, intigralY);
                    _threadCounter += 1;
                    // _rawResult.Add(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                
                _threadCounter -= 1;
                    // Remove(_renderThreads.Single(rt => rt.id == id));
                
                // thisThread.thread.Abort();
            });
            _renderThreads.Add((id, t));
            t.Start();
            
        }

        private ConcurrentBag<(int, int, Color)> Runner(double x, double y, int s, int z, int height, double intigralY)
        {
            var donePixels = new ConcurrentBag<(int, int, Color)>();
            
            for (z = 1; z < height; z++)
            {
                double x1;
                double y1;
                int looper;
                double xx;
                x1 = 0;
                y1 = 0;
                looper = 0;
                while (looper < _loops && Math.Sqrt((x1 * x1) + (y1 * y1)) < 2)
                {
                    looper++;
                    xx = (x1 * x1) - (y1 * y1) + x;
                    y1 = 2 * x1 * y1 + y;
                    x1 = xx;
                }

                // Get the percent of where the looper stopped
                double perc = looper / (double) _loops;
                // Get that part of a 255 scale
                int val = ((int) (perc * 255));
                // Use that number to set the color
                Color color = Color.FromArgb(val, 0, 0);
                // Console.WriteLine((s,z,color).GetHashCode());
                _rawResult.Add((s, z, color));
                // donePixels.Add((s, z, color)); 
                y += intigralY;
            }

            return donePixels;
        }
    }
}