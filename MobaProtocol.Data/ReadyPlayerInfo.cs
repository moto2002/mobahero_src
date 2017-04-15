using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ReadyPlayerInfo
	{
		public string friendTeamId;

		[ProtoMember(1)]
		public PvpMemberInfo baseInfo
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string serverKey
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int serverId
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int newUid
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int roomId
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public string newkey
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public HeroInfo heroInfo
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public byte group
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public byte teamPos
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public string selfDefSkillId
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public string heroSkinId
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public byte isRobot
		{
			get;
			set;
		}

		[ProtoMember(13)]
		public int rankFrame
		{
			get;
			set;
		}

		[ProtoMember(14)]
		public int CharmRankValue
		{
			get;
			set;
		}

		public bool isInRoom
		{
			get;
			set;
		}

		public List<HeroInfo> heroinfolist
		{
			get;
			set;
		}

		public List<HopeRealityValue> hopeRealityValue
		{
			get;
			set;
		}

		public string uid
		{
			get
			{
				return this.baseInfo.userId;
			}
			set
			{
				this.baseInfo.userId = value;
			}
		}

		public string qid
		{
			get
			{
				string uid;
				if (this.friendTeamId != null && this.friendTeamId.Length > 0)
				{
					uid = this.friendTeamId;
				}
				else
				{
					uid = this.uid;
				}
				return uid;
			}
		}

		public bool isClientLink
		{
			get;
			set;
		}

		public int battleId
		{
			get;
			set;
		}

		public byte gameType
		{
			get;
			set;
		}

		public byte state
		{
			get;
			set;
		}

		public bool horeChecked
		{
			get;
			set;
		}

		public bool horeSelected
		{
			get;
			set;
		}

		public bool isOnLine
		{
			get;
			set;
		}

		public bool isRunAway
		{
			get;
			set;
		}

		public byte joinType
		{
			get;
			set;
		}

		public bool isReadySelectHero
		{
			get;
			set;
		}

		public int randomcount
		{
			get;
			set;
		}

		public int backCount
		{
			get;
			set;
		}

		public PlayerModifyState pmstate
		{
			get;
			set;
		}

		public ReadyPlayerSampleInfo SampleData
		{
			get
			{
				return new ReadyPlayerSampleInfo
				{
					newUid = this.newUid,
					serverId = this.serverId,
					group = this.group,
					teamPos = this.teamPos,
					userName = this.baseInfo.userName,
					heroInfo = this.heroInfo,
					readyChecked = this.horeChecked,
					horeSelected = this.horeSelected,
					level = this.baseInfo.Level,
					selfDefSkillId = this.selfDefSkillId,
					heroSkinId = this.heroSkinId,
					SummerId = this.baseInfo.SummerId,
					IsReadySelectHero = this.isReadySelectHero,
					RankFrame = this.rankFrame,
					CharmRankvalue = this.CharmRankValue
				};
			}
		}

		public bool IsRobot()
		{
			return this.isRobot > 0;
		}

		public bool IsEasyRobot()
		{
			return this.isRobot == 1;
		}

		public bool IsNormalRobot()
		{
			return this.isRobot == 2;
		}

		public bool IsHardRobot()
		{
			return this.isRobot == 3;
		}

		public byte GetRobotLevel()
		{
			return this.isRobot;
		}

		public void Init(byte inRobotLevel = 0)
		{
			this.backCount = 0;
			this.isRobot = inRobotLevel;
		}

		public void ResetState()
		{
			this.state = 1;
			this.roomId = 0;
			this.horeChecked = true;
			this.horeSelected = false;
			this.heroInfo = null;
			this.newUid = 0;
			this.group = 0;
			this.teamPos = 0;
			this.isOnLine = true;
			this.isRunAway = false;
			this.heroSkinId = "";
			this.pmstate = PlayerModifyState.PMS_FREE;
			this.backCount++;
		}
	}
}
