using MobaFrame.SkillAction;
using System;

public class Skill_Houzi_02 : Skill
{
	public Skill_Houzi_02(string skill_id, Units self) : base(skill_id, self)
	{
	}

	protected override void OnSkillStart()
	{
		base.OnSkillStart();
		Hero hero = MapManager.Instance.CreateFenShen(this.self, 3f, 2);
		hero.trans.rotation = this.self.trans.rotation;
		hero.effect_id = "2|FenShenDeath,2|DashengS2";
		string higheff_id = "DashengS2_3_l" + this.skillLevel;
		ActionManager.AddHighEffect(higheff_id, this.skillID, hero, this.self, null, true);
		this.self.surface.ClearTarget();
	}
}
