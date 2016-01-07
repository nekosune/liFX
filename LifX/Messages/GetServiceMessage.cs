using System;

namespace LifX
{
	public class GetServiceMessage: Message
	{
		public override int Type {
			get {
				return 2;
			}
		}
		public GetServiceMessage ()
		{
			this.Tagged = true;
		}


		public GetServiceMessage(byte[] bytes):base(bytes)
		{
			Console.Out.WriteLine ("I construct a GetServiceMessage!");
		}
	}
}

