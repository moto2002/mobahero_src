using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaClient
{
	public class PhotonClient
	{
		public enum SendTargetType
		{
			User = 1,
			Room,
			Global,
			Union,
			GM,
			System,
			PrivateMessage
		}

		public delegate void DeleSendMsg2ClientCustom(Photon2ClientMsg msg, object obj);

		public delegate void DeleSendMsg2ClientMasterCode(MobaMasterCode masterCode, object obj);

		public delegate void DeleSendMsg2ClientGameCode(MobaGameCode gameCode, object obj);

		public delegate void DeleSendMsg2ClientPvpCode(PvpCode pvpCode, object obj);

		public delegate void DeleSendMsg2ClientChatCode(MobaChatCode chatCode, object obj);

		public delegate void DeleSendMsg2ClientLobbyCode(LobbyCode chatCode, object obj);

		public delegate void DeleSendMsg2ClientFriendCode(MobaFriendCode chatCode, object obj);

		public delegate void DeleSendMsg2ClientTeamRoomCode(MobaTeamRoomCode chatCode, object obj);

		public delegate void DeleSendMsg2ClientGateCode(MobaGateCode gateCode, object obj);

		public delegate void DeleSendMsg2ClientUserDataCode(MobaUserDataCode userdataCode, object obj);

		private class EnumEquality : IEqualityComparer<MobaPeerType>
		{
			public bool Equals(MobaPeerType x, MobaPeerType y)
			{
				return x == y;
			}

			public int GetHashCode(MobaPeerType x)
			{
				int num = (int)x;
				return num.GetHashCode();
			}
		}

		private readonly Dictionary<MobaPeerType, MobaPeer> mPeerPool = new Dictionary<MobaPeerType, MobaPeer>(new PhotonClient.EnumEquality());

		private MobaPeer mCurrMainPeer = null;

		private MobaPeer mLastMainPeer = null;

		private List<MobaPeer> mAssistantPeerList = new List<MobaPeer>();

		private PhotonClient.DeleSendMsg2ClientCustom mDeleSendMsgCustom = null;

		private PhotonClient.DeleSendMsg2ClientMasterCode mDeleSendMsgMasterCode = null;

		private PhotonClient.DeleSendMsg2ClientGameCode mDeleSendMsgGameCode = null;

		private PhotonClient.DeleSendMsg2ClientPvpCode mDeleSendMsgPvpCode = null;

		private PhotonClient.DeleSendMsg2ClientChatCode mDeleSendMsgChatCode = null;

		private PhotonClient.DeleSendMsg2ClientGateCode mDeleSendMsgGateCode = null;

		private PhotonClient.DeleSendMsg2ClientLobbyCode mDeleSendMsgLobbyCode = null;

		private PhotonClient.DeleSendMsg2ClientFriendCode mDeleSendMsgFriendCode = null;

		private PhotonClient.DeleSendMsg2ClientTeamRoomCode mDeleSendMsgTeamRoomCode = null;

		private PhotonClient.DeleSendMsg2ClientUserDataCode mDeleSendMsgUserDataCode = null;

		public User m_user;

		public ClientData clientData;

		public List<ServerInfo> m_serverlist;

		public int currLoginServerIndex;

		public bool IsReconnect = false;

		public string OnlyServerKey = string.Empty;

		public static int DISCONNECT_TIMEOUT = 20000;

		private UdpDriver udpDriver = null;

		public TimeSyncSystem timeSyncSystem = null;

		private object sendThreadLock = new object();

		private string mGamePeerServerName = string.Empty;

		private string mGamePeerAppName = string.Empty;

		private static readonly SimpleObjectPool<Dictionary<byte, object>> _dictPool = new SimpleObjectPool<Dictionary<byte, object>>(10, delegate(Dictionary<byte, object> dict)
		{
			dict.Clear();
		}, null, "");

		public static bool usePool = false;

		private List<CmdPkgForThread> cmdCacheList = new List<CmdPkgForThread>();

		private List<CmdPkgForThread> cmdCacheListBack = new List<CmdPkgForThread>();

		private event Action<bool, int, int> ConnectEvent;

		private event Action<StatusCode, int, int> ConnectStatusEvent;

		private event Action<int, string, AccountData> RegisterEvent;

		private event Action<int, string, AccountData> LoginEvent;

		private event Action<int, int, string> ErrorEvent;

		private event Action<int, string, List<ServerInfo>> GetServerListEvent;

		private event Action<bool, int, string, ClientData> UpgradeEvent;

		private event Action<int, string, AccountData> LoginByChannelIdEvent;

		public event Action<bool, int, int> ConnectEventCallback
		{
			add
			{
				this.ConnectEvent = (Action<bool, int, int>)Delegate.Combine(this.ConnectEvent, value);
			}
			remove
			{
				this.ConnectEvent = (Action<bool, int, int>)Delegate.Remove(this.ConnectEvent, value);
			}
		}

		public event Action<StatusCode, int, int> ConnectStatusEventCallback
		{
			add
			{
				this.ConnectStatusEvent = (Action<StatusCode, int, int>)Delegate.Combine(this.ConnectStatusEvent, value);
			}
			remove
			{
				this.ConnectStatusEvent = (Action<StatusCode, int, int>)Delegate.Remove(this.ConnectStatusEvent, value);
			}
		}

		public event Action<int, string, AccountData> LoginEventCallback
		{
			add
			{
				this.LoginEvent = (Action<int, string, AccountData>)Delegate.Combine(this.LoginEvent, value);
			}
			remove
			{
				this.LoginEvent = (Action<int, string, AccountData>)Delegate.Remove(this.LoginEvent, value);
			}
		}

		public event Action<int, string, AccountData> RegisterEventCallback
		{
			add
			{
				this.RegisterEvent = (Action<int, string, AccountData>)Delegate.Combine(this.RegisterEvent, value);
			}
			remove
			{
				this.RegisterEvent = (Action<int, string, AccountData>)Delegate.Remove(this.RegisterEvent, value);
			}
		}

		public event Action<int, int, string> ErrorEventCallback
		{
			add
			{
				this.ErrorEvent = (Action<int, int, string>)Delegate.Combine(this.ErrorEvent, value);
			}
			remove
			{
				this.ErrorEvent = (Action<int, int, string>)Delegate.Remove(this.ErrorEvent, value);
			}
		}

		public event Action<int, string, List<ServerInfo>> GetServerListCallback
		{
			add
			{
				this.GetServerListEvent = (Action<int, string, List<ServerInfo>>)Delegate.Combine(this.GetServerListEvent, value);
			}
			remove
			{
				this.GetServerListEvent = (Action<int, string, List<ServerInfo>>)Delegate.Remove(this.GetServerListEvent, value);
			}
		}

		public event Action<bool, int, string, ClientData> UpgradeEventCallback
		{
			add
			{
				this.UpgradeEvent = (Action<bool, int, string, ClientData>)Delegate.Combine(this.UpgradeEvent, value);
			}
			remove
			{
				this.UpgradeEvent = (Action<bool, int, string, ClientData>)Delegate.Remove(this.UpgradeEvent, value);
			}
		}

		public event Action<int, string, AccountData> LoginByChannelIdEventCallBack
		{
			add
			{
				this.LoginByChannelIdEvent = (Action<int, string, AccountData>)Delegate.Combine(this.LoginByChannelIdEvent, value);
			}
			remove
			{
				this.LoginByChannelIdEvent = (Action<int, string, AccountData>)Delegate.Remove(this.LoginByChannelIdEvent, value);
			}
		}

		private event Action<int, string, MobaTeamRoomCode, List<RoomData>> TeamRoomOperationEvent;

		private event Action<int, string, List<TaskData>> CompleteTaskMessageEvent;

		private event Action<int, string, NotificationData> NotificationEvent;

		private event Action<int, string, int, bool> NoticeMedalEvent;

		private event Action<int, string, UserData> RegisterUserEvent;

		private event Action<int, string, UserData> LoginUserEvent;

		private event Action<int, string, List<HeroInfoData>> GetHeroListEvent;

		private event Action<int, string, List<EquipmentInfoData>> GetEquipmentListEvent;

		private event Action<int, string, string> UsingEquipmentEvent;

		private event Action<int, string> HeroAdvanceEvent;

		private event Action<int, string, string[]> GetRewardDropEvent;

		private event Action<int, string, List<EquipmentInfoData>> TryUploadFightResultEvent;

		private event Action<int, string, UserData> GuestLoginEvent;

		private event Action<int, string, List<TalentInfoData>> GetMyTalentListEvent;

		private event Action<int, string, int, List<TalentInfoData>> TryChangeTalentEvent;

		private event Action<int, string> TryBuyTalentPagEvent;

		private event Action<int, string, int> TryModifyCurrUseTalentPagEvent;

		private event Action<int, string, int> TryRestCurrUseTalentPagEvent;

		private event Action<int, string, HeroInfoData> TryUseSoulstonesEvent;

		private event Action<int, string, int> TryUsePropsEvent;

		private event Action<int, string, List<RuneInfoData>> GetMyRuneListEvent;

		private event Action<int, string, List<RuneInfoData>> TryChangeRuneEvent;

		private event Action<int, string> TryBuyRunePagEvent;

		private event Action<int, string, int> TryModifyCurrUseRunePagEvent;

		private event Action<int, string, EquipmentInfoData> TryCoalesceEvent;

		private event Action<int, string, List<BattlesModel>> GetBattlesInfoEvent;

		private event Action<int, string, int> TryBuySkillPointEvent;

		private event Action<int, string> TryUsingSkillPointEvent;

		private event Action<int, string, bool, int> TryCheckUnlockByBattleIdEvent;

		private event Action<int, string> TryUpdateDefFightTeamEvent;

		private event Action<int, string, List<TBCEnemyInfo>> TryGetTBCEnemyInfoEvent;

		private event Action<int, string> TryRestTBCEnemyInfoEvent;

		private event Action<int, string, List<TBCHeroStateInfo>, List<TBCHeroStateInfo>> TrySaveTBCEnemyInfoEvent;

		private event Action<int, string, List<TBCHeroStateInfo>> GetTBCMyHeroStateInfoEvent;

		private event Action<int, string, int> TryReceiveTBCRewardEvent;

		private event Action<int, string, string[]> TryUpdateArenaDefTeamEvent;

		private event Action<int, string, string[]> GetArenaDefTeamEvent;

		private event Action<int, string, List<ArenaData>> GetArenaEnemyInfoEvent;

		private event Action<int, string> TryArenaAtcCheckEvent;

		private event Action<int, string, long, ArenaAccount> TryUploadArenaAtcResultEvent;

		private event Action<int, string, List<ArenaLogData>> GetArenaFightLogEvent;

		private event Action<int, string, ArenaState> GetArenaStateEvent;

		private event Action<int, string> TryRestArenaCountEvent;

		private event Action<int, string, int> TryRestArenaCDTimeEvent;

		private event Action<int, string, List<ArenaData>> GetArenaRankListEvent;

		private event Action<int, string, List<ArenaData>> GetLastDayArenaRankListEvent;

		private event Action<int, string, ArenaData> GetMyArenaRankEvent;

		private event Action<int, string, LuckyDrawData> GetLuckyDrawStateEvent;

		private event Action<int, string, List<LuckyDrawResult>> TryLuckyDrawEvent;

		private event Action<int, string, ShopData> GetShopGoodsByShopTypeEvent;

		private event Action<int, string, int> TryBuyGoodsEvent;

		private event Action<int, string, ShopData> TryRestShopByShopTypeEvent;

		private event Action<int, string, List<TaskData>> GetTaskListEvent;

		private event Action<int, string, int> TryCompleteTaskEvent;

		private event Action<int, string, string> TryAttendanceByTypeEvnet;

		private event Action<int, string, int> TryModfiyNickNameEvent;

		private event Action<int, string> TryModfiyIconEvent;

		private event Action<int, string, int> TrySellPropsEvent;

		private event Action<int, string, List<ExchangeData>> TryExchangeByDimondEvent;

		private event Action<int, string, List<MailData>> GetMailListEvent;

		private event Action<int, string, List<RewardModel>> TryReceiveMailAttachmentEvent;

		private event Action<int, string, int> TryCreateUnionEvent;

		private event Action<int, string, UnionInfoData> GetUnionInfoEvent;

		private event Action<int, string> TryDissolveUnionEvent;

		private event Action<int, string, UnionInfoData> TrySearchUnionEvent;

		private event Action<int, string> TryJoinUnionEvent;

		private event Action<int, string, List<UnionMemberData>> TryKickUnionEvent;

		private event Action<int, string> TryLeaveUnionEvent;

		private event Action<int, string, UnionInfoData> TryModifyUnionSettingEvent;

		private event Action<int, string> TryModifyAnnouncementEvent;

		private event Action<int, string, List<UnionInfoData>> GetUnionListEvent;

		private event Action<int, string, List<UnionLogData>> GetUnionLogsEvent;

		private event Action<int, string, List<UnionMemberData>> GetMemberListEvent;

		private event Action<int, string, string> TryUpgradeMasterEvent;

		private event Action<int, string, string> TryAppointElderEvent;

		private event Action<int, string, string> TryUnAppointElderEvent;

		private event Action<int, string, List<UnionMemberData>> GetUnionRequestListEvent;

		private event Action<int, string, string> TryDisposeUnionReqEvent;

		private event Action<int, string, string> TryEnchantEquipmentEvent;

		private event Action<int, string, List<SweepData>> TrySweepBattleEvent;

		private event Action<int, string, long> TryModifyEmailStateEvent;

		private event Action<int, string> TryUpdateSmallMeleeTeamEvent;

		private event Action<int, string, SmallMeleeData> GetSmallMeleeInfoEvent;

		private event Action<int, string, List<TalentModel>> GetTalentInfoByUserIdEvent;

		private event Action<int, string, KillTitanData> TryChangeKillTitanTeamEvent;

		private event Action<int, string, KillTitanData> GetKillTitanInfoEvent;

		private event Action<int, string, KillTitanReward> TryReceiveKillTitanRewardEvent;

		private event Action<int, string, List<KillTitanSnapshotData>> TryAddKillTitanSnapshotEvent;

		private event Action<int, string, List<KillTitanSnapshotData>> GetKillTitanSnapshotEvent;

		private event Action<int, string, List<AwardInfoData>> TryReceiveSnapshotAwardEvent;

		private event Action<int, string> TryBuySpecialShopOwnEvent;

		private event Action<int, string, int> TryRestTodayBattlesCountEvent;

		private event Action<int, int> TryJoinPvpEvent;

		private event Action<int, string, FriendData> TryApplyAddFriendEvent;

		private event Action<int, string, List<FriendData>> GetFriendListEvent;

		private event Action<int, string, byte, long> TryModifyFriendStatusEvent;

		private event Action<int, string, List<NotificationData>> GetFriendsMessagesEvent;

		private event Action<int, string, string[]> GetUserInfoBySummIdEvent;

		private event Action<int, string, List<bool>> GetMedalDataByUserIdEvent;

		private event Action<int, string> TryBuySkinEvent;

		private event Action<int, string> TryChangeSkinEvent;

		public event Action<int, string, MobaTeamRoomCode, List<RoomData>> TeamRoomOperationEventCallback
		{
			add
			{
				this.TeamRoomOperationEvent = (Action<int, string, MobaTeamRoomCode, List<RoomData>>)Delegate.Combine(this.TeamRoomOperationEvent, value);
			}
			remove
			{
				this.TeamRoomOperationEvent = (Action<int, string, MobaTeamRoomCode, List<RoomData>>)Delegate.Remove(this.TeamRoomOperationEvent, value);
			}
		}

		public event Action<int, string, List<TaskData>> CompleteTaskMessageEventCallback
		{
			add
			{
				this.CompleteTaskMessageEvent = (Action<int, string, List<TaskData>>)Delegate.Combine(this.CompleteTaskMessageEvent, value);
			}
			remove
			{
				this.CompleteTaskMessageEvent = (Action<int, string, List<TaskData>>)Delegate.Remove(this.CompleteTaskMessageEvent, value);
			}
		}

		public event Action<int, string, NotificationData> NotificationEventCallback
		{
			add
			{
				this.NotificationEvent = (Action<int, string, NotificationData>)Delegate.Combine(this.NotificationEvent, value);
			}
			remove
			{
				this.NotificationEvent = (Action<int, string, NotificationData>)Delegate.Remove(this.NotificationEvent, value);
			}
		}

		public event Action<int, string, UserData> LoginUserEventCallback
		{
			add
			{
				this.LoginUserEvent = (Action<int, string, UserData>)Delegate.Combine(this.LoginUserEvent, value);
			}
			remove
			{
				this.LoginUserEvent = (Action<int, string, UserData>)Delegate.Remove(this.LoginUserEvent, value);
			}
		}

		public event Action<int, string, UserData> RegisterUserEventCallback
		{
			add
			{
				this.RegisterUserEvent = (Action<int, string, UserData>)Delegate.Combine(this.RegisterUserEvent, value);
			}
			remove
			{
				this.RegisterUserEvent = (Action<int, string, UserData>)Delegate.Remove(this.RegisterUserEvent, value);
			}
		}

		public event Action<int, string, List<HeroInfoData>> GetHeroListEventCallback
		{
			add
			{
				this.GetHeroListEvent = (Action<int, string, List<HeroInfoData>>)Delegate.Combine(this.GetHeroListEvent, value);
			}
			remove
			{
				this.GetHeroListEvent = (Action<int, string, List<HeroInfoData>>)Delegate.Remove(this.GetHeroListEvent, value);
			}
		}

		public event Action<int, string, List<EquipmentInfoData>> GetEquipmentListEventCallback
		{
			add
			{
				this.GetEquipmentListEvent = (Action<int, string, List<EquipmentInfoData>>)Delegate.Combine(this.GetEquipmentListEvent, value);
			}
			remove
			{
				this.GetEquipmentListEvent = (Action<int, string, List<EquipmentInfoData>>)Delegate.Remove(this.GetEquipmentListEvent, value);
			}
		}

		public event Action<int, string, string> UsingEquiomentEventCallback
		{
			add
			{
				this.UsingEquipmentEvent = (Action<int, string, string>)Delegate.Combine(this.UsingEquipmentEvent, value);
			}
			remove
			{
				this.UsingEquipmentEvent = (Action<int, string, string>)Delegate.Remove(this.UsingEquipmentEvent, value);
			}
		}

		public event Action<int, string> HeroAdvanceEventCallback
		{
			add
			{
				this.HeroAdvanceEvent = (Action<int, string>)Delegate.Combine(this.HeroAdvanceEvent, value);
			}
			remove
			{
				this.HeroAdvanceEvent = (Action<int, string>)Delegate.Remove(this.HeroAdvanceEvent, value);
			}
		}

		public event Action<int, string, string[]> GetRewardDropEventCallback
		{
			add
			{
				this.GetRewardDropEvent = (Action<int, string, string[]>)Delegate.Combine(this.GetRewardDropEvent, value);
			}
			remove
			{
				this.GetRewardDropEvent = (Action<int, string, string[]>)Delegate.Remove(this.GetRewardDropEvent, value);
			}
		}

		public event Action<int, string, List<EquipmentInfoData>> TryUploadFightResultEventCallback
		{
			add
			{
				this.TryUploadFightResultEvent = (Action<int, string, List<EquipmentInfoData>>)Delegate.Combine(this.TryUploadFightResultEvent, value);
			}
			remove
			{
				this.TryUploadFightResultEvent = (Action<int, string, List<EquipmentInfoData>>)Delegate.Remove(this.TryUploadFightResultEvent, value);
			}
		}

		public event Action<int, string, UserData> GuestLoginEventCallback
		{
			add
			{
				this.GuestLoginEvent = (Action<int, string, UserData>)Delegate.Combine(this.GuestLoginEvent, value);
			}
			remove
			{
				this.GuestLoginEvent = (Action<int, string, UserData>)Delegate.Remove(this.GuestLoginEvent, value);
			}
		}

		public event Action<int, string, List<TalentInfoData>> GetMyTalentListEventCallback
		{
			add
			{
				this.GetMyTalentListEvent = (Action<int, string, List<TalentInfoData>>)Delegate.Combine(this.GetMyTalentListEvent, value);
			}
			remove
			{
				this.GetMyTalentListEvent = (Action<int, string, List<TalentInfoData>>)Delegate.Remove(this.GetMyTalentListEvent, value);
			}
		}

		public event Action<int, string, int, List<TalentInfoData>> TryChangeTalentEventCallback
		{
			add
			{
				this.TryChangeTalentEvent = (Action<int, string, int, List<TalentInfoData>>)Delegate.Combine(this.TryChangeTalentEvent, value);
			}
			remove
			{
				this.TryChangeTalentEvent = (Action<int, string, int, List<TalentInfoData>>)Delegate.Remove(this.TryChangeTalentEvent, value);
			}
		}

		public event Action<int, string> TryBuyTalentPagEventCallback
		{
			add
			{
				this.TryBuyTalentPagEvent = (Action<int, string>)Delegate.Combine(this.TryBuyTalentPagEvent, value);
			}
			remove
			{
				this.TryBuyTalentPagEvent = (Action<int, string>)Delegate.Remove(this.TryBuyTalentPagEvent, value);
			}
		}

		public event Action<int, string, int> TryModifyCurrUseTalentPagCallback
		{
			add
			{
				this.TryModifyCurrUseTalentPagEvent = (Action<int, string, int>)Delegate.Combine(this.TryModifyCurrUseTalentPagEvent, value);
			}
			remove
			{
				this.TryModifyCurrUseTalentPagEvent = (Action<int, string, int>)Delegate.Remove(this.TryModifyCurrUseTalentPagEvent, value);
			}
		}

		public event Action<int, string, int> TryRestCurrUseTalentPagEventCallback
		{
			add
			{
				this.TryRestCurrUseTalentPagEvent = (Action<int, string, int>)Delegate.Combine(this.TryRestCurrUseTalentPagEvent, value);
			}
			remove
			{
				this.TryRestCurrUseTalentPagEvent = (Action<int, string, int>)Delegate.Remove(this.TryRestCurrUseTalentPagEvent, value);
			}
		}

		public event Action<int, string, HeroInfoData> TryUseSoulstonesEventCallback
		{
			add
			{
				this.TryUseSoulstonesEvent = (Action<int, string, HeroInfoData>)Delegate.Combine(this.TryUseSoulstonesEvent, value);
			}
			remove
			{
				this.TryUseSoulstonesEvent = (Action<int, string, HeroInfoData>)Delegate.Remove(this.TryUseSoulstonesEvent, value);
			}
		}

		public event Action<int, string, int> TryUsePropsEventCallback
		{
			add
			{
				this.TryUsePropsEvent = (Action<int, string, int>)Delegate.Combine(this.TryUsePropsEvent, value);
			}
			remove
			{
				this.TryUsePropsEvent = (Action<int, string, int>)Delegate.Remove(this.TryUsePropsEvent, value);
			}
		}

		public event Action<int, string, List<RuneInfoData>> GetMyRuneListEventCallback
		{
			add
			{
				this.GetMyRuneListEvent = (Action<int, string, List<RuneInfoData>>)Delegate.Combine(this.GetMyRuneListEvent, value);
			}
			remove
			{
				this.GetMyRuneListEvent = (Action<int, string, List<RuneInfoData>>)Delegate.Remove(this.GetMyRuneListEvent, value);
			}
		}

		public event Action<int, string, List<RuneInfoData>> TryChangeRuneEventCallback
		{
			add
			{
				this.TryChangeRuneEvent = (Action<int, string, List<RuneInfoData>>)Delegate.Combine(this.TryChangeRuneEvent, value);
			}
			remove
			{
				this.TryChangeRuneEvent = (Action<int, string, List<RuneInfoData>>)Delegate.Remove(this.TryChangeRuneEvent, value);
			}
		}

		public event Action<int, string> TryBuyRunePagEventCallback
		{
			add
			{
				this.TryBuyRunePagEvent = (Action<int, string>)Delegate.Combine(this.TryBuyRunePagEvent, value);
			}
			remove
			{
				this.TryBuyRunePagEvent = (Action<int, string>)Delegate.Remove(this.TryBuyRunePagEvent, value);
			}
		}

		public event Action<int, string, int> TryModifyCurrUseRunePagEventCallback
		{
			add
			{
				this.TryModifyCurrUseRunePagEvent = (Action<int, string, int>)Delegate.Combine(this.TryModifyCurrUseRunePagEvent, value);
			}
			remove
			{
				this.TryModifyCurrUseRunePagEvent = (Action<int, string, int>)Delegate.Remove(this.TryModifyCurrUseRunePagEvent, value);
			}
		}

		public event Action<int, string, EquipmentInfoData> TryCoalesceEventCallback
		{
			add
			{
				this.TryCoalesceEvent = (Action<int, string, EquipmentInfoData>)Delegate.Combine(this.TryCoalesceEvent, value);
			}
			remove
			{
				this.TryCoalesceEvent = (Action<int, string, EquipmentInfoData>)Delegate.Remove(this.TryCoalesceEvent, value);
			}
		}

		public event Action<int, string, List<BattlesModel>> GetBattlesInfoEventCallback
		{
			add
			{
				this.GetBattlesInfoEvent = (Action<int, string, List<BattlesModel>>)Delegate.Combine(this.GetBattlesInfoEvent, value);
			}
			remove
			{
				this.GetBattlesInfoEvent = (Action<int, string, List<BattlesModel>>)Delegate.Remove(this.GetBattlesInfoEvent, value);
			}
		}

		public event Action<int, string, int> TryBuySkillPointEventCallback
		{
			add
			{
				this.TryBuySkillPointEvent = (Action<int, string, int>)Delegate.Combine(this.TryBuySkillPointEvent, value);
			}
			remove
			{
				this.TryBuySkillPointEvent = (Action<int, string, int>)Delegate.Remove(this.TryBuySkillPointEvent, value);
			}
		}

		public event Action<int, string> TryUsingSkillPointEventCallback
		{
			add
			{
				this.TryUsingSkillPointEvent = (Action<int, string>)Delegate.Combine(this.TryUsingSkillPointEvent, value);
			}
			remove
			{
				this.TryUsingSkillPointEvent = (Action<int, string>)Delegate.Remove(this.TryUsingSkillPointEvent, value);
			}
		}

		public event Action<int, string, bool, int> TryCheckUnlockByBattleIdEventCallback
		{
			add
			{
				this.TryCheckUnlockByBattleIdEvent = (Action<int, string, bool, int>)Delegate.Combine(this.TryCheckUnlockByBattleIdEvent, value);
			}
			remove
			{
				this.TryCheckUnlockByBattleIdEvent = (Action<int, string, bool, int>)Delegate.Remove(this.TryCheckUnlockByBattleIdEvent, value);
			}
		}

		public event Action<int, string> TryUpdateDefFightTeamEventCallback
		{
			add
			{
				this.TryUpdateDefFightTeamEvent = (Action<int, string>)Delegate.Combine(this.TryUpdateDefFightTeamEvent, value);
			}
			remove
			{
				this.TryUpdateDefFightTeamEvent = (Action<int, string>)Delegate.Remove(this.TryUpdateDefFightTeamEvent, value);
			}
		}

		public event Action<int, string, List<TBCEnemyInfo>> TryGetTBCEnemyInfoEventCallback
		{
			add
			{
				this.TryGetTBCEnemyInfoEvent = (Action<int, string, List<TBCEnemyInfo>>)Delegate.Combine(this.TryGetTBCEnemyInfoEvent, value);
			}
			remove
			{
				this.TryGetTBCEnemyInfoEvent = (Action<int, string, List<TBCEnemyInfo>>)Delegate.Remove(this.TryGetTBCEnemyInfoEvent, value);
			}
		}

		public event Action<int, string> TryRestTBCEnemyInfoEventCallback
		{
			add
			{
				this.TryRestTBCEnemyInfoEvent = (Action<int, string>)Delegate.Combine(this.TryRestTBCEnemyInfoEvent, value);
			}
			remove
			{
				this.TryRestTBCEnemyInfoEvent = (Action<int, string>)Delegate.Remove(this.TryRestTBCEnemyInfoEvent, value);
			}
		}

		public event Action<int, string, List<TBCHeroStateInfo>, List<TBCHeroStateInfo>> TrySaveTBCEnemyInfoEventCallback
		{
			add
			{
				this.TrySaveTBCEnemyInfoEvent = (Action<int, string, List<TBCHeroStateInfo>, List<TBCHeroStateInfo>>)Delegate.Combine(this.TrySaveTBCEnemyInfoEvent, value);
			}
			remove
			{
				this.TrySaveTBCEnemyInfoEvent = (Action<int, string, List<TBCHeroStateInfo>, List<TBCHeroStateInfo>>)Delegate.Remove(this.TrySaveTBCEnemyInfoEvent, value);
			}
		}

		public event Action<int, string, List<TBCHeroStateInfo>> GetTBCMyHeroStateInfoEventCallback
		{
			add
			{
				this.GetTBCMyHeroStateInfoEvent = (Action<int, string, List<TBCHeroStateInfo>>)Delegate.Combine(this.GetTBCMyHeroStateInfoEvent, value);
			}
			remove
			{
				this.GetTBCMyHeroStateInfoEvent = (Action<int, string, List<TBCHeroStateInfo>>)Delegate.Remove(this.GetTBCMyHeroStateInfoEvent, value);
			}
		}

		public event Action<int, string, int> TryReceiveTBCRewardEventCallback
		{
			add
			{
				this.TryReceiveTBCRewardEvent = (Action<int, string, int>)Delegate.Combine(this.TryReceiveTBCRewardEvent, value);
			}
			remove
			{
				this.TryReceiveTBCRewardEvent = (Action<int, string, int>)Delegate.Remove(this.TryReceiveTBCRewardEvent, value);
			}
		}

		public event Action<int, string, string[]> TryUpdateArenaDefTeamEventCallback
		{
			add
			{
				this.TryUpdateArenaDefTeamEvent = (Action<int, string, string[]>)Delegate.Combine(this.TryUpdateArenaDefTeamEvent, value);
			}
			remove
			{
				this.TryUpdateArenaDefTeamEvent = (Action<int, string, string[]>)Delegate.Remove(this.TryUpdateArenaDefTeamEvent, value);
			}
		}

		public event Action<int, string, string[]> GetArenaDefTeamEventCallback
		{
			add
			{
				this.GetArenaDefTeamEvent = (Action<int, string, string[]>)Delegate.Combine(this.GetArenaDefTeamEvent, value);
			}
			remove
			{
				this.GetArenaDefTeamEvent = (Action<int, string, string[]>)Delegate.Remove(this.GetArenaDefTeamEvent, value);
			}
		}

		public event Action<int, string, List<ArenaData>> GetArenaEnemyInfoEventCallback
		{
			add
			{
				this.GetArenaEnemyInfoEvent = (Action<int, string, List<ArenaData>>)Delegate.Combine(this.GetArenaEnemyInfoEvent, value);
			}
			remove
			{
				this.GetArenaEnemyInfoEvent = (Action<int, string, List<ArenaData>>)Delegate.Remove(this.GetArenaEnemyInfoEvent, value);
			}
		}

		public event Action<int, string> TryArenaAtcCheckEventCallback
		{
			add
			{
				this.TryArenaAtcCheckEvent = (Action<int, string>)Delegate.Combine(this.TryArenaAtcCheckEvent, value);
			}
			remove
			{
				this.TryArenaAtcCheckEvent = (Action<int, string>)Delegate.Remove(this.TryArenaAtcCheckEvent, value);
			}
		}

		public event Action<int, string, long, ArenaAccount> TryUploadArenaAtcResultEventCallback
		{
			add
			{
				this.TryUploadArenaAtcResultEvent = (Action<int, string, long, ArenaAccount>)Delegate.Combine(this.TryUploadArenaAtcResultEvent, value);
			}
			remove
			{
				this.TryUploadArenaAtcResultEvent = (Action<int, string, long, ArenaAccount>)Delegate.Remove(this.TryUploadArenaAtcResultEvent, value);
			}
		}

		public event Action<int, string, List<ArenaLogData>> GetArenaFightLogEventCallback
		{
			add
			{
				this.GetArenaFightLogEvent = (Action<int, string, List<ArenaLogData>>)Delegate.Combine(this.GetArenaFightLogEvent, value);
			}
			remove
			{
				this.GetArenaFightLogEvent = (Action<int, string, List<ArenaLogData>>)Delegate.Remove(this.GetArenaFightLogEvent, value);
			}
		}

		public event Action<int, string, ArenaState> GetArenaStateEventCallback
		{
			add
			{
				this.GetArenaStateEvent = (Action<int, string, ArenaState>)Delegate.Combine(this.GetArenaStateEvent, value);
			}
			remove
			{
				this.GetArenaStateEvent = (Action<int, string, ArenaState>)Delegate.Remove(this.GetArenaStateEvent, value);
			}
		}

		public event Action<int, string> TryRestArenaCountEventCallback
		{
			add
			{
				this.TryRestArenaCountEvent = (Action<int, string>)Delegate.Combine(this.TryRestArenaCountEvent, value);
			}
			remove
			{
				this.TryRestArenaCountEvent = (Action<int, string>)Delegate.Remove(this.TryRestArenaCountEvent, value);
			}
		}

		public event Action<int, string, int> TryRestArenaCDTimeEventCallback
		{
			add
			{
				this.TryRestArenaCDTimeEvent = (Action<int, string, int>)Delegate.Combine(this.TryRestArenaCDTimeEvent, value);
			}
			remove
			{
				this.TryRestArenaCDTimeEvent = (Action<int, string, int>)Delegate.Remove(this.TryRestArenaCDTimeEvent, value);
			}
		}

		public event Action<int, string, List<ArenaData>> GetArenaRankListEventCallback
		{
			add
			{
				this.GetArenaRankListEvent = (Action<int, string, List<ArenaData>>)Delegate.Combine(this.GetArenaRankListEvent, value);
			}
			remove
			{
				this.GetArenaRankListEvent = (Action<int, string, List<ArenaData>>)Delegate.Remove(this.GetArenaRankListEvent, value);
			}
		}

		public event Action<int, string, List<ArenaData>> GetLastDayArenaRankListEventCallback
		{
			add
			{
				this.GetLastDayArenaRankListEvent = (Action<int, string, List<ArenaData>>)Delegate.Combine(this.GetLastDayArenaRankListEvent, value);
			}
			remove
			{
				this.GetLastDayArenaRankListEvent = (Action<int, string, List<ArenaData>>)Delegate.Combine(this.GetLastDayArenaRankListEvent, value);
			}
		}

		public event Action<int, string, ArenaData> GetMyArenaRankEventCallback
		{
			add
			{
				this.GetMyArenaRankEvent = (Action<int, string, ArenaData>)Delegate.Combine(this.GetMyArenaRankEvent, value);
			}
			remove
			{
				this.GetMyArenaRankEvent = (Action<int, string, ArenaData>)Delegate.Combine(this.GetMyArenaRankEvent, value);
			}
		}

		public event Action<int, string, LuckyDrawData> GetLuckyDrawStateEventCallback
		{
			add
			{
				this.GetLuckyDrawStateEvent = (Action<int, string, LuckyDrawData>)Delegate.Combine(this.GetLuckyDrawStateEvent, value);
			}
			remove
			{
				this.GetLuckyDrawStateEvent = (Action<int, string, LuckyDrawData>)Delegate.Remove(this.GetLuckyDrawStateEvent, value);
			}
		}

		public event Action<int, string, List<LuckyDrawResult>> TryLuckyDrawEventCallback
		{
			add
			{
				this.TryLuckyDrawEvent = (Action<int, string, List<LuckyDrawResult>>)Delegate.Combine(this.TryLuckyDrawEvent, value);
			}
			remove
			{
				this.TryLuckyDrawEvent = (Action<int, string, List<LuckyDrawResult>>)Delegate.Remove(this.TryLuckyDrawEvent, value);
			}
		}

		public event Action<int, string, ShopData> GetShopGoodsByShopTypeEventCallback
		{
			add
			{
				this.GetShopGoodsByShopTypeEvent = (Action<int, string, ShopData>)Delegate.Combine(this.GetShopGoodsByShopTypeEvent, value);
			}
			remove
			{
				this.GetShopGoodsByShopTypeEvent = (Action<int, string, ShopData>)Delegate.Remove(this.GetShopGoodsByShopTypeEvent, value);
			}
		}

		public event Action<int, string, int> TryBuyGoodsEventCallback
		{
			add
			{
				this.TryBuyGoodsEvent = (Action<int, string, int>)Delegate.Combine(this.TryBuyGoodsEvent, value);
			}
			remove
			{
				this.TryBuyGoodsEvent = (Action<int, string, int>)Delegate.Remove(this.TryBuyGoodsEvent, value);
			}
		}

		public event Action<int, string, ShopData> TryRestShopByShopTypeEventCallback
		{
			add
			{
				this.TryRestShopByShopTypeEvent = (Action<int, string, ShopData>)Delegate.Combine(this.TryRestShopByShopTypeEvent, value);
			}
			remove
			{
				this.TryRestShopByShopTypeEvent = (Action<int, string, ShopData>)Delegate.Remove(this.TryRestShopByShopTypeEvent, value);
			}
		}

		public event Action<int, string, List<TaskData>> GetTaskListEventCallback
		{
			add
			{
				this.GetTaskListEvent = (Action<int, string, List<TaskData>>)Delegate.Combine(this.GetTaskListEvent, value);
			}
			remove
			{
				this.GetTaskListEvent = (Action<int, string, List<TaskData>>)Delegate.Remove(this.GetTaskListEvent, value);
			}
		}

		public event Action<int, string, int> TryCompleteTaskEventCallback
		{
			add
			{
				this.TryCompleteTaskEvent = (Action<int, string, int>)Delegate.Combine(this.TryCompleteTaskEvent, value);
			}
			remove
			{
				this.TryCompleteTaskEvent = (Action<int, string, int>)Delegate.Remove(this.TryCompleteTaskEvent, value);
			}
		}

		public event Action<int, string, string> TryAttendanceByTypeEvnetCallback
		{
			add
			{
				this.TryAttendanceByTypeEvnet = (Action<int, string, string>)Delegate.Combine(this.TryAttendanceByTypeEvnet, value);
			}
			remove
			{
				this.TryAttendanceByTypeEvnet = (Action<int, string, string>)Delegate.Remove(this.TryAttendanceByTypeEvnet, value);
			}
		}

		public event Action<int, string, int> TryModfiyNickNameEventCallback
		{
			add
			{
				this.TryModfiyNickNameEvent = (Action<int, string, int>)Delegate.Combine(this.TryModfiyNickNameEvent, value);
			}
			remove
			{
				this.TryModfiyNickNameEvent = (Action<int, string, int>)Delegate.Remove(this.TryModfiyNickNameEvent, value);
			}
		}

		public event Action<int, string> TryModfiyIconEventCallback
		{
			add
			{
				this.TryModfiyIconEvent = (Action<int, string>)Delegate.Combine(this.TryModfiyIconEvent, value);
			}
			remove
			{
				this.TryModfiyIconEvent = (Action<int, string>)Delegate.Remove(this.TryModfiyIconEvent, value);
			}
		}

		public event Action<int, string, int> TrySellPropsEventCallback
		{
			add
			{
				this.TrySellPropsEvent = (Action<int, string, int>)Delegate.Combine(this.TrySellPropsEvent, value);
			}
			remove
			{
				this.TrySellPropsEvent = (Action<int, string, int>)Delegate.Remove(this.TrySellPropsEvent, value);
			}
		}

		public event Action<int, string, List<ExchangeData>> TryExchangeByDimondEventCallback
		{
			add
			{
				this.TryExchangeByDimondEvent = (Action<int, string, List<ExchangeData>>)Delegate.Combine(this.TryExchangeByDimondEvent, value);
			}
			remove
			{
				this.TryExchangeByDimondEvent = (Action<int, string, List<ExchangeData>>)Delegate.Remove(this.TryExchangeByDimondEvent, value);
			}
		}

		public event Action<int, string, List<MailData>> GetMailListEventCallback
		{
			add
			{
				this.GetMailListEvent = (Action<int, string, List<MailData>>)Delegate.Combine(this.GetMailListEvent, value);
			}
			remove
			{
				this.GetMailListEvent = (Action<int, string, List<MailData>>)Delegate.Remove(this.GetMailListEvent, value);
			}
		}

		public event Action<int, string, List<RewardModel>> TryReceiveMailAttachmentEventCallback
		{
			add
			{
				this.TryReceiveMailAttachmentEvent = (Action<int, string, List<RewardModel>>)Delegate.Combine(this.TryReceiveMailAttachmentEvent, value);
			}
			remove
			{
				this.TryReceiveMailAttachmentEvent = (Action<int, string, List<RewardModel>>)Delegate.Remove(this.TryReceiveMailAttachmentEvent, value);
			}
		}

		public event Action<int, string, int> TryCreateUnionEventCallback
		{
			add
			{
				this.TryCreateUnionEvent = (Action<int, string, int>)Delegate.Combine(this.TryCreateUnionEvent, value);
			}
			remove
			{
				this.TryCreateUnionEvent = (Action<int, string, int>)Delegate.Remove(this.TryCreateUnionEvent, value);
			}
		}

		public event Action<int, string, UnionInfoData> GetUnionInfoEventCallback
		{
			add
			{
				this.GetUnionInfoEvent = (Action<int, string, UnionInfoData>)Delegate.Combine(this.GetUnionInfoEvent, value);
			}
			remove
			{
				this.GetUnionInfoEvent = (Action<int, string, UnionInfoData>)Delegate.Remove(this.GetUnionInfoEvent, value);
			}
		}

		public event Action<int, string> TryDissolveUnionEventCallback
		{
			add
			{
				this.TryDissolveUnionEvent = (Action<int, string>)Delegate.Combine(this.TryDissolveUnionEvent, value);
			}
			remove
			{
				this.TryDissolveUnionEvent = (Action<int, string>)Delegate.Remove(this.TryDissolveUnionEvent, value);
			}
		}

		public event Action<int, string, UnionInfoData> TrySearchUnionEventCallback
		{
			add
			{
				this.TrySearchUnionEvent = (Action<int, string, UnionInfoData>)Delegate.Combine(this.TrySearchUnionEvent, value);
			}
			remove
			{
				this.TrySearchUnionEvent = (Action<int, string, UnionInfoData>)Delegate.Remove(this.TrySearchUnionEvent, value);
			}
		}

		public event Action<int, string> TryJoinUnionEventCallback
		{
			add
			{
				this.TryJoinUnionEvent = (Action<int, string>)Delegate.Combine(this.TryJoinUnionEvent, value);
			}
			remove
			{
				this.TryJoinUnionEvent = (Action<int, string>)Delegate.Remove(this.TryJoinUnionEvent, value);
			}
		}

		public event Action<int, string, List<UnionMemberData>> TryKickUnionEventCallback
		{
			add
			{
				this.TryKickUnionEvent = (Action<int, string, List<UnionMemberData>>)Delegate.Combine(this.TryKickUnionEvent, value);
			}
			remove
			{
				this.TryKickUnionEvent = (Action<int, string, List<UnionMemberData>>)Delegate.Remove(this.TryKickUnionEvent, value);
			}
		}

		public event Action<int, string> TryLeaveUnionEventCallback
		{
			add
			{
				this.TryLeaveUnionEvent = (Action<int, string>)Delegate.Combine(this.TryLeaveUnionEvent, value);
			}
			remove
			{
				this.TryLeaveUnionEvent = (Action<int, string>)Delegate.Remove(this.TryLeaveUnionEvent, value);
			}
		}

		public event Action<int, string, UnionInfoData> TryModifyUnionSettingEventCallback
		{
			add
			{
				this.TryModifyUnionSettingEvent = (Action<int, string, UnionInfoData>)Delegate.Combine(this.TryModifyUnionSettingEvent, value);
			}
			remove
			{
				this.TryModifyUnionSettingEvent = (Action<int, string, UnionInfoData>)Delegate.Remove(this.TryModifyUnionSettingEvent, value);
			}
		}

		public event Action<int, string> TryModifyAnnouncementEventCallback
		{
			add
			{
				this.TryModifyAnnouncementEvent = (Action<int, string>)Delegate.Combine(this.TryModifyAnnouncementEvent, value);
			}
			remove
			{
				this.TryModifyAnnouncementEvent = (Action<int, string>)Delegate.Remove(this.TryModifyAnnouncementEvent, value);
			}
		}

		public event Action<int, string, List<UnionInfoData>> GetUnionListEventCallback
		{
			add
			{
				this.GetUnionListEvent = (Action<int, string, List<UnionInfoData>>)Delegate.Combine(this.GetUnionListEvent, value);
			}
			remove
			{
				this.GetUnionListEvent = (Action<int, string, List<UnionInfoData>>)Delegate.Remove(this.GetUnionListEvent, value);
			}
		}

		public event Action<int, string, List<UnionLogData>> GetUnionLogsEventCallback
		{
			add
			{
				this.GetUnionLogsEvent = (Action<int, string, List<UnionLogData>>)Delegate.Combine(this.GetUnionLogsEvent, value);
			}
			remove
			{
				this.GetUnionLogsEvent = (Action<int, string, List<UnionLogData>>)Delegate.Remove(this.GetUnionLogsEvent, value);
			}
		}

		public event Action<int, string, List<UnionMemberData>> GetMemberListEventCallback
		{
			add
			{
				this.GetMemberListEvent = (Action<int, string, List<UnionMemberData>>)Delegate.Combine(this.GetMemberListEvent, value);
			}
			remove
			{
				this.GetMemberListEvent = (Action<int, string, List<UnionMemberData>>)Delegate.Remove(this.GetMemberListEvent, value);
			}
		}

		public event Action<int, string, string> TryUpgradeMasterEventCallback
		{
			add
			{
				this.TryUpgradeMasterEvent = (Action<int, string, string>)Delegate.Combine(this.TryUpgradeMasterEvent, value);
			}
			remove
			{
				this.TryUpgradeMasterEvent = (Action<int, string, string>)Delegate.Remove(this.TryUpgradeMasterEvent, value);
			}
		}

		public event Action<int, string, string> TryAppointElderEventCallback
		{
			add
			{
				this.TryAppointElderEvent = (Action<int, string, string>)Delegate.Combine(this.TryAppointElderEvent, value);
			}
			remove
			{
				this.TryAppointElderEvent = (Action<int, string, string>)Delegate.Remove(this.TryAppointElderEvent, value);
			}
		}

		public event Action<int, string, string> TryUnAppointElderEventCallback
		{
			add
			{
				this.TryUnAppointElderEvent = (Action<int, string, string>)Delegate.Combine(this.TryUnAppointElderEvent, value);
			}
			remove
			{
				this.TryUnAppointElderEvent = (Action<int, string, string>)Delegate.Remove(this.TryUnAppointElderEvent, value);
			}
		}

		public event Action<int, string, List<UnionMemberData>> GetUnionRequestListEventCallback
		{
			add
			{
				this.GetUnionRequestListEvent = (Action<int, string, List<UnionMemberData>>)Delegate.Combine(this.GetUnionRequestListEvent, value);
			}
			remove
			{
				this.GetUnionRequestListEvent = (Action<int, string, List<UnionMemberData>>)Delegate.Remove(this.GetUnionRequestListEvent, value);
			}
		}

		public event Action<int, string, string> TryDisposeUnionReqEventCallback
		{
			add
			{
				this.TryDisposeUnionReqEvent = (Action<int, string, string>)Delegate.Combine(this.TryDisposeUnionReqEvent, value);
			}
			remove
			{
				this.TryDisposeUnionReqEvent = (Action<int, string, string>)Delegate.Remove(this.TryDisposeUnionReqEvent, value);
			}
		}

		public event Action<int, string, string> TryEnchantEquipmentEventCallback
		{
			add
			{
				this.TryEnchantEquipmentEvent = (Action<int, string, string>)Delegate.Combine(this.TryEnchantEquipmentEvent, value);
			}
			remove
			{
				this.TryEnchantEquipmentEvent = (Action<int, string, string>)Delegate.Remove(this.TryEnchantEquipmentEvent, value);
			}
		}

		public event Action<int, string, List<SweepData>> TrySweepBattleEventCallback
		{
			add
			{
				this.TrySweepBattleEvent = (Action<int, string, List<SweepData>>)Delegate.Combine(this.TrySweepBattleEvent, value);
			}
			remove
			{
				this.TrySweepBattleEvent = (Action<int, string, List<SweepData>>)Delegate.Remove(this.TrySweepBattleEvent, value);
			}
		}

		public event Action<int, string, long> TryModifyEmailStateEventCallback
		{
			add
			{
				this.TryModifyEmailStateEvent = (Action<int, string, long>)Delegate.Combine(this.TryModifyEmailStateEvent, value);
			}
			remove
			{
				this.TryModifyEmailStateEvent = (Action<int, string, long>)Delegate.Remove(this.TryModifyEmailStateEvent, value);
			}
		}

		public event Action<int, string> TryUpdateSmallMeleeTeamEventCallback
		{
			add
			{
				this.TryUpdateSmallMeleeTeamEvent = (Action<int, string>)Delegate.Combine(this.TryUpdateSmallMeleeTeamEvent, value);
			}
			remove
			{
				this.TryUpdateSmallMeleeTeamEvent = (Action<int, string>)Delegate.Remove(this.TryUpdateSmallMeleeTeamEvent, value);
			}
		}

		public event Action<int, string, SmallMeleeData> GetSmallMeleeInfoEventCallback
		{
			add
			{
				this.GetSmallMeleeInfoEvent = (Action<int, string, SmallMeleeData>)Delegate.Combine(this.GetSmallMeleeInfoEvent, value);
			}
			remove
			{
				this.GetSmallMeleeInfoEvent = (Action<int, string, SmallMeleeData>)Delegate.Remove(this.GetSmallMeleeInfoEvent, value);
			}
		}

		public event Action<int, string, List<TalentModel>> GetTalentInfoByUserIdEventCallback
		{
			add
			{
				this.GetTalentInfoByUserIdEvent = (Action<int, string, List<TalentModel>>)Delegate.Combine(this.GetTalentInfoByUserIdEvent, value);
			}
			remove
			{
				this.GetTalentInfoByUserIdEvent = (Action<int, string, List<TalentModel>>)Delegate.Remove(this.GetTalentInfoByUserIdEvent, value);
			}
		}

		public event Action<int, string, KillTitanData> TryChangeKillTitanTeamEventCallback
		{
			add
			{
				this.TryChangeKillTitanTeamEvent = (Action<int, string, KillTitanData>)Delegate.Combine(this.TryChangeKillTitanTeamEvent, value);
			}
			remove
			{
				this.TryChangeKillTitanTeamEvent = (Action<int, string, KillTitanData>)Delegate.Remove(this.TryChangeKillTitanTeamEvent, value);
			}
		}

		public event Action<int, string, KillTitanData> GetKillTitanInfoEventCallback
		{
			add
			{
				this.GetKillTitanInfoEvent = (Action<int, string, KillTitanData>)Delegate.Combine(this.GetKillTitanInfoEvent, value);
			}
			remove
			{
				this.GetKillTitanInfoEvent = (Action<int, string, KillTitanData>)Delegate.Remove(this.GetKillTitanInfoEvent, value);
			}
		}

		public event Action<int, string, KillTitanReward> TryReceiveKillTitanRewardEventCallback
		{
			add
			{
				this.TryReceiveKillTitanRewardEvent = (Action<int, string, KillTitanReward>)Delegate.Combine(this.TryReceiveKillTitanRewardEvent, value);
			}
			remove
			{
				this.TryReceiveKillTitanRewardEvent = (Action<int, string, KillTitanReward>)Delegate.Remove(this.TryReceiveKillTitanRewardEvent, value);
			}
		}

		public event Action<int, string, List<KillTitanSnapshotData>> TryAddKillTitanSnapshotEventCallback
		{
			add
			{
				this.TryAddKillTitanSnapshotEvent = (Action<int, string, List<KillTitanSnapshotData>>)Delegate.Combine(this.TryAddKillTitanSnapshotEvent, value);
			}
			remove
			{
				this.TryAddKillTitanSnapshotEvent = (Action<int, string, List<KillTitanSnapshotData>>)Delegate.Remove(this.TryAddKillTitanSnapshotEvent, value);
			}
		}

		public event Action<int, string, List<KillTitanSnapshotData>> GetKillTitanSnapshotEventCallback
		{
			add
			{
				this.GetKillTitanSnapshotEvent = (Action<int, string, List<KillTitanSnapshotData>>)Delegate.Combine(this.GetKillTitanSnapshotEvent, value);
			}
			remove
			{
				this.GetKillTitanSnapshotEvent = (Action<int, string, List<KillTitanSnapshotData>>)Delegate.Remove(this.GetKillTitanSnapshotEvent, value);
			}
		}

		public event Action<int, string, List<AwardInfoData>> TryReceiveSnapshotAwardEventCallback
		{
			add
			{
				this.TryReceiveSnapshotAwardEvent = (Action<int, string, List<AwardInfoData>>)Delegate.Combine(this.TryReceiveSnapshotAwardEvent, value);
			}
			remove
			{
				this.TryReceiveSnapshotAwardEvent = (Action<int, string, List<AwardInfoData>>)Delegate.Remove(this.TryReceiveSnapshotAwardEvent, value);
			}
		}

		public event Action<int, string> TryBuySpecialShopOwnEventCallback
		{
			add
			{
				this.TryBuySpecialShopOwnEvent = (Action<int, string>)Delegate.Combine(this.TryBuySpecialShopOwnEvent, value);
			}
			remove
			{
				this.TryBuySpecialShopOwnEvent = (Action<int, string>)Delegate.Remove(this.TryBuySpecialShopOwnEvent, value);
			}
		}

		public event Action<int, string, int> TryRestTodayBattlesCountEventCallback
		{
			add
			{
				this.TryRestTodayBattlesCountEvent = (Action<int, string, int>)Delegate.Combine(this.TryRestTodayBattlesCountEvent, value);
			}
			remove
			{
				this.TryRestTodayBattlesCountEvent = (Action<int, string, int>)Delegate.Remove(this.TryRestTodayBattlesCountEvent, value);
			}
		}

		public event Action<int, int> TryJoinPvpEventCallback
		{
			add
			{
				this.TryJoinPvpEvent = (Action<int, int>)Delegate.Combine(this.TryJoinPvpEvent, value);
			}
			remove
			{
				this.TryJoinPvpEvent = (Action<int, int>)Delegate.Remove(this.TryJoinPvpEvent, value);
			}
		}

		public event Action<int, string, FriendData> TryApplyAddFriendEventCallback
		{
			add
			{
				this.TryApplyAddFriendEvent = (Action<int, string, FriendData>)Delegate.Combine(this.TryApplyAddFriendEvent, value);
			}
			remove
			{
				this.TryApplyAddFriendEvent = (Action<int, string, FriendData>)Delegate.Remove(this.TryApplyAddFriendEvent, value);
			}
		}

		public event Action<int, string, List<FriendData>> GetFriendListEventCallback
		{
			add
			{
				this.GetFriendListEvent = (Action<int, string, List<FriendData>>)Delegate.Combine(this.GetFriendListEvent, value);
			}
			remove
			{
				this.GetFriendListEvent = (Action<int, string, List<FriendData>>)Delegate.Remove(this.GetFriendListEvent, value);
			}
		}

		public event Action<int, string, byte, long> TryModifyFriendStatusEventCallback
		{
			add
			{
				this.TryModifyFriendStatusEvent = (Action<int, string, byte, long>)Delegate.Combine(this.TryModifyFriendStatusEvent, value);
			}
			remove
			{
				this.TryModifyFriendStatusEvent = (Action<int, string, byte, long>)Delegate.Remove(this.TryModifyFriendStatusEvent, value);
			}
		}

		public event Action<int, string, List<NotificationData>> GetFriendsMessagesEventCallback
		{
			add
			{
				this.GetFriendsMessagesEvent = (Action<int, string, List<NotificationData>>)Delegate.Combine(this.GetFriendsMessagesEvent, value);
			}
			remove
			{
				this.GetFriendsMessagesEvent = (Action<int, string, List<NotificationData>>)Delegate.Remove(this.GetFriendsMessagesEvent, value);
			}
		}

		public event Action<int, string, string[]> GetUserInfoBySummIdEventCallback
		{
			add
			{
				this.GetUserInfoBySummIdEvent = (Action<int, string, string[]>)Delegate.Combine(this.GetUserInfoBySummIdEvent, value);
			}
			remove
			{
				this.GetUserInfoBySummIdEvent = (Action<int, string, string[]>)Delegate.Remove(this.GetUserInfoBySummIdEvent, value);
			}
		}

		public event Action<int, string, List<bool>> GetMedalDataByUserIdEventCallback
		{
			add
			{
				this.GetMedalDataByUserIdEvent = (Action<int, string, List<bool>>)Delegate.Combine(this.GetMedalDataByUserIdEvent, value);
			}
			remove
			{
				this.GetMedalDataByUserIdEvent = (Action<int, string, List<bool>>)Delegate.Remove(this.GetMedalDataByUserIdEvent, value);
			}
		}

		public event Action<int, string, int, bool> NoticeMedalEventCallback
		{
			add
			{
				this.NoticeMedalEvent = (Action<int, string, int, bool>)Delegate.Combine(this.NoticeMedalEvent, value);
			}
			remove
			{
				this.NoticeMedalEvent = (Action<int, string, int, bool>)Delegate.Remove(this.NoticeMedalEvent, value);
			}
		}

		public event Action<int, string> TryBuySkinEventCallback
		{
			add
			{
				this.TryBuySkinEvent = (Action<int, string>)Delegate.Combine(this.TryBuySkinEvent, value);
			}
			remove
			{
				this.TryBuySkinEvent = (Action<int, string>)Delegate.Remove(this.TryBuySkinEvent, value);
			}
		}

		public event Action<int, string> TryChangeSkinEventCallback
		{
			add
			{
				this.TryChangeSkinEvent = (Action<int, string>)Delegate.Combine(this.TryChangeSkinEvent, value);
			}
			remove
			{
				this.TryChangeSkinEvent = (Action<int, string>)Delegate.Remove(this.TryChangeSkinEvent, value);
			}
		}

		private event Action<int, string, ChatMessage> TryReciveMessageEvent;

		public event Action<int, string, ChatMessage> TryReciveMessageEventCallback
		{
			add
			{
				this.TryReciveMessageEvent = (Action<int, string, ChatMessage>)Delegate.Combine(this.TryReciveMessageEvent, value);
			}
			remove
			{
				this.TryReciveMessageEvent = (Action<int, string, ChatMessage>)Delegate.Remove(this.TryReciveMessageEvent, value);
			}
		}

		public MobaMasterClientPeer master_peer
		{
			get
			{
				return this.GetMobaPeer(MobaPeerType.C2Master) as MobaMasterClientPeer;
			}
		}

		public MobaLobbyClientPeer lobby_peer
		{
			get
			{
				return this.GetMobaPeer(MobaPeerType.C2Lobby) as MobaLobbyClientPeer;
			}
		}

		public MobaPvpServerClientPeer pvpserver_peer
		{
			get
			{
				return this.GetMobaPeer(MobaPeerType.C2PvpServer) as MobaPvpServerClientPeer;
			}
		}

		public MobaGateClientPeer gate_peer
		{
			get
			{
				return this.GetMobaPeer(MobaPeerType.C2GateServer) as MobaGateClientPeer;
			}
		}

		public string recvLobbyIp
		{
			get;
			set;
		}

		public int recvLobbyPort
		{
			get;
			set;
		}

		public string recvLobbyName
		{
			get;
			set;
		}

		public PhotonClient()
		{
			Log.debug("==> new PhotonClient...");
			this.m_user = new User();
			this.udpDriver = new UdpDriver(this, this.pvpserver_peer);
			this.timeSyncSystem = new TimeSyncSystem(this, this.pvpserver_peer);
		}

		public void PostService()
		{
			this.udpDriver.SendAllAck();
			this.udpDriver.TryProcessAvailablePkg();
		}

		public MobaPeer GetMobaPeer(MobaPeerType peerType)
		{
			return this.GetMobaPeer(peerType, false);
		}

		public MobaPeer GetMobaPeer(MobaPeerType peerType, bool isCreate)
		{
			MobaPeer result;
			if (isCreate)
			{
				if (!this.mPeerPool.ContainsKey(peerType))
				{
					this.mPeerPool[peerType] = this.CreateMobaPeer(peerType);
					this.mPeerPool[peerType].ChannelCount = 254;
				}
				result = this.mPeerPool[peerType];
			}
			else if (this.mPeerPool.ContainsKey(peerType))
			{
				result = this.mPeerPool[peerType];
			}
			else
			{
				result = null;
			}
			return result;
		}

		private MobaPeer CreateMobaPeer(MobaPeerType peerType)
		{
			MobaPeer mobaPeer = null;
			switch (peerType)
			{
			case MobaPeerType.C2Master:
				mobaPeer = new MobaMasterClientPeer(ConnectionProtocol.Tcp, this);
				break;
			case MobaPeerType.C2Lobby:
				mobaPeer = new MobaLobbyClientPeer(ConnectionProtocol.Udp, this);
				break;
			case MobaPeerType.C2PvpServer:
				mobaPeer = new MobaPvpServerClientPeer(ConnectionProtocol.Udp, this);
				this.udpDriver.pvpPeer = mobaPeer;
				this.timeSyncSystem.pvpPeer = mobaPeer;
				break;
			case MobaPeerType.C2GateServer:
				mobaPeer = new MobaGateClientPeer(ConnectionProtocol.Udp, this);
				break;
			case MobaPeerType.C2LoginServer:
				mobaPeer = new MobaLoginClientPeer(ConnectionProtocol.Udp, this);
				break;
			}
			return mobaPeer;
		}

		public bool MainPeerConnect(MobaPeerType peerType, string serverName, string appName)
		{
			MobaPeer mobaPeer = this.GetMobaPeer(peerType, true);
			mobaPeer.ServerName = serverName;
			mobaPeer.ApplicationName = appName;
			if (this.mCurrMainPeer == null)
			{
				this.mLastMainPeer = null;
				this.mCurrMainPeer = mobaPeer;
			}
			else if (this.mCurrMainPeer.PeerType == peerType)
			{
				this.mLastMainPeer = null;
				Log.error("Connect Main Peer [" + peerType + "] maybe impossible because you try to connect the same peer!!");
			}
			else
			{
				this.mLastMainPeer = this.mCurrMainPeer;
				this.mCurrMainPeer = mobaPeer;
			}
			bool result;
			if (!this.mCurrMainPeer.ServerConnected)
			{
				result = mobaPeer.PeerConnect();
			}
			else
			{
				Log.error("Connect Main Peer [" + peerType + "] is impossible because it is always connected!!");
				result = false;
			}
			return result;
		}

		public void OnMainPeerConnected(MobaPeerType peerType)
		{
			if (this.mCurrMainPeer != null && this.mCurrMainPeer.PeerType == peerType)
			{
				Log.debug("==> [" + peerType + "] is  connected as main peer!!");
				if (this.mLastMainPeer != null)
				{
					this.mLastMainPeer.PeerDisconnect();
				}
			}
		}

		public void MainPeerUpdate()
		{
			if (this.mCurrMainPeer != null)
			{
				this.mCurrMainPeer.PeerUpdate();
			}
		}

		public void OnUpdate()
		{
			lock (this.sendThreadLock)
			{
				for (int i = 0; i < this.cmdCacheList.Count; i++)
				{
					CmdPkgForThread cmdPkgForThread = this.cmdCacheList[i];
					MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2PvpServer);
					if (this.NeedUnrelivable(cmdPkgForThread.code))
					{
						if (this.udpDriver != null && mobaPeer != null)
						{
							this.udpDriver.Send(cmdPkgForThread.code, cmdPkgForThread.param);
						}
					}
					else if (mobaPeer != null)
					{
						mobaPeer.OpCustom((byte)cmdPkgForThread.code, cmdPkgForThread.param, true, 0, mobaPeer.IsEncryptionAvailable);
					}
					this.ReleaseCmdPkg(cmdPkgForThread);
				}
			}
			this.cmdCacheList.Clear();
			if (this.udpDriver != null)
			{
				this.udpDriver.OnUpdate();
			}
			if (this.timeSyncSystem != null)
			{
				this.timeSyncSystem.Update();
			}
		}

		public MobaPeer GetMainPeer()
		{
			return this.mCurrMainPeer;
		}

		public bool AssistancPeerConnect(MobaPeerType peerType, string serverName, string appName)
		{
			MobaPeer mobaPeer = this.GetMobaPeer(peerType, true);
			mobaPeer.ServerName = serverName;
			mobaPeer.ApplicationName = appName;
			bool result;
			if (this.mCurrMainPeer == mobaPeer)
			{
				Log.error("Connect Assistance Peer [" + peerType + "] is impossible because the peer is main peer");
				result = false;
			}
			else
			{
				bool flag = false;
				foreach (MobaPeer current in this.mAssistantPeerList)
				{
					if (current == mobaPeer)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					Log.error("Connect Assistance Peer [" + peerType + "] is impossible because the peer is already in assistant peer");
					result = false;
				}
				else
				{
					this.mAssistantPeerList.Add(mobaPeer);
					if (!mobaPeer.ServerConnected)
					{
						result = mobaPeer.PeerConnect();
					}
					else
					{
						Log.error("Connect Assistance Peer [" + peerType + "] is impossible because the peer is already connected");
						result = false;
					}
				}
			}
			return result;
		}

		public void AssistancPeerDisconnect(MobaPeerType peerType)
		{
			MobaPeer mobaPeer = this.GetMobaPeer(peerType);
			if (mobaPeer != null)
			{
				if (this.mAssistantPeerList.Contains(mobaPeer))
				{
					mobaPeer.PeerDisconnect();
					this.mAssistantPeerList.Remove(mobaPeer);
				}
			}
		}

		public MobaPeer GetAssistantPeer(MobaPeerType peerType)
		{
			MobaPeer mobaPeer = this.GetMobaPeer(peerType);
			MobaPeer result;
			if (mobaPeer == null)
			{
				result = null;
			}
			else if (this.mAssistantPeerList.Contains(mobaPeer))
			{
				result = mobaPeer;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public bool MobaPeerConnect(MobaPeerType peerType, bool isCreate)
		{
			MobaPeer mobaPeer = this.GetMobaPeer(peerType, isCreate);
			return mobaPeer != null && mobaPeer.PeerConnect();
		}

		public void MobaPeerDisconnect(MobaPeerType peerType)
		{
			MobaPeer mobaPeer = this.GetMobaPeer(peerType);
			if (mobaPeer != null)
			{
				mobaPeer.PeerDisconnect();
			}
		}

		public void OnDisconnected()
		{
			this.udpDriver.OnDisconnect();
		}

		public bool ClientConnectToMaster()
		{
			this.GetMasterIP();
			string serverName = GlobalManager.masterIpForce + ":8080";
			string appName = "MobaServer.Master";
			return this.MainPeerConnect(MobaPeerType.C2Master, serverName, appName);
		}

		private void GetMasterIP()
		{
			if (string.IsNullOrEmpty(GlobalManager.masterIpForce))
			{
				GlobalManager.masterIpForce = "mobaapp.xiaomeng.cc";
			}
		}

		public bool ClientConnectToGate(string serverIP, int serverPort, string appName)
		{
			Log.debug("==> MobaClient：ConnectToGate ...");
			return this.MainPeerConnect(MobaPeerType.C2GateServer, serverIP + ":" + serverPort, appName);
		}

		public bool ClientConnectToLobby()
		{
			Log.debug("==> MobaClient：ConnectToLobby ...");
			MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2Lobby, true);
			mobaPeer.ServerName = this.recvLobbyIp + ":" + this.recvLobbyPort;
			mobaPeer.ApplicationName = this.recvLobbyName;
			return mobaPeer.PeerConnect();
		}

		public bool ClientConnectToPVP(string serverIP, int serverPort, string appName)
		{
			Log.debug("==> MobaClient：ConnectToPVP ...");
			return this.MainPeerConnect(MobaPeerType.C2PvpServer, serverIP + ":" + serverPort, appName);
		}

		public bool GuestLogin()
		{
			bool result = false;
			try
			{
				Log.debug("==> PhotonClient ：游客登录 ");
				MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2Master);
				result = mobaPeer.OpCustom(137, null, true);
			}
			catch (Exception ex)
			{
				result = false;
				Log.error("GuestLogin Error : " + ex.Message);
			}
			return result;
		}

		public void Register(AccountData userdata)
		{
			try
			{
				Log.debug("==> PhotonClient ： 注册用户 ");
				userdata.DeviceToken = "Unknow";
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary[85] = SerializeHelper.Serialize<AccountData>(userdata);
				MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2Master);
				mobaPeer.OpCustom(10, dictionary, true);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public bool Register()
		{
			bool result = false;
			try
			{
				Log.debug("==> PhotonClient ： 注册用户 ");
				MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2Master);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary[85] = SerializeHelper.Serialize<AccountData>(this.m_user.AccountData);
				result = mobaPeer.OpCustom(10, dictionary, true);
			}
			catch (Exception ex)
			{
				result = false;
				Log.error("Register Error : " + ex.Message);
			}
			return result;
		}

		public bool BindChannelId(string channelid, long accountid)
		{
			bool result = false;
			try
			{
				Log.debug("==> MobaClient ： 绑定渠道ID ");
				MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2Master);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary[78] = channelid;
				dictionary[71] = accountid;
				result = mobaPeer.OpCustom(152, dictionary, true);
			}
			catch (Exception ex)
			{
				result = false;
				Log.error("BindChannelId Error : " + ex.Message);
			}
			return result;
		}

		public bool LoginByChannelId(string channelid)
		{
			bool result = false;
			try
			{
				Log.debug("==> MobaClient ： 渠道ID登录 ");
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary[78] = channelid;
				MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2Master);
				result = mobaPeer.OpCustom(6, dictionary, true);
			}
			catch (Exception ex)
			{
				result = false;
				Log.error("LoginByChannelId Error : " + ex.Message);
			}
			return result;
		}

		private bool Login(AccountData userdata)
		{
			bool result = false;
			try
			{
				if (userdata != null)
				{
					Log.debug(" PhotonClient ： 用户登录（第一次登陆） ");
					MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2Master);
					Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
					dictionary[72] = userdata.UserName;
					dictionary[73] = userdata.Mail;
					dictionary[74] = userdata.Password;
					dictionary[75] = userdata.DeviceType;
					dictionary[76] = userdata.DeviceToken;
					dictionary[77] = userdata.UserType;
					dictionary[76] = userdata.DeviceToken;
					dictionary[53] = 0;
					result = mobaPeer.OpCustom(1, dictionary, true, 0, mobaPeer.IsEncryptionAvailable);
				}
			}
			catch (Exception ex)
			{
				result = false;
				Log.error("Login Error : " + ex.Message);
			}
			return result;
		}

		public bool Login()
		{
			bool result = false;
			try
			{
				if (this.m_user != null && this.m_user.AccountData != null)
				{
					Log.debug(" MobaClient ： 用户登录（使用上一次用户信息） ");
					MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2Master);
					AccountData accountData = this.m_user.AccountData;
					Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
					dictionary[85] = SerializeHelper.Serialize<AccountData>(accountData);
					result = mobaPeer.OpCustom(1, dictionary, true, 0, mobaPeer.IsEncryptionAvailable);
				}
			}
			catch (Exception ex)
			{
				result = false;
				Log.error("Login Error : " + ex.Message);
			}
			return result;
		}

		public bool CheckUpgrade(ClientData data)
		{
			bool result = false;
			try
			{
				Log.debug(" MobaClient ： CheckUpgrade...");
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				byte[] value = SerializeHelper.Serialize<ClientData>(data);
				dictionary[84] = value;
				MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2Master);
				result = mobaPeer.OpCustom(141, dictionary, true);
			}
			catch (Exception ex)
			{
				result = false;
				Log.error(" MobaClient Error : " + ex.Message);
			}
			return result;
		}

		public bool CheckResUpgrade(ClientData data)
		{
			bool result = false;
			try
			{
				Log.debug(" MobaClient ： CheckResUpgrade...");
				MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2Master);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				byte[] value = SerializeHelper.Serialize<ClientData>(data);
				dictionary[84] = value;
				result = mobaPeer.OpCustom(158, dictionary, true);
			}
			catch (Exception ex)
			{
				result = false;
				Log.error(" CheckResUpgrade Error : " + ex.Message);
			}
			return result;
		}

		public void SystemNotice()
		{
			try
			{
				MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2Master);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary[57] = "";
				mobaPeer.OpCustom(138, dictionary, true);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void SystemSetting()
		{
			try
			{
				MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2Master);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary[57] = this.m_user.UserData.UserId;
				mobaPeer.OpCustom(136, dictionary, true);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void QueryUserById(string userId)
		{
			try
			{
				MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2Master);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary[57] = userId;
				mobaPeer.OpCustom(135, dictionary, true);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void GetServerList()
		{
			try
			{
				Log.debug(" PhotonClient ： 获取服务器列表... ");
				MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2Master);
				mobaPeer.OpCustom(101, null, true);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void AutoLogin()
		{
			Log.debug(" PhotonClient ： AutoLogin... ");
		}

		private static Dictionary<byte, object> BuildParamsDic(object[] args)
		{
			Dictionary<byte, object> result;
			if (args == null || args.Length <= 0)
			{
				result = null;
			}
			else
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				byte b = 0;
				while ((int)b < args.Length)
				{
					dictionary[b] = args[(int)b];
					b += 1;
				}
				result = dictionary;
			}
			return result;
		}

		public bool SendGateChannelMessage(MobaPeerType peerType, MobaChannel cmdChannel, byte code, Dictionary<byte, object> args)
		{
			bool result = false;
			try
			{
				try
				{
					MobaPeer mobaPeer = this.GetMobaPeer(peerType);
					result = mobaPeer.OpCustom(code, args, true, (byte)cmdChannel, mobaPeer.IsEncryptionAvailable);
				}
				finally
				{
					PhotonClient.ReleaseParam(args);
				}
			}
			catch (Exception ex)
			{
				result = false;
				Debug.LogError("SendGsChannelMessage Error : " + code + ex.Message);
			}
			return result;
		}

		public bool SendGateChannelMessagePM(MobaPeerType peerType, MobaChannel cmdChannel, byte code, params object[] args)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (args != null)
			{
				int num = Math.Min(args.Length, 21);
				byte b = 0;
				while ((int)b < args.Length)
				{
					byte b2 = 100;
					b2 += b;
					dictionary.Add(b2, args[(int)b]);
					b += 1;
				}
			}
			bool flag = this.SendGateChannelMessage(peerType, cmdChannel, code, dictionary);
			if (!flag)
			{
				Debug.LogError("SendLobbyMsg failed for " + code);
			}
			return flag;
		}

		public bool SendGsChannelMessage(MobaChannel cmdChannel, byte code, params object[] args)
		{
			bool result = false;
			try
			{
				Dictionary<byte, object> dictionary = PhotonClient.BuildParams(args);
				try
				{
					MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2GateServer);
					result = mobaPeer.OpCustom(code, dictionary, true, (byte)cmdChannel, mobaPeer.IsEncryptionAvailable);
				}
				finally
				{
					PhotonClient.ReleaseParam(dictionary);
				}
			}
			catch (Exception ex)
			{
				result = false;
				Log.error("SendGsChannelMessage Error : " + code + ex.Message);
			}
			return result;
		}

		public bool RegisterUser(string nickname)
		{
			bool result = false;
			try
			{
				Log.debug("==> PhotonClient ： 注册游戏账户... nickname = " + nickname + ", accountid = " + this.m_user.AccountData.AccountId);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary[59] = nickname;
				dictionary[71] = this.m_user.AccountData.AccountId;
				dictionary[218] = this.m_serverlist[this.currLoginServerIndex].appVer;
				result = this.SendGameChannelMessage(10, dictionary);
			}
			catch (Exception ex)
			{
				result = false;
				Log.error("Register Error : " + ex.Message);
			}
			return result;
		}

		public bool LoginUserByAccounttIdToGate(string token)
		{
			bool result = false;
			try
			{
				if (this.m_user.AccountData != null && this.m_user.AccountData.AccountId != null)
				{
					Log.debug("==> PhotonClient ： 登录已有的游戏账户 ... AccountId = " + this.m_user.AccountData.AccountId);
					this.SendGateSelfChannelMessage(21, new Dictionary<byte, object>
					{
						{
							71,
							this.m_user.AccountData.AccountId
						},
						{
							5,
							token
						}
					});
				}
				else
				{
					result = this.RegisterUser("default");
				}
			}
			catch (Exception ex)
			{
				result = false;
				Log.error("LoginUserByAccounttIdToGate Error : " + ex.Message);
			}
			return result;
		}

		public bool LoginUserByAccounttIdWithGateToGame(string appVer)
		{
			bool result = false;
			try
			{
				if (this.m_user.AccountData != null && this.m_user.AccountData.AccountId != null)
				{
					Log.debug("==> PhotonClient ： 登录已有的游戏账户 ... AccountId = " + this.m_user.AccountData.AccountId);
					Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
					dictionary[57] = this.m_user.AccountData.AccountId;
					dictionary[218] = appVer;
					this.SendGameChannelMessage(1, dictionary);
				}
				else
				{
					result = this.RegisterUser("default");
				}
			}
			catch (Exception ex)
			{
				result = false;
				Log.error("LoginUserByAccounttIdWithGateToGame Error : " + ex.Message);
			}
			return result;
		}

		public bool SendGateSelfChannelMessage(byte code, Dictionary<byte, object> args)
		{
			bool flag = this.SendGateChannelMessage(MobaPeerType.C2GateServer, MobaChannel.Default, code, args);
			if (!flag)
			{
				Log.warning("SendGameMsg failed for " + code);
			}
			return flag;
		}

		public bool SendGameChannelMessage(byte code, Dictionary<byte, object> args)
		{
			bool flag = this.SendGateChannelMessage(MobaPeerType.C2GateServer, MobaChannel.Game, code, args);
			if (!flag)
			{
				Log.warning("SendGameMsg failed for " + code);
			}
			return flag;
		}

		public bool SendSessionChannelMessage(byte code, MobaChannel Session, Dictionary<byte, object> args)
		{
			bool flag = this.SendGateChannelMessage(MobaPeerType.C2GateServer, Session, code, args);
			if (!flag)
			{
				Log.warning("SendSessionMsg failed for " + code);
			}
			return flag;
		}

		public bool SendLobbyChannelMessage(LobbyCode code, params object[] args)
		{
			bool flag = this.SendGateChannelMessagePM(MobaPeerType.C2GateServer, MobaChannel.Lobby, (byte)code, args);
			if (!flag)
			{
				Log.warning("SendLobbyMsg failed for " + code);
			}
			return flag;
		}

		public void SelectGameServer(int index)
		{
			try
			{
				Log.debug(" MobaClient ： 选择游戏服务器... index = " + index);
				this.currLoginServerIndex = index;
			}
			catch (Exception ex)
			{
				Log.debug("==> MobaClient：SelectionGameServer ..." + ex.Message);
				throw ex;
			}
		}

		private static Dictionary<byte, object> BuildParams(object[] args)
		{
			Dictionary<byte, object> result;
			if (args == null || args.Length <= 0)
			{
				result = null;
			}
			else
			{
				Dictionary<byte, object> dictionary = PhotonClient.usePool ? PhotonClient._dictPool.Get() : new Dictionary<byte, object>();
				byte b = 0;
				while ((int)b < args.Length)
				{
					dictionary[b] = args[(int)b];
					b += 1;
				}
				result = dictionary;
			}
			return result;
		}

		private static void ReleaseParam(Dictionary<byte, object> dict)
		{
			if (PhotonClient.usePool && dict != null)
			{
				PhotonClient._dictPool.Release(dict);
			}
		}

		public bool SendLobbyMsg(LobbyCode code, params object[] args)
		{
			bool result = false;
			try
			{
				Dictionary<byte, object> dictionary = PhotonClient.BuildParams(args);
				try
				{
					MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2Lobby);
					result = mobaPeer.OpCustom((byte)code, dictionary, true, 1, mobaPeer.IsEncryptionAvailable);
				}
				finally
				{
					PhotonClient.ReleaseParam(dictionary);
				}
			}
			catch (Exception ex)
			{
				result = false;
				Log.error("SendLobbyMsg Error : " + ex.Message);
			}
			return result;
		}

		private CmdPkgForThread GetCachedCmdPkg()
		{
			CmdPkgForThread result;
			if (this.cmdCacheListBack.Count == 0)
			{
				result = new CmdPkgForThread();
			}
			else
			{
				CmdPkgForThread cmdPkgForThread = this.cmdCacheListBack[0];
				this.cmdCacheListBack.RemoveAt(0);
				result = cmdPkgForThread;
			}
			return result;
		}

		private void ReleaseCmdPkg(CmdPkgForThread pkg)
		{
			pkg.param = null;
			this.cmdCacheListBack.Add(pkg);
		}

		public bool SendPvpServerMsg(PvpCode code, params object[] args)
		{
			bool result = false;
			try
			{
				Dictionary<byte, object> param = PhotonClient.BuildParams(args);
				lock (this.sendThreadLock)
				{
					CmdPkgForThread cachedCmdPkg = this.GetCachedCmdPkg();
					cachedCmdPkg.code = code;
					cachedCmdPkg.param = param;
					this.cmdCacheList.Add(cachedCmdPkg);
				}
				result = true;
			}
			catch (Exception ex)
			{
				result = false;
				Log.error(string.Concat(new object[]
				{
					"SendPvpServerMsg Error : ",
					code,
					" ",
					ex.Message
				}));
			}
			return result;
		}

		private bool NeedUnrelivable(PvpCode code)
		{
			return code == PvpCode.C2P_UseSkill || code == PvpCode.C2P_MoveToPos;
		}

		public long GetPing()
		{
			return 0L;
		}

		public long GetSvrTime()
		{
			long result;
			if (this.timeSyncSystem != null)
			{
				result = this.timeSyncSystem.serverTime;
			}
			else
			{
				result = 0L;
			}
			return result;
		}

		public long GetClientTime()
		{
			long result;
			if (this.timeSyncSystem != null)
			{
				result = this.timeSyncSystem.clientTime;
			}
			else
			{
				result = 0L;
			}
			return result;
		}

		public long GetExtraDelayTime()
		{
			long result;
			if (this.timeSyncSystem != null)
			{
				result = this.timeSyncSystem.ExtraDelayTime;
			}
			else
			{
				result = 0L;
			}
			return result;
		}

		public void OnConnectEvent(bool ConnectStatus, int ServerId, MobaPeerType peerType)
		{
			if (peerType == MobaPeerType.C2GateServer)
			{
				MobaPeer mobaPeer = this.GetMobaPeer(MobaPeerType.C2GateServer);
				if (mobaPeer != null && ConnectStatus && mobaPeer.ConnectedTime > 0)
				{
					if (this.IsReconnect)
					{
						try
						{
							Log.debug(" MobaClient ： 向服务器发送校验信息...");
							Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
							dictionary[57] = this.m_user.UserData.UserId;
							dictionary[182] = this.OnlyServerKey;
							mobaPeer.OpCustom(18, dictionary, true, 1);
						}
						catch (Exception ex)
						{
							Log.error("Main Peer Reconnected Error : " + ex.Message + "->" + ex.ToString());
						}
					}
				}
			}
			if (this.ConnectEvent != null)
			{
				this.ConnectEvent(ConnectStatus, ServerId, (int)peerType);
			}
		}

		public void OnConnectStatusEvent(StatusCode Code, int ServerId, int ServerType)
		{
			if (this.ConnectStatusEvent != null)
			{
				this.ConnectStatusEvent(Code, ServerId, ServerType);
			}
		}

		public void OnRegisterEvent(int Ret, string DebugMessage, AccountData data)
		{
			if (this.RegisterEvent != null)
			{
				this.RegisterEvent(Ret, DebugMessage, data);
			}
		}

		public void OnLoginEvent(int Ret, string DebugMessage, AccountData data)
		{
			if (this.LoginEvent != null)
			{
				this.LoginEvent(Ret, DebugMessage, data);
			}
		}

		public void OnErrorEvent(int operation, int Ret, string DebugMessage)
		{
			if (this.ErrorEvent != null)
			{
				this.ErrorEvent(operation, Ret, DebugMessage);
			}
		}

		public void OnGetServerListEvent(int Ret, string DebugMessage, List<ServerInfo> serverlist)
		{
			if (this.GetServerListEvent != null)
			{
				this.GetServerListEvent(Ret, DebugMessage, serverlist);
			}
		}

		public void OnUpgradeEvent(bool status, int Ret, string DebugMessage, ClientData data)
		{
			if (this.UpgradeEvent != null)
			{
				this.UpgradeEvent(status, Ret, DebugMessage, data);
			}
		}

		public void OnLoginByChannelIdEvent(int Ret, string DebugMessage, AccountData Data)
		{
			if (this.LoginByChannelIdEvent != null)
			{
				this.LoginByChannelIdEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTeamRoomOperationEvent(int Ret, string DebugMessage, MobaTeamRoomCode Code, List<RoomData> Data)
		{
			if (this.TeamRoomOperationEvent != null)
			{
				this.TeamRoomOperationEvent(Ret, DebugMessage, Code, Data);
			}
		}

		public void OnCompleteTaskMessageEvent(int Ret, string DebugMessage, List<TaskData> TaskIds)
		{
			if (this.CompleteTaskMessageEvent != null)
			{
				this.CompleteTaskMessageEvent(Ret, DebugMessage, TaskIds);
			}
		}

		public void OnNotificationEvent(int Ret, string DebugMessage, NotificationData Data)
		{
			if (this.NotificationEvent != null)
			{
				this.NotificationEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnNoticeMedalEvent(int Ret, string DebugMessage, int medalType, bool isBright)
		{
			if (this.NoticeMedalEvent != null)
			{
				this.NoticeMedalEvent(Ret, DebugMessage, medalType, isBright);
			}
		}

		public void OnRegisterUserEvent(int Ret, string DebugMessage, UserData data)
		{
			if (this.RegisterUserEvent != null)
			{
				this.RegisterUserEvent(Ret, DebugMessage, data);
			}
		}

		public void OnLoginUserEvent(int Ret, string DebugMessage, UserData data)
		{
			if (this.LoginUserEvent != null)
			{
				this.LoginUserEvent(Ret, DebugMessage, data);
			}
		}

		public void OnGetHeroListEvent(int Ret, string DebugMessage, List<HeroInfoData> data)
		{
			if (this.GetHeroListEvent != null)
			{
				this.GetHeroListEvent(Ret, DebugMessage, data);
			}
		}

		public void OnGetEquipmentListEvent(int Ret, string DebugMessage, List<EquipmentInfoData> data)
		{
			if (this.GetEquipmentListEvent != null)
			{
				this.GetEquipmentListEvent(Ret, DebugMessage, data);
			}
		}

		public void OnUsingEquipmentEvent(int Ret, string DebugMessage, string EquipId)
		{
			if (this.UsingEquipmentEvent != null)
			{
				this.UsingEquipmentEvent(Ret, DebugMessage, EquipId);
			}
		}

		public void OnHeroAdvanceEvent(int Ret, string DebugMessage)
		{
			if (this.HeroAdvanceEvent != null)
			{
				this.HeroAdvanceEvent(Ret, DebugMessage);
			}
		}

		public void OnGetRewardDropEvent(int Ret, string DebugMessage, string[] Data)
		{
			if (this.GetRewardDropEvent != null)
			{
				this.GetRewardDropEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryUploadFightResultEvent(int Ret, string DebugMessage, List<EquipmentInfoData> data)
		{
			if (this.TryUploadFightResultEvent != null)
			{
				this.TryUploadFightResultEvent(Ret, DebugMessage, data);
			}
		}

		public void OnGuestLoginEvent(int Ret, string DebugMessage, UserData data)
		{
			if (this.GuestLoginEvent != null)
			{
				this.GuestLoginEvent(Ret, DebugMessage, data);
			}
		}

		public void OnGetMyTalentListEvent(int Ret, string DebugMessage, List<TalentInfoData> data)
		{
			if (this.GetMyTalentListEvent != null)
			{
				this.GetMyTalentListEvent(Ret, DebugMessage, data);
			}
		}

		public void OnTryChangeTalentEvent(int Ret, string DebugMessage, int ReqCoin, List<TalentInfoData> data)
		{
			if (this.TryChangeTalentEvent != null)
			{
				this.TryChangeTalentEvent(Ret, DebugMessage, ReqCoin, data);
			}
		}

		public void OnTryBuyTalentPagEvent(int Ret, string DebugMessage)
		{
			if (this.TryBuyTalentPagEvent != null)
			{
				this.TryBuyTalentPagEvent(Ret, DebugMessage);
			}
		}

		public void OnTryModifyCurrUseTalentPagEvent(int Ret, string DebugMessage, int pagId)
		{
			if (this.TryModifyCurrUseTalentPagEvent != null)
			{
				this.TryModifyCurrUseTalentPagEvent(Ret, DebugMessage, pagId);
			}
		}

		public void OnTryRestCurrUseTalentPagEvent(int Ret, string DebugMessage, int reqCoin)
		{
			if (this.TryRestCurrUseTalentPagEvent != null)
			{
				this.TryRestCurrUseTalentPagEvent(Ret, DebugMessage, reqCoin);
			}
		}

		public void OnTryUseSoulstonesEvent(int Ret, string DebugMessage, HeroInfoData data)
		{
			if (this.TryUseSoulstonesEvent != null)
			{
				this.TryUseSoulstonesEvent(Ret, DebugMessage, data);
			}
		}

		public void OnTryUsePropsEvent(int Ret, string DebugMessage, int exp)
		{
			if (this.TryUsePropsEvent != null)
			{
				this.TryUsePropsEvent(Ret, DebugMessage, exp);
			}
		}

		public void OnGetMyRuneListEvent(int Ret, string DebugMessage, List<RuneInfoData> data)
		{
			if (this.GetMyRuneListEvent != null)
			{
				this.GetMyRuneListEvent(Ret, DebugMessage, data);
			}
		}

		public void OnTryChangeRuneEvent(int Ret, string DebugMessage, List<RuneInfoData> data)
		{
			if (this.TryChangeRuneEvent != null)
			{
				this.TryChangeRuneEvent(Ret, DebugMessage, data);
			}
		}

		public void OnTryBuyRunePagEvent(int Ret, string DebugMessage)
		{
			if (this.TryBuyRunePagEvent != null)
			{
				this.TryBuyRunePagEvent(Ret, DebugMessage);
			}
		}

		public void OnTryModifyCurrUseRunePagEvent(int Ret, string DebugMessage, int pagId)
		{
			if (this.TryModifyCurrUseRunePagEvent != null)
			{
				this.TryModifyCurrUseRunePagEvent(Ret, DebugMessage, pagId);
			}
		}

		public void OnTryCoalesceEvent(int Ret, string DebugMessage, EquipmentInfoData data)
		{
			if (this.TryCoalesceEvent != null)
			{
				this.TryCoalesceEvent(Ret, DebugMessage, data);
			}
		}

		public void OnGetBattlesInfoEvent(int Ret, string DebugMessage, List<BattlesModel> data)
		{
			if (this.GetBattlesInfoEvent != null)
			{
				this.GetBattlesInfoEvent(Ret, DebugMessage, data);
			}
		}

		public void OnTryBuySkillPointEvent(int Ret, string DebugMessage, int ReqMoney)
		{
			if (this.TryBuySkillPointEvent != null)
			{
				this.TryBuySkillPointEvent(Ret, DebugMessage, ReqMoney);
			}
		}

		public void OnTryUsingSkillPointEvent(int Ret, string DebugMessage)
		{
			if (this.TryUsingSkillPointEvent != null)
			{
				this.TryUsingSkillPointEvent(Ret, DebugMessage);
			}
		}

		public void OnTryCheckUnlockByBattleIdEvent(int Ret, string DebugMessage, bool IsOnlock, int BattleId)
		{
			if (this.TryCheckUnlockByBattleIdEvent != null)
			{
				this.TryCheckUnlockByBattleIdEvent(Ret, DebugMessage, IsOnlock, BattleId);
			}
		}

		public void OnTryUpdateDefFightTeamEvent(int Ret, string DebugMessage)
		{
			if (this.TryUpdateDefFightTeamEvent != null)
			{
				this.TryUpdateDefFightTeamEvent(Ret, DebugMessage);
			}
		}

		public void OnTryGetTBCEnemyInfoEvent(int Ret, string DebugMessage, List<TBCEnemyInfo> Data)
		{
			if (this.TryGetTBCEnemyInfoEvent != null)
			{
				this.TryGetTBCEnemyInfoEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryRestTBCEnemyInfoEvent(int Ret, string DebugMessage)
		{
			if (this.TryRestTBCEnemyInfoEvent != null)
			{
				this.TryRestTBCEnemyInfoEvent(Ret, DebugMessage);
			}
		}

		public void OnTrySaveTBCEnemyInfoEvent(int Ret, string DebugMessage, List<TBCHeroStateInfo> MyHeroStateInfoList, List<TBCHeroStateInfo> TargetHeroStateInfoList)
		{
			if (this.TrySaveTBCEnemyInfoEvent != null)
			{
				this.TrySaveTBCEnemyInfoEvent(Ret, DebugMessage, MyHeroStateInfoList, TargetHeroStateInfoList);
			}
		}

		public void OnGetTBCMyHeroStateInfoEvent(int Ret, string DebugMessage, List<TBCHeroStateInfo> data)
		{
			if (this.GetTBCMyHeroStateInfoEvent != null)
			{
				this.GetTBCMyHeroStateInfoEvent(Ret, DebugMessage, data);
			}
		}

		public void OnTryReceiveTBCRewardEvent(int Ret, string DebugMessage, int GetCoin)
		{
			if (this.TryReceiveTBCRewardEvent != null)
			{
				this.TryReceiveTBCRewardEvent(Ret, DebugMessage, GetCoin);
			}
		}

		public void OnTryUpdateArenaDefTeamEvent(int Ret, string DebugMessage, string[] HeroArr)
		{
			if (this.TryUpdateArenaDefTeamEvent != null)
			{
				this.TryUpdateArenaDefTeamEvent(Ret, DebugMessage, HeroArr);
			}
		}

		public void OnGetArenaDefTeamEvent(int Ret, string DebugMessage, string[] HeroArr)
		{
			if (this.GetArenaDefTeamEvent != null)
			{
				this.GetArenaDefTeamEvent(Ret, DebugMessage, HeroArr);
			}
		}

		public void OnGetArenaEnemyInfoEvent(int Ret, string DebugMessage, List<ArenaData> Data)
		{
			if (this.GetArenaEnemyInfoEvent != null)
			{
				this.GetArenaEnemyInfoEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryArenaAtcCheckEvent(int Ret, string DebugMessage)
		{
			if (this.TryArenaAtcCheckEvent != null)
			{
				this.TryArenaAtcCheckEvent(Ret, DebugMessage);
			}
		}

		public void OnTryUploadArenaAtcResultEvent(int Ret, string DebugMessage, long Rank, ArenaAccount arenaAccount)
		{
			if (this.TryUploadArenaAtcResultEvent != null)
			{
				object obj = new object();
				this.TryUploadArenaAtcResultEvent(Ret, DebugMessage, Rank, arenaAccount);
			}
		}

		public void OnGetArenaFightLogEvent(int Ret, string DebugMessage, List<ArenaLogData> Data)
		{
			if (this.GetArenaFightLogEvent != null)
			{
				this.GetArenaFightLogEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnGetArenaStateEvent(int Ret, string DebugMessage, ArenaState Data)
		{
			if (this.GetArenaStateEvent != null)
			{
				this.GetArenaStateEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryRestArenaCountEvent(int Ret, string DebugMessage)
		{
			if (this.TryRestArenaCountEvent != null)
			{
				this.TryRestArenaCountEvent(Ret, DebugMessage);
			}
		}

		public void OnTryRestArenaCDTimeEvent(int Ret, string DebugMessage, int ReqDimond)
		{
			if (this.TryRestArenaCDTimeEvent != null)
			{
				this.TryRestArenaCDTimeEvent(Ret, DebugMessage, ReqDimond);
			}
		}

		public void OnGetArenaRankListEvent(int Ret, string DebugMessage, List<ArenaData> Data)
		{
			if (this.GetArenaRankListEvent != null)
			{
				this.GetArenaRankListEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnGetLastDayArenaRankListEvent(int Ret, string DebugMessage, List<ArenaData> Data)
		{
			if (this.GetLastDayArenaRankListEvent != null)
			{
				this.GetLastDayArenaRankListEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnGetMyArenaRankEvent(int Ret, string DebugMessage, ArenaData Data)
		{
			if (this.GetMyArenaRankEvent != null)
			{
				this.GetMyArenaRankEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnGetLuckyDrawStateEvent(int Ret, string DebugMessage, LuckyDrawData Data)
		{
			if (this.GetLuckyDrawStateEvent != null)
			{
				this.GetLuckyDrawStateEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryLuckyDrawEvent(int Ret, string DebugMessage, List<LuckyDrawResult> Data)
		{
			if (this.TryLuckyDrawEvent != null)
			{
				this.TryLuckyDrawEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnGetShopGoodsByShopTypeEvent(int Ret, string DebugMessage, ShopData Data)
		{
			if (this.GetShopGoodsByShopTypeEvent != null)
			{
				this.GetShopGoodsByShopTypeEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryBuyGoodsEvent(int Ret, string DebugMessage, int ReqCoin)
		{
			if (this.TryBuyGoodsEvent != null)
			{
				this.TryBuyGoodsEvent(Ret, DebugMessage, ReqCoin);
			}
		}

		public void OnTryRestShopByShopTypeEvent(int Ret, string DebugMessage, ShopData Data)
		{
			if (this.TryRestShopByShopTypeEvent != null)
			{
				this.TryRestShopByShopTypeEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnGetTaskListEvent(int Ret, string DebugMessage, List<TaskData> Data)
		{
			if (this.GetTaskListEvent != null)
			{
				this.GetTaskListEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryCompleteTaskEvent(int Ret, string DebugMessage, int TaskId)
		{
			if (this.TryCompleteTaskEvent != null)
			{
				this.TryCompleteTaskEvent(Ret, DebugMessage, TaskId);
			}
		}

		public void OnTryAttendanceByTypeEvnet(int Ret, string DebugMessage, string Reward)
		{
			if (this.TryAttendanceByTypeEvnet != null)
			{
				this.TryAttendanceByTypeEvnet(Ret, DebugMessage, Reward);
			}
		}

		public void OnTryModfiyNickNameEvent(int Ret, string DebugMessage, int ReqDimond)
		{
			if (this.TryModfiyNickNameEvent != null)
			{
				this.TryModfiyNickNameEvent(Ret, DebugMessage, ReqDimond);
			}
		}

		public void OnTryModfiyIconEvent(int Ret, string DebugMessage)
		{
			if (this.TryModfiyIconEvent != null)
			{
				this.TryModfiyIconEvent(Ret, DebugMessage);
			}
		}

		public void OnTrySellPropsEvent(int Ret, string DebugMessage, int Coin)
		{
			if (this.TrySellPropsEvent != null)
			{
				this.TrySellPropsEvent(Ret, DebugMessage, Coin);
			}
		}

		public void OnTryExchangeByDimondEvent(int Ret, string DebugMessage, List<ExchangeData> Data)
		{
			if (this.TryExchangeByDimondEvent != null)
			{
				this.TryExchangeByDimondEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnGetMailListEvent(int Ret, string DebugMessage, List<MailData> Data)
		{
			if (this.GetMailListEvent != null)
			{
				this.GetMailListEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryReceiveMailAttachmentEvent(int Ret, string DebugMessage, List<RewardModel> Data)
		{
			if (this.TryReceiveMailAttachmentEvent != null)
			{
				this.TryReceiveMailAttachmentEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryCreateUnionEvent(int Ret, string DebugMessage, int UnionId)
		{
			if (this.TryCreateUnionEvent != null)
			{
				this.TryCreateUnionEvent(Ret, DebugMessage, UnionId);
			}
		}

		public void OnGetUnionInfoEvent(int Ret, string DebugMessage, UnionInfoData Data)
		{
			if (this.GetUnionInfoEvent != null)
			{
				this.GetUnionInfoEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryDissolveUnionEvent(int Ret, string DebugMessage)
		{
			if (this.TryDissolveUnionEvent != null)
			{
				this.TryDissolveUnionEvent(Ret, DebugMessage);
			}
		}

		public void OnTrySearchUnionEvent(int Ret, string DebugMessage, UnionInfoData Data)
		{
			if (this.TrySearchUnionEvent != null)
			{
				this.TrySearchUnionEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryJoinUnionEvent(int Ret, string DebugMessage)
		{
			if (this.TryJoinUnionEvent != null)
			{
				this.TryJoinUnionEvent(Ret, DebugMessage);
			}
		}

		public void OnTryKickUnionEvent(int Ret, string DebugMessage, List<UnionMemberData> Data)
		{
			if (this.TryKickUnionEvent != null)
			{
				this.TryKickUnionEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryLeaveUnionEvent(int Ret, string DebugMessage)
		{
			if (this.TryLeaveUnionEvent != null)
			{
				this.TryLeaveUnionEvent(Ret, DebugMessage);
			}
		}

		public void OnTryModifyUnionSettingEvent(int Ret, string DebugMessage, UnionInfoData Data)
		{
			if (this.TryModifyUnionSettingEvent != null)
			{
				this.TryModifyUnionSettingEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryModifyAnnouncementEvent(int Ret, string DebugMessage)
		{
			if (this.TryModifyAnnouncementEvent != null)
			{
				this.TryModifyAnnouncementEvent(Ret, DebugMessage);
			}
		}

		public void OnGetUnionListEvent(int Ret, string DebugMessage, List<UnionInfoData> Data)
		{
			if (this.GetUnionListEvent != null)
			{
				this.GetUnionListEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnGetUnionLogsEvent(int Ret, string DebugMessage, List<UnionLogData> Data)
		{
			if (this.GetUnionLogsEvent != null)
			{
				this.GetUnionLogsEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnGetMemberListEvent(int Ret, string DebugMessage, List<UnionMemberData> Data)
		{
			if (this.GetMemberListEvent != null)
			{
				this.GetMemberListEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryUpgradeMasterEvent(int Ret, string DebugMessage, string TargetId)
		{
			if (this.TryUpgradeMasterEvent != null)
			{
				this.TryUpgradeMasterEvent(Ret, DebugMessage, TargetId);
			}
		}

		public void OnTryAppointElderEvent(int Ret, string DebugMessage, string TagetId)
		{
			if (this.TryAppointElderEvent != null)
			{
				this.TryAppointElderEvent(Ret, DebugMessage, TagetId);
			}
		}

		public void OnTryUnAppointElderEvent(int Ret, string DebugMessage, string TargetId)
		{
			if (this.TryUnAppointElderEvent != null)
			{
				this.TryUnAppointElderEvent(Ret, DebugMessage, TargetId);
			}
		}

		public void OnGetUnionRequestListEvent(int Ret, string DebugMessage, List<UnionMemberData> Data)
		{
			if (this.GetUnionRequestListEvent != null)
			{
				this.GetUnionRequestListEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryDisposeUnionReqEvent(int Ret, string DebugMessage, string TargetId)
		{
			if (this.TryDisposeUnionReqEvent != null)
			{
				this.TryDisposeUnionReqEvent(Ret, DebugMessage, TargetId);
			}
		}

		public void OnTryEnchantEquipmentEvent(int Ret, string DebugMessage, string EpMagic)
		{
			if (this.TryEnchantEquipmentEvent != null)
			{
				this.TryEnchantEquipmentEvent(Ret, DebugMessage, EpMagic);
			}
		}

		public void OnTrySweepBattleEvent(int Ret, string DebugMessage, List<SweepData> Data)
		{
			if (this.TrySweepBattleEvent != null)
			{
				this.TrySweepBattleEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryModifyEmailStateEvent(int Ret, string DebugMessage, long MailId)
		{
			if (this.TryModifyEmailStateEvent != null)
			{
				this.TryModifyEmailStateEvent(Ret, DebugMessage, MailId);
			}
		}

		public void OnTryUpdateSmallMeleeTeamEvent(int Ret, string DebugMessage)
		{
			if (this.TryUpdateSmallMeleeTeamEvent != null)
			{
				this.TryUpdateSmallMeleeTeamEvent(Ret, DebugMessage);
			}
		}

		public void OnGetSmallMeleeInfoEvent(int Ret, string DebugMessage, SmallMeleeData Data)
		{
			if (this.GetSmallMeleeInfoEvent != null)
			{
				this.GetSmallMeleeInfoEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnGetTalentInfoByUserIdEvent(int Ret, string DebugMessage, List<TalentModel> Data)
		{
			if (this.GetTalentInfoByUserIdEvent != null)
			{
				this.GetTalentInfoByUserIdEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryChangeKillTitanTeamEvent(int Ret, string DebugMessage, KillTitanData Data)
		{
			if (this.TryChangeKillTitanTeamEvent != null)
			{
				this.TryChangeKillTitanTeamEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnGetKillTitanInfoEvent(int Ret, string DebugMessage, KillTitanData Data)
		{
			if (this.GetKillTitanInfoEvent != null)
			{
				this.GetKillTitanInfoEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryReceiveKillTitanRewardEvent(int Ret, string DebugMessage, KillTitanReward Data)
		{
			if (this.TryReceiveKillTitanRewardEvent != null)
			{
				this.TryReceiveKillTitanRewardEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryAddKillTitanSnapshotEvent(int Ret, string DebugMessage, List<KillTitanSnapshotData> Data)
		{
			if (this.TryAddKillTitanSnapshotEvent != null)
			{
				this.TryAddKillTitanSnapshotEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnGetKillTitanSnapshotEvent(int Ret, string DebugMessage, List<KillTitanSnapshotData> Data)
		{
			if (this.GetKillTitanSnapshotEvent != null)
			{
				this.GetKillTitanSnapshotEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryReceiveSnapshotAwardEvent(int Ret, string DebugMessage, List<AwardInfoData> Data)
		{
			if (this.TryReceiveSnapshotAwardEvent != null)
			{
				this.TryReceiveSnapshotAwardEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryBuySpecialShopOwnEvent(int Ret, string DebugMessage)
		{
			if (this.TryBuySpecialShopOwnEvent != null)
			{
				this.TryBuySpecialShopOwnEvent(Ret, DebugMessage);
			}
		}

		public void OnTryRestTodayBattlesCountEvent(int Ret, string DebugMessage, int ReqDimonds)
		{
			if (this.TryRestTodayBattlesCountEvent != null)
			{
				this.TryRestTodayBattlesCountEvent(Ret, DebugMessage, ReqDimonds);
			}
		}

		public void OnTryJoinPvpRoom(int Ret, string DebugMessage, int battleId)
		{
			if (this.TryJoinPvpEvent != null)
			{
				this.TryJoinPvpEvent(Ret, battleId);
			}
		}

		public void OnTryApplyAddFriendEvent(int Ret, string DebugMessage, FriendData Data)
		{
			if (this.TryApplyAddFriendEvent != null)
			{
				this.TryApplyAddFriendEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnGetFriendListEvent(int Ret, string DebugMessage, List<FriendData> Data)
		{
			if (this.GetFriendListEvent != null)
			{
				this.GetFriendListEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnTryModifyFriendStatusEvent(int Ret, string DebugMessage, byte OperType, long TargetSummId)
		{
			if (this.TryModifyFriendStatusEvent != null)
			{
				this.TryModifyFriendStatusEvent(Ret, DebugMessage, OperType, TargetSummId);
			}
		}

		public void OnGetFriendsMessagesEvent(int Ret, string DebugMessage, List<NotificationData> Data)
		{
			if (this.GetFriendsMessagesEvent != null)
			{
				this.GetFriendsMessagesEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnGetUserInfoBySummIdEvent(int Ret, string DebugMessage, string[] Data)
		{
			if (this.GetUserInfoBySummIdEvent != null)
			{
				this.GetUserInfoBySummIdEvent(Ret, DebugMessage, Data);
			}
		}

		public void OnGetMedalDataByUserId(int Ret, string DebugMessage, List<bool> medal)
		{
			if (this.GetMedalDataByUserIdEvent != null)
			{
				this.GetMedalDataByUserIdEvent(Ret, DebugMessage, medal);
			}
		}

		public void OnTryBuySkinEvent(int Ret, string DebugMessage)
		{
			if (this.TryBuySkinEvent != null)
			{
				this.TryBuySkinEvent(Ret, DebugMessage);
			}
		}

		public void OnTryChangeSkinEvent(int Ret, string DebugMessage)
		{
			if (this.TryChangeSkinEvent != null)
			{
				this.TryChangeSkinEvent(Ret, DebugMessage);
			}
		}

		public void OnTryReciveMessageEvent(int Ret, string DebugMessage, ChatMessage TaskIds)
		{
			if (this.TryReciveMessageEvent != null)
			{
				this.TryReciveMessageEvent(Ret, DebugMessage, TaskIds);
			}
		}

		public void RegistSendMsg2ClientCustom(PhotonClient.DeleSendMsg2ClientCustom dele)
		{
			this.mDeleSendMsgCustom = (PhotonClient.DeleSendMsg2ClientCustom)Delegate.Combine(this.mDeleSendMsgCustom, dele);
		}

		public void UnRegistSendMsg2ClientCustom(PhotonClient.DeleSendMsg2ClientCustom dele)
		{
			this.mDeleSendMsgCustom = (PhotonClient.DeleSendMsg2ClientCustom)Delegate.Remove(this.mDeleSendMsgCustom, dele);
		}

		public void RegistSendMsg2ClientMasterCode(PhotonClient.DeleSendMsg2ClientMasterCode dele)
		{
			this.mDeleSendMsgMasterCode = (PhotonClient.DeleSendMsg2ClientMasterCode)Delegate.Combine(this.mDeleSendMsgMasterCode, dele);
		}

		public void UnRegistSendMsg2ClientMasterCode(PhotonClient.DeleSendMsg2ClientMasterCode dele)
		{
			this.mDeleSendMsgMasterCode = (PhotonClient.DeleSendMsg2ClientMasterCode)Delegate.Remove(this.mDeleSendMsgMasterCode, dele);
		}

		public void RegistSendMsg2ClientGameCode(PhotonClient.DeleSendMsg2ClientGameCode dele)
		{
			this.mDeleSendMsgGameCode = (PhotonClient.DeleSendMsg2ClientGameCode)Delegate.Combine(this.mDeleSendMsgGameCode, dele);
		}

		public void UnRegistSendMsg2ClientGameCode(PhotonClient.DeleSendMsg2ClientGameCode dele)
		{
			this.mDeleSendMsgGameCode = (PhotonClient.DeleSendMsg2ClientGameCode)Delegate.Remove(this.mDeleSendMsgGameCode, dele);
		}

		public void RegistSendMsg2ClientGateCode(PhotonClient.DeleSendMsg2ClientGateCode dele)
		{
			this.mDeleSendMsgGateCode = (PhotonClient.DeleSendMsg2ClientGateCode)Delegate.Combine(this.mDeleSendMsgGateCode, dele);
		}

		public void UnRegistSendMsg2ClientGateCode(PhotonClient.DeleSendMsg2ClientGateCode dele)
		{
			this.mDeleSendMsgGateCode = (PhotonClient.DeleSendMsg2ClientGateCode)Delegate.Remove(this.mDeleSendMsgGateCode, dele);
		}

		public void RegistSendMsg2ClientPvpCode(PhotonClient.DeleSendMsg2ClientPvpCode dele)
		{
			this.mDeleSendMsgPvpCode = (PhotonClient.DeleSendMsg2ClientPvpCode)Delegate.Combine(this.mDeleSendMsgPvpCode, dele);
		}

		public void UnRegistSendMsg2ClientPvpCode(PhotonClient.DeleSendMsg2ClientPvpCode dele)
		{
			this.mDeleSendMsgPvpCode = (PhotonClient.DeleSendMsg2ClientPvpCode)Delegate.Remove(this.mDeleSendMsgPvpCode, dele);
		}

		public void RegistSendMsg2ClientChatCode(PhotonClient.DeleSendMsg2ClientChatCode dele)
		{
			this.mDeleSendMsgChatCode = (PhotonClient.DeleSendMsg2ClientChatCode)Delegate.Combine(this.mDeleSendMsgChatCode, dele);
		}

		public void UnRegistSendMsg2ClientChatCode(PhotonClient.DeleSendMsg2ClientChatCode dele)
		{
			this.mDeleSendMsgChatCode = (PhotonClient.DeleSendMsg2ClientChatCode)Delegate.Remove(this.mDeleSendMsgChatCode, dele);
		}

		public void RegistSendMsg2ClientLobbyCode(PhotonClient.DeleSendMsg2ClientLobbyCode dele)
		{
			this.mDeleSendMsgLobbyCode = (PhotonClient.DeleSendMsg2ClientLobbyCode)Delegate.Combine(this.mDeleSendMsgLobbyCode, dele);
		}

		public void UnRegistSendMsg2ClientLobbyCode(PhotonClient.DeleSendMsg2ClientLobbyCode dele)
		{
			this.mDeleSendMsgLobbyCode = (PhotonClient.DeleSendMsg2ClientLobbyCode)Delegate.Remove(this.mDeleSendMsgLobbyCode, dele);
		}

		public void RegistSendMsg2ClientFriendCode(PhotonClient.DeleSendMsg2ClientFriendCode dele)
		{
			this.mDeleSendMsgFriendCode = (PhotonClient.DeleSendMsg2ClientFriendCode)Delegate.Combine(this.mDeleSendMsgFriendCode, dele);
		}

		public void UnRegistSendMsg2ClientFriendCode(PhotonClient.DeleSendMsg2ClientFriendCode dele)
		{
			this.mDeleSendMsgFriendCode = (PhotonClient.DeleSendMsg2ClientFriendCode)Delegate.Remove(this.mDeleSendMsgFriendCode, dele);
		}

		public void RegistSendMsg2ClientTeamRoomCode(PhotonClient.DeleSendMsg2ClientTeamRoomCode dele)
		{
			this.mDeleSendMsgTeamRoomCode = (PhotonClient.DeleSendMsg2ClientTeamRoomCode)Delegate.Combine(this.mDeleSendMsgTeamRoomCode, dele);
		}

		public void UnRegistSendMsg2ClientTeamRoomCode(PhotonClient.DeleSendMsg2ClientTeamRoomCode dele)
		{
			this.mDeleSendMsgTeamRoomCode = (PhotonClient.DeleSendMsg2ClientTeamRoomCode)Delegate.Remove(this.mDeleSendMsgTeamRoomCode, dele);
		}

		public void RegistSendMsg2ClientUserDataCode(PhotonClient.DeleSendMsg2ClientUserDataCode dele)
		{
			this.mDeleSendMsgUserDataCode = (PhotonClient.DeleSendMsg2ClientUserDataCode)Delegate.Combine(this.mDeleSendMsgUserDataCode, dele);
		}

		public void UnRegistSendMsg2ClientUserDataCode(PhotonClient.DeleSendMsg2ClientUserDataCode dele)
		{
			this.mDeleSendMsgUserDataCode = (PhotonClient.DeleSendMsg2ClientUserDataCode)Delegate.Remove(this.mDeleSendMsgUserDataCode, dele);
		}

		public void ExecSendMsg2Client(Photon2ClientMsg msg, object obj)
		{
			if (this.mDeleSendMsgCustom != null)
			{
				this.mDeleSendMsgCustom(msg, obj);
			}
		}

		public void ExecSendMsg2Client(MobaGameCode msg, object obj)
		{
			if (this.mDeleSendMsgGameCode != null)
			{
				this.mDeleSendMsgGameCode(msg, obj);
			}
		}

		public void ExecSendMsg2Client(MobaMasterCode msg, object obj)
		{
			if (this.mDeleSendMsgMasterCode != null)
			{
				this.mDeleSendMsgMasterCode(msg, obj);
			}
		}

		public void ExecSendMsg2Client(MobaGateCode msg, object obj)
		{
			if (this.mDeleSendMsgGateCode != null)
			{
				this.mDeleSendMsgGateCode(msg, obj);
			}
		}

		public void ExecSendMsg2Client(PvpCode msg, object obj)
		{
			if (this.pvpserver_peer != null && this.pvpserver_peer.ServerConnected && this.pvpserver_peer.MsgRecver != null && MobaPvpServerClientPeer.WillTriggerReceiver)
			{
				this.pvpserver_peer.MsgRecver.OnResponse((OperationResponse)obj);
			}
			OperationResponse operationResponse = obj as OperationResponse;
			if (operationResponse.OperationCode == 3)
			{
				if (this.timeSyncSystem != null)
				{
					this.timeSyncSystem.OnServerTimeRsp(obj);
				}
			}
			else if (this.mDeleSendMsgPvpCode != null)
			{
				if (this.NeedCustomRelivableChannel(operationResponse.OperationCode))
				{
					this.udpDriver.OnRecv(obj);
				}
				else
				{
					this.mDeleSendMsgPvpCode(msg, obj);
				}
			}
		}

		public void ExecUnrelivableCmd(PvpCode msg, object obj)
		{
			this.mDeleSendMsgPvpCode(msg, obj);
			if (this.pvpserver_peer != null && this.pvpserver_peer.ServerConnected && this.pvpserver_peer.MsgRecver != null && MobaPvpServerClientPeer.WillTriggerReceiver)
			{
				this.pvpserver_peer.MsgRecver.ExecMethod((byte)msg, obj);
			}
		}

		public void ExecSendMsg2Client(MobaChatCode msg, object obj)
		{
			if (this.mDeleSendMsgChatCode != null)
			{
				this.mDeleSendMsgChatCode(msg, obj);
			}
		}

		public void ExecSendMsg2Client(LobbyCode msg, object obj)
		{
			if (this.mDeleSendMsgLobbyCode != null)
			{
				this.mDeleSendMsgLobbyCode(msg, obj);
			}
		}

		public void ExecSendMsg2Client(MobaFriendCode msg, object obj)
		{
			if (this.mDeleSendMsgFriendCode != null)
			{
				this.mDeleSendMsgFriendCode(msg, obj);
			}
		}

		public void ExecSendMsg2Client(MobaTeamRoomCode msg, object obj)
		{
			if (this.mDeleSendMsgTeamRoomCode != null)
			{
				this.mDeleSendMsgTeamRoomCode(msg, obj);
			}
		}

		public void ExecSendMsg2Client(MobaUserDataCode msg, object obj)
		{
			if (this.mDeleSendMsgUserDataCode != null)
			{
				this.mDeleSendMsgUserDataCode(msg, obj);
			}
		}

		private bool NeedCustomRelivableChannel(byte code)
		{
			return code == UdpDriverBase.OperationCode_Ack || code == UdpDriverBase.OperationCode_Other;
		}
	}
}
