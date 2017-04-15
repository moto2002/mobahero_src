using Assets.Scripts.Model;
using Assets.Scripts.Server;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using Common;
using ExitGames.Client.Photon;
using GameLogin.State;
using MobaHeros.Pvp.State;
using MobaProtocol;
using MobaProtocol.Data;
using Newbie;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MobaHeros.Pvp
{
	public class PvpMatchMgr : GlobalComServerBase
	{
		private class RefreshEscapeCdTask : PopViewTask
		{
			private float _cd;

			private string _fmt;

			private string _name;

			public RefreshEscapeCdTask(float cd, string fmt, string name)
			{
				this._cd = cd;
				this._fmt = fmt;
				this._name = name;
			}

			[DebuggerHidden]
			public override IEnumerator Run(PopViewParam param, NewPopView view)
			{
				PvpMatchMgr.RefreshEscapeCdTask.<Run>c__Iterator4F <Run>c__Iterator4F = new PvpMatchMgr.RefreshEscapeCdTask.<Run>c__Iterator4F();
				<Run>c__Iterator4F.view = view;
				<Run>c__Iterator4F.<$>view = view;
				<Run>c__Iterator4F.<>f__this = this;
				return <Run>c__Iterator4F;
			}
		}

		public const int WaitSeconds = 30;

		private Task _timerTask;

		private PvpRoomType? _roomType;

		private bool _isWaitEnterSelectHero;

		private bool _isCachedSwitchHeroInfo;

		private int _cachedSwtichHeroNewUid;

		private RoomInfo RoomInfo
		{
			get
			{
				return Singleton<PvpManager>.Instance.RoomInfo;
			}
		}

		public static PvpMatchState State
		{
			get;
			private set;
		}

		public static PvpMatchMgr Instance
		{
			get;
			private set;
		}

		public DateTime? RoomReadyTime
		{
			get;
			private set;
		}

		public DateTime? JoinQueueTime
		{
			get;
			private set;
		}

		public DateTime? SelectHeroTime
		{
			get;
			set;
		}

		public float WaitTimeLeft
		{
			get
			{
				DateTime? roomReadyTime = this.RoomReadyTime;
				if (roomReadyTime.HasValue)
				{
					double num = 30.0 - (DateTime.Now - roomReadyTime.Value).TotalSeconds;
					if (num < 0.0)
					{
						num = 0.0;
					}
					return (float)num;
				}
				return 0f;
			}
		}

		public PvpMatchMgr()
		{
			this.Init();
			PvpMatchMgr.Instance = this;
		}

		public override void OnAwake()
		{
			MobaMessageManager.RegistMessage(LobbyCode.C2L_ReadySelectHero, new MobaMessageFunc(this.L2C_ReadySelectHero));
			MobaMessageManager.RegistMessage(LobbyCode.L2C_RoomReady, new MobaMessageFunc(this.L2C_RoomReady));
			MobaMessageManager.RegistMessage(LobbyCode.C2L_JoinQueue, new MobaMessageFunc(this.L2C_JoinQueue));
			MobaMessageManager.RegistMessage(LobbyCode.L2C_ReadyCheckAllOK, new MobaMessageFunc(this.L2C_ReadyCheckAllOK));
			MobaMessageManager.RegistMessage(LobbyCode.C2L_OutQueue, new MobaMessageFunc(this.L2C_OutQueue));
			MobaMessageManager.RegistMessage(LobbyCode.C2L_ReadyCheckOK, new MobaMessageFunc(this.L2C_ReadyCheckOK));
			MobaMessageManager.RegistMessage(LobbyCode.C2L_OutReady, new MobaMessageFunc(this.L2C_OutReady));
			MobaMessageManager.RegistMessage(LobbyCode.L2C_ServerNotify, new MobaMessageFunc(this.L2C_ServerNotify));
			MobaMessageManager.RegistMessage(LobbyCode.C2L_RoomSelectRandomHero, new MobaMessageFunc(this.MatchStatusRoomSelectRandomHero));
			MobaMessageManager.RegistMessage(LobbyCode.C2L_RoomReqChangeHero, new MobaMessageFunc(this.MatchStatusRoomReqChangeHero));
			MobaMessageManager.RegistMessage(LobbyCode.C2L_RoomCancelReqChangeHero, new MobaMessageFunc(this.MatchStatusRoomCancelReqChangeHero));
			MobaMessageManager.RegistMessage(LobbyCode.L2C_CanExchangeHeroList, new MobaMessageFunc(this.MatchGetSwitchHeroInfo));
			MobaMessageManager.RegistMessage(LobbyCode.L2C_LobbyShutdown, new MobaMessageFunc(this.L2C_LobbyShutdown));
			MobaMessageManager.RegistMessage((ClientMsg)25024, new MobaMessageFunc(this.OnSessionEnd));
			MobaMessageManager.RegistMessage(PvpCode.L2G_PunishMatch, new MobaMessageFunc(this.G2C_PunishMatch));
		}

		public override void OnDestroy()
		{
			MobaMessageManager.UnRegistMessage(LobbyCode.C2L_ReadySelectHero, new MobaMessageFunc(this.L2C_ReadySelectHero));
			MobaMessageManager.UnRegistMessage(LobbyCode.L2C_RoomReady, new MobaMessageFunc(this.L2C_RoomReady));
			MobaMessageManager.UnRegistMessage(LobbyCode.C2L_JoinQueue, new MobaMessageFunc(this.L2C_JoinQueue));
			MobaMessageManager.UnRegistMessage(LobbyCode.L2C_ReadyCheckAllOK, new MobaMessageFunc(this.L2C_ReadyCheckAllOK));
			MobaMessageManager.UnRegistMessage(LobbyCode.C2L_OutQueue, new MobaMessageFunc(this.L2C_OutQueue));
			MobaMessageManager.UnRegistMessage(LobbyCode.C2L_ReadyCheckOK, new MobaMessageFunc(this.L2C_ReadyCheckOK));
			MobaMessageManager.UnRegistMessage(LobbyCode.C2L_OutReady, new MobaMessageFunc(this.L2C_OutReady));
			MobaMessageManager.UnRegistMessage(LobbyCode.L2C_ServerNotify, new MobaMessageFunc(this.L2C_ServerNotify));
			MobaMessageManager.UnRegistMessage(LobbyCode.C2L_RoomSelectRandomHero, new MobaMessageFunc(this.MatchStatusRoomSelectRandomHero));
			MobaMessageManager.UnRegistMessage(LobbyCode.C2L_RoomReqChangeHero, new MobaMessageFunc(this.MatchStatusRoomReqChangeHero));
			MobaMessageManager.UnRegistMessage(LobbyCode.C2L_RoomCancelReqChangeHero, new MobaMessageFunc(this.MatchStatusRoomCancelReqChangeHero));
			MobaMessageManager.UnRegistMessage(LobbyCode.L2C_CanExchangeHeroList, new MobaMessageFunc(this.MatchGetSwitchHeroInfo));
			MobaMessageManager.UnRegistMessage(LobbyCode.L2C_LobbyShutdown, new MobaMessageFunc(this.L2C_LobbyShutdown));
			MobaMessageManager.UnRegistMessage((ClientMsg)25024, new MobaMessageFunc(this.OnSessionEnd));
			MobaMessageManager.UnRegistMessage(PvpCode.L2G_PunishMatch, new MobaMessageFunc(this.G2C_PunishMatch));
		}

		public override void OnRestart()
		{
			this.QuitMatch(true);
		}

		public void ForceSelectHero()
		{
			CtrlManager.DestroyAllWindowsExcept(new WindowID[]
			{
				WindowID.MenuView,
				WindowID.MainBg,
				WindowID.MenuTopBarView,
				WindowID.MenuBottomBarView,
				WindowID.ArenaModeView,
				WindowID.PvpRoomView,
				WindowID.NewPopView
			});
			CtrlManager.ReturnPreWindow();
			Singleton<MenuTopBarView>.Instance.ClearTimeTips();
			Task.Clear(ref this._timerTask);
			CtrlManager.OpenWindow(WindowID.PvpSelectHeroView, null);
			if (this.TryClearWaitSelectHeroFlag())
			{
				this.OnWaitSelectHeroFlagCleared();
			}
		}

		public void LeaveQueue()
		{
			if (PvpMatchMgr.State == PvpMatchState.Matched)
			{
				CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("PlayUI_Title_ForbiddanceCancel"), LanguageManager.Instance.GetStringById("PlayUI_Content_ForbiddanceCancel"), delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
				return;
			}
			Singleton<PvpManager>.Instance.LeavePvpQueue();
		}

		private void L2C_JoinQueue(MobaMessage msg)
		{
			PvpLevelStorage.ClearLast();
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[0];
			byte b = (byte)operationResponse.Parameters[1];
			byte b2 = (byte)operationResponse.Parameters[2];
			if (NewbieManager.Instance.IsHandleNewbieServerMsg())
			{
				NewbieManager.Instance.HandleJoinQueue(num, b, b2);
				return;
			}
			PvpErrorCode pvpErrorCode = (PvpErrorCode)b2;
			if (pvpErrorCode == PvpErrorCode.OK)
			{
				PvpMatchMgr.State = PvpMatchState.Matching;
				Singleton<PvpManager>.Instance.SetBattleInfo(num, (PvpJoinType)b);
				this.JoinQueueTime = new DateTime?(DateTime.Now);
				PvpStateManager.Instance.ChangeState(new PvpStateInQueue());
			}
			else if (pvpErrorCode == PvpErrorCode.IsForbidden)
			{
				string text = LanguageManager.Instance.GetStringById("Tips_Hung_Punish_Content1", "惩罚时间中");
				text = text.Replace("*", ModelManager.Instance.Get_PunishmentEndTime().ToString("yyyy.MM.dd HH:mm"));
				CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("Tips_Hung_Punish1"), text, delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
			}
			else if (pvpErrorCode != PvpErrorCode.InPunish)
			{
				string pvpErrorString = PvpManager.GetPvpErrorString(pvpErrorCode);
				CtrlManager.ShowMsgBox("排队失败", pvpErrorString, null, PopViewType.PopOneButton, "确定", "取消", null);
				ClientLogger.Warn("L2C_JoinQueue failed: " + b2);
			}
		}

		private void L2C_ReadyCheckOK(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int userId = (int)operationResponse.Parameters[0];
			if (!(bool)operationResponse.Parameters[1])
			{
				this.RoomInfo.IsOtherCancelConfirm = true;
				foreach (ReadyPlayerSampleInfo current in this.RoomInfo.PvpPlayers)
				{
					current.readyChecked = false;
				}
			}
			else if (!this.RoomInfo.IsOtherCancelConfirm)
			{
				ReadyPlayerSampleInfo readyPlayerSampleInfo = this.RoomInfo.PvpPlayers.FirstOrDefault((ReadyPlayerSampleInfo x) => x.newUid == userId);
				if (readyPlayerSampleInfo != null)
				{
					readyPlayerSampleInfo.readyChecked = true;
				}
				else
				{
					ClientLogger.Error("P2C_ReadyCheckOK: cannot found user #" + userId);
				}
			}
			MobaMessageManager.DispatchMsg(ClientC2C.PvpNewCheckOk, null);
		}

		private void L2C_OutQueue(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			byte b = (byte)operationResponse.Parameters[0];
			PvpLevelStorage.ClearLast();
			if (b == 0)
			{
				ArenaModeView.ShowMatchingState(false);
				this.QuitMatch(false);
				PvpStateManager.Instance.ChangeState(new PvpStateHome());
			}
			else
			{
				ClientLogger.Warn("server side state : " + (PlayerState)b);
			}
		}

		private void L2C_OutReady(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			string text = (string)operationResponse.Parameters[0];
			if (text == null)
			{
				return;
			}
			string[] array = text.Split(new char[]
			{
				','
			});
			List<int> list = new List<int>();
			for (int i = 0; i < array.Length; i++)
			{
				int item;
				if (!int.TryParse(array[i], out item))
				{
					return;
				}
				list.Add(item);
			}
			byte b = (byte)operationResponse.Parameters[1];
			if (Singleton<PvpManager>.Instance.RoomInfo.IsRoomReady)
			{
				bool flag = LevelManager.m_CurLevel.IsRank();
				string title = "提示";
				string content = string.Empty;
				if (b == 1 && !list.Contains(Singleton<PvpManager>.Instance.MyLobbyUserId))
				{
					string text2 = Singleton<PvpManager>.Instance.RoomInfo.GetPlayerInfoByUserId(list[0]).userName;
					for (int j = 1; j < list.Count; j++)
					{
						text2 = text2 + "、" + Singleton<PvpManager>.Instance.RoomInfo.GetPlayerInfoByUserId(list[j]).userName;
					}
					content = ((!flag) ? LanguageManager.Instance.GetStringById("Match_Canceled_Prompt").Replace("*", text2) : LanguageManager.Instance.GetStringById("RankMatch_Canceled_Prompt").Replace("*", text2));
				}
				else if (b == 1 && list.Contains(Singleton<PvpManager>.Instance.MyLobbyUserId))
				{
					content = ((!flag) ? LanguageManager.Instance.GetStringById("Match_Canceled_MyPrompt") : LanguageManager.Instance.GetStringById("RankMatch_Canceled_MyPrompt"));
					if (flag)
					{
						double ladderScore = ModelManager.Instance.Get_userData_X().LadderScore;
						ModelManager.Instance.Get_userData_X().LadderScore = ((ladderScore - 50.0 >= 0.0) ? (ladderScore - 50.0) : 0.0);
						Singleton<MenuView>.Instance.UpdateRankInfo();
					}
				}
				CtrlManager.ShowMsgBox(title, content, delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
			}
			this.QuitMatch(false);
			PvpManager.ResetState();
			PvpStateManager.Instance.ChangeState(new PvpStateHome());
			Singleton<MenuBottomBarView>.Instance.RefreshPlayBtn();
			NewbieManager.Instance.TryHandleOpenHomeBottomView();
		}

		private void L2C_ReadyCheckAllOK(MobaMessage msg)
		{
			if (NewbieManager.Instance.IsHandleNewbieServerMsg())
			{
				NewbieManager.Instance.HandleReadyCheckAllOk();
				return;
			}
			this.SelectHeroTime = new DateTime?(DateTime.Now);
			this.OnSelectHero();
			PvpStateManager.Instance.ChangeState(new PvpStateSelectHero());
		}

		private void L2C_RoomReady(MobaMessage msg)
		{
			if (NewbieManager.Instance.IsHandleNewbieServerMsg())
			{
				NewbieManager.Instance.HandleRoomReady();
				return;
			}
			Task.Clear(ref this._timerTask);
			this._timerTask = new Task(this.Wait_Coroutine(), true);
			this.ProcessReadyMsg(msg);
			Singleton<PvpManager>.Instance.ConfirmReady(true);
		}

		private void G2C_PunishMatch(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			long value = (long)operationResponse.Parameters[0];
			CtrlManager.ShowMsgBox("提示", string.Format("由于您上次匹配未及时选择英雄，{0}分钟无法进行匹配", (int)TimeSpan.FromTicks(value).TotalMinutes), delegate
			{
			}, PopViewType.PopOneButton, "确定", "取消", null);
		}

		private void OnSelectHero()
		{
			if (!this._roomType.HasValue || this._roomType.Value != PvpRoomType.ZiDingYi)
			{
				this.SetWaitSelectHeroFlag();
				CtrlManager.OpenWindow(WindowID.PvpWaitView, null);
			}
			else
			{
				this.ForceSelectHero();
			}
		}

		private void L2C_ReadySelectHero(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[0];
			ReadyPlayerSampleInfo playerInfoByUserId = Singleton<PvpManager>.Instance.RoomInfo.GetPlayerInfoByUserId(num);
			if (playerInfoByUserId == null)
			{
				ClientLogger.Error("L2C_ReadySelectHero: cannot found user #" + num);
				return;
			}
			playerInfoByUserId.IsReadySelectHero = true;
			MobaMessageManager.DispatchMsg((ClientMsg)23065, num, 0f);
		}

		private void L2C_ServerNotify(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			ServerNotifyCode serverNotifyCode = (ServerNotifyCode)((int)operationResponse.Parameters[0]);
			ServerNotifyCode serverNotifyCode2 = serverNotifyCode;
			if (serverNotifyCode2 == ServerNotifyCode.PvpQueueError_EscapeCD)
			{
				string name = (string)operationResponse.Parameters[1];
				float cd = (float)operationResponse.Parameters[2];
				this.PunishEscape(name, cd);
			}
		}

		public void PunishEscape(string name, float cd)
		{
			string id = "PlayUI_Content_EscapeCD_Self";
			if (!string.IsNullOrEmpty(name))
			{
				id = "PlayUI_Content_EscapeCD_Other";
			}
			Singleton<PvpRoomView>.Instance.StopWaitingQueue();
			PvpMatchMgr.RefreshEscapeCdTask task = new PvpMatchMgr.RefreshEscapeCdTask(cd, LanguageManager.Instance.GetStringById(id), name ?? string.Empty);
			CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("PlayUI_Title_EscapeCD"), LanguageManager.Instance.FormatString(id, new object[]
			{
				(int)cd / 60,
				(int)cd % 60,
				name
			}), delegate
			{
			}, PopViewType.PopOneButton, "确定", "取消", task);
		}

		public void L2C_LobbyShutdown(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			string str = (string)operationResponse.Parameters[0];
			PvpLevelStorage.ClearLast();
			this.QuitMatch(true);
			PvpStateManager.Instance.ChangeState(new PvpStateHome());
			CtrlManager.ShowMsgBox("服务器信息", str + ",请稍后再试", null, PopViewType.PopOneButton, "确定", "取消", null);
		}

		private void MatchStatusRoomSelectRandomHero(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int newUid = (int)operationResponse.Parameters[0];
			byte[] buffer = operationResponse.Parameters[1] as byte[];
			HeroInfo heroInfo = SerializeHelper.Deserialize<HeroInfo>(buffer);
			ReadyPlayerSampleInfo readyPlayerSampleInfo = Singleton<PvpManager>.Instance.RoomInfo.PvpPlayers.FirstOrDefault((ReadyPlayerSampleInfo x) => x.newUid == newUid);
			if (readyPlayerSampleInfo != null)
			{
				readyPlayerSampleInfo.heroInfo = heroInfo;
				readyPlayerSampleInfo.horeSelected = true;
				MobaMessageManager.DispatchMsg(ClientC2C.PvpSelectHero, new ParamSelectHero(newUid, heroInfo));
				this.TrySetUserNameForRobot(readyPlayerSampleInfo);
				MobaMessageManager.DispatchMsg(ClientC2C.PvpSelectHeroOk, new ParamSelectHero(newUid, heroInfo));
			}
			else
			{
				ClientLogger.Error("MatchStatusRoomSelectRandomHero cannot found player: " + newUid);
			}
		}

		private void MatchStatusRoomReqChangeHero(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[0];
			if (this._isWaitEnterSelectHero)
			{
				this._isCachedSwitchHeroInfo = true;
				this._cachedSwtichHeroNewUid = num;
			}
			MobaMessageManager.DispatchMsg((ClientMsg)23068, new ParamShowSwitchHeroInfo(EShowSwitchHeroInfoType.RespType, num), 0f);
		}

		private void MatchStatusRoomCancelReqChangeHero(MobaMessage msg)
		{
			if (this._isWaitEnterSelectHero)
			{
				this._isCachedSwitchHeroInfo = false;
				this._cachedSwtichHeroNewUid = 0;
			}
			MobaMessageManager.DispatchMsg((ClientMsg)23070, null, 0f);
		}

		private void TrySetUserNameForRobot(ReadyPlayerSampleInfo inInfo)
		{
			if (inInfo == null)
			{
				return;
			}
			if (!this.IsRobotInGameFightWithRobot(inInfo))
			{
				return;
			}
			if (inInfo.heroInfo == null)
			{
				return;
			}
			string heroId = inInfo.heroInfo.heroId;
			if (!StringUtils.CheckValid(heroId))
			{
				return;
			}
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(heroId);
			if (heroMainData == null)
			{
				return;
			}
			string name = heroMainData.name;
			if (!StringUtils.CheckValid(name))
			{
				return;
			}
			SysLanguageVo languageData = BaseDataMgr.instance.GetLanguageData(name);
			if (languageData == null)
			{
				return;
			}
			inInfo.userName = languageData.content + "(电脑)";
		}

		private bool IsRobotInGameFightWithRobot(ReadyPlayerSampleInfo inInfo)
		{
			return inInfo != null && string.IsNullOrEmpty(inInfo.userName);
		}

		public void SetWaitSelectHeroFlag()
		{
			this._isWaitEnterSelectHero = true;
		}

		public bool TryClearWaitSelectHeroFlag()
		{
			if (this._isWaitEnterSelectHero)
			{
				this._isWaitEnterSelectHero = false;
				return true;
			}
			return false;
		}

		public void OnWaitSelectHeroFlagCleared()
		{
			if (this._isCachedSwitchHeroInfo)
			{
				MobaMessageManager.DispatchMsg((ClientMsg)23068, new ParamShowSwitchHeroInfo(EShowSwitchHeroInfoType.RespType, this._cachedSwtichHeroNewUid), 0f);
				this._isCachedSwitchHeroInfo = false;
				this._cachedSwtichHeroNewUid = 0;
			}
		}

		private void MatchGetSwitchHeroInfo(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			byte[] buffer = operationResponse.Parameters[0] as byte[];
			List<int> list = SerializeHelper.Deserialize<List<int>>(buffer);
			int num = (int)operationResponse.Parameters[1];
			Singleton<PvpSelectHeroView>.Instance.InitRandomSelHeroInfo(list, num);
			MobaMessageManager.DispatchMsg((ClientMsg)23067, new ParamGetCanSwitchHeroInfo(list, num), 0f);
		}

		private void ProcessReadyMsg(MobaMessage msg)
		{
			try
			{
				this.RoomReadyTime = new DateTime?(DateTime.Now);
				PvpMatchMgr.State = PvpMatchState.Matched;
				OperationResponse operationResponse = msg.Param as OperationResponse;
				int roomId = (int)operationResponse.Parameters[0];
				int myNewId = (int)operationResponse.Parameters[1];
				byte[] buffer = operationResponse.Parameters[2] as byte[];
				List<ReadyPlayerSampleInfo> list = SerializeHelper.Deserialize<List<ReadyPlayerSampleInfo>>(buffer);
				Singleton<PvpManager>.Instance.SetRoomInfo(roomId, myNewId, list.ToArray(), null);
				MobaMessageManager.DispatchMsg(ClientC2C.PvpRoomReady, null);
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
			}
		}

		private void OnSessionEnd(MobaMessage msg)
		{
			CtrlManager.CloseWindow(WindowID.PvpWaitView);
		}

		private void Init()
		{
			PvpMatchMgr.State = PvpMatchState.None;
			this.JoinQueueTime = null;
			this.RoomReadyTime = null;
			this.SelectHeroTime = null;
			this._roomType = null;
			Task.Clear(ref this._timerTask);
		}

		public void ClearTask()
		{
			Task.Clear(ref this._timerTask);
		}

		public void QuitMatch(bool teamClear = false)
		{
			this.Init();
			if (Singleton<PvpManager>.Instance.JoinType == PvpJoinType.Single)
			{
				Singleton<PvpManager>.Instance.ClearBattleInfo();
			}
		}

		[DebuggerHidden]
		private IEnumerator Wait_Coroutine()
		{
			PvpMatchMgr.<Wait_Coroutine>c__Iterator4E <Wait_Coroutine>c__Iterator4E = new PvpMatchMgr.<Wait_Coroutine>c__Iterator4E();
			<Wait_Coroutine>c__Iterator4E.<>f__this = this;
			return <Wait_Coroutine>c__Iterator4E;
		}

		public void SetRoomType(PvpRoomType? roomType)
		{
			this._roomType = roomType;
		}
	}
}
