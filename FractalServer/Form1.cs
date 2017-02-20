using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using System.Drawing.Imaging;

namespace FractalServer
{
	public partial class Form1 : Form
	{
		IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 9001);
		public List<Worker> RenderFarm = new List<Worker>();
		public List<Job> JobList = new List<Job>();
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		public Thread ClientThread;
		public double xmin = -2.1;
		public double ymin = -1.3;
		public double xmax = 1;
		public double ymax = 1.3;
		public bool running = false;

		public Form1()
		{
			InitializeComponent();
			Thread ConnectThread = new Thread(Connect);
			ConnectThread.Start();
		}

		private void ConnectBtn_Click(object sender, EventArgs e)
		{
			try
			{
				//Connect();
			}
			catch (Exception ex)
			{
				//MessageBox.Show(ex.Message + "");
				MessageBox.Show(@"Could not connect to IP");
			}
		}

		public void Connect()
		{
			socket.Bind(localEndPoint);
			socket.Listen(100);
			while (true)
			{
				Socket clientSocket = socket.Accept();
				Console.WriteLine("A new client connected");
				byte[] bytes = new byte[1024];
				clientSocket.Receive(bytes);

				Worker temp = new Worker()
				{
					IP = localEndPoint.Address, //FIXME: Not the right address
					Socket = clientSocket,
					Threads = Convert.ToUInt16(Encoding.ASCII.GetString(bytes))
				};
				RenderFarm.Add(temp);
				ClientThread = new Thread(() =>
				{
					Resive(temp);
				});
				ClientThread.Start();
				ClientDatagridview.Rows.Add(temp.IP.ToString(), temp.Threads.ToString());
				ClientDatagridview.Update();
			}
		}

		public void Send(Worker worker) //Call this for each worker that needs a job
		{
			Job job = GetJob();
			if (job != null)
			{
				string jobString = JsonConvert.SerializeObject(job);    //Converts the job Object to a json string
				Byte[] jobByte = Encoding.ASCII.GetBytes(jobString);    //Converts that string to bytes
				worker.Socket.Send(jobByte);                            //Sends those bytes to worker
			}
			else
			{
				running = false;
			}   //sets running to false if there is no more jobs
		}

		public Job GetJob()
		{
			if (JobList.Count > 0)  //Yes this should be done with a queue..
			{
				Job job = JobList.First();
				JobList.Remove(job);
				return job;
			}
			else
			{
				return null;
			}
		}

		public void SetUpJobs()
		{
			running = true;
			Job job = new Job();
			double halfHeight = ymax - (ymax - ymin) / 2;
			double jobXLenth = (xmax - xmin) / RenderFarm.Count;
			for (ushort i = 0; i < RenderFarm.Count * 2; i++)
			{
				double tmp;
				if (i > RenderFarm.Count) { tmp = halfHeight; }
				else { tmp = ymin; }
				job.ID = i;
				job.ymin = tmp;
				if (i < RenderFarm.Count) { tmp = halfHeight; }
				else { tmp = ymax; }
				job.ymax = tmp;
				job.xmax = xmin + (i % RenderFarm.Count) * jobXLenth; //FIXME Fixed?
				job.xmin = xmin + (i - 1) * jobXLenth % RenderFarm.Count;
				job.height = factalPictureBox.Height / 2;
				job.width = factalPictureBox.Width / RenderFarm.Count;

				JobList.Add(job);
			}
		}

		public void Resive(Worker worker)
		{
			try
			{

				while (true)
				{
					byte[] bytes = new Byte[8192];
					string pieceString = "";
					while (worker.Socket.Receive(bytes) > 0)
					{
						pieceString += Encoding.ASCII.GetString(bytes);
					}

					//string pieceString = Encoding.ASCII.GetString(bytes);
					Piece piece = JsonConvert.DeserializeObject<Piece>(pieceString);
					Bitmap bitmap = new Bitmap(10, 10);//factalPictureBox.Width, factalPictureBox.Height);
													   //for (int i = 0; i < 10; i++)
													   //{
													   //	for (int j = 0; j < 10; j++)
													   //	{
													   //		Color color = piece.pixels[(i  * 10) + j].color;
													   //		bitmap.SetPixel(j, i, color);
													   //	}
													   //}
					foreach (var i in piece.pixels)
					{
						int y = Convert.ToInt32(i.Placement) / 10;//factalPictureBox.Width;
						int x = Convert.ToInt32(i.Placement) - 10 * y;//factalPictureBox.Width * y;
						bitmap.SetPixel(x, y, Color.FromArgb(i.color, 0, 0));
					}

					factalPictureBox.Image = bitmap;
					factalPictureBox.Update();
					bitmap.Save("myfilebla.png", ImageFormat.Png);

					//}
					if (running == true)
					{   //If there still are jobs to be done, send the worker a new one
						Send(worker);
					}
				}
			}
			catch (Exception)
			{
				//TODO: Make a popup that askes to remove the tread or retry
				Thread.CurrentThread.Abort();
			}
		}

