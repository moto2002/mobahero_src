using System;
using UnityEngine;

public class CameraControllerAlwaysFree : CameraControllerFree
{
	public CameraControllerAlwaysFree(BattleCameraMgr battleCameraMgr, Transform cameraRootTransform, Camera camera) : base(battleCameraMgr, cameraRootTransform, camera)
	{
	}

	public override void PlayerRespawn()
	{
	}

	public override void SetRoleObj(Units role, bool moveCameraImmediately = false)
	{
	}

	public override void SetTarget(Units target)
	{
	}
}
