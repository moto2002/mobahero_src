using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Color Adjustments/Grayscale"), ExecuteInEditMode]
public class GrayscaleEffect : ImageEffectBase
{
	public Texture textureRamp;

	public float rampOffset;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		base.material.SetTexture("_RampTex", this.textureRamp);
		base.material.SetFloat("_RampOffset", this.rampOffset);
		Graphics.Blit(source, destination, base.material);
	}
}
