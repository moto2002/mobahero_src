using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Rendering/Screen Space Ambient Obscurance"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
[Serializable]
public class AmbientObscurance : PostEffectsBase
{
	[Range(0f, 3f)]
	public float intensity;

	[Range(0.1f, 3f)]
	public float radius;

	[Range(0f, 3f)]
	public int blurIterations;

	[Range(0f, 5f)]
	public float blurFilterDistance;

	[Range(0f, 1f)]
	public int downsample;

	public Texture2D rand;

	public Shader aoShader;

	private Material aoMaterial;

	public AmbientObscurance()
	{
		this.intensity = 0.5f;
		this.radius = 0.2f;
		this.blurIterations = 1;
		this.blurFilterDistance = 1.25f;
	}

	public override bool CheckResources()
	{
		this.CheckSupport(true);
		this.aoMaterial = this.CheckShaderAndCreateMaterial(this.aoShader, this.aoMaterial);
		if (!this.isSupported)
		{
			this.ReportAutoDisable();
		}
		return this.isSupported;
	}

	public override void OnDisable()
	{
		if (this.aoMaterial)
		{
			UnityEngine.Object.DestroyImmediate(this.aoMaterial);
		}
		this.aoMaterial = null;
	}

	[ImageEffectOpaque]
	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.CheckResources())
		{
			Graphics.Blit(source, destination);
		}
		else
		{
			Matrix4x4 projectionMatrix = this.camera.projectionMatrix;
			Matrix4x4 inverse = projectionMatrix.inverse;
			Vector4 vector = new Vector4(-2f / ((float)Screen.width * projectionMatrix[0]), -2f / ((float)Screen.height * projectionMatrix[5]), (1f - projectionMatrix[2]) / projectionMatrix[0], (1f + projectionMatrix[6]) / projectionMatrix[5]);
			this.aoMaterial.SetVector("_ProjInfo", vector);
			this.aoMaterial.SetMatrix("_ProjectionInv", inverse);
			this.aoMaterial.SetTexture("_Rand", this.rand);
			this.aoMaterial.SetFloat("_Radius", this.radius);
			this.aoMaterial.SetFloat("_Radius2", this.radius * this.radius);
			this.aoMaterial.SetFloat("_Intensity", this.intensity);
			this.aoMaterial.SetFloat("_BlurFilterDistance", this.blurFilterDistance);
			int width = source.width;
			int height = source.height;
			RenderTexture renderTexture = RenderTexture.GetTemporary(width >> this.downsample, height >> this.downsample);
			Graphics.Blit(source, renderTexture, this.aoMaterial, 0);
			if (this.downsample > 0)
			{
				RenderTexture temporary = RenderTexture.GetTemporary(width, height);
				Graphics.Blit(renderTexture, temporary, this.aoMaterial, 4);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
			}
			for (int i = 0; i < this.blurIterations; i++)
			{
				this.aoMaterial.SetVector("_Axis", new Vector2(1f, (float)0));
				RenderTexture temporary = RenderTexture.GetTemporary(width, height);
				Graphics.Blit(renderTexture, temporary, this.aoMaterial, 1);
				RenderTexture.ReleaseTemporary(renderTexture);
				this.aoMaterial.SetVector("_Axis", new Vector2((float)0, 1f));
				renderTexture = RenderTexture.GetTemporary(width, height);
				Graphics.Blit(temporary, renderTexture, this.aoMaterial, 1);
				RenderTexture.ReleaseTemporary(temporary);
			}
			this.aoMaterial.SetTexture("_AOTex", renderTexture);
			Graphics.Blit(source, destination, this.aoMaterial, 2);
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}

	public override void Main()
	{
	}
}
