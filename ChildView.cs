using System;
using UnityEngine;

public class ChildView : CompositeView<ChildView>
{
	public ChildView()
	{
	}

	public ChildView(Transform root) : base(root)
	{
	}

	public override ChildView GetChild(Transform root)
	{
		throw new NotImplementedException();
	}

	public override ChildView GetChild(string name)
	{
		throw new NotImplementedException();
	}

	public override void CreateChildView(int dataCount, int itemCount, GameObject itemPrefab, Transform changeRoot = null)
	{
		throw new NotImplementedException();
	}
}
