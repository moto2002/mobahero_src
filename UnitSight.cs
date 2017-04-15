using System;
using UnityEngine;

public struct UnitSight
{
	public Vector3 worldPos;

	public float sight;

	public float fade;

	public UnitSight(Vector3 worldPos, float sight, float fade)
	{
		this.worldPos = worldPos;
		this.sight = sight;
		this.fade = fade;
	}
}
