using System;
using UnityEngine;

public class FxmTestMouse : MonoBehaviour
{
	protected const float m_fDefaultDistance = 8f;

	protected const float m_fDefaultWheelSpeed = 5f;

	public Transform m_TargetTrans;

	public Camera m_GrayscaleCamara;

	public Shader m_GrayscaleShader;

	protected bool m_bRaycastHit;

	public float m_fDistance = 8f;

	public float m_fXSpeed = 350f;

	public float m_fYSpeed = 300f;

	public float m_fWheelSpeed = 5f;

	public float m_fYMinLimit = -90f;

	public float m_fYMaxLimit = 90f;

	public float m_fDistanceMin = 1f;

	public float m_fDistanceMax = 50f;

	public int m_nMoveInputIndex = 1;

	public int m_nRotInputIndex;

	public float m_fXRot;

	public float m_fYRot;

	protected bool m_bHandEnable = true;

	protected Vector3 m_MovePostion;

	protected Vector3 m_OldMousePos;

	protected bool m_bLeftClick;

	protected bool m_bRightClick;

	public void ChangeAngle(float angle)
	{
		this.m_fYRot = angle;
		this.m_fXRot = 0f;
		this.m_MovePostion = Vector3.zero;
	}

	public void SetHandControl(bool bEnable)
	{
		this.m_bHandEnable = bEnable;
	}

	public void SetDistance(float fDistance)
	{
		this.m_fDistance = fDistance;
		PlayerPrefs.SetFloat("FxmTestMouse.m_fDistance", this.m_fDistance);
		this.UpdateCamera(true);
	}

	private void OnEnable()
	{
		this.m_fDistance = PlayerPrefs.GetFloat("FxmTestMouse.m_fDistance", this.m_fDistance);
	}

	private void Start()
	{
		if (Camera.main == null)
		{
			return;
		}
		if (base.rigidbody)
		{
			base.rigidbody.freezeRotation = true;
		}
	}

	private bool IsGUIMousePosition()
	{
		Vector2 gUIMousePosition = NgLayout.GetGUIMousePosition();
		if (FxmTestMain.inst.GetFXMakerControls().GetActionToolbarRect().Contains(gUIMousePosition))
		{
			return true;
		}
		Rect rect = new Rect(0f, 0f, (float)Screen.width, (float)(Screen.height / 10 + 30));
		if (rect.Contains(gUIMousePosition))
		{
			return true;
		}
		Rect rect2 = new Rect(0f, 0f, 40f, (float)Screen.height);
		return rect2.Contains(gUIMousePosition);
	}

	private void Update()
	{
		if (this.IsGUIMousePosition() && !this.m_bLeftClick && !this.m_bRightClick)
		{
			return;
		}
		this.UpdateCamera(false);
	}

	public void UpdateCamera(bool bOnlyZoom)
	{
		if (Camera.main == null)
		{
			return;
		}
		if (this.m_fWheelSpeed < 0f)
		{
			this.m_fWheelSpeed = 5f;
		}
		float num = this.m_fDistance / 8f;
		float fDistance = this.m_fDistance;
		if (this.m_TargetTrans)
		{
			this.m_fDistance = Mathf.Clamp(this.m_fDistance - Input.GetAxis("Mouse ScrollWheel") * this.m_fWheelSpeed * num, this.m_fDistanceMin, this.m_fDistanceMax);
			if (Camera.main.orthographic)
			{
				Camera.main.orthographicSize = this.m_fDistance * 0.6f;
				if (this.m_GrayscaleCamara != null)
				{
					this.m_GrayscaleCamara.orthographicSize = this.m_fDistance * 0.6f;
				}
			}
			if (!bOnlyZoom && this.m_bRightClick && Input.GetMouseButton(this.m_nRotInputIndex))
			{
				this.m_fXRot += Input.GetAxis("Mouse X") * this.m_fXSpeed * 0.02f;
				this.m_fYRot -= Input.GetAxis("Mouse Y") * this.m_fYSpeed * 0.02f;
			}
			if (!bOnlyZoom && Input.GetMouseButtonDown(this.m_nRotInputIndex))
			{
				this.m_bRightClick = true;
			}
			if (!bOnlyZoom && Input.GetMouseButtonUp(this.m_nRotInputIndex))
			{
				this.m_bRightClick = false;
			}
			this.m_fYRot = FxmTestMouse.ClampAngle(this.m_fYRot, this.m_fYMinLimit, this.m_fYMaxLimit);
			Quaternion rotation = Quaternion.Euler(this.m_fYRot, this.m_fXRot, 0f);
			RaycastHit raycastHit;
			if (this.m_bRaycastHit && Physics.Linecast(this.m_TargetTrans.position, Camera.main.transform.position, out raycastHit))
			{
				this.m_fDistance -= raycastHit.distance;
			}
			Vector3 point = new Vector3(0f, 0f, -this.m_fDistance);
			Vector3 position = rotation * point + this.m_TargetTrans.position;
			Camera.main.transform.rotation = rotation;
			Camera.main.transform.position = position;
			this.UpdatePosition(Camera.main.transform);
			if (this.m_GrayscaleCamara != null)
			{
				this.m_GrayscaleCamara.transform.rotation = Camera.main.transform.rotation;
				this.m_GrayscaleCamara.transform.position = Camera.main.transform.position;
			}
			if (fDistance != this.m_fDistance)
			{
				PlayerPrefs.SetFloat("FxmTestMouse.m_fDistance", this.m_fDistance);
			}
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	private void UpdatePosition(Transform camera)
	{
		if (this.m_bHandEnable)
		{
			if (Input.GetMouseButtonDown(this.m_nMoveInputIndex))
			{
				this.m_OldMousePos = Input.mousePosition;
				this.m_bLeftClick = true;
			}
			if (this.m_bLeftClick && Input.GetMouseButton(this.m_nMoveInputIndex))
			{
				Vector3 mousePosition = Input.mousePosition;
				float worldPerScreenPixel = NgLayout.GetWorldPerScreenPixel(this.m_TargetTrans.transform.position);
				this.m_MovePostion += (this.m_OldMousePos - mousePosition) * worldPerScreenPixel;
				this.m_OldMousePos = mousePosition;
			}
			if (Input.GetMouseButtonUp(this.m_nMoveInputIndex))
			{
				this.m_bLeftClick = false;
			}
		}
		camera.Translate(this.m_MovePostion, Space.Self);
	}
}
