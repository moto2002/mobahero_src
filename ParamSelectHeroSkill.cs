using System;

public class ParamSelectHeroSkill
{
	public readonly int userId;

	public readonly string skillId;

	public ParamSelectHeroSkill(int userId, string skillId)
	{
		this.userId = userId;
		this.skillId = skillId;
	}
}
