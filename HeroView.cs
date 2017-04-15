using System;
using UnityEngine;

public class HeroView : CompositeView<HeroItem>
{
	public HeroView(Transform root) : base(root)
	{
	}

	public override HeroItem GetChild(Transform root)
	{
		for (int i = 0; i < base.childList.Count; i++)
		{
			HeroItem heroItem = base.childList[i];
			if (heroItem.transform == root)
			{
				return base.childList[i];
			}
		}
		return null;
	}

	public override HeroItem GetChild(string name)
	{
		for (int i = 0; i < base.childList.Count; i++)
		{
			HeroItem heroItem = base.childList[i];
			if (heroItem.name.Equals(name))
			{
				return base.childList[i];
			}
		}
		return null;
	}

	public override void CreateChildView(int dataCount, int itemCount, GameObject itemPrefab, Transform changeRoot = null)
	{
		if (base.transform == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < itemCount; i++)
		{
			HeroItem heroItem;
			if (i >= base.childCount)
			{
				Transform transform;
				if (changeRoot != null)
				{
					transform = NGUITools.AddChild(changeRoot.gameObject, itemPrefab).transform;
				}
				else
				{
					transform = NGUITools.AddChild(base.gameObject, itemPrefab).transform;
				}
				heroItem = base.CreateInstance<HeroItem>(transform);
				this.AddChild(heroItem);
			}
			else
			{
				heroItem = base.GetChild(i);
				heroItem.SetActive(true);
			}
			heroItem.parent = base.transform;
			num++;
		}
		for (int j = num; j < base.childCount; j++)
		{
			HeroItem child = base.GetChild(j);
			child.SetActive(false);
		}
		base.OnCreateChildView(dataCount, num);
	}
}
