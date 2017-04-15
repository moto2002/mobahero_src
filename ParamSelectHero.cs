using MobaProtocol.Data;
using System;

public class ParamSelectHero
{
	public readonly int userId;

	public readonly HeroInfo heroInfo;

	public ParamSelectHero(int userId, HeroInfo heroInfo)
	{
		this.userId = userId;
		this.heroInfo = heroInfo;
	}
}
