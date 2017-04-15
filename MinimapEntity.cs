using Com.Game.Module;
using System;
using UnityEngine;

public class MinimapEntity : MonoBehaviour
{
	public Texture texture;

	private MinimapTracer _tracer;

	private void OnEnable()
	{
		if (this._tracer == null)
		{
			GameObject gameObject = new GameObject("test");
			UITexture uITexture = gameObject.AddComponent<UITexture>();
			uITexture.mainTexture = this.texture;
			this._tracer = MinimapTracer.CreateTracer(base.gameObject, gameObject.gameObject);
		}
		MinimapTracer.Add(this._tracer);
	}

	private void OnDisable()
	{
		MinimapTracer.Remove(this._tracer);
	}
}
