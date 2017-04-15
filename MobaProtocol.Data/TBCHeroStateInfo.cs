using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class TBCHeroStateInfo
	{
		[ProtoMember(1)]
		public string HeroId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string Life
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string MP
		{
			get;
			set;
		}
	}
}
