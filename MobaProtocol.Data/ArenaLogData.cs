using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ArenaLogData
	{
		[ProtoMember(1)]
		public string UserId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string TargetId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int TargetIcon
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int TargetPictureFrameId
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int TargetExp
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public string TargetNickName
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public long RankRang
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public bool IsWin
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public string FightTime
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public long LogId
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public string taregetTeamInfo
		{
			get;
			set;
		}

		[ProtoMember(13)]
		public int GetCoin
		{
			get;
			set;
		}

		[ProtoMember(14)]
		public int GetDimond
		{
			get;
			set;
		}

		[ProtoMember(15)]
		public int GetArenaMonery
		{
			get;
			set;
		}

		[ProtoMember(16)]
		public bool IsRevenge
		{
			get;
			set;
		}

		[ProtoMember(17)]
		public int LossCoin
		{
			get;
			set;
		}

		[ProtoMember(18)]
		public int LossDimond
		{
			get;
			set;
		}

		[ProtoMember(19)]
		public bool IsRevengeRecord
		{
			get;
			set;
		}

		[ProtoMember(20)]
		public int TargetLevel
		{
			get;
			set;
		}

		[ProtoMember(21)]
		public long TargetRank
		{
			get;
			set;
		}

		[ProtoMember(22)]
		public long SelfRank
		{
			get;
			set;
		}

		[ProtoMember(23)]
		public long TargetPower
		{
			get;
			set;
		}

		[ProtoMember(24)]
		public long MyPower
		{
			get;
			set;
		}

		[ProtoMember(25)]
		public int MyIcon
		{
			get;
			set;
		}

		[ProtoMember(26)]
		public int MyPictureFrameId
		{
			get;
			set;
		}

		[ProtoMember(27)]
		public int MyExp
		{
			get;
			set;
		}

		[ProtoMember(28)]
		public string MyNickName
		{
			get;
			set;
		}

		[ProtoMember(29)]
		public string MyTeamInfo
		{
			get;
			set;
		}
	}
}
