using System;
using UnityEngine;

[Serializable]
public class UVdh : MonoBehaviour
{
	public int scrollSpeed;

	public int countX;

	public int countY;

	public bool _isHasDetailTex;

	private float offsetX;

	private float offsetY;

	private object singleTexSize;

	public UVdh()
	{
		this.scrollSpeed = 5;
		this.countX = 4;
		this.countY = 4;
	}

	public override void Start()
	{
		this.singleTexSize = new Vector2(1f / (float)this.countX, 1f / (float)this.countY);
		this.renderer.material.mainTextureScale = (Vector2)this.singleTexSize;
		if (this.renderer.material.HasProperty("_DetailTex"))
		{
			this._isHasDetailTex = true;
		}
	}

	public override void Update()
	{
		float num = Mathf.Floor(Time.time * (float)this.scrollSpeed);
		this.offsetX = num / (float)this.countX;
		this.offsetY = -(num - num % (float)this.countX) / (float)this.countY / (float)this.countX;
		float x = this.offsetX - Mathf.Floor(this.offsetX);
		float y = this.offsetY - Mathf.Floor(this.offsetY);
		this.renderer.material.SetTextureOffset("_MainTex", new Vector2(x, y));
		if (this._isHasDetailTex)
		{
			this.renderer.material.SetTextureOffset("_DetailTex", new Vector2(x, y));
		}
	}

	public override void Main()
	{
	}
}
