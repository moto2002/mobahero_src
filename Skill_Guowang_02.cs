using MobaFrame.SkillAction;
using System;

public class Skill_Guowang_02 : Skill
{
	public Skill_Guowang_02(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override void OnSkillStartBegin(StartSkillAction action)
	{
		AudioMgr.Play("Play_Box_Appear", this.unit.gameObject, false, false);
		AudioMgr.Play("Play_Businessman_Boxapr", this.unit.gameObject, false, false);
	}
}
