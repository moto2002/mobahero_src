using Com.Game.Module;
using MobaFrame.SkillAction;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Yasuo_04 : Skill
{
	private LinkAction linkAction;

	private float _deltaTime;

	private Units linePointUnit;

	private List<Units> lineTargets = new List<Units>();

	private VTrigger triggerHandle;

	public Skill_Yasuo_04()
	{
	}

	public Skill_Yasuo_04(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override void OnCreate()
	{
		base.OnCreate();
		this.triggerHandle = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitBeJifei, null, new TriggerAction(this.OnJifeiTrigger), -1, "Hero");
	}

	public override void OnExit()
	{
		base.OnExit();
		TriggerManager.DestroyTrigger(this.triggerHandle);
		this.triggerHandle = null;
	}

	public override void DoSkillLevelUp()
	{
		base.DoSkillLevelUp();
		this.ResetSkillIconMask(base.skillIndex);
	}

	public override bool NeedAutoLaunchToHero()
	{
		return false;
	}

	protected override void OnSkillPhase3End(SkillDataKey skill_key)
	{
		base.OnSkillPhase3End(skill_key);
	}

	public override void OnUpdate(float deltaTime)
	{
		base.OnUpdate(deltaTime);
		this.UpdateLink(deltaTime);
	}

	public override void OnSkillReadyPvp()
	{
		base.OnSkillReadyPvp();
		this._deltaTime = 0f;
	}

	public override void OnSkillStartPvp()
	{
	}

	private void ResetSkillIconMask(int _skillindex)
	{
		if (this.unit != PlayerControlMgr.Instance.GetPlayer())
		{
			return;
		}
		if (this.HasLink())
		{
			Singleton<SkillView>.Instance.SetSkillUIForbidMask(_skillindex, false);
		}
		else
		{
			Singleton<SkillView>.Instance.SetSkillUIForbidMask(_skillindex, true);
		}
	}

	private void CreateLink(Units target)
	{
		this._deltaTime = 0f;
		this.linePointUnit = target;
		this.lineTargets.Clear();
		this.lineTargets.Add(target);
		this.linkAction = ActionManager.Link(new SkillDataKey(string.Empty, 0, 0), "PaotaLink", this.self, this.lineTargets, null, null);
		this.ResetSkillIconMask(base.skillIndex);
	}

	private void ClearLink()
	{
		if (this.linkAction != null)
		{
			this.linkAction.Destroy();
			this.linkAction = null;
		}
		this.linePointUnit = null;
		this._deltaTime = 0f;
		this.ResetSkillIconMask(base.skillIndex);
	}

	private bool HasLink()
	{
		return this.linePointUnit != null;
	}

	public void OnJifeiTrigger()
	{
		Units triggerUnit = TriggerManager.GetTriggerUnit();
		float num = Vector3.Distance(triggerUnit.mTransform.position, this.unit.mTransform.position);
		if (this.unit == PlayerControlMgr.Instance.GetPlayer() && triggerUnit.teamType != this.unit.teamType && triggerUnit.isHero && num <= 8f)
		{
			bool flag = true;
			if (this.HasLink())
			{
				float num2 = Vector3.Distance(this.linePointUnit.mTransform.position, this.unit.mTransform.position);
				flag = (num2 < num);
			}
			if (flag)
			{
				if (this.linkAction != null)
				{
					this.ClearLink();
				}
				this.CreateLink(triggerUnit);
			}
		}
	}

	private void UpdateLink(float deltaTime)
	{
		if (this.HasLink())
		{
			this._deltaTime += deltaTime;
			if (this._deltaTime > 5f)
			{
				this.ClearLink();
			}
			else if (!this.linePointUnit.isLive || this.linePointUnit.m_nVisibleState == 2 || !this.linePointUnit.JiFei.IsInState)
			{
				this.ClearLink();
			}
		}
	}

	public override Units ReselectTarget(Units target, bool isCrazyMode = true)
	{
		return this.linePointUnit;
	}
}
