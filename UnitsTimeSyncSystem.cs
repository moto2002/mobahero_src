using System;
/// <summary>
/// 时间同步系统组件
/// </summary>
public class UnitsTimeSyncSystem : UnitComponent
{
    /// <summary>
    /// unit的时间缩放
    /// </summary>
	public float unitsTimeScale = 1f;
    /// <summary>
    /// 客户端之前同步的服务器时间？？？？？
    /// </summary>
	private long unitsSvrTime;
    /// <summary>
    /// unit的客户端计时
    /// </summary>
	public long unitsClientSvrTime;

	private int tmpCnt;

	public UnitsTimeSyncSystem()
	{
		this.donotUpdateByMonster = true;  //Monster不update
	}
    /// <summary>
    /// 重置客户端计时
    /// </summary>
    /// <param name="inSvrTime">时间必须慢与同步服务器时间</param>
	public void ResetClientSvrTime(long inSvrTime)
	{
		if (inSvrTime < this.unitsSvrTime)
		{
			this.unitsClientSvrTime = inSvrTime;
		}
	}

	public override void OnInit()
	{
	}
    /// <summary>
    /// 持续update方法接口
    /// </summary>
    /// <param name="deltaTime"></param>
	public override void OnUpdate(float deltaTime)
	{
		if (this.unitsSvrTime == 0L)
		{
			Units player = PlayerControlMgr.Instance.GetPlayer();
			if (player != null)
			{
				this.unitsSvrTime = NetWorkHelper.Instance.client.GetSvrTime();
				this.unitsClientSvrTime = NetWorkHelper.Instance.client.GetClientTime();
			}
		}
		else
		{
			this.unitsClientSvrTime += (long)(deltaTime * 1000f);
			this.unitsSvrTime = NetWorkHelper.Instance.client.GetSvrTime();
			float num = (float)(this.unitsSvrTime - this.unitsClientSvrTime);
			if (num <= 0f) //客户端跑快乐，要降下来
			{
				this.unitsTimeScale = 0.9f;
				return;
			}
			if (num < 600f)//客户端跑慢了追上来
			{
				this.unitsTimeScale = 1f + num / 1000f;
			}
			else if (num < 2000f)   //最大3倍缩放
			{
				this.unitsTimeScale = 3f;
			}
			else
			{
				this.unitsTimeScale = 1f;
				this.unitsClientSvrTime = this.unitsSvrTime;
			}
			if (this.tmpCnt++ % 2 == 0)
			{
			}
			this.ProcessMsgAfterUpdateTime();
		}
	}
    /// <summary>
    /// 更新完时间，进行消息pvp server msg处理
    /// </summary>
	private void ProcessMsgAfterUpdateTime()
	{
		while (true)
		{
			MobaMessage mobaMessage = this.self.FetchPvpServerMsg();
			if (mobaMessage == null)
			{
				break;
			}
			MobaMessageManager.ExecuteMsg(mobaMessage);
		}
	}

	public override void OnStop()
	{
	}

	public override void OnExit()
	{
	}
}
