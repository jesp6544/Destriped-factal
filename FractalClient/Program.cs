using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json;
using System.Drawing.Imaging;

namespace FractalClient
{
	class Program
	{
		static Socket host = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		static Piece piece = new Piece();
		static bool verbose = true;
		static Byte[] byteArray = new byte[8192];
		static Job GlobalJob = new Job();

		static void Main(string[] args)
		{
			string DefaultIp = "127.0.0.1"; //local host
			Console.WriteLine("This is the render client program");
			Console.WriteLine("Number Of Logical Processors: {0}", Environment.ProcessorCount); //Number og CPU threads.
			Console.WriteLine("Please write host IP, or [Enter] for default ({0}): ", DefaultIp);
			IPAddress goodIP;
			string ip = Console.ReadLine();
			if (ip == "")
			{
				ip = DefaultIp;
			}
			if (IPAddress.TryParse(ip, out goodIP))
			{
				try
				{
					Connect(goodIP);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Closed");  //Used for debugging to avoid crash every time the host shuts down
				}

			}
		}
		static void Connect(IPAddress goodIP)
		{
			host.Connect(goodIP, 9001);
			Console.WriteLine("Connect! ");
			FirstContact();
		}

		static void FirstContact()
		{
			Console.WriteLine("FirstContact");
			string msg = Environment.ProcessorCount.ToString();
			byte[] msgbyte = Encoding.ASCII.GetBytes(msg);
			host.Send(msgbyte);
			Thread ConnectThread = new Thread(Resive);
			ConnectThread.Start();
		}

		static public void Resive()
		{
			Console.WriteLine("Listening");
			byte[] bytes = new byte[1024];
			while (host.Connected)
			{
				host.Receive(bytes);
				Console.WriteLine("Got a job");
				string jobString = Encoding.ASCII.GetString(bytes);
				GlobalJob = JsonConvert.DeserializeObject<Job>(jobString);
				if (verbose)
				{
					Console.WriteLine("value: {0} - {1},", "xmin", GlobalJob.xmin);
					Console.WriteLine("value: {0} - {1},", "xmax", GlobalJob.xmax);
					Console.WriteLine("value: {0} - {1},", "ymin", GlobalJob.ymin);
					Console.WriteLine("value: {0} - {1},", "ymax", GlobalJob.ymax);
					Console.WriteLine("value: {0} - {1},", "height", GlobalJob.height);
					Console.WriteLine("value: {0} - {1},", "weidth", GlobalJob.width);
				}
				Render(GlobalJob);
			}
		}

		static void DoWork()
		{
			Console.WriteLine("Started working ");
			Bitmap temp = Render(GlobalJob);
			//temp.Save("myfile2.png", ImageFormat.Png);
			//Console.WriteLine(piece.ID);
			//GenerateHeader(false, 0);

			//byte[] tmp = BitConverter.GetBytes(job.ID);
			//Array.Copy(tmp, 0, byteArray, 0, tmp.Length);
			//byteArray[0] = (byte)job.ID;
			//piece.ID = job.ID;
			//Console.WriteLine(piece.ID);
			//piece.xLenth = job.width;

			//Sending(temp);
		}

		static void GenerateHeader(bool done, int placement)

		{
			Console.WriteLine("Started working on header");
			int offset = 0;                                                     //The offset will be used to know where to input the next byte of data
			byte[] byteID = BitConverter.GetBytes(GlobalJob.ID);            //2B /The ID will be used by the client to know where to place this box of pixels
			byte[] byteXLenth = BitConverter.GetBytes(GlobalJob.width);     //4B /The lenth along the x axis is used to know what to divide by to get x,y
			byte[] byteRunning = BitConverter.GetBytes(done);               //1B /The running byte is simply if the job is still running
			byte[] bytePlacement = BitConverter.GetBytes(placement);        //4B /The plancement Is used to indicate the placement of the first pixel in this package, enables sending more recreatable pixels arrays without x and y on every one

			Array.Copy(byteID, 0, byteArray, offset, byteID.Length);
			offset += byteID.Length;
			Array.Copy(byteXLenth, 0, byteArray, offset, byteXLenth.Length);
			offset += byteXLenth.Length;
			Array.Copy(byteRunning, 0, byteArray, offset, byteRunning.Length);
			offset += byteRunning.Length;
			Array.Copy(bytePlacement, 0, byteArray, offset, bytePlacement.Length);
			offset += bytePlacement.Length;
			Console.WriteLine("Done with header");
			// ^ 11 Bytes
			//Start payload at 15Bytes, provides room for inprovements
		}

		static void SendPart()
		{
			string pieceString = JsonConvert.SerializeObject(piece);    //Converts the job Object to json (string)
			byte[] pieceByte = Encoding.ASCII.GetBytes(@pieceString);    //Converts that string to bytes
			if (verbose)
			{
				Console.WriteLine(Encoding.ASCII.GetString(pieceByte));
			}

			//Console.WriteLine("sending piece");
			host.Send(pieceByte);
			//Console.WriteLine("Done sending piece");
		}

