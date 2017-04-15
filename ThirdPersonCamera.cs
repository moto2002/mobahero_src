using System;
using UnityEngine;

[Serializable]
public class ThirdPersonCamera : MonoBehaviour
{
	public Transform cameraTransform;

	private Transform _target;

	public float distance;

	public float height;

	public float angularSmoothLag;

	public float angularMaxSpeed;

	public float heightSmoothLag;

	public float snapSmoothLag;

	public float snapMaxSpeed;

	public float clampHeadPositionScreenSpace;

	public float lockCameraTimeout;

	private Vector3 headOffset;

	private Vector3 centerOffset;

	private float heightVelocity;

	private float angleVelocity;

	private bool snap;

	private ThirdPersonController controller;

	private float targetHeight;

	public ThirdPersonCamera()
	{
		this.distance = 7f;
		this.height = 3f;
		this.angularSmoothLag = 0.3f;
		this.angularMaxSpeed = 15f;
		this.heightSmoothLag = 0.3f;
		this.snapSmoothLag = 0.2f;
		this.snapMaxSpeed = 720f;
		this.clampHeadPositionScreenSpace = 0.75f;
		this.lockCameraTimeout = 0.2f;
		this.headOffset = Vector3.zero;
		this.centerOffset = Vector3.zero;
		this.targetHeight = 100000f;
	}

	public override void Awake()
	{
		if (!this.cameraTransform && Camera.main)
		{
			this.cameraTransform = Camera.main.transform;
		}
		if (!this.cameraTransform)
		{
			Debug.Log("Please assign a camera to the ThirdPersonCamera script.");
			this.enabled = false;
		}
		this._target = this.transform;
		if (this._target)
		{
			this.controller = (ThirdPersonController)this._target.GetComponent(typeof(ThirdPersonController));
		}
		if (this.controller)
		{
			CharacterController characterController = (CharacterController)this._target.collider;
			this.centerOffset = characterController.bounds.center - this._target.position;
			this.headOffset = this.centerOffset;
			this.headOffset.y = characterController.bounds.max.y - this._target.position.y;
		}
		else
		{
			Debug.Log("Please assign a target to the camera that has a ThirdPersonController script attached.");
		}
		this.Cut(this._target, this.centerOffset);
	}

	public override void DebugDrawStuff()
	{
		Debug.DrawLine(this._target.position, this._target.position + this.headOffset);
	}

	public override float AngleDistance(float a, float b)
	{
		a = Mathf.Repeat(a, (float)360);
		b = Mathf.Repeat(b, (float)360);
		return Mathf.Abs(b - a);
	}

	public override void Apply(Transform dummyTarget, Vector3 dummyCenter)
	{
		if (this.controller)
		{
			Vector3 vector = this._target.position + this.centerOffset;
			Vector3 headPos = this._target.position + this.headOffset;
			float y = this._target.eulerAngles.y;
			float num = this.cameraTransform.eulerAngles.y;
			float num2 = y;
			if (Input.GetButton("Fire2"))
			{
				this.snap = true;
			}
			if (this.snap)
			{
				if (this.AngleDistance(num, y) < 3f)
				{
					this.snap = false;
				}
				num = Mathf.SmoothDampAngle(num, num2, ref this.angleVelocity, this.snapSmoothLag, this.snapMaxSpeed);
			}
			else
			{
				if (this.controller.GetLockCameraTimer() < this.lockCameraTimeout)
				{
					num2 = num;
				}
				if (this.AngleDistance(num, num2) > (float)160 && this.controller.IsMovingBackwards())
				{
					num2 += (float)180;
				}
				num = Mathf.SmoothDampAngle(num, num2, ref this.angleVelocity, this.angularSmoothLag, this.angularMaxSpeed);
			}
			if (this.controller.IsJumping())
			{
				float num3 = vector.y + this.height;
				if (num3 < this.targetHeight || num3 - this.targetHeight > (float)5)
				{
					this.targetHeight = vector.y + this.height;
				}
			}
			else
			{
				this.targetHeight = vector.y + this.height;
			}
			float num4 = this.cameraTransform.position.y;
			num4 = Mathf.SmoothDamp(num4, this.targetHeight, ref this.heightVelocity, this.heightSmoothLag);
			Quaternion rotation = Quaternion.Euler((float)0, num, (float)0);
			this.cameraTransform.position = vector;
			this.cameraTransform.position = this.cameraTransform.position + rotation * Vector3.back * this.distance;
			float y2 = num4;
			Vector3 position = this.cameraTransform.position;
			float num5 = position.y = y2;
			Vector3 vector2 = this.cameraTransform.position = position;
			this.SetUpRotation(vector, headPos);
		}
	}

	public override void LateUpdate()
	{
		this.Apply(this.transform, Vector3.zero);
	}

	public override void Cut(Transform dummyTarget, Vector3 dummyCenter)
	{
		float num = this.heightSmoothLag;
		float num2 = this.snapMaxSpeed;
		float num3 = this.snapSmoothLag;
		this.snapMaxSpeed = (float)10000;
		this.snapSmoothLag = 0.001f;
		this.heightSmoothLag = 0.001f;
		this.snap = true;
		this.Apply(this.transform, Vector3.zero);
		this.heightSmoothLag = num;
		this.snapMaxSpeed = num2;
		this.snapSmoothLag = num3;
	}

	public override void SetUpRotation(Vector3 centerPos, Vector3 headPos)
	{
		Vector3 position = this.cameraTransform.position;
		Vector3 vector = centerPos - position;
		Quaternion lhs = Quaternion.LookRotation(new Vector3(vector.x, (float)0, vector.z));
		Vector3 forward = Vector3.forward * this.distance + Vector3.down * this.height;
		this.cameraTransform.rotation = lhs * Quaternion.LookRotation(forward);
		Ray ray = this.cameraTransform.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, (float)1));
		Ray ray2 = this.cameraTransform.camera.ViewportPointToRay(new Vector3(0.5f, this.clampHeadPositionScreenSpace, (float)1));
		Vector3 point = ray.GetPoint(this.distance);
		Vector3 point2 = ray2.GetPoint(this.distance);
		float num = Vector3.Angle(ray.direction, ray2.direction);
		float num2 = num / (point.y - point2.y);
		float num3 = num2 * (point.y - centerPos.y);
		if (num3 < num)
		{
			num3 = (float)0;
		}
		else
		{
			num3 -= num;
			this.cameraTransform.rotation = this.cameraTransform.rotation * Quaternion.Euler(-num3, (float)0, (float)0);
		}
	}

	public override Vector3 GetCenterOffset()
	{
		return this.centerOffset;
	}

	public override void Main()
	{
	}
}
