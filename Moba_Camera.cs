using System;
using UnityEngine;

public class Moba_Camera : MonoBehaviour
{
	private const float MAXROTATIONXAXIS = 89f;

	private const float MINROTATIONXAXIS = -89f;

	public bool useFixedUpdate;

	public Moba_Camera_Requirements requirements = new Moba_Camera_Requirements();

	public Moba_Camera_Inputs inputs = new Moba_Camera_Inputs();

	public Moba_Camera_Settings settings = new Moba_Camera_Settings();

	private float _currentZoomAmount;

	private Vector2 _currentCameraRotation = Vector3.zero;

	private bool changeInCamera = true;

	private float deltaMouseDeadZone = 0.2f;

	public float currentZoomAmount
	{
		get
		{
			return this._currentZoomAmount;
		}
		set
		{
			this._currentZoomAmount = value;
			this.changeInCamera = true;
		}
	}

	public Vector3 currentCameraRotation
	{
		get
		{
			return this._currentCameraRotation;
		}
		set
		{
			this._currentCameraRotation = value;
			this.changeInCamera = true;
		}
	}

	private void Start()
	{
		if (!this.requirements.pivot || !this.requirements.offset || !this.requirements.camera)
		{
			string text = string.Empty;
			if (this.requirements.pivot == null)
			{
				text += " / Pivot";
				base.enabled = false;
			}
			if (this.requirements.offset == null)
			{
				text += " / Offset";
				base.enabled = false;
			}
			if (this.requirements.camera == null)
			{
				text += " / Camera";
				base.enabled = false;
			}
			Debug.LogWarning("Moba_Camera Requirements Missing" + text + ". Add missing objects to the requirement tab under the Moba_camera script in the Inspector.");
			Debug.LogWarning("Moba_Camera script requires two empty gameobjects, Pivot and Offset, and a camera.Parent the Offset to the Pivot and the Camera to the Offset. See the Moba_Camera Readme for more information on setup.");
		}
		this._currentZoomAmount = this.settings.zoom.defaultZoom;
		this._currentCameraRotation = this.settings.rotation.defualtRotation;
		if (this.settings.movement.useDefualtHeight && base.enabled)
		{
			Vector3 position = this.requirements.pivot.transform.position;
			position.y = this.settings.movement.defualtHeight;
			this.requirements.pivot.transform.position = position;
		}
	}

	private void Update()
	{
		if (!this.useFixedUpdate)
		{
			this.CameraUpdate();
		}
	}

	private void FixedUpdate()
	{
		if (this.useFixedUpdate)
		{
			this.CameraUpdate();
		}
	}

	private void CameraUpdate()
	{
		this.CalculateCameraZoom();
		this.CalculateCameraRotation();
		this.CalculateCameraMovement();
		this.CalculateCameraUpdates();
		this.CalculateCameraBoundaries();
	}

	private void CalculateCameraZoom()
	{
		float num = 0f;
		int num2 = 1;
		float axis = Input.GetAxis(this.inputs.axis.DeltaScrollWheel);
		if (axis != 0f)
		{
			this.changeInCamera = true;
			if (this.settings.zoom.constZoomRate)
			{
				if ((double)axis != 0.0)
				{
					if ((double)axis > 0.0)
					{
						num = 1f;
					}
					else
					{
						num = -1f;
					}
				}
			}
			else
			{
				num = axis;
			}
		}
		if (!this.settings.zoom.invertZoom)
		{
			num2 = -1;
		}
		this._currentZoomAmount += num * this.settings.zoom.zoomRate * (float)num2 * Time.deltaTime;
	}

