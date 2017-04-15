using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace MobaHeros.Gate
{
	public class GateReconnection : ReconnectionBase
	{
		public enum ConnectState
		{
			Begin,
			ValidateToken,
			ConnectLogin,
			ValidLogin,
			ConnectGateAgain,
			SelectServer,
			End
		}

		private class LeaveReason
		{
			public string reason = string.Empty;

			public float leaveTime;

			public float leaveTimeSpan;

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("reason: {0} leaveTime: {1} leaveTimeSpan: {2}", this.reason, this.leaveTime, this.leaveTimeSpan);
				return stringBuilder.ToString();
			}
		}

		private const int MaxConnectRetryNum = 3;

		private const string NoticeReconnectTitle = "游戏服务器断开";

		private const string NoticeReconnectContent = "网络出故障了，请重试";

		private GateReconnection.ConnectState _connectState = GateReconnection.ConnectState.End;

		private float _timeout;

		private bool _confirmed;

		private GateReconnection.LeaveReason _leaveReason;

		private bool _disableProgressBarOnce;

		private int _retryNum;

		private bool _enabled;

		private bool _available = true;

		public static event Action OnConnectedEvent
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				GateReconnection.OnConnectedEvent = (Action)Delegate.Combine(GateReconnection.OnConnectedEvent, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				GateReconnection.OnConnectedEvent = (Action)Delegate.Remove(GateReconnection.OnConnectedEvent, value);
			}
		}

		public static bool CanTrigger
		{
			get;
			set;
		}

		public GateReconnection.ConnectState State
		{
			get
			{
				return this._connectState;
			}
		}

		public bool Available
		{
			get
			{
				return this._available;
			}
			set
			{
				this._available = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return this._enabled;
			}
			set
			{
				if (this._enabled == value)
				{
					return;
				}
				this._enabled = value;
				if (!value)
				{
					this.End(false);
				}
			}
		}

		public GateReconnection()
		{
			this.Available = true;
		}

		public void LeaveGame(string reason)
		{
			this._leaveReason = new GateReconnection.LeaveReason
			{
				reason = reason,
				leaveTime = Time.realtimeSinceStartup
			};
		}

		public override void Begin()
		{
			if (base.IsBegan)
			{
				return;
			}
			base.Begin();
			if (NetWorkHelper.Instance.IsGateConnected)
			{
				this.OnGateConnected();
			}
			else
			{
				NetWorkHelper.Instance.ConnectToGateServer();
			}
			this._connectState = GateReconnection.ConnectState.Begin;
			this._timeout = Time.realtimeSinceStartup + 5f;
		}

		public override void End(bool success)
		{
			if (!base.IsBegan)
			{
				return;
			}
			base.End(success);
			if (!success)
			{
				NetWorkHelper.Instance.DisconnectFromMasterServer();
				NetWorkHelper.Instance.DisconnectFromGateServer(true);
			}
			else
			{
				MobaMessageManager.ExecuteMsg(ClientC2C.GateConnected, null, 0f);
			}
			this._connectState = GateReconnection.ConnectState.End;
			this.OnReconnectEnd(success);
		}

		public override void OnUpdate()
		{
			if (!base.IsBegan)
			{
				return;
			}
			if (Time.realtimeSinceStartup > this._timeout)
			{
				this.End(false);
			}
		}

		protected override void RegisterCallbacks()
		{
			base.RegisterCallbacks();
			MVC_MessageManager.AddListener_view(MobaGateCode.VerificationKey, new MobaMessageFunc(this.OnVerificationKey));
			MVC_MessageManager.AddListener_view(MobaGateCode.SelectGameServer, new MobaMessageFunc(this.OnSelectGameServer));
			MVC_MessageManager.AddListener_view(MobaMasterCode.Login, new MobaMessageFunc(this.OnValidateLogin));
		}

		protected override void UnregisterCallbacks()
		{
			base.UnregisterCallbacks();
			MVC_MessageManager.RemoveListener_view(MobaGateCode.VerificationKey, new MobaMessageFunc(this.OnVerificationKey));
			MVC_MessageManager.RemoveListener_view(MobaGateCode.SelectGameServer, new MobaMessageFunc(this.OnSelectGameServer));
			MVC_MessageManager.RemoveListener_view(MobaMasterCode.Login, new MobaMessageFunc(this.OnValidateLogin));
		}

		protected override void OnConnect(PeerConnectedMessage msg)
		{
			if (!GateReconnection.CanTrigger)
			{
				return;
			}
			if (msg.ConnectedType == MobaConnectedType.ExceptionOnConnect)
			{
				return;
			}
			if (msg.PeerType == MobaPeerType.C2GateServer)
			{
				if (this.State == GateReconnection.ConnectState.ConnectGateAgain)
				{
					this.SelectServer();
				}
				else if (this.State == GateReconnection.ConnectState.Begin)
				{
					this.OnGateConnected();
				}
			}
			else if (msg.PeerType == MobaPeerType.C2LoginServer)
			{
				this.OnLoginConnected();
			}
		}

		protected override void OnDisconnect(PeerDisconnectedMessage msg)
		{
			if (msg.PeerType != MobaPeerType.C2GateServer)
			{
				return;
			}
			if (!this.Available)
			{
				return;
			}
			this.Available = false;
			MobaMessageManager.ExecuteMsg(ClientC2C.GateDisconnected, null, 0f);
			if (this.Enabled && GateReconnection.CanTrigger)
			{
				this.ConfirmConnect();
			}
		}

		public override void OnApplicationFocus(bool isFocus)
		{
			base.OnApplicationFocus(isFocus);
			if (isFocus && this._leaveReason != null && this.Enabled)
			{
				this.CheckLeave();
			}
		}

		private void CheckLeave()
		{
			this._leaveReason.leaveTimeSpan = Time.realtimeSinceStartup - this._leaveReason.leaveTime;
			if (this._leaveReason.leaveTimeSpan < 60000f)
			{
				this._disableProgressBarOnce = true;
			}
			this._leaveReason = null;
		}

		private void OnGateConnected()
		{
			AccountData accountData = ModelManager.Instance.Get_accountData_X();
			ServerListModelData serverListModelData = ModelManager.Instance.Get_serverInfo();
			string accountId = accountData.AccountId;
			string tokenKey = serverListModelData.m_TokenKey;
			SendMsgManager.Instance.SendGateSelfChannelMessage(21, new SendMsgManager.SendMsgParam(true, string.Empty, true, 15f), new object[]
			{
				accountId,
				tokenKey
			});
			this._connectState = GateReconnection.ConnectState.ValidateToken;
		}

		private void OnVerificationKey(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[6];
			int num2 = num;
			if (num2 != 1)
			{
				if (num != 2)
				{
					ClientLogger.Assert(false, null);
				}
				NetWorkHelper.Instance.DisconnectFromGateServer(true);
				this._connectState = GateReconnection.ConnectState.ConnectLogin;
				if (NetWorkHelper.Instance.MasterServerFlag)
				{
					this.OnLoginConnected();
				}
				else
				{
					NetWorkHelper.Instance.ConnectToMasterServer();
				}
			}
			else
			{
				this.End(true);
			}
		}

		private void OnLoginConnected()
		{
			AccountData tempAccData = LoginStateManager.Instance.TempAccData;
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在登录服务器...", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaMasterCode.Login, param, new object[]
			{
				tempAccData
			});
			this._connectState = GateReconnection.ConnectState.ValidLogin;
		}

		private void OnValidateLogin(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				ClientLogger.Error("OnValidateLogin failed, it's terrible and I cannot handle it");
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			string debugMessage = operationResponse.DebugMessage;
			if (num == 0)
			{
				AccountData accountData = ModelManager.Instance.Get_accountData_X();
				if (accountData != null)
				{
					AnalyticsToolManager.SetAccountId(accountData.AccountId);
				}
				this.ConnectGateAgain();
				this._connectState = GateReconnection.ConnectState.SelectServer;
			}
		}

		private void ConnectGateAgain()
		{
			NetWorkHelper.Instance.DisconnectFromMasterServer();
			if (NetWorkHelper.Instance.IsGateConnected)
			{
				this.SelectServer();
			}
			else
			{
				NetWorkHelper.Instance.ConnectToGateServer();
			}
			this._connectState = GateReconnection.ConnectState.ConnectGateAgain;
		}

		private void SelectServer()
		{
			ServerListModelData serverInfo = ModelManager.Instance.Get_serverInfo();
			ClientLogger.AssertNotNull(serverInfo, "serverInfo == null");
			string tcpaddress = serverInfo.serverlist.Find((ServerInfo obj) => obj.serverid == serverInfo.ServerId).tcpaddress;
			string sessionId = serverInfo.SessionId;
			string lobbyId = serverInfo.LobbyId;
			SendMsgManager.Instance.SendGateSelfChannelMessage(20, new SendMsgManager.SendMsgParam(true, string.Empty, true, 15f), new object[]
			{
				tcpaddress,
				sessionId,
				lobbyId
			});
		}

		private void OnSelectGameServer(MobaMessage msg)
		{
			this.End(true);
		}

		private void BeginReconnect()
		{
			this.Begin();
		}

		private void ReconnectFinish()
		{
			this._disableProgressBarOnce = false;
			this._retryNum = 0;
			this.Available = true;
			this.ShowProgress(false);
			if (GateReconnection.OnConnectedEvent != null)
			{
				GateReconnection.OnConnectedEvent();
			}
		}

		private void OnReconnectEnd(bool ok)
		{
			if (ok)
			{
				this.ReconnectFinish();
			}
			else if (this.Enabled && !this._confirmed)
			{
				this.ConfirmConnect();
			}
		}

		private void ConfirmConnect()
		{
			this._confirmed = true;
			if (this._retryNum < 3)
			{
				this.OnClickConnect();
			}
			else
			{
				this._disableProgressBarOnce = false;
				this._retryNum = 0;
				this.ShowProgress(false);
				CtrlManager.ShowMsgBox("游戏服务器断开", "网络出故障了，请重试", delegate(bool yes)
				{
					if (yes)
					{
						this.OnClickConnect();
					}
					else
					{
						this.OnClickRestart();
					}
				}, PopViewType.PopTwoButton, "确定", "重启游戏", null);
			}
		}

		private void ShowProgress(bool show)
		{
			if (show)
			{
				if (!CtrlManager.IsWindowOpen(WindowID.ProgressView))
				{
					this._retryNum = 1;
				}
				ProgressView.ShowProgressText(string.Concat(new object[]
				{
					"正在尝试重连",
					this._retryNum,
					"/",
					3
				}));
			}
			else
			{
				CtrlManager.CloseWindow(WindowID.ProgressView);
			}
		}

		private void OnClickRestart()
		{
			this._confirmed = false;
			this._retryNum = 0;
			this.ShowProgress(false);
			GlobalObject.ReStartGame();
		}

		private void OnClickConnect()
		{
			this._confirmed = false;
			this._retryNum++;
			if (!this._disableProgressBarOnce)
			{
				this.ShowProgress(true);
			}
			this.BeginReconnect();
		}
	}
}
