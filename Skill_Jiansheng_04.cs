using System;

public class Skill_Jiansheng_04 : Skill
{
	public Skill_Jiansheng_04(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public Skill_Jiansheng_04()
	{
	}

	protected override void OnSkillInterrupt()
	{
		base.OnSkillInterrupt();
		this.self.ForceIdle();
		if (this.self.isMoving)
		{
			this.self.PlayAnim(AnimationType.Move, true, 0, true, true);
		}
	}
}
