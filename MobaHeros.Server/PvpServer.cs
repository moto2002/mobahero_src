using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using MobaClient;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MobaHeros.Server
{
	public class PvpServer : PeerServerBase
	{
		private class RunOnceTask
		{
			public Action action;

			public float happenTime;
		}

		private const int MaxAutoRetryNum = 5;

		private readonly List<PvpServer.RunOnceTask> _tasks = new List<PvpServer.RunOnceTask>();

		private static int _autoRetryNum;

		private bool _isConfirmingReconnect;

		private bool _isConfirmCd;

		public static bool IsMultithreadEnabled
		{
			get;
			private set;
		}

		public PvpServer(PhotonClient client) : base(client, "pvp server")
		{
			PvpServer.IsMultithreadEnabled = GlobalSettings.Instance.PvpSetting.multiThread;
			if (PvpServer.IsMultithreadEnabled)
			{
				base.UseMultithread = true;
			}
		}

		protected override MobaPeer ConnectPeer()
		{
			PvpStartGameInfo loginInfo = Singleton<PvpManager>.Instance.LoginInfo;
			SendMsgManager.Instance.ClientConnectToPVP(loginInfo.serverIp, loginInfo.serverPort, loginInfo.serverName);
			return this._client.pvpserver_peer;
		}

		public override void OnConnected(MobaConnectedType cType)
		{
			if (!base.HasBegun)
			{
				ClientLogger.Error("OnConnected received when end");
				return;
			}
			if (cType == MobaConnectedType.ExceptionOnConnect)
			{
				PvpServer.LockScreen(true);
				this.ConfirmReconnect();
			}
			else
			{
				this.ConnectFlag = true;
				MobaMessageManager.ExecuteMsg(MobaMessageManager.GetMessage((ClientMsg)20007, cType, 0f));
			}
			MobaMessageManager.ExecuteMsg(MobaMessageManager.GetMessage((ClientMsg)20001, new PeerConnectedMessage(MobaPeerType.C2PvpServer, cType), 0f));
		}

		public override void OnDisconnected(MobaDisconnectedType dType)
		{
			bool flag = this._peer == null;
			if (flag)
			{
				PvpServer.LockScreen(false);
			}
			if (this.ConnectFlag)
			{
				this.ConnectFlag = false;
				MobaMessageManager.ExecuteMsg(MobaMessageManager.GetMessage((ClientMsg)20008, dType, 0f));
			}
			this.ConfirmReconnect();
			MobaMessageManager.ExecuteMsg(MobaMessageManager.GetMessage((ClientMsg)20002, new PeerDisconnectedMessage(MobaPeerType.C2PvpServer, dType), 0f));
		}

		public static void LockScreen(bool locked)
		{
			if (locked)
			{
				int num = (PvpServer._autoRetryNum != 0) ? PvpServer._autoRetryNum : 1;
				ProgressView.ShowProgressText(string.Concat(new object[]
				{
					"重连中",
					num,
					"/",
					5
				}));
			}
			else
			{
				CtrlManager.CloseWindow(WindowID.ProgressView);
			}
		}

		private void ReconnectFinish()
		{
			PvpServer._autoRetryNum = 0;
			PvpServer.LockScreen(false);
		}

		private void ConfirmReconnect()
		{
			if (GlobalSettings.NoReConnnect || this._isConfirmCd || this._peer == null)
			{
				return;
			}
			if (this.ConnectFlag)
			{
				this.ReconnectFinish();
				return;
			}
			if (this._isConfirmingReconnect)
			{
				return;
			}
			if (PvpServer._autoRetryNum < 5)
			{
				PvpServer._autoRetryNum++;
				PvpServer.LockScreen(true);
				this.Connect();
			}
			else
			{
				CtrlManager.CloseWindow(WindowID.ProgressView);
				PvpServer._autoRetryNum = 0;
				this._isConfirmingReconnect = true;
				CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("ServerResponse_Title_GameServerDisconnect", "游戏服务器断开"), LanguageManager.Instance.GetStringById("ServerResponse_Content_GameServerDisconnect", "网络出故障了，请重试"), new Action<bool>(this.ConnectOrRestart), PopViewType.PopTwoButton, "确定", "重启游戏", null);
			}
		}

		private void Connect()
		{
			if (this.ConnectFlag)
			{
				return;
			}
			PvpStartGameInfo loginInfo = Singleton<PvpManager>.Instance.LoginInfo;
			SendMsgManager.Instance.ClientConnectToPVP(loginInfo.serverIp, loginInfo.serverPort, loginInfo.serverName);
			this._isConfirmCd = true;
			this.RunOnce(delegate
			{
				this._isConfirmCd = false;
				this.ConfirmReconnect();
			}, 3f);
		}

		private void RunOnce(Action action, float delay)
		{
			this._tasks.Add(new PvpServer.RunOnceTask
			{
				action = action,
				happenTime = Time.realtimeSinceStartup + delay
			});
		}

		private void ConnectOrRestart(bool yes)
		{
			this._isConfirmingReconnect = false;
			if (yes)
			{
				this.Connect();
			}
			else
			{
				PvpServer._autoRetryNum = 0;
				PvpServer.LockScreen(false);
				GlobalObject.ReStartGame();
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this._tasks.Count == 0)
			{
				return;
			}
			float now = Time.realtimeSinceStartup;
			List<Action> list = new List<Action>();
			list.AddRange(from x in this._tasks
			where now > x.happenTime
			select x.action);
			this._tasks.RemoveAll((PvpServer.RunOnceTask x) => now > x.happenTime);
			list.ForEach(delegate(Action x)
			{
				try
				{
					x();
				}
				catch (Exception e)
				{
					ClientLogger.LogException(e);
				}
			});
		}

		public override void OnStatusChanged(StatusCode code)
		{
			base.OnStatusChanged(code);
			if (!ModelManager.Instance.Get_IsInWhiteList())
			{
				return;
			}
			if (this._client != null && this._client.pvpserver_peer != null && (code == StatusCode.QueueIncomingReliableWarning || code == StatusCode.QueueOutgoingReliableWarning))
			{
				string text = " PvpPeer.StatusChanged " + code;
				text = string.Format("{0}, QueuedIncomingCommands:{1}, ByteCountCurrentDispatch:{2}, BytesIn:{3}, BytesOut:{4}, WarningSize:{5}", new object[]
				{
					text,
					this._client.pvpserver_peer.QueuedIncomingCommands,
					this._client.pvpserver_peer.ByteCountCurrentDispatch,
					this._client.pvpserver_peer.BytesIn,
					this._client.pvpserver_peer.BytesOut,
					this._client.pvpserver_peer.WarningSize
				});
				return;
			}
			if (code == StatusCode.Connect || code == StatusCode.Disconnect || code != StatusCode.EncryptionEstablished)
			{
			}
		}
	}
}
