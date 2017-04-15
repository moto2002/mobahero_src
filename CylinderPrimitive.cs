using System;
using UnityEngine;

[AddComponentMenu("")]
public class CylinderPrimitive : Primitive
{
	public float ColliderDetails = 1f;

	public float Radius = 0.5f;

	public float Height = 1f;

	public int Sides = 12;

	public int HeightSegments = 1;

	protected override void UpdateMeshFunc()
	{
		bool flag = this.Radius != 0f && this.Height != 0f;
		if (flag)
		{
			this.mMesh.name = "Cylinder";
			float num = this.Height * 0.5f;
			int num2 = this.HeightSegments + 1;
			float num3 = this.Height / (float)this.HeightSegments;
			float num4 = 1f / (float)this.Sides;
			int num5 = 0;
			int num6 = 0;
			Vector3[] array = new Vector3[(this.Sides + 1) * 2 + this.Sides * num2 + num2];
			Vector2[] array2 = new Vector2[array.Length];
			Vector3[] array3 = new Vector3[array.Length];
			int[] array4 = new int[this.Sides * 6 + this.Sides * this.HeightSegments * 6];
			array[num5] = new Vector3(0f, num, 0f);
			array3[num5] = new Vector3(0f, 1f, 0f);
			array2[num5] = new Vector2(0.5f, 0.5f);
			num5++;
			for (int i = 0; i < this.Sides; i++)
			{
				float f = (float)i / (float)this.Sides * 6.28318548f;
				float num7 = Mathf.Cos(f);
				float num8 = Mathf.Sin(f);
				array[num5] = new Vector3(this.Radius * num7, num, this.Radius * num8);
				array3[num5] = new Vector3(0f, 1f, 0f);
				array2[num5] = new Vector2(num7 * 0.5f + 0.5f, num8 * 0.5f + 0.5f);
				num5++;
			}
			for (int i = 0; i < this.Sides - 1; i++)
			{
				array4[num6] = 0;
				array4[num6 + 1] = i + 2;
				array4[num6 + 2] = i + 1;
				num6 += 3;
			}
			array4[num6] = 0;
			array4[num6 + 1] = 1;
			array4[num6 + 2] = this.Sides;
			num6 += 3;
			array[num5] = new Vector3(0f, -num, 0f);
			array3[num5] = new Vector3(0f, -1f, 0f);
			array2[num5] = new Vector2(0.5f, 0.5f);
			num5++;
			for (int i = 0; i < this.Sides; i++)
			{
				float f2 = (float)i / (float)this.Sides * 6.28318548f;
				float num9 = Mathf.Cos(f2);
				float num10 = Mathf.Sin(f2);
				array[num5] = new Vector3(this.Radius * num9, -num, this.Radius * num10);
				array3[num5] = new Vector3(0f, -1f, 0f);
				array2[num5] = new Vector2(-num9 * 0.5f + 0.5f, num10 * 0.5f + 0.5f);
				num5++;
			}
			int num11 = this.Sides + 1;
			for (int i = 0; i < this.Sides - 1; i++)
			{
				array4[num6] = num11;
				array4[num6 + 1] = num11 + i + 1;
				array4[num6 + 2] = num11 + i + 2;
				num6 += 3;
			}
			array4[num6] = num11;
			array4[num6 + 1] = num11 + this.Sides;
			array4[num6 + 2] = num11 + 1;
			num6 += 3;
			for (int j = 0; j < num2; j++)
			{
				for (int i = 0; i < this.Sides + 1; i++)
				{
					float f3 = (float)i / (float)this.Sides * 6.28318548f;
					float y = -num + num3 * (float)j;
					float num12 = Mathf.Cos(f3);
					float num13 = Mathf.Sin(f3);
					array[num5] = new Vector3(this.Radius * num12, y, this.Radius * num13);
					array3[num5] = new Vector3(num12, 0f, num13);
					array2[num5] = new Vector2((float)i * num4, num3 * (float)j / this.Height);
					num5++;
				}
			}
			int num14 = num11 + (this.Sides + 1);
			for (int j = 0; j < num2 - 1; j++)
			{
				for (int i = 0; i < this.Sides; i++)
				{
					int num15 = j * (this.Sides + 1) + i;
					int num16 = j * (this.Sides + 1) + (i + 1);
					int num17 = (j + 1) * (this.Sides + 1) + (i + 1);
					int num18 = (j + 1) * (this.Sides + 1) + i;
					array4[num6] = num14 + num15;
					array4[num6 + 1] = num14 + num17;
					array4[num6 + 2] = num14 + num16;
					array4[num6 + 3] = num14 + num17;
					array4[num6 + 4] = num14 + num15;
					array4[num6 + 5] = num14 + num18;
					num6 += 6;
				}
			}
			this.mMesh.vertices = array;
			this.mMesh.uv = array2;
			this.mMesh.triangles = array4;
			this.mMesh.normals = array3;
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
		mesh.name = "CylinderCollider";
		float num = this.Height * 0.5f;
		int num2 = 2;
		int num3 = 0;
		int num4 = 0;
		int num5 = (int)Mathf.Max(3f, (float)this.Sides * this.ColliderDetails);
		Vector3[] array = new Vector3[(num5 + 1) * 2 + num5 * num2 + num2];
		int[] array2 = new int[this.Sides * 6 + this.Sides * 6];
		array[num3] = new Vector3(0f, num, 0f);
		num3++;
		for (int i = 0; i < num5; i++)
		{
			float f = (float)i / (float)num5 * 6.28318548f;
			float num6 = Mathf.Cos(f);
			float num7 = Mathf.Sin(f);
			array[num3] = new Vector3(this.Radius * num6, num, this.Radius * num7);
			num3++;
		}
		for (int i = 0; i < num5 - 1; i++)
		{
			array2[num4] = 0;
			array2[num4 + 1] = i + 2;
			array2[num4 + 2] = i + 1;
			num4 += 3;
		}
		array2[num4] = 0;
		array2[num4 + 1] = 1;
		array2[num4 + 2] = num5;
		num4 += 3;
		array[num3] = new Vector3(0f, -num, 0f);
		num3++;
		for (int i = 0; i < num5; i++)
		{
			float f2 = (float)i / (float)num5 * 6.28318548f;
			float num8 = Mathf.Cos(f2);
			float num9 = Mathf.Sin(f2);
			array[num3] = new Vector3(this.Radius * num8, -num, this.Radius * num9);
			num3++;
		}
		int num10 = num5 + 1;
		for (int i = 0; i < num5 - 1; i++)
		{
			array2[num4] = num10;
			array2[num4 + 1] = num10 + i + 1;
			array2[num4 + 2] = num10 + i + 2;
			num4 += 3;
		}
		array2[num4] = num10;
		array2[num4 + 1] = num10 + num5;
		array2[num4 + 2] = num10 + 1;
		num4 += 3;
		for (int j = 0; j < num2; j++)
		{
			for (int i = 0; i < num5 + 1; i++)
			{
				float f3 = (float)i / (float)num5 * 6.28318548f;
				float y = -num + this.Height * (float)j;
				float num11 = Mathf.Cos(f3);
				float num12 = Mathf.Sin(f3);
				array[num3] = new Vector3(this.Radius * num11, y, this.Radius * num12);
				num3++;
			}
		}
		int num13 = num10 + (num5 + 1);
		for (int j = 0; j < num2 - 1; j++)
		{
			for (int i = 0; i < num5; i++)
			{
				int num14 = j * (num5 + 1) + i;
				int num15 = j * (num5 + 1) + (i + 1);
				int num16 = (j + 1) * (num5 + 1) + (i + 1);
				int num17 = (j + 1) * (num5 + 1) + i;
				array2[num4] = num13 + num14;
				array2[num4 + 1] = num13 + num16;
				array2[num4 + 2] = num13 + num15;
				array2[num4 + 3] = num13 + num16;
				array2[num4 + 4] = num13 + num14;
				array2[num4 + 5] = num13 + num17;
				num4 += 6;
			}
		}
		mesh.vertices = array;
		mesh.triangles = array2;
		mesh.RecalculateBounds();
		meshCollider.sharedMesh = mesh;
	}
}
