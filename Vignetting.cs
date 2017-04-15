using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Camera/Vignette and Chromatic Aberration"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
[Serializable]
public class Vignetting : PostEffectsBase
{
	[Serializable]
	public enum AberrationMode
	{
		Simple,
		Advanced
	}

	public Vignetting.AberrationMode mode;

	public float intensity;

	public float chromaticAberration;

	public float axialAberration;

	public float blur;

	public float blurSpread;

	public float luminanceDependency;

	public float blurDistance;

	public Shader vignetteShader;

	private Material vignetteMaterial;

	public Shader separableBlurShader;

	private Material separableBlurMaterial;

	public Shader chromAberrationShader;

	private Material chromAberrationMaterial;

	public Vignetting()
	{
		this.mode = Vignetting.AberrationMode.Simple;
		this.intensity = 0.375f;
		this.chromaticAberration = 0.2f;
		this.axialAberration = 0.5f;
		this.blurSpread = 0.75f;
		this.luminanceDependency = 0.25f;
		this.blurDistance = 2.5f;
	}

	public override bool CheckResources()
	{
		this.CheckSupport(false);
		this.vignetteMaterial = this.CheckShaderAndCreateMaterial(this.vignetteShader, this.vignetteMaterial);
		this.separableBlurMaterial = this.CheckShaderAndCreateMaterial(this.separableBlurShader, this.separableBlurMaterial);
		this.chromAberrationMaterial = this.CheckShaderAndCreateMaterial(this.chromAberrationShader, this.chromAberrationMaterial);
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
			bool flag = (Mathf.Abs(this.blur) > (float)0) ?? (Mathf.Abs(this.intensity) > (float)0);
			float num = 1f * (float)width / (1f * (float)height);
			float num2 = 0.001953125f;
			RenderTexture renderTexture = null;
			RenderTexture renderTexture2 = null;
			if (flag)
			{
				renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
				if (Mathf.Abs(this.blur) > (float)0)
				{
					renderTexture2 = RenderTexture.GetTemporary(width / 2, height / 2, 0, source.format);
					Graphics.Blit(source, renderTexture2, this.chromAberrationMaterial, 0);
					for (int i = 0; i < 2; i++)
					{
						this.separableBlurMaterial.SetVector("offsets", new Vector4((float)0, this.blurSpread * num2, (float)0, (float)0));
						RenderTexture temporary = RenderTexture.GetTemporary(width / 2, height / 2, 0, source.format);
						Graphics.Blit(renderTexture2, temporary, this.separableBlurMaterial);
						RenderTexture.ReleaseTemporary(renderTexture2);
						this.separableBlurMaterial.SetVector("offsets", new Vector4(this.blurSpread * num2 / num, (float)0, (float)0, (float)0));
						renderTexture2 = RenderTexture.GetTemporary(width / 2, height / 2, 0, source.format);
						Graphics.Blit(temporary, renderTexture2, this.separableBlurMaterial);
						RenderTexture.ReleaseTemporary(temporary);
					}
				}
				this.vignetteMaterial.SetFloat("_Intensity", this.intensity);
				this.vignetteMaterial.SetFloat("_Blur", this.blur);
				this.vignetteMaterial.SetTexture("_VignetteTex", renderTexture2);
				Graphics.Blit(source, renderTexture, this.vignetteMaterial, 0);
			}
			this.chromAberrationMaterial.SetFloat("_ChromaticAberration", this.chromaticAberration);
			this.chromAberrationMaterial.SetFloat("_AxialAberration", this.axialAberration);
			this.chromAberrationMaterial.SetVector("_BlurDistance", new Vector2(-this.blurDistance, this.blurDistance));
			this.chromAberrationMaterial.SetFloat("_Luminance", 1f / Mathf.Max(1.401298E-45f, this.luminanceDependency));
			if (flag)
			{
				renderTexture.wrapMode = TextureWrapMode.Clamp;
			}
			else
			{
				source.wrapMode = TextureWrapMode.Clamp;
			}
			Graphics.Blit((!flag) ? source : renderTexture, destination, this.chromAberrationMaterial, (this.mode != Vignetting.AberrationMode.Advanced) ? 1 : 2);
			RenderTexture.ReleaseTemporary(renderTexture);
			RenderTexture.ReleaseTemporary(renderTexture2);
		}
	}

	public override void Main()
	{
	}
}
