using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using UnityEngine;

public class GoldViewDeathTimer : MonoBehaviour
{
	public UILabel timer;

	public UISlider timerSlider;

	private Units player;

	private VTrigger PlayerTimer;

	private UIBloodBar playerBlood;

	private GameObject timerLabel;

	private float interval = 0.2f;

	private float lastUpdateTime;

	private float liveTime;

	private float liveTimeLength;

	private float lastUpdateTime1;

	private int sum;

	private int _lastTime = -1;

	private void Start()
	{
		this.timerLabel = this.timer.transform.parent.gameObject;
		this.timerLabel.SetActive(false);
		MobaMessageManager.RegistMessage((ClientMsg)25036, new MobaMessageFunc(this.OnPlayerAttached));
		MobaMessageManager.RegistMessage((ClientMsg)25035, new MobaMessageFunc(this.OnUnitDeathTime));
	}

	private void OnUnitDeathTime(MobaMessage msg)
	{
		ParamUnitDeathTime paramUnitDeathTime = msg.Param as ParamUnitDeathTime;
		if (paramUnitDeathTime.uniqueId == this.player.unique_id)
		{
			this.liveTime = Time.realtimeSinceStartup + paramUnitDeathTime.reliveTime;
			this.liveTimeLength = paramUnitDeathTime.reliveTime;
		}
	}

	private void OnPlayerAttached(MobaMessage msg)
	{
		this.player = PlayerControlMgr.Instance.GetPlayer();
		if (this.player != null)
		{
			this.playerBlood = this.player.GetUnitComponent<SurfaceManager>().mHpBar;
		}
	}

	private void Update()
	{
		if (Time.time - this.lastUpdateTime1 < this.interval / 4f)
		{
			return;
		}
		this.lastUpdateTime1 = Time.time;
		this.sum++;
		if (this.liveTime > Time.realtimeSinceStartup)
		{
			this.timerLabel.SetActive(true);
			this.timerSlider.gameObject.SetActive(true);
			if (this.sum > 10)
			{
				this.SetDeathTime(this.liveTime - Time.realtimeSinceStartup);
				this.sum = 0;
			}
			this.timerSlider.value = (this.liveTime - Time.realtimeSinceStartup) / this.liveTimeLength;
		}
		else if (this.liveTime != 0f)
		{
			this.liveTime = 0f;
			if (this.sum > 10)
			{
				this.SetDeathTime(0f);
				this.sum = 0;
			}
			this.timerLabel.SetActive(false);
			this.timerSlider.gameObject.SetActive(false);
			Singleton<PvpManager>.Instance.StartCheckRelive(2f);
		}
		if (Time.time - this.lastUpdateTime < this.interval)
		{
			return;
		}
		this.lastUpdateTime = Time.time;
		bool flag = true;
		if (this.player != null)
		{
			flag = this.player.isLive;
		}
		if (this.playerBlood != null)
		{
			this.playerBlood.ShowAITimer((int)StrategyManager.Instance.Time2Auto, StrategyManager.Instance.Time2Auto <= 5f && StrategyManager.Instance.Time2Auto != 0f && flag);
		}
	}

	private void OnDisable()
	{
		MobaMessageManager.UnRegistMessage((ClientMsg)25036, new MobaMessageFunc(this.OnPlayerAttached));
		MobaMessageManager.UnRegistMessage((ClientMsg)25035, new MobaMessageFunc(this.OnUnitDeathTime));
	}

	private void SetDeathTime(float time)
	{
		int num = (int)time;
		if (num != this._lastTime || this._lastTime < 0)
		{
			this._lastTime = num;
			if (time <= 0f)
			{
				this.timer.text = string.Empty;
			}
			else
			{
				this.timer.text = num.ToString();
			}
		}
	}
}
