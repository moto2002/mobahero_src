using System;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
	public float Magnitude = 1f;

	public float Duration = 1f;

	public static ShakeCamera Shake(float magnitude, float duration)
	{
		OrbitGameObject orbitGameObject = (OrbitGameObject)UnityEngine.Object.FindObjectOfType(typeof(OrbitGameObject));
		ShakeCamera shakeCamera = orbitGameObject.gameObject.AddComponent<ShakeCamera>();
		shakeCamera.Magnitude = magnitude;
		shakeCamera.Duration = duration;
		return shakeCamera;
	}

	private void Update()
	{
		this.Duration -= Time.deltaTime;
		if (this.Duration < 0f)
		{
			UnityEngine.Object.Destroy(this);
		}
		OrbitGameObject component = base.gameObject.GetComponent<OrbitGameObject>();
		if (component)
		{
			component.ShakeVal.y = Mathf.Sin(1000f * Time.time) * this.Duration * this.Magnitude;
		}
	}
}
