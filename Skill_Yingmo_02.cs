using MobaHeros;
using System;

public class Skill_Yingmo_02 : Skill_Yingmo_01
{
	public Skill_Yingmo_02()
	{
	}

	public Skill_Yingmo_02(string skill_id, Units self) : base(skill_id, self)
	{
		this.mainSkill = this.unit.skillManager.getSkillById("Skill_Yingmo_01");
	}

	public override void DoSkillLevelUp()
	{
		base.DoSkillLevelUp();
		base.Data.damage_ids = this.mainSkill.Data.damage_ids;
		base.Data.cost_ids = this.mainSkill.Data.cost_ids;
	}

	public override float GetCostValue(AttrType type)
	{
		return this.mainSkill.GetCostValue(type);
	}
}
