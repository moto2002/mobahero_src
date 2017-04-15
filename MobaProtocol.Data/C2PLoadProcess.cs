using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class C2PLoadProcess
	{
		[ProtoMember(1)]
		public int newUid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public byte process
		{
			get;
			set;
		}
	}
}
