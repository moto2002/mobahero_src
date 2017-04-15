using MobaFrame.SkillAction;
using System;

public class Skill_Tufu_02 : Skill
{
	public Skill_Tufu_02()
	{
	}

	public Skill_Tufu_02(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override void OnSkillReadyBegin(ReadySkillAction action)
	{
		base.OnSkillReadyBegin(action);
		if (action.targetUnits != null && action.targetUnits.Count > 0)
		{
			if (action.targetUnits[0] == this.unit)
			{
				this.unit.animController.PlayAnim(AnimationType.Conjure, true, 5, true, false);
			}
			else
			{
				this.unit.animController.PlayAnim(AnimationType.Conjure, true, 2, true, false);
			}
		}
	}

	public override void OnSkillHitBegin(HitSkillAction action)
	{
		base.OnSkillHitBegin(action);
		if (action.targetUnits != null && action.targetUnits.Count > 0)
		{
			if (!(action.targetUnits[0] == this.unit))
			{
				this.unit.animController.PlayAnim(AnimationType.Conjure, true, 5, true, false);
			}
		}
	}
}
