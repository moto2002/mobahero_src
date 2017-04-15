using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace MobaClient
{
	public class User
	{
		public static bool bShowFriend_Flag = false;

		public static bool bShowGameSetting_Flag = false;

		public static bool bBuyThingsInIos = false;

		public static Action userDataChangedEvent;

		protected static User instance;

		private AccountData accountData = null;

		private UserData userData = null;

		private List<UserData> playerList = null;

		public static event Action UserDataChangedEvent
		{
			add
			{
				User.userDataChangedEvent = (Action)Delegate.Combine(User.userDataChangedEvent, value);
			}
			remove
			{
				User.userDataChangedEvent = (Action)Delegate.Remove(User.userDataChangedEvent, value);
			}
		}

		public List<UserData> Friends
		{
			get;
			set;
		}

		public List<HeroInfoData> HeroInfoList
		{
			get;
			set;
		}

		public List<EquipmentInfoData> EquipmentList
		{
			get;
			set;
		}

		public List<TalentInfoData> TalentInfoList
		{
			get;
			set;
		}

		public List<int> TempDrop
		{
			get;
			set;
		}

		public List<RuneInfoData> RuneInfoList
		{
			get;
			set;
		}

		public List<BattlesModel> BattlesList
		{
			get;
			set;
		}

		public List<TBCEnemyInfo> GlobalTBCBattleList
		{
			get;
			set;
		}

		public int TBCRestCount
		{
			get;
			set;
		}

		public List<TBCHeroStateInfo> TBCHeroStateInfoList
		{
			get;
			set;
		}

		public string[] MyArenaDefTeam
		{
			get;
			set;
		}

		public List<ArenaData> ArenaEnemyList
		{
			get;
			set;
		}

		public List<ArenaLogData> ArenaFightLogList
		{
			get;
			set;
		}

		public ArenaState ArenaStateInfo
		{
			get;
			set;
		}

		public List<ArenaData> ArenaRankList
		{
			get;
			set;
		}

		public List<ArenaData> LastDayArenaRankList
		{
			get;
			set;
		}

		public ArenaData MyArenaRankData
		{
			get;
			set;
		}

		public LuckyDrawData LuckyDrawStateInfo
		{
			get;
			set;
		}

		public List<LuckyDrawResult> LuckyDrawResultList
		{
			get;
			set;
		}

		public List<ShopData> AllShopInfoList
		{
			get;
			set;
		}

		public List<TaskData> TaskInfoList
		{
			get;
			set;
		}

		public List<MailData> MailList
		{
			get;
			set;
		}

		public UnionInfoData SelfUnionInfo
		{
			get;
			set;
		}

		public List<UnionMemberData> UnionMemberList
		{
			get;
			set;
		}

		public List<UnionInfoData> UnionInfoList
		{
			get;
			set;
		}

		public List<UnionLogData> UnionLogList
		{
			get;
			set;
		}

		public List<UnionMemberData> UnionReqMemberList
		{
			get;
			set;
		}

		public SmallMeleeData SelfSmallMeleeInfo
		{
			get;
			set;
		}

		public List<SmallMeleeData> SmallMeleeEnemyList
		{
			get;
			set;
		}

		public KillTitanData KillTitanInfo
		{
			get;
			set;
		}

		public List<KillTitanSnapshotData> KillTitanSnapshotList
		{
			get;
			set;
		}

		public int TitanOpenLocation
		{
			get;
			set;
		}

		public int NextOpenLocationSecond
		{
			get;
			set;
		}

		public List<HerosPositionData> HerosPositionInfo
		{
			get;
			set;
		}

		public string LastAwardBossId
		{
			get;
			set;
		}

		public int LastAwardBossHp
		{
			get;
			set;
		}

		public ChatClientInfo ChatClientInfo
		{
			get;
			set;
		}

		public List<ChatMessage> CahtMessageList
		{
			get;
			set;
		}

		public List<FriendData> FriendList
		{
			get;
			set;
		}

		public List<FriendData> ApplyList
		{
			get;
			set;
		}

		public List<FriendData> BlackList
		{
			get;
			set;
		}

		public List<TBCHeroStateInfo> myHeroStateInfo
		{
			get;
			set;
		}

		public List<TBCHeroStateInfo> targetHeroStateInfo
		{
			get;
			set;
		}

		public string TempAnnouncement
		{
			get;
			set;
		}

		public string ChatAddress
		{
			get;
			set;
		}

		public GameStatus GameStatus
		{
			get;
			set;
		}

		public int CurrentPlayInfo
		{
			get;
			set;
		}

		public long LogId
		{
			get;
			set;
		}

		public bool AppScore
		{
			get;
			set;
		}

		public bool GameID
		{
			get;
			set;
		}

		public MedalData MedalData
		{
			get;
			set;
		}

		public static User Singleton
		{
			get
			{
				return User.instance;
			}
		}

		public AccountData AccountData
		{
			get
			{
				return this.accountData;
			}
			set
			{
				if (value != null)
				{
					this.accountData = value;
					this.AccountInfoChanged();
				}
			}
		}

		public UserData UserData
		{
			get
			{
				return this.userData;
			}
			set
			{
				if (value != null)
				{
					this.userData = value;
					this.UserInfoChanged();
				}
			}
		}

		public List<UserData> PlayerList
		{
			get
			{
				return this.playerList;
			}
			set
			{
				if (value != null)
				{
					this.playerList = value;
					this.UserInfoChanged();
				}
			}
		}

		public List<bool> getListMedalData()
		{
			return new List<bool>
			{
				this.MedalData.Brave,
				this.MedalData.Death,
				this.MedalData.Raider,
				this.MedalData.Solitaire,
				this.MedalData.King,
				this.MedalData.Team,
				this.MedalData.Money,
				this.MedalData.Diamond,
				this.MedalData.Summoner,
				this.MedalData.Season,
				this.MedalData.Teammaster
			};
		}

		public User()
		{
			this.GameStatus = GameStatus.Offline;
		}

		public void UserInfoChanged()
		{
			if (User.userDataChangedEvent != null)
			{
				User.userDataChangedEvent();
			}
		}

		public void AccountInfoChanged()
		{
			if (User.userDataChangedEvent != null)
			{
				User.userDataChangedEvent();
			}
		}

		public void SetUserData(string nickname, int avatar)
		{
			if (this.UserData == null)
			{
				this.UserData = new UserData();
			}
			this.UserData.NickName = nickname;
			this.UserData.Avatar = avatar;
		}

		public void SaveDeviceToken(string deviceToken)
		{
		}
	}
}
