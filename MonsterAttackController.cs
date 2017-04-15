using System;
using UnityEngine;

public class MonsterAttackController : AttackController
{
	private int _attackTotalNum = 1;

	private int _curAttackIndex;

	private bool isComboAttack;

	private bool isConjure;

	public override void ComboAttack(Units target = null)
	{
		this.ComboAttack(this._curAttackIndex, target);
	}

	public void ComboAttack(int index, Units target)
	{
		if (this.self.CanAttack && !this.isComboAttack)
		{
			this.isComboAttack = true;
			Skill attackByIndex = this.self.getAttackByIndex(index);
			if (attackByIndex != null)
			{
				if (target == null)
				{
					target = this.self.GetAttackTarget();
				}
				attackByIndex.attackTarget = target;
				if (attackByIndex.CheckCondition() && attackByIndex.CheckTargets() && !base.IsCurAttackRunnning())
				{
					attackByIndex.Start();
					attackByIndex.OnSkillStartCallback = new Callback<Skill>(base.OnAttackStart);
					attackByIndex.OnSkillEndCallback = new Callback<Skill>(base.OnAttackEnd);
					base.CurAttack = attackByIndex;
					this.UpdateCurAttackIndex();
				}
			}
			this.isComboAttack = false;
		}
	}

	public override void Conjure(string skillId, Units target = null, Vector3? targetPos = null)
	{
		if (this.self.CanSkill && !this.isConjure)
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
			if (skillById.CheckCondition() && skillById.CheckTargets())
			{
				skillById.Start();
				skillById.OnSkillStartCallback = new Callback<Skill>(base.OnSkillStart);
				skillById.OnSkillEndCallback = new Callback<Skill>(base.OnSkillEnd);
				base.currSkill = skillById;
			}
			this.isConjure = false;
		}
	}

	private void InitAttackInfo()
	{
		if (this.self != null && this.self.skillManager != null)
		{
			this._attackTotalNum = this.self.skillManager.GetAttackTotalNum();
			this._curAttackIndex = 0;
		}
	}

	private void UpdateCurAttackIndex()
	{
		this._curAttackIndex++;
		if (this._curAttackIndex >= this._attackTotalNum)
		{
			this._curAttackIndex = 0;
		}
	}

	public override void OnStart()
	{
		this.InitAttackInfo();
	}
}
