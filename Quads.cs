using System;
using UnityEngine;

[Serializable]
public class Quads : MonoBehaviour
{
	[NonSerialized]
	public static Mesh[] meshes;

	[NonSerialized]
	public static int currentQuads;

	public static bool HasMeshes()
	{
		int arg_44_0;
		if (Quads.meshes == null)
		{
			arg_44_0 = 0;
		}
		else
		{
			int i = 0;
			Mesh[] array = Quads.meshes;
			int length = array.Length;
			while (i < length)
			{
				if (null == array[i])
				{
					arg_44_0 = 0;
					return arg_44_0 != 0;
				}
				i++;
			}
			arg_44_0 = 1;
		}
		return arg_44_0 != 0;
	}

	public static void Cleanup()
	{
		if (Quads.meshes != null)
		{
			int i = 0;
			Mesh[] array = Quads.meshes;
			int length = array.Length;
			while (i < length)
			{
				if (null != array[i])
				{
					UnityEngine.Object.DestroyImmediate(array[i]);
					array[i] = null;
				}
				i++;
			}
			Quads.meshes = null;
		}
	}

	public static Mesh[] GetMeshes(int totalWidth, int totalHeight)
	{
		Mesh[] arg_99_0;
		if (Quads.HasMeshes() && Quads.currentQuads == totalWidth * totalHeight)
		{
			arg_99_0 = Quads.meshes;
		}
		else
		{
			int num = 10833;
			int num2 = totalWidth * totalHeight;
			Quads.currentQuads = num2;
			int num3 = Mathf.CeilToInt(1f * (float)num2 / (1f * (float)num));
			Quads.meshes = new Mesh[num3];
			int num4 = 0;
			for (int i = 0; i < num2; i += num)
			{
				int triCount = Mathf.FloorToInt((float)Mathf.Clamp(num2 - i, 0, num));
				Quads.meshes[num4] = Quads.GetMesh(triCount, i, totalWidth, totalHeight);
				num4++;
			}
			arg_99_0 = Quads.meshes;
		}
		return arg_99_0;
	}

	public static Mesh GetMesh(int triCount, int triOffset, int totalWidth, int totalHeight)
	{
		Mesh mesh = new Mesh();
		mesh.hideFlags = HideFlags.DontSave;
		Vector3[] array = new Vector3[triCount * 4];
		Vector2[] array2 = new Vector2[triCount * 4];
		Vector2[] array3 = new Vector2[triCount * 4];
		int[] array4 = new int[triCount * 6];
		for (int i = 0; i < triCount; i++)
		{
			int num = i * 4;
			int num2 = i * 6;
			int num3 = triOffset + i;
			float num4 = Mathf.Floor((float)(num3 % totalWidth)) / (float)totalWidth;
			float num5 = Mathf.Floor((float)(num3 / totalWidth)) / (float)totalHeight;
			Vector3 vector = new Vector3(num4 * (float)2 - (float)1, num5 * (float)2 - (float)1, 1f);
			array[num + 0] = vector;
			array[num + 1] = vector;
			array[num + 2] = vector;
			array[num + 3] = vector;
			array2[num + 0] = new Vector2((float)0, (float)0);
			array2[num + 1] = new Vector2(1f, (float)0);
			array2[num + 2] = new Vector2((float)0, 1f);
			array2[num + 3] = new Vector2(1f, 1f);
			array3[num + 0] = new Vector2(num4, num5);
			array3[num + 1] = new Vector2(num4, num5);
			array3[num + 2] = new Vector2(num4, num5);
			array3[num + 3] = new Vector2(num4, num5);
			array4[num2 + 0] = num + 0;
			array4[num2 + 1] = num + 1;
			array4[num2 + 2] = num + 2;
			array4[num2 + 3] = num + 1;
			array4[num2 + 4] = num + 2;
			array4[num2 + 5] = num + 3;
		}
		mesh.vertices = array;
		mesh.triangles = array4;
		mesh.uv = array2;
		mesh.uv2 = array3;
		return mesh;
	}

	public override void Main()
	{
	}
}
