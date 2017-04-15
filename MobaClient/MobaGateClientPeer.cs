using ExitGames.Client.Photon;
using MobaProtocol;
using System;

namespace MobaClient
{
	public class MobaGateClientPeer : MobaPeer, IPhotonPeerListener
	{
		private StatusCode _lastStatusCode;

		public MobaGateClientPeer(ConnectionProtocol protocolType, PhotonClient client) : base(protocolType, client, MobaPeerType.C2GateServer)
		{
			this.mClient = client;
			base.Listener = this;
		}

		public void DebugReturn(DebugLevel level, string message)
		{
			this.mDebugMessage = message;
			Log.debug(string.Concat(new object[]
			{
				"==> MobaMasterClientPeer : DebugReturn level ",
				level,
				" message ",
				message
			}));
		}

		public void OnStatusChanged(StatusCode statusCode)
		{
			this.mClient.ExecSendMsg2Client(Photon2ClientMsg.PeerStatusChanged, new PeerStatusChangedMessage(this.mPeerType, statusCode));
			Log.debug("==> MobaMasterClient ： OnStatusChanged " + statusCode);
			switch (statusCode)
			{
			case StatusCode.ExceptionOnConnect:
				this.mClient.ExecSendMsg2Client(Photon2ClientMsg.PeerConnected, new PeerConnectedMessage(this.mPeerType, MobaConnectedType.ExceptionOnConnect));
				break;
			case StatusCode.Connect:
				base.EstablishEncryption();
				break;
			case StatusCode.Disconnect:
				base.DisconnectedLogic();
				break;
			default:
				switch (statusCode)
				{
				case StatusCode.TimeoutDisconnect:
					this.mDisconnectedType = MobaDisconnectedType.TimeoutDisconnect;
					break;
				case StatusCode.DisconnectByServer:
					this.mDisconnectedType = MobaDisconnectedType.DisconnectByServer;
					break;
				case StatusCode.DisconnectByServerUserLimit:
					this.mDisconnectedType = MobaDisconnectedType.DisconnectByServerUserLimit;
					break;
				case StatusCode.DisconnectByServerLogic:
					this.mDisconnectedType = MobaDisconnectedType.DisconnectByServerLogic;
					break;
				case StatusCode.EncryptionEstablished:
					if (this._lastStatusCode == StatusCode.Connect)
					{
						base.ConnectedLogic();
					}
					break;
				}
				break;
			}
			this._lastStatusCode = statusCode;
		}

		public void OnEvent(EventData eventData)
		{
			Log.debug("==> MobaClient ： OnEvent " + eventData.Code);
			this.OnOperationResponse(new OperationResponse
			{
				OperationCode = eventData.Code,
				Parameters = eventData.Parameters
			});
		}

		public void OnOperationResponse(OperationResponse operationResponse)
		{
			Log.debug("==> MobaGateClientPeer : OnOperationResponse " + operationResponse.OperationCode);
			object obj = 0;
			MobaChannel mobaChannel = MobaChannel.Default;
			if (operationResponse.Parameters.TryGetValue(255, out obj))
			{
				mobaChannel = (MobaChannel)obj;
			}
			MobaChannel mobaChannel2 = mobaChannel;
			switch (mobaChannel2)
			{
			case MobaChannel.Game:
				this.mClient.ExecSendMsg2Client((MobaGameCode)operationResponse.OperationCode, operationResponse);
				break;
			case MobaChannel.Lobby:
				this.mClient.ExecSendMsg2Client((LobbyCode)operationResponse.OperationCode, operationResponse);
				break;
			default:
				switch (mobaChannel2)
				{
				case MobaChannel.Friend:
					this.mClient.ExecSendMsg2Client((MobaFriendCode)operationResponse.OperationCode, operationResponse);
					return;
				case MobaChannel.Chat:
					this.mClient.ExecSendMsg2Client((MobaChatCode)operationResponse.OperationCode, operationResponse);
					return;
				case MobaChannel.Team:
					this.mClient.ExecSendMsg2Client((MobaTeamRoomCode)operationResponse.OperationCode, operationResponse);
					return;
				case MobaChannel.UserData:
					this.mClient.ExecSendMsg2Client((MobaUserDataCode)operationResponse.OperationCode, operationResponse);
					return;
				}
				this.mClient.ExecSendMsg2Client((MobaGateCode)operationResponse.OperationCode, operationResponse);
				break;
			}
		}
	}
}
