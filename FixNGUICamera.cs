using System;
using UnityEngine;

public class FixNGUICamera : MonoBehaviour
{
	private void Start()
	{
		float num = 960f / (float)Screen.width;
		float num2 = 540f / (float)Screen.height;
		if (num > num2)
		{
			base.camera.orthographicSize = num2;
		}
		else
		{
			base.camera.orthographicSize = num;
		}
	}

	private void Update()
	{
	}
}
