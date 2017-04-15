using System;
using UnityEngine;

[Serializable]
public class Boundary
{
	public Vector2 min;

	public Vector2 max;

	public Boundary()
	{
		this.min = Vector2.zero;
		this.max = Vector2.zero;
	}
}
