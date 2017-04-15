using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Camera/Tilt Shift (Lens Blur)"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
[Serializable]
public class TiltShiftHdr : PostEffectsBase
{
	[Serializable]
	public enum TiltShiftMode
	{
		TiltShiftMode,
		IrisMode
	}

	[Serializable]
	public enum TiltShiftQuality
	{
		Preview,
		Normal,
		High
	}

	public TiltShiftHdr.TiltShiftMode mode;

	public TiltShiftHdr.TiltShiftQuality quality;

	[Range(0f, 15f)]
	public float blurArea;

	[Range(0f, 25f)]
	public float maxBlurSize;

	[Range(0f, 1f)]
	public int downsample;

	public Shader tiltShiftShader;

	private Material tiltShiftMaterial;

	public TiltShiftHdr()
	{
		this.mode = TiltShiftHdr.TiltShiftMode.TiltShiftMode;
		this.quality = TiltShiftHdr.TiltShiftQuality.Normal;
		this.blurArea = 1f;
		this.maxBlurSize = 5f;
	}

	public override bool CheckResources()
	{
		this.CheckSupport(true);
		this.tiltShiftMaterial = this.CheckShaderAndCreateMaterial(this.tiltShiftShader, this.tiltShiftMaterial);
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
			this.tiltShiftMaterial.SetFloat("_BlurSize", (this.maxBlurSize >= (float)0) ? this.maxBlurSize : ((float)0));
			this.tiltShiftMaterial.SetFloat("_BlurArea", this.blurArea);
			source.filterMode = FilterMode.Bilinear;
			RenderTexture renderTexture = destination;
			if (this.downsample != 0)
			{
				renderTexture = RenderTexture.GetTemporary(source.width >> this.downsample, source.height >> this.downsample, 0, source.format);
				renderTexture.filterMode = FilterMode.Bilinear;
			}
			int num = (int)this.quality;
			num *= 2;
			Graphics.Blit(source, renderTexture, this.tiltShiftMaterial, (this.mode != TiltShiftHdr.TiltShiftMode.TiltShiftMode) ? (num + 1) : num);
			if (this.downsample != 0)
			{
				this.tiltShiftMaterial.SetTexture("_Blurred", renderTexture);
				Graphics.Blit(source, destination, this.tiltShiftMaterial, 6);
			}
			if (renderTexture != destination)
			{
				RenderTexture.ReleaseTemporary(renderTexture);
			}
		}
	}

	public override void Main()
	{
	}
}
