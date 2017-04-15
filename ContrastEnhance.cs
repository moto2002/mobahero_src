using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Color Adjustments/Contrast Enhance (Unsharp Mask)"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
[Serializable]
public class ContrastEnhance : PostEffectsBase
{
	public float intensity;

	public float threshhold;

	private Material separableBlurMaterial;

	private Material contrastCompositeMaterial;

	public float blurSpread;

	public Shader separableBlurShader;

	public Shader contrastCompositeShader;

	public ContrastEnhance()
	{
		this.intensity = 0.5f;
		this.blurSpread = 1f;
	}

	public override bool CheckResources()
	{
		this.CheckSupport(false);
		this.contrastCompositeMaterial = this.CheckShaderAndCreateMaterial(this.contrastCompositeShader, this.contrastCompositeMaterial);
		this.separableBlurMaterial = this.CheckShaderAndCreateMaterial(this.separableBlurShader, this.separableBlurMaterial);
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
			int width = source.width;
			int height = source.height;
			RenderTexture temporary = RenderTexture.GetTemporary(width / 2, height / 2, 0);
			Graphics.Blit(source, temporary);
			RenderTexture temporary2 = RenderTexture.GetTemporary(width / 4, height / 4, 0);
			Graphics.Blit(temporary, temporary2);
			RenderTexture.ReleaseTemporary(temporary);
			this.separableBlurMaterial.SetVector("offsets", new Vector4((float)0, this.blurSpread * 1f / (float)temporary2.height, (float)0, (float)0));
			RenderTexture temporary3 = RenderTexture.GetTemporary(width / 4, height / 4, 0);
			Graphics.Blit(temporary2, temporary3, this.separableBlurMaterial);
			RenderTexture.ReleaseTemporary(temporary2);
			this.separableBlurMaterial.SetVector("offsets", new Vector4(this.blurSpread * 1f / (float)temporary2.width, (float)0, (float)0, (float)0));
			temporary2 = RenderTexture.GetTemporary(width / 4, height / 4, 0);
			Graphics.Blit(temporary3, temporary2, this.separableBlurMaterial);
			RenderTexture.ReleaseTemporary(temporary3);
			this.contrastCompositeMaterial.SetTexture("_MainTexBlurred", temporary2);
			this.contrastCompositeMaterial.SetFloat("intensity", this.intensity);
			this.contrastCompositeMaterial.SetFloat("threshhold", this.threshhold);
			Graphics.Blit(source, destination, this.contrastCompositeMaterial);
			RenderTexture.ReleaseTemporary(temporary2);
		}
	}

	public override void Main()
	{
	}
}
