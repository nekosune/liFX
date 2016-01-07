using System;

namespace LifX
{
	public class StateServiceMessage:Message
	{
		public byte Service{ get; set;}
		public int Port{get;set;}
		public StateServiceMessage (byte[] bytes):base(bytes)
		{
			Service = bytes [36];
			Port = BitConverter.ToInt32 (bytes, 37);

		}
	}
}

