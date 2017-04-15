using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;

namespace MobaClient
{
	public class MobaPeer : PhotonPeer
	{
		protected const int TIMEOUT_MSEC = 10000;

		protected PhotonClient mClient;

		protected string mDebugMessage;

		protected MobaPeerType mPeerType;

		protected int mConnectedTimes;

		protected bool mIsReconnect;

		protected MobaDisconnectedType mDisconnectedType;

		protected Dictionary<byte, INetEventHandleBase> mEventHandles;

		public bool ServerConnected
		{
			get;
			set;
		}

		public string ServerName
		{
			get;
			set;
		}

		public string ApplicationName
		{
			get;
			set;
		}

		public MobaPeerType PeerType
		{
			get
			{
				return this.mPeerType;
			}
		}

		public int ConnectedTime
		{
			get
			{
				return this.mConnectedTimes;
			}
			set
			{
				this.mConnectedTimes = value;
			}
		}

		public byte NodeId
		{
			get;
			set;
		}

		public MobaPeer(ConnectionProtocol protocolType, PhotonClient client, MobaPeerType peerType) : base(protocolType)
		{
			this.mClient = client;
			this.mPeerType = peerType;
			this.mEventHandles = new Dictionary<byte, INetEventHandleBase>();
			this.mConnectedTimes = 0;
			this.mIsReconnect = false;
			this.ServerConnected = false;
		}

		public virtual bool PeerConnect()
		{
			bool flag = this.Connect(this.ServerName, this.ApplicationName);
			Log.debug(string.Concat(new object[]
			{
				"==> [",
				this.PeerType,
				"] connect ",
				flag,
				",ServerName = ",
				this.ServerName,
				",ApplicationName = ",
				this.ApplicationName
			}));
			return flag;
		}

		public virtual void PeerDisconnect()
		{
			this.Disconnect();
			Log.debug("==> [" + this.PeerType + "] disconnect !!");
		}

		public virtual void PeerReconnect()
		{
			this.mIsReconnect = true;
			this.PeerConnect();
		}

		public virtual void PeerUpdate()
		{
			this.Service();
			this.mClient.PostService();
		}

		public void RegistNetEventChannel(byte channel, INetEventHandleBase eventHandle)
		{
			if (this.mEventHandles.ContainsKey(channel))
			{
				this.mEventHandles.Remove(channel);
			}
			this.mEventHandles[channel] = eventHandle;
		}

		protected bool ExecResponse(byte channel, OperationResponse operationResponse)
		{
			INetEventHandleBase netEventHandleBase = null;
			bool result;
			if (this.mEventHandles.TryGetValue(channel, out netEventHandleBase))
			{
				netEventHandleBase.OnResponse(operationResponse);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		protected void ConnectedLogic()
		{
			MobaConnectedType cType;
			if (this.mConnectedTimes == 0)
			{
				cType = MobaConnectedType.Original;
			}
			else if (this.mIsReconnect)
			{
				cType = MobaConnectedType.Reconnect;
			}
			else
			{
				cType = MobaConnectedType.Multi;
			}
			this.mClient.ExecSendMsg2Client(Photon2ClientMsg.PeerConnected, new PeerConnectedMessage(this.mPeerType, cType));
			this.ServerConnected = true;
			this.mConnectedTimes++;
			this.mIsReconnect = false;
			this.mClient.OnMainPeerConnected(this.mPeerType);
		}

		protected void DisconnectedLogic()
		{
			this.ServerConnected = false;
			this.mClient.ExecSendMsg2Client(Photon2ClientMsg.PeerDisconnected, new PeerDisconnectedMessage(this.mPeerType, this.mDisconnectedType));
			this.mDisconnectedType = MobaDisconnectedType.Normal;
			this.mClient.OnDisconnected();
		}
	}
}
