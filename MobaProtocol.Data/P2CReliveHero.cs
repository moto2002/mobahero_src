using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CReliveHero
	{
		[ProtoMember(1)]
		public int unitId
		{
			get;
			set;
		}
	}
}
