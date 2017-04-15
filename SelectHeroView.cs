using System;
using UnityEngine;

public class SelectHeroView : CompositeView<SelectHeroItem>
{
	public SelectHeroView(Transform root) : base(root)
	{
	}

	public override SelectHeroItem GetChild(Transform root)
	{
		for (int i = 0; i < base.childList.Count; i++)
		{
			SelectHeroItem selectHeroItem = base.childList[i];
			if (selectHeroItem.transform == root)
			{
				return base.childList[i];
			}
		}
		return null;
	}

	public override SelectHeroItem GetChild(string name)
	{
		for (int i = 0; i < base.childList.Count; i++)
		{
			SelectHeroItem selectHeroItem = base.childList[i];
			if (selectHeroItem.name.Equals(name))
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
			SelectHeroItem selectHeroItem;
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
				selectHeroItem = base.CreateInstance<SelectHeroItem>(transform);
				this.AddChild(selectHeroItem);
			}
			else
			{
				selectHeroItem = base.GetChild(i);
				selectHeroItem.SetActive(true);
			}
			selectHeroItem.parent = base.transform;
			num++;
		}
		for (int j = num; j < base.childCount; j++)
		{
			SelectHeroItem child = base.GetChild(j);
			child.SetActive(false);
		}
		base.OnCreateChildView(dataCount, num);
	}
}
