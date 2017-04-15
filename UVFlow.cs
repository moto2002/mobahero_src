using System;
using UnityEngine;

public class UVFlow : MonoBehaviour
{
	public float xSpeed = 0.1f;

	public float ySpeed = 0.1f;

	private float xCur;

	private float yCur;

	private float xScale = 1f;

	private float yScale = 1f;

	public Material mat;

	private void Update()
	{
		if (this.mat == null)
		{
			return;
		}
		this.xCur += this.xSpeed;
		this.yCur += this.ySpeed;
		this.mat.SetTextureOffset("_MainTex", new Vector2(this.xCur, this.yCur));
		this.mat.SetTextureScale("_MainTex", new Vector2(this.xScale, this.yScale));
	}
}
