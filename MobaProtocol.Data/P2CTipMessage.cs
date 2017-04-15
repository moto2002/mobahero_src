using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CTipMessage
	{
		[ProtoMember(1)]
		public byte type
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string msg
		{
			get;
			set;
		}
	}
}
