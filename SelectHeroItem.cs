using System;
using UnityEngine;

public class SelectHeroItem : HeroItem
{
	public SelectHeroItem(Transform root) : base(root)
	{
	}

	public override void UpdateSelect(bool isSelected, bool isMaster = false)
	{
		if (isSelected)
		{
			this.m_Selected = true;
			this.UpdateMaster(isMaster);
		}
		else
		{
			this.m_Selected = false;
			this.UpdateMaster(false);
		}
	}

	protected override void UpdateMaster(bool isMaster)
	{
		if (isMaster)
		{
			this.UIFrame.spriteName = "zy_kuang_yingxiong02";
			this.m_MasterHero = true;
		}
		else
		{
			this.UIFrame.spriteName = "zy_kuang_yingxiong01";
			this.m_MasterHero = false;
		}
	}
}
