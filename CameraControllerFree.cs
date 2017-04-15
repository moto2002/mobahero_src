using MobaHeros.Pvp;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerFree : CameraControllerBase
{
	private enum eOperatePhase
	{
		SingleFinger,
		MultiTouch,
		None
	}

	private Vector3 m_moveDirection;

	private Vector3 m_priorPoint = Vector3.zero;

	private Vector3 m_priorPoint2 = Vector3.zero;

	private Vector3 m_priorPoint3 = Vector3.zero;

	private float m_fDistance;

	private float m_fIncludedAngle;

	private float m_fDampingValue = 0.93f;

	private bool m_bIsFirstTouch = true;

	private bool m_bCanSlide;

	private CameraControllerFree.eOperatePhase m_curOperatePhase = CameraControllerFree.eOperatePhase.None;

	private Vector3 m_firstPoint;

	private Vector3 m_firstTouchPoint;

	private int oldTouchCount;

	private List<int> newfinger = new List<int>();

	private Touch[] oldTouch;

	private bool isFind;

	private bool needStop = true;

	private int finger;

	public CameraControllerFree(BattleCameraMgr battleCameraMgr, Transform CameraRootTransform, Camera camera) : base(battleCameraMgr, CameraRootTransform, camera)
	{
	}

	public void SetClipDistance(float fDistance)
	{
		this.m_fDistance = fDistance;
	}

	public void SetIncludedAngle(float angle)
	{
		this.m_fIncludedAngle = 0f - angle;
	}

	private Vector3 CalcMoveDirection(Vector3 startPoint, Vector3 endPoint)
	{
		return this.Camera.ViewportToWorldPoint(startPoint) - this.Camera.ViewportToWorldPoint(endPoint);
	}

	public void Operate_Battle(Vector3 point)
	{
		point = new Vector3(point.x, point.y, this.m_fDistance);
		if (this.m_priorPoint != Vector3.zero)
		{
			this.m_moveDirection = this.CalcMoveDirection(this.m_priorPoint, point);
			this.CameraRootTransform.position += new Vector3(this.m_moveDirection.x, 0f, this.m_moveDirection.z) * (BattleCameraMgr.Instance.GetSpeedSliderValue() * 1.5f + 0.5f);
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
	}

	public bool CheckSlide()
	{
		if (Mathf.Abs(this.m_moveDirection.x) < 0.01f && Mathf.Abs(this.m_moveDirection.z) < 0.01f)
		{
			return false;
		}
		this.m_moveDirection -= this.m_moveDirection * ((1f - this.m_fDampingValue) * (60f * Time.deltaTime));
		this.CameraRootTransform.position += new Vector3(this.m_moveDirection.x, 0f, this.m_moveDirection.z);
		return true;
	}

	public void Stop()
	{
		this.m_priorPoint3 = (this.m_priorPoint2 = (this.m_priorPoint = Vector3.zero));
		this.m_moveDirection = Vector3.zero;
	}

	public void StopSlide()
	{
		this.m_moveDirection = Vector3.zero;
	}

	public override void Update()
	{
		if (TeamSignalManager.IsBegin)
		{
			return;
		}
		this.ProcessTouch();
		if (this.m_bCanSlide && !this.CheckSlide())
		{
			this.m_bCanSlide = false;
		}
	}

	private void ProcessTouch()
	{
		if (Input.touchCount > 0)
		{
			if (this.oldTouchCount != Input.touchCount && Input.touchCount < 3)
			{
				if (this.oldTouchCount < Input.touchCount)
				{
					for (int i = 0; i < Input.touchCount; i++)
					{
						if (!this.newfinger.Contains(i))
						{
							this.newfinger.Add(Input.touches[i].fingerId);
						}
					}
				}
				else
				{
					int num = -1;
					for (int j = 0; j < this.newfinger.Count; j++)
					{
						this.isFind = false;
						if (num == -1)
						{
							for (int k = 0; k < Input.touchCount; k++)
							{
								if (Input.touches[k].fingerId == this.newfinger[j])
								{
									this.isFind = true;
								}
							}
							if (!this.isFind)
							{
								num = j;
							}
						}
					}
					this.newfinger.RemoveAt(num);
				}
				this.oldTouchCount = Input.touchCount;
			}
			if (this.m_bIsFirstTouch)
			{
				this.m_bIsFirstTouch = false;
				this.m_bCanSlide = false;
				this.StopSlide();
			}
		}
		else if (Input.touchCount == 0)
		{
			this.m_bIsFirstTouch = true;
			if (this.m_curOperatePhase != CameraControllerFree.eOperatePhase.None)
			{
				this.Stop();
				this.m_bCanSlide = true;
				this.m_curOperatePhase = CameraControllerFree.eOperatePhase.None;
				this.newfinger.Clear();
				this.oldTouchCount = 0;
			}
		}
		if (Input.touchCount > 0 && Input.GetTouch(this.GetNewFinger()).phase == TouchPhase.Moved)
		{
			Vector2 deltaPosition = Input.GetTouch(this.GetNewFinger()).deltaPosition;
			this.CameraRootTransform.Translate(-deltaPosition.x * BattleCameraMgr.Instance.GetSpeed(), 0f, -deltaPosition.y * BattleCameraMgr.Instance.GetSpeed());
		}
	}

	private int GetNewFinger()
	{
		if (this.newfinger.Count > 0)
		{
			this.finger = this.newfinger[this.newfinger.Count - 1];
			if (this.finger >= Input.touchCount)
			{
				this.finger = Input.touchCount - 1;
			}
			return this.finger;
		}
		return 0;
	}

	private void CalcDistanceAndAngle()
	{
		Ray ray = this.Camera.ScreenPointToRay(this.m_firstPoint / (float)Gray.rtScal);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, 999f, Layer.GroundMask))
		{
			this.SetClipDistance(Vector3.Distance(this.Camera.transform.position, raycastHit.point));
		}
	}

	public override void SetRoleObj(Units role, bool moveCameraImmediately = false)
	{
		CameraControllerBase cameraController = this.BattleCameraMgr.GetCameraController(CameraControllerType.Follow);
		cameraController.SetRoleObj(role, moveCameraImmediately);
		this.BattleCameraMgr.ChangeCameraController(CameraControllerType.Follow);
	}

	public override void PlayerRespawn()
	{
		if (this.BattleCameraMgr.GetCameraControllerTypeBeforeDeath() == CameraControllerType.Free)
		{
			this.BattleCameraMgr.ChangeCameraController(CameraControllerType.Center);
			this.BattleCameraMgr.DelayChangeToFree();
		}
		else
		{
			this.BattleCameraMgr.ChangeCameraController(CameraControllerType.Center);
		}
	}

	public override void SetTouchMiniMapPosition()
	{
		this.BattleCameraMgr.ChangeCameraController(CameraControllerType.MoveByTap);
	}
}
