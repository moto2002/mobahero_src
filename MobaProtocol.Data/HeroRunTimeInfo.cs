using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class HeroRunTimeInfo
	{
		[ProtoMember(1)]
		public int money;

		[ProtoMember(2)]
		public int skillPoint;

		[ProtoMember(3)]
		public int exp;

		[ProtoMember(4)]
		public Dictionary<string, SkillDynData> skillInfo;

		[ProtoMember(5)]
		public List<ItemDynData> itemInfo;

		[ProtoMember(6)]
		public int totalMoney;

		[ProtoMember(7)]
		public Dictionary<string, string> playerVar;
	}
}
