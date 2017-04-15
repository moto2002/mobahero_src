using System;
using UnityEngine;

public class OrbitLookAt : Orbit
{
	public Vector3 LookAt = Vector3.zero;

	private void Start()
	{
		this.Data.Zenith = -0.3f;
		this.Data.Length = -6f;
	}

	protected override void Update()
	{
		base.Update();
		base.gameObject.transform.position += this.LookAt;
		base.gameObject.transform.LookAt(this.LookAt);
	}
}