		static void Sending(Bitmap img)
		{
			Console.WriteLine("Started sending");

			int width = img.Width;
			List<Pixel> test = new List<Pixel>();
			for (int i = 0; i < img.Height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					Pixel pix = new Pixel();
					pix.Placement = (uint)((width * i) + j);
					pix.color = img.GetPixel(j, i).R;
					if (verbose)
					{
						Console.WriteLine("Now adding pix to piece" + Environment.NewLine +
										  "Pix Placement: {0}", pix.Placement + Environment.NewLine +
										  "Pix Color{0}", pix.color + Environment.NewLine +
										  "Pixel Color {0}", img.GetPixel(j, i));
					}
					test.Add(pix);
					if (test.Count == 200) //Or ((i * img.Height + j) % 2000 == 0) TODO test what is faster
					{
						Console.WriteLine("Now sending a part");
						//piece.pixels = null;

						piece.pixels = test;

						piece.done = false;
						Console.WriteLine("Test start");
						//Thread.Sleep(100);
						SendPart();
						test.Clear();
						Console.WriteLine("Test slut");
					}
				}
			}
			//Now sending rest if there are any more, and sending the 'done' bool
			if (test.Count > 0)
			{
				Console.WriteLine("Now ending send");
				piece.pixels = test;
			}
			else
			{
				piece.pixels = null;
			}
			piece.done = true;
			SendPart();

			//string pieceString = JsonConvert.SerializeObject(piece);	//Converts the job Object to a json string
			//byte[] pieceByte = Encoding.ASCII.GetBytes(@pieceString); //Converts that string to bytes
			//if (verbose)
			//{
			//	Console.WriteLine(Encoding.ASCII.GetString(pieceByte));
			//}
			//Console.WriteLine("sending piece");
			//host.Send(pieceByte);
			//Console.WriteLine("Done sending piece");
			//piece = null;
		}

		static public Bitmap Render(Job job)
		{
			int offset = 15;    //start offset for payload
			int lastSend = 0;
			//byte[] bytePixelArray = new byte[8192];
			Console.WriteLine("Started render");
			// Holds all of the possible colors
			//Color[] cs = new Color[256];

			// Creates the Bitmap we draw to
			Bitmap b = new Bitmap(job.width, job.height);
			double x, y, x1, y1, xx, intigralX, intigralY = 0.0;

			int looper = 0;
			intigralX = (job.xmax - job.xmin) / b.Width;
			intigralY = (job.ymax - job.ymin) / b.Height;
			x = job.xmin;

			for (int s = 1; s < b.Width; s++)
			{
				y = job.ymin;
				for (int z = 1; z < b.Height; z++)
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
					byte byteVal = (byte)(perc * 255);
					// Use that number to set the color
					Color color = Color.FromArgb(val, 0, 0);

					//Enable send from here?
					byte[] byteColor = BitConverter.GetBytes(byteVal);
					Array.Copy(byteColor, 0, byteArray, offset, byteColor.Length);
					offset += byteColor.Length;     //TODO benchmark if a hardcoded value is faster
					if (offset > 8190)
					{
						Console.WriteLine("Buffer is now full, sending");
						GenerateHeader(false, lastSend);
						offset = 15;
						printShit(byteArray);
						host.Send(byteArray);
						//byteArray = null;
						lastSend = ((s - 1) * b.Width) + z;
					}
					//b.SetPixel(s, z, color);
					y += intigralY;
				}
				x += intigralX;
			}
			Console.WriteLine("Performing last send");
			GenerateHeader(true, lastSend);
			host.Send(byteArray);
			Console.WriteLine("Done render");
			return b;
		}
		public static void printShit(byte[] bytes)
		{
			ushort temp1 = BitConverter.ToUInt16(bytes, 0);
			int temp2 = BitConverter.ToInt32(bytes, 2);
			bool temp3 = BitConverter.ToBoolean(bytes, 6);
			int placement = BitConverter.ToInt32(bytes, 7);
			Console.WriteLine(temp1 + Environment.NewLine +
							  temp2 + Environment.NewLine +
							  temp3 + Environment.NewLine +
							  placement);
		}
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
		public bool done { get; set; }
		public int xLenth { get; set; }
	}
	public class Pixel
	{
		public uint Placement { get; set; }         //Int is enough for 1 job to handle 65.000x65.000 picture, no need to go higher really..
		public byte color { get; set; }        //Is it more efficient to use 3 ints as RGB?  //TODO: test this
	}
	public class Worker
	{
		public IPAddress IP { get; set; }
		public ushort Threads { get; set; }     //I guess the program won't incounter a machine with over 32,767 threads ¯\_(ツ)_/¯
		public sbyte Progress { get; set; }     //Only goes from 0-100 (%) so might as well save 8bits..
		public Socket Socket { get; set; }
	}
}
