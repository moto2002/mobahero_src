using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class NewbieRespSpawnHeroData
	{
		[ProtoMember(1)]
		public int heroUniqueId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public byte groupType
		{
			get;
			set;
		}
	}
}
