using System.Net;
using System.Net.Sockets;

namespace Core.Models
{
    public class Worker
    {
        public IPAddress Ip { get; set; }
        public ushort Threads { get; set; }  	//I guess the program won't encounter a machine with over 65,535 threads ¯\_(ツ)_/¯
        public sbyte Progress { get; set; } 	//Only goes from 0-100 (%) so might as well save 8bits..
        public Socket Socket { get; set; }
        
    }
}