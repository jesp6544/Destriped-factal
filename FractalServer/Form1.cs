﻿using System;
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
		public List<Thread> ThreadList = new List<Thread>();
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		public Thread ClientThread;
		public double xmin = -2.1;
		public double ymin = -1.3;
		public double xmax = 1;
		public double ymax = 1.3;
		public bool running = false;
		public Bitmap tempBitmap;
		public readonly _logic;

		public Form1()
		{
			InitializeComponent();
			Thread ConnectThread = new Thread(Connect);
			ConnectThread.Start();
			ThreadList.Add(ConnectThread);
			_logic = new Logic();
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
			_logic.Connect();
		}

		public void Send(Worker worker) //Call this for each worker that needs a job
		{
			_logic.Send(worker);
			
		}

		public Job GetJob()
		{
			_logic.GetJob();
		}

		public void SetUpJobs()
		{
			_logic.SetUpJobs();
		}
		
		public void RenderThis(Piece piece, int lastPlacement, byte[] bytes)
		{
			_logic.RenderThis(piece, lastPlacemnt, bytes)
		}

		public byte[] CopyPartOfArray(int arraySize, int startCopyAt, byte[] fromArray)
		{
			byte[] byteTemp = new byte[arraySize];
			for (int a = 0; a < arraySize; a++)
			{
				byteTemp[a] = fromArray[startCopyAt + a];
			}
			return byteTemp;
		}

		public void Resive(Worker worker)
		{
			try
			{
				while (true)
				{
					byte[] bytes = new byte[8192];
					//string pieceString = "";
					//while (worker.Socket.Receive(bytes) > 0)  //This might not be needed anymore, the client now sends every time a buffer is full
					//{
					//	pieceString += Encoding.ASCII.GetString(bytes);
					//}
					worker.Socket.Receive(bytes);


					//string pieceString = Encoding.ASCII.GetString(bytes);
					//Piece piece = JsonConvert.DeserializeObject<Piece>(pieceString);
					//ClientThread = new Thread(() =>
					//{
					//	FormPiece(bytes);
					//});
					//ClientThread.Start();
					Piece piece = new Piece();
					piece.ID = BitConverter.ToUInt16(bytes, 0);
					piece.xLenth = BitConverter.ToInt32(bytes, 2);
					piece.done = BitConverter.ToBoolean(bytes, 6);
					int placement = BitConverter.ToInt32(bytes, 7);
					RenderThis(piece, placement, bytes);
					//int y;
					//int x;
					//foreach (var i in piece.pixels)
					//{
					//	if (piece.ID < RenderFarm.Count)
					//	{
					//		y = Convert.ToInt32(i.Placement) / piece.xLenth;
					//		x = (piece.xLenth * piece.ID) + Convert.ToInt32(i.Placement) % piece.xLenth;
					//	}
					//	else
					//	{
					//		y = factalPictureBox.Height / 2 + Convert.ToInt32(i.Placement) / piece.xLenth;
					//		x = (piece.xLenth * Convert.ToInt32(piece.ID / 2)) + Convert.ToInt32(i.Placement) % piece.xLenth;
					//	}
					//	//int x = (piece.xLenth * piece.ID) + Convert.ToInt32(i.Placement) % piece.xLenth;
					//	try
					//	{
					//		tempBitmap.SetPixel(x, y, Color.FromArgb(i.color, 0, 0));
					//	}
					//	catch (Exception ex)
					//	{
					//		MessageBox.Show(ex.Message + "");
					//	}

					//}

					//factalPictureBox.Image = tempBitmap;
					//factalPictureBox.Update();
					//tempBitmap.Save("myfilebla.png", ImageFormat.Png);

					//if (piece.done == true)
					//{   //If there still are jobs to be done, send the worker a new one
					//	Send(worker);
					//}
					bytes = null;
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
			//Color[] cs = new Color[256]; //Not used as of this time

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
			factalPictureBox.Image = tempBitmap;
			factalPictureBox.Update();
			//xmin = -2.1;
			//ymin = -1.3;
			//xmax = 1.0;
			//ymax = 1.3;
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
			tempBitmap = new Bitmap(
				Height = factalPictureBox.Height,
				Width = factalPictureBox.Width);
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			foreach (var iThread in ThreadList)
			{
				iThread.Abort();
			}
		}
	}
}
