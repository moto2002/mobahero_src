using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using GameLogin.State;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;

namespace MobaHeros.Pvp.State
{
	public class PvpStateSelectHero : PvpStateBase
	{
		private bool _isGsConnected = true;

		public PvpStateSelectHero() : base(PvpStateCode.PvpSelectHero)
		{
		}

		protected override void OnExit()
		{
			base.OnExit();
			PvpMatchMgr.Instance.ClearTask();
		}

		private void RegMsgs(PvpStateBase.MsgRegFn func, PvpStateBase.MsgRegFnLobby funcLobby)
		{
			funcLobby(LobbyCode.C2L_RoomSelectHero, new MobaMessageFunc(this.L2C_RoomSelectHero));
			funcLobby(LobbyCode.C2L_RoomSelectHeroOK, new MobaMessageFunc(this.L2C_RoomSelectHeroOK));
			funcLobby(LobbyCode.C2L_RoomRespChangeHero, new MobaMessageFunc(this.L2C_RoomRespChangeHero));
			funcLobby(LobbyCode.L2C_ModifyState, new MobaMessageFunc(this.L2C_ModifyState));
			funcLobby(LobbyCode.C2L_SelectSelfDefSkill, new MobaMessageFunc(this.L2C_SelectSelfDefSkill));
			funcLobby(LobbyCode.C2L_SelectHeroSkin, new MobaMessageFunc(this.L2C_SelectHeroSkin));
			funcLobby(LobbyCode.L2C_WaitForSkin, new MobaMessageFunc(this.L2C_WaitForSkin));
			funcLobby(LobbyCode.L2C_StartGame, new MobaMessageFunc(this.L2C_StartGame));
			func(PvpCode.P2C_LoginFight, new MobaMessageFunc(this.P2C_LoginFight));
			func(PvpCode.C2P_LoginAsViewer, new MobaMessageFunc(this.P2C_LoginAsViewer));
			funcLobby(LobbyCode.C2L_CaptionLobby, new MobaMessageFunc(this.P2C_CaptionLobby));
		}

		protected override void RegistCallbacks()
		{
			this.RegMsgs(new PvpStateBase.MsgRegFn(MobaMessageManager.RegistMessage), new PvpStateBase.MsgRegFnLobby(MobaMessageManager.RegistMessage));
		}

		protected override void UnregistCallbacks()
		{
			this.RegMsgs(new PvpStateBase.MsgRegFn(MobaMessageManager.UnRegistMessage), new PvpStateBase.MsgRegFnLobby(MobaMessageManager.UnRegistMessage));
		}

		protected override void OnConnectServer(MobaPeerType type)
		{
			base.OnConnectServer(type);
			if (type == MobaPeerType.C2PvpServer)
			{
				PvpStartGameInfo loginInfo = Singleton<PvpManager>.Instance.LoginInfo;
				if (Singleton<PvpManager>.Instance.IsObserver)
				{
					PvpStateBase.LogState("send C2P_LoginAsViewer");
					SendMsgManager.Instance.SendPvpLoginMsgBase<C2PLoginAsViewer>(PvpCode.C2P_LoginAsViewer, new C2PLoginAsViewer
					{
						newUid = int.Parse(ModelManager.Instance.Get_userData_X().UserId),
						roomId = loginInfo.roomId
					}, loginInfo.roomId);
				}
				else
				{
					PvpStateBase.LogState("send C2P_LoginFight");
					SendMsgManager.Instance.SendPvpLoginMsgBase<C2PLoginFight>(PvpCode.C2P_LoginFight, new C2PLoginFight
					{
						roomId = loginInfo.roomId,
						newUid = loginInfo.newUid,
						newkey = loginInfo.newKey
					}, loginInfo.roomId);
				}
			}
		}

		protected override void OnDisconnectServer(MobaPeerType type)
		{
			base.OnDisconnectServer(type);
			if (this._isGsConnected && type == MobaPeerType.C2GateServer)
			{
				PvpStateManager.Instance.ChangeState(new PvpStateRecover(PvpStateCode.PvpSelectHero));
			}
			if (!this._isGsConnected && type == MobaPeerType.C2PvpServer)
			{
				PvpStateManager.Instance.ChangeState(new PvpStateRecover(PvpStateCode.PvpLoad));
			}
		}

		private void L2C_RoomSelectHero(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[0];
			byte[] buffer = operationResponse.Parameters[1] as byte[];
			HeroInfo heroInfo = SerializeHelper.Deserialize<HeroInfo>(buffer);
			PvpStateBase.LogState(string.Format("===>receive: L2C_RoomSelectHero user {0} select {1}", num, StringUtils.DumpObject(heroInfo)));
			Singleton<PvpManager>.Instance.OnSelectHero(num, heroInfo);
		}

		private void L2C_RoomSelectHeroOK(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[0];
			bool flag = (bool)operationResponse.Parameters[1];
			PvpStateBase.LogState(string.Format("L2C_RoomSelectHeroOK user {0} ok={1}", num, flag));
			Singleton<PvpManager>.Instance.OnSelectHeroOk(num, flag);
		}

		private void L2C_RoomRespChangeHero(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			byte inResult = (byte)operationResponse.Parameters[0];
			int inNewUid = (int)operationResponse.Parameters[1];
			MobaMessageManager.DispatchMsg((ClientMsg)23069, new ParamShowSwitchHeroResult(inNewUid, inResult), 0f);
		}

