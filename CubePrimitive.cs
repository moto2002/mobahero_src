using System;
using UnityEngine;

[AddComponentMenu("")]
public class CubePrimitive : Primitive
{
	public float Width = 1f;

	public float Length = 1f;

	public float Height = 1f;

	public int WidthSegments = 1;

	public int LengthSegments = 1;

	public int HeightSegments = 1;

	protected override void UpdateMeshFunc()
	{
		bool flag = this.Width != 0f && this.Length != 0f;
		if (flag)
		{
			this.mMesh.name = "Cube";
			int num = this.WidthSegments + 1;
			int num2 = this.LengthSegments + 1;
			int num3 = this.HeightSegments + 1;
			int num4 = num * num2;
			int num5 = num3 * num2;
			int num6 = num3 * num;
			Vector3[] array = new Vector3[num4 * 2 + num5 * 2 + num6 * 2];
			Vector2[] array2 = new Vector2[array.Length];
			int[] array3 = new int[this.WidthSegments * this.LengthSegments * 12 + this.LengthSegments * this.HeightSegments * 12 + this.HeightSegments * this.WidthSegments * 12];
			int num7 = 0;
			int num8 = 0;
			float num9 = 1f / (float)this.WidthSegments;
			float num10 = 1f / (float)this.LengthSegments;
			float num11 = 1f / (float)this.HeightSegments;
			float num12 = this.Width * num9;
			float num13 = this.Length * num10;
			float num14 = this.Height * num11;
			float num15 = this.Width * 0.5f;
			float num16 = this.Length * 0.5f;
			float num17 = this.Height * 0.5f;
			int num18 = 0;
			for (float num19 = 0f; num19 < (float)num2; num19 += 1f)
			{
				for (float num20 = 0f; num20 < (float)num; num20 += 1f)
				{
					array[num7] = new Vector3(num20 * num12 - num15, num17, num19 * num13 - num16);
					Vector2[] array4 = array2;
					int num21 = num7;
					num7 = num21 + 1;
					array4[num21] = new Vector2(num20 * num9, num19 * num10);
				}
			}
			for (int i = 0; i < this.LengthSegments; i++)
			{
				for (int j = 0; j < this.WidthSegments; j++)
				{
					array3[num8] = i * num + j;
					array3[num8 + 1] = (i + 1) * num + j;
					array3[num8 + 2] = i * num + j + 1;
					array3[num8 + 3] = (i + 1) * num + j;
					array3[num8 + 4] = (i + 1) * num + j + 1;
					array3[num8 + 5] = i * num + j + 1;
					num8 += 6;
				}
			}
			int num22 = num18 + num4;
			for (float num19 = 0f; num19 < (float)num2; num19 += 1f)
			{
				for (float num20 = 0f; num20 < (float)num; num20 += 1f)
				{
					array[num7] = new Vector3(num20 * num12 - num15, -num17, num19 * num13 - num16);
					Vector2[] array5 = array2;
					int num23 = num7;
					num7 = num23 + 1;
					array5[num23] = new Vector2(1f - num20 * num9, num19 * num10);
				}
			}
			for (int i = 0; i < this.LengthSegments; i++)
			{
				for (int j = 0; j < this.WidthSegments; j++)
				{
					array3[num8 + 1] = num22 + i * num + j;
					array3[num8] = num22 + (i + 1) * num + j;
					array3[num8 + 2] = num22 + i * num + j + 1;
					array3[num8 + 4] = num22 + (i + 1) * num + j;
					array3[num8 + 3] = num22 + (i + 1) * num + j + 1;
					array3[num8 + 5] = num22 + i * num + j + 1;
					num8 += 6;
				}
			}
			int num24 = num22 + num4;
			for (float num19 = 0f; num19 < (float)num3; num19 += 1f)
			{
				for (float num20 = 0f; num20 < (float)num2; num20 += 1f)
				{
					array[num7] = new Vector3(num15, num19 * num14 - num17, num20 * num13 - num16);
					Vector2[] array6 = array2;
					int num25 = num7;
					num7 = num25 + 1;
					array6[num25] = new Vector2(num20 * num10, num19 * num11);
				}
			}
			for (int i = 0; i < this.HeightSegments; i++)
			{
				for (int j = 0; j < this.LengthSegments; j++)
				{
					array3[num8] = num24 + i * num2 + j;
					array3[num8 + 1] = num24 + (i + 1) * num2 + j;
					array3[num8 + 2] = num24 + i * num2 + j + 1;
					array3[num8 + 3] = num24 + (i + 1) * num2 + j;
					array3[num8 + 4] = num24 + (i + 1) * num2 + j + 1;
					array3[num8 + 5] = num24 + i * num2 + j + 1;
					num8 += 6;
				}
			}
			int num26 = num24 + num5;
			for (float num19 = 0f; num19 < (float)num3; num19 += 1f)
			{
				for (float num20 = 0f; num20 < (float)num2; num20 += 1f)
				{
					array[num7] = new Vector3(-num15, num19 * num14 - num17, num20 * num13 - num16);
					Vector2[] array7 = array2;
					int num27 = num7;
					num7 = num27 + 1;
					array7[num27] = new Vector2(1f - num20 * num10, num19 * num11);
				}
			}
			for (int i = 0; i < this.HeightSegments; i++)
			{
				for (int j = 0; j < this.LengthSegments; j++)
				{
					array3[num8 + 1] = num26 + i * num2 + j;
					array3[num8] = num26 + (i + 1) * num2 + j;
					array3[num8 + 2] = num26 + i * num2 + j + 1;
					array3[num8 + 4] = num26 + (i + 1) * num2 + j;
					array3[num8 + 3] = num26 + (i + 1) * num2 + j + 1;
					array3[num8 + 5] = num26 + i * num2 + j + 1;
					num8 += 6;
				}
			}
			int num28 = num26 + num5;
			for (float num19 = 0f; num19 < (float)num3; num19 += 1f)
			{
				for (float num20 = 0f; num20 < (float)num; num20 += 1f)
				{
					array[num7] = new Vector3(num20 * num12 - num15, num19 * num14 - num17, num16);
					Vector2[] array8 = array2;
					int num29 = num7;
					num7 = num29 + 1;
					array8[num29] = new Vector2(1f - num20 * num9, num19 * num11);
				}
			}
			for (int i = 0; i < this.HeightSegments; i++)
			{
				for (int j = 0; j < this.WidthSegments; j++)
				{
					array3[num8 + 1] = num28 + i * num + j;
					array3[num8] = num28 + (i + 1) * num + j;
					array3[num8 + 2] = num28 + i * num + j + 1;
					array3[num8 + 4] = num28 + (i + 1) * num + j;
					array3[num8 + 3] = num28 + (i + 1) * num + j + 1;
					array3[num8 + 5] = num28 + i * num + j + 1;
					num8 += 6;
				}
			}
			int num30 = num28 + num6;
			for (float num19 = 0f; num19 < (float)num3; num19 += 1f)
			{
				for (float num20 = 0f; num20 < (float)num; num20 += 1f)
				{
					array[num7] = new Vector3(num20 * num12 - num15, num19 * num14 - num17, -num16);
					Vector2[] array9 = array2;
					int num31 = num7;
					num7 = num31 + 1;
					array9[num31] = new Vector2(num20 * num9, num19 * num11);
				}
			}
			for (int i = 0; i < this.HeightSegments; i++)
			{
				for (int j = 0; j < this.WidthSegments; j++)
				{
					array3[num8] = num30 + i * num + j;
					array3[num8 + 1] = num30 + (i + 1) * num + j;
					array3[num8 + 2] = num30 + i * num + j + 1;
					array3[num8 + 3] = num30 + (i + 1) * num + j;
					array3[num8 + 4] = num30 + (i + 1) * num + j + 1;
					array3[num8 + 5] = num30 + i * num + j + 1;
					num8 += 6;
				}
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
			base.gameObject.AddComponent<BoxCollider>();
		}
		else if (!(base.collider is BoxCollider))
		{
			base.RemoveCollider();
			base.gameObject.AddComponent<BoxCollider>();
		}
		BoxCollider boxCollider = base.collider as BoxCollider;
		boxCollider.size = new Vector3(this.Width, this.Height, this.Length);
	}
}
