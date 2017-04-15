using System;
using UnityEngine;

public class Hover : MonoBehaviour
{
	public float hoverSpeedX = 0.8f;

	public float hoverSpeedY = 0.6f;

	public float hoverSpeedZ = 1f;

	public float hoverDistance = 1f;

	private void Update()
	{
		float num = Mathf.Sin(Time.time * this.hoverSpeedX) * Time.deltaTime * this.hoverDistance;
		float num2 = Mathf.Sin(Time.time * this.hoverSpeedY) * Time.deltaTime * this.hoverDistance;
		float num3 = Mathf.Sin(Time.time * this.hoverSpeedZ) * Time.deltaTime * this.hoverDistance;
		base.transform.position = new Vector3(num + base.transform.position.x, num2 + base.transform.position.y, num3 + base.transform.position.z);
	}

	private void OnBecameInvisible()
	{
		base.enabled = false;
	}

	private void OnBecameVisible()
	{
		base.enabled = true;
	}
}
