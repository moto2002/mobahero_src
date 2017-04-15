using System;
using UnityEngine;

public class SimpleConfiner : IConfineCamera
{
	private Vector2 rangeX;

	private Vector2 rangeZ;

	private Vector2 rangeX1;

	private Vector2 rangeZ1;

	private Vector2 rangeX2;

	private Vector2 rangeZ2;

	public void ConfineCamera(Transform cam)
	{
		if (cam)
		{
			if (BattleCameraMgr.Instance._currenCameraControllerType != CameraControllerType.Free)
			{
				cam.localPosition = Vector3.zero;
			}
			Vector3 position = cam.position;
			if (position.x < this.rangeX.x)
			{
				position.x = this.rangeX.x;
			}
			if (position.x > this.rangeX.y)
			{
				position.x = this.rangeX.y;
			}
			if (position.z < this.rangeZ.x)
			{
				position.z = this.rangeZ.x;
			}
			if (position.z > this.rangeZ.y)
			{
				position.z = this.rangeZ.y;
			}
			cam.position = position;
		}
	}

	public void ChangeRange(int nIndex)
	{
		if (nIndex == 1)
		{
			this.rangeZ = this.rangeZ1;
		}
		if (nIndex == 2)
		{
			this.rangeZ = this.rangeZ2;
		}
	}

	public static IConfineCamera Create(Component c)
	{
		if (!c)
		{
			return null;
		}
		if (c is CameraParams)
		{
			CameraParams cameraParams = c as CameraParams;
			SimpleConfiner simpleConfiner = new SimpleConfiner();
			simpleConfiner.rangeX = cameraParams.rangeX;
			simpleConfiner.rangeZ = cameraParams.rangeZ;
			simpleConfiner.rangeX1 = cameraParams.rangeX;
			simpleConfiner.rangeZ1 = cameraParams.rangeZ;
			simpleConfiner.rangeX2 = cameraParams.rangeX;
			simpleConfiner.rangeZ2.x = cameraParams.rangeZ.x - 5f;
			simpleConfiner.rangeZ2.y = cameraParams.rangeZ.y - 5f;
			return simpleConfiner;
		}
		return null;
	}
}
