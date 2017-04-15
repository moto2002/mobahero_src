using System;
using UnityEngine;

[AddComponentMenu("")]
public class TorusPrimitive : Primitive
{
	public float ColliderDetails = 1f;

	public float OuterRadius = 1f;

	public float InnerRadius = 0.5f;

	public int Sides = 12;

	public int Segments = 12;

	protected override void UpdateMeshFunc()
	{
		bool flag = this.OuterRadius != 0f && this.InnerRadius != 0f && this.InnerRadius < this.OuterRadius;
		if (flag)
		{
			this.mMesh.name = "Torus";
			this.Segments++;
			Vector3[] array = new Vector3[(this.Sides + 1) * (this.Segments + 1)];
			Vector2[] array2 = new Vector2[array.Length];
			Vector3[] array3 = new Vector3[array.Length];
			int[] array4 = new int[this.Sides * this.Segments * 6];
			int num = 0;
			int num2 = 0;
			for (int i = 0; i <= this.Sides; i++)
			{
				for (int j = 0; j <= this.Segments; j++)
				{
					float num3 = (float)j / ((float)this.Segments - 1f);
					float num4 = (float)i / ((float)this.Sides - 1f);
					float f = num3 * 6.28318548f;
					float f2 = num4 * 6.28318548f;
					float num5 = Mathf.Cos(f);
					float num6 = Mathf.Sin(f);
					float num7 = Mathf.Cos(f2);
					float num8 = Mathf.Sin(f2);
					float x = this.OuterRadius * num5 + this.InnerRadius * num5 * num7;
					float z = this.OuterRadius * num6 + this.InnerRadius * num6 * num7;
					float y = this.InnerRadius * num8;
					float x2 = num5 * num7;
					float z2 = num6 * num7;
					float y2 = num8;
					array[num] = new Vector3(x, y, z);
					array2[num] = new Vector2(num3, num4);
					array3[num] = new Vector3(x2, y2, z2);
					num++;
				}
			}
			for (int i = 1; i <= this.Sides; i++)
			{
				for (int j = 1; j <= this.Segments; j++)
				{
					int num9 = (this.Segments + 1) * i + j - 1;
					int num10 = (this.Segments + 1) * (i - 1) + j - 1;
					int num11 = (this.Segments + 1) * (i - 1) + j;
					int num12 = (this.Segments + 1) * i + j;
					array4[num2] = num10;
					array4[num2 + 1] = num9;
					array4[num2 + 2] = num11;
					array4[num2 + 3] = num11;
					array4[num2 + 4] = num9;
					array4[num2 + 5] = num12;
					num2 += 6;
				}
			}
			this.Segments--;
			this.mMesh.vertices = array;
			this.mMesh.uv = array2;
			this.mMesh.normals = array3;
			this.mMesh.triangles = array4;
			this.mMesh.RecalculateBounds();
		}
	}

	protected override void UpdateColliderFunc()
	{
		if (base.collider == null)
		{
			base.gameObject.AddComponent<MeshCollider>();
		}
		else if (!(base.collider is MeshCollider))
		{
			base.RemoveCollider();
			base.gameObject.AddComponent<MeshCollider>();
		}
		MeshCollider meshCollider = base.collider as MeshCollider;
		Mesh mesh = new Mesh();
		mesh.name = "TorusCollider";
		this.Segments++;
		int num = (int)Mathf.Max(4f, (float)this.Segments * this.ColliderDetails);
		int num2 = (int)Mathf.Max(4f, (float)this.Sides * this.ColliderDetails);
		Vector3[] array = new Vector3[(num2 + 1) * (num + 1)];
		int[] array2 = new int[num2 * num * 6];
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i <= num2; i++)
		{
			for (int j = 0; j <= num; j++)
			{
				float num5 = (float)j / ((float)num - 1f);
				float num6 = (float)i / ((float)num2 - 1f);
				float f = num5 * 6.28318548f;
				float f2 = num6 * 6.28318548f;
				float num7 = Mathf.Cos(f);
				float num8 = Mathf.Sin(f);
				float num9 = Mathf.Cos(f2);
				float num10 = Mathf.Sin(f2);
				float x = this.OuterRadius * num7 + this.InnerRadius * num7 * num9;
				float z = this.OuterRadius * num8 + this.InnerRadius * num8 * num9;
				float y = this.InnerRadius * num10;
				array[num3] = new Vector3(x, y, z);
				num3++;
			}
		}
		for (int i = 1; i <= num2; i++)
		{
			for (int j = 1; j <= num; j++)
			{
				int num11 = (num + 1) * i + j - 1;
				int num12 = (num + 1) * (i - 1) + j - 1;
				int num13 = (num + 1) * (i - 1) + j;
				int num14 = (num + 1) * i + j;
				array2[num4] = num12;
				array2[num4 + 1] = num11;
				array2[num4 + 2] = num13;
				array2[num4 + 3] = num13;
				array2[num4 + 4] = num11;
				array2[num4 + 5] = num14;
				num4 += 6;
			}
		}
		this.Segments--;
		mesh.vertices = array;
		mesh.triangles = array2;
		mesh.RecalculateBounds();
		meshCollider.sharedMesh = mesh;
	}
}
