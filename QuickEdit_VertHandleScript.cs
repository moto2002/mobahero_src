using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class QuickEdit_VertHandleScript : MonoBehaviour
{
	public bool isActive;

	public bool isSelected;

	private List<int> myVertIndices = new List<int>();

	public Mesh mMesh;

	public int mVertIdx;

	public int mUvIdx;

	public int mClrIdx;

	public Vector2 mUV = Vector3.zero;

	public Color mColor = Color.white;

	public void Activate(Mesh mesh, int vertidx, int uvidx, int clridx)
	{
		this.isActive = true;
		this.mMesh = mesh;
		this.mVertIdx = vertidx;
		this.mClrIdx = clridx;
		this.mUvIdx = uvidx;
		Vector2[] uv = mesh.uv;
		Color[] colors = mesh.colors;
		if (uv != null && uvidx < uv.Length)
		{
			this.mUV = uv[uvidx];
		}
		if (colors != null && clridx < colors.Length)
		{
			this.mColor = colors[clridx];
		}
	}

	public void Update()
	{
		if (this.mMesh == null)
		{
			return;
		}
		Vector3[] vertices = this.mMesh.vertices;
		Color[] colors = this.mMesh.colors;
		Vector2[] uv = this.mMesh.uv;
		if (vertices != null && this.mVertIdx < vertices.Length)
		{
			vertices[this.mVertIdx] = base.transform.parent.InverseTransformPoint(base.transform.position);
		}
		if (colors != null && this.mClrIdx < colors.Length)
		{
			colors[this.mClrIdx] = this.mColor;
		}
		if (uv != null && this.mUvIdx < uv.Length)
		{
			uv[this.mUvIdx] = this.mUV;
		}
		this.mMesh.vertices = vertices;
		this.mMesh.colors = colors;
		this.mMesh.uv = uv;
	}

	public void AddVertIndex(int vertIndex)
	{
		this.myVertIndices.Add(vertIndex);
	}

	public void UpdateAttachedVerts(Mesh theMesh)
	{
		Vector3[] vertices = theMesh.vertices;
		for (int i = 0; i < this.myVertIndices.Count; i++)
		{
			vertices[i] = base.transform.parent.InverseTransformPoint(base.transform.position);
		}
		theMesh.vertices = vertices;
	}

	private void OnDrawGizmos()
	{
		if (this.isActive)
		{
			if (this.isSelected)
			{
				Gizmos.DrawIcon(base.transform.position, "vertpic/VertOn.tga", false);
			}
			else
			{
				Gizmos.DrawIcon(base.transform.position, "vertpic/VertOff.tga", false);
			}
		}
	}
}
