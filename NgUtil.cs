using System;
using UnityEngine;

public class NgUtil
{
	public static void LogDevelop(object msg)
	{
	}

	public static void LogMessage(object msg)
	{
		Debug.Log(msg);
	}

	public static void LogError(object msg)
	{
		Debug.Log(msg);
	}

	public static float GetArcRadian(float fHeight, float fWidth)
	{
		float num = fWidth / 2f;
		float arcRadius = NgUtil.GetArcRadius(fHeight, fWidth);
		float num2 = Mathf.Sin(num / arcRadius);
		return num2 * 2f;
	}

	public static float GetArcRadius(float fHeight, float fWidth)
	{
		float num = fWidth / 2f;
		return (fHeight * fHeight + num * num) / (2f * fHeight);
	}

	public static float GetArcLength(float fHeight, float fWidth)
	{
		float num = fWidth / 2f;
		float arcRadius = NgUtil.GetArcRadius(fHeight, fWidth);
		float num2 = num / arcRadius;
		return arcRadius * (2f * num2);
	}

	public static int NextPowerOf2(int val)
	{
		int i;
		for (i = Mathf.ClosestPowerOfTwo(val); i < val; i <<= 1)
		{
		}
		return i;
	}

	public static void ClearStrings(string[] strings)
	{
		if (strings == null)
		{
			return;
		}
		for (int i = 0; i < strings.Length; i++)
		{
			strings[i] = string.Empty;
		}
	}

	public static void ClearBools(bool[] bools)
	{
		if (bools == null)
		{
			return;
		}
		for (int i = 0; i < bools.Length; i++)
		{
			bools[i] = false;
		}
	}

	public static void ClearObjects(UnityEngine.Object[] objects)
	{
		if (objects == null)
		{
			return;
		}
		for (int i = 0; i < objects.Length; i++)
		{
			objects[i] = null;
		}
	}
}
