using System;
using UnityEngine;

public class CameraControllerCenter : CameraControllerBase
{
	private Units _unit;

	public CameraControllerCenter(BattleCameraMgr battleCameraMgr, Transform cameraRootTransform, Camera camera) : base(battleCameraMgr, cameraRootTransform, camera)
	{
	}

	public override void Update()
	{
		if (this._unit != null && this._unit.transform != null)
		{
			this.CameraRootTransform.position = this._unit.transform.position + this.BattleCameraMgr.DiffOfCameraRootAndRole;
		}
	}

	public override void SetRoleObj(Units role, bool moveCameraImmediately = false)
	{
		this._unit = role;
	}

	public override void SetTarget(Units target)
	{
	}

	public override void PlayerDeath()
	{
		this.BattleCameraMgr.ChangeCameraController(CameraControllerType.Free);
	}

	public override void SetTouchMiniMapPosition()
	{
		this.BattleCameraMgr.ChangeCameraController(CameraControllerType.MoveByTap);
	}
}
