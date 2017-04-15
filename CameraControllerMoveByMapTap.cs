using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CameraControllerMoveByMapTap : CameraControllerBase
{
	private Transform _xzOffsetNodeTransform;

	private Units _unit;

	private Transform _unitTransform;

	private Units _targetUnit;

	private Transform _targetUnitTransform;

	private Vector2 _xzOffsetVector2;

	private int _lastState;

	private CameraMoveTask _rootTask;

	private bool firstCalcXZOffset;

	private Vector3 touchPosition;

	private float moveTime;

	private CoroutineManager coroutineManager = new CoroutineManager();

	private float _lastTimeChangeState;

	private Vector3 _lastXZTargetPos;

	private Vector3 _lastUnitPos;

	private float _lastTimeChangeXZOffsetTargetPos;

	private bool changeTarget;

	public CameraControllerMoveByMapTap(BattleCameraMgr battleCameraMgr, Transform cameraRoot, Camera camera) : base(battleCameraMgr, cameraRoot, camera)
	{
		this._xzOffsetNodeTransform = this.CameraRootTransform.FindChild("XZOffsetNode");
		this._rootTask = new CameraMoveTask(this.CameraRootTransform);
		this.firstCalcXZOffset = true;
	}

	private int CalcCurState()
	{
		int num = this._lastState;
		if (this._targetUnit == null || !this._targetUnit.isLive)
		{
			num = 1;
		}
		else
		{
			Vector3 vector = this.Camera.WorldToScreenPoint(this._unitTransform.position);
			Vector3 vector2 = this.Camera.WorldToScreenPoint(this._targetUnitTransform.position);
			float num2 = Math.Abs(vector.x - vector2.x);
			float num3 = Math.Abs(vector.y - vector2.y);
			if ((double)num2 > (double)Screen.width * 0.5)
			{
				if (this._lastState != 1 && (double)num2 > (double)Screen.width * 0.5 + 10.0)
				{
					num = 1;
				}
				else
				{
					num = this._lastState;
				}
			}
			else if ((double)num2 <= (double)Screen.width * 0.5 && (double)num3 <= (double)Screen.height * 0.3)
			{
				if (this._lastState != 2 && (double)num2 <= (double)Screen.width * 0.5 - 10.0 && (double)num3 <= (double)Screen.height * 0.3 - 5.0)
				{
					num = 2;
				}
				else
				{
					num = this._lastState;
				}
			}
			else
			{
				Vector3 lhs = this._targetUnitTransform.position - this._unitTransform.position;
				if (Vector3.Dot(lhs, this._unitTransform.forward) > 0f)
				{
					num = 3;
				}
				else
				{
					num = 1;
				}
				if (this._lastState != num && (double)num3 < (double)Screen.height * 0.3 + 10.0)
				{
					num = this._lastState;
				}
			}
		}
		return num;
	}

	private bool Move(Transform trans, Vector3 pos, float speed)
	{
		Vector3 vector = pos - trans.localPosition;
		float num = Time.deltaTime * speed;
		Vector3 b = vector;
		bool result = true;
		if (vector.magnitude > num)
		{
			b = vector.normalized * num;
			result = false;
		}
		trans.localPosition += b;
		return result;
	}

	public override void Update()
	{
	}

	private Vector3 CalcRootTargetPos()
	{
		Vector3 vector = this.touchPosition + this.BattleCameraMgr.DiffOfCameraRootAndRole;
		return new Vector3(vector.x, this.CameraRootTransform.position.y, vector.z);
	}

	private Vector3 CalcXZTargetPos(int state)
	{
		switch (state)
		{
		case 1:
			return this.GetXZOffsetTargetPosition();
		case 2:
		{
			Vector3 direction = (this._targetUnitTransform.position + this._unitTransform.position) * 0.5f - this._unitTransform.position;
			Vector3 result = this.Camera.transform.TransformDirection(direction);
			result.y = 0f;
			return result;
		}
		case 3:
			return new Vector3(0f, 0f, this.GetXZOffsetTargetPosition().z);
		default:
			return Vector3.zero;
		}
	}

	private Vector3 GetXZOffsetTargetPosition()
	{
		if (this.firstCalcXZOffset)
		{
			this.firstCalcXZOffset = false;
			this._lastUnitPos = this.touchPosition;
			Vector3 xZOffsetTargetPositionByDiff = this.GetXZOffsetTargetPositionByDiff(this.touchPosition);
			this._lastXZTargetPos = xZOffsetTargetPositionByDiff;
			return this._lastXZTargetPos;
		}
		if ((double)Vector3.Distance(this.touchPosition, this._lastUnitPos) > 0.1)
		{
			Vector3 xZOffsetTargetPositionByDiff2 = this.GetXZOffsetTargetPositionByDiff(this.touchPosition - this._lastUnitPos);
			this._lastUnitPos = this.touchPosition;
			if (Time.realtimeSinceStartup - this._lastTimeChangeXZOffsetTargetPos > this.BattleCameraMgr.ChangeXZTargetPosTimeDiff)
			{
				this._lastTimeChangeXZOffsetTargetPos = Time.realtimeSinceStartup;
				this._lastXZTargetPos = xZOffsetTargetPositionByDiff2;
				return xZOffsetTargetPositionByDiff2;
			}
		}
		return this._lastXZTargetPos;
	}

	private Vector3 GetXZOffsetTargetPositionByDiff(Vector3 diff)
	{
		bool flag = diff.x > 0f;
		Vector3 result = new Vector3((!flag) ? (-this._xzOffsetVector2.x) : this._xzOffsetVector2.x, 0f, 0f);
		bool flag2 = false;
		bool flag3 = false;
		float num = Vector3.Dot(diff.normalized, Vector3.forward);
		if ((double)num > 0.7)
		{
			flag3 = true;
			if ((double)num > 0.85)
			{
				result.x = 0f;
			}
		}
		else if ((double)num < -0.7)
		{
			flag2 = true;
			if ((double)num < -0.85)
			{
				result.x = 0f;
			}
		}
		if (flag2)
		{
			result.z = -this._xzOffsetVector2.y;
		}
		else if (flag3)
		{
			result.z = this._xzOffsetVector2.y * 1.7f;
		}
		return result;
	}

	public override void SetRoleObj(Units role, bool moveCameraImmediately = false)
	{
		if (this._unit != role)
		{
			this._targetUnit = null;
			this._targetUnitTransform = null;
		}
		this._unit = role;
		if (role.transform != null)
		{
			this._unitTransform = role.transform;
		}
		if (role.tag == "Home")
		{
			this._xzOffsetVector2 = Vector2.zero;
		}
		else if (role.atk_type == 1)
		{
			this._xzOffsetVector2 = new Vector2(0.09f * this.BattleCameraMgr.m_fScreenWidth, 0.03f * this.BattleCameraMgr.m_fScreenWidth);
		}
		else if (role.atk_type == 2)
		{
			this._xzOffsetVector2 = new Vector2(0.18f * this.BattleCameraMgr.m_fScreenWidth, 0.06f * this.BattleCameraMgr.m_fScreenWidth);
		}
		if (moveCameraImmediately || this.moveTime == 0f)
		{
			this.CameraRootTransform.localPosition = this.CalcRootTargetPos();
			this.moveTime = 0.1f;
		}
		else
		{
			this._rootTask.MoveFromTo(this.CameraRootTransform.localPosition, this.CalcRootTargetPos(), this.moveTime);
		}
	}

	public override void SetTarget(Units target)
	{
		this._targetUnit = target;
		this._targetUnitTransform = target.transform;
		this.changeTarget = true;
	}

	public override void PlayerDeath()
	{
		this.BattleCameraMgr.ChangeCameraController(CameraControllerType.Free);
	}

	public override void PlayerRespawn()
	{
	}

	public override void RestoreCameraController()
	{
		if (Singleton<PvpManager>.Instance.IsObserver && PvpObserveMgr.IsObserveFree)
		{
			this.moveTime = 0f;
			return;
		}
		if (GlobalSettings.Instance.isLockView)
		{
			Units player = PlayerControlMgr.Instance.GetPlayer();
			if (player != null && player.isLive)
			{
				if (player.isShowSkillPointer())
				{
					return;
				}
				this.touchPosition = PlayerControlMgr.Instance.GetPlayer().transform.position;
				this.moveTime = 0.1f;
				this.SetRoleObj(PlayerControlMgr.Instance.GetPlayer(), false);
			}
		}
		this.BattleCameraMgr.corMg.StartCoroutine(this.DelayRestoreCameraController(), true);
	}

	public override void RestoreCameraController(float inMoveTime)
	{
		if (Singleton<PvpManager>.Instance.IsObserver && PvpObserveMgr.IsObserveFree)
		{
			return;
		}
		if (GlobalSettings.Instance.isLockView)
		{
			Units player = PlayerControlMgr.Instance.GetPlayer();
			if (player != null && player.isLive)
			{
				this.touchPosition = PlayerControlMgr.Instance.GetPlayer().transform.position;
				this.moveTime = inMoveTime;
				this.SetRoleObj(PlayerControlMgr.Instance.GetPlayer(), false);
			}
		}
		this.BattleCameraMgr.corMg.StartCoroutine(this.DelayRestoreCameraController(), true);
	}

	[DebuggerHidden]
	private IEnumerator DelayRestoreCameraController()
	{
		CameraControllerMoveByMapTap.<DelayRestoreCameraController>c__Iterator19 <DelayRestoreCameraController>c__Iterator = new CameraControllerMoveByMapTap.<DelayRestoreCameraController>c__Iterator19();
		<DelayRestoreCameraController>c__Iterator.<>f__this = this;
		return <DelayRestoreCameraController>c__Iterator;
	}

	public override void SetPosition(Vector3 v3)
	{
		this.touchPosition = v3;
		this.SetRoleObj(PlayerControlMgr.Instance.GetPlayer(), false);
	}

	public override void SetPositionAndMoveTime(Vector3 inPos, float inMoveTime)
	{
		this.touchPosition = inPos;
		this.moveTime = inMoveTime;
		this.SetRoleObj(PlayerControlMgr.Instance.GetPlayer(), false);
		UnityEngine.Debug.Log("SetPositionAndMoveTime");
	}
}
