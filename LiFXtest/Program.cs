using System;
using LifX;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace LiFXtest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			Message.Initialize ();
			GetServiceMessage message = new GetServiceMessage ();
			Console.WriteLine(string.Join(",", message.toBytes ()));
			UdpClient client=new UdpClient();
			IPEndPoint ip = new IPEndPoint (IPAddress.Broadcast, 56700);
			byte[] bytes = message.toBytes ();
			client.Send (bytes,bytes.Length,  ip);
			client.Close ();
			TempReciever tmp = new TempReciever ();
			tmp.StartListening ();
			while (true) {
				Thread.Sleep (10);
			}

		}
	}
}