	private void CalculateCameraRotation()
	{
		float num = 0f;
		float num2 = 0f;
		Screen.lockCursor = false;
		if ((!this.inputs.useKeyCodeInputs) ? Input.GetButton(this.inputs.axis.button_rotate_camera) : (Input.GetKey(this.inputs.keycodes.RotateCamera) && this.inputs.useKeyCodeInputs))
		{
			Screen.lockCursor = true;
			if (!this.settings.rotation.lockRotationX)
			{
				float axis = Input.GetAxis(this.inputs.axis.DeltaMouseVertical);
				if ((double)axis != 0.0)
				{
					if (this.settings.rotation.constRotationRate)
					{
						if (axis > this.deltaMouseDeadZone)
						{
							num = 1f;
						}
						else if (axis < -this.deltaMouseDeadZone)
						{
							num = -1f;
						}
					}
					else
					{
						num = axis;
					}
					this.changeInCamera = true;
				}
			}
			if (!this.settings.rotation.lockRotationY)
			{
				float axis2 = Input.GetAxis(this.inputs.axis.DeltaMouseHorizontal);
				if (axis2 != 0f)
				{
					if (this.settings.rotation.constRotationRate)
					{
						if (axis2 > this.deltaMouseDeadZone)
						{
							num2 = 1f;
						}
						else if (axis2 < -this.deltaMouseDeadZone)
						{
							num2 = -1f;
						}
					}
					else
					{
						num2 = axis2;
					}
					this.changeInCamera = true;
				}
			}
		}
		this._currentCameraRotation.y = this._currentCameraRotation.y + num2 * this.settings.rotation.cameraRotationRate.y * Time.deltaTime;
		this._currentCameraRotation.x = this._currentCameraRotation.x + num * this.settings.rotation.cameraRotationRate.x * Time.deltaTime;
	}

	private void CalculateCameraMovement()
	{
		if (((!this.inputs.useKeyCodeInputs) ? (Input.GetButtonDown(this.inputs.axis.button_lock_camera) && this.settings.lockTargetTransform != null) : Input.GetKeyDown(this.inputs.keycodes.LockCamera)) && this.settings.lockTargetTransform != null)
		{
			this.settings.cameraLocked = !this.settings.cameraLocked;
		}
		if (this.settings.lockTargetTransform != null && (this.settings.cameraLocked || ((!this.inputs.useKeyCodeInputs) ? Input.GetButton(this.inputs.axis.button_char_focus) : Input.GetKey(this.inputs.keycodes.characterFocus))))
		{
			Vector3 position = this.settings.lockTargetTransform.position;
			if ((this.requirements.pivot.position - position).magnitude > 0.2f)
			{
				if (this.settings.movement.useDefualtHeight && !this.settings.movement.useLockTargetHeight)
				{
					position.y = this.settings.movement.defualtHeight;
				}
				else if (!this.settings.movement.useLockTargetHeight)
				{
					position.y = this.requirements.pivot.position.y;
				}
				this.requirements.pivot.position = Vector3.Lerp(this.requirements.pivot.position, position, this.settings.movement.lockTransitionRate);
			}
		}
		else
		{
			Vector3 a = new Vector3(0f, 0f, 0f);
			if ((Input.mousePosition.x < this.settings.movement.edgeHoverOffset && this.settings.movement.edgeHoverMovement) || ((!this.inputs.useKeyCodeInputs) ? Input.GetButton(this.inputs.axis.button_camera_move_left) : Input.GetKey(this.inputs.keycodes.CameraMoveLeft)))
			{
				a += this.requirements.pivot.transform.right;
			}
			if ((Input.mousePosition.x > (float)Screen.width - this.settings.movement.edgeHoverOffset && this.settings.movement.edgeHoverMovement) || ((!this.inputs.useKeyCodeInputs) ? Input.GetButton(this.inputs.axis.button_camera_move_right) : Input.GetKey(this.inputs.keycodes.CameraMoveRight)))
			{
				a -= this.requirements.pivot.transform.right;
			}
			if ((Input.mousePosition.y < this.settings.movement.edgeHoverOffset && this.settings.movement.edgeHoverMovement) || ((!this.inputs.useKeyCodeInputs) ? Input.GetButton(this.inputs.axis.button_camera_move_backward) : Input.GetKey(this.inputs.keycodes.CameraMoveBackward)))
			{
				a += this.requirements.pivot.transform.forward;
			}
			if ((Input.mousePosition.y > (float)Screen.height - this.settings.movement.edgeHoverOffset && this.settings.movement.edgeHoverMovement) || ((!this.inputs.useKeyCodeInputs) ? Input.GetButton(this.inputs.axis.button_camera_move_forward) : Input.GetKey(this.inputs.keycodes.CameraMoveForward)))
			{
				a -= this.requirements.pivot.transform.forward;
			}
			this.requirements.pivot.position += a.normalized * this.settings.movement.cameraMovementRate * Time.deltaTime;
			Vector3 zero = Vector3.zero;
			Vector3 vector = new Vector3(0f, this.requirements.pivot.position.y, 0f);
			if (this.settings.movement.useDefualtHeight)
			{
				zero.y = this.settings.movement.defualtHeight;
			}
			else
			{
				zero.y = this.requirements.pivot.position.y;
			}
			if ((zero - vector).magnitude > 0.2f)
			{
				Vector3 vector2 = Vector3.Lerp(vector, zero, this.settings.movement.lockTransitionRate);
				this.requirements.pivot.position = new Vector3(this.requirements.pivot.position.x, vector2.y, this.requirements.pivot.position.z);
			}
		}
	}

