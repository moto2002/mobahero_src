using MobaFrame.SkillAction;
using System;

public class Skill_Tulun_02_1 : Skill
{
	public Skill_Tulun_02_1()
	{
	}

	public Skill_Tulun_02_1(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override void OnSkillStartBegin(StartSkillAction action)
	{
		base.OnSkillStartBegin(action);
		if (action != null && action.targetPosition.HasValue)
		{
			this.unit.trans.LookAt(action.targetPosition.Value);
		}
	}
}
