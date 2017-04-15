using System;
using UnityEngine;

[AddComponentMenu("")]
public class MultiQuad : Primitive
{
	public float Width = 1f;

	public float Length = 1f;

	public int WidthSegments = 1;

	public int LengthSegments = 1;

	public int numparticles = 1;

	public float rollingrad0 = 1f;

	public float rollingrad1 = 1f;

	public float rndheigh = 1f;

	public float normalrange = 0.1f;

	public float alphasegments = 1f;

	public float alphagaps = 1f;

	public int shapeType = 1;

	public float rad0 = 1f;

	public float rad1 = 1f;

	public float rndalpha;

	public Vector3 linePoint0 = Vector3.zero;

	public Vector3 linePoint1 = Vector3.right * 10f;

	public AnimationCurve alpcurves0;

	public AnimationCurve alpcurves1;

	public AnimationCurve alpcurves2;

	private Quaternion acummulatedRot = Quaternion.identity;

	public override void UpdateMaterial()
	{
		if (this.defaultMaterial == null)
		{
			Shader shader = Shader.Find(this.shader);
			this.defaultMaterial = new Material(shader);
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
		bool flag = this.Width != 0f && this.Length != 0f;
		if (flag)
		{
			int num = this.WidthSegments + 1;
			int num2 = this.LengthSegments + 1;
			int num3 = num * num2 * this.numparticles;
			Vector3[] array = new Vector3[num3];
			Vector2[] array2 = new Vector2[num3];
			Vector2[] array3 = new Vector2[num3];
			Vector2[] array4 = new Vector2[num3];
			Color[] array5 = new Color[num3];
			Vector3[] array6 = new Vector3[num3];
			int[] array7 = new int[this.WidthSegments * this.LengthSegments * 6 * this.numparticles];
			int num4 = 0;
			int num5 = 0;
			float num6 = 1f / (float)this.WidthSegments;
			float num7 = 1f / (float)this.LengthSegments;
			float num8 = this.Width * num6;
			float num9 = this.Length * num7;
			float num10 = this.Width * 0.5f;
			float num11 = this.Length * 0.5f;
			float num12 = 0f;
			float b = 0f;
			float num13 = 0f;
			float num14 = 1f;
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			bool flag2 = base.transform.rotation != Quaternion.identity;
			if (flag2)
			{
				this.acummulatedRot = base.transform.rotation;
			}
			else
			{
				this.acummulatedRot = Quaternion.identity;
			}
			for (int i = 0; i < this.numparticles; i++)
			{
				if (this.shapeType == 0)
				{
					vector2 = new Vector3(UnityEngine.Random.RandomRange(-1000f, 1000f) * 0.001f, 0f, UnityEngine.Random.RandomRange(-1000f, 1000f) * 0.001f);
					vector2 *= this.normalrange;
					vector2.y = 1f;
					vector2.Normalize();
					vector = new Vector3(UnityEngine.Random.RandomRange(-1000f, 1000f) * 0.001f * this.rollingrad0, UnityEngine.Random.RandomRange(-1000f, 1000f) * 0.001f * this.rndheigh, UnityEngine.Random.RandomRange(-1000f, 1000f) * 0.001f * this.rollingrad1);
					if (this.rollingrad0 > this.rollingrad1)
					{
						vector += new Vector3(this.rollingrad0, 0f, 0f);
					}
					else
					{
						vector += new Vector3(0f, 0f, this.rollingrad1);
					}
					float num15 = this.rndalpha;
					float num16 = 1f / (float)this.numparticles;
					if (this.alpcurves0 != null)
					{
						num14 = this.alpcurves0.Evaluate(num16 * (float)i);
					}
					if (num15 < 0f)
					{
						num15 = -num15;
						num12 = UnityEngine.Random.RandomRange(0f, num15) / num15;
						b = UnityEngine.Random.RandomRange(0f, num15) / num15;
					}
					else if (num15 >= 0f)
					{
						num12 = num15;
						b = num15;
					}
					float num17 = vector.z / (this.rollingrad1 * 2f);
					if (this.alpcurves2 != null)
					{
						num13 = this.alpcurves2.Evaluate(num17);
					}
					else
					{
						num13 = num17;
					}
					if (this.alphagaps >= 0f)
					{
						num14 = this.alphagaps;
					}
					num14 = num13 + num14;
					num13 *= 0.5f;
					num14 *= 0.5f;
				}
				else if (this.shapeType == 1)
				{
					vector2 = new Vector3(UnityEngine.Random.RandomRange(-1000f, 1000f) * 0.001f, 0f, UnityEngine.Random.RandomRange(-1000f, 1000f) * 0.001f);
					vector2 *= this.normalrange;
					vector2.y = 1f;
					vector2.Normalize();
					float d = UnityEngine.Random.RandomRange(this.rollingrad0, this.rollingrad1);
					float f = UnityEngine.Random.Range(-3.14159274f, 3.14159274f);
					vector = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f)) * d;
					float num18 = this.rndalpha;
					if (num18 < 0f)
					{
						num18 = -num18;
						num12 = UnityEngine.Random.RandomRange(0f, num18) / num18;
						b = UnityEngine.Random.RandomRange(0f, num18) / num18;
					}
					else
					{
						num12 = num18;
						b = num18;
					}
				}
				else if (this.shapeType == 2)
				{
					vector2 = new Vector3(UnityEngine.Random.RandomRange(-1000f, 1000f) * 0.001f, 0f, UnityEngine.Random.RandomRange(-1000f, 1000f) * 0.001f);
					vector2 *= this.normalrange;
					vector2.y = 1f;
					vector2.Normalize();
					float num19 = 6.28318548f / (float)this.numparticles;
					vector = new Vector3(Mathf.Cos((float)i * num19) * this.rad0, 0f, Mathf.Sin((float)i * num19) * this.rad1);
					float num20 = 1f / (float)this.numparticles;
					float num21 = this.rndalpha;
					if (this.alpcurves0 != null)
					{
						num14 = this.alpcurves0.Evaluate(num20 * (float)i);
					}
					if (num21 < 0f)
					{
						num21 = -num21;
						num12 = UnityEngine.Random.RandomRange(0f, num21) / num21;
						b = 0f;
					}
					else
					{
						num12 = num20 * (float)i;
						if (this.alpcurves1 != null)
						{
							num12 = this.alpcurves1.Evaluate(num12);
						}
						b = 0f;
					}
					if (this.alpcurves2 != null)
					{
						num13 = this.alpcurves2.Evaluate(num20 * (float)i);
					}
					if (this.alphagaps >= 0f)
					{
						num14 = this.alphagaps;
					}
					num14 = num13 + num14;
					num13 *= 0.5f;
					num14 *= 0.5f;
				}
				else if (this.shapeType == 3)
				{
					vector2 = new Vector3(UnityEngine.Random.RandomRange(-1000f, 1000f) * 0.001f, 0f, UnityEngine.Random.RandomRange(-1000f, 1000f) * 0.001f);
					vector2 *= this.normalrange;
					vector2.y = 1f;
					vector2.Normalize();
					Vector3 a = this.linePoint1 - this.linePoint0;
					a.Normalize();
					float num22 = Vector3.Distance(this.linePoint0, this.linePoint1);
					float d2 = num22 / (float)this.numparticles;
					vector = this.linePoint0 + a * d2 * (float)i;
					float num23 = 1f / (float)this.numparticles;
					float num24 = this.rndalpha;
					if (this.alpcurves0 != null)
					{
						num14 = this.alpcurves0.Evaluate(num23 * (float)i);
					}
					if (num24 < 0f)
					{
						num24 = -num24;
						num12 = UnityEngine.Random.RandomRange(0f, num24) / num24;
						b = 0f;
					}
					else
					{
						num12 = num23 * (float)i;
						if (this.alpcurves1 != null)
						{
							num12 = this.alpcurves1.Evaluate(num12);
						}
						b = 0f;
					}
					if (this.alpcurves2 != null)
					{
						num13 = this.alpcurves2.Evaluate(num23 * (float)i);
					}
					if (this.alphagaps >= 0f)
					{
						num14 = this.alphagaps;
					}
					num14 = num13 + num14;
					num13 *= 0.5f;
					num14 *= 0.5f;
				}
				int num25 = num4;
				for (float num26 = 0f; num26 < (float)num2; num26 += 1f)
				{
					for (float num27 = 0f; num27 < (float)num; num27 += 1f)
					{
						Vector3 b2 = Vector3.zero;
						if (flag2)
						{
							b2 = base.transform.rotation * vector;
						}
						else
						{
							b2 = vector;
						}
						array[num4] = new Vector3(num27 * num8 - num10, num26 * num9 - num11, 0f) + b2;
						array2[num4] = new Vector2(num27 * num6, num26 * num7);
						array3[num4] = new Vector2(num13, num14);
						if (this.isbillboard)
						{
							array5[num4] = new Color(array2[num4].x, array2[num4].y, b, num12);
						}
						else
						{
							array5[num4] = new Color(1f, 1f, b, num12);
						}
						if (this.needexParam)
						{
							array6[num4] = vector2;
						}
						num4++;
					}
				}
				for (int j = 0; j < this.LengthSegments; j++)
				{
					for (int k = 0; k < this.WidthSegments; k++)
					{
						array7[num5] = num25 + j * num + k;
						array7[num5 + 1] = num25 + (j + 1) * num + k;
						array7[num5 + 2] = num25 + j * num + k + 1;
						array7[num5 + 3] = num25 + (j + 1) * num + k;
						array7[num5 + 4] = num25 + (j + 1) * num + k + 1;
						array7[num5 + 5] = num25 + j * num + k + 1;
						num5 += 6;
					}
				}
			}
			if (flag2)
			{
				base.transform.rotation = Quaternion.identity;
			}
			this.mMesh.vertices = array;
			this.mMesh.uv = array2;
			if (this.exParam1.x * this.exParam1.y == 1f)
			{
				this.mMesh.uv1 = array3;
			}
			this.mMesh.colors = array5;
			this.mMesh.triangles = array7;
			if (this.needexParam)
			{
				this.mMesh.normals = array6;
			}
			else
			{
				this.mMesh.RecalculateNormals();
			}
			this.mMesh.RecalculateBounds();
			if (this.isrenew)
			{
				this.isrenew = false;
				Mesh mMesh = this.mMesh;
				this.mMesh = this.newMesh;
				base.GetComponent<MeshFilter>().mesh = this.mMesh;
				this.newMesh = mMesh;
			}
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

	public void OnDrawGizmos()
	{
		Vector3 position = base.transform.position;
		Vector3 forward = base.transform.forward;
		Vector3 right = base.transform.right;
		Vector3 up = base.transform.up;
		if (this.shapeType == 0)
		{
			if (this.rollingrad0 > this.rollingrad1)
			{
				Gizmos.DrawLine(position, position + this.acummulatedRot * (right * this.rollingrad0 * 2f));
			}
			else
			{
				Gizmos.DrawLine(position, position + this.acummulatedRot * (forward * this.rollingrad1 * 2f));
			}
		}
		else if (this.shapeType == 3)
		{
			Vector3 vector = this.linePoint1 - this.linePoint0;
			Gizmos.DrawLine(position, position + this.acummulatedRot * (forward * vector.z + right * vector.x + up * vector.y));
		}
		else if (this.shapeType == 1 || this.shapeType == 2)
		{
			Gizmos.DrawWireSphere(position, this.rollingrad1);
		}
	}
}
