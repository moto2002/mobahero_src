using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Color Adjustments/Color Correction (Curves, Saturation)"), ExecuteInEditMode]
[Serializable]
public class ColorCorrectionCurves : PostEffectsBase
{
	public AnimationCurve redChannel;

	public AnimationCurve greenChannel;

	public AnimationCurve blueChannel;

	public bool useDepthCorrection;

	public AnimationCurve zCurve;

	public AnimationCurve depthRedChannel;

	public AnimationCurve depthGreenChannel;

	public AnimationCurve depthBlueChannel;

	private Material ccMaterial;

	private Material ccDepthMaterial;

	private Material selectiveCcMaterial;

	private Texture2D rgbChannelTex;

	private Texture2D rgbDepthChannelTex;

	private Texture2D zCurveTex;

	public float saturation;

	public bool selectiveCc;

	public Color selectiveFromColor;

	public Color selectiveToColor;

	public ColorCorrectionMode mode;

	public bool updateTextures;

	public Shader colorCorrectionCurvesShader;

	public Shader simpleColorCorrectionCurvesShader;

	public Shader colorCorrectionSelectiveShader;

	private bool updateTexturesOnStartup;

	public ColorCorrectionCurves()
	{
		this.saturation = 1f;
		this.selectiveFromColor = Color.white;
		this.selectiveToColor = Color.white;
		this.updateTextures = true;
		this.updateTexturesOnStartup = true;
	}

	public override void Start()
	{
		base.Start();
		this.updateTexturesOnStartup = true;
	}

	public override void Awake()
	{
	}

	public override bool CheckResources()
	{
		this.CheckSupport(this.mode == ColorCorrectionMode.Advanced);
		this.ccMaterial = this.CheckShaderAndCreateMaterial(this.simpleColorCorrectionCurvesShader, this.ccMaterial);
		this.ccDepthMaterial = this.CheckShaderAndCreateMaterial(this.colorCorrectionCurvesShader, this.ccDepthMaterial);
		this.selectiveCcMaterial = this.CheckShaderAndCreateMaterial(this.colorCorrectionSelectiveShader, this.selectiveCcMaterial);
		if (!this.rgbChannelTex)
		{
			this.rgbChannelTex = new Texture2D(256, 4, TextureFormat.ARGB32, false, true);
		}
		if (!this.rgbDepthChannelTex)
		{
			this.rgbDepthChannelTex = new Texture2D(256, 4, TextureFormat.ARGB32, false, true);
		}
		if (!this.zCurveTex)
		{
			this.zCurveTex = new Texture2D(256, 1, TextureFormat.ARGB32, false, true);
		}
		this.rgbChannelTex.hideFlags = HideFlags.DontSave;
		this.rgbDepthChannelTex.hideFlags = HideFlags.DontSave;
		this.zCurveTex.hideFlags = HideFlags.DontSave;
		this.rgbChannelTex.wrapMode = TextureWrapMode.Clamp;
		this.rgbDepthChannelTex.wrapMode = TextureWrapMode.Clamp;
		this.zCurveTex.wrapMode = TextureWrapMode.Clamp;
		if (!this.isSupported)
		{
			this.ReportAutoDisable();
		}
		return this.isSupported;
	}

	public override void UpdateParameters()
	{
		this.CheckResources();
		if (this.redChannel != null && this.greenChannel != null && this.blueChannel != null)
		{
			for (float num = (float)0; num <= 1f; num += 0.003921569f)
			{
				float num2 = Mathf.Clamp(this.redChannel.Evaluate(num), (float)0, 1f);
				float num3 = Mathf.Clamp(this.greenChannel.Evaluate(num), (float)0, 1f);
				float num4 = Mathf.Clamp(this.blueChannel.Evaluate(num), (float)0, 1f);
				this.rgbChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 0, new Color(num2, num2, num2));
				this.rgbChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 1, new Color(num3, num3, num3));
				this.rgbChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 2, new Color(num4, num4, num4));
				float num5 = Mathf.Clamp(this.zCurve.Evaluate(num), (float)0, 1f);
				this.zCurveTex.SetPixel((int)Mathf.Floor(num * 255f), 0, new Color(num5, num5, num5));
				num2 = Mathf.Clamp(this.depthRedChannel.Evaluate(num), (float)0, 1f);
				num3 = Mathf.Clamp(this.depthGreenChannel.Evaluate(num), (float)0, 1f);
				num4 = Mathf.Clamp(this.depthBlueChannel.Evaluate(num), (float)0, 1f);
				this.rgbDepthChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 0, new Color(num2, num2, num2));
				this.rgbDepthChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 1, new Color(num3, num3, num3));
				this.rgbDepthChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 2, new Color(num4, num4, num4));
			}
			this.rgbChannelTex.Apply();
			this.rgbDepthChannelTex.Apply();
			this.zCurveTex.Apply();
		}
	}

	public override void UpdateTextures()
	{
		this.UpdateParameters();
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.CheckResources())
		{
			Graphics.Blit(source, destination);
		}
		else
		{
			if (this.updateTexturesOnStartup)
			{
				this.UpdateParameters();
				this.updateTexturesOnStartup = false;
			}
			if (this.useDepthCorrection)
			{
				this.camera.depthTextureMode = (this.camera.depthTextureMode | DepthTextureMode.Depth);
			}
			RenderTexture renderTexture = destination;
			if (this.selectiveCc)
			{
				renderTexture = RenderTexture.GetTemporary(source.width, source.height);
			}
			if (this.useDepthCorrection)
			{
				this.ccDepthMaterial.SetTexture("_RgbTex", this.rgbChannelTex);
				this.ccDepthMaterial.SetTexture("_ZCurve", this.zCurveTex);
				this.ccDepthMaterial.SetTexture("_RgbDepthTex", this.rgbDepthChannelTex);
				this.ccDepthMaterial.SetFloat("_Saturation", this.saturation);
				Graphics.Blit(source, renderTexture, this.ccDepthMaterial);
			}
			else
			{
				this.ccMaterial.SetTexture("_RgbTex", this.rgbChannelTex);
				this.ccMaterial.SetFloat("_Saturation", this.saturation);
				Graphics.Blit(source, renderTexture, this.ccMaterial);
			}
			if (this.selectiveCc)
			{
				this.selectiveCcMaterial.SetColor("selColor", this.selectiveFromColor);
				this.selectiveCcMaterial.SetColor("targetColor", this.selectiveToColor);
				Graphics.Blit(renderTexture, destination, this.selectiveCcMaterial);
				RenderTexture.ReleaseTemporary(renderTexture);
			}
		}
	}

	public override void Main()
	{
	}
}
