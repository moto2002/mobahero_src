using System;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
	private Camera _unityCamera;

	private Camera UnityCamera
	{
		get
		{
			if (this._unityCamera == null)
			{
				this._unityCamera = base.GetComponent<Camera>();
				if (this._unityCamera == null)
				{
					Debug.LogError("A unity camera must be attached to the GameCamera script");
				}
			}
			return this._unityCamera;
		}
	}

	public Camera ScreenCamera
	{
		get
		{
			return this.UnityCamera;
		}
	}

	private void Awake()
	{
	}

	private Vector2 GetScreenPixelDimensions(GameCamera settings)
	{
		Vector2 result = new Vector2(this.ScreenCamera.pixelWidth, this.ScreenCamera.pixelHeight);
		return result;
	}
}
