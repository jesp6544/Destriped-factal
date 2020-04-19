using System;
using System.Drawing;
using Core.Models;

namespace Core
{
    public class Render
    {
        public Bitmap GetMandlebrot(Job job)
        {
            
            //MessageBox.Show(xmin + "\n" + xmax + "\n" + ymin + "\n" + ymax + "\n");
            // Holds all of the possible colors
            //Color[] cs = new Color[256];

            // Creates the Bitmap we draw to
            var b = new Bitmap(job.Width, job.Height);
            double x, y, x1, y1, xx, intigralX, intigralY = 0.0;

            int looper, s, z = 0;
            intigralX = (job.XMax - job.XMin) / b.Width;
            intigralY = (job.YMax - job.YMin) / b.Height;
            x = job.XMin;

            for (s = 1; s < b.Width; s++)
            {
                y = job.YMin;
                for (z = 1; z < b.Height; z++)
                {
                    x1 = 0;
                    y1 = 0;
                    looper = 0;
                    while (looper < 250 && Math.Sqrt((x1 * x1) + (y1 * y1)) < 2)
                    {
                        looper++;
                        xx = (x1 * x1) - (y1 * y1) + x;
                        y1 = 2 * x1 * y1 + y;
                        x1 = xx;
                    }

                    // Get the percent of where the looper stopped
                    double perc = looper / (250.0);
                    // Get that part of a 255 scale
                    int val = ((int)(perc * 255));
                    // Use that number to set the color
                    Color color = Color.FromArgb(val, 0, 0);
                    b.SetPixel(s, z, color);
                    y += intigralY;
                }
                x += intigralX;
            }
            Console.WriteLine("Done render");
            return b;
        }
    }
}