using System;
using UnityEngine;

public class SpriteRendererAlpha : UIRect
{
	private Vector3[] mCorners = new Vector3[1];

	public new float finalAlpha = 1f;

	private Color color = new Color(1f, 1f, 1f, 1f);

	public override float alpha
	{
		get
		{
			return this.color.a;
		}
		set
		{
			this.color = new Color(1f, 1f, 1f, value);
			base.GetComponent<SpriteRenderer>().color = this.color;
		}
	}

	public override Vector3[] localCorners
	{
		get
		{
			this.mCorners[0] = new Vector3(0f, 0f, 0f);
			return this.mCorners;
		}
	}

	public override Vector3[] worldCorners
	{
		get
		{
			this.mCorners[0] = new Vector3(0f, 0f, 0f);
			return this.mCorners;
		}
	}

	public override float CalculateFinalAlpha(int frameID)
	{
		return 0f;
	}

	protected override void OnAnchor()
	{
	}

	protected override void OnStart()
	{
	}
}
