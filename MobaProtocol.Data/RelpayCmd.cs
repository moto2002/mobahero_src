using ProtoBuf;
using System;
using System.Collections;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class RelpayCmd
	{
		[ProtoMember(1)]
		public byte code
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long time
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public long frame
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public byte[] paramListBytes
		{
			get;
			set;
		}

		public ArrayList paramList
		{
			get
			{
				ByteBuffer byteBuffer = new ByteBuffer(this.paramListBytes, 0, this.paramListBytes.Length);
				return byteBuffer.ReadArray();
			}
			set
			{
				ByteBuffer byteBuffer = new ByteBuffer(true);
				byteBuffer.Write(value);
				this.paramListBytes = byteBuffer.ToBytes();
			}
		}
	}
}
