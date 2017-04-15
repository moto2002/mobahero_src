using System;
using UnityEngine;

[ExecuteInEditMode]
public class MyAnchorCamera : MonoBehaviour
{
	public enum AnchorModel
	{
		Auto,
		Tall,
		Width
	}

	public MyAnchorCamera.AnchorModel Model;

	public float resolutionScale = 1f;

	public float scale = 1f;

	public Camera cam;

	public float suitableUI_width;

	public float suitableUI_height;

	public float gameViewPixelWidth;

	public float gameViewPixelHeight;

	public float gameViewAspect;

	public bool ShowResolution;

	public bool isNGUIHierarchy;

	private UIRoot ui_root;

	private Matrix4x4 m = default(Matrix4x4);

	private void Awake()
	{
		this.cam = base.GetComponent<Camera>();
		base.transform.parent.localScale = 0.005f * Vector3.one;
	}

	private void Start()
	{
		this.gameViewPixelWidth = (float)Screen.width;
		this.gameViewPixelHeight = (float)Screen.height;
		this.gameViewAspect = this.gameViewPixelWidth / this.gameViewPixelHeight;
		if (this.gameViewPixelWidth < 1920f)
		{
			this.gameViewPixelHeight = 1920f / this.gameViewAspect;
			this.gameViewPixelWidth = 1920f;
		}
		if (this.cam != null)
		{
			this.UpdateCameraMatrix();
		}
	}

	private void SelectMode()
	{
		switch (this.Model)
		{
		case MyAnchorCamera.AnchorModel.Auto:
			if (this.suitableUI_height != 0f && this.suitableUI_width != 0f)
			{
				if (Mathf.Abs((float)Screen.width - this.suitableUI_width) > Mathf.Abs((float)Screen.height - this.suitableUI_height))
				{
					this.scale = (float)Screen.width / this.suitableUI_width;
				}
				else
				{
					this.scale = (float)Screen.height / this.suitableUI_height;
				}
			}
			else
			{
				this.scale = 1f;
			}
			break;
		case MyAnchorCamera.AnchorModel.Tall:
			if (this.suitableUI_height != 0f)
			{
				this.scale = (float)Screen.height / this.suitableUI_height;
			}
			else
			{
				this.scale = 1f;
			}
			break;
		case MyAnchorCamera.AnchorModel.Width:
			if (this.suitableUI_width != 0f)
			{
				this.scale = (float)Screen.width / this.suitableUI_width;
			}
			else
			{
				this.scale = 1f;
			}
			break;
		}
		this.resolutionScale = this.scale;
		if (this.isNGUIHierarchy)
		{
			if (this.ui_root == null)
			{
				this.ui_root = MyAnchorCameraTool.FindInParents<UIRoot>(base.gameObject);
			}
			float x = this.ui_root.transform.localScale.x;
			this.scale *= 1f / x;
		}
	}

	public void UpdateCameraMatrix()
	{
		this.SelectMode();
		float num = 0f;
		float num2 = 0f;
		float pixelWidth = this.cam.pixelWidth;
		float pixelHeight = this.cam.pixelHeight;
		float farClipPlane = this.cam.farClipPlane;
		float nearClipPlane = this.cam.nearClipPlane;
		float value = 2f / (pixelWidth - num) * this.scale;
		float value2 = 2f / (pixelHeight - num2) * this.scale;
		float value3 = -2f / (farClipPlane - nearClipPlane);
		float value4 = 0f;
		float value5 = 0f;
		float value6;
		if (this.isNGUIHierarchy)
		{
			value6 = 0f;
		}
		else
		{
			value6 = -1f;
		}
		this.m[0, 0] = value;
		this.m[0, 1] = 0f;
		this.m[0, 2] = 0f;
		this.m[0, 3] = value4;
		this.m[1, 0] = 0f;
		this.m[1, 1] = value2;
		this.m[1, 2] = 0f;
		this.m[1, 3] = value5;
		this.m[2, 0] = 0f;
		this.m[2, 1] = 0f;
		this.m[2, 2] = value3;
		this.m[2, 3] = value6;
		this.m[3, 0] = 0f;
		this.m[3, 1] = 0f;
		this.m[3, 2] = 0f;
		this.m[3, 3] = 1f;
		this.cam.projectionMatrix = this.m;
	}
}
