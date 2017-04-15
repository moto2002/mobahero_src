using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaHeros;
using MobaHeros.Pvp;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Com.Game.Module
{
	public class PvpRoomView : BaseView<PvpRoomView>
	{
		private Transform LeftAnchor;

		private UIToggle L_FriendBtn;

		private UIToggle L_TeamBtn;

		private Transform L_FriendLabel;

		private UIInput L_Input;

		private Transform L_AddFriendsBtn;

		private Transform L_TeamLabel;

		private Transform L_JoinTeamBtn;

		private UIScrollView L_FriendPanel;

		private UIGrid L_FriendGrid;

		private RoomChatShifter L_Chat;

		private Transform CenterAnchor;

		private UIScrollView C_ZDYPanel;

		private UIScrollView C_KHPanel;

		private Transform C_Note;

		private UIToggle C_KHBtn;

		private UIToggle C_ZDYBtn;

		private UIDragScrollView C_SpriteDrag;

		private UIProgressBar C_ProgressBar;

		private Transform C_Down;

		private Transform C_Up;

		private Transform RightAnchor;

		private Transform R_BackRoomBtn;

		private Transform R_GoToGameBtn;

		private Transform R_GrayGoToGameBtn;

		private UIScrollView R_Note;

		private UIGrid R_NoteGrid;

		private Transform R_Map;

		private Transform FindPanel;

		private UIInput F_Input;

		private Transform F_BackBtn;

		private Transform F_FindBtn;

		private Transform F_AddBtn;

		private Transform F_Item;

		private UILabel F_Name;

		private UISprite F_Texture;

		private UISprite F_Sprite;

		private UILabel F_LVLabel;

		private UILabel BottleLv;

		private UILabel RankLv;

		private UILabel MeiliLv;

		private UILabel MyMobaID;

		private Transform Delete;

		private readonly TeamDataManagement _teamDataManagement = new TeamDataManagement();

		private FriendOrTeamType _friendOrTeamType;

		private PvpRoomType _roomType = PvpRoomType.KaiHei;

		private bool _isRoomOwner;

		private bool _isFightWithRobot;

		private LevelStorage? _lastStorage;

		private bool _autoInvited;

		private Task listTask;

		private string _battleId = string.Empty;

		private List<PvpRoomNote> _noteDict = new List<PvpRoomNote>();

		private List<FriendData> _friendList = new List<FriendData>();

		private string _roomId;

		private readonly List<MatchTypeItem> _recordMatchTypeItems = new List<MatchTypeItem>();

		private KHFriendItem _khFriendItemPrefab;

		private MatchTypeItem _matchTypeItemPrefab;

		private UILabel _noteItemPrefab;

		private bool _isRegister;

		private CoroutineManager cMgr = new CoroutineManager();

		private Task task_checkQueryMsgState;

		private Task task_checkCreateMsgState;

		private PvpRoomType RoomType
		{
			get
			{
				return this._roomType;
			}
			set
			{
				if (this._roomType != value)
				{
					this._roomType = value;
				}
			}
		}

		public string battleId
		{
			get
			{
				return this._battleId;
			}
		}

		public bool IsLock
		{
			get;
			set;
		}

		private byte RoomTypeInByte
		{
			get
			{
				return (this.RoomType != PvpRoomType.KaiHei) ? 5 : 3;
			}
		}

		public PvpRoomView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Team/KHOrZDYView");
		}

		public override void Init()
		{
			this._khFriendItemPrefab = Resources.Load<KHFriendItem>("Prefab/UI/Team/KHFriendItem");
			this._matchTypeItemPrefab = Resources.Load<MatchTypeItem>("Prefab/UI/Team/MatchTypeItem");
			this._noteItemPrefab = Resources.Load<UILabel>("Prefab/UI/Team/NoteItem");
			this.LeftAnchor = this.transform.Find("LeftAnchor");
			this.L_FriendBtn = this.LeftAnchor.Find("Btn/Friend").GetComponent<UIToggle>();
			this.L_TeamBtn = this.LeftAnchor.Find("Btn/Team").GetComponent<UIToggle>();
			this.L_FriendLabel = this.LeftAnchor.Find("FriendLabel");
			this.L_Input = this.LeftAnchor.Find("Input").GetComponent<UIInput>();
			this.L_AddFriendsBtn = this.LeftAnchor.Find("AddFriends");
			this.L_TeamLabel = this.LeftAnchor.Find("TeamLabel");
			this.L_JoinTeamBtn = this.LeftAnchor.Find("TeamLabel/JoinTeam");
			this.L_FriendPanel = this.LeftAnchor.Find("FriendPanel").GetComponent<UIScrollView>();
			this.L_FriendGrid = this.LeftAnchor.Find("FriendPanel/Grid").GetComponent<UIGrid>();
			this.L_Chat = this.LeftAnchor.Find("RoomChat").GetComponent<RoomChatShifter>();
			this.CenterAnchor = this.transform.Find("CenterAnchor");
			this.C_ZDYPanel = this.CenterAnchor.Find("ZDYPanel").GetComponent<UIScrollView>();
			this.C_KHPanel = this.CenterAnchor.Find("KHPanel").GetComponent<UIScrollView>();
			this.C_Note = this.CenterAnchor.Find("Note");
			this.C_KHBtn = this.CenterAnchor.Find("Btn/KH").GetComponent<UIToggle>();
			this.C_ZDYBtn = this.CenterAnchor.Find("Btn/ZDY").GetComponent<UIToggle>();
			this.C_SpriteDrag = this.CenterAnchor.Find("Sprite").GetComponent<UIDragScrollView>();
			this.C_ProgressBar = this.CenterAnchor.Find("ProgressBar").GetComponent<UIProgressBar>();
			this.C_Down = this.CenterAnchor.Find("Position/Down");
			this.C_Up = this.CenterAnchor.Find("Position/Up");
			this.RightAnchor = this.transform.Find("RightAnchor");
			this.R_BackRoomBtn = this.RightAnchor.Find("Btn/BackRoom");
			this.R_GoToGameBtn = this.RightAnchor.Find("Btn/GoToGame");
			this.R_GrayGoToGameBtn = this.RightAnchor.Find("Btn/GoToGame/Gray");
			this.R_Note = this.RightAnchor.Find("Note").GetComponent<UIScrollView>();
			this.R_NoteGrid = this.RightAnchor.Find("Note/Grid").GetComponent<UIGrid>();
			this.R_Map = this.RightAnchor.Find("Map");
			this.FindPanel = this.LeftAnchor.Find("FindPanel");
			this.F_Input = this.FindPanel.Find("Input").GetComponent<UIInput>();
			this.F_BackBtn = this.FindPanel.Find("BackBtn");
			this.F_FindBtn = this.FindPanel.Find("FindBtn");
			this.F_AddBtn = this.FindPanel.Find("AddBtn");
			this.F_Item = this.FindPanel.Find("FindFriendItem");
			this.F_Name = this.FindPanel.Find("FindFriendItem/Name").GetComponent<UILabel>();
			this.F_Texture = this.FindPanel.Find("FindFriendItem/GeneralItem/ItemUITexture").GetComponent<UISprite>();
			this.F_Sprite = this.FindPanel.Find("FindFriendItem/GeneralItem/Texture").GetComponent<UISprite>();
			this.F_LVLabel = this.FindPanel.Find("FindFriendItem/GeneralItem/Number").GetComponent<UILabel>();
			this.BottleLv = this.F_Item.Find("Moping/Label").GetComponent<UILabel>();
			this.RankLv = this.F_Item.Find("Paiwei/Label").GetComponent<UILabel>();
			this.MeiliLv = this.F_Item.Find("Meili/Label").GetComponent<UILabel>();
			this.MyMobaID = this.FindPanel.Find("TitleLabel").GetComponent<UILabel>();
			this.Delete = this.FindPanel.Find("Delet");
			UIEventListener.Get(this.L_FriendBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickL_FriendBtn);
			UIEventListener.Get(this.L_AddFriendsBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickL_AddFriendsBtn);
			UIEventListener.Get(this.L_JoinTeamBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickL_JoinTeamBtn);
			UIEventListener.Get(this.C_KHBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickC_KHBtn);
			UIEventListener.Get(this.C_ZDYBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickC_ZDYBtn);
			UIEventListener.Get(this.R_BackRoomBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickR_BackRoomBtn);
			UIEventListener.Get(this.R_GoToGameBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickR_GoToGameBtn);
			UIEventListener.Get(this.F_FindBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickF_FindBtn);
			UIEventListener.Get(this.F_AddBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickF_AddBtn);
			UIEventListener.Get(this.F_BackBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickF_BackBtn);
			UIEventListener.Get(this.Delete.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickDelelteBtn);
		}

		public override void HandleAfterOpenView()
		{
			this._autoInvited = false;
			this._lastStorage = PvpLevelStorage.FetchLast();
			PvpLevelStorage.ClearLast();
			if (this._isRoomOwner)
			{
				this.gameObject.SetActive(false);
				this.SendRoomOperationEvent(MobaTeamRoomCode.Room_Create, this.RoomTypeInByte, string.Empty, string.Empty, (FriendGameType)this.RoomType, this._battleId, this.GetHeroCount(), string.Empty);
				this.DoCheckMsgState(MobaTeamRoomCode.Room_Create);
			}
			else
			{
				this.RefreshUI();
			}
			ModelManager.Instance.Set_Lobby_Chat_Clear();
			this.L_Chat.SetDefault();
			AutoTestController.InvokeTestLogic(AutoTestTag.Fight, delegate
			{
				this.ClickR_GoToGameBtn(null);
			}, 1f);
		}

		public override void HandleBeforeCloseView()
		{
			if (this.FindPanel != null && this.FindPanel.gameObject.activeInHierarchy)
			{
				this.FindPanel.gameObject.SetActive(false);
			}
			this.ClearData();
			Singleton<HomeChatview>.Instance.ClearRoomChat();
		}

		public override void RegisterUpdateHandler()
		{
		}

		public override void CancelUpdateHandler()
		{
		}

		public void RegisterCallBack()
		{
			if (!this._isRegister)
			{
				this._isRegister = true;
				MVC_MessageManager.AddListener_view(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg_GetFriendList));
				MVC_MessageManager.AddListener_view(MobaFriendCode.Friend_ApplyAddFriend, new MobaMessageFunc(this.OnGetMsg_ApplyAddFriend));
				MVC_MessageManager.AddListener_view(MobaGameCode.GetUserInfoBySummId, new MobaMessageFunc(this.OnGetMsg_GetUserInfoBySummId));
				MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_Join, new MobaMessageFunc(this.OnGetMsg_Room_Join));
				MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_Levea, new MobaMessageFunc(this.OnGetMsg_Room_Leave));
				MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_StartGame, new MobaMessageFunc(this.OnGetMsg_Room_StartGame));
				MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_ChangeTeamType, new MobaMessageFunc(this.OnGetMsg_Room_ChangeTeamType));
				MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_Destory, new MobaMessageFunc(this.OnGetMsg_Room_Destory));
				MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_Create, new MobaMessageFunc(this.OnGetMsg_Room_Create));
				MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_Kick, new MobaMessageFunc(this.OnGetMsg_Room_Kick));
				MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_CurrData, new MobaMessageFunc(this.OnGetMsg_Room_CurrData));
				MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_ChangeRoomType, new MobaMessageFunc(this.OnGetMsg_Room_ChangeRoomType));
				MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_ComeBack, new MobaMessageFunc(this.OnGetMsg_Room_ComBack));
				MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_ExchangeTeamType, new MobaMessageFunc(this.OnGetMsg_Room_ExchangeTeamType));
				MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_OwnerQuit, new MobaMessageFunc(this.OnGetMsg_Room_OwnerQuit));
				MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_PlayerEscapeCD, new MobaMessageFunc(this.OnGetMsg_Room_PlayerEscapeCD));
				MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_InviteJoinRoom, new MobaMessageFunc(this.OnGetMsg_Room_InviteJoinRoom));
				MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_QueryRoomState, new MobaMessageFunc(this.OnGetMsg_QueryRoomInfo));
				MVC_MessageManager.AddListener_view(MobaGameCode.InviteManger, new MobaMessageFunc(this.OnGetMsg_InviteManger));
				MVC_MessageManager.AddListener_view(MobaGameCode.TeamRoomOperation, new MobaMessageFunc(this.OnGetMsg_TeamRoomOperation));
				MobaMessageManager.RegistMessage(LobbyCode.C2L_JoinQueue, new MobaMessageFunc(this.L2C_JoinQueue));
				MobaMessageManager.RegistMessage((ClientMsg)25062, new MobaMessageFunc(this.OnPeerConnected));
				MobaMessageManager.RegistMessage((ClientMsg)25061, new MobaMessageFunc(this.OnPeerDisconnected));
			}
		}

		public void CancelCallBack()
		{
			if (this._isRegister)
			{
				this._isRegister = false;
				MVC_MessageManager.RemoveListener_view(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg_GetFriendList));
				MVC_MessageManager.RemoveListener_view(MobaFriendCode.Friend_ApplyAddFriend, new MobaMessageFunc(this.OnGetMsg_ApplyAddFriend));
				MVC_MessageManager.RemoveListener_view(MobaGameCode.GetUserInfoBySummId, new MobaMessageFunc(this.OnGetMsg_GetUserInfoBySummId));
				MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_Join, new MobaMessageFunc(this.OnGetMsg_Room_Join));
				MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_Levea, new MobaMessageFunc(this.OnGetMsg_Room_Leave));
				MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_StartGame, new MobaMessageFunc(this.OnGetMsg_Room_StartGame));
				MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_ChangeTeamType, new MobaMessageFunc(this.OnGetMsg_Room_ChangeTeamType));
				MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_Destory, new MobaMessageFunc(this.OnGetMsg_Room_Destory));
				MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_Create, new MobaMessageFunc(this.OnGetMsg_Room_Create));
				MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_Kick, new MobaMessageFunc(this.OnGetMsg_Room_Kick));
				MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_CurrData, new MobaMessageFunc(this.OnGetMsg_Room_CurrData));
				MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_ChangeRoomType, new MobaMessageFunc(this.OnGetMsg_Room_ChangeRoomType));
				MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_ComeBack, new MobaMessageFunc(this.OnGetMsg_Room_ComBack));
				MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_ExchangeTeamType, new MobaMessageFunc(this.OnGetMsg_Room_ExchangeTeamType));
				MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_OwnerQuit, new MobaMessageFunc(this.OnGetMsg_Room_OwnerQuit));
				MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_PlayerEscapeCD, new MobaMessageFunc(this.OnGetMsg_Room_PlayerEscapeCD));
				MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_InviteJoinRoom, new MobaMessageFunc(this.OnGetMsg_Room_InviteJoinRoom));
				MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_QueryRoomState, new MobaMessageFunc(this.OnGetMsg_QueryRoomInfo));
				MVC_MessageManager.RemoveListener_view(MobaGameCode.InviteManger, new MobaMessageFunc(this.OnGetMsg_InviteManger));
				MVC_MessageManager.RemoveListener_view(MobaGameCode.TeamRoomOperation, new MobaMessageFunc(this.OnGetMsg_TeamRoomOperation));
				MobaMessageManager.UnRegistMessage(LobbyCode.C2L_JoinQueue, new MobaMessageFunc(this.L2C_JoinQueue));
				MobaMessageManager.UnRegistMessage((ClientMsg)25062, new MobaMessageFunc(this.OnPeerConnected));
				MobaMessageManager.UnRegistMessage((ClientMsg)25061, new MobaMessageFunc(this.OnPeerDisconnected));
			}
		}

		private void OnPeerDisconnected(MobaMessage msg)
		{
			if (base.IsOpen)
			{
				ScreenLock.Lock("pvproomview.disconn", "连接Game服务器...", false);
			}
		}

		private void DoCheckMsgState(MobaTeamRoomCode _teamCode)
		{
			if (_teamCode != MobaTeamRoomCode.Room_Create)
			{
				if (_teamCode == MobaTeamRoomCode.Room_QueryRoomState)
				{
					this.task_checkQueryMsgState = this.cMgr.StartCoroutine(this.CheckQueryMsgState(), true);
				}
			}
			else
			{
				this.task_checkCreateMsgState = this.cMgr.StartCoroutine(this.CheckCreateMsgState(), true);
			}
		}

		private void UndoCheckMsgState(MobaTeamRoomCode _teamcCode)
		{
			Task t;
			if (_teamcCode != MobaTeamRoomCode.Room_Create)
			{
				if (_teamcCode != MobaTeamRoomCode.Room_QueryRoomState)
				{
					return;
				}
				t = this.task_checkQueryMsgState;
			}
			else
			{
				t = this.task_checkCreateMsgState;
			}
			this.cMgr.StopCoroutine(t);
		}

		[DebuggerHidden]
		private IEnumerator CheckQueryMsgState()
		{
			PvpRoomView.<CheckQueryMsgState>c__Iterator189 <CheckQueryMsgState>c__Iterator = new PvpRoomView.<CheckQueryMsgState>c__Iterator189();
			<CheckQueryMsgState>c__Iterator.<>f__this = this;
			return <CheckQueryMsgState>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator CheckCreateMsgState()
		{
			PvpRoomView.<CheckCreateMsgState>c__Iterator18A <CheckCreateMsgState>c__Iterator18A = new PvpRoomView.<CheckCreateMsgState>c__Iterator18A();
			<CheckCreateMsgState>c__Iterator18A.<>f__this = this;
			return <CheckCreateMsgState>c__Iterator18A;
		}

		private void OnGetMsg_QueryRoomInfo(MobaMessage msg)
		{
			this.UndoCheckMsgState(MobaTeamRoomCode.Room_QueryRoomState);
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			MobaErrorCode mobaErrorCode = (MobaErrorCode)((int)operationResponse.Parameters[1]);
			MobaErrorCode mobaErrorCode2 = mobaErrorCode;
			if (mobaErrorCode2 != MobaErrorCode.UserNotInRoom)
			{
				ScreenLock.Unlock("pvproomview.disconn");
				ClientLogger.Assert(false, "unknown how to handle " + mobaErrorCode);
			}
			else
			{
				this.LeaveRoom();
				ScreenLock.Unlock("pvproomview.disconn");
			}
		}

		private void OnPeerConnected(MobaMessage msg)
		{
			if (base.IsOpen)
			{
				if (!this.SendRoomOperationEvent(MobaTeamRoomCode.Room_QueryRoomState, this.RoomTypeInByte, string.Empty, this.GetRoomId(), (FriendGameType)this.RoomType, this.GetOwnerRoomData().MapId, 5, string.Empty))
				{
					ClientLogger.Error("OnPeerConnected: query room state failed");
				}
				this.DoCheckMsgState(MobaTeamRoomCode.Room_QueryRoomState);
			}
		}

		public override void RefreshUI()
		{
			if (this.IsLock)
			{
				return;
			}
			this.ShowGrayGameBtn(!this.IsRoomOwner());
			this.gameObject.SetActive(true);
			if (this._battleId != null)
			{
				this.UpdateMap(this._battleId);
			}
			this.UpdateKHOrZDYView(this.RoomType);
			if (this._battleId == "80006" || this._isFightWithRobot)
			{
				this.C_ZDYBtn.GetComponent<BoxCollider>().enabled = false;
				this.C_ZDYBtn.GetComponent<UISprite>().color = new Color32(68, 68, 68, 255);
				this.C_ZDYBtn.GetComponentInChildren<UILabel>().color = new Color32(99, 99, 99, 255);
			}
			else
			{
				this.C_ZDYBtn.GetComponent<BoxCollider>().enabled = true;
				this.C_ZDYBtn.GetComponent<UISprite>().color = new Color32(10, 60, 109, 255);
				this.C_ZDYBtn.GetComponentInChildren<UILabel>().color = Color.white;
			}
			this.RenewFriendList();
			this.UpdateLeftBtnState(this._friendOrTeamType, this.RoomType);
			this.UpdateLeftView(this._friendOrTeamType);
		}

		public override void Destroy()
		{
			this.ClearData();
			base.Destroy();
		}

		private bool IsFightWithRobot(string inBattleId)
		{
			if (!StringUtils.CheckValid(inBattleId))
			{
				return false;
			}
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(inBattleId);
			return dataById != null && (dataById.belonged_battletype == 6 || dataById.belonged_battletype == 7 || dataById.belonged_battletype == 8);
		}

		private void EnableSelfDefineGame()
		{
			this.C_ZDYBtn.GetComponent<BoxCollider>().enabled = true;
			this.C_ZDYBtn.GetComponent<UISprite>().color = new Color32(10, 60, 109, 255);
			this.C_ZDYBtn.GetComponentInChildren<UILabel>().color = Color.white;
		}

		private void DisableSelfDefineGame()
		{
			this.C_ZDYBtn.GetComponent<BoxCollider>().enabled = false;
			this.C_ZDYBtn.GetComponent<UISprite>().color = new Color32(68, 68, 68, 255);
			this.C_ZDYBtn.GetComponentInChildren<UILabel>().color = new Color32(99, 99, 99, 255);
		}

		private void ClearData()
		{
			this.C_KHPanel.ResetPosition();
			this.C_ZDYPanel.ResetPosition();
			Singleton<PvpRoomView>.Instance.SetAsRoomOwner(false);
			this._friendOrTeamType = FriendOrTeamType.friendType;
			this.RoomType = PvpRoomType.KaiHei;
			this._friendList = new List<FriendData>();
			this._roomId = null;
			this._noteDict = new List<PvpRoomNote>();
			this.ClearNoteView();
			this.ClearKHOrZDYData();
			this.FindPanel.gameObject.SetActive(false);
		}

		private void UpdateLeftBtnState(FriendOrTeamType choiceType = FriendOrTeamType.friendType, PvpRoomType gameType = PvpRoomType.KaiHei)
		{
			UIToggle arg_1B_0 = this.L_TeamBtn;
			bool flag = choiceType == FriendOrTeamType.friendType;
			this.L_FriendBtn.value = flag;
			arg_1B_0.value = !flag;
			UIToggle arg_3B_0 = this.C_ZDYBtn;
			flag = (gameType == PvpRoomType.KaiHei);
			this.C_KHBtn.value = flag;
			arg_3B_0.value = !flag;
		}

		private void UpdateLeftView(FriendOrTeamType choiceType = FriendOrTeamType.friendType)
		{
			if (choiceType == FriendOrTeamType.friendType)
			{
				if (this.listTask != null)
				{
					this.listTask.Stop();
				}
				this.listTask = this.cMgr.StartCoroutine(this.UpdateFriendGridView(this._friendList), true);
			}
			else
			{
				this.UpdateTeamGridView(this._friendList);
			}
		}

		[DebuggerHidden]
		private IEnumerator UpdateFriendGridView(List<FriendData> friends)
		{
			PvpRoomView.<UpdateFriendGridView>c__Iterator18B <UpdateFriendGridView>c__Iterator18B = new PvpRoomView.<UpdateFriendGridView>c__Iterator18B();
			<UpdateFriendGridView>c__Iterator18B.friends = friends;
			<UpdateFriendGridView>c__Iterator18B.<$>friends = friends;
			<UpdateFriendGridView>c__Iterator18B.<>f__this = this;
			return <UpdateFriendGridView>c__Iterator18B;
		}

		private void UpdateTeamGridView(List<FriendData> friends)
		{
			bool flag = friends == null || friends.Count == 0;
			this.L_FriendPanel.gameObject.SetActive(!flag);
			this.L_FriendLabel.gameObject.SetActive(false);
			this.L_TeamLabel.gameObject.SetActive(flag);
			if (!flag)
			{
				GridHelper.FillGrid<KHFriendItem>(this.L_FriendGrid, this._khFriendItemPrefab, this._friendList.Count, delegate(int idx, KHFriendItem comp)
				{
					comp.gameObject.SetActive(true);
					comp.Init(friends[idx], RoomFriendStatus.Online);
					comp.name = this._friendList[idx].SummId.ToString();
				});
				this.L_FriendGrid.Reposition();
			}
		}

		private void UpdateKHOrZDYView(PvpRoomType gameType = PvpRoomType.KaiHei)
		{
			this.GetRankRoomDataList();
			if (gameType == PvpRoomType.KaiHei)
			{
				this.C_SpriteDrag.scrollView = this.C_KHPanel;
				this.UpdateKHListView(this._teamDataManagement.RoomMemberList, this.GetHeroCount());
			}
			else
			{
				this.C_SpriteDrag.scrollView = this.C_ZDYPanel;
				this.UpdateZdyListView();
			}
		}

		private void UpdateKHListView(List<RoomData> teamList, int num)
		{
			this._recordMatchTypeItems.Clear();
			this.C_Note.gameObject.SetActive(false);
			this.C_ZDYPanel.gameObject.SetActive(false);
			this.C_KHPanel.gameObject.SetActive(true);
			GameObject gameObject;
			if (this.C_KHPanel.transform.childCount == 0)
			{
				gameObject = NGUITools.AddChild(this.C_KHPanel.gameObject, this._matchTypeItemPrefab.gameObject);
			}
			else
			{
				gameObject = this.C_KHPanel.transform.GetChild(0).gameObject;
				gameObject.gameObject.SetActive(true);
			}
			MatchTypeItem component = gameObject.GetComponent<MatchTypeItem>();
			component.OnDelete = new Callback<GameObject>(this.ClickRemoveQueuePlayer);
			component.OnAddFriend = new Callback<GameObject>(this.AddFriend);
			component.Init(teamList, num, MatchTeamType.Blue, this.IsRoomOwner());
			this._recordMatchTypeItems.Add(component);
		}

		private int GetObserserMax()
		{
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(this._battleId);
			if (dataById == null)
			{
				return 3;
			}
			return dataById.viewer_count;
		}

		private void UpdateZdyListView()
		{
			List<RoomData> blueDataList = this.GetBlueDataList();
			List<RoomData> redDataList = this.GetRedDataList();
			List<RoomData> dataListByTeam = this.GetDataListByTeam(3);
			int heroCount = this.GetHeroCount();
			this._recordMatchTypeItems.Clear();
			this.C_Note.gameObject.SetActive(true);
			this.C_ZDYPanel.gameObject.SetActive(true);
			this.C_KHPanel.gameObject.SetActive(false);
			GameObject gameObject;
			if (this.C_ZDYPanel.transform.childCount == 0)
			{
				gameObject = NGUITools.AddChild(this.C_ZDYPanel.gameObject, this._matchTypeItemPrefab.gameObject);
			}
			else
			{
				gameObject = this.C_ZDYPanel.transform.GetChild(0).gameObject;
				gameObject.gameObject.SetActive(true);
			}
			gameObject.name = "MatchTypeItem1";
			MatchTypeItem component = gameObject.GetComponent<MatchTypeItem>();
			component.OnDelete = new Callback<GameObject>(this.ClickRemoveQueuePlayer);
			component.OnAddFriend = new Callback<GameObject>(this.AddFriend);
			component.Init(blueDataList, heroCount, MatchTeamType.Blue, this.IsRoomOwner());
			this._recordMatchTypeItems.Add(component);
			if (this.C_ZDYPanel.transform.childCount < 2)
			{
				gameObject = NGUITools.AddChild(this.C_ZDYPanel.gameObject, this._matchTypeItemPrefab.gameObject);
			}
			else
			{
				gameObject = this.C_ZDYPanel.transform.GetChild(1).gameObject;
				gameObject.gameObject.SetActive(true);
			}
			gameObject.name = "MatchTypeItem2";
			component = gameObject.GetComponent<MatchTypeItem>();
			component.Init(redDataList, heroCount, MatchTeamType.Red, this.IsRoomOwner());
			component.OnDelete = new Callback<GameObject>(this.ClickRemoveQueuePlayer);
			component.OnAddFriend = new Callback<GameObject>(this.AddFriend);
			this._recordMatchTypeItems.Add(component);
			gameObject.transform.localPosition = new Vector3(0f, (float)(-(float)(104 * this.GetHeroCount() + 50)), 0f);
			if (this.C_ZDYPanel.transform.childCount < 3)
			{
				gameObject = NGUITools.AddChild(this.C_ZDYPanel.gameObject, this._matchTypeItemPrefab.gameObject);
			}
			else
			{
				gameObject = this.C_ZDYPanel.transform.GetChild(2).gameObject;
				gameObject.gameObject.SetActive(true);
			}
			gameObject.name = "MatchTypeItem3";
			component = gameObject.GetComponent<MatchTypeItem>();
			component.Init(dataListByTeam, this.GetObserserMax(), MatchTeamType.Gray, this.IsRoomOwner());
			component.OnDelete = new Callback<GameObject>(this.ClickRemoveQueuePlayer);
			component.OnAddFriend = new Callback<GameObject>(this.AddFriend);
			this._recordMatchTypeItems.Add(component);
			gameObject.transform.localPosition = new Vector3(0f, (float)(-(float)(104 * this.GetHeroCount() * 2 + 50 + 50)), 0f);
		}

		private void UpdateMap(string num)
		{
			switch (num)
			{
			case "80001":
			case "80008":
			case "80009":
			case "80010":
				this.R_Map.FindChild("80001").gameObject.SetActive(true);
				this.R_Map.FindChild("80003").gameObject.SetActive(false);
				this.R_Map.FindChild("80005").gameObject.SetActive(false);
				this.R_Map.FindChild("800055").gameObject.SetActive(false);
				break;
			case "80003":
			case "80011":
			case "80012":
			case "80013":
				this.R_Map.FindChild("80001").gameObject.SetActive(false);
				this.R_Map.FindChild("80003").gameObject.SetActive(true);
				this.R_Map.FindChild("80005").gameObject.SetActive(false);
				this.R_Map.FindChild("800055").gameObject.SetActive(false);
				break;
			case "80005":
				this.R_Map.FindChild("80001").gameObject.SetActive(false);
				this.R_Map.FindChild("80003").gameObject.SetActive(false);
				this.R_Map.FindChild("80005").gameObject.SetActive(true);
				this.R_Map.FindChild("800055").gameObject.SetActive(false);
				break;
			case "800055":
			case "80006":
			case "80014":
			case "80015":
			case "80016":
				this.R_Map.FindChild("80001").gameObject.SetActive(false);
				this.R_Map.FindChild("80003").gameObject.SetActive(false);
				this.R_Map.FindChild("80005").gameObject.SetActive(false);
				this.R_Map.FindChild("800055").gameObject.SetActive(true);
				break;
			}
		}

		private void ClearKHOrZDYData()
		{
			for (int i = 0; i < this._recordMatchTypeItems.Count; i++)
			{
				this._recordMatchTypeItems[i].ClearCurData();
			}
		}

		private void ShowGrayGameBtn(bool isGray)
		{
			this.R_GrayGoToGameBtn.gameObject.SetActive(isGray);
		}

		private void UpdateNote()
		{
			GridHelper.FillGrid<UILabel>(this.R_NoteGrid, this._noteItemPrefab, this._noteDict.Count, delegate(int idx, UILabel comp)
			{
				comp.gameObject.SetActive(true);
				comp.text = ">" + this._noteDict[idx].HeroName + ":" + ((!this._noteDict[idx].IsOut) ? ("[00ff00]" + LanguageManager.Instance.GetStringById("GangUpUI_EnterRoom")) : ("[ff0000]" + LanguageManager.Instance.GetStringById("GangUpUI_ExitRoom")));
			});
			this.R_NoteGrid.Reposition();
		}

		private void UpdateNoteDict(PvpRoomNote note)
		{
			if (this._noteDict.Count >= 4)
			{
				this._noteDict.Remove(this._noteDict[0]);
			}
			this._noteDict.Add(note);
			this.UpdateNote();
		}

		private void ClearNoteView()
		{
			if (this.R_NoteGrid == null)
			{
				return;
			}
			for (int i = 0; i < this.R_NoteGrid.transform.childCount; i++)
			{
				this.R_NoteGrid.transform.GetChild(i).gameObject.SetActive(false);
			}
			this._noteDict.Clear();
		}

		private List<FriendData> GetOnlineFriends()
		{
			return this._friendList.FindAll((FriendData obj) => (int)obj.GameStatus == 1);
		}

		private List<RoomData> GetBlueDataList()
		{
			return this._teamDataManagement.RoomMemberList.FindAll((RoomData obj) => obj.TeamType == 1);
		}

		private List<RoomData> GetRedDataList()
		{
			return this._teamDataManagement.RoomMemberList.FindAll((RoomData obj) => obj.TeamType == 2);
		}

		private List<RoomData> GetDataListByTeam(int teamType)
		{
			return this._teamDataManagement.RoomMemberList.FindAll((RoomData obj) => (int)obj.TeamType == teamType);
		}

		private RoomData GetOwnerRoomData()
		{
			return this._teamDataManagement.RoomMemberList.Find((RoomData obj) => obj.IsHomeMain);
		}

		private bool IsRoomOwner()
		{
			string userId = ModelManager.Instance.Get_userData_X().UserId;
			return this._teamDataManagement.IsEmpty() || this.GetOwnerRoomData().UserId == userId;
		}

		private bool IsSelf(string userId)
		{
			return ModelManager.Instance.Get_userData_X().UserId == userId;
		}

		private string GetRoomId()
		{
			string result = string.Empty;
			if (!this._teamDataManagement.IsEmpty())
			{
				result = this._teamDataManagement.RoomMemberList[0].RoomId;
			}
			return result;
		}

		private List<MatchItem> GetCurTypeMacthItems()
		{
			List<MatchItem> list = new List<MatchItem>();
			for (int i = 0; i < this._recordMatchTypeItems.Count; i++)
			{
				list.AddRange(this._recordMatchTypeItems[i].TeamObjList);
			}
			return list;
		}

		private int GetHeroCount()
		{
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(this._battleId);
			if (this._battleId == "80006")
			{
				return 2;
			}
			if (dataById == null)
			{
				return 3;
			}
			return dataById.hero1_number_cap;
		}

		private RoomFriendStatus GetFriendNetType(sbyte gameStatus)
		{
			switch (gameStatus + 1)
			{
			case 0:
				return RoomFriendStatus.Refuse;
			case 1:
				return RoomFriendStatus.Offline;
			case 2:
			case 4:
				return RoomFriendStatus.Online;
			case 8:
				return RoomFriendStatus.InRoom;
			}
			return RoomFriendStatus.Playing;
		}

		private void GetRankRoomDataList()
		{
			RoomData ownerRoomData = this.GetOwnerRoomData();
			ClientLogger.AssertNotNull(ownerRoomData, null);
			if (ownerRoomData != null)
			{
				this._teamDataManagement.Remove(ownerRoomData);
				this._teamDataManagement.Insert(0, ownerRoomData);
			}
		}

		private void RenewFriendList()
		{
			this._friendList = new List<FriendData>(ModelManager.Instance.Get_FriendDataList_X().ToArray());
		}

		public PvpRoomType GetRoomType()
		{
			return this.RoomType;
		}

		public void SetMatchInfo(string battleId, bool isRoomOwner, FriendOrTeamType type1, PvpRoomType type2)
		{
			this._battleId = battleId;
			this._isFightWithRobot = this.IsFightWithRobot(this._battleId);
			this.SetAsRoomOwner(isRoomOwner);
			this._friendOrTeamType = type1;
			this.RoomType = type2;
		}

		public void SetAsRoomOwner(bool isOwner)
		{
			this._isRoomOwner = isOwner;
		}

		public void UpdateFriendListView()
		{
			if (this.listTask != null)
			{
				this.listTask.Stop();
			}
			this.listTask = this.cMgr.StartCoroutine(this.UpdateFriendGridView(this._friendList), true);
		}

		private string TeamTypeToTargetId(MatchTeamType teamType)
		{
			if (teamType == MatchTeamType.Blue)
			{
				return "1";
			}
			if (teamType == MatchTeamType.Red)
			{
				return "2";
			}
			return "3";
		}

		public void ReplaceMatchPosition(MatchItem matchItem)
		{
			List<MatchItem> curTypeMacthItems = this.GetCurTypeMacthItems();
			if (curTypeMacthItems.Count == 0)
			{
				ClientLogger.Error("MatchList is null");
				return;
			}
			for (int i = 0; i < curTypeMacthItems.Count; i++)
			{
				curTypeMacthItems[i].IsShowFrame(false);
			}
			List<MatchItem> list = (from obj in curTypeMacthItems
			orderby Mathf.Abs(obj.transform.position.y - matchItem.transform.position.y)
			select obj).ToList<MatchItem>();
			MatchItem matchItem2 = curTypeMacthItems.FirstOrDefault((MatchItem obj) => obj.recordUseId == matchItem.recordUseId);
			if (matchItem2 == null)
			{
				ClientLogger.Error("matchItem is error===is null");
				return;
			}
			if (matchItem2 == list[0])
			{
				return;
			}
			RoomData recordRoomData = matchItem2.recordRoomData;
			InviteType recordInviteType = matchItem2.recordInviteType;
			bool recordHoner = matchItem2.recordHoner;
			MatchTeamType recordTeamType = matchItem2.recordTeamType;
			MatchTeamType recordTeamType2 = list[0].recordTeamType;
			matchItem2.Init(list[0].recordRoomData, list[0].recordInviteType, list[0].recordHoner, list[0].recordTeamType);
			list[0].Init(recordRoomData, recordInviteType, recordHoner, recordTeamType);
			this.ReplacePositionNews(list[0].recordTeamType != matchItem2.recordTeamType, matchItem.recordUseId, matchItem2.recordUseId, this.TeamTypeToTargetId(recordTeamType2));
		}

		public void ShowMatchItemFrame(MatchItem matchItem)
		{
			List<MatchItem> curTypeMacthItems = this.GetCurTypeMacthItems();
			if (curTypeMacthItems.Count == 0)
			{
				ClientLogger.Error("MatchList is null");
				return;
			}
			List<MatchItem> list = (from obj in curTypeMacthItems
			orderby Mathf.Abs(obj.transform.position.y - matchItem.transform.position.y)
			select obj).ToList<MatchItem>();
			for (int i = 0; i < curTypeMacthItems.Count; i++)
			{
				if (curTypeMacthItems[i] != list[0])
				{
					curTypeMacthItems[i].IsShowFrame(false);
				}
				else
				{
					list[0].IsShowFrame(true);
				}
			}
		}

		public void Glide(Transform trans)
		{
			if (this.RoomType == PvpRoomType.KaiHei)
			{
				return;
			}
			Vector3[] localCorners = this.C_ZDYPanel.GetComponent<UIPanel>().localCorners;
			Bounds bounds = this.C_ZDYPanel.bounds;
			float num = this.C_Up.position.y - (float)this.C_Up.GetComponent<UIWidget>().height / (float)Screen.height - trans.position.y;
			if (num <= 0f)
			{
				if (bounds.max.y <= localCorners[1].y)
				{
					return;
				}
				if (this.C_ProgressBar.value > 0f)
				{
					this.C_ProgressBar.value = this.C_ProgressBar.value - 0.05f;
				}
				if (this.C_ProgressBar.value <= 0f)
				{
					this.C_ProgressBar.value = 0f;
				}
			}
			num = this.C_Down.position.y + (float)this.C_Down.GetComponent<UIWidget>().height / (float)Screen.height - trans.position.y;
			if (num >= 0f)
			{
				if (bounds.min.y >= localCorners[0].y)
				{
					return;
				}
				if (this.C_ProgressBar.value < 1f)
				{
					this.C_ProgressBar.value = this.C_ProgressBar.value + 0.05f;
				}
				if (this.C_ProgressBar.value >= 1f)
				{
					this.C_ProgressBar.value = 1f;
				}
			}
		}

		public bool JudgeRelativePosition(GameObject obj, bool isUp)
		{
			if (obj.transform.position.y >= this.C_Up.position.y && isUp)
			{
				obj.transform.position = new Vector3(obj.transform.position.x, this.C_Up.position.y, obj.transform.position.z);
				return false;
			}
			if (obj.transform.position.y <= this.C_Down.position.y && !isUp)
			{
				obj.transform.position = new Vector3(obj.transform.position.x, this.C_Down.position.y, obj.transform.position.z);
				return false;
			}
			return true;
		}

		public void InviteGoToRoom(string note)
		{
			this.RegisterCallBack();
			this._roomId = note.Split(new char[]
			{
				'|'
			})[1];
			string str = note.Split(new char[]
			{
				'|'
			})[0];
			string summerId = note.Split(new char[]
			{
				'|'
			})[2];
			Singleton<InvitationView>.Instance.AddInvitation(this._roomId, LanguageManager.Instance.GetStringById("FriendsUI_TeamInvited"), str + LanguageManager.Instance.GetStringById("FriendsUI_InviteYouToJoinGame"), summerId, InvitateType.KHOrZDY);
		}

		public void RefuseGoToRoom(string note)
		{
			if (this.transform == null)
			{
				return;
			}
			if (note.Split(new char[]
			{
				'|'
			}) != null && note.Split(new char[]
			{
				'|'
			}).Length >= 3)
			{
				this._roomId = note.Split(new char[]
				{
					'|'
				})[1];
				string text = note.Split(new char[]
				{
					'|'
				})[0];
				string name = note.Split(new char[]
				{
					'|'
				})[2];
				KHFriendItem component = this.L_FriendGrid.transform.Find(name).GetComponent<KHFriendItem>();
				if (component == null)
				{
					return;
				}
				component.SetGameState(this.GetFriendNetType(-1));
			}
		}

		public void InvitationToRoom(string roomId, bool isAccept, string targetSummerId)
		{
			this._roomId = roomId;
			this.RegisterCallBack();
			if (isAccept)
			{
				if (!this.SendRoomOperationEvent(MobaTeamRoomCode.Room_Join, this.RoomTypeInByte, string.Empty, this._roomId, FriendGameType.KH, "8002", 5, string.Empty))
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_Tips_JoinTheRoom"), 1f);
				}
			}
			else
			{
				long num = 0L;
				if (!long.TryParse(targetSummerId, out num))
				{
					ClientLogger.Error("GoToRoom: parse id failed " + targetSummerId);
					return;
				}
				if (!SendMsgManager.Instance.SendChannelMsg(MobaChannel.Team, 16, null, new object[]
				{
					num,
					roomId
				}))
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_Tips_RefuseToJoinTheRoom"), 1f);
				}
			}
		}

		private void OnGetMsg_RoomsManager(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				if (mobaErrorCode != MobaErrorCode.TableNotExist)
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_Tips_JoinTheRoom"), 1f);
				}
				else
				{
					Singleton<TipView>.Instance.ShowViewSetText("房间已经不存在", 1f);
				}
			}
		}

		private void OnGetMsg_InviteManger(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_Tips_RefuseToJoinTheRoom"), 1f);
			}
		}

		public void UpdateApplyState(string text = null)
		{
			if (Singleton<PvpRoomView>.Instance.IsOpened)
			{
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_RefreshData"), true, 15f);
				SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_GetFriendList, param, new object[0]);
			}
		}

		public bool IsFriend(string accountId)
		{
			long num = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			if (accountId == num.ToString())
			{
				return true;
			}
			FriendData friendData = this._friendList.FirstOrDefault((FriendData obj) => obj.SummId.ToString() == accountId);
			return friendData != null;
		}

		private void ClickL_FriendBtn(GameObject obj = null)
		{
			if (this._friendOrTeamType == FriendOrTeamType.friendType)
			{
				return;
			}
			this._friendOrTeamType = FriendOrTeamType.friendType;
			this.UpdateLeftView(this._friendOrTeamType);
		}

		private void ClickL_TeamBtn(GameObject obj = null)
		{
			UIToggle arg_20_0 = this.L_FriendBtn;
			bool flag = this._friendOrTeamType == FriendOrTeamType.teamType;
			this.L_TeamBtn.value = flag;
			arg_20_0.value = !flag;
			Singleton<TipView>.Instance.ShowViewSetText("===暂未开放===", 1f);
		}

		private void ClickL_AddFriendsBtn(GameObject obj = null)
		{
			if (ModelManager.Instance.Get_FriendDataList_X().Count >= 50)
			{
				Singleton<TipView>.Instance.ShowViewSetText("对不起，您的好友数目已达上限", 1f);
				return;
			}
			this.FindPanel.gameObject.SetActive(true);
			this.F_Item.gameObject.SetActive(false);
			this.F_AddBtn.gameObject.SetActive(false);
			this.F_Input.value = string.Empty;
			this.MyMobaID.text = ModelManager.Instance.Get_userData_X().SummonerId.ToString();
		}

		private void ClickF_FindBtn(GameObject obj = null)
		{
			if (string.IsNullOrEmpty(this.F_Input.value))
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_PleaseInputSummonerID"), 1f);
				return;
			}
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("GangUpUI_Tips_LookingForSummonInformation"), true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetUserInfoBySummId, param, new object[]
			{
				long.Parse(this.F_Input.value)
			});
		}

		private void ClickF_AddBtn(GameObject obj = null)
		{
			long num = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			if (this.F_Input.value == string.Empty)
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_PleaseInputSummonerID"), 1f);
				return;
			}
			if (this.F_Input.value == num.ToString())
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_CanNotAddYouself"), 1f);
				return;
			}
			if (ModelManager.Instance.Get_FriendDataList_X().Find((FriendData item) => item.TargetId.ToString() == this.F_Input.value) != null)
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_HeIsYouFriend"), 1f);
				return;
			}
			this.F_AddBtn.GetComponent<TweenScale>().PlayForward();
			this.F_AddBtn.FindChild("fly").gameObject.SetActive(true);
			TweenPosition component = this.F_AddBtn.transform.FindChild("fly").GetComponent<TweenPosition>();
			component.ResetToBeginning();
			TweenAlpha component2 = this.F_AddBtn.transform.FindChild("fly").GetComponent<TweenAlpha>();
			component2.ResetToBeginning();
			component.PlayForward();
			component2.PlayForward();
			this.F_AddBtn.GetComponent<UISprite>().spriteName = "Add_friend_icons_friend_02";
			this.F_AddBtn.Find("Label").GetComponent<UILabel>().color = new Color32(31, 249, 116, 255);
			this.F_AddBtn.Find("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_HasBeenSend");
			UIEventListener.Get(this.F_AddBtn.gameObject).onClick = null;
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("FriendsUI_Button_AddFriend"), true, 15f);
			SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_ApplyAddFriend, param, new object[]
			{
				long.Parse(this.F_Input.value)
			});
		}

		private void ClickF_BackBtn(GameObject obj = null)
		{
			this.FindPanel.gameObject.SetActive(false);
		}

		private void ClickDelelteBtn(GameObject obj = null)
		{
			this.F_Input.value = null;
		}

		private void ClickL_JoinTeamBtn(GameObject obj = null)
		{
		}

		private void ClickL_Chat(GameObject obj = null)
		{
		}

		private void ClickC_KHBtn(GameObject obj = null)
		{
			if (this.RoomType == PvpRoomType.KaiHei)
			{
				return;
			}
			if (!this.IsRoomOwner())
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_CannotSwitchMode"), 1f);
				UIToggle arg_56_0 = this.C_ZDYBtn;
				bool flag = this.RoomType == PvpRoomType.KaiHei;
				this.C_KHBtn.value = flag;
				arg_56_0.value = !flag;
				return;
			}
			if (this._teamDataManagement.RoomMemberList.Count > this.GetHeroCount())
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_Tips_CanNotSwitch"), 1f);
				this.C_ZDYBtn.value = true;
			}
			else
			{
				this.ClearKHOrZDYData();
				this.SendRoomOperationEvent(MobaTeamRoomCode.Room_ChangeRoomType, this.RoomTypeInByte, string.Empty, this.GetRoomId(), FriendGameType.KH, "8002", 5, string.Empty);
				this.RoomType = PvpRoomType.KaiHei;
				this.UpdateKHOrZDYView(this.RoomType);
				this.C_KHPanel.ResetPosition();
				this.C_KHPanel.Scroll(0.01f);
				this.C_ProgressBar.value = 0f;
			}
		}

		private void ClickC_ZDYBtn(GameObject obj = null)
		{
			if (this.RoomType == PvpRoomType.ZiDingYi)
			{
				return;
			}
			if (!this.IsRoomOwner())
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_CannotSwitchMode"), 1f);
				UIToggle arg_56_0 = this.C_ZDYBtn;
				bool flag = this.RoomType == PvpRoomType.KaiHei;
				this.C_KHBtn.value = flag;
				arg_56_0.value = !flag;
				return;
			}
			this.ClearKHOrZDYData();
			this.SendRoomOperationEvent(MobaTeamRoomCode.Room_ChangeRoomType, this.RoomTypeInByte, string.Empty, this.GetRoomId(), FriendGameType.KH, "8002", 5, string.Empty);
			this.RoomType = PvpRoomType.ZiDingYi;
			this.UpdateKHOrZDYView(this.RoomType);
			this.C_ZDYPanel.ResetPosition();
			this.C_ZDYPanel.Scroll(0.01f);
			this.C_ProgressBar.value = 0f;
		}

		private void ClickR_BackRoomBtn(GameObject obj = null)
		{
			if (this._teamDataManagement == null)
			{
				return;
			}
			string targetId = ModelManager.Instance.Get_userData_filed_X("UserId");
			if (!this.SendRoomOperationEvent(MobaTeamRoomCode.Room_Levea, this.RoomTypeInByte, targetId, this.GetRoomId(), (FriendGameType)this.RoomType, this.GetOwnerRoomData().MapId, 5, string.Empty))
			{
				Singleton<TipView>.Instance.SetText(LanguageManager.Instance.GetStringById("GangUpUI_Tips_LeaveUnsuccessful"), 0f);
			}
		}

		private void ClickR_GoToGameBtn(GameObject obj = null)
		{
			if (GlobalSettings.Instance.PvpSetting.DirectLinkLobby)
			{
				Singleton<PvpManager>.Instance.ChooseSingleGame(this.GetOwnerRoomData().MapId, "排队中");
				CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
				return;
			}
			if (!this.IsRoomOwner())
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_Tips_NotOwnerCanNotStartGame"), 1f);
				CtrlManager.CloseWindow(WindowID.TipView);
				return;
			}
			if (this.RoomType == PvpRoomType.ZiDingYi)
			{
				if (this.GetBlueDataList().Count <= 0 || this.GetRedDataList().Count <= 0)
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_NeedOpponent"), 1f);
					return;
				}
				if (this.GetBlueDataList().Count != this.GetRedDataList().Count)
				{
					CtrlManager.ShowMsgBox("提示", "双方人数不均，是否确定开始游戏？", new Action<bool>(this.ReComfirm), PopViewType.PopTwoButton, "确定", "取消", null);
					return;
				}
			}
			if (this.SendRoomOperationEvent(MobaTeamRoomCode.Room_StartGame, this.RoomTypeInByte, string.Empty, this.GetRoomId(), (FriendGameType)this.RoomType, this.GetOwnerRoomData().MapId, 5, string.Empty))
			{
				MobaMessageManagerTools.BeginWaiting_manual("WaitingQueue", "正在尝试进入队列...", true, 15f, true);
				this.cMgr.StartCoroutine(this.CheckJoinQueue(), true);
			}
			else
			{
				Singleton<TipView>.Instance.SetText(LanguageManager.Instance.GetStringById("GangUpUI_Tips_EntryIsNotSuccessful"), 0f);
			}
		}

		private void ReComfirm(bool isOK)
		{
			if (isOK)
			{
				if (this.SendRoomOperationEvent(MobaTeamRoomCode.Room_StartGame, this.RoomTypeInByte, string.Empty, this.GetRoomId(), (FriendGameType)this.RoomType, this.GetOwnerRoomData().MapId, 5, string.Empty))
				{
					MobaMessageManagerTools.BeginWaiting_manual("WaitingQueue", "正在尝试进入队列...", true, 15f, true);
					this.cMgr.StartCoroutine(this.CheckJoinQueue(), true);
				}
				else
				{
					Singleton<TipView>.Instance.SetText(LanguageManager.Instance.GetStringById("GangUpUI_Tips_EntryIsNotSuccessful"), 0f);
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator CheckJoinQueue()
		{
			PvpRoomView.<CheckJoinQueue>c__Iterator18C <CheckJoinQueue>c__Iterator18C = new PvpRoomView.<CheckJoinQueue>c__Iterator18C();
			<CheckJoinQueue>c__Iterator18C.<>f__this = this;
			return <CheckJoinQueue>c__Iterator18C;
		}

		private void RetryJoinQueue(bool isRetry)
		{
			this.StopCoroutine();
			if (isRetry)
			{
				MobaMessageManagerTools.BeginWaiting_manual("WaitingQueue", "正在尝试进入队列...", true, 15f, true);
				this.cMgr.StartCoroutine(this.CheckJoinQueue(), true);
			}
			else
			{
				Singleton<MenuBottomBarView>.Instance.HidePlayBtnEffect();
				CtrlManager.CloseWindow(WindowID.PvpRoomView);
				CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
			}
		}

		private void L2C_JoinQueue(MobaMessage msg)
		{
			this.StopCoroutine();
		}

		public void StopWaitingQueue()
		{
			MobaMessageManagerTools.EndWaiting_manual("WaitingQueue");
			this.cMgr.StopAllCoroutine();
		}

		private void StopCoroutine()
		{
			MobaMessageManagerTools.EndWaiting_manual("WaitingQueue");
			this.cMgr.StopAllCoroutine();
		}

		public void ClickAddFriend(GameObject obj = null)
		{
			if (!this.IsRoomOwner())
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_CannotInvitFriend"), 1f);
				return;
			}
			string name = obj.transform.parent.name;
			if (!string.IsNullOrEmpty(name) && !SendMsgManager.Instance.SendMsg(MobaTeamRoomCode.Room_InviteJoinRoom, null, new object[]
			{
				long.Parse(name),
				this.GetRoomId()
			}))
			{
				Singleton<TipView>.Instance.SetText(LanguageManager.Instance.GetStringById("GangUpUI_Tips_InvitationFailure"), 0f);
				return;
			}
			KHFriendItem componentInParent = obj.transform.GetComponentInParent<KHFriendItem>();
			if (componentInParent != null)
			{
				componentInParent.InviteToRoom();
			}
		}

		public void AutoInviteFriend(string sumID)
		{
			if (!this.IsRoomOwner())
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_CannotInvitFriend"), 1f);
				return;
			}
			if (!string.IsNullOrEmpty(sumID) && !SendMsgManager.Instance.SendMsg(MobaTeamRoomCode.Room_InviteJoinRoom, null, new object[]
			{
				long.Parse(sumID),
				this.GetRoomId()
			}))
			{
				Singleton<TipView>.Instance.SetText(LanguageManager.Instance.GetStringById("GangUpUI_Tips_InvitationFailure"), 0f);
				return;
			}
		}

		public void ClickRemoveQueuePlayer(GameObject obj = null)
		{
			if (!this.IsRoomOwner())
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_Tips_NotOwnerCanNotKick"), 1f);
				return;
			}
			RoomData recordRoomData = obj.transform.GetComponentInParent<MatchItem>().recordRoomData;
			if (!this.SendRoomOperationEvent(MobaTeamRoomCode.Room_Kick, (byte)this.RoomType, recordRoomData.UserId, this.GetRoomId(), (FriendGameType)this.RoomType, this.GetOwnerRoomData().MapId, 5, string.Empty))
			{
				Singleton<TipView>.Instance.SetText(LanguageManager.Instance.GetStringById("GangUpUI_Tips_KickFailed"), 0f);
				return;
			}
			this._teamDataManagement.Remove(recordRoomData);
			this.RefreshUI();
		}

		private void AddFriend(GameObject obj = null)
		{
			MatchItem componentInParent = obj.GetComponentInParent<MatchItem>();
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("FriendsUI_Button_AddFriend"), true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetUserInfoBySummId, param, new object[]
			{
				long.Parse(componentInParent.recordRoomData.AccountId)
			});
		}

		private void ReplacePositionNews(bool isReplace, string useIdSelf, string useIdTarget, string target2)
		{
			if (isReplace)
			{
				if (!string.IsNullOrEmpty(useIdTarget))
				{
					this.SendRoomOperationEvent(MobaTeamRoomCode.Room_ExchangeTeamType, (byte)this.RoomType, useIdSelf, this.GetRoomId(), (FriendGameType)this.RoomType, this.GetOwnerRoomData().MapId, 5, useIdTarget);
				}
				else
				{
					this.SendRoomOperationEvent(MobaTeamRoomCode.Room_ChangeTeamType, (byte)this.RoomType, useIdSelf, this.GetRoomId(), (FriendGameType)this.RoomType, this.GetOwnerRoomData().MapId, 5, target2);
				}
			}
			else
			{
				this.RefreshUI();
			}
		}

		private bool SendRoomOperationEvent(MobaTeamRoomCode code, byte roomType, string targetId, string roomId, FriendGameType friendType = FriendGameType.KH, string str = "8002", int playerMax = 5, string targetId2 = "")
		{
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在进行房间操作", true, 60f);
			if (code == MobaTeamRoomCode.Room_Create)
			{
				return SendMsgManager.Instance.SendChannelMsg(MobaChannel.Team, (byte)code, param, new object[]
				{
					roomType,
					targetId,
					roomId,
					friendType,
					str,
					playerMax,
					targetId2
				});
			}
			return SendMsgManager.Instance.SendChannelMsg(MobaChannel.Team, (byte)code, null, new object[]
			{
				roomType,
				targetId,
				roomId,
				friendType,
				str,
				playerMax,
				targetId2
			});
		}

		private void OnGetMsg_TeamRoomOperation(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			MobaMessageManagerTools.EndWaiting_manual("RoomsManager");
			int num = (int)operationResponse.Parameters[1];
			MobaTeamRoomCode mobaTeamRoomCode = (MobaTeamRoomCode)((byte)operationResponse.Parameters[175]);
			if (num == 30108)
			{
				PvpMatchMgr.Instance.PunishEscape((string)operationResponse[59], (float)operationResponse[101]);
				return;
			}
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			switch (mobaErrorCode)
			{
			case MobaErrorCode.GuaJiPunishedSelf:
				if (mobaTeamRoomCode == MobaTeamRoomCode.Room_Create)
				{
					string text = LanguageManager.Instance.GetStringById("Tips_Hung_Punish_Content1", "惩罚时间中");
					text = text.Replace("*", ModelManager.Instance.Get_PunishmentEndTime().ToString("yyyy.MM.dd HH:mm"));
					Singleton<TipView>.Instance.ShowViewSetText(text, 1f);
				}
				this.StopCoroutine();
				break;
			case MobaErrorCode.GuaJiPunishedTarget:
				if (mobaTeamRoomCode == MobaTeamRoomCode.Room_InviteJoinRoom)
				{
					Singleton<TipView>.Instance.ShowViewSetText("对方正处于系统惩罚时间内，暂时不能参加 PVP 比赛。", 1f);
				}
				this.StopCoroutine();
				break;
			case MobaErrorCode.GuaJiPunished:
				if (mobaTeamRoomCode == MobaTeamRoomCode.Room_StartGame)
				{
					string arg = operationResponse.Parameters[57] as string;
					Singleton<TipView>.Instance.ShowViewSetText(string.Format("{0}正处于系统惩罚时间内，暂时不能参加 PVP 比赛。", arg), 1f);
				}
				this.StopCoroutine();
				break;
			default:
				if (mobaErrorCode != MobaErrorCode.Ok)
				{
					if (mobaErrorCode != MobaErrorCode.LobbyShutDown)
					{
						if (mobaErrorCode != MobaErrorCode.RoomPlayerHeroCountNotEnough && mobaErrorCode != MobaErrorCode.LevelError)
						{
							this.ReceiveRoomOperationCallBack(num, operationResponse.DebugMessage, mobaTeamRoomCode, null);
						}
						else
						{
							string arg2 = operationResponse.Parameters[57] as string;
							Singleton<TipView>.Instance.ShowViewSetText(string.Format("{0}英雄数量不足5个", arg2), 1f);
							this.StopCoroutine();
						}
					}
					else
					{
						CtrlManager.ShowMsgBox("匹配失败", "匹配服务器维护中，请稍后再试…", delegate
						{
						}, PopViewType.PopOneButton, "确定", "取消", null);
						this.StopCoroutine();
					}
				}
				else
				{
					List<RoomData> roomDataList = null;
					if (operationResponse.Parameters.ContainsKey(2))
					{
						byte[] buffer = operationResponse.Parameters[2] as byte[];
						roomDataList = SerializeHelper.Deserialize<List<RoomData>>(buffer);
					}
					this.ReceiveRoomOperationCallBack(num, operationResponse.DebugMessage, mobaTeamRoomCode, roomDataList);
				}
				break;
			}
		}

		private void OnGetMsg_Room_Join(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			operationResponse.Parameters[175] = MobaTeamRoomCode.Room_Join;
			this.OnGetMsg_TeamRoomOperation(msg);
		}

		private void OnGetMsg_Room_Leave(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			operationResponse.Parameters[175] = MobaTeamRoomCode.Room_Levea;
			this.OnGetMsg_TeamRoomOperation(msg);
		}

		private void OnGetMsg_Room_StartGame(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			operationResponse.Parameters[175] = MobaTeamRoomCode.Room_StartGame;
			this.OnGetMsg_TeamRoomOperation(msg);
		}

		private void OnGetMsg_Room_ChangeTeamType(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			operationResponse.Parameters[175] = MobaTeamRoomCode.Room_ChangeTeamType;
			this.OnGetMsg_TeamRoomOperation(msg);
		}

		private void OnGetMsg_Room_Destory(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			operationResponse.Parameters[175] = MobaTeamRoomCode.Room_Destory;
			this.OnGetMsg_TeamRoomOperation(msg);
		}

		private void OnGetMsg_Room_Create(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			operationResponse.Parameters[175] = MobaTeamRoomCode.Room_Create;
			this.OnGetMsg_TeamRoomOperation(msg);
		}

		private void OnGetMsg_Room_Kick(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			operationResponse.Parameters[175] = MobaTeamRoomCode.Room_Kick;
			this.OnGetMsg_TeamRoomOperation(msg);
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("ServerResponse_WaitingServerResponse"), true, 15f);
			SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_GetFriendList, param, new object[0]);
		}

		private void OnGetMsg_Room_CurrData(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			operationResponse.Parameters[175] = MobaTeamRoomCode.Room_CurrData;
			this.OnGetMsg_TeamRoomOperation(msg);
		}

		private void OnGetMsg_Room_ChangeRoomType(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			operationResponse.Parameters[175] = MobaTeamRoomCode.Room_ChangeRoomType;
			this.OnGetMsg_TeamRoomOperation(msg);
		}

		private void OnGetMsg_Room_ComBack(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			operationResponse.Parameters[175] = MobaTeamRoomCode.Room_ComeBack;
			this.OnGetMsg_TeamRoomOperation(msg);
		}

		private void OnGetMsg_Room_ExchangeTeamType(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			operationResponse.Parameters[175] = MobaTeamRoomCode.Room_ExchangeTeamType;
			this.OnGetMsg_TeamRoomOperation(msg);
		}

		private void OnGetMsg_Room_OwnerQuit(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			operationResponse.Parameters[175] = MobaTeamRoomCode.Room_OwnerQuit;
			this.OnGetMsg_TeamRoomOperation(msg);
		}

		private void OnGetMsg_Room_PlayerEscapeCD(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			operationResponse.Parameters[175] = MobaTeamRoomCode.Room_PlayerEscapeCD;
			this.OnGetMsg_TeamRoomOperation(msg);
		}

		private void OnGetMsg_Room_InviteJoinRoom(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			operationResponse.Parameters[175] = MobaTeamRoomCode.Room_InviteJoinRoom;
			this.OnGetMsg_TeamRoomOperation(msg);
		}

		private void ReceiveRoomOperationCallBack(int idx, string str, MobaTeamRoomCode code, List<RoomData> roomDataList)
		{
			Singleton<TipView>.Instance.GetErrorInformation(idx);
			if (idx == 0)
			{
				switch (code)
				{
				case MobaTeamRoomCode.Room_Join:
					this.Room_JoinCallBack(roomDataList);
					goto IL_130;
				case MobaTeamRoomCode.Room_Levea:
					this.Room_LeaveCallBack(roomDataList);
					goto IL_130;
				case MobaTeamRoomCode.Room_StartGame:
					this.Room_StartGameCallBack(roomDataList);
					goto IL_130;
				case MobaTeamRoomCode.Room_ChangeTeamType:
					this.Room_ChangeTeamTypeCallBack(roomDataList);
					goto IL_130;
				case MobaTeamRoomCode.Room_Destory:
					if (PvpMatchMgr.State != PvpMatchState.Matched)
					{
						Singleton<PvpManager>.Instance.ClearBattleInfo();
					}
					this.Room_DestoryCallBack(roomDataList);
					goto IL_130;
				case MobaTeamRoomCode.Room_Create:
					this.Room_CreateCallBack(roomDataList);
					goto IL_130;
				case MobaTeamRoomCode.Room_Kick:
					this.Room_KickCallBack(roomDataList);
					goto IL_130;
				case MobaTeamRoomCode.Room_CurrData:
					this.Room_CurrDataCallBack(roomDataList);
					goto IL_130;
				case MobaTeamRoomCode.Room_ChangeRoomType:
					this.Room_ChangeRoomTypeCallBack(roomDataList);
					goto IL_130;
				case MobaTeamRoomCode.Room_ComeBack:
					this.Room_ComeBackCallBack(roomDataList);
					goto IL_130;
				case MobaTeamRoomCode.Room_ExchangeTeamType:
					this.Room_ChangeTeamTypeCallBack(roomDataList);
					goto IL_130;
				case MobaTeamRoomCode.Room_OwnerQuit:
					this.OwnerQuitCallBack(roomDataList);
					goto IL_130;
				case MobaTeamRoomCode.Room_InviteJoinRoom:
					this.Room_JoinCallBack(roomDataList);
					goto IL_130;
				}
				ClientLogger.Error("invalid room code:" + code);
				IL_130:;
			}
			else
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_Tips_RoomOperationWithoutSuccess"), 1f);
			}
		}

		private void Room_CreateCallBack(List<RoomData> roomDataList)
		{
			this.UndoCheckMsgState(MobaTeamRoomCode.Room_Create);
			if (Singleton<ArenaModeView>.Instance.IsOpen)
			{
				CtrlManager.CloseWindow(WindowID.ArenaModeView);
			}
			if (Singleton<InvitationView>.Instance.IsOpen)
			{
				CtrlManager.CloseWindow(WindowID.InvitationView);
			}
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("ServerResponse_WaitingServerResponse"), true, 15f);
			this._teamDataManagement.SetRoomDataList(roomDataList);
			this.RefreshUI();
			SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_GetFriendList, param, new object[0]);
		}

		private void Room_DestoryCallBack(List<RoomData> roomDataList)
		{
			this._teamDataManagement.Clear();
			Singleton<MenuTopBarView>.Instance.ClearTimeTips();
			CtrlManager.CloseWindow(WindowID.PvpRoomView);
			if (!Singleton<PvpSelectHeroView>.Instance.IsOpen)
			{
				CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
			}
		}

		private void Room_JoinCallBack(List<RoomData> roomDataList)
		{
			if (roomDataList == null)
			{
				return;
			}
			List<RoomData> list = new List<RoomData>();
			foreach (RoomData roomData in roomDataList)
			{
				if (this._teamDataManagement.RoomMemberList.Find((RoomData x) => x.UserId == roomData.UserId) == null)
				{
					list.Add(roomData);
				}
			}
			this._teamDataManagement.SetRoomDataList(roomDataList);
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("ServerResponse_WaitingServerResponse"), true, 15f);
			SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_GetFriendList, param, new object[0]);
			if (roomDataList != null)
			{
				this._battleId = roomDataList[0].MapId;
				this.UpdateRoomType(roomDataList[0].RoomType);
			}
			if (this.IsOpened)
			{
				this.RefreshUI();
			}
			else
			{
				if (Singleton<PropertyView>.Instance.IsOpen)
				{
					CtrlManager.ReturnPreWindow();
				}
				if (Singleton<SacrificialView>.Instance.IsOpen)
				{
					CtrlManager.ReturnPreWindow();
				}
				if (Singleton<RankView>.Instance.IsOpen)
				{
					CtrlManager.CloseWindow(WindowID.RankView);
				}
				if (Singleton<InvitationView>.Instance.IsOpen)
				{
					CtrlManager.CloseWindow(WindowID.InvitationView);
				}
				CtrlManager.OpenWindow(WindowID.PvpRoomView, null);
			}
			CtrlManager.CloseWindow(WindowID.ArenaModeView);
			if (Singleton<UIPvpEntranceCtrl>.Instance.IsOpen)
			{
				Singleton<UIPvpEntranceCtrl>.Instance.DelayCloseView();
			}
			if (list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.UpdateNoteDict(new PvpRoomNote
					{
						HeroName = list[i].NickName
					});
				}
			}
		}

		private void Room_CurrDataCallBack(List<RoomData> roomDataList)
		{
			this._teamDataManagement.SetRoomDataList(roomDataList);
			this._battleId = roomDataList[0].MapId;
			this.UpdateRoomType(roomDataList[0].RoomType);
			if (this.IsOpened)
			{
				this.RefreshUI();
			}
			else
			{
				if (Singleton<PropertyView>.Instance.IsOpen)
				{
					CtrlManager.ReturnPreWindow();
				}
				if (Singleton<SacrificialView>.Instance.IsOpen)
				{
					CtrlManager.ReturnPreWindow();
				}
				if (Singleton<RankView>.Instance.IsOpen)
				{
					CtrlManager.CloseWindow(WindowID.RankView);
				}
				if (Singleton<InvitationView>.Instance.IsOpen)
				{
					CtrlManager.CloseWindow(WindowID.InvitationView);
				}
				if (Singleton<ArenaModeView>.Instance.IsOpen)
				{
					CtrlManager.CloseWindow(WindowID.ArenaModeView);
				}
				if (Singleton<UIPvpEntranceCtrl>.Instance.IsOpen)
				{
					Singleton<UIPvpEntranceCtrl>.Instance.DelayCloseView();
				}
				CtrlManager.OpenWindow(WindowID.PvpRoomView, null);
				if (Singleton<MenuBottomBarView>.Instance.IsOpen)
				{
					CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
				}
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("ServerResponse_WaitingServerResponse"), true, 15f);
				SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_GetFriendList, param, new object[0]);
			}
		}

		private void Room_ChangeTeamTypeCallBack(List<RoomData> roomDataList)
		{
			if (this.IsOpened)
			{
				this._teamDataManagement.ReplaceRoomData(roomDataList);
				this.RefreshUI();
			}
		}

		private void Room_KickCallBack(List<RoomData> roomDataList)
		{
			string a = ModelManager.Instance.Get_userData_filed_X("UserId");
			this._teamDataManagement.RemoveRoomData(roomDataList);
			if (a == roomDataList[0].UserId)
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_Tips_YouAreKickedByOwner"), 1f);
				this._teamDataManagement.Clear();
				CtrlManager.CloseWindow(WindowID.PvpRoomView);
				CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
			}
			else
			{
				this.RefreshUI();
				this.UpdateNoteDict(new PvpRoomNote
				{
					HeroName = roomDataList[0].NickName,
					IsOut = true
				});
			}
		}

		private void CloseRoomView()
		{
			CtrlManager.OpenWindow(WindowID.MenuView, null);
			CtrlManager.OpenWindow(WindowID.MenuTopBarView, null);
			CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
			this.Room_DestoryCallBack(null);
		}

		private void Room_LeaveCallBack(List<RoomData> curRoomMembers)
		{
			if (curRoomMembers == null)
			{
				UnityEngine.Debug.LogError("curRoomMember == null");
				return;
			}
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("ServerResponse_WaitingServerResponse"), true, 15f);
			SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_GetFriendList, param, new object[0]);
			List<RoomData> list = new List<RoomData>();
			foreach (RoomData roomData in this._teamDataManagement.RoomMemberList)
			{
				if (curRoomMembers.Find((RoomData x) => x.UserId == roomData.UserId) == null)
				{
					list.Add(roomData);
				}
			}
			this._teamDataManagement.SetRoomDataList(curRoomMembers);
			if (list != null && list.Count > 0 && this.IsSelf(list[0].UserId))
			{
				this._teamDataManagement.Clear();
				CtrlManager.CloseWindow(WindowID.PvpRoomView);
				CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
				if (!Singleton<MenuTopBarView>.Instance.IsOpen)
				{
					CtrlManager.OpenWindow(WindowID.MenuTopBarView, null);
				}
			}
			if (this.IsOpened)
			{
				this.RefreshUI();
				if (list != null && list.Count > 0)
				{
					this.UpdateNoteDict(new PvpRoomNote
					{
						HeroName = list[0].NickName,
						IsOut = true
					});
				}
			}
		}

		public void ErrorToLeve()
		{
			this.LeaveRoom();
		}

		private void LeaveRoom()
		{
			this._teamDataManagement.Clear();
			CtrlManager.CloseWindow(WindowID.PvpRoomView);
			PvpMatchMgr.Instance.QuitMatch(false);
			if (!Singleton<ArenaModeView>.Instance.IsOpen)
			{
				CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
			}
		}

		private void Room_StartGameCallBack(List<RoomData> roomDataList)
		{
			RoomData ownerRoomData = this.GetOwnerRoomData();
			if (ownerRoomData != null)
			{
				PvpLevelStorage.JoinAsRoomMember(ownerRoomData.SummerId.ToString(), (from x in this._teamDataManagement.RoomMemberList
				select x.SummerId.ToString()).ToList<string>());
				Singleton<PvpManager>.Instance.ChooseTeamGame(int.Parse(this._battleId), this.RoomType == PvpRoomType.ZiDingYi);
			}
			else
			{
				ClientLogger.AssertNotNull(ownerRoomData, "GetOwnerRoomData return null");
			}
		}

		private void Room_ChangeRoomTypeCallBack(List<RoomData> roomDataList)
		{
			this.UpdateRoomType(roomDataList[0].RoomType);
			if (this.IsOpened)
			{
				this.ClearKHOrZDYData();
				this._teamDataManagement.ReplaceRoomData(roomDataList);
				this.RefreshUI();
			}
		}

		private void OwnerQuitCallBack(List<RoomData> roomDataList)
		{
			this._teamDataManagement.ReplaceRoomData(roomDataList);
			if (this.IsOpened)
			{
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("ServerResponse_WaitingServerResponse"), true, 15f);
				SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_GetFriendList, param, new object[0]);
				this.RefreshUI();
			}
			else
			{
				this._teamDataManagement.Clear();
			}
		}

		private void Room_ComeBackCallBack(List<RoomData> roomDataList)
		{
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("ServerResponse_WaitingServerResponse"), true, 15f);
			SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_GetFriendList, param, new object[0]);
			this.UpdateRoomType(roomDataList[0].RoomType);
			Singleton<PvpRoomView>.Instance.SetAsRoomOwner(false);
			CtrlManager.OpenWindow(WindowID.PvpRoomView, null);
			if (Singleton<UIPvpEntranceCtrl>.Instance.IsOpen)
			{
				Singleton<UIPvpEntranceCtrl>.Instance.DelayCloseView();
			}
			CtrlManager.CloseWindow(WindowID.ArenaModeView);
		}

		private void OnGetMsg_GetFriendList(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				this.GetFriendList(num, operationResponse.DebugMessage, null);
			}
			else
			{
				byte[] buffer = operationResponse.Parameters[27] as byte[];
				List<FriendData> friendDataList = SerializeHelper.Deserialize<List<FriendData>>(buffer);
				this.GetFriendList(num, operationResponse.DebugMessage, friendDataList);
			}
		}

		private void GetFriendList(int arg1, string arg2, List<FriendData> friendDataList)
		{
			if (!this.IsOpened)
			{
				return;
			}
			friendDataList = friendDataList.FindAll((FriendData item) => (int)item.Status == 1);
			this._friendList = Singleton<FriendView>.Instance.FixFriendDataListByOnline(friendDataList);
			if (this.IsRoomOwner())
			{
				if (this.listTask != null)
				{
					this.listTask.Stop();
				}
				this.listTask = this.cMgr.StartCoroutine(this.UpdateFriendGridView(this._friendList), true);
			}
			else
			{
				if (this.listTask != null)
				{
					this.listTask.Stop();
				}
				this.listTask = this.cMgr.StartCoroutine(this.UpdateFriendGridView(this._friendList), true);
				this.UpdateKHOrZDYView(this.RoomType);
			}
			this.cMgr.StartCoroutine(this.TryAutoInvite(), true);
		}

		public void SetLevelStorage(PvpTeamInfo teamInfo)
		{
			PvpLevelStorage.FightAgain();
			this._lastStorage = PvpLevelStorage.FetchLast();
			this._autoInvited = false;
		}

		private List<string> GetFriendTeamSumIDs(PvpTeamInfo pvpTeamInfo, string sumID)
		{
			if (pvpTeamInfo == null)
			{
				return null;
			}
			List<string> list = new List<string>();
			List<PvpTeamMember> list2 = new List<PvpTeamMember>();
			if (pvpTeamInfo.Teams[0].Find((PvpTeamMember obj) => obj.baseInfo.SummerId.ToString() == sumID) != null)
			{
				list2 = pvpTeamInfo.Teams[0];
			}
			else if (pvpTeamInfo.Teams[1].Find((PvpTeamMember obj) => obj.baseInfo.SummerId.ToString() == sumID) != null)
			{
				list2 = pvpTeamInfo.Teams[1];
			}
			else if (pvpTeamInfo.Teams[3].Find((PvpTeamMember obj) => obj.baseInfo.SummerId.ToString() == sumID) != null)
			{
				list2 = pvpTeamInfo.Teams[3];
			}
			if (list2 == null)
			{
				UnityEngine.Debug.LogError("SelfTeam is Null" + list2);
			}
			for (int i = 0; i < list2.Count; i++)
			{
				list.Add(list2[i].baseInfo.SummerId.ToString());
			}
			return list;
		}

		[DebuggerHidden]
		private IEnumerator TryAutoInvite()
		{
			PvpRoomView.<TryAutoInvite>c__Iterator18D <TryAutoInvite>c__Iterator18D = new PvpRoomView.<TryAutoInvite>c__Iterator18D();
			<TryAutoInvite>c__Iterator18D.<>f__this = this;
			return <TryAutoInvite>c__Iterator18D;
		}

		private void OnGetMsg_GetUserInfoBySummId(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				this.GetUserInfoBySummId(num, operationResponse.DebugMessage, null);
			}
			else
			{
				byte[] buffer = operationResponse.Parameters[86] as byte[];
				FriendData userInfo = SerializeHelper.Deserialize<FriendData>(buffer);
				this.GetUserInfoBySummId(num, operationResponse.DebugMessage, userInfo);
			}
		}

		private void GetUserInfoBySummId(int arg1, string arg2, FriendData userInfo)
		{
			if (!this.IsOpened)
			{
				return;
			}
			if (!this.FindPanel.gameObject.activeInHierarchy)
			{
				return;
			}
			if (userInfo == null)
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_NoThisSummonerID"), 1f);
				return;
			}
			UIEventListener.Get(this.F_AddBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickF_AddBtn);
			this.F_AddBtn.GetComponent<UISprite>().spriteName = "Add_friend_icons_friend_01";
			this.F_AddBtn.Find("Label").GetComponent<UILabel>().color = new Color32(0, 246, 229, 255);
			this.F_AddBtn.Find("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_AddFriends");
			this.F_AddBtn.gameObject.SetActive(true);
			this.F_Item.gameObject.SetActive(true);
			this.F_Name.text = userInfo.TargetName;
			this.F_LVLabel.text = CharacterDataMgr.instance.GetUserLevel((long)((int)userInfo.Exp)).ToString();
			SysSummonersPictureframeVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(userInfo.PictureFrame.ToString());
			this.F_Sprite.spriteName = dataById.pictureframe_icon;
			SysSummonersHeadportraitVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(userInfo.Icon.ToString());
			this.F_Texture.spriteName = dataById2.headportrait_icon;
			this.BottleLv.text = userInfo.bottlelevel.ToString();
			this.RankLv.text = LanguageManager.Instance.GetStringById(Singleton<FriendView>.Instance.GetState(userInfo.LadderScore));
			this.MeiliLv.text = userInfo.charm.ToString();
			this.F_Item.transform.Find("ID/IDLabel").GetComponent<UILabel>().text = this.F_Input.value;
		}

		private void OnGetMsg_ApplyAddFriend(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			switch (mobaErrorCode)
			{
			case MobaErrorCode.FriendExist:
				Singleton<TipView>.Instance.ShowViewSetText("你们已经是好友啦", 1f);
				return;
			case MobaErrorCode.FriendNotExist:
			case MobaErrorCode.FriendOffline:
			case MobaErrorCode.MyBlackCountLimit:
				IL_57:
				if (mobaErrorCode != MobaErrorCode.Ok)
				{
					this.ApplyAddFriend(num, operationResponse.DebugMessage, null);
					return;
				}
				this.ApplyAddFriend(num, operationResponse.DebugMessage, null);
				return;
			case MobaErrorCode.FriendInBlacklist:
				Singleton<TipView>.Instance.ShowViewSetText("对方在您黑名单中", 1f);
				return;
			case MobaErrorCode.MyFriendCountLimit:
				Singleton<TipView>.Instance.ShowViewSetText("您的好友已达上限", 1f);
				return;
			case MobaErrorCode.TargetFriendCountLimit:
				Singleton<TipView>.Instance.ShowViewSetText("对方的好友已达上限", 1f);
				return;
			case MobaErrorCode.InTargetBlacklist:
				Singleton<TipView>.Instance.ShowViewSetText("您在对方黑名单中", 1f);
				return;
			case MobaErrorCode.ApplyFriendAlready:
				Singleton<TipView>.Instance.ShowViewSetText("已发送好友请求", 1f);
				return;
			}
			goto IL_57;
		}

		private void ApplyAddFriend(int arg1, string arg2, FriendData arg3)
		{
			if (!this.IsOpened)
			{
				return;
			}
			List<FriendData> list = ModelManager.Instance.Get_blackList_X();
			if (arg1 == 20104)
			{
				if (list.Find((FriendData obj) => obj.TargetId == long.Parse(this.F_Input.value)) != null)
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_RemoveFromBlackList"), 1f);
				}
				else
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_SorryYouAreBacklisted"), 1f);
				}
			}
		}

		public void UpdateFriendState(string content)
		{
			if (!string.IsNullOrEmpty(content))
			{
				string[] stateStrs = content.Split(new char[]
				{
					'|'
				});
				if (stateStrs.Length >= 3)
				{
					KHFriendItem kHFriendItem = this.L_FriendGrid.transform.Find(stateStrs[1]).TryGetComp(null);
					if (kHFriendItem != null)
					{
						kHFriendItem.SetGameState(this.GetFriendNetType((sbyte)int.Parse(stateStrs[2])));
						if (this._friendList.Find((FriendData _obj) => _obj.TargetId.ToString() == stateStrs[1]) != null)
						{
							this._friendList.Find((FriendData _obj) => _obj.TargetId.ToString() == stateStrs[1]).GameStatus = (sbyte)int.Parse(stateStrs[2]);
							if (this.listTask != null)
							{
								this.listTask.Stop();
							}
							this.listTask = this.cMgr.StartCoroutine(this.UpdateFriendGridView(this._friendList), true);
							return;
						}
					}
				}
			}
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(false, LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_RefreshData"), true, 15f);
			SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_GetFriendList, param, new object[0]);
		}

		private void UpdateRoomType(int roomType)
		{
			if (roomType == 3)
			{
				this.RoomType = PvpRoomType.KaiHei;
			}
			else
			{
				this.RoomType = PvpRoomType.ZiDingYi;
			}
			PvpMatchMgr.Instance.SetRoomType(new PvpRoomType?(this.RoomType));
		}
	}
}
