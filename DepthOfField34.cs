using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Camera/Depth of Field (deprecated)"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
[Serializable]
public class DepthOfField34 : PostEffectsBase
{
	[NonSerialized]
	private static int SMOOTH_DOWNSAMPLE_PASS = 6;

	[NonSerialized]
	private static float BOKEH_EXTRA_BLUR = 2f;

	public Dof34QualitySetting quality;

	public DofResolution resolution;

	public bool simpleTweakMode;

	public float focalPoint;

	public float smoothness;

	public float focalZDistance;

	public float focalZStartCurve;

	public float focalZEndCurve;

	private float focalStartCurve;

	private float focalEndCurve;

	private float focalDistance01;

	public Transform objectFocus;

	public float focalSize;

	public DofBlurriness bluriness;

	public float maxBlurSpread;

	public float foregroundBlurExtrude;

	public Shader dofBlurShader;

	private Material dofBlurMaterial;

	public Shader dofShader;

	private Material dofMaterial;

	public bool visualize;

	public BokehDestination bokehDestination;

	private float widthOverHeight;

	private float oneOverBaseSize;

	public bool bokeh;

	public bool bokehSupport;

	public Shader bokehShader;

	public Texture2D bokehTexture;

	public float bokehScale;

	public float bokehIntensity;

	public float bokehThreshholdContrast;

	public float bokehThreshholdLuminance;

	public int bokehDownsample;

	private Material bokehMaterial;

	private RenderTexture foregroundTexture;

	private RenderTexture mediumRezWorkTexture;

	private RenderTexture finalDefocus;

	private RenderTexture lowRezWorkTexture;

	private RenderTexture bokehSource;

	private RenderTexture bokehSource2;

	public DepthOfField34()
	{
		this.quality = Dof34QualitySetting.OnlyBackground;
		this.resolution = DofResolution.Low;
		this.simpleTweakMode = true;
		this.focalPoint = 1f;
		this.smoothness = 0.5f;
		this.focalZStartCurve = 1f;
		this.focalZEndCurve = 1f;
		this.focalStartCurve = 2f;
		this.focalEndCurve = 2f;
		this.focalDistance01 = 0.1f;
		this.bluriness = DofBlurriness.High;
		this.maxBlurSpread = 1.75f;
		this.foregroundBlurExtrude = 1.15f;
		this.bokehDestination = BokehDestination.Background;
		this.widthOverHeight = 1.25f;
		this.oneOverBaseSize = 0.001953125f;
		this.bokehSupport = true;
		this.bokehScale = 2.4f;
		this.bokehIntensity = 0.15f;
		this.bokehThreshholdContrast = 0.1f;
		this.bokehThreshholdLuminance = 0.55f;
		this.bokehDownsample = 1;
	}

	public override void CreateMaterials()
	{
		this.dofBlurMaterial = this.CheckShaderAndCreateMaterial(this.dofBlurShader, this.dofBlurMaterial);
		this.dofMaterial = this.CheckShaderAndCreateMaterial(this.dofShader, this.dofMaterial);
		this.bokehSupport = this.bokehShader.isSupported;
		if (this.bokeh && this.bokehSupport && this.bokehShader)
		{
			this.bokehMaterial = this.CheckShaderAndCreateMaterial(this.bokehShader, this.bokehMaterial);
		}
	}

	public override bool CheckResources()
	{
		this.CheckSupport(true);
		this.dofBlurMaterial = this.CheckShaderAndCreateMaterial(this.dofBlurShader, this.dofBlurMaterial);
		this.dofMaterial = this.CheckShaderAndCreateMaterial(this.dofShader, this.dofMaterial);
		this.bokehSupport = this.bokehShader.isSupported;
		if (this.bokeh && this.bokehSupport && this.bokehShader)
		{
			this.bokehMaterial = this.CheckShaderAndCreateMaterial(this.bokehShader, this.bokehMaterial);
		}
		if (!this.isSupported)
		{
			this.ReportAutoDisable();
		}
		return this.isSupported;
	}

	public override void OnDisable()
	{
		Quads.Cleanup();
	}

	public override void OnEnable()
	{
		this.camera.depthTextureMode = (this.camera.depthTextureMode | DepthTextureMode.Depth);
	}