		private void RenderBtn_Click(object sender, EventArgs e)
		{
			//SetUpJobs();
			Job bla = new Job()
			{
				ID = 1,
				ymin = ymin,
				ymax = ymax,
				xmax = xmax,
				xmin = xmin,
				height = factalPictureBox.Height,
				width = factalPictureBox.Width
			};
			factalPictureBox.Image = Render(bla);
			factalPictureBox.Update();
		}
		public Bitmap Render(Job job)
		{
			// Holds all of the possible colors
			Color[] cs = new Color[256]; //Not used as of this time

			// Creates the Bitmap we draw to
			Bitmap b = new Bitmap(job.width, job.height);
			// From here on out is just converted from the c++ version.
			double x, y, x1, y1, xx, intigralX, intigralY = 0.0;

			int looper, s, z = 0;
			intigralX = (job.xmax - job.xmin) / b.Width; // Make it fill the whole window
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

		private void factalPictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			double temp = (xmax - xmin) / 4;
			double tempX = xmin + (xmax - xmin) * (Convert.ToDouble(e.X) / factalPictureBox.Width);
			double tempY = ymin + (ymax - ymin) * (Convert.ToDouble(e.Y) / factalPictureBox.Height);
			xmin = tempX - temp;
			xmax = tempX + temp;
			ymin = tempY - temp;
			ymax = tempY + temp;
			Job job = new Job();
			/*
			if (false)  //This piece of code is for makin the jobs for rendering and it not done at this point in time. 
			{
				double halfHeight = ymax - (ymax - ymin) / 2;
				double jobXLenth = temp * 4 / RenderFarm.Count;
				for (ushort i = 0; i < RenderFarm.Count * 2; i++)
				{  
					double tmp;
					if (i > RenderFarm.Count) { tmp = halfHeight; }
					else { tmp = ymin; }
					job.ID = i;
					job.ymin = tmp;
					job.ymax = ymax;
					job.xmax = xmin + i * jobXLenth % RenderFarm.Count;
					job.xmin = xmin + (i - 1) * jobXLenth % RenderFarm.Count;
					job.height = factalPictureBox.Height / 2;
					job.width = factalPictureBox.Width / RenderFarm.Count;
				}
			}
			*/
			if (true)
			{
				job.ymin = ymin;
				job.ymax = ymax;
				job.xmax = xmax;
				job.xmin = xmin;
				job.height = factalPictureBox.Height;
				job.width = factalPictureBox.Width;
			}
			factalPictureBox.Image = Render(job);
			factalPictureBox.Update();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			xmin = -2.1;
			ymin = -1.3;
			xmax = 1.0;
			ymax = 1.3;
		}

		private void SaveBtn_Click(object sender, EventArgs e)
		{
			SetUpJobs();
			foreach (var i in RenderFarm)
			{
				Send(i);
			}

			//int kSize = 100;  //This is the number of vertical and horizontal pixels
			//Job bla = new Job()
			//{
			//	ID = 2,
			//	ymin = ymin,
			//	ymax = ymax,
			//	xmax = xmax,
			//	xmin = xmin,
			//	height = kSize,
			//	width = kSize
			//};
			//Bitmap bit = Render(bla);	//Renders the picture as a bitmap
			//bit.Save("myfile2.bmp", ImageFormat.Bmp);	//saves the bit bitmap as a .Bmp

		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}
	}

	public class Job
	{
		public ushort ID { get; set; }
		//Some vars that carry the calculation and size
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
		public List<Pixel> pixels { get; set; }
		public bool done { get; set; }  //Used to send to the host if this is the last part of the piece
	}
	public class Pixel
	{
		public uint Placement { get; set; }         //Int is enough for 1 job to handle 65.000x65.000 picture, no need to go higher really..
		public byte color { get; set; }        //0-255 colors, might make this better in the furture
	}
	public class Worker
	{
		public IPAddress IP { get; set; }
		public ushort Threads { get; set; }     //I guess the program won't incounter a machine with over 65,535 threads ¯\_(ツ)_/¯
		public byte Progress { get; set; }     //Only goes from 0-100 (%) so might as well save 8bits..
		public Socket Socket { get; set; }
	}

}
