using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using MobaProtocol.Helper;
using System;
using System.Collections.Generic;

namespace MobaClient
{
	public class MobaMasterClientPeer : MobaPeer, IPhotonPeerListener
	{
		private StatusCode _lastStatusCode;

		public MobaMasterClientPeer(ConnectionProtocol protocolType, PhotonClient client) : base(protocolType, client, MobaPeerType.C2Master)
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
		}

		public void OnOperationResponse(OperationResponse operationResponse)
		{
			Log.debug("==> MobaMasterClientPeer : OnOperationResponse " + operationResponse.OperationCode);
			this.mClient.ExecSendMsg2Client((MobaMasterCode)operationResponse.OperationCode, operationResponse);
		}

		private void LoginResponse(OperationResponse operationResponse)
		{
			Log.debug(" MobaClient : 响应登录 " + operationResponse.ReturnCode);
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				this.mClient.OnLoginEvent(num, operationResponse.DebugMessage, null);
			}
			else
			{
				Log.debug(" MobaClient : 登录唯一账号成功!!");
				Dictionary<byte, object> serObj = operationResponse.Parameters[85] as Dictionary<byte, object>;
				AccountData accountData = DataHelper.ToAccountData(serObj);
				if (accountData != null)
				{
					this.mClient.m_user.AccountData = accountData;
					this.mClient.OnLoginEvent(num, operationResponse.DebugMessage, accountData);
				}
				else
				{
					this.mClient.OnLoginEvent(506, "数据获取错误!", null);
				}
			}
		}

		private void RegisterResponse(OperationResponse operationResponse)
		{
			Log.debug(" PhotonClient : OnLoginResponse 处理注册返回 " + operationResponse.ReturnCode);
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				this.mClient.OnRegisterEvent(num, operationResponse.DebugMessage, null);
			}
			else
			{
				Log.debug(" RegisterResponse : 注册成功! 获取数据...");
				AccountData accountData = SerializeHelper.Deserialize<AccountData>(operationResponse.Parameters[85] as byte[]);
				if (accountData != null)
				{
					this.mClient.m_user.AccountData = accountData;
					Log.debug(" PhotonClient : 注册成功! accountid = " + this.mClient.m_user.AccountData.AccountId);
					this.mClient.OnRegisterEvent(num, operationResponse.DebugMessage, accountData);
				}
				else
				{
					this.mClient.OnRegisterEvent(506, "数据获取错误!", null);
				}
			}
		}

		private void LoginByChannelIdResponse(OperationResponse operationResponse)
		{
			Log.debug(" PhotonClient : LoginByChannelIdResponse 处理渠道ID登录返回 " + operationResponse.ReturnCode);
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				this.mClient.OnLoginByChannelIdEvent(num, operationResponse.DebugMessage, null);
			}
			else
			{
				Log.debug(" LoginByChannelIdResponse : 登录成功! 获取数据...");
				AccountData accountData = SerializeHelper.Deserialize<AccountData>(operationResponse.Parameters[85] as byte[]);
				if (accountData != null)
				{
					this.mClient.m_user.AccountData = accountData;
					Log.debug(" PhotonClient : 登录成功! accountid = " + this.mClient.m_user.AccountData.AccountId);
					this.mClient.OnLoginByChannelIdEvent(num, operationResponse.DebugMessage, accountData);
				}
				else
				{
					this.mClient.OnLoginByChannelIdEvent(506, "数据获取错误!", null);
				}
			}
		}

		private void UpgradeUrlResponse(OperationResponse operationResponse)
		{
			Log.debug(" MobaClient : 响应更新 " + operationResponse.ReturnCode);
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				this.mClient.OnUpgradeEvent(false, num, operationResponse.DebugMessage, null);
			}
			else
			{
				Log.debug(" PhotonClient : 获取更新成功!");
				string appUpgradeUrl = operationResponse.Parameters[32] as string;
				byte[] buffer = operationResponse.Parameters[84] as byte[];
				ClientData clientData = SerializeHelper.Deserialize<ClientData>(buffer);
				clientData.AppUpgradeUrl = appUpgradeUrl;
				this.mClient.clientData = new ClientData();
				this.mClient.clientData = clientData;
				this.mClient.OnUpgradeEvent(true, num, operationResponse.DebugMessage, clientData);
			}
		}

		private void GetAllGameServersResponse(OperationResponse operationResponse)
		{
			Log.debug(" MobaClient : 获取在线人数 " + operationResponse.ReturnCode);
			short returnCode = operationResponse.ReturnCode;
			if (returnCode != 0)
			{
				this.mClient.OnErrorEvent((int)operationResponse.OperationCode, (int)operationResponse.ReturnCode, operationResponse.DebugMessage);
			}
			else
			{
				Dictionary<byte, object> listObj = operationResponse.Parameters[49] as Dictionary<byte, object>;
				List<ServerInfo> list = DataHelper.ToServerList(listObj);
				if (list != null)
				{
					this.mClient.m_serverlist = list;
					this.mClient.OnGetServerListEvent((int)operationResponse.ReturnCode, operationResponse.DebugMessage, list);
				}
				else
				{
					this.mClient.OnGetServerListEvent(506, "数据获取错误!", list);
				}
			}
		}
	}
}
