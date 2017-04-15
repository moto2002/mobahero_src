using ExitGames.Client.Photon;
using MobaProtocol;
using System;

namespace MobaClient
{
	public class MobaPvpServerClientPeer : MobaPeer, IPhotonPeerListener
	{
		public static bool WillTriggerReceiver = true;

		private StatusCode _lastStatusCode;

		public ClientMsgRecv MsgRecver
		{
			get;
			set;
		}

		public MobaPvpServerClientPeer(ConnectionProtocol protocolType, PhotonClient client) : base(protocolType, client, MobaPeerType.C2PvpServer)
		{
			this.mClient = client;
			base.Listener = this;
		}

		public void DebugReturn(DebugLevel level, string message)
		{
			this.mDebugMessage = message;
			Log.debug(string.Concat(new object[]
			{
				"==> MobaPVPClientPeer : DebugReturn level ",
				level,
				" message ",
				message
			}));
		}

		public void OnStatusChanged(StatusCode statusCode)
		{
			this.mClient.ExecSendMsg2Client(Photon2ClientMsg.PeerStatusChanged, new PeerStatusChangedMessage(this.mPeerType, statusCode));
			Log.debug("==> MobaPvpServerClientPeer ： OnStatusChanged " + statusCode);
			switch (statusCode)
			{
			case StatusCode.ExceptionOnConnect:
				this.mClient.ExecSendMsg2Client(Photon2ClientMsg.PeerConnected, new PeerConnectedMessage(this.mPeerType, MobaConnectedType.ExceptionOnConnect));
				this.mIsReconnect = false;
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
					goto IL_1CC;
				case StatusCode.DisconnectByServer:
					this.mDisconnectedType = MobaDisconnectedType.DisconnectByServer;
					if (this.MsgRecver != null)
					{
						this.MsgRecver.OnDisconnect(statusCode);
					}
					goto IL_1CC;
				case StatusCode.DisconnectByServerUserLimit:
					this.mDisconnectedType = MobaDisconnectedType.DisconnectByServerUserLimit;
					if (this.MsgRecver != null)
					{
						this.MsgRecver.OnDisconnect(statusCode);
					}
					goto IL_1CC;
				case StatusCode.DisconnectByServerLogic:
					this.mDisconnectedType = MobaDisconnectedType.DisconnectByServerLogic;
					if (this.MsgRecver != null)
					{
						this.MsgRecver.OnDisconnect(statusCode);
					}
					goto IL_1CC;
				case StatusCode.EncryptionEstablished:
					if (this._lastStatusCode == StatusCode.Connect)
					{
						if (this.MsgRecver != null)
						{
							if (this.mIsReconnect)
							{
								this.MsgRecver.OnReconnect();
							}
							else
							{
								this.MsgRecver.OnConnect();
							}
						}
						base.ConnectedLogic();
					}
					goto IL_1CC;
				}
				if (this.MsgRecver != null)
				{
					this.MsgRecver.OnDisconnect(statusCode);
				}
				break;
			}
			IL_1CC:
			this._lastStatusCode = statusCode;
		}

		public void OnEvent(EventData eventData)
		{
			Log.debug("==> MobaClient ： OnEvent " + eventData.Code);
			if (this.MsgRecver != null && MobaPvpServerClientPeer.WillTriggerReceiver)
			{
				this.MsgRecver.OnEvent(eventData);
			}
		}

		public void OnOperationResponse(OperationResponse operationResponse)
		{
			this.mClient.ExecSendMsg2Client((PvpCode)operationResponse.OperationCode, operationResponse);
		}
	}
}
