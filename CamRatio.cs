using System;
using UnityEngine;

public class CamRatio
{
	public static void SetupCameras(Camera[] cams, float expectRatio = 0f)
	{
		if (expectRatio == 0f)
		{
			expectRatio = 0.5625f;
		}
		if (cams == null || cams.Length < 1)
		{
			return;
		}
		for (int i = 0; i < cams.Length; i++)
		{
			CamRatio.SetupCamera(cams[i], expectRatio);
		}
	}

	public static void SetupCamera(Camera cam, float expectRatio = 0f)
	{
		if (cam == null)
		{
			return;
		}
		float num = (float)Screen.height / (float)Screen.width;
		if (expectRatio == 0f)
		{
			expectRatio = 0.5625f;
			expectRatio = num;
		}
		if (num > expectRatio)
		{
			float num2 = (num - expectRatio) / num;
			float top = num2 / 2f;
			cam.rect = new Rect(0f, top, 1f, 1f - num2);
		}
		else if (num < expectRatio)
		{
			float num2 = (1f / num - 1f / expectRatio) * num;
			float left = num2 / 2f;
			cam.rect = new Rect(left, 0f, 1f - num2, 1f);
		}
	}

	public static void RestoreCamera(Camera cam)
	{
		if (cam)
		{
			cam.rect = new Rect(0f, 0f, 1f, 1f);
		}
	}
}
