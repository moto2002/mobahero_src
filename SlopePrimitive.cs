using System;
using UnityEngine;

[AddComponentMenu("")]
public class SlopePrimitive : Primitive
{
	public float Width = 1f;

	public float Length = 1f;

	public float Height = 1f;

	public int WidthSegments = 1;

	public int LengthSegments = 1;

	protected override void UpdateMeshFunc()
	{
		bool flag = this.Width != 0f && this.Length != 0f && this.Height != 0f;
		if (flag)
		{
			this.mMesh.name = "Slope";
			int num = this.WidthSegments + 1;
			int num2 = this.LengthSegments + 1;
			int num3 = num * num2;
			Vector3[] array = new Vector3[num3 * 3 + (1 + num2) * num2 / 2 * 2];
			Vector2[] array2 = new Vector2[array.Length];
			int[] array3 = new int[this.WidthSegments * this.LengthSegments * 6 * 3 + this.LengthSegments * this.LengthSegments * 6];
			int num4 = 0;
			int num5 = 0;
			float num6 = 1f / (float)this.WidthSegments;
			float num7 = 1f / (float)this.LengthSegments;
			float num8 = this.Width * num6;
			float num9 = this.Length * num7;
			float num10 = this.Width * 0.5f;
			float num11 = this.Length * 0.5f;
			float num12 = this.Height / (float)this.LengthSegments;
			for (float num13 = 0f; num13 < (float)num2; num13 += 1f)
			{
				for (float num14 = 0f; num14 < (float)num; num14 += 1f)
				{
					array[num4] = new Vector3(num14 * num8 - num10, 0f, num13 * num9 - num11);
					Vector2[] array4 = array2;
					int num15 = num4;
					num4 = num15 + 1;
					array4[num15] = new Vector2(num14 * num6, 1f - num13 * num7);
				}
			}
			for (int i = 0; i < this.LengthSegments; i++)
			{
				for (int j = 0; j < this.WidthSegments; j++)
				{
					array3[num5] = (i + 1) * num + j;
					array3[num5 + 1] = i * num + j;
					array3[num5 + 2] = i * num + j + 1;
					array3[num5 + 3] = (i + 1) * num + j + 1;
					array3[num5 + 4] = (i + 1) * num + j;
					array3[num5 + 5] = i * num + j + 1;
					num5 += 6;
				}
			}
			int num16 = num4;
			for (float num13 = 0f; num13 < (float)num2; num13 += 1f)
			{
				for (float num14 = 0f; num14 < (float)num; num14 += 1f)
				{
					array[num4] = new Vector3(num14 * num8 - num10, num12 * num13, num13 * num9 - num11);
					Vector2[] array5 = array2;
					int num17 = num4;
					num4 = num17 + 1;
					array5[num17] = new Vector2(num14 * num6, num13 * num7);
				}
			}
			for (int i = 0; i < this.LengthSegments; i++)
			{
				for (int j = 0; j < this.WidthSegments; j++)
				{
					array3[num5] = num16 + i * num + j;
					array3[num5 + 1] = num16 + (i + 1) * num + j + 1;
					array3[num5 + 2] = num16 + i * num + j + 1;
					array3[num5 + 3] = num16 + i * num + j;
					array3[num5 + 4] = num16 + (i + 1) * num + j;
					array3[num5 + 5] = num16 + (i + 1) * num + j + 1;
					num5 += 6;
				}
			}
			num16 = num4;
			for (float num13 = 0f; num13 < (float)num2; num13 += 1f)
			{
				for (float num14 = 0f; num14 < (float)num; num14 += 1f)
				{
					array[num4] = new Vector3(num14 * num8 - num10, num13 * num12, num11);
					Vector2[] array6 = array2;
					int num18 = num4;
					num4 = num18 + 1;
					array6[num18] = new Vector2(1f - num14 * num6, num13 * num7);
				}
			}
			for (int i = 0; i < this.LengthSegments; i++)
			{
				for (int j = 0; j < this.WidthSegments; j++)
				{
					array3[num5] = num16 + (i + 1) * num + j + 1;
					array3[num5 + 1] = num16 + (i + 1) * num + j;
					array3[num5 + 2] = num16 + i * num + j + 1;
					array3[num5 + 3] = num16 + (i + 1) * num + j;
					array3[num5 + 4] = num16 + i * num + j;
					array3[num5 + 5] = num16 + i * num + j + 1;
					num5 += 6;
				}
			}
			num16 = num4;
			int num19 = 0;
			for (float num13 = 0f; num13 < (float)num2; num13 += 1f)
			{
				for (float num14 = 0f; num14 < (float)(num2 - num19); num14 += 1f)
				{
					array[num4] = new Vector3(num10, num13 * num12, -num14 * num9 + num11);
					Vector2[] array7 = array2;
					int num20 = num4;
					num4 = num20 + 1;
					array7[num20] = new Vector2(-num14 * num7, num13 * num7);
				}
				num19++;
			}
			num19 = 0;
			for (int i = 0; i < this.LengthSegments; i++)
			{
				for (int j = 0; j < this.LengthSegments - num19; j++)
				{
					if (j < this.LengthSegments - num19 - 1)
					{
						array3[num5] = num16 + (num2 - num19) + j + 1;
						array3[num5 + 1] = num16 + (num2 - num19) + j;
						array3[num5 + 2] = num16 + j + 1;
						num5 += 3;
					}
					array3[num5] = num16 + j;
					array3[num5 + 1] = num16 + j + 1;
					array3[num5 + 2] = num16 + (num2 - num19) + j;
					num5 += 3;
				}
				num16 += num2 - num19;
				num19++;
			}
			num16 = num4;
			num19 = 0;
			for (float num13 = 0f; num13 < (float)num2; num13 += 1f)
			{
				for (float num14 = 0f; num14 < (float)(num2 - num19); num14 += 1f)
				{
					array[num4] = new Vector3(-num10, num13 * num12, -num14 * num9 + num11);
					Vector2[] array8 = array2;
					int num21 = num4;
					num4 = num21 + 1;
					array8[num21] = new Vector2(num14 * num7, num13 * num7);
				}
				num19++;
			}
			num19 = 0;
			for (int i = 0; i < this.LengthSegments; i++)
			{
				for (int j = 0; j < this.LengthSegments - num19; j++)
				{
					if (j < this.LengthSegments - num19 - 1)
					{
						array3[num5] = num16 + (num2 - num19) + j + 1;
						array3[num5 + 1] = num16 + j + 1;
						array3[num5 + 2] = num16 + (num2 - num19) + j;
						num5 += 3;
					}
					array3[num5] = num16 + j + 1;
					array3[num5 + 1] = num16 + j;
					array3[num5 + 2] = num16 + (num2 - num19) + j;
					num5 += 3;
				}
				num16 += num2 - num19;
				num19++;
			}
			this.mMesh.vertices = array;
			this.mMesh.uv = array2;
			this.mMesh.triangles = array3;
			this.mMesh.RecalculateNormals();
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
		mesh.name = "SlopeCollider";
		Vector3[] array = new Vector3[6];
		int[] array2 = new int[24];
		float num = this.Width * 0.5f;
		float num2 = this.Length * 0.5f;
		array[0] = new Vector3(-num, 0f, -num2);
		array[1] = new Vector3(num, 0f, -num2);
		array[2] = new Vector3(num, 0f, num2);
		array[3] = new Vector3(-num, 0f, num2);
		array[4] = new Vector3(num, this.Height, num2);
		array[5] = new Vector3(-num, this.Height, num2);
		array2[0] = 0;
		array2[1] = 1;
		array2[2] = 2;
		array2[3] = 2;
		array2[4] = 3;
		array2[5] = 0;
		array2[6] = 0;
		array2[7] = 4;
		array2[8] = 1;
		array2[9] = 0;
		array2[10] = 5;
		array2[11] = 4;
		array2[12] = 4;
		array2[13] = 3;
		array2[14] = 2;
		array2[15] = 4;
		array2[16] = 5;
		array2[17] = 3;
		array2[18] = 0;
		array2[19] = 3;
		array2[20] = 5;
		array2[21] = 1;
		array2[22] = 4;
		array2[23] = 2;
		mesh.vertices = array;
		mesh.triangles = array2;
		mesh.RecalculateBounds();
		meshCollider.sharedMesh = mesh;
		meshCollider.convex = true;
	}
}
