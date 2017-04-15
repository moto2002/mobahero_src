using System;
using UnityEngine;

public class UVdhC : MonoBehaviour
{
	public float scrollSpeed = 5f;

	public int countX = 4;

	public int countY = 4;

	private Vector2 offsetVector;

	private Vector2 singleTexSize;

	private float frame;

	private System.Random random;

	private bool _isHaveDetailTex;

	private void Start()
	{
		this.offsetVector = Vector2.zero;
		this.singleTexSize = new Vector2(1f / (float)this.countX, 1f / (float)this.countY);
		base.renderer.material.mainTextureScale = this.singleTexSize;
		if (base.renderer.material.HasProperty("_DetailTex"))
		{
			this._isHaveDetailTex = true;
			base.renderer.material.SetTextureScale("_DetailTex", this.singleTexSize);
		}
		this.random = new System.Random();
	}

	private void Update()
	{
		this.frame = Mathf.Floor((float)this.random.Next(1, 100) * 0.01f * this.scrollSpeed);
		this.offsetVector.x = this.frame / (float)this.countX;
		this.offsetVector.y = -((this.frame - this.frame % (float)this.countX) / (float)this.countY) / (float)this.countX;
		base.renderer.material.SetTextureOffset("_MainTex", this.offsetVector);
		if (this._isHaveDetailTex)
		{
			base.renderer.material.SetTextureOffset("_DetailTex", this.offsetVector);
		}
	}
}
