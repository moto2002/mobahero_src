using System;
using UnityEngine;

[AddComponentMenu("")]
public class PyramidPrimitive : Primitive
{
	public float Width = 1f;

	public float Length = 1f;

	public float Height = 1f;

	public int HeightSegments = 1;

	protected override void UpdateMeshFunc()
	{
		bool flag = this.Width != 0f && this.Length != 0f && this.Height != 0f;
		if (flag)
		{
			this.mMesh.name = "Pyramid";
			Vector3[] array = new Vector3[8 + this.HeightSegments * 2 * 4];
			Vector2[] array2 = new Vector2[array.Length];
			int[] array3 = new int[18 + (this.HeightSegments - 1) * 6 * 4];
			float num = this.Width * 0.5f;
			float num2 = this.Length * 0.5f;
			float num3 = this.Height / (float)this.HeightSegments;
			int num4 = 0;
			int num5 = 0;
			array[0] = new Vector3(-num, 0f, -num2);
			array[1] = new Vector3(-num, 0f, num2);
			array[2] = new Vector3(num, 0f, num2);
			array[3] = new Vector3(num, 0f, -num2);
			array2[0] = new Vector2(0f, 0f);
			array2[1] = new Vector2(1f, 0f);
			array2[2] = new Vector2(1f, 1f);
			array2[3] = new Vector2(0f, 1f);
			array3[0] = 3;
			array3[1] = 2;
			array3[2] = 1;
			array3[3] = 1;
			array3[4] = 0;
			array3[5] = 3;
			num5 += 6;
			array[4] = new Vector3(0f, this.Height, 0f);
			array[5] = new Vector3(0f, this.Height, 0f);
			array[6] = new Vector3(0f, this.Height, 0f);
			array[7] = new Vector3(0f, this.Height, 0f);
			array2[4] = new Vector2(0.5f, 1f);
			array2[5] = new Vector2(0.5f, 1f);
			array2[6] = new Vector2(0.5f, 1f);
			array2[7] = new Vector2(0.5f, 1f);
			num4 += 8;
			int num6 = num4;
			for (int i = 0; i < this.HeightSegments; i++)
			{
				float num7 = MathU.scale((float)i, 0f, (float)this.HeightSegments, num, 0f);
				float y = num3 * (float)i;
				float z = MathU.scale((float)i, 0f, (float)this.HeightSegments, num2, 0f);
				array[num4] = new Vector3(-num7, y, z);
				array[num4 + 1] = new Vector3(num7, y, z);
				array2[num4] = new Vector2(1f - (float)i / (float)this.HeightSegments * 0.5f, (float)i / (float)this.HeightSegments);
				array2[num4 + 1] = new Vector2((float)i / (float)this.HeightSegments * 0.5f, (float)i / (float)this.HeightSegments);
				num4 += 2;
			}
			for (int i = 0; i < this.HeightSegments - 1; i++)
			{
				array3[num5] = num6 + 2;
				array3[num5 + 1] = num6;
				array3[num5 + 2] = num6 + 1;
				array3[num5 + 3] = num6 + 3;
				array3[num5 + 4] = num6 + 2;
				array3[num5 + 5] = num6 + 1;
				num5 += 6;
				num6 += 2;
			}
			array3[num5] = num4 - 2;
			array3[num5 + 1] = num4 - 1;
			array3[num5 + 2] = 4;
			num5 += 3;
			num6 = num4;
			for (int i = 0; i < this.HeightSegments; i++)
			{
				float num8 = MathU.scale((float)i, 0f, (float)this.HeightSegments, num, 0f);
				float y2 = num3 * (float)i;
				float z2 = MathU.scale((float)i, 0f, (float)this.HeightSegments, -num2, 0f);
				array[num4] = new Vector3(-num8, y2, z2);
				array[num4 + 1] = new Vector3(num8, y2, z2);
				array2[num4] = new Vector2((float)i / (float)this.HeightSegments * 0.5f, (float)i / (float)this.HeightSegments);
				array2[num4 + 1] = new Vector2(1f - (float)i / (float)this.HeightSegments * 0.5f, (float)i / (float)this.HeightSegments);
				num4 += 2;
			}
			for (int i = 0; i < this.HeightSegments - 1; i++)
			{
				array3[num5] = num6 + 3;
				array3[num5 + 1] = num6 + 1;
				array3[num5 + 2] = num6;
				array3[num5 + 3] = num6;
				array3[num5 + 4] = num6 + 2;
				array3[num5 + 5] = num6 + 3;
				num5 += 6;
				num6 += 2;
			}
			array3[num5] = num4 - 1;
			array3[num5 + 1] = num4 - 2;
			array3[num5 + 2] = 5;
			num5 += 3;
			num6 = num4;
			for (int i = 0; i < this.HeightSegments; i++)
			{
				float x = MathU.scale((float)i, 0f, (float)this.HeightSegments, num, 0f);
				float y3 = num3 * (float)i;
				float num9 = MathU.scale((float)i, 0f, (float)this.HeightSegments, num2, 0f);
				array[num4] = new Vector3(x, y3, -num9);
				array[num4 + 1] = new Vector3(x, y3, num9);
				array2[num4] = new Vector2((float)i / (float)this.HeightSegments * 0.5f, (float)i / (float)this.HeightSegments);
				array2[num4 + 1] = new Vector2(1f - (float)i / (float)this.HeightSegments * 0.5f, (float)i / (float)this.HeightSegments);
				num4 += 2;
			}
			for (int i = 0; i < this.HeightSegments - 1; i++)
			{
				array3[num5] = num6;
				array3[num5 + 1] = num6 + 2;
				array3[num5 + 2] = num6 + 3;
				array3[num5 + 3] = num6 + 3;
				array3[num5 + 4] = num6 + 1;
				array3[num5 + 5] = num6;
				num5 += 6;
				num6 += 2;
			}
			array3[num5] = num4 - 1;
			array3[num5 + 1] = num4 - 2;
			array3[num5 + 2] = 6;
			num5 += 3;
			num6 = num4;
			for (int i = 0; i < this.HeightSegments; i++)
			{
				float x2 = MathU.scale((float)i, 0f, (float)this.HeightSegments, -num, 0f);
				float y4 = num3 * (float)i;
				float num10 = MathU.scale((float)i, 0f, (float)this.HeightSegments, num2, 0f);
				array[num4] = new Vector3(x2, y4, -num10);
				array[num4 + 1] = new Vector3(x2, y4, num10);
				array2[num4] = new Vector2(1f - (float)i / (float)this.HeightSegments * 0.5f, (float)i / (float)this.HeightSegments);
				array2[num4 + 1] = new Vector2((float)i / (float)this.HeightSegments * 0.5f, (float)i / (float)this.HeightSegments);
				num4 += 2;
			}
			for (int i = 0; i < this.HeightSegments - 1; i++)
			{
				array3[num5] = num6 + 1;
				array3[num5 + 1] = num6 + 3;
				array3[num5 + 2] = num6 + 2;
				array3[num5 + 3] = num6 + 2;
				array3[num5 + 4] = num6;
				array3[num5 + 5] = num6 + 1;
				num5 += 6;
				num6 += 2;
			}
			array3[num5] = num4 - 2;
			array3[num5 + 1] = num4 - 1;
			array3[num5 + 2] = 7;
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
		mesh.name = "PyramidCollider";
		Vector3[] array = new Vector3[5];
		int[] array2 = new int[18];
		float num = this.Width * 0.5f;
		float num2 = this.Length * 0.5f;
		array[0] = new Vector3(-num, 0f, -num2);
		array[1] = new Vector3(-num, 0f, num2);
		array[2] = new Vector3(num, 0f, num2);
		array[3] = new Vector3(num, 0f, -num2);
		array2[0] = 3;
		array2[1] = 2;
		array2[2] = 1;
		array2[3] = 1;
		array2[4] = 0;
		array2[5] = 3;
		array[4] = new Vector3(0f, this.Height, 0f);
		array2[6] = 0;
		array2[7] = 1;
		array2[8] = 4;
		array2[9] = 1;
		array2[10] = 2;
		array2[11] = 4;
		array2[12] = 2;
		array2[13] = 3;
		array2[14] = 4;
		array2[15] = 3;
		array2[16] = 0;
		array2[17] = 4;
		mesh.vertices = array;
		mesh.triangles = array2;
		mesh.RecalculateBounds();
		meshCollider.sharedMesh = mesh;
		meshCollider.convex = true;
	}
}
