using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class NewbieReqSpawnHeroData
	{
		[ProtoMember(1)]
		public byte isSelfTeam
		{
			get;
			set;
		}
	}
}
