using System;
using UnityEngine;

public class FixNGUICamera1 : MonoBehaviour
{
	private void Start()
	{
		float num = 1280f / (float)Screen.width;
		float num2 = 720f / (float)Screen.height;
		if (num > num2)
		{
			base.camera.orthographicSize = num;
		}
		else
		{
			base.camera.orthographicSize = num2;
		}
	}

	private void Update()
	{
	}
}
