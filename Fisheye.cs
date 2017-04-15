using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Displacement/Fisheye"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
[Serializable]
public class Fisheye : PostEffectsBase
{
	public float strengthX;

	public float strengthY;

	public Shader fishEyeShader;

	private Material fisheyeMaterial;

	public Fisheye()
	{
		this.strengthX = 0.05f;
		this.strengthY = 0.05f;
	}

	public override bool CheckResources()
	{
		this.CheckSupport(false);
		this.fisheyeMaterial = this.CheckShaderAndCreateMaterial(this.fishEyeShader, this.fisheyeMaterial);
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
			float num = 0.15625f;
			float num2 = (float)source.width * 1f / ((float)source.height * 1f);
			this.fisheyeMaterial.SetVector("intensity", new Vector4(this.strengthX * num2 * num, this.strengthY * num, this.strengthX * num2 * num, this.strengthY * num));
			Graphics.Blit(source, destination, this.fisheyeMaterial);
		}
	}

	public override void Main()
	{
	}
}
