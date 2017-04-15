using System;
using System.Collections.Generic;
using UnityEngine;

public class Mesh_Circle : MonoBehaviour
{
	private float centerpoint;

	private Vector3[] vertices;

	private Vector3[] key_point_out = new Vector3[6];

	private Vector3[] key_point_in = new Vector3[6];

	private Vector3[] unit_point = new Vector3[6];

	private Vector2[] uvs = new Vector2[12];

	private Color32[] color32 = new Color32[12];

	public float[] value_out = new float[6];

	public float[] value_in = new float[6];

	private int[] triangles;

	private int num_poly;

	private int num_random;

	private List<Vector3> pointsOut = new List<Vector3>();

	private List<Vector3> pointsIn = new List<Vector3>();

	public void Init()
	{
		this.num_poly = 6;
		for (int i = 0; i < 6; i++)
		{
			this.value_out[i] = this.value_in[i] + 0.03f;
		}
		this.GenerateMesh();
	}

	private void DrawTheLine()
	{
		this.vertices = new Vector3[this.num_poly * 2];
		this.triangles = new int[this.num_poly * 3 * 2];
		for (int i = 0; i < this.num_poly; i++)
		{
			this.vertices[i * 2] = this.pointsOut[i];
			this.vertices[i * 2 + 1] = this.pointsIn[i];
			if (i < this.num_poly - 1)
			{
				this.triangles[6 * i] = 2 * (i + 1);
				this.triangles[6 * i + 1] = 2 * (i + 1) - 1;
				this.triangles[6 * i + 2] = 2 * i;
				this.triangles[6 * i + 3] = 2 * (i + 1) + 1;
				this.triangles[6 * i + 4] = 2 * (i + 1) - 1;
				this.triangles[6 * i + 5] = 2 * (i + 1);
			}
			else
			{
				this.triangles[6 * i] = 0;
				this.triangles[6 * i + 1] = 2 * (i + 1) - 1;
				this.triangles[6 * i + 2] = 2 * i;
				this.triangles[6 * i + 3] = 1;
				this.triangles[6 * i + 4] = 2 * this.num_poly - 1;
				this.triangles[6 * i + 5] = 0;
			}
		}
	}

	private void GenerateMesh()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		MeshRenderer component2 = base.GetComponent<MeshRenderer>();
		this.ConfirmPoint();
		for (int i = 0; i < this.num_poly * 2; i++)
		{
			switch (i % 3)
			{
			case 0:
				this.uvs[i] = new Vector2(0f, 0f);
				break;
			case 1:
				this.uvs[i] = new Vector2(0f, 1f);
				break;
			case 2:
				this.uvs[i] = new Vector2(1f, 1f);
				break;
			}
		}
		for (int j = 0; j < this.num_poly * 2; j++)
		{
			this.color32[j] = new Color32(255, 0, 132, 255);
		}
		Mesh mesh = new Mesh();
		mesh.hideFlags = HideFlags.DontSave;
		mesh.vertices = this.vertices;
		mesh.triangles = this.triangles;
		mesh.colors32 = this.color32;
		mesh.uv = this.uvs;
		mesh.RecalculateNormals();
		component.mesh = mesh;
	}

	private void ConfirmPoint()
	{
		for (int i = 0; i < this.num_poly; i++)
		{
			float f = (float)(2 * i) * 3.14159274f / (float)this.num_poly;
			this.unit_point[i] = new Vector3(Mathf.Sin(f), Mathf.Cos(f), 0f);
			this.key_point_out[i] = this.value_out[i] * this.unit_point[i];
			this.key_point_in[i] = this.value_in[i] * this.unit_point[i];
			this.pointsOut.Add(this.key_point_out[i]);
			this.pointsIn.Add(this.key_point_in[i]);
		}
		this.DrawTheLine();
	}
}
