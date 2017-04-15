using Com.Game.Utils;
using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FOWEffect : MonoBehaviour
{
	private GameObject quad;

	public Shader shader;

	public Color unexploredColor = new Color(0.3f, 0.3f, 0.4f, 1f);

	public Color exploredColor = new Color(0.2f, 0.2f, 0.2f, 1f);

	public FOWSystem mFog;

	private Camera mCam;

	private Matrix4x4 mInverseMVP;

	private Material mMat;

	private RenderTexture accumTexture;

	private float meshoffset = 0.01f;

	public void createImg()
	{
		this.shader = (Resources.Load("FOW_IMG", typeof(Shader)) as Shader);
		this.mMat = new Material(this.shader);
	}

	public GameObject createMesh()
	{
		GameObject gameObject = new GameObject("Quad_Auto");
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.rotation = Quaternion.identity;
		float num = 0.0174532924f * this.mCam.fieldOfView;
		float num2 = Mathf.Tan(num / 2f) * (this.mCam.nearClipPlane + this.meshoffset) * 2f;
		Vector2 vector = new Vector2(num2 * this.mCam.aspect, num2) * 0.5f;
		Vector3[] vertices = new Vector3[]
		{
			new Vector3(-vector.x, -vector.y, 0f),
			new Vector3(-vector.x, vector.y, 0f),
			new Vector3(vector.x, vector.y, 0f),
			new Vector3(vector.x, -vector.y, 0f)
		};
		Vector2[] uv = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f)
		};
		Color[] colors = new Color[]
		{
			new Color(0f, 0f, 0f),
			new Color(0f, 0f, 0f),
			new Color(0f, 0f, 0f),
			new Color(0f, 0f, 0f)
		};
		int[] triangles = new int[]
		{
			0,
			1,
			2,
			0,
			2,
			3
		};
		Vector3[] normals = new Vector3[]
		{
			Vector3.forward,
			Vector3.forward,
			Vector3.forward,
			Vector3.forward
		};
		Vector4[] tangents = new Vector4[]
		{
			new Vector4(-1f, 0f, 0f, -1f),
			new Vector4(-1f, 0f, 0f, -1f),
			new Vector4(-1f, 0f, 0f, -1f),
			new Vector4(-1f, 0f, 0f, -1f)
		};
		Mesh mesh = new Mesh();
		gameObject.AddComponent<MeshFilter>().mesh = mesh;
		gameObject.AddComponent<MeshRenderer>().material = new Material(Resources.Load("FOW_MESH", typeof(Shader)) as Shader);
		this.mMat = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.tangents = tangents;
		mesh.colors = colors;
		gameObject.transform.position = base.transform.position + base.transform.forward * (base.camera.nearClipPlane + this.meshoffset);
		gameObject.transform.LookAt(base.transform.position + base.transform.forward * 2f, base.transform.up);
		gameObject.transform.parent = base.transform;
		if (this.quad != null)
		{
			UnityEngine.Object.DestroyImmediate(this.quad);
		}
		this.quad = gameObject;
		if (!FOWSystem.effectVisible || GlobalSettings.FogMode != 3)
		{
			gameObject.SetActive(false);
		}
		else
		{
			gameObject.SetActive(true);
		}
		return gameObject;
	}

	private void OnEnable()
	{
		this.mCam = base.camera;
		this.mCam.depthTextureMode = DepthTextureMode.Depth;
	}

	public void manualstart()
	{
		if (this.mFog == null)
		{
			return;
		}
		this.createMesh();
	}

	private void Start()
	{
		this.manualstart();
	}

	private void Update()
	{
		if (!FOWSystem.effectVisible || GlobalSettings.FogMode == 2)
		{
			if (this.quad != null && this.quad.active)
			{
				this.quad.SetActive(false);
			}
			return;
		}
		if (GlobalSettings.FogMode > 2 && this.quad != null && !this.quad.active)
		{
			this.quad.SetActive(true);
		}
		this.updatashader();
	}

	public void enabelFog(bool enabel)
	{
		if (this.quad != null)
		{
			this.quad.SetActive(enabel);
		}
	}

	private void updatashader()
	{
		if (this.mFog != null && this.mFog.enabled)
		{
			if (!this.mFog.getenabelfog())
			{
				return;
			}
			this.mInverseMVP = (this.mCam.projectionMatrix * this.mCam.worldToCameraMatrix).inverse;
			float num = 1f / (float)this.mFog.worldSize;
			Transform transform = this.mFog.transform;
			float num2 = transform.position.x - (float)this.mFog.worldSize * 0.5f;
			float num3 = transform.position.z - (float)this.mFog.worldSize * 0.5f;
			Vector4 vector = this.mCam.transform.position;
			if (QualitySettings.antiAliasing > 0)
			{
				RuntimePlatform platform = Application.platform;
				if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsWebPlayer)
				{
					vector.w = 1f;
				}
			}
			if (this.mMat != null)
			{
				Vector4 vector2 = new Vector4(-num2 * num, -num3 * num, num, this.mFog.blendFactor);
				this.mMat.SetVector("_CamPos", vector);
				this.mMat.SetVector("_Params", vector2);
				this.mMat.SetMatrix("_InverseMVP", this.mInverseMVP);
				this.mMat.SetTexture("_MainTex", this.mFog.texture1);
			}
			else
			{
				ClientLogger.Warn("err mMat...");
			}
		}
		else
		{
			ClientLogger.Warn("err fog..");
		}
	}
}
