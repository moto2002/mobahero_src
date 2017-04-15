using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class UserData
	{
		[ProtoMember(1)]
		public string UserId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string NickName
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int Avatar
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public UserStatus Status
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public long Money
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public long Diamonds
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public long Exp
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public long RankId
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int Achievements
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public bool HasBuy
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public int VIP
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public long SummonerId
		{
			get;
			set;
		}

		[ProtoMember(13)]
		public int ServerId
		{
			get;
			set;
		}

		[ProtoMember(14)]
		public int Energy
		{
			get;
			set;
		}

		[ProtoMember(15)]
		public int SkillPoint
		{
			get;
			set;
		}

		[ProtoMember(16)]
		public int SkillPointFullTimeleft
		{
			get;
			set;
		}

		[ProtoMember(17)]
		public string ServerTime
		{
			get;
			set;
		}

		[ProtoMember(18)]
		public int ArenaMoney
		{
			get;
			set;
		}

		[ProtoMember(19)]
		public int TBCMoney
		{
			get;
			set;
		}

		[ProtoMember(20)]
		public int BigFightMoney
		{
			get;
			set;
		}

		[ProtoMember(21)]
		public int UnioMoney
		{
			get;
			set;
		}

		[ProtoMember(22)]
		public int AttendanceCount
		{
			get;
			set;
		}

		[ProtoMember(23)]
		public bool DayIsAttendance
		{
			get;
			set;
		}

		[ProtoMember(24)]
		public int ChangNickNameCount
		{
			get;
			set;
		}

		[ProtoMember(25)]
		public string OwnIconStr
		{
			get;
			set;
		}

		[ProtoMember(26)]
		public string OwnPictureFrame
		{
			get;
			set;
		}

		[ProtoMember(27)]
		public int PictureFrame
		{
			get;
			set;
		}

		[ProtoMember(28)]
		public string UserContent
		{
			get;
			set;
		}

		[ProtoMember(29)]
		public int PayMoneySum
		{
			get;
			set;
		}

		[ProtoMember(30)]
		public int BuyCoinCount
		{
			get;
			set;
		}

		[ProtoMember(31)]
		public int BuyEnergyCount
		{
			get;
			set;
		}

		[ProtoMember(32)]
		public int BuySkillPointCount
		{
			get;
			set;
		}

		[ProtoMember(33)]
		public string LastExitUnionTime
		{
			get;
			set;
		}

		[ProtoMember(34)]
		public int UnionId
		{
			get;
			set;
		}

		[ProtoMember(35)]
		public int LastUnionId
		{
			get;
			set;
		}

		[ProtoMember(36)]
		public long BestRank
		{
			get;
			set;
		}

		[ProtoMember(37)]
		public long TrendRank
		{
			get;
			set;
		}

		[ProtoMember(38)]
		public long JoinUnionCount
		{
			get;
			set;
		}

		[ProtoMember(39)]
		public sbyte GameStatus
		{
			get;
			set;
		}

		[ProtoMember(40)]
		public double LadderScore
		{
			get;
			set;
		}

		[ProtoMember(41)]
		public int LastDayLadderRank
		{
			get;
			set;
		}

		[ProtoMember(42)]
		public int RankUp
		{
			get;
			set;
		}

		[ProtoMember(43)]
		public int LadderWinCount
		{
			get;
			set;
		}

		[ProtoMember(44)]
		public int LadderLossCount
		{
			get;
			set;
		}

		[ProtoMember(45)]
		public int LadderMoney
		{
			get;
			set;
		}

		[ProtoMember(46)]
		public string NewerIds
		{
			get;
			set;
		}

		[ProtoMember(47)]
		public int EnergyRemainSeconds
		{
			get;
			set;
		}

		[ProtoMember(48)]
		public string JPushRegId
		{
			get;
			set;
		}

		[ProtoMember(49)]
		public string SummSkills
		{
			get;
			set;
		}

		[ProtoMember(50)]
		public string RechargedAmounts
		{
			get;
			set;
		}

		[ProtoMember(51)]
		public int MonthEndTime
		{
			get;
			set;
		}

		[ProtoMember(52)]
		public int TotalRechargeAmount
		{
			get;
			set;
		}

		[ProtoMember(53)]
		public int TotalRechargeGold
		{
			get;
			set;
		}

		[ProtoMember(54)]
		public int serverId
		{
			get;
			set;
		}

		[ProtoMember(55)]
		public long magicbottleid
		{
			get;
			set;
		}

		[ProtoMember(56)]
		public string captionIds
		{
			get;
			set;
		}

		[ProtoMember(57)]
		public int SmallCap
		{
			get;
			set;
		}

		[ProtoMember(58)]
		public int LoginCount
		{
			get;
			set;
		}

		[ProtoMember(59)]
		public int TicketCount
		{
			get;
			set;
		}

		[ProtoMember(60)]
		public int Charm
		{
			get;
			set;
		}

		[ProtoMember(61)]
		public int HighScore
		{
			get;
			set;
		}

		[ProtoMember(62)]
		public int Guide_type
		{
			get;
			set;
		}

		[ProtoMember(63)]
		public int Guide_stage
		{
			get;
			set;
		}

		[ProtoMember(64)]
		public int Guide_normalCastSkill
		{
			get;
			set;
		}

		[ProtoMember(65)]
		public int Speaker
		{
			get;
			set;
		}

		[ProtoMember(66)]
		public int ReportCount
		{
			get;
			set;
		}

		[ProtoMember(67)]
		public DateTime SeasonStartTime
		{
			get;
			set;
		}

		[ProtoMember(68)]
		public DateTime SeasonEndTime
		{
			get;
			set;
		}

		[ProtoMember(69)]
		public int CharmRankValue
		{
			get;
			set;
		}

		public DateTime ReportCountTime
		{
			get;
			set;
		}

		public int inneraccount
		{
			get;
			set;
		}

		public GameStatus GetGameStatus()
		{
			return (GameStatus)this.GameStatus;
		}
	}
}
