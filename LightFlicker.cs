using System;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
	private void Update()
	{
		float num = Mathf.Sin(base.transform.position.x + 15.58213f * Time.time) + Mathf.Sin(base.transform.position.y + 6.4624f * Time.time);
		num *= 0.25f;
		num += 0.5f;
		base.light.intensity = 1.5f + 0.35f * num;
	}
}
