using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FractalClient
{
    class Program
    {
		static IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 9001);
		static Socket host = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static void Main(string[] args)
        {
			Console.WriteLine("This is the render client program");
			Console.WriteLine("Please wait make sure the render host is running");
			Console.WriteLine("Trying to connect..");
			Connect();
		}
		static void Connect() {
			host.Bind(localEndPoint);
			host.Listen(100);
			host = host.Accept();
		}
		static void FirstContact() {
			string msg = "";
			byte[] msgbyte = Encoding.ASCII.GetBytes(msg);
			host.Send(msgbyte);
		}
		//byte[] msgByte = Encoding.ASCII.GetBytes(msg);
    }
}
