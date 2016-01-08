using System;
using System.Net.Sockets;
using System.Net;
using LifX;

namespace LiFXtest
{
	public class TempReciever
	{
		private readonly UdpClient udp=new UdpClient(56700);
		public TempReciever ()
		{
		}

		public void StartListening()
		{
			this.udp.BeginReceive (Receive, new object ());
		}

		private void Receive(IAsyncResult ar)
		{
			IPEndPoint ip = new IPEndPoint (IPAddress.Any, 56700);
			byte[] bytes = udp.EndReceive (ar, ref ip);
			Console.WriteLine("Returned: {0}",string.Join(",", bytes));
			StateServiceMessage message = (StateServiceMessage) Message.GetMessage (bytes);
			Console.Out.WriteLine ("{0}", message.ToString ());
			Console.Out.WriteLine ("IP: {0}", ip.Address.ToString());

		}
	}
}

