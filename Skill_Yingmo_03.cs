using System;

public class Skill_Yingmo_03 : Skill_Yingmo_01
{
	public Skill_Yingmo_03()
	{
	}

	public Skill_Yingmo_03(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override void DoSkillLevelUp()
	{
		base.DoSkillLevelUp();
		this.mainSkill = this.unit.skillManager.getSkillById("Skill_Yingmo_01");
		base.Data.damage_ids = this.mainSkill.Data.damage_ids;
		base.Data.cost_ids = this.mainSkill.Data.cost_ids;
	}
}
