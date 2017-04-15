using System;
using System.Collections.Generic;

public class ParamGetCanSwitchHeroInfo
{
	public readonly List<int> canSwitchHeroNewUids;

	public readonly int randomSelHeroPoint;

	public ParamGetCanSwitchHeroInfo(List<int> inNewUids, int inRandomPoint)
	{
		this.canSwitchHeroNewUids = inNewUids;
		this.randomSelHeroPoint = inRandomPoint;
	}
}
