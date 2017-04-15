using Com.Game.Module;
using MobaFrame.SkillAction;
using MobaProtocol.Data;
using System;
using UnityEngine;

public class Skill_Jiuwei_04 : Skill
{
	private long timeRunning;

	protected BuffData buffData;

	private int lastBuffLayer;

	protected int currCount
	{
		get
		{
			return this.unit.buffManager.GetBuffLayer(this.NeedBuff);
		}
	}

	private bool isUpdateTime
	{
		get
		{
			return this.currCount > 0;
		}
	}

	protected string NeedBuff
	{
		get;
		private set;
	}

	public Skill_Jiuwei_04(string skill_id, Units self) : base(skill_id, self)
	{
		this.NeedBuff = "buff_" + base.skillMainId;
		this.buffData = Singleton<BuffDataManager>.Instance.GetVo(this.NeedBuff);
		if (this.buffData == null)
		{
			Debug.LogError("BuffHostSkill buff is null: " + this.NeedBuff);
		}
	}

	public new bool CheckCondition()
	{
		return base.CheckState() && this.CheckCD(true) && base.CheckCostValue();
	}

	private void ForceCD()
	{
	}

	public override void OnUpdate(float deltaTime)
	{
		base.OnUpdate(deltaTime);
	}

	private void UpdateTime(float deltaTime)
	{
		if (!this.CheckCD(false))
		{
			return;
		}
		if (this.lastBuffLayer != 0 && this.currCount == 0)
		{
			this.ForceCD();
		}
		else if (!this.unit.isLive && this.lastBuffLayer != 0)
		{
			this.ForceCD();
		}
		this.lastBuffLayer = this.currCount;
	}

	public new virtual void OnStop()
	{
		this.lastBuffLayer = 0;
		this.HideSkillActiveHint();
	}

	public override void SynInfo(SynSkillInfo info)
	{
		base.SynInfo(info);
		if (this.unit.isPlayer)
		{
			Singleton<SkillView>.Instance.CheckIconToGrayByCanUseAll(null);
		}
	}

	public new virtual void OnExit()
	{
		this.HideSkillActiveHint();
	}

	public new virtual void OnDeath(Units attacker)
	{
		this.HideSkillActiveHint();
	}

	private void ShowSkillActiveHint()
	{
		if (this.unit != null)
		{
			Singleton<SkillView>.Instance.ShowTriggerBornPowerObjHint(base.skillMainId);
		}
	}

	private void HideSkillActiveHint()
	{
		if (this.unit != null)
		{
			Singleton<SkillView>.Instance.HideTriggerBornPowerObjHint(base.skillMainId);
		}
	}
}
