using System;
using UnityEngine;

public class SelfRotate : MonoBehaviour
{
	public float xRotate;

	public float yRotate;

	public float zRotate;

	private void Update()
	{
		base.transform.Rotate(this.xRotate * Time.deltaTime, this.yRotate * Time.deltaTime, this.zRotate * Time.deltaTime);
	}
}
