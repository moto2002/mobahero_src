using System;
using UnityEngine;

public class move : MonoBehaviour
{
	private Vector3 up = new Vector3(0f, 1f, 0f);

	private Vector3 down = new Vector3(0f, -1f, 0f);

	private float lastPos;

	private void Start()
	{
		this.lastPos = base.transform.position.y;
	}

	private void Update()
	{
		float y = base.transform.position.y;
		if ((y - this.lastPos >= 0f && y < 2f) || y < -2f)
		{
			base.transform.Translate(this.up * Time.deltaTime);
		}
		else
		{
			base.transform.Translate(this.down * Time.deltaTime);
		}
		this.lastPos = y;
	}
}
