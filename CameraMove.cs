using System;
using UnityEngine;

public class CameraMove
{
	private enum eOperatePhase
	{
		SingleFinger,
		MultiTouch,
		None
	}

	private Transform m_cameraRoot;

	private Camera m_camera;

	private Vector3 m_moveDirection;

	private Vector3 m_priorPoint = Vector3.zero;

	private Vector3 m_priorPoint2 = Vector3.zero;

	private Vector3 m_priorPoint3 = Vector3.zero;

	private float m_fDistance;

	private float m_fIncludedAngle;

	private float m_fDampingValue = 0.93f;

	private float m_fMaxZ = 14f;

	private float m_fMinZ = 6f;

	private float m_fMaxX = 23f;

	private float m_fMinX = -16f;

	private bool m_bIsFirstTouch = true;

	private bool m_bCanSlide;

	private CameraMove.eOperatePhase m_curOperatePhase = CameraMove.eOperatePhase.None;

	private Vector3 m_firstPoint;

	private Vector3 m_firstTouchPoint;

	public void Init(Transform cameraRoot, Camera camera)
	{
		this.m_cameraRoot = cameraRoot;
		this.m_camera = camera;
	}

	public void SetClipDistance(float fDistance)
	{
		this.m_fDistance = fDistance;
	}

	public void SetIncludedAngle(float angle)
	{
		this.m_fIncludedAngle = 0f - angle;
	}

	public void SetBoundaryValue(float minX, float maxX, float minZ, float maxZ)
	{
		this.m_fMinX = minX;
		this.m_fMaxX = maxX;
		this.m_fMinZ = minZ;
		this.m_fMaxZ = maxZ;
	}

	private Vector3 CalcMoveDirection(Vector3 startPoint, Vector3 endPoint)
	{
		return this.m_camera.ViewportToWorldPoint(startPoint) - this.m_camera.ViewportToWorldPoint(endPoint);
	}

	public void Operate_Battle(Vector3 point)
	{
		point = new Vector3(point.x, point.y, this.m_fDistance);
		if (this.m_priorPoint != Vector3.zero)
		{
			this.m_moveDirection = this.CalcMoveDirection(this.m_priorPoint, point);
			this.m_cameraRoot.position += new Vector3(this.m_moveDirection.x, 0f, this.m_moveDirection.z);
		}
		if (this.m_priorPoint3 != Vector3.zero)
		{
			this.m_moveDirection = this.CalcMoveDirection(this.m_priorPoint3, point) * 0.33f;
		}
		else if (this.m_priorPoint2 != Vector3.zero)
		{
			this.m_moveDirection = this.CalcMoveDirection(this.m_priorPoint2, point) * 0.5f;
		}
		this.m_priorPoint3 = this.m_priorPoint2;
		this.m_priorPoint2 = this.m_priorPoint;
		this.m_priorPoint = point;
		this.CheckMoveRange();
	}

	public bool CheckSlide()
	{
		if (Mathf.Abs(this.m_moveDirection.x) < 0.01f && Mathf.Abs(this.m_moveDirection.z) < 0.01f)
		{
			return false;
		}
		this.m_moveDirection -= this.m_moveDirection * ((1f - this.m_fDampingValue) * (60f * Time.deltaTime));
		this.m_cameraRoot.position += new Vector3(this.m_moveDirection.x, 0f, this.m_moveDirection.z);
		this.CheckMoveRange();
		return true;
	}

	public void Stop()
	{
		this.m_priorPoint3 = (this.m_priorPoint2 = (this.m_priorPoint = Vector3.zero));
	}

	public void StopSlide()
	{
		this.m_moveDirection = Vector3.zero;
	}

	private void CheckMoveRange()
	{
		if (this.m_cameraRoot.localPosition.x > this.m_fMaxX)
		{
			this.m_cameraRoot.localPosition = new Vector3(this.m_fMaxX, this.m_cameraRoot.localPosition.y, this.m_cameraRoot.localPosition.z);
		}
		else if (this.m_cameraRoot.localPosition.x < this.m_fMinX)
		{
			this.m_cameraRoot.localPosition = new Vector3(this.m_fMinX, this.m_cameraRoot.localPosition.y, this.m_cameraRoot.localPosition.z);
		}
		if (this.m_cameraRoot.localPosition.z > this.m_fMaxZ)
		{
			this.m_cameraRoot.localPosition = new Vector3(this.m_cameraRoot.localPosition.x, this.m_cameraRoot.localPosition.y, this.m_fMaxZ);
		}
		else if (this.m_cameraRoot.localPosition.z < this.m_fMinZ)
		{
			this.m_cameraRoot.localPosition = new Vector3(this.m_cameraRoot.localPosition.x, this.m_cameraRoot.localPosition.y, this.m_fMinZ);
		}
	}

	public void Update()
	{
		this.ProcessTouch();
		if (this.m_curOperatePhase == CameraMove.eOperatePhase.SingleFinger)
		{
			this.CalcDistanceAndAngle();
			this.Operate_Battle(this.m_camera.ScreenToViewportPoint(this.m_firstPoint / (float)Gray.rtScal));
		}
		if (this.m_bCanSlide && !this.CheckSlide())
		{
			this.m_bCanSlide = false;
		}
		if (GameManager.IsPlaying())
		{
			this.CheckMoveRange();
		}
	}

	private void ProcessTouch()
	{
		if (Input.touchCount > 0)
		{
			if (this.m_bIsFirstTouch)
			{
				this.m_firstTouchPoint = Input.GetTouch(0).position;
				this.m_bIsFirstTouch = false;
				this.m_bCanSlide = false;
				this.StopSlide();
			}
			if (this.m_curOperatePhase == CameraMove.eOperatePhase.SingleFinger)
			{
				this.m_firstPoint = Input.GetTouch(0).position;
			}
			else if (Vector3.Distance(this.m_firstTouchPoint, Input.GetTouch(0).position) > 5f)
			{
				this.m_curOperatePhase = CameraMove.eOperatePhase.SingleFinger;
				this.m_firstPoint = Input.GetTouch(0).position;
			}
		}
		else if (Input.touchCount == 0)
		{
			if (!this.m_bIsFirstTouch)
			{
				this.m_bIsFirstTouch = true;
			}
			if (this.m_curOperatePhase != CameraMove.eOperatePhase.None)
			{
				this.Stop();
				this.m_bCanSlide = true;
				this.m_curOperatePhase = CameraMove.eOperatePhase.None;
			}
		}
	}

	private void CalcDistanceAndAngle()
	{
		Ray ray = this.m_camera.ScreenPointToRay(this.m_firstPoint / (float)Gray.rtScal);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, 999f, 1 << LayerMask.NameToLayer("Ground")))
		{
			this.SetClipDistance(Vector3.Distance(this.m_camera.transform.position, raycastHit.point));
		}
	}
}
