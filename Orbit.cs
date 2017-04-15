using System;
using UnityEngine;

public class Orbit : MonoBehaviour
{
	public SphericalVector Data = new SphericalVector(0f, 0f, 1f);

	protected virtual void Update()
	{
		base.gameObject.transform.position = this.Data.Position;
	}
}
