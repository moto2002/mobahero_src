using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MyHeroItem : MonoBehaviour
{
	public UITexture HeroPic;

	public UILabel HeroName;

	public UILabel HeroUse;

	public UILabel HeroAllSkin;

	public UILabel MySkin;

	public UILabel WinPercent;

	public UILabel WinCount;

	public void Init(List<int> skinlist, HeroUsedData herouseddata)
	{
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(herouseddata.heroid);
		if (heroMainData == null)
		{
			ClientLogger.Error(" Can't find id=" + herouseddata.heroid + "in SysHeroMainVo");
		}
		else
		{
			this.HeroPic.mainTexture = ResourceManager.Load<Texture>(heroMainData.Loading_icon, true, true, null, 0, false);
			this.HeroName.text = LanguageManager.Instance.GetStringById(heroMainData.name);
			base.transform.name = heroMainData.name;
			if (heroMainData.skin_id != "[]")
			{
				string[] array = heroMainData.skin_id.Split(new char[]
				{
					','
				});
				this.HeroAllSkin.text = "/" + array.Length.ToString();
			}
			else
			{
				this.HeroAllSkin.text = "/0";
			}
		}
		this.HeroUse.text = ToolsFacade.Instance.GetMillionsSuffix(herouseddata.useinfo);
		this.WinCount.text = (herouseddata.wincount + herouseddata.losecount).ToString();
		if (herouseddata.wincount + herouseddata.losecount == 0)
		{
			this.WinPercent.text = "0%";
		}
		else
		{
			this.WinPercent.text = ((double)herouseddata.wincount / (double)(herouseddata.wincount + herouseddata.losecount) * 100.0).ToString("F0") + "%";
		}
		if (skinlist == null)
		{
			this.MySkin.text = "0";
		}
		else
		{
			this.MySkin.text = skinlist.Count.ToString();
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
