using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;

namespace FractalClient
{
    class Program
    {
		static Socket host = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
			Console.WriteLine("This is the render client program");
			Console.WriteLine("Please wait make sure the render host is running");
			Console.WriteLine("Number Of Logical Processors: {0}", Environment.ProcessorCount);
			Console.WriteLine("Please write host IP: ");
			IPAddress goodIP;
			string ip = Console.ReadLine();
			if (IPAddress.TryParse(ip, out goodIP))
			{ 
				Connect(goodIP);
			}


		}
		static void Connect(IPAddress goodIP) {
			host.Connect(goodIP, 9001);
			Console.WriteLine("Connect! ");
			FirstContact();
		}
		static void FirstContact() {
			string msg = "4";
			byte[] msgbyte = Encoding.ASCII.GetBytes(msg);
			host.Send(msgbyte);
			byte[] bytes = new byte[1024];
			host.Receive(bytes);
		}
		public Bitmap Render(Job job)
		{
			//MessageBox.Show(xmin + "\n" + xmax + "\n" + ymin + "\n" + ymax + "\n");
			// Holds all of the possible colors
			//Color[] cs = new Color[256];

			// Creates the Bitmap we draw to
			Bitmap b = new Bitmap(job.width, job.height);
			double x, y, x1, y1, xx, intigralX, intigralY = 0.0;

			int looper, s, z = 0;
			intigralX = (job.xmax - job.xmin) / b.Width;
			intigralY = (job.ymax - job.ymin) / b.Height;
			x = job.xmin;

			for (s = 1; s < b.Width; s++)
			{
				y = job.ymin;
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
			return b;
		}


		//byte[] msgByte = Encoding.ASCII.GetBytes(msg);
    }
	public class Job
	{
		public ushort ID { get; set; }
		//Some var(s) that carry the calculation
		public double xmin { get; set; }
		public double xmax { get; set; }
		public double ymin { get; set; }
		public double ymax { get; set; }
		public int height { get; set; }
		public int width { get; set; }
	}

	public class Piece
	{
		public ushort ID { get; set; }  //same as Job ID  //Can handle up to 16383 total cores in renderfarm
										//TODO: might add offset to enable transfer while going
		public List<Pixel> pixels { get; set; }
	}
	public class Pixel
	{
		public uint Placement { get; set; }         //Int is enough for 1 job to handle 65.000x65.000 picture, no need to go higher really..
		public Color color { get; set; }        //Is it more efficient to use 3 ints as RGB?  //TODO: test this
	}
	public class Worker
	{
		public IPAddress IP { get; set; }
		public ushort Threads { get; set; }     //I guess the program won't incounter a machine with over 32,767 threads ¯\_(ツ)_/¯
		public sbyte Progress { get; set; }     //Only goes from 0-100 (%) so might as well save 8bits..
		public Socket Socket { get; set; }
	}
}
