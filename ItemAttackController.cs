using System;
using UnityEngine;

public class ItemAttackController : AttackController
{
	private bool isComboAttack;

	private bool isConjure;

	public override void ComboAttack(Units target = null)
	{
		this.ComboAttack(0, target);
	}

	private void ComboAttack(int index, Units target)
	{
		if (!this.isComboAttack)
		{
			this.isComboAttack = true;
			Skill attackByIndex = this.self.getAttackByIndex(index);
			if (target == null)
			{
				target = this.self.GetAttackTarget();
			}
			attackByIndex.attackTarget = target;
			if (attackByIndex.CheckTargets())
			{
				attackByIndex.Start();
				attackByIndex.OnSkillStartCallback = new Callback<Skill>(base.OnAttackStart);
				attackByIndex.OnSkillEndCallback = new Callback<Skill>(base.OnAttackEnd);
				base.CurAttack = attackByIndex;
			}
			this.isComboAttack = false;
		}
	}

	public override void Conjure(string skillId, Units target = null, Vector3? targetPos = null)
	{
		if (!this.isConjure)
		{
			this.isConjure = true;
			this.self.InterruptAction(SkillInterruptType.Initiative);
			Skill skillById = this.self.getSkillById(skillId);
			if (target == null)
			{
				target = this.self.GetAttackTarget();
			}
			skillById.attackTarget = target;
			skillById.attackPosition = targetPos;
			if (skillById.CheckTargets())
			{
				skillById.Start();
				skillById.OnSkillStartCallback = new Callback<Skill>(base.OnSkillStart);
				skillById.OnSkillEndCallback = new Callback<Skill>(base.OnSkillEnd);
				base.currSkill = skillById;
			}
			this.isConjure = false;
		}
	}
}
