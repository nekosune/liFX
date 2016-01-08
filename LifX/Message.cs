using System;
using System.Collections.Generic;
using System.Reflection;

namespace LifX
{
	public class Message
	{


		public static Dictionary<int,Type> types = new Dictionary<int,Type> ();
		/// <summary>
		/// Gets or sets the size of the message.
		/// </summary>
		/// <value>The size.</value>
		public int Size{get;set;}

		/// <summary>
		/// Gets or sets the origin [ Must be 0 in current protocol].
		/// </summary>
		/// <value>The origin.</value>
		public int Origin{get;set;}
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="LifX.Message"/> is tagged.
		/// </summary>
		/// <value><c>true</c> if tagged; otherwise, <c>false</c>.</value>
		public bool Tagged{ get; set;}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="LifX.Message"/> is addressable.
		/// </summary>
		/// <value><c>true</c> if addressable; otherwise, <c>false</c>.</value>
		public bool Addressable { get; set;}

		/// <summary>
		/// Gets or sets the protocol.
		/// </summary>
		/// <value>The protocol.</value>
		public int Protocol{ get; set;}

		/// <summary>
		/// Gets or sets the source.
		/// </summary>
		/// <value>The source.</value>
		public int Source{ get; set;}

		/// <summary>
		/// Gets or sets the target device(0 for all).
		/// </summary>
		/// <value>The target.</value>
		public UInt64 Target{ get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="LifX.Message"/> ack required.
		/// </summary>
		/// <value><c>true</c> if ack required; otherwise, <c>false</c>.</value>
		public bool Ack_Required{ get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="LifX.Message"/> res required.
		/// </summary>
		/// <value><c>true</c> if res required; otherwise, <c>false</c>.</value>
		public bool Res_Required{ get; set; }

		/// <summary>
		/// Gets or sets the sequence.
		/// </summary>
		/// <value>The sequence.</value>
		public byte Sequence{ get; set; }
		public virtual int Type{ get; }

		public Message ()
		{
			Origin = 0;
			Addressable = true;
			Protocol = 1024;

		}

		public Message(byte[] bytes)
		{
			Console.Out.WriteLine ("Base construction");
			ushort size = (ushort)BitConverter.ToChar (bytes, 0);
			this.Size = size;
			ushort fragment = (ushort)BitConverter.ToChar (bytes, 2);
			this.Origin = (fragment & 0xC000) >> 14;
			this.Tagged = (fragment & 0x2000) >> 13 == 1;
			this.Addressable = (fragment & 0x1000) >> 12 == 1;
			this.Protocol = fragment & 0xFFF;
			this.Source = BitConverter.ToInt32 (bytes, 4);
			this.Target = BitConverter.ToUInt64 (bytes, 8) ;
			byte acks = bytes[22];
			this.Ack_Required = (acks & 0x2) >> 1 == 1;
			this.Res_Required = (acks & 0x1) == 1;
			this.Sequence = bytes [23];

			Console.Out.WriteLine (Res_Required);
		}

		public byte[] toBytes()
		{
			List<byte> bytes = new List<byte> ();
			bytes.Add (0);
			bytes.Add (0);

			ushort fragment = 0;
			ushort origin = (ushort) (this.Origin & (ushort)0x3);
			origin = (ushort)(origin << (ushort)14);
			fragment = (ushort)(fragment | (ushort)origin);

			if (this.Tagged)
				fragment |= 1 << 13;
			if (this.Addressable)
				fragment |= 1 << 12;
			ushort protocol = (ushort)Protocol;
			protocol = (ushort)(protocol & 0xFFF);
			fragment = (ushort)(fragment | protocol);
			byte[] FragmentBytes = GetBytes (fragment);
			bytes.AddRange (FragmentBytes);
			Int32 source = Source;
			byte[] SourceBytes = GetBytes (source);
			bytes.AddRange (SourceBytes);
			UInt64 target = Target; //<< 16;
			bytes.AddRange (GetBytes(target));
			bytes.AddRange (new byte[]{ 0, 0, 0, 0, 0, 0 });
			byte acks = 0;
			if (Ack_Required)
				acks |= 1 << 1;
			if (Res_Required)
				acks |= 1;
			bytes.Add (acks);
			bytes.Add (Sequence);
			bytes.AddRange (new byte[]{ 0, 0, 0, 0, 0, 0, 0, 0 });
			bytes.AddRange (GetBytes (Type));
			bytes.AddRange (new byte[]{ 0, 0 });

			GeneratePayload (bytes);

			ushort size = (ushort)bytes.Count;
			byte[] sizeByte = GetBytes (size);
			bytes [0] = sizeByte [0];
			bytes [1] = sizeByte [1];

			return bytes.ToArray ();
		}

	
		public virtual void GeneratePayload (List<byte> packet)
		{
		}

		public static Message GetMessage(byte[] bytes)
		{
			int type = BitConverter.ToInt32 (bytes, 32);
			Console.Out.WriteLine(types[type]);
			Type typ = types [type];
			ConstructorInfo c=typ.GetConstructor(new Type[]{typeof(byte[])});

			return (Message)c.Invoke (new Object[]{ bytes });
		}


		public static byte[] GetBytes(ushort input)
		{
			byte[] FragmentBytes = BitConverter.GetBytes (input);
			if (!BitConverter.IsLittleEndian)
				Array.Reverse (FragmentBytes);
			return FragmentBytes;
		}
		public static byte[] GetBytes(Int32 input)
		{
			byte[] FragmentBytes = BitConverter.GetBytes (input);
			if (!BitConverter.IsLittleEndian)
				Array.Reverse (FragmentBytes);
			return FragmentBytes;
		}

		public static byte[] GetBytes(UInt64 input)
		{
			byte[] FragmentBytes = BitConverter.GetBytes (input);
			if (!BitConverter.IsLittleEndian)
				Array.Reverse (FragmentBytes);
			return FragmentBytes;
		}


		public static void Initialize()
		{
			types.Add (2, typeof(GetServiceMessage));
			types.Add (3, typeof(StateServiceMessage));
		}

		public override string ToString ()
		{
			return string.Format ("[Message: Size={0}, Origin={1}, Tagged={2}, Addressable={3}, Protocol={4}, Source={5}, Target={6}, Ack_Required={7}, Res_Required={8}, Sequence={9}, Type={10}]", Size, Origin, Tagged, Addressable, Protocol, Source, Target, Ack_Required, Res_Required, Sequence, Type);
		}
	}
}

