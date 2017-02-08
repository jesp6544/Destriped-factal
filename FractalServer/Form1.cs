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
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public Thread ClientThread;
		public double xmin = -2.1;
		public double ymin = -1.3;
		public double xmax = 1;
		public double ymax = 1.3;

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
                Connect();
            }
            catch (Exception ex)
            {
				MessageBox.Show(ex.Message + "");
                MessageBox.Show(@"Could not connect to IP");
            }
        }

		public void Connect()
		{
			socket.Bind(localEndPoint);
			socket.Listen(100);
			//IPAddress remoteIpAddress;
			while (true)
			{
				Socket clientSocket = socket.Accept();
				Console.WriteLine("A new client connected");
				byte[] bytes = new byte[1024];
				clientSocket.Receive(bytes);

				Worker temp = new Worker()
				{
					IP = localEndPoint.Address, //Not the right address
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
			}
		
                
        }

        public void Send()
        {
            foreach (Worker worker in RenderFarm)
            {
                //TODO: make a method that returns the next job
                Job job = new Job();
                string jobString = JsonConvert.SerializeObject(job);
                Byte[] jobByte = Encoding.ASCII.GetBytes(jobString);
                worker.Socket.Send(jobByte);
            }
        }

        public void Resive(Worker worker)
        {
            try
            {
                byte[] bytes = new byte[1024];
                while (true)
                {
                    worker.Socket.Receive(bytes);
                    string pieceString = Encoding.ASCII.GetString(bytes);
                    Piece piece = JsonConvert.DeserializeObject<Piece>(pieceString);
                    //foreach (var i in ClientDatagridview)
                    //{
                        
                    //}
                    int tempNum = piece.ID*(1000);

                }
            }
            catch (Exception)
            {
                //TODO: make a popup that askes to remove the tread or retry
                Thread.CurrentThread.Abort();
            }
        }

        private void RenderBtn_Click(object sender, EventArgs e)
        {
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
			//MessageBox.Show(xmin + "\n" + xmax + "\n" + ymin + "\n" + ymax + "\n");
			// Holds all of the possible colors
  			Color[] cs = new Color[256];

			//var temp = (xmax - ymax)/2;
			// Creates the Bitmap we draw to
			Bitmap b = new Bitmap(job.width, job.height);
			// From here on out is just converted from the c++ version.
			double x, y, x1, y1, xx, intigralX, intigralY = 0.0;

			int looper, s, z = 0;
		    //xmin = -2.1;//Sx; // Start x value, normally -2.1
		    //ymin = -1.5;//Sy; // Start y value, normally -1.3
		    //xmax = 0.9;//Fx; // Finish x value, normally 1
		    //ymax = 1.5;//Fy; // Finish y value, normally 1.3
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

			//this.BackgroundImage = (Image)bq; // Draw it to the form
    }

        private void factalPictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
			//MessageBox.Show(e.Y +"");
			double temp = (xmax - xmin) / 4;
			//MessageBox.Show("(" + xmax + "-" + xmin + ")" + "*" + "(" + e.X + "/"+ factalPictureBox.Width +")"+ "=" + (xmax - xmin) * (e.X / factalPictureBox.Width) + "");
			//MessageBox.Show(e.X / 500 + "");
			//MessageBox.Show(e.X + "")
			double tempX = xmin + (xmax - xmin) * (Convert.ToDouble(e.X) / factalPictureBox.Width);
			double tempY = ymin + (ymax - ymin) * (Convert.ToDouble(e.Y) / factalPictureBox.Height);
			xmin = tempX - temp;
			xmax = tempX + temp;
			ymin = tempY - temp;
			ymax = tempY + temp;
			Job job = new Job();
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
			factalPictureBox.Image = Render(job);
			factalPictureBox.Update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        xmin = -2.1;
        ymin = -1.5;
        xmax = 0.9;
        ymax = 1.5;
    }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
			Job bla = new Job()
			{
				ID = 2,
				ymin = ymin,
				ymax = ymax,
				xmax = xmax,
				xmin = xmin,
				height = 1200,
				width = 1200
			};
			Render(bla).Save("myfile2.png", ImageFormat.Png);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
    }
    public class Pixel
    {
        public uint Placement { get; set; } 		//Int is enough for 1 job to handle 65.000x65.000 picture, no need to go higher really..
        public Color color { get; set; }  		//Is it more efficient to use 3 ints as RGB?  //TODO: test this
    }
    public class Worker
    {
        public IPAddress IP { get; set; }
        public ushort Threads { get; set; }  	//I guess the program won't incounter a machine with over 32,767 threads ¯\_(ツ)_/¯
		public sbyte Progress { get; set; } 	//Only goes from 0-100 (%) so might as well save 8bits..
        public Socket Socket { get; set; }
    }

}
