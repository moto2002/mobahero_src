using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Other/Antialiasing"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
[Serializable]
public class AntialiasingAsPostEffect : PostEffectsBase
{
	public AAMode mode;

	public bool showGeneratedNormals;

	public float offsetScale;

	public float blurRadius;

	public float edgeThresholdMin;

	public float edgeThreshold;

	public float edgeSharpness;

	public bool dlaaSharp;

	public Shader ssaaShader;

	private Material ssaa;

	public Shader dlaaShader;

	private Material dlaa;

	public Shader nfaaShader;

	private Material nfaa;

	public Shader shaderFXAAPreset2;

	private Material materialFXAAPreset2;

	public Shader shaderFXAAPreset3;

	private Material materialFXAAPreset3;

	public Shader shaderFXAAII;

	private Material materialFXAAII;

	public Shader shaderFXAAIII;

	private Material materialFXAAIII;

	public AntialiasingAsPostEffect()
	{
		this.mode = AAMode.FXAA3Console;
		this.offsetScale = 0.2f;
		this.blurRadius = 18f;
		this.edgeThresholdMin = 0.05f;
		this.edgeThreshold = 0.2f;
		this.edgeSharpness = 4f;
	}

	public override Material CurrentAAMaterial()
	{
		AAMode aAMode = this.mode;
		Material result;
		if (aAMode == AAMode.FXAA3Console)
		{
			result = this.materialFXAAIII;
		}
		else if (aAMode == AAMode.FXAA2)
		{
			result = this.materialFXAAII;
		}
		else if (aAMode == AAMode.FXAA1PresetA)
		{
			result = this.materialFXAAPreset2;
		}
		else if (aAMode == AAMode.FXAA1PresetB)
		{
			result = this.materialFXAAPreset3;
		}
		else if (aAMode == AAMode.NFAA)
		{
			result = this.nfaa;
		}
		else if (aAMode == AAMode.SSAA)
		{
			result = this.ssaa;
		}
		else if (aAMode == AAMode.DLAA)
		{
			result = this.dlaa;
		}
		else
		{
			result = null;
		}
		return result;
	}

	public override bool CheckResources()
	{
		this.CheckSupport(false);
		this.materialFXAAPreset2 = this.CreateMaterial(this.shaderFXAAPreset2, this.materialFXAAPreset2);
		this.materialFXAAPreset3 = this.CreateMaterial(this.shaderFXAAPreset3, this.materialFXAAPreset3);
		this.materialFXAAII = this.CreateMaterial(this.shaderFXAAII, this.materialFXAAII);
		this.materialFXAAIII = this.CreateMaterial(this.shaderFXAAIII, this.materialFXAAIII);
		this.nfaa = this.CreateMaterial(this.nfaaShader, this.nfaa);
		this.ssaa = this.CreateMaterial(this.ssaaShader, this.ssaa);
		this.dlaa = this.CreateMaterial(this.dlaaShader, this.dlaa);
		if (!this.ssaaShader.isSupported)
		{
			this.NotSupported();
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
		else if (this.mode == AAMode.FXAA3Console && this.materialFXAAIII != null)
		{
			this.materialFXAAIII.SetFloat("_EdgeThresholdMin", this.edgeThresholdMin);
			this.materialFXAAIII.SetFloat("_EdgeThreshold", this.edgeThreshold);
			this.materialFXAAIII.SetFloat("_EdgeSharpness", this.edgeSharpness);
			Graphics.Blit(source, destination, this.materialFXAAIII);
		}
		else if (this.mode == AAMode.FXAA1PresetB && this.materialFXAAPreset3 != null)
		{
			Graphics.Blit(source, destination, this.materialFXAAPreset3);
		}
		else if (this.mode == AAMode.FXAA1PresetA && this.materialFXAAPreset2 != null)
		{
			source.anisoLevel = 4;
			Graphics.Blit(source, destination, this.materialFXAAPreset2);
			source.anisoLevel = 0;
		}
		else if (this.mode == AAMode.FXAA2 && this.materialFXAAII != null)
		{
			Graphics.Blit(source, destination, this.materialFXAAII);
		}
		else if (this.mode == AAMode.SSAA && this.ssaa != null)
		{
			Graphics.Blit(source, destination, this.ssaa);
		}
		else if (this.mode == AAMode.DLAA && this.dlaa != null)
		{
			source.anisoLevel = 0;
			RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height);
			Graphics.Blit(source, temporary, this.dlaa, 0);
			Graphics.Blit(temporary, destination, this.dlaa, (!this.dlaaSharp) ? 1 : 2);
			RenderTexture.ReleaseTemporary(temporary);
		}
		else if (this.mode == AAMode.NFAA && this.nfaa != null)
		{
			source.anisoLevel = 0;
			this.nfaa.SetFloat("_OffsetScale", this.offsetScale);
			this.nfaa.SetFloat("_BlurRadius", this.blurRadius);
			Graphics.Blit(source, destination, this.nfaa, (!this.showGeneratedNormals) ? 0 : 1);
		}
		else
		{
			Graphics.Blit(source, destination);
		}
	}

	public override void Main()
	{
	}
}
