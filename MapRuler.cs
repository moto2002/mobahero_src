using System;
using UnityEngine;

public class MapRuler : MonoBehaviour
{
	public static MapRuler map;

	public float screen2Map = 1f;

	public Vector3 left = new Vector3(-23f, 0f, 0f);

	public Vector3 right = new Vector3(23f, 0f, 0f);

	public Vector3 bottom = new Vector3(0f, 0f, -8f);

	public Vector3 top = new Vector3(0f, 0f, -3f);

	public float[,] footMask;

	public int width = 144;

	public int height = 20;

	private float lx;

	private float rx;

	private float by;

	private float ty;

	private float mapW;

	private float mapH;

	private void Start()
	{
		MapRuler.map = this;
		this.lx = this.left.x;
		this.rx = this.right.x;
		this.by = this.bottom.z;
		this.ty = this.top.z;
		this.mapW = Mathf.Abs(this.rx - this.lx);
		this.mapH = Mathf.Abs(this.ty - this.by);
		this.footMask = new float[this.width, this.height];
		for (int i = 0; i < this.width; i++)
		{
			for (int j = 0; j < this.height; j++)
			{
				this.footMask[i, j] = 1f;
			}
		}
	}

	public void GetCamFootXY(Vector3 pos, out int x, out int y)
	{
		x = 0;
		y = 0;
		float num = pos.x - this.lx;
		float num2 = pos.z - this.by;
		x = (int)(num / this.mapW * (float)this.width);
		y = (int)(num2 / this.mapH * (float)this.height);
	}

	private void GetCamXYRatio(Vector3 pos, out float x, out float y)
	{
		x = 0f;
		y = 0f;
		float num = pos.x - this.lx;
		float num2 = pos.z - this.by;
		x = num / this.mapW;
		y = num2 / this.mapH;
	}

	public void FlushPixel(Vector3 camPos, Texture2D scr)
	{
		int num = 0;
		int num2 = 0;
		this.GetCamFootXY(camPos, out num, out num2);
		int num3 = scr.width;
		int num4 = scr.height;
		for (int i = 0; i < num3; i++)
		{
			for (int j = 0; j < num4; j++)
			{
				float a = scr.GetPixel(i, j).a;
				int num5 = Mathf.Max(0, num + (int)(this.screen2Map * (float)(i - num3 / 2)));
				num5 = Mathf.Min(num5, this.width - 1);
				int num6 = Mathf.Max(0, num2 + (int)(this.screen2Map * (float)(j - num4 / 2)));
				num6 = Mathf.Min(num6, this.height - 1);
				this.SetPoint(num5, num6, a);
			}
		}
	}

	private void SetPoint(int x, int y, float val)
	{
		if (this.footMask[x, y] > val)
		{
			this.footMask[x, y] = val;
		}
	}

	public float ReadMask(int x, int y)
	{
		return this.footMask[x, y];
	}
}
