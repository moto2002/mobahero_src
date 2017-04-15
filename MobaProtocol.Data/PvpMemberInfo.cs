using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class PvpMemberInfo
	{
		[ProtoMember(1)]
		public string accountId;

		[ProtoMember(2)]
		public string userId;

		[ProtoMember(3)]
		public string userName;

		[ProtoMember(4)]
		public int Level
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int Icon
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int PictureFrame
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public long UnionId
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public string UnionName
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int FightPower
		{
			get
			{
				return (int)this.LadderScore;
			}
			set
			{
				this.LadderScore = (double)value;
			}
		}

		[ProtoMember(10)]
		public double LadderScore
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public long SummerId
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public List<TalentModel> TalentInfo
		{
			get;
			set;
		}

		[ProtoMember(13)]
		public string serverkey
		{
			get;
			set;
		}

		[ProtoMember(14)]
		public List<HeroInfo> heroinfolist
		{
			get;
			set;
		}

		[ProtoMember(15)]
		public int battleFightNum
		{
			get;
			set;
		}

		[ProtoMember(16)]
		public PvpJoinType jointype
		{
			get;
			set;
		}

		[ProtoMember(17)]
		public int inneraccount
		{
			get;
			set;
		}

		[ProtoMember(18)]
		public bool isInRoom
		{
			get;
			set;
		}

		[ProtoMember(19)]
		public List<HopeRealityValue> hopeRealityValue
		{
			get;
			set;
		}

		[ProtoMember(20)]
		public List<pvpScoreData> newPvpScore
		{
			get;
			set;
		}

		[ProtoMember(21)]
		public List<string> freeHeros
		{
			get;
			set;
		}

		[ProtoMember(22)]
		public int randomvalue
		{
			get;
			set;
		}

		[ProtoMember(23)]
		public string summSkillSave
		{
			get;
			set;
		}

		[ProtoMember(24)]
		public bool IsDeactiveDuringFight
		{
			get;
			set;
		}

		[ProtoMember(25)]
		public int rankFrame
		{
			get;
			set;
		}

		[ProtoMember(26)]
		public int CharmRankvalue
		{
			get;
			set;
		}
	}
}
