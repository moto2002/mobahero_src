using Com.Game.Module;
using MobaFrame.SkillAction;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EditorBuff
{
	private BuffData buffData;

	private float curTime;

	private float curEffectTime;

	private EditorSkill skill;

	private EditorUnit unit;

	private List<EditorPerform> performs;

	public virtual void Start(string buffId, EditorSkill skill, EditorUnit unit)
	{
		this.buffData = Singleton<BuffDataManager>.Instance.GetVo(buffId);
		this.curTime = 0f;
		this.curEffectTime = 0f;
		this.skill = skill;
		this.unit = unit;
		this.DoEffect();
	}

	public virtual void OnUpdate()
	{
		this.curTime += Time.deltaTime;
		this.curEffectTime += Time.deltaTime;
		if (this.buffData.IsEffective() && this.curEffectTime > this.buffData.config.effective_time)
		{
			this.curEffectTime = 0f;
			if (this.buffData.config.isReaptPerform == 1f)
			{
				this.DoEffect();
			}
		}
	}

	public virtual void DoEffect()
	{
		this.performs = EditorSkill.StartPerform(this.buffData.perform_ids, this.unit, new List<EditorUnit>
		{
			this.unit
		}, null, this.skill);
	}

	public virtual void OnRemove()
	{
		EditorSkill.RemovePerform(this.performs);
	}

	public bool IsOver()
	{
		return this.curTime > this.buffData.config.buff_time;
	}
}
