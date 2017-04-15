using Com.Game.Module;
using System;
using UnityEngine;

public class AttackController : StaticUnitComponent
{
	private Skill m_CurAttack;

	private Skill m_CurSkill;

	public Skill CurAttack
	{
		get
		{
			return this.m_CurAttack;
		}
		set
		{
			this.m_CurAttack = value;
		}
	}

	public Skill currSkill
	{
		get
		{
			return this.m_CurSkill;
		}
		set
		{
			this.m_CurSkill = value;
		}
	}

	public override void OnInit()
	{
	}

	public override void OnStart()
	{
	}

	public override void OnUpdate(float deltaTime)
	{
	}

	public override void OnStop()
	{
	}

	public override void OnExit()
	{
		if (this.m_CurAttack != null)
		{
			this.m_CurAttack.End();
			this.m_CurAttack = null;
		}
		if (this.m_CurSkill != null)
		{
			this.m_CurSkill.End();
			this.m_CurSkill = null;
		}
	}

	public bool IsCurAttackRunnning()
	{
		return this.CurAttack != null && this.CurAttack.IsCastting;
	}

	public bool IsCurSkillRunnning()
	{
		return this.currSkill != null && this.currSkill.IsCastting;
	}

	public virtual void ComboAttack(Units target = null)
	{
		throw new NotImplementedException();
	}

	public virtual void Conjure(string skillId, Units target = null, Vector3? targetPos = null)
	{
		throw new NotImplementedException();
	}

	public virtual void ConjurePassive(string skillId, Units target = null, Vector3? targetPos = null)
	{
		throw new NotImplementedException();
	}

	public virtual bool CheckSkillCondition(string skillId)
	{
		throw new NotImplementedException();
	}

	public virtual bool CheckSkillCondition(int skillIndex)
	{
		throw new NotImplementedException();
	}

	public void InterruptAttack(SkillInterruptType type)
	{
		if (this.CurAttack != null)
		{
			this.CurAttack.Interrupt(type);
		}
	}

	public void StopAttack()
	{
		if (this.CurAttack != null)
		{
			this.CurAttack.End();
		}
	}

	public void StopAttack(string attackId)
	{
		Skill attackById = this.self.getAttackById(attackId);
		if (attackById != null)
		{
			attackById.End();
		}
	}

	public void interruptSkill(SkillInterruptType type)
	{
		if (this.currSkill != null)
		{
			this.currSkill.Interrupt(type);
		}
	}

	public void StopConjure()
	{
		if (this.currSkill != null)
		{
			this.currSkill.End();
		}
	}

	public void StopConjure(string skillId)
	{
		Skill skillById = this.self.getSkillById(skillId);
		if (skillById != null)
		{
			skillById.End();
		}
	}

	[Obsolete("legacy")]
	public void InterruptAction(SkillInterruptType type)
	{
		this.InterruptAttack(type);
		this.interruptSkill(type);
	}

	protected void OnAttackStart(Skill s)
	{
	}

	protected void OnAttackEnd(Skill s)
	{
	}

	protected void OnAttackFailedBeforeStart(Skill s)
	{
		if (this.self.isPlayer)
		{
			Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitAttackFailed, this.self, null, null);
			Singleton<TriggerManager>.Instance.SendUnitSkillStateEvent(UnitEvent.UnitSkillCmdFailed, this.self, s);
		}
	}

	protected void OnSkillStart(Skill s)
	{
	}

	protected void OnSkillEnd(Skill s)
	{
	}

	protected void OnSkillFailedBeforeStart(Skill s)
	{
		if (this.self.isPlayer)
		{
			Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitSkillFailed, this.self, null, null);
			Singleton<TriggerManager>.Instance.SendUnitSkillStateEvent(UnitEvent.UnitSkillCmdFailed, this.self, s);
		}
	}

	public void SetCurSkill(Skill skill)
	{
		this.currSkill = skill;
	}

	public void SetCurAttack(Skill skill)
	{
		this.CurAttack = skill;
	}
}
