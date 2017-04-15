using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Rendering/Sun Shafts"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
[Serializable]
public class SunShafts : PostEffectsBase
{
	public SunShaftsResolution resolution;

	public ShaftsScreenBlendMode screenBlendMode;

	public Transform sunTransform;

	public int radialBlurIterations;

	public Color sunColor;

	public float sunShaftBlurRadius;

	public float sunShaftIntensity;

	public float useSkyBoxAlpha;

	public float maxRadius;

	public bool useDepthTexture;

	public Shader sunShaftsShader;

	private Material sunShaftsMaterial;

	public Shader simpleClearShader;

	private Material simpleClearMaterial;

	public SunShafts()
	{
		this.resolution = SunShaftsResolution.Normal;
		this.screenBlendMode = ShaftsScreenBlendMode.Screen;
		this.radialBlurIterations = 2;
		this.sunColor = Color.white;
		this.sunShaftBlurRadius = 2.5f;
		this.sunShaftIntensity = 1.15f;
		this.useSkyBoxAlpha = 0.75f;
		this.maxRadius = 0.75f;
		this.useDepthTexture = true;
	}

	public override bool CheckResources()
	{
		this.CheckSupport(this.useDepthTexture);
		this.sunShaftsMaterial = this.CheckShaderAndCreateMaterial(this.sunShaftsShader, this.sunShaftsMaterial);
		this.simpleClearMaterial = this.CheckShaderAndCreateMaterial(this.simpleClearShader, this.simpleClearMaterial);
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
			if (this.useDepthTexture)
			{
				this.camera.depthTextureMode = (this.camera.depthTextureMode | DepthTextureMode.Depth);
			}
			int num = 4;
			if (this.resolution == SunShaftsResolution.Normal)
			{
				num = 2;
			}
			else if (this.resolution == SunShaftsResolution.High)
			{
				num = 1;
			}
			Vector3 vector = Vector3.one * 0.5f;
			if (this.sunTransform)
			{
				vector = this.camera.WorldToViewportPoint(this.sunTransform.position);
			}
			else
			{
				vector = new Vector3(0.5f, 0.5f, (float)0);
			}
			int width = source.width / num;
			int height = source.height / num;
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0);
			this.sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(1f, 1f, (float)0, (float)0) * this.sunShaftBlurRadius);
			this.sunShaftsMaterial.SetVector("_SunPosition", new Vector4(vector.x, vector.y, vector.z, this.maxRadius));
			this.sunShaftsMaterial.SetFloat("_NoSkyBoxMask", 1f - this.useSkyBoxAlpha);
			if (!this.useDepthTexture)
			{
				RenderTexture temporary2 = RenderTexture.GetTemporary(source.width, source.height, 0);
				RenderTexture.active = temporary2;
				GL.ClearWithSkybox(false, this.camera);
				this.sunShaftsMaterial.SetTexture("_Skybox", temporary2);
				Graphics.Blit(source, temporary, this.sunShaftsMaterial, 3);
				RenderTexture.ReleaseTemporary(temporary2);
			}
			else
			{
				Graphics.Blit(source, temporary, this.sunShaftsMaterial, 2);
			}
			this.DrawBorder(temporary, this.simpleClearMaterial);
			this.radialBlurIterations = Mathf.Clamp(this.radialBlurIterations, 1, 4);
			float num2 = this.sunShaftBlurRadius * 0.00130208337f;
			this.sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(num2, num2, (float)0, (float)0));
			this.sunShaftsMaterial.SetVector("_SunPosition", new Vector4(vector.x, vector.y, vector.z, this.maxRadius));
			for (int i = 0; i < this.radialBlurIterations; i++)
			{
				RenderTexture temporary3 = RenderTexture.GetTemporary(width, height, 0);
				Graphics.Blit(temporary, temporary3, this.sunShaftsMaterial, 1);
				RenderTexture.ReleaseTemporary(temporary);
				num2 = this.sunShaftBlurRadius * (((float)i * 2f + 1f) * 6f) / 768f;
				this.sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(num2, num2, (float)0, (float)0));
				temporary = RenderTexture.GetTemporary(width, height, 0);
				Graphics.Blit(temporary3, temporary, this.sunShaftsMaterial, 1);
				RenderTexture.ReleaseTemporary(temporary3);
				num2 = this.sunShaftBlurRadius * (((float)i * 2f + 2f) * 6f) / 768f;
				this.sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(num2, num2, (float)0, (float)0));
			}
			if (vector.z >= (float)0)
			{
				this.sunShaftsMaterial.SetVector("_SunColor", new Vector4(this.sunColor.r, this.sunColor.g, this.sunColor.b, this.sunColor.a) * this.sunShaftIntensity);
			}
			else
			{
				this.sunShaftsMaterial.SetVector("_SunColor", Vector4.zero);
			}
			this.sunShaftsMaterial.SetTexture("_ColorBuffer", temporary);
			Graphics.Blit(source, destination, this.sunShaftsMaterial, (this.screenBlendMode != ShaftsScreenBlendMode.Screen) ? 4 : 0);
			RenderTexture.ReleaseTemporary(temporary);
		}
	}

	public override void Main()
	{
	}
}
