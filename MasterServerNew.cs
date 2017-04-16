using Assets.Scripts.Model;
using Com.Game.Module;
using MobaClient;
using System;
using UnityEngine;
/// <summary>
/// Master服务器类
/// </summary>
public class MasterServerNew : ServerHelpCom
{
	private const float waitTime = 3f;
    /// <summary>
    /// 服务器连接通信客户端
    /// </summary>
	private PhotonClient _client;

	private bool bConnect;

	private bool enable;

	private float disConnectTime;
    /// <summary>
    /// 重连次数计数值
    /// </summary>
	private int _retryNum;
    /// <summary>
    /// 暂停开始时间
    /// </summary>
	private float pauseStartTime;
    /// <summary>
    /// 连接标记，用来判断通信是否连接
    /// </summary>
	public override bool ConnectFlag
	{
		get
		{
			return this.bConnect;
		}
		protected set
		{
			if (value != this.bConnect)
			{
				this.bConnect = value;
				if (!this.bConnect) //如果没有连接，启用重连计时器
				{
					this.ResetTimer();
				}
				else
				{
					this._retryNum = 0; // 连接成功，则重置重连次数
				}
			}
		}
	}

	public MasterServerNew(PhotonClient client)
	{
		this._client = client;
	}

	public override void OnAwake()
	{
		this.enable = false;
	}

	public override void OnStart()
	{
		ModelManager.Instance.Set_masterForceIp(GlobalSettings.Instance.MasterIpForce);
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (this.enable)
		{
			this._client.GetMobaPeer(MobaPeerType.C2Master).PeerUpdate();
		}
	}

	public override void OnDestroy()
	{
		this.End();
	}
    /// <summary>
    /// 开始进行连接
    /// </summary>
	public override void Begin()
	{
		if (!this.enable)
		{
			this.enable = true;
			this.Connect();
		}
	}
    /// <summary>
    /// 断开关闭连接
    /// </summary>
	public override void End()
	{
		if (this.enable)
		{
			this.enable = false;
			if (this._client != null && this._client.master_peer != null)
			{
				this._client.master_peer.PeerDisconnect();
				this._client.master_peer.StopThread();
			}
		}
	}
    /// <summary>
    /// master连接成功回调处理
    /// </summary>
    /// <param name="cType"></param>
	public override void OnConnected(MobaConnectedType cType)
	{
		if (cType != MobaConnectedType.ExceptionOnConnect)
		{
			this.ConnectFlag = true;
			MobaMessageManagerTools.EndWaiting_manual("ConnectMasterServer");
			MobaMessageManager.ExecuteMsg(MobaMessageManager.GetMessage((ClientMsg)20003, cType, 0f));
		}
	}

	public override void OnDisconnected(MobaDisconnectedType dType)
	{
		this.ConnectFlag = false;
		MobaMessageManagerTools.EndWaiting_manual("ConnectMasterServer");
		MobaMessageManager.ExecuteMsg(MobaMessageManager.GetMessage((ClientMsg)20004, dType, 0f));
		if (this.enable)
		{
			if (this.TimeOut())
			{
				this.InqueryConnect();
			}
			else
			{
				this.Connect();
			}
		}
	}
    /// <summary>
    /// 连接失败处理
    /// </summary>
	private void InqueryConnect()
	{
		if (this._retryNum >= 2)
		{
			CtrlManager.ShowMsgBox("游戏服务器无法连接", "网络出故障了，请重试", delegate(bool yes)
			{
				if (yes)
				{
					this.OnHandle_connect();
				}
				else
				{
					this.OnHandle_restart();
				}
			}, PopViewType.PopTwoButton, "确定", "重启游戏", null);
		}
		else
		{
			CtrlManager.ShowMsgBox("服务器无法连接", "网络出故障了，请重试", delegate
			{
				this.OnHandle_connect();
			}, PopViewType.PopOneButton, "确定", "取消", null);
		}
	}
    /// <summary>
    /// 游戏重新开始处理
    /// </summary>
	private void OnHandle_restart()
	{
		GlobalObject.ReStartGame();
	}
    /// <summary>
    /// 重连处理
    /// </summary>
	private void OnHandle_connect()
	{
		this._retryNum++;
		this.ResetTimer();
		this.Connect();
	}

	private void Connect()
	{
		if (!this.ConnectFlag)
		{
			MobaMessageManagerTools.BeginWaiting_manual("ConnectMasterServer", "连接Master服务器...", false, 1000f, true);
			if (!SendMsgManager.Instance.ClientConnectToMaster())
			{
				MobaMessageManagerTools.EndWaiting_manual("ConnectMasterServer");
				CtrlManager.ShowMsgBox("游戏服务器无法连接", "网络出故障了，请重试", delegate(bool yes)
				{
					if (yes)
					{
						this.OnHandle_connect();
					}
					else
					{
						this.OnHandle_restart();
					}
				}, PopViewType.PopTwoButton, "确定", "重启游戏", null);
				Debug.LogError("MasterServerNew Connect failed");
			}
		}
	}
    /// <summary>
    /// 重置断线计时器
    /// </summary>
	private void ResetTimer()
	{
		this.disConnectTime = Time.realtimeSinceStartup;
	}
    /// <summary>
    /// 是否断线超时
    /// </summary>
    /// <returns></returns>
	private bool TimeOut()
	{
		return this.disConnectTime + 3f < Time.realtimeSinceStartup;
	}

	public override void OnApplicationPause(bool isPause)
	{
		if (!this.enable)
		{
			return;
		}
		if (isPause)
		{
			this.pauseStartTime = Time.realtimeSinceStartup;
		}
		else if (this.pauseStartTime + 5f < Time.realtimeSinceStartup)//暂停超过5秒重新连接
		{
			this.Connect();
		}
	}

	public override void OnApplicationFocus(bool isFocus)
	{
		if (!this.enable)
		{
			return;
		}
	}
}
