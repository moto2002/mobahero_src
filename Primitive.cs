using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu(""), ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Primitive : MonoBehaviour
{
	public bool EditorMode;

	public Vector4 mParam = Vector4.zero;

	public Vector3 exParam = Vector3.zero;

	public Vector2 exParam1 = new Vector2(1f, 1f);

	public Vector2 exParam2 = Vector2.zero;

	public List<Transform> vertHandles;

	public bool isbillboard;

	public bool needexParam;

	public bool needexParam1;

	public bool needexParam2;

	public string shader = "Diffuse";

	public Mesh custommesh;

	public bool FlipNormals;

	public bool FlipUVsVertical;

	public bool FlipUVsHorizontal;

	public bool GenerateCollider;

	public bool OptimizeMesh = true;

	public bool GenerateTangents = true;

	public Mesh mMesh;

	public Material defaultMaterial;

	public bool iscustomemesh;

	public AnimationCurve morphcurve;

	public bool isrenew;

	public Mesh newMesh;

	public Quaternion bakRot = Quaternion.identity;

	public virtual void UpdatePrimitiveEx()
	{
	}

	private void OnEnable()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (this.mMesh == null)
		{
			this.mMesh = new Mesh();
			component.mesh = this.mMesh;
		}
	}

	public void RefreshNormal()
	{
		if (this.mMesh == null)
		{
			return;
		}
		Vector3[] normals = this.mMesh.normals;
		for (int i = 0; i < normals.Length; i++)
		{
			normals[i] = new Vector3(this.exParam.x, this.exParam.y, this.exParam.z);
		}
		this.mMesh.normals = normals;
	}

	public void RefreshUV1()
	{
		if (this.mMesh == null)
		{
			return;
		}
		Vector2[] uv = this.mMesh.uv1;
		for (int i = 0; i < uv.Length; i++)
		{
			uv[i] = new Vector2(this.exParam1.x, this.exParam1.y);
		}
		this.mMesh.uv1 = uv;
	}

	public void RefreshUV2()
	{
		if (this.mMesh == null)
		{
			return;
		}
		Vector2[] uv = this.mMesh.uv2;
		for (int i = 0; i < uv.Length; i++)
		{
			uv[i] = new Vector2(this.exParam2.x, this.exParam2.y);
		}
		this.mMesh.uv2 = uv;
	}

	public void RefreshColor(int idx, int k = -1)
	{
		if (this.mMesh == null)
		{
			return;
		}
		Color[] colors = this.mMesh.colors;
		if (!this.isbillboard)
		{
			for (int i = 0; i < colors.Length; i++)
			{
				colors[i][idx] = ((k >= 0) ? ((float)k) : this.mParam[idx]);
			}
		}
		else
		{
			for (int j = 0; j < colors.Length; j++)
			{
				Color color = colors[j];
				color.b = this.mParam.z;
				color.a = this.mParam.w;
				colors[j] = color;
			}
		}
		this.mMesh.colors = colors;
	}

	public void UpdateAll()
	{
		this.UpdateMaterial();
		this.UpdatePrimitive();
		GPUParticleCtrl gPUParticleCtrl = base.gameObject.GetComponent<GPUParticleCtrl>();
		if (gPUParticleCtrl == null)
		{
			gPUParticleCtrl = base.gameObject.AddComponent<GPUParticleCtrl>();
			gPUParticleCtrl.setCtrlObject(base.gameObject);
		}
	}

	public virtual void UpdateMaterial()
	{
		if (this.defaultMaterial == null)
		{
			this.defaultMaterial = new Material(Shader.Find(this.shader));
			base.renderer.material = this.defaultMaterial;
		}
	}

	private void OnDisable()
	{
	}

	private void Reset()
	{
	}

	public void setNew(bool rn)
	{
		this.isrenew = rn;
		this.bakRot = base.transform.rotation;
	}

	public void closeNew()
	{
		base.transform.rotation = this.bakRot;
	}

	public Mesh getNew()
	{
		if (this.newMesh != null)
		{
			return this.newMesh;
		}
		return this.mMesh;
	}

	public void UpdatePrimitive()
	{
		if (!this.iscustomemesh)
		{
			this.mMesh.Clear();
		}
		this.UpdateMeshFunc();
		if (this.FlipNormals)
		{
			this.ReverseNormals();
		}
		if (this.FlipUVsHorizontal)
		{
			this.ReverseHorizontalUVs();
		}
		if (this.FlipUVsVertical)
		{
			this.ReverseVerticalUVs();
		}
		if (this.GenerateTangents)
		{
			this.RecalculateTangents();
		}
		if (this.OptimizeMesh)
		{
			this.mMesh.Optimize();
		}
		if (this.GenerateCollider)
		{
			this.UpdateColliderFunc();
		}
		else
		{
			this.RemoveCollider();
		}
	}

	protected void ReverseNormals()
	{
		Vector3[] normals = this.mMesh.normals;
		for (int i = 0; i < normals.Length; i++)
		{
			normals[i] = -normals[i];
		}
		this.mMesh.normals = normals;
		for (int j = 0; j < this.mMesh.subMeshCount; j++)
		{
			int[] triangles = this.mMesh.GetTriangles(j);
			for (int i = 0; i < triangles.Length; i += 3)
			{
				int num = triangles[i];
				triangles[i] = triangles[i + 1];
				triangles[i + 1] = num;
			}
			this.mMesh.SetTriangles(triangles, j);
		}
	}

	protected void ReverseHorizontalUVs()
	{
		Vector2[] uv = this.mMesh.uv;
		for (int i = 0; i < uv.Length; i++)
		{
			uv[i] = new Vector2(-uv[i].x, uv[i].y);
		}
		this.mMesh.uv = uv;
	}

	protected void ReverseVerticalUVs()
	{
		Vector2[] uv = this.mMesh.uv;
		for (int i = 0; i < uv.Length; i++)
		{
			uv[i] = new Vector2(uv[i].x, -uv[i].y);
		}
		this.mMesh.uv = uv;
	}

	protected void RemoveCollider()
	{
		if (Application.isEditor && base.collider != null)
		{
			UnityEngine.Object.DestroyImmediate(base.collider);
		}
	}

	protected void RecalculateTangents()
	{
	}

	protected virtual void UpdateMeshFunc()
	{
		if (this.custommesh != null)
		{
			base.GetComponent<MeshFilter>().mesh = this.custommesh;
			base.GetComponent<MeshFilter>().sharedMesh = this.custommesh;
			this.mMesh = this.custommesh;
			if (!this.needexParam)
			{
				this.mMesh.RecalculateNormals();
			}
			this.mMesh.RecalculateBounds();
		}
	}

	protected virtual void UpdateColliderFunc()
	{
	}
}
