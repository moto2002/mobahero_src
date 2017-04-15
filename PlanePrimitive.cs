using System;
using UnityEngine;

[AddComponentMenu("")]
public class PlanePrimitive : Primitive
{
	public float Width = 1f;

	public float Length = 1f;

	public int WidthSegments = 1;

	public int LengthSegments = 1;

	protected override void UpdateMeshFunc()
	{
		bool flag = this.Width != 0f && this.Length != 0f;
		if (flag)
		{
			int num = this.WidthSegments + 1;
			int num2 = this.LengthSegments + 1;
			int num3 = num * num2;
			Vector3[] array = new Vector3[num3];
			Vector2[] array2 = new Vector2[num3];
			Color[] array3 = new Color[num3];
			Vector3[] array4 = new Vector3[num3];
			Vector2[] array5 = new Vector2[num3];
			Vector2[] array6 = new Vector2[num3];
			int[] array7 = new int[this.WidthSegments * this.LengthSegments * 6];
			int num4 = 0;
			float num5 = 1f / (float)this.WidthSegments;
			float num6 = 1f / (float)this.LengthSegments;
			float num7 = this.Width * num5;
			float num8 = this.Length * num6;
			float num9 = this.Width * 0.5f;
			float num10 = this.Length * 0.5f;
			for (float num11 = 0f; num11 < (float)num2; num11 += 1f)
			{
				for (float num12 = 0f; num12 < (float)num; num12 += 1f)
				{
					array[num4] = new Vector3(num12 * num7 - num9, num11 * num8 - num10, 0f);
					Vector2[] array8 = array2;
					int num13 = num4;
					num4 = num13 + 1;
					array8[num13] = new Vector2(num12 * num5, num11 * num6);
					array3[num13] = Color.white;
					if (this.needexParam)
					{
						array4[num13] = Vector3.zero;
					}
					if (this.needexParam1)
					{
						array5[num13] = new Vector2(1f, 1f);
					}
					if (this.needexParam2)
					{
						array6[num13] = Vector2.zero;
					}
				}
			}
			num4 = 0;
			for (int i = 0; i < this.LengthSegments; i++)
			{
				for (int j = 0; j < this.WidthSegments; j++)
				{
					array7[num4] = i * num + j;
					array7[num4 + 1] = (i + 1) * num + j;
					array7[num4 + 2] = i * num + j + 1;
					array7[num4 + 3] = (i + 1) * num + j;
					array7[num4 + 4] = (i + 1) * num + j + 1;
					array7[num4 + 5] = i * num + j + 1;
					num4 += 6;
				}
			}
			this.mMesh.vertices = array;
			this.mMesh.uv = array2;
			if (this.needexParam1)
			{
				this.mMesh.uv1 = array5;
			}
			if (this.needexParam2)
			{
				this.mMesh.uv2 = array6;
			}
			this.mMesh.colors = array3;
			this.mMesh.triangles = array7;
			if (this.needexParam)
			{
				this.mMesh.normals = array4;
			}
			else
			{
				this.mMesh.RecalculateNormals();
			}
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
		boxCollider.size = new Vector3(this.Width, 0f, this.Length);
	}
}
