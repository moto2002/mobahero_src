using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaClient;
using MobaHeros.Gate;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace MobaHeros.Pvp.State
{
	public abstract class PvpStateBase
	{
		protected delegate void MsgRegFn(PvpCode code, MobaMessageFunc func);

		protected delegate void MsgRegFnLobby(LobbyCode code, MobaMessageFunc func);

		public PvpStateCode StateCode
		{
			get;
			private set;
		}

		protected PvpStateBase(PvpStateCode stateCode)
		{
			this.StateCode = stateCode;
		}

		private void RegMsgs(PvpStateBase.MsgRegFn func, PvpStateBase.MsgRegFnLobby funcLobby)
		{
			funcLobby(LobbyCode.L2C_TipMessage, new MobaMessageFunc(this.L2C_TipMessage));
			func(PvpCode.P2C_Packages, new MobaMessageFunc(this.P2C_Packages));
			func(PvpCode.P2C_ViewerUpdate, new MobaMessageFunc(this.P2C_ViewerUpdate));
			func(PvpCode.P2C_LoadingOK, new MobaMessageFunc(this.P2C_LoadingOK));
			func(PvpCode.BattleEnd, new MobaMessageFunc(this.OnBattleEnd));
			func(PvpCode.P2C_OtherLogin, new MobaMessageFunc(this.OtherLogin));
		}

		public void Enter()
		{
			GateReconnection.OnConnectedEvent += new Action(this.GateServer_OnConnected);
			MobaMessageManager.RegistMessage((ClientMsg)20001, new MobaMessageFunc(this.OnConnect));
			MobaMessageManager.RegistMessage((ClientMsg)20002, new MobaMessageFunc(this.OnDisconnect));
			this.RegMsgs(new PvpStateBase.MsgRegFn(MobaMessageManager.RegistMessage), new PvpStateBase.MsgRegFnLobby(MobaMessageManager.RegistMessage));
			this.RegistCallbacks();
			this.OnEnter();
			MobaMessageManager.ExecuteMsg(ClientC2C.PvpStateEnter, this.StateCode, 0f);
		}

		public void Exit()
		{
			MobaMessageManager.ExecuteMsg(ClientC2C.PvpStateExit, this.StateCode, 0f);
			this.OnExit();
			this.UnregistCallbacks();
			GateReconnection.OnConnectedEvent -= new Action(this.GateServer_OnConnected);
			MobaMessageManager.UnRegistMessage((ClientMsg)20001, new MobaMessageFunc(this.OnConnect));
			MobaMessageManager.UnRegistMessage((ClientMsg)20002, new MobaMessageFunc(this.OnDisconnect));
			this.RegMsgs(new PvpStateBase.MsgRegFn(MobaMessageManager.UnRegistMessage), new PvpStateBase.MsgRegFnLobby(MobaMessageManager.UnRegistMessage));
		}

		public virtual void OnUpdate()
		{
		}

		protected virtual void RegistCallbacks()
		{
		}

		protected virtual void UnregistCallbacks()
		{
		}

		protected virtual void OnEnter()
		{
		}

		protected virtual void OnExit()
		{
		}

		protected virtual void OnConnectServer(MobaPeerType type)
		{
		}

		protected virtual void OnDisconnectServer(MobaPeerType type)
		{
		}

		protected virtual void OnAfterBattleEnd(P2CBattleEndInfo info)
		{
			Singleton<PvpManager>.Instance.AbandonGame(PvpErrorCode.UnknowError);
			Singleton<PvpManager>.Instance.RoomInfo.WinTeam = null;
			Singleton<PvpManager>.Instance.RoomInfo.BattleResult = null;
			NetWorkHelper.Instance.DisconnectPvpServer();
		}

		protected virtual void OnAfterLoadOk(InBattleRuntimeInfo info)
		{
		}

		private void OnConnect(MobaMessage msg)
		{
			PeerConnectedMessage peerConnectedMessage = msg.Param as PeerConnectedMessage;
			MobaPeerType peerType = peerConnectedMessage.PeerType;
			if (peerConnectedMessage.ConnectedType == MobaConnectedType.ExceptionOnConnect)
			{
				return;
			}
			if (peerConnectedMessage.PeerType == MobaPeerType.C2GateServer)
			{
				return;
			}
			this.OnConnectServer(peerType);
		}

		private void GateServer_OnConnected()
		{
			this.OnConnectServer(MobaPeerType.C2GateServer);
		}

		private void OnDisconnect(MobaMessage msg)
		{
			PeerDisconnectedMessage peerDisconnectedMessage = msg.Param as PeerDisconnectedMessage;
			MobaPeerType peerType = peerDisconnectedMessage.PeerType;
			this.OnDisconnectServer(peerType);
		}

		private void L2C_TipMessage(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			string text = (string)operationResponse.Parameters[0];
			switch (Convert.ToInt32(operationResponse.Parameters[1]))
			{
			case 0:
				Singleton<TipView>.Instance.SetText(text, 0f);
				CtrlManager.OpenWindow(WindowID.TipView, null);
				break;
			case 1:
				CtrlManager.ShowMsgBox("错误", text, delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
				break;
			}
		}

		public static Dictionary<byte, object> BuildParams(byte[] args)
		{
			if (args == null || args.Length < 0)
			{
				return null;
			}
			return new Dictionary<byte, object>
			{
				{
					0,
					args
				}
			};
		}

		private void P2C_Packages(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			byte[] buffer = (byte[])operationResponse.Parameters[0];
			Packages packages = SerializeHelper.Deserialize<Packages>(buffer);
			FrameSyncManager.Instance.newNetFrameNum = ((!FrameSyncManager.Instance.UseFrame) ? packages.tick : packages.frameId);
			if (packages.packages != null)
			{
				if (!Singleton<PvpManager>.Instance.IsObserver)
				{
				}
				for (int i = 0; i < packages.packages.Length; i++)
				{
					RelpayCmd relpayCmd = packages.packages[i];
					MobaMessage message = MobaMessageManager.GetMessage((PvpCode)relpayCmd.code, relpayCmd.paramListBytes, 0f, null);
					if (MobaMessageManager.IsHandlerExists(message))
					{
						FrameSyncManager.Instance.ReceiveMsg(message);
					}
					else if (!Singleton<PvpManager>.Instance.IsObserver)
					{
					}
				}
			}
			if (FrameSyncManager.Instance.WaitFrameTime || packages.packages != null)
			{
				FrameSyncManager.Instance.ReceiveMsg(MobaMessageManager.GetMessage(PvpCode.P2C_FrameSync, FrameSyncManager.Instance.newNetFrameNum, 0f, null));
			}
		}

		private void P2C_ViewerUpdate(MobaMessage msg)
		{
			ViewerInfo probufMsg = msg.GetProbufMsg<ViewerInfo>();
			int count = probufMsg.count;
			Singleton<PvpManager>.Instance.ObserverCount = count;
		}

		private void P2C_LoadingOK(MobaMessage msg)
		{
			InBattleRuntimeInfo probufMsg = msg.GetProbufMsg<InBattleRuntimeInfo>();
			PvpStateBase.LogState("receive P2C_LoadingOK " + StringUtils.DumpObject(probufMsg));
			Singleton<PvpManager>.Instance.HasRecvLoadingOk = true;
			Singleton<PvpManager>.Instance.GameStartTime = new DateTime?(DateTime.Now);
			this.OnAfterLoadOk(probufMsg);
		}

		private void OnBattleEnd(MobaMessage msg)
		{
			P2CBattleEndInfo probufMsg = msg.GetProbufMsg<P2CBattleEndInfo>();
			PvpTeamInfo teamInfo = probufMsg.teamInfo;
			Singleton<PvpManager>.Instance.RoomInfo.WinTeam = new TeamType?(PvpProtocolTools.GroupToTeam((int)probufMsg.winGroup));
			Singleton<PvpManager>.Instance.RoomInfo.BattleResult = teamInfo;
			this.OnAfterBattleEnd(probufMsg);
			AutoTestController.InvokeTestLogic(AutoTestTag.LeavePvp, delegate
			{
				PvpUtils.GoHome();
			}, 1f);
		}

		private void OtherLogin(MobaMessage msg)
		{
			NetWorkHelper.Instance.Enable(false);
			CtrlManager.ShowMsgBox("重复登录", "英雄，您在别的地方登录了,传送门将关闭", delegate
			{
				if (GlobalSettings.isLoginByAnySDK)
				{
					InitAnySDK.getInstance().logout();
				}
				else if (GlobalSettings.isLoginByHoolaiSDK)
				{
					InitSDK.instance.SDKLogout(true);
				}
				else
				{
					GlobalObject.ReStartGame();
				}
			}, PopViewType.PopOneButton, "确定", "取消", null);
		}

		protected static void LogState(object msg)
		{
		}
	}
}
