using System;
using UnityEngine;

[Serializable]
public class AnimationUV : MonoBehaviour
{
	public int uvAnimationTileX;

	public int uvAnimationTileY;

	public float framesPerSecond;

	public bool loop;

	public bool play;

	private int index;

	private float offsettime;

	public bool Hidewhenstopplaying;

	public AnimationUV()
	{
		this.uvAnimationTileX = 24;
		this.uvAnimationTileY = 1;
		this.framesPerSecond = 10f;
		this.play = true;
	}

	public override void Start()
	{
		this.offsettime = Time.time;
	}

	public override void Update()
	{
		this.index = (int)((Time.time - this.offsettime) * this.framesPerSecond);
		if (this.play)
		{
			this.index %= this.uvAnimationTileX * this.uvAnimationTileY;
			Vector2 scale = new Vector2(1f / (float)this.uvAnimationTileX, 1f / (float)this.uvAnimationTileY);
			int num = this.index % this.uvAnimationTileX;
			int num2 = this.index / this.uvAnimationTileX;
			Vector2 offset = new Vector2((float)num * scale.x, 1f - scale.y - (float)num2 * scale.y);
			this.renderer.material.SetTextureOffset("_MainTex", offset);
			this.renderer.material.SetTextureScale("_MainTex", scale);
		}
		if (!this.loop && this.index >= this.uvAnimationTileX * this.uvAnimationTileY - 1)
		{
			this.play = false;
			if (this.Hidewhenstopplaying)
			{
				this.renderer.active = false;
			}
		}
	}

	public override void Main()
	{
	}
}
