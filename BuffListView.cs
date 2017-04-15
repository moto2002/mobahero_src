using System;
using UnityEngine;

public class BuffListView : CompositeView<BuffItem>
{
	public Transform LevelView;

	public UILabel ChapterTitle;

	private Vector3 originPosition;

	public BuffListView(Transform root) : base(root)
	{
		base.gridView = base.transform.GetComponent<UIGrid>();
	}

	public override void CreateChildView(int dataCount, int itemCount, GameObject itemPrefab, Transform changeRoot = null)
	{
		if (base.transform == null)
		{
			return;
		}
		if (dataCount < itemCount)
		{
			itemCount = dataCount;
		}
		int num = 0;
		for (int i = 0; i < itemCount; i++)
		{
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
				BuffItem buffItem = transform.GetComponent<BuffItem>();
				this.AddChild(buffItem);
			}
			else
			{
				BuffItem buffItem = base.GetChild(i);
				if (buffItem != null)
				{
					buffItem.SetActive(true);
				}
			}
			num++;
		}
		for (int j = num; j < base.childCount; j++)
		{
			BuffItem child = base.GetChild(j);
			if (child != null)
			{
				child.SetActive(false);
			}
		}
		base.OnCreateChildView(dataCount, num);
	}

	public override BuffItem GetChild(Transform root)
	{
		for (int i = 0; i < base.childList.Count; i++)
		{
			BuffItem buffItem = base.childList[i];
			if (buffItem.transform == root)
			{
				return base.childList[i];
			}
		}
		return null;
	}

	public override BuffItem GetChild(string name)
	{
		for (int i = 0; i < base.childList.Count; i++)
		{
			BuffItem buffItem = base.childList[i];
			if (buffItem.name.Equals(name))
			{
				return base.childList[i];
			}
		}
		return null;
	}
}
