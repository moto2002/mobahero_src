using System;
using UnityEngine;

public class UVScroller : MonoBehaviour
{
	public float scrollSpeedX = 0.1f;

	public float scrollSpeedY = 0.1f;

	private Renderer myRenderer;

	private bool _isHaveDetailTex;

	private void OnEnable()
	{
		this.myRenderer = base.renderer;
		if (!this.myRenderer)
		{
			base.enabled = false;
		}
		if (this.myRenderer != null && this.myRenderer.material != null)
		{
			this._isHaveDetailTex = this.myRenderer.material.HasProperty("_DetailTex");
		}
	}

	private void Update()
	{
		float num = Time.time * this.scrollSpeedX;
		num -= (float)((int)num);
		float num2 = Time.time * this.scrollSpeedY;
		num2 -= (float)((int)num2);
		if (this.myRenderer)
		{
			this.myRenderer.material.SetTextureOffset("_MainTex", new Vector2(num, num2));
		}
		if (this._isHaveDetailTex)
		{
			this.myRenderer.material.SetTextureOffset("_DetailTex", new Vector2(num, num2));
		}
	}
}
