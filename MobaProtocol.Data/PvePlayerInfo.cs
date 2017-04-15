using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class PvePlayerInfo
	{
		[ProtoMember(1)]
		public int newUid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string uid
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string userName
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public HeroInfo heroInfo
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public string selfDefSkillId
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public string heroSkinId
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public List<TalentModel> talentInfo
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int group
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int teamPos
		{
			get;
			set;
		}
	}
}
