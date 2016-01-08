using System;
using System.Net.Sockets;
using System.Net;

namespace LifX
{
	public class LightBulb
	{
		public ulong MacAddress{ get; set;}
		public IPAddress CurrentIP{ get; set;}
		/// <summary>
		/// Gets or sets a value indicating whether this instance is active.
		/// </summary>
		/// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
		public bool IsActive{ get; set; }
		public LightBulb()
		{
			
		}
	}
}