	private void CalculateCameraUpdates()
	{
		if (!this.changeInCamera)
		{
			return;
		}
		if (this.settings.zoom.maxZoom < this.settings.zoom.minZoom)
		{
			this.settings.zoom.maxZoom = this.settings.zoom.minZoom + 1f;
		}
		if (this._currentZoomAmount < this.settings.zoom.minZoom)
		{
			this._currentZoomAmount = this.settings.zoom.minZoom;
		}
		if (this._currentZoomAmount > this.settings.zoom.maxZoom)
		{
			this._currentZoomAmount = this.settings.zoom.maxZoom;
		}
		if (this._currentCameraRotation.x > 89f)
		{
			this._currentCameraRotation.x = 89f;
		}
		else if (this._currentCameraRotation.x < -89f)
		{
			this._currentCameraRotation.x = -89f;
		}
		Vector3 forward = Quaternion.AngleAxis(this._currentCameraRotation.y, Vector3.up) * Vector3.forward;
		this.requirements.pivot.transform.rotation = Quaternion.LookRotation(forward);
		Vector3 vector = this.requirements.pivot.transform.TransformDirection(Vector3.forward);
		vector = Quaternion.AngleAxis(this._currentCameraRotation.x, this.requirements.pivot.transform.TransformDirection(Vector3.right)) * vector;
		this.requirements.offset.position = vector * this._currentZoomAmount + this.requirements.pivot.position;
		this.requirements.offset.transform.LookAt(this.requirements.pivot);
		this.changeInCamera = false;
	}

	private void CalculateCameraBoundaries()
	{
		if (this.settings.useBoundaries && !((!this.inputs.useKeyCodeInputs) ? Input.GetButton(this.inputs.axis.button_camera_move_right) : Input.GetKey(this.inputs.keycodes.CameraMoveRight)) && !Moba_Camera_Boundaries.isPointInBoundary(this.requirements.pivot.position))
		{
			Moba_Camera_Boundary closestBoundary = Moba_Camera_Boundaries.GetClosestBoundary(this.requirements.pivot.position);
			if (closestBoundary != null)
			{
				this.requirements.pivot.position = Moba_Camera_Boundaries.GetClosestPointOnBoundary(closestBoundary, this.requirements.pivot.position);
			}
		}
	}

	public void SetTargetTransform(Transform t)
	{
		if (base.transform != null)
		{
			this.settings.lockTargetTransform = t;
		}
	}

	public void SetCameraRotation(Vector2 rotation)
	{
		this.currentCameraRotation = new Vector2(rotation.x, rotation.y);
	}

	public void SetCameraRotation(float x, float y)
	{
		this.currentCameraRotation = new Vector2(x, y);
	}

	public void SetCameraZoom(float amount)
	{
		this.currentZoomAmount = amount;
	}

	public Camera GetCamera()
	{
		return this.requirements.camera;
	}
}
