using System;
using UnityEngine;

public class PVPCardInfo
{
	public int userID
	{
		get;
		set;
	}

	public Color32 color
	{
		get;
		set;
	}

	public Texture texture
	{
		get;
		set;
	}

	public string HeroName
	{
		get;
		set;
	}

	public string SummonerName
	{
		get;
		set;
	}

	public string SkillID
	{
		get;
		set;
	}

	public Action<PVPHeroCard> OnLoadFinish
	{
		get;
		set;
	}

	public int RankFrame
	{
		get;
		set;
	}

	public int lastCharmRank
	{
		get;
		set;
	}

	public void ClearResources()
	{
		if (this.texture != null)
		{
			Resources.UnloadAsset(this.texture);
		}
	}
}
