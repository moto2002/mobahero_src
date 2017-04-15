using System;

public class UnitsTimeSyncSystem : UnitComponent
{
	public float unitsTimeScale = 1f;

	private long unitsSvrTime;

	public long unitsClientSvrTime;

	private int tmpCnt;

	public UnitsTimeSyncSystem()
	{
		this.donotUpdateByMonster = true;
	}

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
			if (num <= 0f)
			{
				this.unitsTimeScale = 0.9f;
				return;
			}
			if (num < 600f)
			{
				this.unitsTimeScale = 1f + num / 1000f;
			}
			else if (num < 2000f)
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
