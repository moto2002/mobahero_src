using System;
using UnityEngine;

[Serializable]
public struct SphericalVector
{
	public float Length;

	public float Zenith;

	public float Azimuth;

	public Vector3 Position
	{
		get
		{
			return this.Length * this.Direction;
		}
	}

	public Vector3 Direction
	{
		get
		{
			float f = this.Zenith * 3.14159274f / 2f;
			Vector3 result;
			result.y = Mathf.Sin(f);
			float num = Mathf.Cos(f);
			float f2 = this.Azimuth * 3.14159274f;
			result.x = num * Mathf.Sin(f2);
			result.z = num * Mathf.Cos(f2);
			return result;
		}
	}

	public SphericalVector(float azimuth, float zenith, float length)
	{
		this.Length = length;
		this.Zenith = zenith;
		this.Azimuth = azimuth;
	}

	public override string ToString()
	{
		return string.Format("Azimuth {0:0.0000} : Zenith {1:0.0000} : Length {2:0.0000}]", this.Azimuth, this.Zenith, this.Length);
	}
}
