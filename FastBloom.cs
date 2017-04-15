using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Bloom and Glow/Bloom (Optimized)"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
[Serializable]
public class FastBloom : PostEffectsBase
{
	[Serializable]
	public enum Resolution
	{
		Low,
		High
	}

	[Serializable]
	public enum BlurType
	{
		Standard,
		Sgx
	}

	[Range(0f, 1.5f)]
	public float threshhold;

	[Range(0f, 2.5f)]
	public float intensity;

	[Range(0.25f, 5.5f)]
	public float blurSize;

	public FastBloom.Resolution resolution;

	[Range(1f, 4f)]
	public int blurIterations;

	public FastBloom.BlurType blurType;

	public Shader fastBloomShader;

	private Material fastBloomMaterial;

	public FastBloom()
	{
		this.threshhold = 0.25f;
		this.intensity = 0.75f;
		this.blurSize = 1f;
		this.resolution = FastBloom.Resolution.Low;
		this.blurIterations = 1;
		this.blurType = FastBloom.BlurType.Standard;
	}

	public override bool CheckResources()
	{
		this.CheckSupport(false);
		this.fastBloomMaterial = this.CheckShaderAndCreateMaterial(this.fastBloomShader, this.fastBloomMaterial);
		if (!this.isSupported)
		{
			this.ReportAutoDisable();
		}
		return this.isSupported;
	}

	public override void OnDisable()
	{
		if (this.fastBloomMaterial)
		{
			UnityEngine.Object.DestroyImmediate(this.fastBloomMaterial);
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
			int num = (this.resolution != FastBloom.Resolution.Low) ? 2 : 4;
			float num2 = (this.resolution != FastBloom.Resolution.Low) ? 1f : 0.5f;
			this.fastBloomMaterial.SetVector("_Parameter", new Vector4(this.blurSize * num2, (float)0, this.threshhold, this.intensity));
			source.filterMode = FilterMode.Bilinear;
			int width = source.width / num;
			int height = source.height / num;
			RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
			renderTexture.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, renderTexture, this.fastBloomMaterial, 1);
			int num3 = (this.blurType != FastBloom.BlurType.Standard) ? 2 : 0;
			for (int i = 0; i < this.blurIterations; i++)
			{
				this.fastBloomMaterial.SetVector("_Parameter", new Vector4(this.blurSize * num2 + (float)i * 1f, (float)0, this.threshhold, this.intensity));
				RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, this.fastBloomMaterial, 2 + num3);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
				temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, this.fastBloomMaterial, 3 + num3);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
			}
			this.fastBloomMaterial.SetTexture("_Bloom", renderTexture);
			Graphics.Blit(source, destination, this.fastBloomMaterial, 0);
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}

	public override void Main()
	{
	}
}
