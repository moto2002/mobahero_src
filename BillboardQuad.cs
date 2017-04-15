using System;
using UnityEngine;

[AddComponentMenu("")]
public class BillboardQuad : Primitive
{
	public float Width = 1f;

	public float Length = 1f;

	public int WidthSegments = 1;

	public int LengthSegments = 1;

	public bool verticle;

	public override void UpdateMaterial()
	{
		if (this.defaultMaterial == null)
		{
			this.defaultMaterial = new Material(Shader.Find(this.shader));
			base.renderer.material = this.defaultMaterial;
		}
	}

	protected override void UpdateMeshFunc()
	{
		if (this.isrenew)
		{
			this.newMesh = this.mMesh;
			this.mMesh = new Mesh();
		}
		Matrix4x4 inverse = base.transform.localToWorldMatrix.inverse;
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
			bool flag2 = base.transform.rotation != Quaternion.identity;
			for (float num11 = 0f; num11 < (float)num2; num11 += 1f)
			{
				for (float num12 = 0f; num12 < (float)num; num12 += 1f)
				{
					array[num4] = new Vector3(num12 * num7 - num9, num11 * num8 - num10, 0f);
					Vector3 vector = new Vector3(1f, 1f, 1f);
					if (!this.isbillboard && flag2)
					{
						array[num4] = base.transform.rotation * array[num4];
					}
					Vector2[] array8 = array2;
					int num13 = num4;
					num4 = num13 + 1;
					array8[num13] = new Vector2(num12 * num5, num11 * num6);
					if (this.isbillboard)
					{
						array3[num13] = new Color(array2[num13].x, array2[num13].y, 1f);
					}
					else
					{
						Vector2 vector2 = array2[num13] - new Vector2(0.5f, 0.5f);
						vector2 = base.transform.rotation * vector2;
						float num14 = 0.7071067f;
						vector2 += new Vector2(num14, num14);
						vector2 /= num14 + num14;
						array3[num13] = new Color(vector2.x, vector2.y, 1f);
					}
					if (this.needexParam)
					{
						array4[num13] = this.exParam;
					}
					if (this.needexParam1)
					{
						array5[num13] = this.exParam1;
					}
					if (this.needexParam2)
					{
						array6[num13] = this.exParam2;
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
			if (flag2)
			{
				base.transform.rotation = Quaternion.identity;
			}
			if ((this.exParam1.x != 1f || this.exParam1.y != 1f) && this.needexParam1)
			{
				Vector3 localScale = base.transform.localScale;
				if (localScale.x != 1f || localScale.y != 1f)
				{
					base.transform.localScale = new Vector3(1f, 1f, 1f);
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
		if (this.isrenew)
		{
			this.isrenew = false;
			Mesh mMesh = this.mMesh;
			this.mMesh = this.newMesh;
			base.GetComponent<MeshFilter>().mesh = this.mMesh;
			this.newMesh = mMesh;
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
