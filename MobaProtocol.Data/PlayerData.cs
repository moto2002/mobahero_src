using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class PlayerData
	{
		[ProtoMember(1)]
		public long SummId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long SummLevel
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string SummName
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int Icon
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int Icon2
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int LadderScore
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int LadderRank
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int bottlelevel
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int bottleRank
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public int usercp
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public int tripleKill
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public int quadraKill
		{
			get;
			set;
		}

		[ProtoMember(13)]
		public int pentaKill
		{
			get;
			set;
		}

		[ProtoMember(14)]
		public int godlike
		{
			get;
			set;
		}

		[ProtoMember(15)]
		public int likeCount
		{
			get;
			set;
		}

		[ProtoMember(16)]
		public sbyte GameStatus
		{
			get;
			set;
		}

		[ProtoMember(17)]
		public BattleInfoData[] battleinfos
		{
			get;
			set;
		}

		[ProtoMember(18)]
		public int CharmRankvalue
		{
			get;
			set;
		}
	}
}
