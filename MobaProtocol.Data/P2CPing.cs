using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CPing
	{
		[ProtoMember(1)]
		public long clientTime
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long serverTime
		{
			get;
			set;
		}
	}
}
