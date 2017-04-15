using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CKillInfo
	{
		[ProtoMember(1)]
		public int attackerId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int targetId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public long relivetime
		{
			get;
			set;
		}
	}
}
