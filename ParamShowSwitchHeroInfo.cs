using Common;
using System;

public class ParamShowSwitchHeroInfo
{
	public readonly EShowSwitchHeroInfoType showSwitchHeroInfoType;

	public readonly int newUid;

	public ParamShowSwitchHeroInfo(EShowSwitchHeroInfoType inType, int inNewUid)
	{
		this.showSwitchHeroInfoType = inType;
		this.newUid = inNewUid;
	}
}
