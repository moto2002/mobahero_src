using System;
using UnityEngine;

public class LookCamera : MonoBehaviour
{
	private Transform cacheTransform;

	private Transform cacheMainCameraTransform;

	private void Awake()
	{
		this.cacheTransform = base.transform;
		if (Camera.main != null)
		{
			this.cacheMainCameraTransform = Camera.main.transform;
		}
	}

	private void Update()
	{
		if (this.cacheMainCameraTransform == null && Camera.main != null)
		{
			this.cacheMainCameraTransform = Camera.main.transform;
		}
		if (this.cacheMainCameraTransform == null)
		{
			return;
		}
		this.cacheTransform.LookAt(this.cacheMainCameraTransform);
	}
}
