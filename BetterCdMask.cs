using System;
using UnityEngine;

public class BetterCdMask : MonoBehaviour
{
	public UISprite mask;

	public UILabel label;

	public UIPanel newParent;

	public UIWidget RefWidget;

	private Transform _originParent;

	private void Awake()
	{
		this._originParent = base.transform.parent;
		if (this.newParent)
		{
			base.transform.parent = this.newParent.transform;
			this.Update();
		}
	}

	private void Update()
	{
		if (null != this.newParent && null != this.RefWidget && null != base.transform && null != this._originParent)
		{
			this.newParent.depth = this.RefWidget.panel.depth + 10;
			base.transform.position = this._originParent.position;
		}
	}
}
