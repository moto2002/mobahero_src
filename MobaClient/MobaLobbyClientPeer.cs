using ExitGames.Client.Photon;
using MobaProtocol;
using System;

namespace MobaClient
{
	public class MobaLobbyClientPeer : MobaPeer, IPhotonPeerListener
	{
		private StatusCode _lastStatusCode;

		public ClientMsgRecv MsgRecver
		{
			get;
			set;
		}

		public MobaLobbyClientPeer(ConnectionProtocol protocolType, PhotonClient client) : base(protocolType, client, MobaPeerType.C2Lobby)
		{
			this.mClient = client;
			base.Listener = this;
		}

		void IPhotonPeerListener.DebugReturn(DebugLevel level, string message)
		{
		}

		public void OnStatusChanged(StatusCode statusCode)
		{
			this.mClient.ExecSendMsg2Client(Photon2ClientMsg.PeerStatusChanged, new PeerStatusChangedMessage(this.mPeerType, statusCode));
			Log.debug("==> MobaLobbyClient ： OnStatusChanged " + statusCode);
			switch (statusCode)
			{
			case StatusCode.ExceptionOnConnect:
				this.mClient.ExecSendMsg2Client(Photon2ClientMsg.PeerConnected, new PeerConnectedMessage(this.mPeerType, MobaConnectedType.ExceptionOnConnect));
				break;
			case StatusCode.Connect:
				base.EstablishEncryption();
				break;
			case StatusCode.Disconnect:
				if (this.MsgRecver != null)
				{
					this.MsgRecver.OnDisconnect(statusCode);
				}
				base.DisconnectedLogic();
				break;
			default:
				switch (statusCode)
				{
				case StatusCode.TimeoutDisconnect:
					this.mDisconnectedType = MobaDisconnectedType.TimeoutDisconnect;
					if (this.MsgRecver != null)
					{
						this.MsgRecver.OnDisconnect(statusCode);
					}
					goto IL_1C5;
				case StatusCode.DisconnectByServer:
					this.mDisconnectedType = MobaDisconnectedType.DisconnectByServer;
					if (this.MsgRecver != null)
					{
						this.MsgRecver.OnDisconnect(statusCode);
					}
					goto IL_1C5;
				case StatusCode.DisconnectByServerUserLimit:
					this.mDisconnectedType = MobaDisconnectedType.DisconnectByServerUserLimit;
					if (this.MsgRecver != null)
					{
						this.MsgRecver.OnDisconnect(statusCode);
					}
					goto IL_1C5;
				case StatusCode.DisconnectByServerLogic:
					this.mDisconnectedType = MobaDisconnectedType.DisconnectByServerLogic;
					if (this.MsgRecver != null)
					{
						this.MsgRecver.OnDisconnect(statusCode);
					}
					goto IL_1C5;
				case StatusCode.EncryptionEstablished:
					if (this._lastStatusCode == StatusCode.Connect)
					{
						if (this.MsgRecver != null)
						{
							this.MsgRecver.OnConnect();
						}
						base.ConnectedLogic();
					}
					goto IL_1C5;
				}
				this.mClient.OnConnectStatusEvent(statusCode, 0, (int)base.PeerType);
				break;
			}
			IL_1C5:
			this._lastStatusCode = statusCode;
		}

		public void OnEvent(EventData eventData)
		{
			Log.debug("==> MobaClient ： OnEvent " + eventData.Code);
		}

		public void OnOperationResponse(OperationResponse operationResponse)
		{
			if (this.MsgRecver != null)
			{
				this.MsgRecver.OnResponse(operationResponse);
			}
			this.mClient.ExecSendMsg2Client((LobbyCode)operationResponse.OperationCode, operationResponse);
		}
	}
}
