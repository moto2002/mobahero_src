using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RecieverObjCtrl : MonoBehaviour
{
	private float m_nearClipPlane = 0.1f;

	private float m_farClipPlane = 5f;

	private float m_fieldOfView = 60f;

	private float m_aspectRatio = 1f;

	private bool m_isOrthographic;

	private float m_orthographicSize = 2f;

	private Material mmat;

	private Mesh mmesh;

	private MeshFilter mmeshfilter;

	private MeshRenderer mmeshrender;

	public new Transform light;

	private CustomProjector proj;

	public SkinnedMeshRenderer skintarget;

	public Vector3[] vertis;

	public Matrix4x4 prjmat = default(Matrix4x4);

	private Quaternion staticrot = Quaternion.identity;

	private Vector3 staticpos = Vector3.zero;

	public string currclip = string.Empty;

	public string npcid = string.Empty;

	private static Dictionary<string, Texture2D> texHolder = new Dictionary<string, Texture2D>();

	public static bool usefakeshadow = false;

	private Vector3[] dr0 = new Vector3[]
	{
		new Vector3(1f, 0f, 0f),
		new Vector3(0f, 0f, 1f),
		new Vector3(-1f, 0f, 0f),
		new Vector3(0f, 0f, -1f)
	};

	public void setvis(bool vis)
	{
		if (!RecieverObjCtrl.usefakeshadow)
		{
			return;
		}
		if (this.mmeshrender == null)
		{
			return;
		}
		this.mmeshrender.enabled = vis;
		if (!vis)
		{
		}
	}

	public void initialize(Vector3 ps, Quaternion qt)
	{
		this.staticrot = qt;
		this.staticpos = ps;
	}

	public void create0()
	{
	}

	public Mesh createMesh()
	{
		if (this.vertis == null)
		{
			return this.forcecreateMesh(new Vector3(0f, -0.8f, 0.6f));
		}
		Vector3[] array = new Vector3[4];
		int[] triangles = new int[]
		{
			0,
			1,
			2,
			2,
			1,
			3
		};
		Mesh mesh = new Mesh();
		mesh.name = "autoShadow_Awake";
		array[0] = (array[1] = (array[2] = (array[3] = Vector3.up)));
		Vector3 a = Vector3.zero;
		for (int i = 0; i < 4; i++)
		{
			a += this.vertis[i];
		}
		a *= 0.25f;
		for (int j = 0; j < 4; j++)
		{
			this.vertis[j].z = a.z + (this.vertis[j].z - a.z) * 1f;
		}
		mesh.vertices = this.vertis;
		mesh.normals = array;
		mesh.triangles = triangles;
		mesh.RecalculateBounds();
		return mesh;
	}

	public Material createMat()
	{
		return new Material(Shader.Find("newshader/Shadow_atlas"))
		{
			name = "mat_Awake"
		};
	}

	private void Awake()
	{
	}

	public void setnpcid(string id)
	{
		this.npcid = id;
	}

	public void doStart()
	{
		if (!RecieverObjCtrl.usefakeshadow)
		{
			return;
		}
		this.mmeshfilter = base.GetComponent<MeshFilter>();
		this.mmeshrender = base.GetComponent<MeshRenderer>();
		this.mmat = this.mmeshrender.sharedMaterial;
		if (this.mmesh == null)
		{
			this.mmesh = this.createMesh();
		}
		if (this.mmat == null)
		{
			this.mmat = this.createMat();
			this.updatashadowtex();
		}
		this.mmeshfilter.mesh = this.mmesh;
		this.mmeshrender.material = this.mmat;
		this.updataproj3();
		this.updataproj2();
	}

	public void updataproj3()
	{
		Matrix4x4 lhs = default(Matrix4x4);
		Matrix4x4 rhs = default(Matrix4x4);
		lhs.SetRow(0, new Vector4(0.5f, 0f, 0f, 0.5f));
		lhs.SetRow(1, new Vector4(0f, 0.5f, 0f, 0.5f));
		lhs.SetRow(2, new Vector4(0f, 0f, 0.5f, 0.5f));
		lhs.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
		rhs.SetRow(0, new Vector4(1f / this.m_orthographicSize, 0f, 0f, 0f));
		rhs.SetRow(1, new Vector4(0f, 1f / this.m_orthographicSize, 0f, 0f));
		rhs.SetRow(2, new Vector4(0f, 0f, -2f / (this.m_farClipPlane - this.m_nearClipPlane), 0f));
		rhs.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
		this.prjmat = lhs * rhs * Matrix4x4.TRS(new Vector3(0f, 4.05f, -2.3f), Quaternion.Euler(54.29196f, 0f, 0f), new Vector3(1f, 1f, 1f)).inverse;
	}

	public void updataproj()
	{
		if (this.skintarget == null)
		{
			this.skintarget = base.transform.parent.GetComponentInChildren<SkinnedMeshRenderer>();
		}
		this.skintarget.sharedMaterial.SetMatrix("_xProjector", this.proj.prjMatrix());
		this.skintarget.sharedMaterial.SetVector("_fwd", this.light.forward);
	}

	public void updataproj1(Matrix4x4 mat)
	{
		this.prjmat = mat;
		this.mmat.SetMatrix("_xProjector", this.prjmat);
	}

	public void updataproj2()
	{
		this.mmat.SetMatrix("_xProjector", this.prjmat);
	}

	public void setclip(string clp)
	{
		if (!RecieverObjCtrl.usefakeshadow)
		{
			return;
		}
		if (this.currclip != clp)
		{
			this.currclip = clp;
			this.updatashadowtex();
		}
	}

	public static void releaseAllTex()
	{
		RecieverObjCtrl.texHolder.Clear();
	}

	public void preloadtex()
	{
		string str = this.npcid;
		string text = "ShadowAnim/Shadow_" + str + "_" + this.currclip;
		if (RecieverObjCtrl.texHolder.ContainsKey(text))
		{
			Texture texture = RecieverObjCtrl.texHolder[text];
		}
		else
		{
			Texture texture = Resources.Load(text, typeof(Texture)) as Texture;
			RecieverObjCtrl.texHolder.Add(text, (Texture2D)texture);
		}
	}

	public void updatashadowtex()
	{
		if (this.mmat == null)
		{
			this.mmat = base.GetComponent<MeshRenderer>().sharedMaterial;
			if (this.mmat == null)
			{
				return;
			}
		}
		if (this.skintarget == null)
		{
			return;
		}
		string name = this.skintarget.transform.parent.parent.name;
		if (this.npcid != string.Empty)
		{
			name = this.npcid;
		}
		string text = "ShadowAnim/Shadow_" + name + "_" + this.currclip;
		Texture texture;
		if (RecieverObjCtrl.texHolder.ContainsKey(text))
		{
			texture = RecieverObjCtrl.texHolder[text];
		}
		else
		{
			texture = (Resources.Load(text, typeof(Texture)) as Texture);
			RecieverObjCtrl.texHolder.Add(text, (Texture2D)texture);
		}
		if (texture != null)
		{
			this.mmat.SetTexture("_xShadowTex", texture);
		}
	}

	public bool GetPlaneIntersectionT(Vector3 ldir, Vector3[] vertices, Plane plane)
	{
		if (this.skintarget == null)
		{
			return false;
		}
		if (0f <= Vector3.Dot(ldir, plane.normal))
		{
			vertices[0] = (vertices[1] = (vertices[2] = (vertices[3] = Vector3.zero)));
			return true;
		}
		GameObject gameObject = this.skintarget.transform.parent.parent.gameObject;
		Vector3 right = gameObject.transform.right;
		Vector3 up = gameObject.transform.up;
		Vector3 forward = gameObject.transform.forward;
		BoxCollider boxCollider = gameObject.GetComponentInChildren<BoxCollider>();
		if (boxCollider == null)
		{
			Renderer renderer = this.skintarget;
			boxCollider = renderer.gameObject.AddComponent<BoxCollider>();
		}
		Bounds bounds = boxCollider.bounds;
		boxCollider = gameObject.GetComponentInChildren<BoxCollider>();
		if (boxCollider != null)
		{
			UnityEngine.Object.DestroyImmediate(boxCollider);
		}
		else
		{
			boxCollider = gameObject.GetComponent<BoxCollider>();
			if (boxCollider != null)
			{
				UnityEngine.Object.DestroyImmediate(boxCollider);
			}
		}
		float num = Mathf.Abs(Vector3.Dot(right, ldir));
		float num2 = bounds.extents.x * Mathf.Sqrt(Mathf.Max(0f, 1f - num * num));
		float num3 = Mathf.Abs(Vector3.Dot(up, ldir));
		float num4 = bounds.extents.y * Mathf.Sqrt(Mathf.Max(0f, 1f - num3 * num3));
		float num5 = Mathf.Abs(Vector3.Dot(forward, ldir));
		float num6 = bounds.extents.z * Mathf.Sqrt(Mathf.Max(0f, 1f - num5 * num5));
		Vector3 vector;
		if (num4 <= num2 && num6 <= num2)
		{
			vector = gameObject.transform.right;
		}
		else if (num2 <= num4 && num6 <= num4)
		{
			vector = gameObject.transform.up;
		}
		else
		{
			vector = gameObject.transform.forward;
		}
		Vector3 vector2 = Vector3.Cross(ldir, vector).normalized;
		vector = Vector3.Cross(vector2, ldir);
		num2 = bounds.extents.x * Mathf.Abs(Vector3.Dot(right, vector)) + bounds.extents.y * Mathf.Abs(Vector3.Dot(up, vector)) + bounds.extents.z * Mathf.Abs(Vector3.Dot(forward, vector));
		num4 = bounds.extents.x * Mathf.Abs(Vector3.Dot(right, vector2)) + bounds.extents.y * Mathf.Abs(Vector3.Dot(up, vector2)) + bounds.extents.z * Mathf.Abs(Vector3.Dot(forward, vector2));
		Vector3 position = base.transform.position;
		vector *= num2;
		vector2 *= num4;
		Vector3 vector3 = position - vector - vector2;
		Vector3 vector4 = position - vector + vector2;
		Vector3 vector5 = position + vector - vector2;
		Vector3 vector6 = position + vector + vector2;
		float num7 = 1f / Vector3.Dot(ldir, plane.normal);
		vertices[0] = vector3 - num7 * (plane.distance + Vector3.Dot(vector3, plane.normal)) * ldir;
		vertices[1] = vector4 - num7 * (plane.distance + Vector3.Dot(vector4, plane.normal)) * ldir;
		vertices[2] = vector5 - num7 * (plane.distance + Vector3.Dot(vector5, plane.normal)) * ldir;
		vertices[3] = vector6 - num7 * (plane.distance + Vector3.Dot(vector6, plane.normal)) * ldir;
		return true;
	}

	public Mesh forcecreateMesh(Vector3 ldr)
	{
		Vector3[] array = new Vector3[4];
		int[] triangles = new int[]
		{
			0,
			1,
			2,
			2,
			1,
			3
		};
		Mesh mesh = new Mesh();
		mesh.name = "autoShadow_force";
		Plane plane = new Plane(new Vector3(0f, 1f, 0f), 0f);
		this.vertis = new Vector3[4];
		if (!this.GetPlaneIntersectionT(ldr, this.vertis, plane))
		{
			this.vertis = null;
			return null;
		}
		array[0] = (array[1] = (array[2] = (array[3] = plane.normal)));
		mesh.vertices = this.vertis;
		mesh.normals = array;
		mesh.triangles = triangles;
		mesh.RecalculateBounds();
		return mesh;
	}

	public void updatamesh3()
	{
		this.mmesh = this.createMesh();
	}

	public void updatamesh()
	{
		if (this.light == null)
		{
			return;
		}
		if (this.proj == null)
		{
			this.proj = this.light.GetComponent<CustomProjector>();
		}
		if (this.mmeshfilter == null)
		{
			this.mmeshfilter = base.GetComponent<MeshFilter>();
		}
		this.mmeshfilter.mesh = this.proj.createRecieverMesh(this.vertis);
	}

	public void updatamat()
	{
		this.mmat = base.GetComponent<MeshRenderer>().sharedMaterial;
		if (this.mmat == null)
		{
			return;
		}
	}

	public void setsize(int row, int col)
	{
		if (this.mmat == null)
		{
			this.mmat = base.GetComponent<MeshRenderer>().sharedMaterial;
			if (this.mmat == null)
			{
				return;
			}
		}
		this.mmat.SetFloat("_row", (float)row);
		this.mmat.SetFloat("_col", (float)col);
	}

	public void setframe(float curr)
	{
		if (!RecieverObjCtrl.usefakeshadow)
		{
			return;
		}
		if (this.mmat == null)
		{
			return;
		}
		curr = ((curr != 64f) ? curr : 63f);
		this.mmat.SetFloat("_Count", curr);
	}

	private void updateorent()
	{
		Transform parent = this.skintarget.transform.parent.parent;
		for (int i = 0; i < 4; i++)
		{
			float num = Vector3.Dot(parent.forward, this.dr0[i]);
			if (num > 0.5f)
			{
				this.mmat.SetFloat("_orent", (float)i);
				return;
			}
		}
	}

	private void Update()
	{
		if (!RecieverObjCtrl.usefakeshadow)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (this.skintarget == null)
		{
			this.skintarget = base.transform.parent.GetComponentInChildren<SkinnedMeshRenderer>();
		}
		if (this.skintarget == null)
		{
			return;
		}
		if (this.mmat == null)
		{
			this.mmat = base.GetComponent<MeshRenderer>().sharedMaterial;
			if (this.mmat == null)
			{
				return;
			}
		}
		this.updateorent();
		base.transform.rotation = this.staticrot;
		base.transform.localPosition = this.staticpos + Vector3.up * 0.2f;
	}
}
