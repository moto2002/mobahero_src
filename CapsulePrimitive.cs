using System;
using UnityEngine;

[AddComponentMenu("")]
public class CapsulePrimitive : Primitive
{
	public float Radius = 0.5f;

	public float Height = 2f;

	public int HeightSegments = 1;

	public int Rings = 6;

	public int Sides = 12;

	protected override void UpdateMeshFunc()
	{
		bool flag = this.Radius != 0f && this.Height != 0f;
		if (flag)
		{
			this.mMesh.name = "Capsule";
			int num = this.Rings + 1;
			float num2 = this.Height * 0.5f;
			float num3 = this.Height - this.Radius * 2f;
			float num4 = num3 * 0.5f;
			float num5 = (this.Height - num3) * 0.5f;
			int num6 = this.HeightSegments + 1;
			float num7 = num3 / (float)this.HeightSegments;
			this.Sides++;
			int num8 = this.Rings * 2 + 1;
			Vector3[] array = new Vector3[(num8 + 1) * this.Sides + num6 * this.Sides];
			Vector2[] array2 = new Vector2[array.Length];
			Vector3[] array3 = new Vector3[array.Length];
			int[] array4 = new int[this.Rings * 2 * (this.Sides - 1) * 6 + this.HeightSegments * (this.Sides - 1) * 6];
			float num9 = 1f / (float)(num8 - 1);
			float num10 = 1f / (float)(this.Sides - 1);
			int num11 = 0;
			int num12 = 0;
			int num13 = num11;
			for (int i = 0; i < num6; i++)
			{
				for (int j = 0; j < this.Sides; j++)
				{
					float f = (float)j / (float)(this.Sides - 1) * 6.28318548f;
					float num14 = -num4 + num7 * (float)i;
					float num15 = Mathf.Cos(f);
					float num16 = Mathf.Sin(f);
					array[num11] = new Vector3(this.Radius * num15, num14, this.Radius * num16);
					array3[num11] = new Vector3(num15, 0f, num16);
					array2[num11] = new Vector2((float)j * num10, MathU.scale(num14, -(num4 + num5), num4 + num5, 0f, 1f));
					num11++;
				}
			}
			for (int i = 0; i < num6 - 1; i++)
			{
				for (int j = 0; j < this.Sides - 1; j++)
				{
					int num17 = num13 + i * this.Sides + j;
					int num18 = num13 + i * this.Sides + (j + 1);
					int num19 = num13 + (i + 1) * this.Sides + (j + 1);
					int num20 = num13 + (i + 1) * this.Sides + j;
					array4[num12] = num17;
					array4[num12 + 1] = num19;
					array4[num12 + 2] = num18;
					array4[num12 + 3] = num19;
					array4[num12 + 4] = num17;
					array4[num12 + 5] = num20;
					num12 += 6;
				}
			}
			this.Sides--;
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
			base.gameObject.AddComponent<CapsuleCollider>();
		}
		else if (!(base.collider is CapsuleCollider))
		{
			base.RemoveCollider();
			base.gameObject.AddComponent<CapsuleCollider>();
		}
		CapsuleCollider capsuleCollider = base.collider as CapsuleCollider;
		capsuleCollider.radius = this.Radius;
		capsuleCollider.height = this.Height;
	}
}
