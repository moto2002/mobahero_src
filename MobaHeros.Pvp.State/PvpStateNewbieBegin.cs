using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using GameLogin.State;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using Newbie;
using System;

namespace MobaHeros.Pvp.State
{
	public class PvpStateNewbieBegin : PvpStateBase
	{
		public PvpStateNewbieBegin() : base(PvpStateCode.PvpNewbieBegin)
		{
		}

		protected override void OnEnter()
		{
			base.OnEnter();
		}

		private void RegMsgs(PvpStateBase.MsgRegFn func, PvpStateBase.MsgRegFnLobby funcLobby)
		{
			funcLobby(LobbyCode.C2L_Newbie, new MobaMessageFunc(this.L2C_Newbie));
			func(PvpCode.P2C_LoginFight, new MobaMessageFunc(this.NewbieP2C_LoginFight));
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
				PvpStateBase.LogState("newbie send C2P_LoginFight");
				SendMsgManager.Instance.SendPvpLoginMsgBase<C2PLoginFight>(PvpCode.C2P_LoginFight, new C2PLoginFight
				{
					roomId = loginInfo.roomId,
					newUid = loginInfo.newUid,
					newkey = loginInfo.newKey
				}, loginInfo.roomId);
			}
		}

		protected override void OnDisconnectServer(MobaPeerType type)
		{
			base.OnDisconnectServer(type);
		}

		private void L2C_Newbie(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			byte b = (byte)operationResponse.Parameters[0];
			byte[] startGameDataOrig = operationResponse.Parameters[1] as byte[];
			if (b == 1)
			{
				this.HandleNewbieStartGame(startGameDataOrig);
			}
		}

		private void HandleNewbieStartGame(byte[] startGameDataOrig)
		{
			NewbieStartGameData newbieStartGameData = SerializeHelper.Deserialize<NewbieStartGameData>(startGameDataOrig);
			PvpStartGameInfo startGameInfo = newbieStartGameData.startGameInfo;
			BattleRoomInfo btRoomInfo = newbieStartGameData.btRoomInfo;
			ReadyPlayerSampleInfo[] playerInfos = newbieStartGameData.playerInfos;
			if (startGameInfo != null && btRoomInfo != null && playerInfos != null)
			{
				this.NewbieSetRoomInfo(startGameInfo.newUid, playerInfos);
				NetWorkHelper.Instance.DisconnectFromGateServer(false);
				NetWorkHelper.Instance.DisconnectLobbyServer();
				Singleton<PvpManager>.Instance.LoginInfo = startGameInfo;
				Singleton<PvpManager>.Instance.ServerBattleRoomInfo = btRoomInfo;
				NetWorkHelper.Instance.ConnectToPvpServer();
			}
			else
			{
				ClientLogger.Error("L2C_StartGame: PvpStartGameInfo is null");
				PvpStateManager.Instance.ChangeState(new PvpStateHome());
			}
		}

		private void NewbieSetRoomInfo(int inMyUserId, ReadyPlayerSampleInfo[] inPlayerInfos)
		{
			Singleton<PvpManager>.Instance.RoomInfo.NewbieSetInfo(inMyUserId, inPlayerInfos);
		}

		private void NewbieP2C_LoginFight(MobaMessage msg)
		{
			RetaMsg probufMsg = msg.GetProbufMsg<RetaMsg>();
			byte retaCode = probufMsg.retaCode;
			if (retaCode == 0)
			{
				NewbieManager.Instance.SetSpecialEnterBattleSuc();
				Singleton<PvpManager>.Instance.LoadPvpSceneBegin();
				PvpStateManager.Instance.ChangeState(new PvpStateLoad());
			}
			else
			{
				ClientLogger.Error("P2C_LoginFight: 别怕，帮助@汪洋定位问题，failed for " + retaCode);
				CtrlManager.ShowMsgBox("错误", "服务器错误码" + retaCode, new Action(PvpUtils.GoHome), PopViewType.PopOneButton, "确定", "取消", null);
			}
		}
	}
}
