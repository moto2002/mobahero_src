using Assets.Scripts.Model;
using MobaClient;
using MobaHeros.Gate;
using System;
/// <summary>
/// 网关服务器类
/// </summary>
public class GateServer : ServerHelpCom
{
	private const string LockConnect = "ConnectGateServer";

	private readonly PhotonClient _client;

	private bool _connect;

	private bool _enable;

	public override bool ConnectFlag
	{
		get
		{
			return this._connect;
		}
		protected set
		{
			this._connect = value;
		}
	}

	public GateServer(PhotonClient client)
	{
		this._client = client;
	}

	public override void OnAwake()
	{
		this._enable = false;
	}

	public override void OnStart()
	{
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (this._enable)
		{
			MobaPeer mobaPeer = this._client.GetMobaPeer(MobaPeerType.C2GateServer);
			mobaPeer.PeerUpdate();
		}
	}

	public override void OnDestroy()
	{
		this._client.IsReconnect = false;
		this.End();
	}

	public override void Begin()
	{
		if (!this._enable)
		{
			this._enable = true;
			if (this._client.gate_peer != null)
			{
				this._client.gate_peer.TrafficStatsReset();
				this._client.gate_peer.Service();
			}
			this.Connect();
		}
	}

	public override void End()
	{
		if (this._enable)
		{
			this._enable = false;
			if (this._client.gate_peer != null)
			{
				this._client.gate_peer.PeerDisconnect();
				this._client.gate_peer.StopThread();
			}
		}
	}

	public override void OnConnected(MobaConnectedType cType)
	{
		if (cType != MobaConnectedType.ExceptionOnConnect)
		{
			this.ConnectFlag = true;
			if (cType == MobaConnectedType.Original)
			{
				this._client.IsReconnect = true;
			}
			else if (cType != MobaConnectedType.Multi || NetWorkHelper.Instance.client.IsReconnect)
			{
			}
			MobaMessageManager.ExecuteMsg(MobaMessageManager.GetMessage((ClientMsg)20009, cType, 0f));
		}
	}

	public override void OnDisconnected(MobaDisconnectedType dType)
	{
		this.ConnectFlag = false;
		MobaMessageManager.ExecuteMsg(MobaMessageManager.GetMessage((ClientMsg)20010, dType, 0f));
		if (this._enable && GateReconnection.CanTrigger && NetWorkHelper.Instance.GateReconnection.State == GateReconnection.ConnectState.End)
		{
			NetWorkHelper.Instance.GateReconnection.Begin();
		}
	}

	private void Connect()
	{
		SendMsgManager.Instance.ClientConnectToGate();
	}
}
