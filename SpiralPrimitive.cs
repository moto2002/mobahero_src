using System;
using UnityEngine;

[AddComponentMenu("")]
public class SpiralPrimitive : Primitive
{
	public float Radius = 0.5f;

	public float Ynums = 2f;

	public float YHeigh = 2f;

	public float startwidth = 1f;

	public float endwidth = 1f;

	public float angle = 1f;

	public float heigh = 1f;

	public int LatitudeSegments = 1;

	public int LongitudeSegments = 12;

	public AnimationCurve Ymorph;

	protected override void UpdateMeshFunc()
	{
		if (this.Radius != 0f)
		{
			Vector3[] array = new Vector3[(this.LatitudeSegments + 1) * (this.LongitudeSegments + 1)];
			Vector2[] array2 = new Vector2[array.Length];
			Vector3[] array3 = new Vector3[array.Length];
			int num = 0;
			int[] array4 = new int[this.LatitudeSegments * this.LongitudeSegments * 6];
			for (int i = 0; i < this.LongitudeSegments + 1; i++)
			{
				float num2 = (float)i / (float)this.LongitudeSegments;
				float num3 = 1f;
				if (this.Ymorph != null)
				{
					num3 = this.Ymorph.Evaluate(num2);
				}
				Vector3 vector = new Vector3(Mathf.Cos(6.28318548f * num2 * this.Ynums) * this.Radius * (num3 + 1f), num2 * this.YHeigh, Mathf.Sin(6.28318548f * num2 * this.Ynums) * this.Radius * (num3 + 1f));
				Vector3 to = vector;
				to.y = 0f;
				to.Normalize();
				float d = Mathf.Lerp(this.startwidth, this.endwidth, num2);
				Vector3 vector2 = Vector3.Lerp(Vector3.up, to, this.angle);
				array[num] = vector + vector2 * d * 0.5f;
				array[num + 1] = vector - vector2 * d * 0.5f;
				array2[num] = new Vector2(num2, 0f);
				array2[num + 1] = new Vector2(num2, 1f);
				array3[num] = vector2;
				array3[num + 1] = vector2;
				num += 2;
			}
			int num4 = 0;
			for (int j = 0; j < array.Length / 2 - 1; j++)
			{
				int num5 = j * 2;
				array4[num4] = num5;
				array4[num4 + 1] = num5 + 2;
				array4[num4 + 2] = num5 + 1;
				array4[num4 + 3] = num5 + 1;
				array4[num4 + 4] = num5 + 2;
				array4[num4 + 5] = num5 + 3;
				num4 += 6;
			}
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
