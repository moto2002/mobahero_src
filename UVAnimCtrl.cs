using System;
using UnityEngine;

public class UVAnimCtrl : MonoBehaviour
{
	public bool loop = true;

	public float scrollSpeed = 5f;

	public float countX = 4f;

	public float countY = 4f;

	private float offsetX;

	private float offsetY;

	private Vector2 singleTextureSize;

	private float _frame;

	private float stepx;

	private float stepy;

	public bool isOffset;

	public float ox;

	public float oy;

	private bool _isEnable = true;

	public bool isEnable
	{
		get
		{
			return this._isEnable;
		}
		set
		{
			this._isEnable = value;
			if (value)
			{
				base.enabled = true;
			}
		}
	}

	private void Awake()
	{
		this.singleTextureSize = new Vector2(1f / this.countX, 1f / this.countY);
		base.renderer.material.mainTextureScale = this.singleTextureSize;
		Material[] materials = base.renderer.materials;
		for (int i = 0; i < materials.Length; i++)
		{
			Material material = materials[i];
			material.mainTextureScale = this.singleTextureSize;
		}
	}

	private void Update()
	{
		if (this._isEnable)
		{
			this._OnUpdate();
		}
		else
		{
			base.enabled = false;
		}
	}

	public void UpdateFrame()
	{
		this._OnUpdate();
	}

	private void _OnUpdate()
	{
		if (this.isOffset)
		{
			if (this.loop)
			{
				if (this.offsetX < -1f)
				{
					this.offsetX += 1f;
				}
				if (this.offsetY < -1f)
				{
					this.offsetY += 1f;
				}
				if (this.offsetX > 1f)
				{
					this.offsetX -= 1f;
				}
				if (this.offsetY > 1f)
				{
					this.offsetY -= 1f;
				}
			}
			else
			{
				if (this.offsetX < -1f)
				{
					return;
				}
				if (this.offsetY < -1f)
				{
					return;
				}
				if (this.offsetX > 1f)
				{
					return;
				}
				if (this.offsetY > 1f)
				{
					return;
				}
			}
			this.offsetX += this.ox;
			this.offsetY += this.oy;
		}
		else
		{
			if (this.stepy >= this.countY)
			{
				if (!this.loop)
				{
					return;
				}
				this.stepy = 0f;
			}
			this._frame += Time.deltaTime;
			this.stepx = Mathf.Floor(this._frame * this.scrollSpeed);
			this.offsetX = this.stepx / this.countX;
			if (this.stepx >= this.countX)
			{
				this.stepy += 1f;
				this._frame = 0f;
			}
			this.offsetY = this.stepy / this.countY;
		}
		Material[] materials = base.renderer.materials;
		for (int i = 0; i < materials.Length; i++)
		{
			Material material = materials[i];
			material.SetTextureOffset("_MainTex", new Vector2(this.offsetX, this.offsetY));
		}
	}
}
