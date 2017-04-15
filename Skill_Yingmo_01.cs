using Com.Game.Module;
using MobaProtocol.Data;
using System;

public class Skill_Yingmo_01 : Skill
{
	protected Skill skill2;

	protected Skill skill3;

	protected Skill mainSkill;

	public Skill_Yingmo_01()
	{
	}

	public Skill_Yingmo_01(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override bool CanLevelUp()
	{
		return this.self.level == 1 || this.self.level >= 4 || this.self.skillManager.SkillPointsLeft > 0;
	}

	public override void DoSkillLevelUp()
	{
		base.DoSkillLevelUp();
		if (this.skillLevel == 1)
		{
			for (int i = 0; i < 3; i++)
			{
				if (i != base.skillIndex)
				{
					Skill skillByIndex = this.self.skillManager.getSkillByIndex(i);
					if (skillByIndex.skillLevel == 0)
					{
						skillByIndex.skillLevel = 1;
						this.self.skillManager.SkillPointsLeft--;
						if (this.self.isPlayer)
						{
							Singleton<SkillView>.Instance.UpdateSkillItem(i);
							Singleton<SkillView>.Instance.canUseLevelUpPoints--;
						}
					}
				}
			}
		}
	}

	public override void SynInfo(SynSkillInfo info)
	{
	}
}
