using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using GameLogin.State;
using MobaHeros.Server;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace MobaHeros.Pvp.State
{
	public abstract class PvpStateRecoverBase : PvpStateBase
	{
		protected enum ActiveLink
		{
			GateServer,
			PvpServer
		}

		protected PlayerState TargetState;

		protected PvpStateRecoverBase.ActiveLink CurrentLink
		{
			get;
			private set;
		}

		protected PvpStateRecoverBase(PvpStateCode stateCode, PvpStateRecoverBase.ActiveLink activeLink) : base(stateCode)
		{
			this.CurrentLink = activeLink;
		}

		protected void QueryGsPvpState()
		{
			PvpStateBase.LogState("QueryGsPvpState: send C2L_QueryPvpState");
			Singleton<PvpManager>.Instance.SendLobbyMsg(LobbyCode.C2L_QueryPvpState, new object[0]);
		}

		protected void QueryPsPvpState()
		{
			PvpStateBase.LogState("QueryPsPvpState: send C2P_QueryInFightInfo");
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_QueryInFightInfo, null);
		}

		protected void SendUserBackToGs()
		{
			PvpStateBase.LogState("SendUserBackToGs: send C2L_UserBack");
			Singleton<PvpManager>.Instance.SendLobbyMsgEx(LobbyCode.C2L_UserBack, null, new object[0]);
		}

		protected void SayHelloToPs(PvpCode code)
		{
			PvpStateBase.LogState("SayHelloToPs: send " + code);
			PvpStartGameInfo loginInfo = Singleton<PvpManager>.Instance.LoginInfo;
			object probufObj = null;
			switch (code)
			{
			case PvpCode.C2P_Reconnect:
				probufObj = new C2PReconnect
				{
					roomId = loginInfo.roomId,
					newUid = loginInfo.newUid,
					newkey = loginInfo.newKey
				};
				goto IL_F4;
			case PvpCode.P2C_Reconnect:
				IL_39:
				if (code != PvpCode.C2P_LoginAsViewer)
				{
					goto IL_F4;
				}
				probufObj = new C2PLoginAsViewer
				{
					roomId = loginInfo.roomId,
					newUid = int.Parse(ModelManager.Instance.Get_userData_X().UserId),
					newkey = loginInfo.newKey
				};
				goto IL_F4;
			case PvpCode.C2P_BackGame:
				probufObj = new C2PBackGame
				{
					roomId = loginInfo.roomId,
					newUid = loginInfo.newUid,
					newkey = loginInfo.newKey
				};
				goto IL_F4;
			}
			goto IL_39;
			IL_F4:
			SendMsgManager.Instance.SendPvpLoginMsgBase<object>(code, probufObj, loginInfo.roomId);
		}

		protected void RecoverFinish(PvpStateBase state)
		{
			PvpStateBase.LogState("RecoverFinish to " + state.StateCode);
			PvpStateManager.Instance.ChangeState(state);
		}

		protected abstract void OnL2CQueryPvpState(PlayerState playerState);

		protected virtual void OnP2CBackGame(PvpErrorCode code)
		{
		}

		private void RegMsgs(PvpStateBase.MsgRegFn func, PvpStateBase.MsgRegFnLobby funcLobby)
		{
			funcLobby(LobbyCode.C2L_QueryPvpState, new MobaMessageFunc(this.L2C_QueryPvpState));
			funcLobby(LobbyCode.C2L_UserBack, new MobaMessageFunc(this.L2C_UserBack));
			func(PvpCode.P2C_BackGame, new MobaMessageFunc(this.P2C_BackGame));
			func(PvpCode.P2C_Reconnect, new MobaMessageFunc(this.P2C_Reconnect));
			func(PvpCode.C2P_LoginAsViewer, new MobaMessageFunc(this.P2C_LoginAsViewer));
			func(PvpCode.P2C_BackLoadingInfo, new MobaMessageFunc(this.P2C_BackLoadingInfo));
			func(PvpCode.P2C_RefreshInFightInfo, new MobaMessageFunc(this.P2C_RefreshInFightInfo));
			func(PvpCode.C2P_QueryInFightInfo, new MobaMessageFunc(this.P2C_QueryInFightInfo));
		}

		protected sealed override void RegistCallbacks()
		{
			this.RegMsgs(new PvpStateBase.MsgRegFn(MobaMessageManager.RegistMessage), new PvpStateBase.MsgRegFnLobby(MobaMessageManager.RegistMessage));
		}

		protected sealed override void UnregistCallbacks()
		{
			this.RegMsgs(new PvpStateBase.MsgRegFn(MobaMessageManager.UnRegistMessage), new PvpStateBase.MsgRegFnLobby(MobaMessageManager.UnRegistMessage));
		}

		private void L2C_QueryPvpState(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			byte b = (byte)operationResponse.Parameters[0];
			PlayerState playerState = (PlayerState)b;
			PvpStateBase.LogState(string.Concat(new object[]
			{
				"receive L2C_QueryPvpState ",
				playerState,
				", ",
				Time.frameCount
			}));
			this.OnL2CQueryPvpState(playerState);
		}

		private void RecoverFromLoadingState(BattleRoomInfo roomInfo, PvpStartGameInfo loginInfo, ReadyPlayerSampleInfo[] playerInfos, bool isFighting)
		{
			PvpStateBase.LogState("RecoverFromLoadingState: " + StringUtils.DumpObject(roomInfo) + " " + StringUtils.DumpObject(loginInfo));
			Singleton<PvpManager>.Instance.SetBattleInfoWithoutJoinType(roomInfo.battleId);
			Singleton<PvpManager>.Instance.SetRoomInfo(roomInfo, loginInfo.newUid, playerInfos, null);
			Singleton<PvpManager>.Instance.LoginInfo = loginInfo;
			if (isFighting)
			{
				Singleton<PvpManager>.Instance.RoomInfo.UpdateAllLoadOk();
			}
			NetWorkHelper.Instance.DisconnectFromGateServer(false);
			NetWorkHelper.Instance.ConnectToPvpServer();
			this.CurrentLink = PvpStateRecoverBase.ActiveLink.PvpServer;
			if (!string.IsNullOrEmpty(roomInfo.roomVoiceID))
			{
			}
		}

		private void RecoverViewsForSelectHero()
		{
			CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
			Singleton<ArenaModeView>.Instance.ShowSelectHeroView();
		}

		private void L2C_UserBack(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			byte b = (byte)operationResponse.Parameters[0];
			byte[] buffer = operationResponse.Parameters[1] as byte[];
			PlayerState playerState = (PlayerState)b;
			PvpStateBase.LogState("receive L2C_UserBack " + playerState + " ");
			Singleton<PvpManager>.Instance.LoginInfo = null;
			Singleton<PvpManager>.Instance.ServerBattleRoomInfo = null;
			this.TargetState = PlayerState.Free;
			switch (playerState)
			{
			case PlayerState.Free:
				this.RecoverFinish(new PvpStateHome());
				return;
			case PlayerState.CheckReady:
				ClientLogger.Error("Cannot be here anymore.");
				this.RecoverFinish(new PvpStateHome());
				return;
			case PlayerState.SelectHore:
			{
				CtrlManager.CloseWindow(WindowID.PvpWaitView);
				InSelectAllInfo inSelectAllInfo = SerializeHelper.Deserialize<InSelectAllInfo>(buffer);
				Singleton<PvpManager>.Instance.SetBattleInfoWithoutJoinType(inSelectAllInfo.roomInfo.battleId);
				Singleton<PvpManager>.Instance.SetRoomInfo(inSelectAllInfo.roomInfo, inSelectAllInfo.newUid, inSelectAllInfo.playerInfos, null);
				PvpMatchMgr.Instance.SelectHeroTime = new DateTime?(DateTime.Now - TimeSpan.FromTicks(inSelectAllInfo.useTime));
				this.RecoverViewsForSelectHero();
				this.RecoverFinish(new PvpStateSelectHero());
				return;
			}
			case PlayerState.Loading:
			{
				InBattleLobbyInfo inBattleLobbyInfo = SerializeHelper.Deserialize<InBattleLobbyInfo>(buffer);
				InBattleRoomInfo inBattleRoomInfo = SerializeHelper.Deserialize<InBattleRoomInfo>(inBattleLobbyInfo.InBattleRoomInfoBytes);
				this.RecoverFromLoadingState(inBattleRoomInfo.roomInfo, inBattleLobbyInfo.loginInfo, inBattleRoomInfo.playerInfos, false);
				this.TargetState = PlayerState.Loading;
				return;
			}
			case PlayerState.InFight:
			{
				InBattleLobbyInfo inBattleLobbyInfo2 = SerializeHelper.Deserialize<InBattleLobbyInfo>(buffer);
				InBattleRoomInfo inBattleRoomInfo2 = SerializeHelper.Deserialize<InBattleRoomInfo>(inBattleLobbyInfo2.InBattleRoomInfoBytes);
				this.RecoverFromLoadingState(inBattleRoomInfo2.roomInfo, inBattleLobbyInfo2.loginInfo, inBattleRoomInfo2.playerInfos, true);
				this.TargetState = PlayerState.InFight;
				return;
			}
			}
			ClientLogger.Warn("L2C_UserBack: unknown state " + playerState);
		}

		private void P2C_BackGame(MobaMessage msg)
		{
			PvpServer.LockScreen(false);
			RetaMsg probufMsg = msg.GetProbufMsg<RetaMsg>();
			byte retaCode = probufMsg.retaCode;
			PvpErrorCode pvpErrorCode = (PvpErrorCode)retaCode;
			PvpStateBase.LogState("receive P2C_BackGame " + pvpErrorCode);
			GameManager.Instance.ReConnected();
			Singleton<PvpManager>.Instance.LoadPvpSceneBegin();
			this.OnP2CBackGame(pvpErrorCode);
			if (pvpErrorCode != PvpErrorCode.OK)
			{
				Singleton<PvpManager>.Instance.AbandonGame(pvpErrorCode);
			}
		}

		private void P2C_Reconnect(MobaMessage msg)
		{
			PvpServer.LockScreen(false);
			RetaMsg probufMsg = msg.GetProbufMsg<RetaMsg>();
			byte retaCode = probufMsg.retaCode;
			PvpErrorCode pvpErrorCode = (PvpErrorCode)retaCode;
			PvpStateBase.LogState("receive P2C_Reconnect " + pvpErrorCode);
			if (pvpErrorCode != PvpErrorCode.OK)
			{
				NetWorkHelper.Instance.DisconnectPvpServer();
				PvpStateBase.LogState("P2C_Reconnect failed " + pvpErrorCode);
				this.ConfirmQuiting("游戏已经结束，戳确认退出", "游戏结束");
			}
		}

		protected virtual void P2C_LoginAsViewer(MobaMessage msg)
		{
			PvpServer.LockScreen(false);
			RetaMsg probufMsg = msg.GetProbufMsg<RetaMsg>();
			byte retaCode = probufMsg.retaCode;
			PvpStateBase.LogState("===>receive: P2C_LoginAsViewer:" + retaCode);
			if (retaCode == 0)
			{
				Singleton<PvpManager>.Instance.LoadPvpSceneBegin();
				this.RecoverFinish(new PvpStateLoad());
			}
			else
			{
				ClientLogger.Error("P2C_LoginAsViewer: failed for " + retaCode);
				CtrlManager.ShowMsgBox("观战失败", "观战失败，游戏已经结束或者没有观战的权限", new Action(PvpUtils.GoHome), PopViewType.PopOneButton, "确定", "取消", null);
			}
		}

		private void P2C_BackLoadingInfo(MobaMessage msg)
		{
			InLoadingRuntimeInfo probufMsg = msg.GetProbufMsg<InLoadingRuntimeInfo>();
			PvpStateBase.LogState("receive P2C_BackLoadingInfo  " + StringUtils.DumpObject(probufMsg));
			Singleton<PvpManager>.Instance.RoomInfo.UpdateAllLoadProgress(probufMsg.loadProcessDic);
		}

		private void P2C_RefreshInFightInfo(MobaMessage msg)
		{
			PvpServer.LockScreen(false);
			InBattleRuntimeInfo probufMsg = msg.GetProbufMsg<InBattleRuntimeInfo>();
			PvpStateBase.LogState("receive P2C_RefreshInFightInfo  " + StringUtils.DumpObject(probufMsg));
			PvpProtocolTools.SyncFightInfo(probufMsg);
			if (probufMsg != null && probufMsg.roomState == 3)
			{
				Singleton<PvpManager>.Instance.AbandonGame(PvpErrorCode.StateError);
			}
			this.RecoverFinish(new PvpStateStart(PvpStateCode.PvpStart));
		}

		private void P2C_QueryInFightInfo(MobaMessage msg)
		{
			InBattleRuntimeInfo probufMsg = msg.GetProbufMsg<InBattleRuntimeInfo>();
			PvpStateBase.LogState("receive P2C_QueryInFightInfo " + StringUtils.DumpObject(probufMsg));
			if (probufMsg == null)
			{
				PvpStateBase.LogState("no fight info");
			}
			else
			{
				GameManager.Instance.ReplayController.SaveReconnectSyncMsgForRecord(msg.Param);
				PvpProtocolTools.SyncFightInfo(probufMsg);
				if (probufMsg.roomState == 3)
				{
					Singleton<PvpManager>.Instance.AbandonGame(PvpErrorCode.StateError);
				}
			}
			if (this.TargetState == PlayerState.InFight || probufMsg != null)
			{
				this.RecoverFinish(new PvpStateStart(PvpStateCode.PvpStart));
			}
			else
			{
				SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_LoadingOK, null);
				this.RecoverFinish(new PvpStateLoad());
			}
		}

		private void ConfirmQuiting(string text, string title = "游戏结束")
		{
			CtrlManager.ShowMsgBox(title, text, delegate
			{
				this.RecoverFinish(new PvpStateHome());
			}, PopViewType.PopOneButton, "确定", "取消", null);
		}
	}
}
