using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Other/Screen Overlay"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
[Serializable]
public class ScreenOverlay : PostEffectsBase
{
	[Serializable]
	public enum OverlayBlendMode
	{
		Additive,
		ScreenBlend,
		Multiply,
		Overlay,
		AlphaBlend
	}

	public ScreenOverlay.OverlayBlendMode blendMode;

	public float intensity;

	public Texture2D texture;

	public Shader overlayShader;

	private Material overlayMaterial;

	public ScreenOverlay()
	{
		this.blendMode = ScreenOverlay.OverlayBlendMode.Overlay;
		this.intensity = 1f;
	}

	public override bool CheckResources()
	{
		this.CheckSupport(false);
		this.overlayMaterial = this.CheckShaderAndCreateMaterial(this.overlayShader, this.overlayMaterial);
		if (!this.isSupported)
		{
			this.ReportAutoDisable();
		}
		return this.isSupported;
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.CheckResources())
		{
			Graphics.Blit(source, destination);
		}
		else
		{
			Vector4 vector = new Vector4((float)1, (float)0, (float)0, (float)1);
			this.overlayMaterial.SetVector("_UV_Transform", vector);
			this.overlayMaterial.SetFloat("_Intensity", this.intensity);
			this.overlayMaterial.SetTexture("_Overlay", this.texture);
			Graphics.Blit(source, destination, this.overlayMaterial, (int)this.blendMode);
		}
	}

	public override void Main()
	{
	}
}
