using System;
using UnityEngine;

public abstract class CameraControllerBase
{
	protected BattleCameraMgr BattleCameraMgr;

	protected Transform CameraRootTransform;

	protected Camera Camera;

	protected CameraControllerBase(BattleCameraMgr battleCameraMgr, Transform cameraRootTransform, Camera camera)
	{
		this.BattleCameraMgr = battleCameraMgr;
		this.CameraRootTransform = cameraRootTransform;
		this.Camera = camera;
	}

	public virtual void OnEnter()
	{
	}

	public abstract void Update();

	public virtual void OnExit()
	{
	}

	public virtual void SetRoleObj(Units role, bool moveCameraImmediately = false)
	{
	}

	public virtual void SetTarget(Units target)
	{
	}

	public virtual void PlayerDeath()
	{
	}

	public virtual void PlayerRespawn()
	{
	}

	public virtual void SetTouchMiniMapPosition()
	{
	}

	public virtual void RestoreCameraController()
	{
	}

	public virtual void RestoreCameraController(float inMoveTime)
	{
	}

	public virtual void SetPosition(Vector3 v3)
	{
	}

	public virtual void SetPositionAndMoveTime(Vector3 inPos, float inMoveTime)
	{
	}
}
