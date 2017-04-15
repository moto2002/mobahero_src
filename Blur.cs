using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Blur/Blur (Optimized)"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
[Serializable]
public class Blur : PostEffectsBase
{
	[Serializable]
	public enum BlurType
	{
		StandardGauss,
		SgxGauss
	}

	[Range(0f, 2f)]
	public int downsample;

	[Range(0f, 10f)]
	public float blurSize;

	[Range(1f, 4f)]
	public int blurIterations;

	public Blur.BlurType blurType;

	public Shader blurShader;

	private Material blurMaterial;

	public Blur()
	{
		this.downsample = 1;
		this.blurSize = 3f;
		this.blurIterations = 2;
		this.blurType = Blur.BlurType.StandardGauss;
	}

	public override bool CheckResources()
	{
		this.CheckSupport(false);
		this.blurMaterial = this.CheckShaderAndCreateMaterial(this.blurShader, this.blurMaterial);
		if (!this.isSupported)
		{
			this.ReportAutoDisable();
		}
		return this.isSupported;
	}

	public override void OnDisable()
	{
		if (this.blurMaterial)
		{
			UnityEngine.Object.DestroyImmediate(this.blurMaterial);
		}
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.CheckResources())
		{
			Graphics.Blit(source, destination);
		}
		else
		{
			float num = 1f / (1f * (float)(1 << this.downsample));
			this.blurMaterial.SetVector("_Parameter", new Vector4(this.blurSize * num, -this.blurSize * num, (float)0, (float)0));
			source.filterMode = FilterMode.Bilinear;
			int width = source.width >> this.downsample;
			int height = source.height >> this.downsample;
			RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
			renderTexture.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, renderTexture, this.blurMaterial, 0);
			int num2 = (this.blurType != Blur.BlurType.StandardGauss) ? 2 : 0;
			for (int i = 0; i < this.blurIterations; i++)
			{
				float num3 = (float)i * 1f;
				this.blurMaterial.SetVector("_Parameter", new Vector4(this.blurSize * num + num3, -this.blurSize * num - num3, (float)0, (float)0));
				RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, this.blurMaterial, 1 + num2);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
				temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, this.blurMaterial, 2 + num2);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
			}
			Graphics.Blit(renderTexture, destination);
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}

	public override void Main()
	{
	}
}
