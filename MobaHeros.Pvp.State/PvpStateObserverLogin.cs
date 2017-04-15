using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using GameLogin.State;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Diagnostics;

namespace MobaHeros.Pvp.State
{
	public class PvpStateObserverLogin : PvpStateBase
	{
		private const string Key = "PvpStateObserverLogin";

		private int _roomId;

		private readonly string _userId;

		private Task _checkFailTask;

		public PvpStateObserverLogin(PvpStateCode stateCode, string userId) : base(stateCode)
		{
			this._userId = userId;
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			Singleton<PvpManager>.Instance.SendLobbyMsg(LobbyCode.C2L_LookFriendFight, new object[]
			{
				this._userId
			});
			this._checkFailTask = new Task(this.CheckFail(), true);
			MobaMessageManagerTools.BeginWaiting_manual("PvpStateObserverLogin", "连接服务器", false, 1000f, false);
		}

		protected override void OnExit()
		{
			base.OnExit();
			Task.Clear(ref this._checkFailTask);
			MobaMessageManagerTools.EndWaiting_manual("PvpStateObserverLogin");
		}

		protected override void RegistCallbacks()
		{
			base.RegistCallbacks();
			MobaMessageManager.RegistMessage(LobbyCode.C2L_LookFriendFight, new MobaMessageFunc(this.L2C_LookFriendFight));
			MobaMessageManager.RegistMessage(PvpCode.C2P_LoginAsViewer, new MobaMessageFunc(this.C2P_LoginAsViewer));
		}

		protected override void UnregistCallbacks()
		{
			base.UnregistCallbacks();
			MobaMessageManager.UnRegistMessage(LobbyCode.C2L_LookFriendFight, new MobaMessageFunc(this.L2C_LookFriendFight));
			MobaMessageManager.UnRegistMessage(PvpCode.C2P_LoginAsViewer, new MobaMessageFunc(this.C2P_LoginAsViewer));
		}

		private void L2C_LookFriendFight(MobaMessage msg)
		{
			this._roomId = 0;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			string text = (string)operationResponse.Parameters[0];
			PlayerState playerState = (PlayerState)((byte)operationResponse.Parameters[1]);
			byte[] buffer = (byte[])operationResponse.Parameters[2];
			if (playerState == PlayerState.InFight || playerState == PlayerState.Loading)
			{
				InBattleLobbyInfo inBattleLobbyInfo = SerializeHelper.Deserialize<InBattleLobbyInfo>(buffer);
				if (inBattleLobbyInfo != null)
				{
					InBattleRoomInfo inBattleRoomInfo = SerializeHelper.Deserialize<InBattleRoomInfo>(inBattleLobbyInfo.InBattleRoomInfoBytes);
					this.SaveCurrentState(inBattleRoomInfo.roomInfo, inBattleLobbyInfo.loginInfo, inBattleRoomInfo.playerInfos);
					return;
				}
			}
			this.AlertAndGoHome("战斗已经结束");
		}

		private void SaveCurrentState(BattleRoomInfo roomInfo, PvpStartGameInfo loginInfo, ReadyPlayerSampleInfo[] playerInfos)
		{
			this._roomId = roomInfo.roomId;
			Singleton<PvpManager>.Instance.LoginInfo = loginInfo;
			Singleton<PvpManager>.Instance.ServerBattleRoomInfo = roomInfo;
			Singleton<PvpManager>.Instance.SetBattleInfoWithoutJoinType(roomInfo.battleId);
			Singleton<PvpManager>.Instance.SetRoomInfo(roomInfo, -2147483648, playerInfos, ToolsFacade.Instance.GetSummIdByUserid(long.Parse(this._userId)).ToString());
			NetWorkHelper.Instance.DisconnectFromGateServer(false);
			NetWorkHelper.Instance.ConnectToPvpServer();
			MobaMessageManager.RegistMessage((ClientMsg)20007, new MobaMessageFunc(this.OnConnectPvp));
		}

		private void OnConnectPvp(MobaMessage msg)
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)20007, new MobaMessageFunc(this.OnConnectPvp));
			SendMsgManager.Instance.SendPvpLoginMsgBase<C2PLoginAsViewer>(PvpCode.C2P_LoginAsViewer, new C2PLoginAsViewer
			{
				newUid = int.Parse(ModelManager.Instance.Get_userData_X().UserId),
				roomId = this._roomId
			}, this._roomId);
		}

		private void C2P_LoginAsViewer(MobaMessage msg)
		{
			RetaMsg probufMsg = msg.GetProbufMsg<RetaMsg>();
			byte retaCode = probufMsg.retaCode;
			if (retaCode == 0)
			{
				Singleton<PvpManager>.Instance.LoadPvpSceneBegin();
				PvpStateManager.Instance.ChangeState(new PvpStateLoad());
			}
			else
			{
				ClientLogger.Error("C2P_LoginAsViewer: failed for " + retaCode);
				this.AlertAndGoHome("连接Pvp服务器失败");
			}
		}

		[DebuggerHidden]
		private IEnumerator CheckFail()
		{
			PvpStateObserverLogin.<CheckFail>c__Iterator52 <CheckFail>c__Iterator = new PvpStateObserverLogin.<CheckFail>c__Iterator52();
			<CheckFail>c__Iterator.<>f__this = this;
			return <CheckFail>c__Iterator;
		}

		private void AlertAndGoHome(string msg)
		{
			PvpStateManager.Instance.ChangeState(new PvpStateHome());
			CtrlManager.ShowMsgBox("观战", msg, delegate
			{
			}, PopViewType.PopOneButton, "确定", "取消", null);
		}
	}
}
