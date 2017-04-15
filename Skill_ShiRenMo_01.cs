using MobaFrame.SkillAction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class Skill_ShiRenMo_01 : Skill
{
	public override void StartCastSkill(SkillDataKey skill_key)
	{
		this._DoStartCastSkill(skill_key);
		GlobalObject.Instance.StartCoroutine(this._StartCastSkillEnumerator(skill_key));
	}

	[DebuggerHidden]
	private IEnumerator _StartCastSkillEnumerator(SkillDataKey skill_key)
	{
		Skill_ShiRenMo_01.<_StartCastSkillEnumerator>c__Iterator98 <_StartCastSkillEnumerator>c__Iterator = new Skill_ShiRenMo_01.<_StartCastSkillEnumerator>c__Iterator98();
		<_StartCastSkillEnumerator>c__Iterator.skill_key = skill_key;
		<_StartCastSkillEnumerator>c__Iterator.<$>skill_key = skill_key;
		<_StartCastSkillEnumerator>c__Iterator.<>f__this = this;
		return <_StartCastSkillEnumerator>c__Iterator;
	}

	private void _DoStartCastSkill(SkillDataKey skill_key)
	{
		this.AddHighEff(skill_key, SkillPhrase.Start, this.attackTargets, this.GetSkillPosition());
		this.AddBuff(skill_key, SkillPhrase.Start, this.attackTargets);
		StartSkillAction startSkillAction = ActionManager.StartSkill(this.skillKey, this.unit, this.attackTargets, this.attackPosition, true, null);
		if (startSkillAction != null)
		{
			startSkillAction.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
			startSkillAction.OnSkillEndCallback = new Callback<BaseSkillAction>(base.OnStartSkillActionEnd);
			this.AddAction(SkillCastPhase.Cast_In, startSkillAction);
		}
	}
}
