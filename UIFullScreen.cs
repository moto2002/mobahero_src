using Com.Game.Module;
using System;
using UnityEngine;

public class UIFullScreen : MonoBehaviour
{
	public UISprite sp;

	public UITexture tex;

	public int addWidth;

	public int addHeight;

	private void Awake()
	{
		if (this.sp == null)
		{
			this.sp = base.gameObject.GetComponent<UISprite>();
		}
		if (this.sp == null)
		{
			this.tex = base.gameObject.GetComponent<UITexture>();
		}
		if (this.sp != null)
		{
			this.sp.width = (int)ViewTree.anchorCamera.gameViewPixelWidth + this.addWidth;
			this.sp.height = (int)ViewTree.anchorCamera.gameViewPixelHeight + this.addHeight;
		}
		if (this.tex != null)
		{
			this.tex.width = (int)ViewTree.anchorCamera.gameViewPixelWidth + this.addWidth;
			this.tex.height = (int)ViewTree.anchorCamera.gameViewPixelHeight + this.addHeight;
		}
	}
}
