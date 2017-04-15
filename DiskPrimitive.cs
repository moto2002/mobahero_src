using System;
using UnityEngine;

[AddComponentMenu("")]
public class DiskPrimitive : Primitive
{
	public float Radius0;

	public float Radius1 = 1f;

	public int Sides = 14;

	public int depth = 10;

	protected override void UpdateMeshFunc()
	{
		this.mMesh.name = "Disk";
		int num = 0;
		float num2 = (this.Radius1 - this.Radius0) / (float)(this.depth - 1);
		float num3 = 1f / (float)(this.depth - 1);
		int num4 = this.Sides * this.depth;
		Vector3[] array = new Vector3[num4];
		Vector2[] array2 = new Vector2[num4];
		Vector3[] array3 = new Vector3[num4];
		Color[] array4 = new Color[num4];
		int[] array5 = new int[this.Sides * (this.depth - 1) * 6];
		int num5 = 0;
		for (int i = 0; i < this.depth; i++)
		{
			for (int j = 0; j < this.Sides; j++)
			{
				float num6 = 6.28318548f / (float)this.Sides;
				float x = Mathf.Cos(num6 * (float)j);
				float z = Mathf.Sin(num6 * (float)j);
				float d = this.Radius1 - (float)i * num2;
				float num7 = 1f - (float)i * num3;
				array[num] = new Vector3(x, 0f, z) * d;
				Vector2 vector = new Vector2(array[num].x, array[num].z) + new Vector2(this.Radius1, this.Radius1);
				vector *= 1f / (this.Radius1 * 2f);
				array2[num] = vector;
				array4[num] = Color.white;
				if (i < this.depth - 1)
				{
					if (j < this.Sides - 1)
					{
						array5[num5] = num;
						array5[num5 + 1] = num + this.Sides + 1;
						array5[num5 + 2] = num + this.Sides;
						array5[num5 + 3] = num;
						array5[num5 + 4] = num + 1;
						array5[num5 + 5] = num + this.Sides + 1;
						num5 += 6;
					}
					else
					{
						array5[num5] = num;
						array5[num5 + 1] = num + 1;
						array5[num5 + 2] = num + this.Sides;
						array5[num5 + 3] = num;
						array5[num5 + 4] = num - this.Sides + 1;
						array5[num5 + 5] = num + 1;
						num5 += 6;
					}
				}
				num++;
			}
		}
		Debug.Log("ti " + num5);
		this.mMesh.vertices = array;
		this.mMesh.uv = array2;
		this.mMesh.colors = array4;
		this.mMesh.triangles = array5;
		this.mMesh.RecalculateNormals();
		this.mMesh.RecalculateBounds();
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
	}
}
