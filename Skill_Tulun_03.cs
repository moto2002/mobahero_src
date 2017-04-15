using System;

public class Skill_Tulun_03 : Skill
{
	public Skill_Tulun_03()
	{
	}

	public Skill_Tulun_03(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override Units CustomTargetInCrazy()
	{
		return FindTargetHelper.FindNearstLabisi(this.self, this.self.trans.position);
	}

	public override bool NeedAutoLaunchToHero()
	{
		return false;
	}

	public override bool NeedCustomTargetInCrazy()
	{
		return true;
	}
}