	public override float FocalDistance01(float worldDist)
	{
		return this.camera.WorldToViewportPoint((worldDist - this.camera.nearClipPlane) * this.camera.transform.forward + this.camera.transform.position).z / (this.camera.farClipPlane - this.camera.nearClipPlane);
	}

	public override int GetDividerBasedOnQuality()
	{
		int result = 1;
		if (this.resolution == DofResolution.Medium)
		{
			result = 2;
		}
		else if (this.resolution == DofResolution.Low)
		{
			result = 2;
		}
		return result;
	}

	public override int GetLowResolutionDividerBasedOnQuality(int baseDivider)
	{
		int num = baseDivider;
		if (this.resolution == DofResolution.High)
		{
			num *= 2;
		}
		if (this.resolution == DofResolution.Low)
		{
			num *= 2;
		}
		return num;
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.CheckResources())
		{
			Graphics.Blit(source, destination);
		}
		else
		{
			if (this.smoothness < 0.1f)
			{
				this.smoothness = 0.1f;
			}
			bool arg_46_1;
			if (arg_46_1 = this.bokeh)
			{
				arg_46_1 = this.bokehSupport;
			}
			this.bokeh = arg_46_1;
			float num = (!this.bokeh) ? 1f : DepthOfField34.BOKEH_EXTRA_BLUR;
			bool flag = this.quality > Dof34QualitySetting.OnlyBackground;
			float num2 = this.focalSize / (this.camera.farClipPlane - this.camera.nearClipPlane);
			if (this.simpleTweakMode)
			{
				this.focalDistance01 = ((!this.objectFocus) ? this.FocalDistance01(this.focalPoint) : (this.camera.WorldToViewportPoint(this.objectFocus.position).z / this.camera.farClipPlane));
				this.focalStartCurve = this.focalDistance01 * this.smoothness;
				this.focalEndCurve = this.focalStartCurve;
				bool arg_12C_0;
				if (arg_12C_0 = flag)
				{
					arg_12C_0 = (this.focalPoint > this.camera.nearClipPlane + 1.401298E-45f);
				}
				flag = arg_12C_0;
			}
			else
			{
				if (this.objectFocus)
				{
					Vector3 vector = this.camera.WorldToViewportPoint(this.objectFocus.position);
					vector.z /= this.camera.farClipPlane;
					this.focalDistance01 = vector.z;
				}
				else
				{
					this.focalDistance01 = this.FocalDistance01(this.focalZDistance);
				}
				this.focalStartCurve = this.focalZStartCurve;
				this.focalEndCurve = this.focalZEndCurve;
				bool arg_1D0_0;
				if (arg_1D0_0 = flag)
				{
					arg_1D0_0 = (this.focalPoint > this.camera.nearClipPlane + 1.401298E-45f);
				}
				flag = arg_1D0_0;
			}
			this.widthOverHeight = 1f * (float)source.width / (1f * (float)source.height);
			this.oneOverBaseSize = 0.001953125f;
			this.dofMaterial.SetFloat("_ForegroundBlurExtrude", this.foregroundBlurExtrude);
			this.dofMaterial.SetVector("_CurveParams", new Vector4((!this.simpleTweakMode) ? this.focalStartCurve : (1f / this.focalStartCurve), (!this.simpleTweakMode) ? this.focalEndCurve : (1f / this.focalEndCurve), num2 * 0.5f, this.focalDistance01));
			this.dofMaterial.SetVector("_InvRenderTargetSize", new Vector4(1f / (1f * (float)source.width), 1f / (1f * (float)source.height), (float)0, (float)0));
			int dividerBasedOnQuality = this.GetDividerBasedOnQuality();
			int lowResolutionDividerBasedOnQuality = this.GetLowResolutionDividerBasedOnQuality(dividerBasedOnQuality);
			this.AllocateTextures(flag, source, dividerBasedOnQuality, lowResolutionDividerBasedOnQuality);
			Graphics.Blit(source, source, this.dofMaterial, 3);
			this.Downsample(source, this.mediumRezWorkTexture);
			this.Blur(this.mediumRezWorkTexture, this.mediumRezWorkTexture, DofBlurriness.Low, 4, this.maxBlurSpread);
			if (this.bokeh && (this.bokehDestination & BokehDestination.Background) != (BokehDestination)0)
			{
				this.dofMaterial.SetVector("_Threshhold", new Vector4(this.bokehThreshholdContrast, this.bokehThreshholdLuminance, 0.95f, (float)0));
				Graphics.Blit(this.mediumRezWorkTexture, this.bokehSource2, this.dofMaterial, 11);
				Graphics.Blit(this.mediumRezWorkTexture, this.lowRezWorkTexture);
				this.Blur(this.lowRezWorkTexture, this.lowRezWorkTexture, this.bluriness, 0, this.maxBlurSpread * num);
			}
			else
			{
				this.Downsample(this.mediumRezWorkTexture, this.lowRezWorkTexture);
				this.Blur(this.lowRezWorkTexture, this.lowRezWorkTexture, this.bluriness, 0, this.maxBlurSpread);
			}
			this.dofBlurMaterial.SetTexture("_TapLow", this.lowRezWorkTexture);
			this.dofBlurMaterial.SetTexture("_TapMedium", this.mediumRezWorkTexture);
			Graphics.Blit(null, this.finalDefocus, this.dofBlurMaterial, 3);
			if (this.bokeh && (this.bokehDestination & BokehDestination.Background) != (BokehDestination)0)
			{
				this.AddBokeh(this.bokehSource2, this.bokehSource, this.finalDefocus);
			}
			this.dofMaterial.SetTexture("_TapLowBackground", this.finalDefocus);
			this.dofMaterial.SetTexture("_TapMedium", this.mediumRezWorkTexture);
			Graphics.Blit(source, (!flag) ? destination : this.foregroundTexture, this.dofMaterial, (!this.visualize) ? 0 : 2);
			if (flag)
			{
				Graphics.Blit(this.foregroundTexture, source, this.dofMaterial, 5);
				this.Downsample(source, this.mediumRezWorkTexture);
				this.BlurFg(this.mediumRezWorkTexture, this.mediumRezWorkTexture, DofBlurriness.Low, 2, this.maxBlurSpread);
				if (this.bokeh && (this.bokehDestination & BokehDestination.Foreground) != (BokehDestination)0)
				{
					this.dofMaterial.SetVector("_Threshhold", new Vector4(this.bokehThreshholdContrast * 0.5f, this.bokehThreshholdLuminance, (float)0, (float)0));
					Graphics.Blit(this.mediumRezWorkTexture, this.bokehSource2, this.dofMaterial, 11);
					Graphics.Blit(this.mediumRezWorkTexture, this.lowRezWorkTexture);
					this.BlurFg(this.lowRezWorkTexture, this.lowRezWorkTexture, this.bluriness, 1, this.maxBlurSpread * num);
				}
				else
				{
					this.BlurFg(this.mediumRezWorkTexture, this.lowRezWorkTexture, this.bluriness, 1, this.maxBlurSpread);
				}
				Graphics.Blit(this.lowRezWorkTexture, this.finalDefocus);
				this.dofMaterial.SetTexture("_TapLowForeground", this.finalDefocus);
				Graphics.Blit(source, destination, this.dofMaterial, (!this.visualize) ? 4 : 1);
				if (this.bokeh && (this.bokehDestination & BokehDestination.Foreground) != (BokehDestination)0)
				{
					this.AddBokeh(this.bokehSource2, this.bokehSource, destination);
				}
			}
			this.ReleaseTextures();
		}
	}

	public override void Blur(RenderTexture from, RenderTexture to, DofBlurriness iterations, int blurPass, float spread)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(to.width, to.height);
		if (iterations > DofBlurriness.Low)
		{
			this.BlurHex(from, to, blurPass, spread, temporary);
			if (iterations > DofBlurriness.High)
			{
				this.dofBlurMaterial.SetVector("offsets", new Vector4((float)0, spread * this.oneOverBaseSize, (float)0, (float)0));
				Graphics.Blit(to, temporary, this.dofBlurMaterial, blurPass);
				this.dofBlurMaterial.SetVector("offsets", new Vector4(spread / this.widthOverHeight * this.oneOverBaseSize, (float)0, (float)0, (float)0));
				Graphics.Blit(temporary, to, this.dofBlurMaterial, blurPass);
			}
		}
		else
		{
			this.dofBlurMaterial.SetVector("offsets", new Vector4((float)0, spread * this.oneOverBaseSize, (float)0, (float)0));
			Graphics.Blit(from, temporary, this.dofBlurMaterial, blurPass);
			this.dofBlurMaterial.SetVector("offsets", new Vector4(spread / this.widthOverHeight * this.oneOverBaseSize, (float)0, (float)0, (float)0));
			Graphics.Blit(temporary, to, this.dofBlurMaterial, blurPass);
		}
		RenderTexture.ReleaseTemporary(temporary);
	}

	public override void BlurFg(RenderTexture from, RenderTexture to, DofBlurriness iterations, int blurPass, float spread)
	{
		this.dofBlurMaterial.SetTexture("_TapHigh", from);
		RenderTexture temporary = RenderTexture.GetTemporary(to.width, to.height);
		if (iterations > DofBlurriness.Low)
		{
			this.BlurHex(from, to, blurPass, spread, temporary);
			if (iterations > DofBlurriness.High)
			{
				this.dofBlurMaterial.SetVector("offsets", new Vector4((float)0, spread * this.oneOverBaseSize, (float)0, (float)0));
				Graphics.Blit(to, temporary, this.dofBlurMaterial, blurPass);
				this.dofBlurMaterial.SetVector("offsets", new Vector4(spread / this.widthOverHeight * this.oneOverBaseSize, (float)0, (float)0, (float)0));
				Graphics.Blit(temporary, to, this.dofBlurMaterial, blurPass);
			}
		}
		else
		{
			this.dofBlurMaterial.SetVector("offsets", new Vector4((float)0, spread * this.oneOverBaseSize, (float)0, (float)0));
			Graphics.Blit(from, temporary, this.dofBlurMaterial, blurPass);
			this.dofBlurMaterial.SetVector("offsets", new Vector4(spread / this.widthOverHeight * this.oneOverBaseSize, (float)0, (float)0, (float)0));
			Graphics.Blit(temporary, to, this.dofBlurMaterial, blurPass);
		}
		RenderTexture.ReleaseTemporary(temporary);
	}

	public override void BlurHex(RenderTexture from, RenderTexture to, int blurPass, float spread, RenderTexture tmp)
	{
		this.dofBlurMaterial.SetVector("offsets", new Vector4((float)0, spread * this.oneOverBaseSize, (float)0, (float)0));
		Graphics.Blit(from, tmp, this.dofBlurMaterial, blurPass);
		this.dofBlurMaterial.SetVector("offsets", new Vector4(spread / this.widthOverHeight * this.oneOverBaseSize, (float)0, (float)0, (float)0));
		Graphics.Blit(tmp, to, this.dofBlurMaterial, blurPass);
		this.dofBlurMaterial.SetVector("offsets", new Vector4(spread / this.widthOverHeight * this.oneOverBaseSize, spread * this.oneOverBaseSize, (float)0, (float)0));
		Graphics.Blit(to, tmp, this.dofBlurMaterial, blurPass);
		this.dofBlurMaterial.SetVector("offsets", new Vector4(spread / this.widthOverHeight * this.oneOverBaseSize, -spread * this.oneOverBaseSize, (float)0, (float)0));
		Graphics.Blit(tmp, to, this.dofBlurMaterial, blurPass);
	}

	public override void Downsample(RenderTexture from, RenderTexture to)
	{
		this.dofMaterial.SetVector("_InvRenderTargetSize", new Vector4(1f / (1f * (float)to.width), 1f / (1f * (float)to.height), (float)0, (float)0));
		Graphics.Blit(from, to, this.dofMaterial, DepthOfField34.SMOOTH_DOWNSAMPLE_PASS);
	}

	public override void AddBokeh(RenderTexture bokehInfo, RenderTexture tempTex, RenderTexture finalTarget)
	{
		if (this.bokehMaterial)
		{
			Mesh[] meshes = Quads.GetMeshes(tempTex.width, tempTex.height);
			RenderTexture.active = tempTex;
			GL.Clear(false, true, new Color((float)0, (float)0, (float)0, (float)0));
			GL.PushMatrix();
			GL.LoadIdentity();
			bokehInfo.filterMode = FilterMode.Point;
			float num = (float)bokehInfo.width * 1f / ((float)bokehInfo.height * 1f);
			float num2 = 2f / (1f * (float)bokehInfo.width);
			num2 += this.bokehScale * this.maxBlurSpread * DepthOfField34.BOKEH_EXTRA_BLUR * this.oneOverBaseSize;
			this.bokehMaterial.SetTexture("_Source", bokehInfo);
			this.bokehMaterial.SetTexture("_MainTex", this.bokehTexture);
			this.bokehMaterial.SetVector("_ArScale", new Vector4(num2, num2 * num, 0.5f, 0.5f * num));
			this.bokehMaterial.SetFloat("_Intensity", this.bokehIntensity);
			this.bokehMaterial.SetPass(0);
			int i = 0;
			Mesh[] array = meshes;
			int length = array.Length;
			while (i < length)
			{
				if (array[i])
				{
					Graphics.DrawMeshNow(array[i], Matrix4x4.identity);
				}
				i++;
			}
			GL.PopMatrix();
			Graphics.Blit(tempTex, finalTarget, this.dofMaterial, 8);
			bokehInfo.filterMode = FilterMode.Bilinear;
		}
	}

	public override void ReleaseTextures()
	{
		if (this.foregroundTexture)
		{
			RenderTexture.ReleaseTemporary(this.foregroundTexture);
		}
		if (this.finalDefocus)
		{
			RenderTexture.ReleaseTemporary(this.finalDefocus);
		}
		if (this.mediumRezWorkTexture)
		{
			RenderTexture.ReleaseTemporary(this.mediumRezWorkTexture);
		}
		if (this.lowRezWorkTexture)
		{
			RenderTexture.ReleaseTemporary(this.lowRezWorkTexture);
		}
		if (this.bokehSource)
		{
			RenderTexture.ReleaseTemporary(this.bokehSource);
		}
		if (this.bokehSource2)
		{
			RenderTexture.ReleaseTemporary(this.bokehSource2);
		}
	}

	public override void AllocateTextures(bool blurForeground, RenderTexture source, int divider, int lowTexDivider)
	{
		this.foregroundTexture = null;
		if (blurForeground)
		{
			this.foregroundTexture = RenderTexture.GetTemporary(source.width, source.height, 0);
		}
		this.mediumRezWorkTexture = RenderTexture.GetTemporary(source.width / divider, source.height / divider, 0);
		this.finalDefocus = RenderTexture.GetTemporary(source.width / divider, source.height / divider, 0);
		this.lowRezWorkTexture = RenderTexture.GetTemporary(source.width / lowTexDivider, source.height / lowTexDivider, 0);
		this.bokehSource = null;
		this.bokehSource2 = null;
		if (this.bokeh)
		{
			this.bokehSource = RenderTexture.GetTemporary(source.width / (lowTexDivider * this.bokehDownsample), source.height / (lowTexDivider * this.bokehDownsample), 0, RenderTextureFormat.ARGBHalf);
			this.bokehSource2 = RenderTexture.GetTemporary(source.width / (lowTexDivider * this.bokehDownsample), source.height / (lowTexDivider * this.bokehDownsample), 0, RenderTextureFormat.ARGBHalf);
			this.bokehSource.filterMode = FilterMode.Bilinear;
			this.bokehSource2.filterMode = FilterMode.Bilinear;
			RenderTexture.active = this.bokehSource2;
			GL.Clear(false, true, new Color((float)0, (float)0, (float)0, (float)0));
		}
		source.filterMode = FilterMode.Bilinear;
		this.finalDefocus.filterMode = FilterMode.Bilinear;
		this.mediumRezWorkTexture.filterMode = FilterMode.Bilinear;
		this.lowRezWorkTexture.filterMode = FilterMode.Bilinear;
		if (this.foregroundTexture)
		{
			this.foregroundTexture.filterMode = FilterMode.Bilinear;
		}
	}

	public override void Main()
	{
	}
}
