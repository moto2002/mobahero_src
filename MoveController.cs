using Com.Game.Module;
using Com.Game.Utils;
using Common;
using MobaFrame.Move;
using MobaFrame.SkillAction;
using MobaHeros.Pvp;
using MobaHeros.Spawners;
using MobaProtocol;
using MobaProtocol.Data;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MoveController : UnitComponent
{
	public float specialMoveSpeed;

	private float specialMoveStopDist;

	private Units specialMoveTarget;

	private Vector3 specialMovePos;

	private Vector3 curTargetPos;

	private Vector3 nextTargetPos;

	private bool newIsMoving;

	private bool hasNextTargetPos;

	private float newLastMoveTime;

	public Vector3 CurPosition;

	public Vector3 LocalPosition;

	private Vector3 forcePos;

	private bool isForced;

	private MoveState moveState;

	public float CurFacing;

	public float FinalFacing;

	public static float TurnSpeed = 2400f;

	public bool isMovingEntity;

	public NavgationAgent navAgent;

	public static bool TestSmoothMove = true;

	public Vector3 ServerPosition;

	public Vector3 ServerStartPos;

	public Vector3 ServerTargetPos;

	public float ServerMoveTime;

	public bool SendMoveToPosWithPath;

	private DistanceToTargetType distanceType;

	private Vector3 playerPos = MoveController.InvalidPos;

	private static readonly float sSearchInterval = 0.5f;

	private bool _pauseMoveInPvp;

	private long lastPauseMoveInPvp;

	private Vector3 lastSendTartgetPoint = new Vector3(999f, -1111111f, 12321f);

	private float lastSendTime;

	private EMoveState curMoveState;

	private Vector3 curMoveStatePos = MoveController.InvalidPos;

	private Vector3 curMoveStatePos_Stash = MoveController.InvalidPos;

	private static readonly Vector3 InvalidPos = new Vector3(909999f, 123128f, 783234f);

	private int curMoveStateTargetId;

	private long lastSearchPath;

	private Vector3 movingPoint = Vector3.zero;

	public static bool ShowMoveDelay = false;

	private static long TestMoveOrderTime = 0L;

	private MoveWithPath moveWithPath;

	private ABPath path;

	public static readonly Vector3 ServerPosInvalide = new Vector3(99999f, 99999f, 99999f);

	private Task TaskTurn;

	protected Vector3 lastFoundWaypointPosition;

	private long lastServerTick;

	private Vector3 lastServerPos;

	private string testString;

	private double lastSnapFrameTime;

	private Vector3 dir;

	private Vector3 preMovePos;

	private float preMoveServerTime;

	private float preMoveClientTime;

	public float moveSpeedFactorOfSrcPosDiff = 1f;

	public bool isMoving
	{
		get
		{
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				return this.newIsMoving;
			}
			return this.navAgent != null && this.navAgent.isMoving;
		}
	}

	public DistanceToTargetType m_DistanceToTargetType
	{
		get
		{
			return this.distanceType;
		}
		set
		{
			this.distanceType = value;
			if (this.navAgent != null)
			{
				this.navAgent.m_DistanceToTargetType = this.distanceType;
			}
		}
	}

	public bool PauseMoveInPvp
	{
		get
		{
			return this._pauseMoveInPvp;
		}
		set
		{
			if (!this._pauseMoveInPvp && value)
			{
				this.lastPauseMoveInPvp = DateTime.Now.Ticks;
			}
			this._pauseMoveInPvp = value;
		}
	}

	public MoveController()
	{
	}

	public MoveController(Units self) : base(self)
	{
	}

	public override void OnInit()
	{
		this.newLastMoveTime = 0f;
		this.curTargetPos = base.transform.position;
		this.nextTargetPos = this.curTargetPos;
		this.hasNextTargetPos = false;
		this.newIsMoving = false;
		this.isForced = false;
		this.CurPosition = base.transform.position;
		this.LocalPosition = base.transform.position;
		this.FinalFacing = base.transform.rotation.eulerAngles.y;
		this.CurFacing = this.FinalFacing;
		if (this.isMovingEntity && !Singleton<PvpManager>.Instance.IsInPvp)
		{
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				this.navAgent.isCreateObstacle = (this.self.isHero && BaseSpawner.IsDynamicObstacle);
			}
			else
			{
				this.navAgent.isCreateObstacle = BaseSpawner.IsDynamicObstacle;
			}
			this.navAgent.OnInit();
		}
	}

	public override void OnStart()
	{
		if (this.navAgent != null)
		{
			NavgationAgent expr_11 = this.navAgent;
			expr_11.OnStopPathCallback = (Callback)Delegate.Combine(expr_11.OnStopPathCallback, new Callback(this.OnStopPathCB));
		}
	}

	public static void InitMapHeight()
	{
	}

	public void OnUpdateByFrame(float deltaTime)
	{
		deltaTime *= this.moveSpeedFactorOfSrcPosDiff;
		if (this.curMoveState == EMoveState.MoveState_MoveToPos && this.curMoveStatePos_Stash != MoveController.InvalidPos && (float)(DateTime.Now.Ticks - this.lastSearchPath) > 1E+07f * MoveController.sSearchInterval)
		{
			this.MoveToPoint(new Vector3?(this.curMoveStatePos_Stash), 0f, true);
		}
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			if (this.specialMoveSpeed > 0f)
			{
				Vector3 a = (!this.specialMoveTarget) ? this.specialMovePos : this.specialMoveTarget.transform.position;
				Vector3 vector = a - this.CurPosition;
				float num = vector.Magnitude2D();
				float num2 = this.specialMoveSpeed * ((!MoveController.TestSmoothMove) ? FrameSyncManager.Instance.mNetDeltaTime : deltaTime);
				if (num2 + this.specialMoveStopDist < num)
				{
					this.CurPosition += vector * (num2 / num);
				}
				else
				{
					if (this.specialMoveStopDist < num)
					{
						this.CurPosition = a - vector * (this.specialMoveStopDist / num);
					}
					this.specialMoveSpeed = 0f;
					if (this.moveState != MoveState.MoveState_StartMove && this.moveState != MoveState.MoveState_NextPos)
					{
						this.newIsMoving = false;
					}
				}
				this.self.SetPosition(this.CurPosition, false);
			}
			else if (this.newIsMoving)
			{
				this.testString = string.Empty;
				Vector3 curPosition = this.CurPosition;
				float num3 = (this.CurPosition - this.self.transform.position).Magnitude2D();
				float num4 = this.self.move_speed * ((!MoveController.TestSmoothMove) ? FrameSyncManager.Instance.mNetDeltaTime : deltaTime);
				if (!FrameSyncManager.Instance.WaitFrameTime)
				{
					Vector3 vector2 = this.ServerTargetPos - this.ServerPosition;
					float num5 = vector2.Magnitude2D();
					if (num4 < num5)
					{
						this.ServerPosition += vector2 * (num4 / num5);
					}
					else
					{
						this.ServerPosition = this.ServerTargetPos;
					}
				}
				float num6;
				while (true)
				{
					num6 = (this.curTargetPos - this.CurPosition).Magnitude2D();
					if (num4 < num6)
					{
						break;
					}
					this.CurPosition = this.curTargetPos;
					if (!this.hasNextTargetPos)
					{
						goto IL_2D3;
					}
					this.hasNextTargetPos = false;
					this.curTargetPos = this.nextTargetPos;
					num4 -= num6;
				}
				this.CurPosition += (this.curTargetPos - this.CurPosition) / num6 * num4;
				goto IL_2EB;
				IL_2D3:
				this.testString = "!";
				this.newIsMoving = false;
				this.OnStopMove();
				IL_2EB:
				if (!GlobalSettings.Instance.PvpSetting.isPlayerMoveBeforeServer || this.self.isPlayer)
				{
				}
				this.playerPos = this.CurPosition;
			}
			else if (!GlobalSettings.Instance.PvpSetting.isPlayerMoveBeforeServer || !this.self.isPlayer)
			{
				if (this.self.animController != null && Time.time - this.newLastMoveTime > 0.1f && this.self.animController.IsInMove())
				{
					this.self.animController.PlayAnim(AnimationType.Move, false, 0, true, false);
				}
			}
			if (this.isForced)
			{
				this.CurPosition = this.forcePos;
				this.self.SetPosition(this.forcePos, false);
				this.isForced = false;
			}
			if (FrameSyncManager.Instance.WaitFrameTime)
			{
				Vector3 vector3 = this.CurPosition - this.LocalPosition;
				float num7 = vector3.Magnitude2D();
				if (num7 > 0.01f)
				{
					float num8 = this.self.move_speed * ((!MoveController.TestSmoothMove) ? FrameSyncManager.Instance.mNetDeltaTime : deltaTime);
					if (num8 < num7)
					{
						vector3 = vector3 / num7 * num8;
					}
					Vector3 vector4 = this.preMovePos - this.LocalPosition;
					float num9 = vector4.Magnitude2D();
					if (num8 < num9)
					{
						vector4 = vector4 / num9 * num8;
					}
					this.newLastMoveTime = Time.time;
					Vector3 newPos = this.LocalPosition + (vector4 + vector3) / 2f;
					this.TurnTo(vector4 + vector3, false);
					this.self.SetPosition(newPos, false);
				}
			}
			else if ((this.CurPosition - this.LocalPosition).Magnitude2D() > 0.01f)
			{
				Vector3 vector5 = this.ServerPosition - this.CurPosition;
				float num10 = vector5.Magnitude2D();
				float num11 = 0.1f * this.self.move_speed * ((!MoveController.TestSmoothMove) ? FrameSyncManager.Instance.mNetDeltaTime : deltaTime);
				if (num11 < num10)
				{
					this.CurPosition += vector5 * (num11 / num10);
				}
				else
				{
					this.CurPosition = this.ServerPosition;
				}
				this.self.SetPosition(this.CurPosition, false);
			}
			if (GlobalSettings.Instance.PvpSetting.isPlayerMoveBeforeServer && this.self.isPlayer && this.playerPos != MoveController.InvalidPos && this.XZSqrMagnitude(this.self.transform.position, this.playerPos) > 25f)
			{
				this.self.SetPosition(this.playerPos, false);
			}
		}
		this.UpdateTurnning(deltaTime);
	}

	public override void OnUpdate(float deltaTime)
	{
	}

	public void SpecialMove(Units target, float stopDist, float speed)
	{
		this.moveState = MoveState.MoveState_SpecialMove;
		if (this.isMoving && this.specialMoveSpeed == 0f)
		{
			this.OnStopMove();
		}
		if (!this.self.isPlayer || !GlobalSettings.Instance.PvpSetting.isPlayerMoveBeforeServer)
		{
			this.specialMoveSpeed = speed;
			this.specialMoveTarget = target;
			this.specialMoveStopDist = stopDist;
			this.curTargetPos = target.transform.position;
			this.nextTargetPos = this.curTargetPos;
			this.hasNextTargetPos = false;
			this.newIsMoving = true;
		}
		else
		{
			this.CurPosition = target.transform.position;
			this.playerPos = this.CurPosition;
		}
		this.PauseMoveInPvp = false;
	}

	public void SpecialMove(Vector3 pos, float _speed)
	{
		this.moveState = MoveState.MoveState_SpecialMove;
		if (this.isMoving && this.specialMoveSpeed == 0f)
		{
			this.OnStopMove();
		}
		if (!this.self.isPlayer || !GlobalSettings.Instance.PvpSetting.isPlayerMoveBeforeServer)
		{
			this.specialMoveSpeed = _speed;
			if (_speed == 0f)
			{
				this.CurPosition = pos;
				this.self.SetPosition(pos, false);
				this.newIsMoving = false;
			}
			else
			{
				this.specialMoveTarget = null;
				this.specialMoveStopDist = 0f;
				this.specialMovePos = pos;
				this.curTargetPos = pos;
				this.nextTargetPos = pos;
				this.hasNextTargetPos = false;
				this.newIsMoving = true;
			}
		}
		else
		{
			this.CurPosition = pos;
			this.playerPos = pos;
		}
		this.PauseMoveInPvp = false;
	}

	public override void OnStop()
	{
		this.newIsMoving = false;
		this.specialMoveSpeed = 0f;
		if (this.navAgent != null)
		{
			NavgationAgent expr_23 = this.navAgent;
			expr_23.OnStopPathCallback = (Callback)Delegate.Remove(expr_23.OnStopPathCallback, new Callback(this.OnStopPathCB));
		}
	}

	public override void OnExit()
	{
		if (this.navAgent != null)
		{
			NavgationAgent expr_11 = this.navAgent;
			expr_11.OnStopPathCallback = (Callback)Delegate.Remove(expr_11.OnStopPathCallback, new Callback(this.OnStopPathCB));
		}
	}

	public void ResetMoveInfosOnRelive()
	{
		this.newLastMoveTime = 0f;
		this.curTargetPos = base.transform.position;
		this.nextTargetPos = this.curTargetPos;
		this.hasNextTargetPos = false;
		this.newIsMoving = false;
		this.CurPosition = base.transform.position;
		this.FinalFacing = base.transform.rotation.eulerAngles.y;
		this.CurFacing = this.FinalFacing;
		this.playerPos = base.transform.position;
		this.curMoveState = EMoveState.MoveState_Idle;
		this.curMoveStatePos = MoveController.InvalidPos;
		this.lastServerPos = base.transform.position;
		this.self.serverPos = base.transform.position;
		this.PauseMoveInPvp = false;
	}

	public int GetTagsChange()
	{
		if (this.navAgent != null)
		{
			return this.navAgent.GetTagsChange();
		}
		return 0;
	}

	public void SetTagsChange(int tag)
	{
		if (this.navAgent != null)
		{
			this.navAgent.SetTagsChange(tag);
		}
	}

	public void ResetTagsChange()
	{
		if (this.navAgent != null)
		{
			this.navAgent.ResetTagsChange();
		}
	}

	public bool isReachedTarget()
	{
		return this.navAgent != null && this.navAgent.isMoving && this.navAgent.CheckReached();
	}

	public void SendMoveToPos(Vector3 targetPoint, float stop_distance)
	{
		MoveToPos data = new MoveToPos
		{
			unitId = this.self.unique_id,
			pos = new SVector3
			{
				x = targetPoint.x,
				y = targetPoint.y,
				z = targetPoint.z
			}
		};
		if (this.self.isPlayer || (this.self.MirrorState && this.self.ParentUnit.isPlayer))
		{
			PvpEvent.SendMoveToPos(SerializeHelper.Serialize<MoveToPos>(data));
		}
		this.lastSendTartgetPoint = targetPoint;
	}

	private void SendMoveToTarget(int targetId, Vector3 targetPos, float stopDistance)
	{
		if ((this.self.isPlayer || (this.self.MirrorState && this.self.ParentUnit.isPlayer)) && Singleton<PvpManager>.Instance.IsInPvp && GameManager.IsPlaying())
		{
			MoveToTarget data = new MoveToTarget
			{
				unitId = this.self.unique_id,
				pos = MoveController.Vector3ToSVector3(this.self.transform.position),
				targetId = targetId,
				targetPos = MoveController.Vector3ToSVector3(targetPos),
				stopDis = stopDistance
			};
			PvpEvent.SendMoveToTarget(SerializeHelper.Serialize<MoveToTarget>(data));
		}
	}

	public void MoveToTarget(Units target)
	{
		this.MoveToTarget(target, this.self.GetAttackRange(target) * 0.8f);
	}

	public void MoveToTarget(Units target, float stopDistance)
	{
		if (target != null && this.self.CanMove)
		{
			bool flag = false;
			if (this.curMoveState == EMoveState.MoveState_MoveToTarget)
			{
				if (this.curMoveStateTargetId == target.unique_id)
				{
					if (DateTime.Now.Ticks - this.lastSearchPath > 5000000L)
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			this.curMoveState = EMoveState.MoveState_MoveToTarget;
			this.curMoveStateTargetId = target.unique_id;
			this.lastSearchPath = DateTime.Now.Ticks;
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				if (this.self.isPlayer || (this.self.MirrorState && this.self.ParentUnit.isPlayer))
				{
					this.self.InterruptAction(SkillInterruptType.Initiative);
					float num = UnitFeature.DistanceToTarget(this.self.mTransform, target.mTransform);
					if (num < stopDistance + 3f)
					{
						this.m_DistanceToTargetType = DistanceToTargetType.NearByTarget;
					}
					else
					{
						this.m_DistanceToTargetType = DistanceToTargetType.FarFromTarget;
					}
					if (GlobalSettings.Instance.PvpSetting.isPlayerMoveBeforeServer && this.navAgent != null)
					{
						this.navAgent.MoveToTarget(target, stopDistance);
					}
					this.SendMoveToTarget(target.unique_id, target.transform.position, stopDistance);
				}
			}
			else
			{
				this.self.InterruptAction(SkillInterruptType.Initiative);
				float num2 = UnitFeature.DistanceToTarget(this.self.mTransform, target.mTransform);
				if (num2 < stopDistance + 3f)
				{
					this.m_DistanceToTargetType = DistanceToTargetType.NearByTarget;
				}
				else
				{
					this.m_DistanceToTargetType = DistanceToTargetType.FarFromTarget;
				}
				if (this.navAgent != null)
				{
					this.navAgent.MoveToTarget(target, stopDistance);
				}
			}
		}
	}

	public bool ContinueMove()
	{
		if (this.movingPoint == Vector3.zero)
		{
			return false;
		}
		Vector3 forward = this.self.transform.forward;
		forward.y = 0f;
		Vector3 rhs = this.movingPoint - this.self.transform.position;
		rhs.Normalize();
		if (Vector3.Dot(forward, rhs) < 0.3f)
		{
			this.movingPoint = Vector3.zero;
			return false;
		}
		float num = Vector3.Distance(this.self.transform.position, this.movingPoint);
		if (num < 0.5f)
		{
			this.movingPoint = Vector3.zero;
			return false;
		}
		this.MoveToPoint(new Vector3?(this.movingPoint), -1f, false);
		return true;
	}

	public void MoveToPoint(Vector3? targetPoint, float stop_distance, bool forceSearch = false)
	{
		if (targetPoint.HasValue && this.self.CanMove)
		{
			bool flag = forceSearch;
			if (!forceSearch)
			{
				if (this.curMoveState == EMoveState.MoveState_MoveToPos)
				{
					if ((double)Vector3.Dot(targetPoint.Value - this.self.transform.position, this.self.transform.forward) < 0.7)
					{
						flag = true;
					}
					else
					{
						float num = (this.curMoveStatePos - targetPoint.Value).Magnitude2D();
						float num2 = num / (this.self.transform.position - targetPoint.Value).Magnitude2D();
						if ((double)num2 < 0.04 || num < 16f)
						{
							this.curMoveStatePos_Stash = targetPoint.Value;
						}
						else
						{
							flag = true;
						}
					}
				}
				else
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			this.curMoveState = EMoveState.MoveState_MoveToPos;
			this.curMoveStatePos = targetPoint.Value;
			this.lastSearchPath = DateTime.Now.Ticks;
			this.curMoveStatePos_Stash = MoveController.InvalidPos;
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				if (GlobalSettings.Instance.PvpSetting.isPlayerMoveBeforeServer && this.self.isPlayer)
				{
					if (this.navAgent != null)
					{
						this.navAgent.MoveToPoint(targetPoint, stop_distance);
					}
					this.self.InterruptAction(SkillInterruptType.Initiative);
					this.self.PlayAnim(AnimationType.Move, true, 0, true, false);
				}
				if (this.self.isPlayer || (this.self.MirrorState && this.self.ParentUnit.isPlayer))
				{
					if (MoveController.ShowMoveDelay)
					{
						MoveController.TestMoveOrderTime = Stopwatch.GetTimestamp();
					}
					this.SendMoveToPos(targetPoint.Value, stop_distance);
				}
			}
			else
			{
				this.self.InterruptAction(SkillInterruptType.Initiative);
				if (this.navAgent != null)
				{
					this.navAgent.MoveToPoint(targetPoint, stop_distance);
				}
			}
		}
	}

	public void MoveToPoint_Impl(SVector3 curPos, SVector3 targetPoint, float stop_distance)
	{
	}

	public void MoveToPosWithPath_Impl(MoveWithPath item)
	{
		if (GlobalSettings.Instance.PvpSetting.isPlayerMoveBeforeServer && Singleton<PvpManager>.Instance.IsInPvp && this.self.isPlayer)
		{
			return;
		}
		if (item == null)
		{
			return;
		}
		this.moveWithPath = item;
		this.self.serverTargetPos = MoveController.ServerPosInvalide;
		if (this.self.isMonster)
		{
			this.self.serverTargetPos = MoveController.SVectgor3ToVector3(item.toPos);
			this.path = ABPath.Construct(this.self.transform.position, MoveController.SVectgor3ToVector3(item.toPos), null);
			this.path.vectorPath = new List<Vector3>();
			foreach (SVector3 current in item.path)
			{
				this.path.vectorPath.Add(MoveController.SVectgor3ToVector3(current));
			}
			this.navAgent.MoveWithPath(item.targetUnitId, MoveController.SVectgor3ToVector3(item.toPos), item.stopDistance, this.path);
			if (item.tick > this.lastServerTick)
			{
				this.lastServerTick = item.tick;
				this.lastServerPos = MoveController.SVectgor3ToVector3(item.pos);
			}
		}
	}

	public void StopMove()
	{
		if (!Singleton<PvpManager>.Instance.IsInPvp)
		{
			if (this.isMoving && this.navAgent != null)
			{
				this.navAgent.StopMove();
			}
		}
		else if ((this.self.isHero || this.self.isPlayer) && this.isMoving)
		{
			StopMove stopMove = new StopMove();
			stopMove.rotate = this.self.transform.eulerAngles.y;
			stopMove.unitId = this.self.unique_id;
			stopMove.pos = MoveController.Vector3ToSVector3(this.self.transform.position);
			stopMove.tick = UnitsSnapReporter.Instance.SyncTicks;
			if (this.self.IsMaster)
			{
			}
		}
		this.curMoveState = EMoveState.MoveState_Idle;
	}

	public void StopMoveForSkill()
	{
		if (!Singleton<PvpManager>.Instance.IsInPvp)
		{
			if (this.isMoving && this.navAgent != null)
			{
				this.navAgent.StopMove();
			}
		}
		else if ((this.self.isHero || this.self.isPlayer) && this.isMoving)
		{
			StopMove data = new StopMove
			{
				rotate = this.self.transform.eulerAngles.y,
				unitId = this.self.unique_id,
				pos = MoveController.Vector3ToSVector3(this.self.transform.position),
				tick = UnitsSnapReporter.Instance.SyncTicks
			};
			if (this.self.IsMaster)
			{
				PvpEvent.SendStopMove(SerializeHelper.Serialize<StopMove>(data));
			}
		}
		this.curMoveState = EMoveState.MoveState_Idle;
	}

	public void StopMove_Impl(SVector3 curPos, float rotate)
	{
		if (GlobalSettings.Instance.PvpSetting.isPlayerMoveBeforeServer && Singleton<PvpManager>.Instance.IsInPvp && this.self.isPlayer)
		{
			return;
		}
		this.self.serverTargetPos = MoveController.ServerPosInvalide;
		Vector3 position = base.transform.position;
		Vector3 vector = MoveController.SVectgor3ToVector3(curPos);
		Vector3 serverTargetPos = this.self.serverTargetPos;
		if (this.navAgent != null)
		{
			this.navAgent.StopMove();
		}
	}

	private void OnStopMove()
	{
		if (this.self.isPlayer)
		{
			if (this.self.mCmdRunningController != null)
			{
				this.self.mCmdRunningController.OnMoveEnd();
			}
			this.OnStopPathCB();
		}
		this.TryMonsterCreepTurnToSpawnDir();
		this.TryMonsterCreepStopMoveAnim();
	}

	public void clearContinueMovingTarget()
	{
		this.movingPoint = Vector3.zero;
	}

	public void setContinueMoveTarget(Vector3 target)
	{
		this.movingPoint = target;
	}

	private void OnStopPathCB()
	{
		bool flag = this.movingPoint == Vector3.zero;
		if (!flag)
		{
			flag = (0.5f > Vector3.Distance(this.movingPoint, this.self.transform.position));
		}
		if (flag)
		{
			this.movingPoint = Vector3.zero;
		}
		this.m_DistanceToTargetType = DistanceToTargetType.ReachedTarget;
	}

	public void AddSearchingCallback(Callback OnSearchPathCallback, Callback OnStopPathCallback)
	{
		if (this.navAgent != null)
		{
			NavgationAgent expr_11 = this.navAgent;
			expr_11.OnSearchPathCallback = (Callback)Delegate.Combine(expr_11.OnSearchPathCallback, OnSearchPathCallback);
			NavgationAgent expr_2D = this.navAgent;
			expr_2D.OnStopPathCallback = (Callback)Delegate.Combine(expr_2D.OnStopPathCallback, OnStopPathCallback);
		}
	}

	public void RemoveSearchingCallback(Callback OnSearchPathCallback, Callback OnStopPathCallback)
	{
		if (this.navAgent != null)
		{
			NavgationAgent expr_11 = this.navAgent;
			expr_11.OnSearchPathCallback = (Callback)Delegate.Remove(expr_11.OnSearchPathCallback, OnSearchPathCallback);
			NavgationAgent expr_2D = this.navAgent;
			expr_2D.OnStopPathCallback = (Callback)Delegate.Remove(expr_2D.OnStopPathCallback, OnStopPathCallback);
		}
	}

	public Task TurnToTarget(Vector3? targetPoint, bool isFast, bool isForce = false, float limitTime = 0f)
	{
		if (!this.self.isLive)
		{
			return null;
		}
		if (!this.self.CanRoatate && !isForce)
		{
			return null;
		}
		if (targetPoint.HasValue)
		{
			this.TurnTo(targetPoint.Value - base.transform.position, isFast);
			return null;
		}
		return null;
	}

	public void TurnTo(float facAngle, bool isFast)
	{
		this.FinalFacing = facAngle;
		if (isFast || MoveController.TurnSpeed > 9998.9f)
		{
			this.CurFacing = this.FinalFacing;
			this.self.transform.rotation = Quaternion.Euler(0f, this.CurFacing, 0f);
		}
	}

	public void TurnTo(Vector3 facVect, bool isFast)
	{
		this.TurnTo(Mathf.Atan2(facVect.x, facVect.z) * 57.29578f, isFast);
	}

	public void UpdateTurnning(float deltaTime)
	{
		if (GlobalSettings.Instance.PvpSetting.isPlayerMoveBeforeServer && Singleton<PvpManager>.Instance.IsInPvp && this.self.isPlayer)
		{
			return;
		}
		if (Mathf.Abs(this.CurFacing - this.FinalFacing) < 0.1f || MoveController.TurnSpeed == 0f)
		{
			return;
		}
		float num = 1.4f;
		float num2 = 179f;
		float num3;
		for (num3 = this.FinalFacing - this.CurFacing; num3 < -180f; num3 += 360f)
		{
		}
		while (num3 >= 180f)
		{
			num3 -= 360f;
		}
		float num4 = Mathf.Abs(num3);
		float num5 = (num4 <= num2) ? (MoveController.TurnSpeed * (num4 + 1f) / (num2 + 1f)) : MoveController.TurnSpeed;
		num5 *= num;
		float num6 = num5 * deltaTime;
		if (num4 < num6)
		{
			this.CurFacing = this.FinalFacing;
		}
		else if (num3 < 0f)
		{
			this.CurFacing -= num6;
		}
		else
		{
			this.CurFacing += num6;
		}
		this.self.transform.rotation = Quaternion.Euler(0f, this.CurFacing, 0f);
	}

	[DebuggerHidden]
	private IEnumerator TurnToTarget_Coroutinue(Quaternion newRotation)
	{
		MoveController.<TurnToTarget_Coroutinue>c__Iterator36 <TurnToTarget_Coroutinue>c__Iterator = new MoveController.<TurnToTarget_Coroutinue>c__Iterator36();
		<TurnToTarget_Coroutinue>c__Iterator.newRotation = newRotation;
		<TurnToTarget_Coroutinue>c__Iterator.<$>newRotation = newRotation;
		<TurnToTarget_Coroutinue>c__Iterator.<>f__this = this;
		return <TurnToTarget_Coroutinue>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator TurnToTarget_Coroutinue2(Quaternion newRotation)
	{
		MoveController.<TurnToTarget_Coroutinue2>c__Iterator37 <TurnToTarget_Coroutinue2>c__Iterator = new MoveController.<TurnToTarget_Coroutinue2>c__Iterator37();
		<TurnToTarget_Coroutinue2>c__Iterator.newRotation = newRotation;
		<TurnToTarget_Coroutinue2>c__Iterator.<$>newRotation = newRotation;
		<TurnToTarget_Coroutinue2>c__Iterator.<>f__this = this;
		return <TurnToTarget_Coroutinue2>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator TurnToTarget_Coroutinue3(Quaternion newRotation, float limitTime)
	{
		MoveController.<TurnToTarget_Coroutinue3>c__Iterator38 <TurnToTarget_Coroutinue3>c__Iterator = new MoveController.<TurnToTarget_Coroutinue3>c__Iterator38();
		<TurnToTarget_Coroutinue3>c__Iterator.newRotation = newRotation;
		<TurnToTarget_Coroutinue3>c__Iterator.limitTime = limitTime;
		<TurnToTarget_Coroutinue3>c__Iterator.<$>newRotation = newRotation;
		<TurnToTarget_Coroutinue3>c__Iterator.<$>limitTime = limitTime;
		<TurnToTarget_Coroutinue3>c__Iterator.<>f__this = this;
		return <TurnToTarget_Coroutinue3>c__Iterator;
	}

	public void StopTurn()
	{
		if (this.TaskTurn != null)
		{
			this.m_CoroutineManager.StopCoroutine(this.TaskTurn);
		}
	}

	public void SetIsMoving(bool moving)
	{
		this.navAgent.isMoving = moving;
	}

	public static SVector3 Vector3ToSVector3(Vector3 pos)
	{
		return new SVector3
		{
			x = pos.x,
			y = pos.y,
			z = pos.z
		};
	}

	public static Vector3 SVectgor3ToVector3(SVector3 pos)
	{
		return new Vector3(pos.x, pos.y, pos.z);
	}

	private float CalcForward(Vector3 position, Vector3 curServerPos, ABPath path)
	{
		float result = 1f;
		int index = this.GetIndex(position, path, 1f);
		int index2 = this.GetIndex(curServerPos, path, 1f);
		if (index < index2)
		{
			float magnitude = (curServerPos - position).magnitude;
			float num = 1f;
			result = magnitude / num / this.self.move_speed + 1f;
		}
		else if (index2 < index)
		{
			float magnitude2 = (curServerPos - position).magnitude;
			float num2 = 1f;
			result = 1f / (magnitude2 / num2 / this.self.move_speed + 1f);
		}
		return result;
	}

	private int GetIndex(Vector3 pos, ABPath path, float distance)
	{
		if (path == null || path.vectorPath == null || path.vectorPath.Count == 0)
		{
			return 0;
		}
		List<Vector3> vectorPath = path.vectorPath;
		if (vectorPath.Count == 1)
		{
			vectorPath.Insert(0, this.lastServerPos);
		}
		int i = vectorPath.Count - 1;
		if (i >= vectorPath.Count)
		{
			i = vectorPath.Count - 1;
		}
		if (i <= 1)
		{
			i = 1;
		}
		while (i > 0)
		{
			float num = this.XZSqrMagnitude(vectorPath[i], pos);
			if (num < distance * distance)
			{
				this.lastFoundWaypointPosition = this.lastServerPos;
				return i;
			}
			i--;
		}
		return i;
	}

	private Vector3 CalcCurServerPos(long lastServerTick, Vector3 lastServerPos, ABPath path)
	{
		long syncTicks = UnitsSnapReporter.Instance.SyncTicks;
		float num = ((float)(syncTicks - lastServerTick) / 1E+07f - 0.1f) * this.self.move_speed;
		if (path == null || path.vectorPath == null || path.vectorPath.Count == 0)
		{
			return lastServerPos;
		}
		List<Vector3> vectorPath = path.vectorPath;
		if (vectorPath.Count == 1)
		{
			vectorPath.Insert(0, lastServerPos);
		}
		int num2 = this.GetIndex(lastServerPos, path, 1f);
		Vector3 vector = vectorPath[num2];
		float num3 = (vector - lastServerPos).magnitude;
		if (num3 < num)
		{
			bool flag = false;
			while (num2 + 1 < vectorPath.Count)
			{
				float magnitude = (vectorPath[num2 + 1] - vectorPath[num2]).magnitude;
				num3 += magnitude;
				if (num3 > num)
				{
					num3 -= magnitude;
					vector = this.CalculateTargetPoint(vectorPath[num2], vectorPath[num2], vectorPath[num2 + 1], num - num3);
					flag = true;
					break;
				}
				num2++;
			}
			if (!flag)
			{
				vector = vectorPath[vectorPath.Count - 1];
			}
		}
		return vector;
	}

	private string DumpPath(ABPath p)
	{
		string text = "length:" + p.vectorPath.Count;
		foreach (Vector3 current in p.vectorPath)
		{
			text = text + "," + current;
		}
		return text;
	}

	protected float XZSqrMagnitude(Vector3 a, Vector3 b)
	{
		float num = b.x - a.x;
		float num2 = b.z - a.z;
		return num * num + num2 * num2;
	}

	protected Vector3 CalculateTargetPoint(Vector3 p, Vector3 a, Vector3 b, float distance)
	{
		a.y = p.y;
		b.y = p.y;
		float magnitude = (a - b).magnitude;
		if (magnitude == 0f)
		{
			return a;
		}
		float num = AstarMath.Clamp01(AstarMath.NearestPointFactor(a, b, p));
		Vector3 a2 = (b - a) * num + a;
		float magnitude2 = (a2 - p).magnitude;
		float num2 = Mathf.Clamp(distance - magnitude2, 0f, distance);
		float num3 = num2 / magnitude;
		num3 = Mathf.Clamp(num3 + num, 0f, 1f);
		return (b - a) * num3 + a;
	}

	public void PreMove(Vector3 targPos)
	{
		this.preMovePos = targPos;
		this.preMoveServerTime = (float)((double)FrameSyncManager.Instance.newNetFrameNum * ((!FrameSyncManager.Instance.UseFrame) ? 1E-07 : FrameSyncManager.Instance.OneFrameTime));
		this.preMoveClientTime = Time.time;
		this.newLastMoveTime = Time.time;
		if (!FrameSyncManager.Instance.WaitFrameTime)
		{
			float num = this.preMoveServerTime;
			float num2 = num - this.ServerMoveTime;
			Vector3 vector = this.ServerTargetPos - this.ServerStartPos;
			float num3 = vector.Magnitude2D();
			float num4 = this.self.move_speed * num2;
			if (num4 < num3)
			{
				this.ServerStartPos += vector * (num4 / num3);
			}
			else
			{
				this.ServerStartPos = this.ServerTargetPos;
			}
			this.ServerMoveTime = num;
			this.ServerTargetPos = targPos;
			this.ServerPosition = this.ServerStartPos;
			num2 = (float)FrameSyncManager.Instance.mPredictNetTime - num;
			if (num2 > 0f)
			{
				vector = this.ServerTargetPos - this.ServerStartPos;
				num3 = vector.Magnitude2D();
				num4 = this.self.move_speed * num2;
				if (num4 < num3)
				{
					this.ServerPosition += vector * (num4 / num3);
				}
				else
				{
					this.ServerPosition = this.ServerTargetPos;
				}
			}
		}
		if (!this.newIsMoving && !this.self.IsMonsterCreep())
		{
			this.self.animController.PlayAnim(AnimationType.Move, true, 0, true, false);
		}
		this.dir = targPos - this.nextTargetPos;
		if (this.dir.x > 0.001f || this.dir.x < -0.001f || this.dir.z > 0.001f || this.dir.z < -0.001f)
		{
			this.TurnTo(this.dir, false);
			if (this.self.IsMonsterCreep())
			{
				this.TryMonsterCreepPlayMoveAnim();
			}
		}
	}

	public void PreStop(Vector3 targPos)
	{
		if (!FrameSyncManager.Instance.WaitFrameTime)
		{
			this.ServerStartPos = targPos;
			this.ServerTargetPos = targPos;
			this.ServerPosition = targPos;
			this.ServerMoveTime = (float)((double)FrameSyncManager.Instance.newNetFrameNum * ((!FrameSyncManager.Instance.UseFrame) ? 1E-07 : FrameSyncManager.Instance.OneFrameTime));
		}
	}

	public void ServerMove(Vector3 targPos)
	{
	}

	public void P2C_UnitsSnap(UnitSnapInfo info)
	{
		if (info == null)
		{
			return;
		}
		info.pos.y = this.self.mTransform.position.y;
		this.dir = info.pos.ToVector3() - this.nextTargetPos;
		if (this.dir.x < 0.001f && this.dir.x > -0.001f && this.dir.z < 0.001f && this.dir.z > -0.001f)
		{
			return;
		}
		this.moveState = info.state;
		switch (info.state)
		{
		case MoveState.MoveState_StartMove:
			this.self.moveController.PreMove(info.pos.ToVector3());
			if (this.self.isPlayer && MoveController.TestMoveOrderTime != 0L)
			{
				double num = (double)(Stopwatch.GetTimestamp() - MoveController.TestMoveOrderTime) / (double)Stopwatch.Frequency;
				Cheat.Msg(new object[]
				{
					"Delay: ",
					num
				});
				MoveController.TestMoveOrderTime = 0L;
			}
			break;
		case MoveState.MoveState_NextPos:
			break;
		case MoveState.MoveState_StopMove:
			goto IL_2DC;
		case MoveState.MoveState_SpecialMove:
			goto IL_36D;
		case MoveState.MoveState_StopSpecialMove:
			this.specialMoveSpeed = 0f;
			goto IL_2DC;
		case MoveState.MoveState_ForceMove:
			this.forcePos = info.pos.ToVector3();
			this.isForced = true;
			goto IL_372;
		default:
			goto IL_36D;
		}
		this.self.moveController.PreMove(info.pos.ToVector3());
		this.hasNextTargetPos = (this.newIsMoving && info.state == MoveState.MoveState_NextPos);
		if (this.hasNextTargetPos)
		{
			this.curTargetPos = this.nextTargetPos;
			this.nextTargetPos = info.pos.ToVector3();
			this.dir = this.nextTargetPos - this.curTargetPos;
		}
		else
		{
			this.curTargetPos = info.pos.ToVector3();
			this.nextTargetPos = info.pos.ToVector3();
			this.dir = this.nextTargetPos - base.transform.position;
		}
		Vector3 a = info.srcPos.ToVector3();
		float num2 = Vector3.Distance(a, this.nextTargetPos);
		float num3 = Vector3.Distance(this.self.mTransform.position, this.nextTargetPos);
		if (num2 != 0f)
		{
			this.moveSpeedFactorOfSrcPosDiff = num3 / num2;
		}
		else
		{
			this.moveSpeedFactorOfSrcPosDiff = 1f;
		}
		this.newIsMoving = true;
		this.TurnTo(this.dir, false);
		if (!GlobalSettings.Instance.PvpSetting.isPlayerMoveBeforeServer || !this.self.isPlayer)
		{
			if (!this.self.IsMonsterCreep())
			{
				this.self.animController.PlayAnim(AnimationType.Move, true, 0, true, false);
			}
			else
			{
				this.TryMonsterCreepPlayMoveAnim();
			}
		}
		goto IL_372;
		IL_2DC:
		this.self.moveController.PreStop(info.pos.ToVector3());
		this.CurPosition = this.self.mTransform.position;
		this.self.SetPosition(this.CurPosition, false);
		this.curTargetPos = this.CurPosition;
		this.nextTargetPos = this.CurPosition;
		this.hasNextTargetPos = false;
		this.newIsMoving = false;
		this.OnStopMove();
		IL_36D:
		IL_372:
		this.lastServerPos = MoveController.SVectgor3ToVector3(info.pos);
		this.self.serverPos = MoveController.SVectgor3ToVector3(info.pos);
	}

	private void TryMonsterCreepTurnToSpawnDir()
	{
		if (!this.self.IsMonsterCreep() || !this.self.IsMonsterCreepAiStatus(EMonsterCreepAiStatus.AtSpawnPoint))
		{
			return;
		}
		Vector3 vector = this.self.mTransform.position - this.self.spwan_pos;
		vector.y = 0f;
		if (vector.sqrMagnitude > 1.33f)
		{
			return;
		}
		ActionManager.TweenRotate(this.self, this.self.mTransform.rotation.eulerAngles.y, this.self.spwan_rotation.eulerAngles.y, 0.2f);
	}

	private void TryMonsterCreepPlayMoveAnim()
	{
		if (this.self.animController.IsCanPlayMoveAnim())
		{
			this.self.animController.PlayAnim(AnimationType.Move, true, 0, true, false);
		}
	}

	private void TryMonsterCreepStopMoveAnim()
	{
		if (this.self.IsMonsterCreep())
		{
			this.self.animController.StopMoveAnim();
		}
	}
}
