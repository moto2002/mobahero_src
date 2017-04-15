using System;
using UnityEngine;

public struct TexSight
{
	public float fade;

	public static float maxDist = 1000f;

	public float texX;

	public float texY;

	public float sight;

	public TexSight(float x, float y, float sight, float fade)
	{
		this.texX = x;
		this.texY = y;
		this.sight = sight;
		this.fade = fade;
	}

	public static float GetNear(int texIndexX, int texIndexY, TexSight[] points)
	{
		float num = TexSight.maxDist;
		float result = 0f;
		for (int i = 0; i < points.Length; i++)
		{
			float num2 = Mathf.Abs(points[i].texX - (float)texIndexX);
			float num3 = Mathf.Abs(points[i].texY - (float)texIndexY);
			float num4 = Mathf.Sqrt(num2 * num2 + num3 * num3);
			if (num4 <= points[i].sight + points[i].fade && num4 < num)
			{
				num = num4;
				result = 1f - Mathf.Max(0f, num4 - points[i].sight) / points[i].fade;
			}
		}
		return result;
	}

	public static float GetMistAmt(int indexX, int indexY, TexSight[] points)
	{
		return TexSight.GetNear(indexX, indexY, points);
	}

	public static TexSight ConvertUnitSight(UnitSight u, Camera cam)
	{
		TexSight result = default(TexSight);
		Vector3 vector = cam.WorldToScreenPoint(u.worldPos);
		result.texX = vector.x / cam.pixelWidth * (float)PostMist.texW;
		result.texY = vector.y / cam.pixelHeight * (float)PostMist.texH;
		result.sight = u.sight;
		result.fade = u.fade;
		return result;
	}
}
