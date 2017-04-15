using MobaFrame.SkillAction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class Skill_GuangMingShen_03 : Skill
{
	private bool _isCanTriggerBornPowerObj;

	private float _bornPowerObjLifeTime = 4.9f;

	public Skill_GuangMingShen_03(string skill_id, Units self) : base(skill_id, self)
	{
		this.InitBornPowerObjLifeTime();
	}

	private void InitBornPowerObjLifeTime()
	{
		List<SkillData> skillDataList = this.skillDataList;
		if (skillDataList != null && skillDataList.Count > 0)
		{
			SkillData skillData = skillDataList[0];
			if (skillData.hitTimes != null && skillData.hitTimes.Length > 0)
			{
				this._bornPowerObjLifeTime = skillData.hitTimes[0];
			}
		}
	}

	public override bool IsCanTriggerBornPowerObj()
	{
		return this._isCanTriggerBornPowerObj;
	}

	public override void OnBornPowerObj()
	{
		if (!this._isCanTriggerBornPowerObj)
		{
			this._isCanTriggerBornPowerObj = true;
			this.self.skillManager.TryShowBornPowerObjHint(this.skillID, this.self.isPlayer);
			this.TryStartProtectDoSkill();
		}
	}

	public override void OnBornPowerObjTriggered()
	{
		this._isCanTriggerBornPowerObj = false;
	}

	public override void OnSynced(byte inUseState)
	{
		if (inUseState != 2 && this._isCanTriggerBornPowerObj)
		{
			this._isCanTriggerBornPowerObj = false;
			this.self.skillManager.TryRemoveBornPowerObjInfo(this.skillID, this.self.isPlayer);
		}
	}

	private void TryStartProtectDoSkill()
	{
		Units units = null;
		if (PlayerControlMgr.Instance != null)
		{
			units = PlayerControlMgr.Instance.GetPlayer();
		}
		if (this.self != null && units != null && this.self.unique_id == units.unique_id)
		{
			GlobalObject.Instance.StartCoroutine(this.ProtectDoSkill());
		}
	}

	[DebuggerHidden]
	private IEnumerator ProtectDoSkill()
	{
		Skill_GuangMingShen_03.<ProtectDoSkill>c__Iterator9A <ProtectDoSkill>c__Iterator9A = new Skill_GuangMingShen_03.<ProtectDoSkill>c__Iterator9A();
		<ProtectDoSkill>c__Iterator9A.<>f__this = this;
		return <ProtectDoSkill>c__Iterator9A;
	}
}
