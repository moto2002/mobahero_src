using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class GateCastData
	{
		[ProtoMember(1)]
		public byte gateCastType;

		[ProtoMember(2)]
		public List<long> target;

		[ProtoMember(3)]
		public byte code;

		[ProtoMember(4)]
		public byte[] data;

		[ProtoMember(5)]
		public byte channel;
	}
}
