using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Rendering/Global Fog"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
[Serializable]
public class GlobalFog : PostEffectsBase
{
	[Serializable]
	public enum FogMode
	{
		AbsoluteYAndDistance,
		AbsoluteY,
		Distance,
		RelativeYAndDistance
	}

	public GlobalFog.FogMode fogMode;

	private float CAMERA_NEAR;

	private float CAMERA_FAR;

	private float CAMERA_FOV;

	private float CAMERA_ASPECT_RATIO;

	public float startDistance;

	public float globalDensity;

	public float heightScale;

	public float height;

	public Color globalFogColor;

	public Shader fogShader;

	private Material fogMaterial;

	public GlobalFog()
	{
		this.fogMode = GlobalFog.FogMode.AbsoluteYAndDistance;
		this.CAMERA_NEAR = 0.5f;
		this.CAMERA_FAR = 50f;
		this.CAMERA_FOV = 60f;
		this.CAMERA_ASPECT_RATIO = 1.333333f;
		this.startDistance = 200f;
		this.globalDensity = 1f;
		this.heightScale = 100f;
		this.globalFogColor = Color.grey;
	}

	public override bool CheckResources()
	{
		this.CheckSupport(true);
		this.fogMaterial = this.CheckShaderAndCreateMaterial(this.fogShader, this.fogMaterial);
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
			this.CAMERA_NEAR = this.camera.nearClipPlane;
			this.CAMERA_FAR = this.camera.farClipPlane;
			this.CAMERA_FOV = this.camera.fieldOfView;
			this.CAMERA_ASPECT_RATIO = this.camera.aspect;
			Matrix4x4 identity = Matrix4x4.identity;
			Vector4 vector = default(Vector4);
			Vector3 vector2 = default(Vector3);
			float num = this.CAMERA_FOV * 0.5f;
			Vector3 b = this.camera.transform.right * this.CAMERA_NEAR * Mathf.Tan(num * 0.0174532924f) * this.CAMERA_ASPECT_RATIO;
			Vector3 b2 = this.camera.transform.up * this.CAMERA_NEAR * Mathf.Tan(num * 0.0174532924f);
			Vector3 vector3 = this.camera.transform.forward * this.CAMERA_NEAR - b + b2;
			float num2 = vector3.magnitude * this.CAMERA_FAR / this.CAMERA_NEAR;
			vector3.Normalize();
			vector3 *= num2;
			Vector3 vector4 = this.camera.transform.forward * this.CAMERA_NEAR + b + b2;
			vector4.Normalize();
			vector4 *= num2;
			Vector3 vector5 = this.camera.transform.forward * this.CAMERA_NEAR + b - b2;
			vector5.Normalize();
			vector5 *= num2;
			Vector3 vector6 = this.camera.transform.forward * this.CAMERA_NEAR - b - b2;
			vector6.Normalize();
			vector6 *= num2;
			identity.SetRow(0, vector3);
			identity.SetRow(1, vector4);
			identity.SetRow(2, vector5);
			identity.SetRow(3, vector6);
			this.fogMaterial.SetMatrix("_FrustumCornersWS", identity);
			this.fogMaterial.SetVector("_CameraWS", this.camera.transform.position);
			this.fogMaterial.SetVector("_StartDistance", new Vector4(1f / this.startDistance, num2 - this.startDistance));
			this.fogMaterial.SetVector("_Y", new Vector4(this.height, 1f / this.heightScale));
			this.fogMaterial.SetFloat("_GlobalDensity", this.globalDensity * 0.01f);
			this.fogMaterial.SetColor("_FogColor", this.globalFogColor);
			GlobalFog.CustomGraphicsBlit(source, destination, this.fogMaterial, (int)this.fogMode);
		}
	}

	public static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr)
	{
		RenderTexture.active = dest;
		fxMaterial.SetTexture("_MainTex", source);
		GL.PushMatrix();
		GL.LoadOrtho();
		fxMaterial.SetPass(passNr);
		GL.Begin(7);
		GL.MultiTexCoord2(0, (float)0, (float)0);
		GL.Vertex3((float)0, (float)0, 3f);
		GL.MultiTexCoord2(0, 1f, (float)0);
		GL.Vertex3(1f, (float)0, 2f);
		GL.MultiTexCoord2(0, 1f, 1f);
		GL.Vertex3(1f, 1f, 1f);
		GL.MultiTexCoord2(0, (float)0, 1f);
		GL.Vertex3((float)0, 1f, (float)0);
		GL.End();
		GL.PopMatrix();
	}

	public override void Main()
	{
	}
}
