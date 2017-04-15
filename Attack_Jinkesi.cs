using MobaFrame.SkillAction;
using System;

public class Attack_Jinkesi : Skill
{
	public Attack_Jinkesi()
	{
	}

	public Attack_Jinkesi(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override void OnSkillStartBegin(StartSkillAction action)
	{
		base.OnSkillStartBegin(action);
	}

	public override void OnSkillReadyBegin(ReadySkillAction action)
	{
		base.OnSkillReadyBegin(action);
		if (this.unit.buffManager.IsHaveBuff(Skill_Jinkesi_01.CannonBuff))
		{
			if (base.skillIndex == 0 || base.skillIndex == 2)
			{
				base.data.ready_actions = new string[]
				{
					"Attack_Jinkesi_02_0"
				};
			}
			if (base.skillIndex == 1 || base.skillIndex == 3)
			{
				base.data.ready_actions = new string[]
				{
					"Attack_Jinkesi_04_0"
				};
			}
		}
		else
		{
			if (base.skillIndex == 0 || base.skillIndex == 2)
			{
				base.data.ready_actions = new string[]
				{
					"Attack_Jinkesi_01_0"
				};
			}
			if (base.skillIndex == 1 || base.skillIndex == 3)
			{
				base.data.ready_actions = new string[]
				{
					"Attack_Jinkesi_03_0"
				};
			}
		}
	}
}
