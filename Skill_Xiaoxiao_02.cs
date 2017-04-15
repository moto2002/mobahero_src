using MobaFrame.SkillAction;
using System;
using System.Collections.Generic;

public class Skill_Xiaoxiao_02 : Skill
{
	private string normalHighEff = "HighEff_189";

	private string hasBuffHigheff = "HighEff_189_1";

	private string hasBuff = "1023040104";

	public Skill_Xiaoxiao_02(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override void OnSkillDamage(BaseSkillAction action, List<Units> targets)
	{
		if (targets != null)
		{
			for (int i = 0; i < targets.Count; i++)
			{
				if (targets[i].buffManager.IsHaveBuff(this.hasBuff))
				{
					ActionManager.AddHighEffect(this.hasBuffHigheff, base.skillMainId, targets[i], this.self, this.GetSkillPosition(), true);
				}
				else
				{
					ActionManager.AddHighEffect(this.normalHighEff, base.skillMainId, targets[i], this.self, this.GetSkillPosition(), true);
				}
			}
		}
		base.OnSkillDamage(action, targets);
	}
}