		private void L2C_ModifyState(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[0];
			int inNewUid = (int)operationResponse.Parameters[1];
			if (num == 1)
			{
				MobaMessageManager.DispatchMsg((ClientMsg)23069, new ParamShowSwitchHeroResult(inNewUid, 2), 0f);
			}
		}

		private void L2C_StartGame(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			byte[] array = (byte[])operationResponse.Parameters[0];
			byte[] buffer = (byte[])operationResponse.Parameters[1];
			PvpStartGameInfo pvpStartGameInfo = SerializeHelper.Deserialize<PvpStartGameInfo>(array);
			BattleRoomInfo battleRoomInfo = SerializeHelper.Deserialize<BattleRoomInfo>(buffer);
			PvpStateBase.LogState("L2C_StartGame:" + (StringUtils.DumpObject(array) ?? "null"));
			if (pvpStartGameInfo != null && battleRoomInfo != null)
			{
				this._isGsConnected = false;
				NetWorkHelper.Instance.DisconnectFromGateServer(false);
				NetWorkHelper.Instance.DisconnectLobbyServer();
				Singleton<PvpManager>.Instance.LoginInfo = pvpStartGameInfo;
				Singleton<PvpManager>.Instance.ServerBattleRoomInfo = battleRoomInfo;
				NetWorkHelper.Instance.ConnectToPvpServer();
				if (!string.IsNullOrEmpty(battleRoomInfo.roomVoiceID))
				{
				}
			}
			else
			{
				ClientLogger.Error("L2C_StartGame: PvpStartGameInfo is null");
				PvpStateManager.Instance.ChangeState(new PvpStateHome());
			}
		}

		private void P2C_LoginAsViewer(MobaMessage msg)
		{
			RetaMsg probufMsg = msg.GetProbufMsg<RetaMsg>();
			byte retaCode = probufMsg.retaCode;
			PvpStateBase.LogState("===>receive: P2C_LoginAsViewer:" + retaCode);
			if (retaCode == 0)
			{
				Singleton<PvpManager>.Instance.LoadPvpSceneBegin();
				PvpStateManager.Instance.ChangeState(new PvpStateLoad());
			}
			else
			{
				ClientLogger.Error("P2C_LoginAsViewer: 别怕，帮助@汪洋定位问题，failed for " + retaCode);
				CtrlManager.ShowMsgBox("观战失败", "观战失败，游戏已经结束或者没有观战的权限", new Action(PvpUtils.GoHome), PopViewType.PopOneButton, "确定", "取消", null);
			}
		}

		private void P2C_LoginFight(MobaMessage msg)
		{
			RetaMsg probufMsg = msg.GetProbufMsg<RetaMsg>();
			byte retaCode = probufMsg.retaCode;
			PvpStateBase.LogState("===>receive: P2C_LoginFight:" + retaCode);
			if (retaCode == 0)
			{
				Singleton<PvpManager>.Instance.LoadPvpSceneBegin();
				PvpStateManager.Instance.ChangeState(new PvpStateLoad());
			}
			else
			{
				ClientLogger.Error("P2C_LoginFight: 别怕，帮助@liuchen定位问题，failed for " + (PvpErrorCode)retaCode);
				CtrlManager.ShowMsgBox("错误", "服务器错误码" + retaCode, new Action(PvpUtils.GoHome), PopViewType.PopOneButton, "确定", "取消", null);
			}
		}

		private void P2C_CaptionLobby(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			string text = (string)operationResponse.Parameters[1];
			PvpStateBase.LogState("===>receive: On_GetBarrage:" + text);
			if (text != string.Empty)
			{
				ModelManager.Instance.BarrageQueue_Enqueue(text);
			}
			else
			{
				ClientLogger.Error("P2C_Caption: null barrage content. ");
			}
		}

		private void L2C_SelectSelfDefSkill(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[0];
			string text = (string)operationResponse.Parameters[1];
			PvpStateBase.LogState(string.Concat(new object[]
			{
				"===>receive: C2L_SelectSelfDefSkill:",
				num,
				" ",
				text
			}));
			Singleton<PvpManager>.Instance.OnSelectHeroSkill(num, text);
		}

		private void L2C_SelectHeroSkin(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[0];
			string text = (string)operationResponse.Parameters[1];
			PvpStateBase.LogState(string.Concat(new object[]
			{
				"===>receive: C2L_SelectHeroSkin:",
				num,
				" ",
				text
			}));
			Singleton<PvpManager>.Instance.OnSelectHeroSkin(num, text);
		}

		private void L2C_WaitForSkin(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[0];
			PvpStateBase.LogState("===>receive: L2C_WaitForSkin:" + num);
			this.GuardState(num);
		}

		private void GuardState(int leftSeconds)
		{
			leftSeconds = ((leftSeconds >= 0) ? leftSeconds : 0);
			new Task(delegate
			{
				if (PvpStateManager.Instance.StateCode == base.StateCode && this._isGsConnected)
				{
					PvpStateManager.Instance.ChangeState(new PvpStateRecover(PvpStateCode.PvpSelectHero, 5));
				}
			}, (float)(leftSeconds + 5));
		}
	}
}
