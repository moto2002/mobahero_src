using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Camera/Camera Motion Blur"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
[Serializable]
public class CameraMotionBlur : PostEffectsBase
{
	[Serializable]
	public enum MotionBlurFilter
	{
		CameraMotion,
		LocalBlur,
		Reconstruction,
		ReconstructionDX11,
		ReconstructionDisc
	}

	[NonSerialized]
	public static int MAX_RADIUS = (int)10f;

	public CameraMotionBlur.MotionBlurFilter filterType;

	public bool preview;

	public Vector3 previewScale;

	public float movementScale;

	public float rotationScale;

	public float maxVelocity;

	public float minVelocity;

	public float velocityScale;

	public float softZDistance;

	public int velocityDownsample;

	public LayerMask excludeLayers;

	private GameObject tmpCam;

	public Shader shader;

	public Shader dx11MotionBlurShader;

	public Shader replacementClear;

	private Material motionBlurMaterial;

	private Material dx11MotionBlurMaterial;

	public Texture2D noiseTexture;

	public float jitter;

	public bool showVelocity;

	public float showVelocityScale;

	private Matrix4x4 currentViewProjMat;

	private Matrix4x4 prevViewProjMat;

	private int prevFrameCount;

	private bool wasActive;

	private Vector3 prevFrameForward;

	private Vector3 prevFrameRight;

	private Vector3 prevFrameUp;

	private Vector3 prevFramePos;

	public CameraMotionBlur()
	{
		this.filterType = CameraMotionBlur.MotionBlurFilter.Reconstruction;
		this.previewScale = Vector3.one;
		this.rotationScale = 1f;
		this.maxVelocity = 8f;
		this.minVelocity = 0.1f;
		this.velocityScale = 0.375f;
		this.softZDistance = 0.005f;
		this.velocityDownsample = 1;
		this.jitter = 0.05f;
		this.showVelocityScale = 1f;
		this.prevFrameForward = Vector3.forward;
		this.prevFrameRight = Vector3.right;
		this.prevFrameUp = Vector3.up;
		this.prevFramePos = Vector3.zero;
	}

	private void CalculateViewProjection()
	{
		Matrix4x4 worldToCameraMatrix = this.camera.worldToCameraMatrix;
		Matrix4x4 gPUProjectionMatrix = GL.GetGPUProjectionMatrix(this.camera.projectionMatrix, true);
		this.currentViewProjMat = gPUProjectionMatrix * worldToCameraMatrix;
	}

	public override void Start()
	{
		this.CheckResources();
		this.wasActive = this.gameObject.activeInHierarchy;
		this.CalculateViewProjection();
		this.Remember();
		this.wasActive = false;
	}

	public override void OnEnable()
	{
		this.camera.depthTextureMode = (this.camera.depthTextureMode | DepthTextureMode.Depth);
	}

	public override void OnDisable()
	{
		if (null != this.motionBlurMaterial)
		{
			UnityEngine.Object.DestroyImmediate(this.motionBlurMaterial);
			this.motionBlurMaterial = null;
		}
		if (null != this.dx11MotionBlurMaterial)
		{
			UnityEngine.Object.DestroyImmediate(this.dx11MotionBlurMaterial);
			this.dx11MotionBlurMaterial = null;
		}
		if (null != this.tmpCam)
		{
			UnityEngine.Object.DestroyImmediate(this.tmpCam);
			this.tmpCam = null;
		}
	}

