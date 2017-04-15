using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Color Adjustments/Sepia Tone"), ExecuteInEditMode]
public class SepiaToneEffect : ImageEffectBase
{
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, base.material);
	}
}
