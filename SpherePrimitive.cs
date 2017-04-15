using System;
using UnityEngine;

[AddComponentMenu("")]
public class SpherePrimitive : Primitive
{
	public float Radius = 0.5f;

	public int LatitudeSegments = 12;

	public int LongitudeSegments = 12;

	public float uvdistort;

	public AnimationCurve Ymorph;

	protected override void UpdateMeshFunc()
	{
		if (this.Radius != 0f)
		{
			this.LongitudeSegments++;
			Vector3[] array = new Vector3[this.LatitudeSegments * this.LongitudeSegments];
			Vector2[] array2 = new Vector2[array.Length];
			Vector3[] array3 = new Vector3[array.Length];
			int[] array4 = new int[(this.LatitudeSegments - 1) * (this.LongitudeSegments - 1) * 6];
			float num = 1f / (float)(this.LatitudeSegments - 1);
			float num2 = 1f / (float)(this.LongitudeSegments - 1);
			int num3 = 0;
			for (int i = 0; i < this.LatitudeSegments; i++)
			{
				for (int j = 0; j < this.LongitudeSegments; j++)
				{
					float num4 = (float)j * num2;
					float num5 = (float)i * num;
					float num6 = 1f;
					if (this.Ymorph != null)
					{
						num6 = this.Ymorph.Evaluate(num5);
					}
					float num7 = Mathf.Sin(-1.57079637f + 3.14159274f * num5);
					float num8 = Mathf.Cos(6.28318548f * num4) * Mathf.Sin(3.14159274f * num5);
					float num9 = Mathf.Sin(6.28318548f * num4) * Mathf.Sin(3.14159274f * num5);
					array[num3] = new Vector3(num8 * this.Radius, num7 * this.Radius * (num6 + 1f), num9 * this.Radius);
					num4 += num5 * this.uvdistort;
					if (num4 > 1f)
					{
						num4 -= (float)((int)num4);
					}
					array2[num3] = new Vector2(num4, num5);
					array3[num3] = new Vector3(num8, num7, num9);
					num3++;
				}
			}
			num3 = 0;
			for (int i = 0; i < this.LatitudeSegments - 1; i++)
			{
				for (int j = 0; j < this.LongitudeSegments - 1; j++)
				{
					int num10 = i * this.LongitudeSegments + j;
					int num11 = i * this.LongitudeSegments + (j + 1);
					int num12 = (i + 1) * this.LongitudeSegments + (j + 1);
					int num13 = (i + 1) * this.LongitudeSegments + j;
					array4[num3] = num10;
					array4[num3 + 1] = num12;
					array4[num3 + 2] = num11;
					array4[num3 + 3] = num12;
					array4[num3 + 4] = num10;
					array4[num3 + 5] = num13;
					num3 += 6;
				}
			}
			this.LongitudeSegments--;
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
			base.gameObject.AddComponent<SphereCollider>();
		}
		else if (!(base.collider is SphereCollider))
		{
			base.RemoveCollider();
			base.gameObject.AddComponent<SphereCollider>();
		}
		SphereCollider sphereCollider = base.collider as SphereCollider;
		sphereCollider.radius = this.Radius;
	}
}
