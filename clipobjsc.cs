using System;
using UnityEngine;

public class clipobjsc : MonoBehaviour
{
	public float _width = 200f;

	public float _height = 200f;

	public Vector2 segs = new Vector3(1f, 1f);

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDrawGizmos()
	{
		Vector3 vector = base.transform.right * this._width * 0.5f + base.transform.forward * this._height * 0.5f;
		Vector3 vector2 = base.transform.right * -this._width * 0.5f + base.transform.forward * this._height * 0.5f;
		Vector3 vector3 = base.transform.right * -this._width * 0.5f + base.transform.forward * -this._height * 0.5f;
		Vector3 vector4 = base.transform.right * this._width * 0.5f + base.transform.forward * -this._height * 0.5f;
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, vector3);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.DrawLine(vector4, vector);
	}
}
