using System;
using UnityEngine;

public class OrbitGameObject : Orbit
{
	public GameObject Target;

	public GameObject CameraObject;

	public Vector3 TargetOffset = Vector3.zero;

	public float CameraLength;

	public Vector3 ShakeVal = Vector3.zero;

	private float rotationXtemp = 0f;

	private float rotationYtemp = 0f;

	private Vector2 controllerPositionTemp;

	private Vector2 controllerPositionNext;

	private void Start()
	{
		this.Data.Zenith = -0.4f;
		this.Data.Length = this.CameraLength;
		if (this.CameraObject)
		{
		}
	}

	public void HoldAim()
	{
		if (this.CameraObject)
		{
		}
	}

	protected override void Update()
	{
	}

	private void mobileController()
	{
		for (int i = 0; i < Input.touchCount; i++)
		{
			if (Input.GetTouch(i).position.x < (float)(Screen.width / 2))
			{
				if (Input.GetTouch(i).phase == TouchPhase.Began || Input.GetTouch(i).phase == TouchPhase.Stationary)
				{
				}
			}
			else if (Input.GetTouch(i).phase == TouchPhase.Began)
			{
				this.controllerPositionNext = new Vector2(Input.GetTouch(i).position.x, (float)Screen.height - Input.GetTouch(i).position.y);
				this.controllerPositionTemp = this.controllerPositionNext;
			}
			else
			{
				this.controllerPositionNext = new Vector2(Input.GetTouch(i).position.x, (float)Screen.height - Input.GetTouch(i).position.y);
				Vector2 vector = this.controllerPositionNext - this.controllerPositionTemp;
				this.Data.Azimuth = this.rotationXtemp + vector.x * 0.01f * Time.deltaTime;
				this.Data.Zenith = this.rotationYtemp + -vector.y * 0.01f * Time.deltaTime;
				this.controllerPositionTemp = Vector2.Lerp(this.controllerPositionTemp, this.controllerPositionNext, 0.5f);
			}
		}
	}
}
