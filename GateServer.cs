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
    /// <summary>
    /// 通信连接客户端
    /// </summary>
	private readonly PhotonClient _client;

	private bool _connect;
    /// <summary>
    /// 是否启用的标记
    /// </summary>
	private bool _enable;
    /// <summary>
    /// 是否连接，判断连接状态的标记
    /// </summary>
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
    /// <summary>
    /// 更新方法接口
    /// </summary>
	public override void OnUpdate()
	{
		base.OnUpdate();
		if (this._enable)
		{
			MobaPeer mobaPeer = this._client.GetMobaPeer(MobaPeerType.C2GateServer);
			mobaPeer.PeerUpdate();
		}
	}
    /// <summary>
    /// 断开连接，结束通信
    /// </summary>
	public override void OnDestroy()
	{
		this._client.IsReconnect = false;
		this.End();
	}
    /// <summary>
    /// 如果网关没有启用，启用，并进行连接
    /// </summary>
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
    /// <summary>
    /// 结束网关连接，断开连接并结束相关逻辑处理
    /// </summary>
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
    /// <summary>
    /// 服务器连接成功回调处理
    /// </summary>
    /// <param name="cType">连接类型</param>
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
    /// <summary>
    /// 服务器连接断开回调处理
    /// </summary>
    /// <param name="dType"></param>
	public override void OnDisconnected(MobaDisconnectedType dType)
	{
		this.ConnectFlag = false;
		MobaMessageManager.ExecuteMsg(MobaMessageManager.GetMessage((ClientMsg)20010, dType, 0f));
		if (this._enable && GateReconnection.CanTrigger && NetWorkHelper.Instance.GateReconnection.State == GateReconnection.ConnectState.End)
		{
			NetWorkHelper.Instance.GateReconnection.Begin();
		}
	}
    /// <summary>
    /// 进行连接
    /// </summary>
	private void Connect()
	{
		SendMsgManager.Instance.ClientConnectToGate();
	}
}
