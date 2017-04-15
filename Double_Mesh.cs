using System;
using UnityEngine;

public class Double_Mesh : MonoBehaviour
{
	private Vector3 centerposition;

	private Vector3[] vertices = new Vector3[18];

	private Vector3[] key_point = new Vector3[6];

	private Vector3[] unit_point = new Vector3[6];

	private Vector2[] uvs = new Vector2[18];

	private Color32[] color32 = new Color32[18];

	public float[] value = new float[6];

	private int[] triangles = new int[18];

	private int num_poly = 6;

	private int index1;

	private int index2;

	public void init()
	{
		this.num_poly = 6;
		this.GenerateMesh();
	}

	private void GenerateMesh()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		MeshRenderer component2 = base.GetComponent<MeshRenderer>();
		this.ConfirmPoint();
		for (int i = 0; i < this.num_poly * 3; i++)
		{
			this.triangles[i] = i;
		}
		for (int j = 0; j < this.num_poly * 3; j++)
		{
			switch (j % 3)
			{
			case 0:
				this.uvs[j] = new Vector2(0f, 0f);
				break;
			case 1:
				this.uvs[j] = new Vector2(0f, 1f);
				break;
			case 2:
				this.uvs[j] = new Vector2(1f, 1f);
				break;
			}
		}
		for (int k = 0; k < this.num_poly * 3; k++)
		{
			this.color32[k] = new Color32(139, 0, 85, 219);
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
			this.key_point[i] = this.value[i] * this.unit_point[i];
			Debug.Log(this.key_point[i]);
		}
		for (int j = 0; j < this.num_poly * 3; j++)
		{
			switch (j % 3)
			{
			case 0:
				this.vertices[j] = Vector3.zero;
				break;
			case 1:
				this.vertices[j] = this.key_point[this.index1];
				this.index1 = ((this.index1 < this.num_poly - 1) ? (this.index1 + 1) : 0);
				break;
			case 2:
				this.index2 = ((this.index2 < this.num_poly - 1) ? (this.index2 + 1) : 0);
				this.vertices[j] = this.key_point[this.index2];
				break;
			}
		}
	}
}
