using System;
using System.Collections.Generic;
using UnityEngine;

public class meshDrawer : MonoBehaviour
{
	public Material mMat;

	private static Vector3[] verts;

	private static Vector2[] uvs;

	private static Color[] clrs;

	private static int[] tris;

	private static Mesh mMesh;

	private static int cidx;

	private static int maxnum;

	public static meshDrawer inst;

	private static List<int> drawerlist;

	public static bool enb;

	public static bool gapdt = true;

	public static void setEnable(bool eb)
	{
		meshDrawer.enb = eb;
		if (meshDrawer.inst != null && meshDrawer.inst.gameObject != null)
		{
			meshDrawer.inst.gameObject.GetComponent<Renderer>().enabled = eb;
		}
	}

	public static meshDrawer Instance()
	{
		return meshDrawer.inst;
	}

	private void Awake()
	{
		if (!meshDrawer.enb)
		{
			return;
		}
		meshDrawer.inst = this;
		meshDrawer.mMesh = new Mesh();
		base.GetComponent<MeshFilter>().mesh = meshDrawer.mMesh;
		base.GetComponent<MeshRenderer>().material = this.mMat;
		meshDrawer.CreateQuads(30);
	}

	public static void CreateQuads(int num)
	{
		if (!meshDrawer.enb)
		{
			return;
		}
		meshDrawer.maxnum = num;
		meshDrawer.verts = new Vector3[num * 4];
		meshDrawer.uvs = new Vector2[num * 4];
		meshDrawer.clrs = new Color[num * 4];
		meshDrawer.tris = new int[num * 6];
		meshDrawer.drawerlist = new List<int>();
		meshDrawer.mMesh.vertices = meshDrawer.verts;
		meshDrawer.mMesh.colors = meshDrawer.clrs;
		meshDrawer.mMesh.uv = meshDrawer.uvs;
		meshDrawer.mMesh.triangles = meshDrawer.tris;
		meshDrawer.mMesh.RecalculateNormals();
		meshDrawer.mMesh.RecalculateBounds();
	}

	public static void Release()
	{
		meshDrawer.verts = null;
		meshDrawer.uvs = null;
		meshDrawer.clrs = null;
		meshDrawer.tris = null;
		meshDrawer.drawerlist.Clear();
		meshDrawer.drawerlist = null;
	}

	public static void drawQuad(int drawer, Vector3 pos, Vector2 size)
	{
		if (meshDrawer.mMesh == null || !meshDrawer.enb)
		{
			return;
		}
		meshDrawer.gapdt = false;
		if (meshDrawer.drawerlist.Contains(drawer))
		{
			for (int i = meshDrawer.cidx; i < meshDrawer.maxnum * 4; i++)
			{
				meshDrawer.verts[i] = Vector3.left * 9999f;
			}
			meshDrawer.mMesh.vertices = meshDrawer.verts;
			meshDrawer.mMesh.colors = meshDrawer.clrs;
			meshDrawer.mMesh.uv = meshDrawer.uvs;
			meshDrawer.mMesh.triangles = meshDrawer.tris;
			meshDrawer.drawerlist.Clear();
			meshDrawer.cidx = 0;
			meshDrawer.gapdt = true;
		}
		if (meshDrawer.cidx + 4 > meshDrawer.maxnum * 4)
		{
			return;
		}
		meshDrawer.drawerlist.Add(drawer);
		Vector3 vector = pos + Vector3.right * -size.x + Vector3.forward * -size.y;
		Vector3 vector2 = pos + Vector3.right * size.x + Vector3.forward * -size.y;
		Vector3 vector3 = pos + Vector3.right * size.x + Vector3.forward * size.y;
		Vector3 vector4 = pos + Vector3.right * -size.x + Vector3.forward * size.y;
		meshDrawer.verts[meshDrawer.cidx] = vector;
		meshDrawer.verts[meshDrawer.cidx + 1] = vector2;
		meshDrawer.verts[meshDrawer.cidx + 2] = vector3;
		meshDrawer.verts[meshDrawer.cidx + 3] = vector4;
		meshDrawer.uvs[meshDrawer.cidx] = new Vector2(0f, 0f);
		meshDrawer.uvs[meshDrawer.cidx + 1] = new Vector2(1f, 0f);
		meshDrawer.uvs[meshDrawer.cidx + 2] = new Vector2(1f, 1f);
		meshDrawer.uvs[meshDrawer.cidx + 3] = new Vector2(0f, 1f);
		meshDrawer.clrs[meshDrawer.cidx] = Color.white;
		meshDrawer.clrs[meshDrawer.cidx + 1] = Color.white;
		meshDrawer.clrs[meshDrawer.cidx + 2] = Color.white;
		meshDrawer.clrs[meshDrawer.cidx + 3] = Color.white;
		int num = meshDrawer.cidx / 4 * 6;
		meshDrawer.tris[num] = meshDrawer.cidx;
		meshDrawer.tris[num + 1] = meshDrawer.cidx + 3;
		meshDrawer.tris[num + 2] = meshDrawer.cidx + 2;
		meshDrawer.tris[num + 3] = meshDrawer.cidx;
		meshDrawer.tris[num + 4] = meshDrawer.cidx + 2;
		meshDrawer.tris[num + 5] = meshDrawer.cidx + 1;
		meshDrawer.cidx += 4;
	}
}
