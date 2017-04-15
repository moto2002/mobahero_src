using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class CallArgsData
	{
		[ProtoMember(1)]
		public byte code;

		[ProtoMember(2)]
		public byte[] args;

		[ProtoMember(3)]
		public string uid;
	}
}