	public override bool CheckResources()
	{
		this.CheckSupport(true, true);
		this.motionBlurMaterial = this.CheckShaderAndCreateMaterial(this.shader, this.motionBlurMaterial);
		if (this.supportDX11 && this.filterType == CameraMotionBlur.MotionBlurFilter.ReconstructionDX11)
		{
			this.dx11MotionBlurMaterial = this.CheckShaderAndCreateMaterial(this.dx11MotionBlurShader, this.dx11MotionBlurMaterial);
		}
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
			if (this.filterType == CameraMotionBlur.MotionBlurFilter.CameraMotion)
			{
				this.StartFrame();
			}
			RenderTextureFormat format = (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGHalf)) ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.RGHalf;
			RenderTexture temporary = RenderTexture.GetTemporary(this.divRoundUp(source.width, this.velocityDownsample), this.divRoundUp(source.height, this.velocityDownsample), 0, format);
			this.maxVelocity = Mathf.Max(2f, this.maxVelocity);
			float value = this.maxVelocity;
			bool flag = false;
			if (this.filterType == CameraMotionBlur.MotionBlurFilter.ReconstructionDX11 && this.dx11MotionBlurMaterial == null)
			{
				flag = true;
			}
			int num;
			int height;
			if (this.filterType == CameraMotionBlur.MotionBlurFilter.Reconstruction || flag || this.filterType == CameraMotionBlur.MotionBlurFilter.ReconstructionDisc)
			{
				this.maxVelocity = Mathf.Min(this.maxVelocity, (float)CameraMotionBlur.MAX_RADIUS);
				num = this.divRoundUp(temporary.width, (int)this.maxVelocity);
				height = this.divRoundUp(temporary.height, (int)this.maxVelocity);
				value = (float)(temporary.width / num);
			}
			else
			{
				num = this.divRoundUp(temporary.width, (int)this.maxVelocity);
				height = this.divRoundUp(temporary.height, (int)this.maxVelocity);
				value = (float)(temporary.width / num);
			}
			RenderTexture temporary2 = RenderTexture.GetTemporary(num, height, 0, format);
			RenderTexture temporary3 = RenderTexture.GetTemporary(num, height, 0, format);
			temporary.filterMode = FilterMode.Point;
			temporary2.filterMode = FilterMode.Point;
			temporary3.filterMode = FilterMode.Point;
			if (this.noiseTexture)
			{
				this.noiseTexture.filterMode = FilterMode.Point;
			}
			source.wrapMode = TextureWrapMode.Clamp;
			temporary.wrapMode = TextureWrapMode.Clamp;
			temporary3.wrapMode = TextureWrapMode.Clamp;
			temporary2.wrapMode = TextureWrapMode.Clamp;
			this.CalculateViewProjection();
			if (this.gameObject.activeInHierarchy && !this.wasActive)
			{
				this.Remember();
			}
			this.wasActive = this.gameObject.activeInHierarchy;
			Matrix4x4 matrix4x = Matrix4x4.Inverse(this.currentViewProjMat);
			this.motionBlurMaterial.SetMatrix("_InvViewProj", matrix4x);
			this.motionBlurMaterial.SetMatrix("_PrevViewProj", this.prevViewProjMat);
			this.motionBlurMaterial.SetMatrix("_ToPrevViewProjCombined", this.prevViewProjMat * matrix4x);
			this.motionBlurMaterial.SetFloat("_MaxVelocity", value);
			this.motionBlurMaterial.SetFloat("_MaxRadiusOrKInPaper", value);
			this.motionBlurMaterial.SetFloat("_MinVelocity", this.minVelocity);
			this.motionBlurMaterial.SetFloat("_VelocityScale", this.velocityScale);
			this.motionBlurMaterial.SetFloat("_Jitter", this.jitter);
			this.motionBlurMaterial.SetTexture("_NoiseTex", this.noiseTexture);
			this.motionBlurMaterial.SetTexture("_VelTex", temporary);
			this.motionBlurMaterial.SetTexture("_NeighbourMaxTex", temporary3);
			this.motionBlurMaterial.SetTexture("_TileTexDebug", temporary2);
			if (this.preview)
			{
				Matrix4x4 worldToCameraMatrix = this.camera.worldToCameraMatrix;
				Matrix4x4 identity = Matrix4x4.identity;
				identity.SetTRS(this.previewScale * 0.3333f, Quaternion.identity, Vector3.one);
				Matrix4x4 gPUProjectionMatrix = GL.GetGPUProjectionMatrix(this.camera.projectionMatrix, true);
				this.prevViewProjMat = gPUProjectionMatrix * identity * worldToCameraMatrix;
				this.motionBlurMaterial.SetMatrix("_PrevViewProj", this.prevViewProjMat);
				this.motionBlurMaterial.SetMatrix("_ToPrevViewProjCombined", this.prevViewProjMat * matrix4x);
			}
			if (this.filterType == CameraMotionBlur.MotionBlurFilter.CameraMotion)
			{
				Vector4 zero = Vector4.zero;
				float num2 = Vector3.Dot(this.transform.up, Vector3.up);
				Vector3 rhs = this.prevFramePos - this.transform.position;
				float magnitude = rhs.magnitude;
				float num3 = Vector3.Angle(this.transform.up, this.prevFrameUp) / this.camera.fieldOfView * ((float)source.width * 0.75f);
				zero.x = this.rotationScale * num3;
				num3 = Vector3.Angle(this.transform.forward, this.prevFrameForward) / this.camera.fieldOfView * ((float)source.width * 0.75f);
				zero.y = this.rotationScale * num2 * num3;
				num3 = Vector3.Angle(this.transform.forward, this.prevFrameForward) / this.camera.fieldOfView * ((float)source.width * 0.75f);
				zero.z = this.rotationScale * (1f - num2) * num3;
				if (magnitude > 1.401298E-45f && this.movementScale > 1.401298E-45f)
				{
					zero.w = this.movementScale * Vector3.Dot(this.transform.forward, rhs) * ((float)source.width * 0.5f);
					zero.x += this.movementScale * Vector3.Dot(this.transform.up, rhs) * ((float)source.width * 0.5f);
					zero.y += this.movementScale * Vector3.Dot(this.transform.right, rhs) * ((float)source.width * 0.5f);
				}
				if (this.preview)
				{
					this.motionBlurMaterial.SetVector("_BlurDirectionPacked", new Vector4(this.previewScale.y, this.previewScale.x, (float)0, this.previewScale.z) * 0.5f * this.camera.fieldOfView);
				}
				else
				{
					this.motionBlurMaterial.SetVector("_BlurDirectionPacked", zero);
				}
			}
			else
			{
				Graphics.Blit(source, temporary, this.motionBlurMaterial, 0);
				Camera camera = null;
				if (this.excludeLayers.value != 0)
				{
					camera = this.GetTmpCam();
				}
				if (camera && this.excludeLayers.value != 0 && this.replacementClear && this.replacementClear.isSupported)
				{
					camera.targetTexture = temporary;
					camera.cullingMask = this.excludeLayers;
					camera.RenderWithShader(this.replacementClear, string.Empty);
				}
			}
			if (!this.preview && Time.frameCount != this.prevFrameCount)
			{
				this.prevFrameCount = Time.frameCount;
				this.Remember();
			}
			source.filterMode = FilterMode.Bilinear;
			if (this.showVelocity)
			{
				this.motionBlurMaterial.SetFloat("_DisplayVelocityScale", this.showVelocityScale);
				Graphics.Blit(temporary, destination, this.motionBlurMaterial, 1);
			}
			else if (this.filterType == CameraMotionBlur.MotionBlurFilter.ReconstructionDX11 && !flag)
			{
				this.dx11MotionBlurMaterial.SetFloat("_MinVelocity", this.minVelocity);
				this.dx11MotionBlurMaterial.SetFloat("_VelocityScale", this.velocityScale);
				this.dx11MotionBlurMaterial.SetFloat("_Jitter", this.jitter);
				this.dx11MotionBlurMaterial.SetTexture("_NoiseTex", this.noiseTexture);
				this.dx11MotionBlurMaterial.SetTexture("_VelTex", temporary);
				this.dx11MotionBlurMaterial.SetTexture("_NeighbourMaxTex", temporary3);
				this.dx11MotionBlurMaterial.SetFloat("_SoftZDistance", Mathf.Max(0.00025f, this.softZDistance));
				this.dx11MotionBlurMaterial.SetFloat("_MaxRadiusOrKInPaper", value);
				Graphics.Blit(temporary, temporary2, this.dx11MotionBlurMaterial, 0);
				Graphics.Blit(temporary2, temporary3, this.dx11MotionBlurMaterial, 1);
				Graphics.Blit(source, destination, this.dx11MotionBlurMaterial, 2);
			}
			else if (this.filterType == CameraMotionBlur.MotionBlurFilter.Reconstruction || flag)
			{
				this.motionBlurMaterial.SetFloat("_SoftZDistance", Mathf.Max(0.00025f, this.softZDistance));
				Graphics.Blit(temporary, temporary2, this.motionBlurMaterial, 2);
				Graphics.Blit(temporary2, temporary3, this.motionBlurMaterial, 3);
				Graphics.Blit(source, destination, this.motionBlurMaterial, 4);
			}
			else if (this.filterType == CameraMotionBlur.MotionBlurFilter.CameraMotion)
			{
				Graphics.Blit(source, destination, this.motionBlurMaterial, 6);
			}
			else if (this.filterType == CameraMotionBlur.MotionBlurFilter.ReconstructionDisc)
			{
				this.motionBlurMaterial.SetFloat("_SoftZDistance", Mathf.Max(0.00025f, this.softZDistance));
				Graphics.Blit(temporary, temporary2, this.motionBlurMaterial, 2);
				Graphics.Blit(temporary2, temporary3, this.motionBlurMaterial, 3);
				Graphics.Blit(source, destination, this.motionBlurMaterial, 7);
			}
			else
			{
				Graphics.Blit(source, destination, this.motionBlurMaterial, 5);
			}
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(temporary2);
			RenderTexture.ReleaseTemporary(temporary3);
		}
	}

	public override void Remember()
	{
		this.prevViewProjMat = this.currentViewProjMat;
		this.prevFrameForward = this.transform.forward;
		this.prevFrameRight = this.transform.right;
		this.prevFrameUp = this.transform.up;
		this.prevFramePos = this.transform.position;
	}

	public override Camera GetTmpCam()
	{
		if (this.tmpCam == null)
		{
			string name = "_" + this.camera.name + "_MotionBlurTmpCam";
			GameObject y = GameObject.Find(name);
			if (null == y)
			{
				this.tmpCam = new GameObject(name, new Type[]
				{
					typeof(Camera)
				});
			}
			else
			{
				this.tmpCam = y;
			}
		}
		this.tmpCam.hideFlags = HideFlags.DontSave;
		this.tmpCam.transform.position = this.camera.transform.position;
		this.tmpCam.transform.rotation = this.camera.transform.rotation;
		this.tmpCam.transform.localScale = this.camera.transform.localScale;
		this.tmpCam.camera.CopyFrom(this.camera);
		this.tmpCam.camera.enabled = false;
		this.tmpCam.camera.depthTextureMode = DepthTextureMode.None;
		this.tmpCam.camera.clearFlags = CameraClearFlags.Nothing;
		return this.tmpCam.camera;
	}

	public override void StartFrame()
	{
		this.prevFramePos = Vector3.Slerp(this.prevFramePos, this.transform.position, 0.75f);
	}

	public override int divRoundUp(int x, int d)
	{
		return (x + d - 1) / d;
	}

	public override void Main()
	{
	}
}
