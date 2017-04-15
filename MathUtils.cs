using System;
using UnityEngine;

public class MathUtils
{
	public static Vector3 RadomCirclePoint(Vector3 center, float radius)
	{
		Quaternion rotation = Quaternion.Euler(0f, (float)(UnityEngine.Random.Range(0, 36) * 10), 0f);
		return center + rotation * new Vector3(0f, 0f, radius);
	}

	public static Vector3 RadomInsideCirclePoint(Vector3 center, float radius)
	{
		Vector2 vector = UnityEngine.Random.insideUnitCircle * radius;
		return center + new Vector3(vector.x, 0f, vector.y);
	}

	public static Vector3 RadomOnSpherePoint(Vector3 center, float radius)
	{
		Vector3 b = UnityEngine.Random.onUnitSphere * radius;
		return center + b;
	}

	public static bool Rand(float prop)
	{
		return UnityEngine.Random.Range(0f, 100f) >= 100f - prop;
	}

	public static float DistanceIgnoreY(Vector3 pos1, Vector3 pos2)
	{
		pos1.y = (pos2.y = 0f);
		return Vector3.Distance(pos1, pos2);
	}
}
