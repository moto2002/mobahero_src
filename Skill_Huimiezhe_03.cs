using Com.Game.Module;
using System;

public class Skill_Huimiezhe_03 : Skill
{
	public Skill_Huimiezhe_03(string skill_id, Units self) : base(skill_id, self)
	{
	}

	protected override void OnSkillInterrupt()
	{
		base.OnSkillInterrupt();
		if (this.unit.isPlayer)
		{
			Singleton<SkillView>.Instance.CheckIconToGrayByCanUseAll(null);
		}
	}
}
