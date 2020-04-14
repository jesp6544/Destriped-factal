public class Worker
	{
		public IPAddress IP { get; set; }
		public ushort Threads { get; set; }     //I guess the program won't incounter a machine with over 65,535 threads ¯\_(ツ)_/¯
		public byte Progress { get; set; }     //Only goes from 0-100 (%) so might as well save 8bits..
		public Socket Socket { get; set; }
	}