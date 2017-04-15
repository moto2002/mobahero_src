using System;

public class Skill_Emowushi_03 : Skill
{
	public Skill_Emowushi_03(string skill_id, Units self) : base(skill_id, self)
	{
	}

	protected override void OnSkillInterrupt()
	{
		base.OnSkillInterrupt();
		this.unit.ForceIdle();
	}
}
