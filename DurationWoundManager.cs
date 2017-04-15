using MobaFrame.SkillAction;
using System;
using UnityEngine;

public class DurationWoundManager : UnitComponent
{
	private float durationTime = 3f;

	private float lastTime;

	private float lastWoundTime;

	private float lastPlayEffectTime;

	private bool isInDurationWound;

	private bool isPlayingEffect;

	private int woundCount;

	private float loseHealth;

	private string effectId = string.Empty;

	public void SetDurationTime(float time)
	{
		this.durationTime = time;
	}

	public void SetEffectId(string id)
	{
		this.effectId = id;
	}

	public override void OnCreate()
	{
		base.OnCreate();
		if (GlobalSettings.IsTowerDurationWound)
		{
			this.SetEffectId("XiaoxiaoS1");
		}
	}

	public override void OnInit()
	{
		base.OnInit();
		if (!this.self.isHero)
		{
			this.donotUpdateByMonster = true;
		}
	}

	public override void OnStart()
	{
		base.OnStart();
		this.lastTime = Time.time;
		this.lastWoundTime = Time.time;
		this.lastPlayEffectTime = 0f;
		this.isInDurationWound = false;
		this.isPlayingEffect = false;
		this.loseHealth = 0f;
		this.woundCount = 0;
	}

	public override void OnUpdate(float deltaTime)
	{
		if (Time.time - this.lastTime > 0.5f)
		{
			this.lastTime = Time.time;
			if (this.woundCount >= 5 || this.loseHealth > this.self.hp_max * 0.1f)
			{
				this.isInDurationWound = true;
			}
			else
			{
				this.isInDurationWound = false;
			}
		}
		if (Time.time - this.lastWoundTime > 3f)
		{
			this.lastWoundTime = Time.time;
			this.woundCount--;
			this.loseHealth = 0f;
			this.woundCount = Mathf.Clamp(this.woundCount, 0, 2147483647);
		}
		if (Time.time - this.lastPlayEffectTime > 3f)
		{
			this.lastPlayEffectTime = Time.time;
			this.isPlayingEffect = false;
		}
	}

	public override void OnWound(Units attacker, float damage)
	{
		base.OnWound(attacker, damage);
		this.woundCount++;
		this.loseHealth += damage;
		this.woundCount = Mathf.Clamp(this.woundCount, 0, 6);
		if (this.isInDurationWound)
		{
			this.DoDurationWound();
		}
	}

	private void DoDurationWound()
	{
		if (!this.isPlayingEffect)
		{
			this.isPlayingEffect = true;
			ActionManager.PlayEffect(this.effectId, this.self, null, null, true, string.Empty, null);
		}
	}
}
