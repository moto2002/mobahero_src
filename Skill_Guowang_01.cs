using MobaFrame.SkillAction;
using System;

public class Skill_Guowang_01 : Skill
{
	public Skill_Guowang_01(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override void OnSkillStartBegin(StartSkillAction action)
	{
		AudioMgr.Play("Play_Coin_Throw", this.unit.gameObject, false, false);
	}
}
