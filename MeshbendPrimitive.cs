using System;
using UnityEngine;

[AddComponentMenu("")]
public class MeshbendPrimitive : Primitive
{
	private float groundY;

	public float maxheight = 2f;

	public AnimationCurve curves;

	public void RefreshColorByCurve(int idx, AnimationCurve cv)
	{
		if (this.mMesh == null)
		{
			return;
		}
		Color[] colors = this.mMesh.colors;
		if (colors == null)
		{
			return;
		}
		Vector3[] vertices = this.mMesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			float time = (vertices[i].y - this.groundY) / this.maxheight;
			float value = cv.Evaluate(time);
			colors[i][idx] = value;
		}
		this.mMesh.colors = colors;
	}

	public override void UpdatePrimitiveEx()
	{
		Mesh sharedMesh = base.GetComponent<MeshFilter>().sharedMesh;
		if (sharedMesh != null)
		{
			sharedMesh.RecalculateNormals();
		}
	}

	protected override void UpdateMeshFunc()
	{
		if (this.custommesh != null)
		{
			base.GetComponent<MeshFilter>().mesh = this.custommesh;
			base.GetComponent<MeshFilter>().sharedMesh = this.custommesh;
			this.mMesh = this.custommesh;
			this.custommesh = null;
			if (!this.needexParam)
			{
				this.mMesh.RecalculateNormals();
			}
			this.mMesh.RecalculateBounds();
			this.groundY = this.mMesh.bounds.min.y;
		}
		else if (this.mMesh != null)
		{
			this.mMesh.RecalculateBounds();
			this.groundY = this.mMesh.bounds.min.y;
		}
		this.mMesh.uv2 = null;
		this.mMesh.RecalculateNormals();
	}

	protected override void UpdateColliderFunc()
	{
	}
}
