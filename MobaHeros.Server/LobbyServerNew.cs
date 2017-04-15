using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using MobaClient;
using MobaHeros.Pvp;
using MobaProtocol;
using System;

namespace MobaHeros.Server
{
	public class LobbyServerNew : PeerServerBase
	{
		private string _userId;

		private string _userName;

		private string _serverId;

		public LobbyServerNew(PhotonClient client) : base(client, "lobby server")
		{
		}

		private void SetServerInfo(string serverId, string userId, string userName)
		{
			this._userId = userId;
			this._userName = userName;
			this._serverId = serverId;
		}

		protected override void RegisterCmds()
		{
			base.RegisterCmds();
			MobaMessageManager.RegistMessage(LobbyCode.L2C_LoginLobby, new MobaMessageFunc(this.L2C_LoginLobby));
		}

		protected override void UnregisterCmds()
		{
			base.UnregisterCmds();
			MobaMessageManager.UnRegistMessage(LobbyCode.L2C_LoginLobby, new MobaMessageFunc(this.L2C_LoginLobby));
		}

		public override void Begin()
		{
			string userName = ModelManager.Instance.Get_accountData_filed_X("UserName");
			string userId = ModelManager.Instance.Get_userData_filed_X("UserId");
			string serverId = "todo";
			this.SetServerInfo(serverId, userId, userName);
			base.Begin();
		}

		protected override MobaPeer ConnectPeer()
		{
			MobaPeer mobaPeer = this._client.GetMobaPeer(MobaPeerType.C2Lobby, true);
			mobaPeer.ServerName = GlobalSettings.Instance.PvpSetting.LobbyServerName;
			mobaPeer.ApplicationName = "MobaServer.lobby";
			if (!mobaPeer.ServerConnected)
			{
				if (!mobaPeer.PeerConnect())
				{
					ClientLogger.Error("PeerConnect failed for lobby");
				}
			}
			else
			{
				ClientLogger.Warn("Peer already connected");
			}
			return mobaPeer;
		}

		public override void OnConnected(MobaConnectedType cType)
		{
			if (cType != MobaConnectedType.ExceptionOnConnect)
			{
				this.ConnectFlag = true;
				MobaMessageManagerTools.EndWaiting_manual("ConnectLobby");
				int hashCode = this._serverId.GetHashCode();
				SendMsgManager.Instance.SendLobbyMsg(PvpCode.C2L_FakeLoginLobby, new object[]
				{
					this._userName,
					this._userId,
					hashCode
				});
			}
		}

		public override void OnDisconnected(MobaDisconnectedType dType)
		{
			if (this.ConnectFlag)
			{
				this.ConnectFlag = false;
			}
			if (this._peer != null)
			{
				MobaMessageManagerTools.EndWaiting_manual("ConnectLobby");
				CtrlManager.ShowMsgBox("Lobby服务器断开", "网络出故障了，请重试", new Action(this.Connect), PopViewType.PopOneButton, "确定", "取消", null);
			}
		}

		private void Connect()
		{
			MobaMessageManagerTools.BeginWaiting_manual("ConnectLobby", "连接Lobby服务器...", true, 15f, true);
			this.ConnectPeer();
		}

		private void L2C_LoginLobby(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			byte b = (byte)operationResponse.Parameters[0];
			PvpManager.IsDirectLinkLobby = true;
			if (b == 0)
			{
				Singleton<PvpManager>.Instance.SendLobbyMsg(LobbyCode.C2L_JoinQueue, new object[]
				{
					0,
					Singleton<PvpManager>.Instance.BattleId
				});
			}
			else
			{
				PvpUtils.ShowNetworkError((int)b);
			}
		}
	}
}
