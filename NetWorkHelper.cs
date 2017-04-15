using Assets.Scripts.Server;
using Com.Game.Utils;
using MobaClient;
using MobaHeros.Gate;
using MobaHeros.Server;
using MobaProtocol;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class NetWorkHelper : IGlobalComServer
{
	protected enum ServerType
	{
		none,
		master,
		game,
		lobby,
		pvp,
		Gate,
		Login
	}

	private static NetWorkHelper m_instance;

	public PhotonClient client;

	private Dictionary<NetWorkHelper.ServerType, ServerHelpCom> dicServer;

	private List<ServerHelpCom> listServer;

	private float mPhysicsCheckElapsedTime;

	private float mPhysicsCheckThresholdTime = 1f;

	private NetworkReachability mPhysicsStatus;

	private readonly ThreadStorage _storage = new ThreadStorage();

	private readonly List<MsgData> _msgDatas = new List<MsgData>();

	private GateReconnection _gateReconnection;

	public static NetWorkHelper Instance
	{
		get
		{
			if (NetWorkHelper.m_instance == null)
			{
				ClientLogger.Error("MasterServer script is missing");
			}
			return NetWorkHelper.m_instance;
		}
	}

	public GateReconnection GateReconnection
	{
		get
		{
			return this._gateReconnection;
		}
	}

	public bool MasterServerFlag
	{
		get
		{
			return this.GetServerFlag(NetWorkHelper.ServerType.master);
		}
	}

	public bool PvpServerFlag
	{
		get
		{
			return this.GetServerFlag(NetWorkHelper.ServerType.pvp);
		}
	}

	public bool IsGateAvailable
	{
		get
		{
			return this._gateReconnection.Available;
		}
	}

	public bool IsGateConnected
	{
		get
		{
			return this.GetServerFlag(NetWorkHelper.ServerType.Gate);
		}
	}

	public void OnAwake()
	{
		NetWorkHelper.m_instance = this;
		this.InitClient();
		for (int i = 0; i < this.listServer.Count; i++)
		{
			this.listServer[i].OnAwake();
		}
		this._gateReconnection.OnAwake();
	}

	public void OnStart()
	{
		for (int i = 0; i < this.listServer.Count; i++)
		{
			this.listServer[i].OnStart();
		}
		this._gateReconnection.OnStart();
	}

	public void Enable(bool b)
	{
		if (!b)
		{
			for (int i = 0; i < this.listServer.Count; i++)
			{
				this.listServer[i].End();
			}
			this._gateReconnection.Enabled = false;
		}
	}

	public void OnRestart()
	{
		GateReconnection.CanTrigger = false;
		this.Enable(false);
	}

	public void OnApplicationQuit()
	{
		if (this.IsGateAvailable)
		{
		}
	}

	public void OnApplicationFocus(bool isFocus)
	{
		for (int i = 0; i < this.listServer.Count; i++)
		{
			this.listServer[i].OnApplicationFocus(isFocus);
		}
		this._gateReconnection.OnApplicationFocus(isFocus);
	}

	public void OnApplicationPause(bool isPause)
	{
		for (int i = 0; i < this.listServer.Count; i++)
		{
			this.listServer[i].OnApplicationPause(isPause);
		}
		this._gateReconnection.OnApplicationPause(isPause);
	}

	private void InitClient()
	{
		ThreadUtils.Init();
		this._gateReconnection = new GateReconnection();
		GlobalManager.ClientType = 2;
		this.client = new PhotonClient();
		this.dicServer = new Dictionary<NetWorkHelper.ServerType, ServerHelpCom>
		{
			{
				NetWorkHelper.ServerType.master,
				new MasterServerNew(this.client)
			},
			{
				NetWorkHelper.ServerType.lobby,
				new LobbyServerNew(this.client)
			},
			{
				NetWorkHelper.ServerType.pvp,
				new PvpServer(this.client)
			},
			{
				NetWorkHelper.ServerType.Gate,
				new GateServer(this.client)
			}
		};
		this.listServer = new List<ServerHelpCom>(this.dicServer.Values.Count);
		foreach (ServerHelpCom current in this.dicServer.Values)
		{
			this.listServer.Add(current);
		}
		this.client.RegistSendMsg2ClientCustom(new PhotonClient.DeleSendMsg2ClientCustom(this.DelePhotonMsgCustom));
		this.client.RegistSendMsg2ClientMasterCode(new PhotonClient.DeleSendMsg2ClientMasterCode(this.DelePhotonMsgMasterCode));
		this.client.RegistSendMsg2ClientGameCode(new PhotonClient.DeleSendMsg2ClientGameCode(this.DelePhotonMsgGameCode));
		this.client.RegistSendMsg2ClientPvpCode(new PhotonClient.DeleSendMsg2ClientPvpCode(this.DelePhotonMsgPvpCode));
		this.client.RegistSendMsg2ClientChatCode(new PhotonClient.DeleSendMsg2ClientChatCode(this.DelePhotonMsgChatCode));
		this.client.RegistSendMsg2ClientFriendCode(new PhotonClient.DeleSendMsg2ClientFriendCode(this.DelePhotonMsgFriendCode));
		this.client.RegistSendMsg2ClientTeamRoomCode(new PhotonClient.DeleSendMsg2ClientTeamRoomCode(this.DelePhotonMsgTeamRoomCode));
		this.client.RegistSendMsg2ClientGateCode(new PhotonClient.DeleSendMsg2ClientGateCode(this.DelePhotonMsgGateCode));
		this.client.RegistSendMsg2ClientLobbyCode(new PhotonClient.DeleSendMsg2ClientLobbyCode(this.DelePhotonMsgLobbyCode));
		this.client.RegistSendMsg2ClientUserDataCode(new PhotonClient.DeleSendMsg2ClientUserDataCode(this.DelePhotonMsgUserDataCode));
		MobaMessageManager.RegistMessage(Photon2ClientMsg.PeerConnected, new MobaMessageFunc(this.OnPeerConnected));
		MobaMessageManager.RegistMessage(Photon2ClientMsg.PeerDisconnected, new MobaMessageFunc(this.OnPeerDisconnected));
		MobaMessageManager.RegistMessage(Photon2ClientMsg.PeerStatusChanged, new MobaMessageFunc(this.OnPeerStatusChanged));
	}

	public void OnDestroy()
	{
		for (int i = 0; i < this.listServer.Count; i++)
		{
			this.listServer[i].OnDestroy();
		}
		this._gateReconnection.OnDestroy();
		this.client.UnRegistSendMsg2ClientCustom(new PhotonClient.DeleSendMsg2ClientCustom(this.DelePhotonMsgCustom));
		this.client.UnRegistSendMsg2ClientMasterCode(new PhotonClient.DeleSendMsg2ClientMasterCode(this.DelePhotonMsgMasterCode));
		this.client.UnRegistSendMsg2ClientGameCode(new PhotonClient.DeleSendMsg2ClientGameCode(this.DelePhotonMsgGameCode));
		this.client.UnRegistSendMsg2ClientPvpCode(new PhotonClient.DeleSendMsg2ClientPvpCode(this.DelePhotonMsgPvpCode));
		this.client.UnRegistSendMsg2ClientChatCode(new PhotonClient.DeleSendMsg2ClientChatCode(this.DelePhotonMsgChatCode));
		this.client.UnRegistSendMsg2ClientFriendCode(new PhotonClient.DeleSendMsg2ClientFriendCode(this.DelePhotonMsgFriendCode));
		this.client.UnRegistSendMsg2ClientTeamRoomCode(new PhotonClient.DeleSendMsg2ClientTeamRoomCode(this.DelePhotonMsgTeamRoomCode));
		this.client.UnRegistSendMsg2ClientGateCode(new PhotonClient.DeleSendMsg2ClientGateCode(this.DelePhotonMsgGateCode));
		this.client.UnRegistSendMsg2ClientLobbyCode(new PhotonClient.DeleSendMsg2ClientLobbyCode(this.DelePhotonMsgLobbyCode));
		this.client.UnRegistSendMsg2ClientUserDataCode(new PhotonClient.DeleSendMsg2ClientUserDataCode(this.DelePhotonMsgUserDataCode));
		MobaMessageManager.UnRegistMessage(Photon2ClientMsg.PeerConnected, new MobaMessageFunc(this.OnPeerConnected));
		MobaMessageManager.UnRegistMessage(Photon2ClientMsg.PeerDisconnected, new MobaMessageFunc(this.OnPeerDisconnected));
		MobaMessageManager.UnRegistMessage(Photon2ClientMsg.PeerStatusChanged, new MobaMessageFunc(this.OnPeerStatusChanged));
	}

	private void ProcessQueuedMsgs()
	{
		this._msgDatas.Clear();
		this._storage.GetAllMsgs(this._msgDatas);
		for (int i = 0; i < this._msgDatas.Count; i++)
		{
			MsgData msgData = this._msgDatas[i];
			if (msgData.msgType == MsgType.PvpMsg)
			{
				this.ProcessPvpMsg((PvpCode)msgData.msgID, msgData.msgParam, msgData.protoObj, msgData.svrTime);
			}
			else if (msgData.msgType == MsgType.NotifyMsg)
			{
				MobaMessage message = MobaMessageManager.GetMessage((Photon2ClientMsg)msgData.msgID, msgData.msgParam, 0f);
				MobaMessageManager.ExecuteMsg(message);
			}
		}
	}

	public void OnUpdate()
	{
		for (int i = 0; i < this.listServer.Count; i++)
		{
			this.listServer[i].OnUpdate();
		}
		this._gateReconnection.OnUpdate();
		this.mPhysicsCheckElapsedTime += Time.deltaTime;
		if (this.mPhysicsCheckElapsedTime > this.mPhysicsCheckThresholdTime)
		{
			this.mPhysicsStatus = Application.internetReachability;
			this.mPhysicsCheckElapsedTime = 0f;
		}
		if (!PvpServer.IsMultithreadEnabled)
		{
			if (this.client != null)
			{
				this.client.OnUpdate();
			}
		}
		else
		{
			this.ProcessQueuedMsgs();
		}
	}

	public bool IsWifi()
	{
		return this.mPhysicsStatus == NetworkReachability.ReachableViaLocalAreaNetwork;
	}

	public bool IsAvailable()
	{
		return this.mPhysicsStatus != NetworkReachability.NotReachable;
	}

	private bool GetServerFlag(NetWorkHelper.ServerType sType)
	{
		return this.dicServer.ContainsKey(sType) && this.dicServer[sType].ConnectFlag;
	}

	private void LogPeer(MobaPeer peer)
	{
	}

	public void ConnectToMasterServer()
	{
		this.ConnectToServer(NetWorkHelper.ServerType.master);
		this.LogPeer(this.client.master_peer);
	}

	public void DisconnectFromMasterServer()
	{
		this.DisconnectFromServer(NetWorkHelper.ServerType.master);
	}

	public void ConnectToGateServer()
	{
		this._gateReconnection.Enabled = true;
		this.ConnectToServer(NetWorkHelper.ServerType.Gate);
		this.LogPeer(this.client.gate_peer);
	}

	public void DisconnectFromGateServer(bool reconnect = false)
	{
		if (!reconnect)
		{
			this._gateReconnection.Enabled = false;
		}
		this.DisconnectFromServer(NetWorkHelper.ServerType.Gate);
	}

	public void ConnectToLobbyServer()
	{
		this.ConnectToServer(NetWorkHelper.ServerType.lobby);
		this.LogPeer(this.client.lobby_peer);
	}

	public void DisconnectLobbyServer()
	{
		this.DisconnectFromServer(NetWorkHelper.ServerType.lobby);
	}

	public void ConnectToPvpServer()
	{
		this.ConnectToServer(NetWorkHelper.ServerType.pvp);
		this.LogPeer(this.client.pvpserver_peer);
	}

	public void DisconnectPvpServer()
	{
		this.DisconnectFromServer(NetWorkHelper.ServerType.pvp);
	}

	protected void ConnectToServer(NetWorkHelper.ServerType type)
	{
		if (this.dicServer.ContainsKey(type))
		{
			this.dicServer[type].Begin();
		}
	}

	protected void DisconnectFromServer(NetWorkHelper.ServerType type)
	{
		if (this.dicServer.ContainsKey(type))
		{
			this.dicServer[type].End();
		}
	}

	private void DelePhotonMsgCustom(Photon2ClientMsg msgID, object msgParam)
	{
		if (PvpServer.IsMultithreadEnabled)
		{
			this._storage.AddMsg((int)msgID, msgParam, MsgType.NotifyMsg, 0L);
		}
		else
		{
			MobaMessage message = MobaMessageManager.GetMessage(msgID, msgParam, 0f);
			MobaMessageManager.ExecuteMsg(message);
		}
	}

	private void DelePhotonMsgMasterCode(MobaMasterCode msgID, object msgParam)
	{
		MobaMessage message = MobaMessageManager.GetMessage(msgID, msgParam, 0f);
		if (MobaMessageManager.IsHandlerExists(message))
		{
			MobaMessageManager.ExecuteMsg(message);
		}
	}

	private void DelePhotonMsgGameCode(MobaGameCode msgID, object msgParam)
	{
		MobaMessage message = MobaMessageManager.GetMessage(msgID, msgParam, 0f);
		if (MobaMessageManager.IsHandlerExists(message))
		{
			MobaMessageManager.ExecuteMsg(message);
		}
	}

	private void DelePhotonMsgGateCode(MobaGateCode msgID, object msgParam)
	{
		MobaMessage message = MobaMessageManager.GetMessage(msgID, msgParam, 0f);
		if (MobaMessageManager.IsHandlerExists(message))
		{
			MobaMessageManager.ExecuteMsg(message);
		}
		else
		{
			ClientLogger.Error("No handler for MobaGateCode: " + msgID);
		}
	}

	private void DelePhotonMsgLobbyCode(LobbyCode msgID, object msgParam)
	{
		MobaMessage message = MobaMessageManager.GetMessage(msgID, msgParam, 0f);
		if (MobaMessageManager.IsHandlerExists(message))
		{
			MobaMessageManager.ExecuteMsg(message);
		}
		else
		{
			ClientLogger.Error("No handler for LobbyCode: " + msgID);
		}
	}

	private void DelePhotonMsgUserDataCode(MobaUserDataCode msgID, object msgParam)
	{
		MobaMessage message = MobaMessageManager.GetMessage(msgID, msgParam, 0f);
		if (MobaMessageManager.IsHandlerExists(message))
		{
			MobaMessageManager.ExecuteMsg(message);
		}
		else
		{
			ClientLogger.Error("No handler for MobaUserDataCode: " + msgID);
		}
	}

	private void DelePhotonMsgPvpCode(PvpCode msgID, object msgParam)
	{
		if (PvpServer.IsMultithreadEnabled)
		{
			this._storage.AddMsg((int)msgID, msgParam, MsgType.PvpMsg, 0L);
		}
		else
		{
			this.ProcessPvpMsg(msgID, msgParam, null, 0L);
		}
	}

	private void ProcessPvpMsg(PvpCode msgID, object msgParam, object protoObj, long svrTime)
	{
		if (GameManager.Instance.ReplayController.HandlePvpMsg(msgID, msgParam))
		{
			return;
		}
		MobaMessage message = MobaMessageManager.GetMessage(msgID, msgParam, 0f, protoObj);
		message.svrTime = svrTime;
		if (MobaMessageManager.IsHandlerExists(message))
		{
			if (message.ID != 143)
			{
				FrameSyncManager.Instance.ReceiveMsg(message);
			}
			else
			{
				MobaMessageManager.ExecuteMsg(message);
			}
		}
	}

	private void DelePhotonMsgChatCode(MobaChatCode msgID, object msgParam)
	{
		MobaMessage message = MobaMessageManager.GetMessage(msgID, msgParam, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	private void DelePhotonMsgFriendCode(MobaFriendCode msgID, object msgParam)
	{
		MobaMessage message = MobaMessageManager.GetMessage(msgID, msgParam, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	private void DelePhotonMsgTeamRoomCode(MobaTeamRoomCode msgID, object msgParam)
	{
		MobaMessage message = MobaMessageManager.GetMessage(msgID, msgParam, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	private void OnPeerConnected(MobaMessage msg)
	{
		PeerConnectedMessage peerConnectedMessage = msg.Param as PeerConnectedMessage;
		if (peerConnectedMessage == null)
		{
			return;
		}
		NetWorkHelper.ServerType serverType = NetWorkHelper.ServerType.none;
		switch (peerConnectedMessage.PeerType)
		{
		case MobaPeerType.C2Master:
			serverType = NetWorkHelper.ServerType.master;
			break;
		case MobaPeerType.C2Lobby:
			serverType = NetWorkHelper.ServerType.lobby;
			break;
		case MobaPeerType.C2PvpServer:
			serverType = NetWorkHelper.ServerType.pvp;
			break;
		case MobaPeerType.C2GateServer:
			serverType = NetWorkHelper.ServerType.Gate;
			break;
		}
		if (this.dicServer.ContainsKey(serverType))
		{
			this.dicServer[serverType].OnConnected(peerConnectedMessage.ConnectedType);
		}
		if (serverType != NetWorkHelper.ServerType.pvp)
		{
			MobaMessageManager.ExecuteMsg(MobaMessageManager.GetMessage((ClientMsg)20001, peerConnectedMessage, 0f));
		}
	}

	private void OnPeerStatusChanged(MobaMessage msg)
	{
		PeerStatusChangedMessage peerStatusChangedMessage = msg.Param as PeerStatusChangedMessage;
		if (peerStatusChangedMessage == null)
		{
			return;
		}
		NetWorkHelper.ServerType key = NetWorkHelper.ServerType.none;
		switch (peerStatusChangedMessage.PeerType)
		{
		case MobaPeerType.C2Master:
			key = NetWorkHelper.ServerType.master;
			break;
		case MobaPeerType.C2Lobby:
			key = NetWorkHelper.ServerType.lobby;
			break;
		case MobaPeerType.C2PvpServer:
			key = NetWorkHelper.ServerType.pvp;
			break;
		case MobaPeerType.C2GateServer:
			key = NetWorkHelper.ServerType.lobby;
			break;
		}
		if (this.dicServer.ContainsKey(key))
		{
			this.dicServer[key].OnStatusChanged(peerStatusChangedMessage.StatCode);
		}
	}

	private void OnPeerDisconnected(MobaMessage msg)
	{
		PeerDisconnectedMessage peerDisconnectedMessage = msg.Param as PeerDisconnectedMessage;
		if (peerDisconnectedMessage == null)
		{
			return;
		}
		NetWorkHelper.ServerType serverType = NetWorkHelper.ServerType.none;
		switch (peerDisconnectedMessage.PeerType)
		{
		case MobaPeerType.C2Master:
			serverType = NetWorkHelper.ServerType.master;
			NetWorkHelper.DebugInfo("MasterServer:networkHelper 检测到断线");
			break;
		case MobaPeerType.C2Lobby:
			serverType = NetWorkHelper.ServerType.lobby;
			break;
		case MobaPeerType.C2PvpServer:
			serverType = NetWorkHelper.ServerType.pvp;
			break;
		case MobaPeerType.C2GateServer:
			serverType = NetWorkHelper.ServerType.Gate;
			break;
		}
		if (this.dicServer.ContainsKey(serverType))
		{
			this.dicServer[serverType].OnDisconnected(peerDisconnectedMessage.DisconnectedType);
		}
		if (serverType != NetWorkHelper.ServerType.pvp)
		{
			MobaMessageManager.ExecuteMsg(MobaMessageManager.GetMessage((ClientMsg)20002, peerDisconnectedMessage, 0f));
		}
	}

	[Obsolete("反射产生临时变量，频繁调用，可能会导致GC")]
	private void InvokeEveryCom(string methodName, object[] param = null)
	{
		if (!string.IsNullOrEmpty(methodName))
		{
			foreach (NetWorkHelper.ServerType current in this.dicServer.Keys)
			{
				ServerHelpCom serverHelpCom = this.dicServer[current];
				Type type = serverHelpCom.GetType();
				MethodInfo method = type.GetMethod(methodName);
				if (method != null)
				{
					method.Invoke(serverHelpCom, param);
				}
			}
		}
	}

	public static void DebugInfo(string s)
	{
	}
}
