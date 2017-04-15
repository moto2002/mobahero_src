using System;
using UnityEngine;

public class WaveController : MonoBehaviour
{
	public float rotationY;

	public float scaleY = 0.2f;

	public float scaleX = 1f;

	public AnimationCurve positionY;

	private void Update()
	{
		Vector3 localPosition = base.transform.localPosition;
		float num = this.scaleX * Time.time;
		float time = num - (float)((int)num);
		localPosition.z = this.positionY.Evaluate(time) * this.scaleY;
		base.transform.localPosition = localPosition;
		base.transform.Rotate(Vector3.forward, this.rotationY * Time.deltaTime, Space.Self);
	}
}
