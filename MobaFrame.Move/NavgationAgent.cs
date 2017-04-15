using Assets.MobaTools.TriggerPlugin.Scripts;
using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.Move
{
	public abstract class NavgationAgent : UnitComponent
	{
		protected const float m_Padding = 0.02f;

		protected NavgationState m_NavgationState;

		public bool needResearchWhenDynamicObstacleChange;

		private bool is_moving;

		private Units m_target;

		protected Transform m_NavTarget;

		protected float m_StopDistance;

		private static Transform m_navTargets;

		private static Dictionary<string, Transform> m_targetList;

		public DistanceToTargetType m_DistanceToTargetType;

		protected bool isSearchNearTarget;

		public bool isCreateObstacle = true;

		public Callback OnSearchPathCallback;

		public Callback OnStopPathCallback;

		public virtual bool isMoving
		{
			get
			{
				return this.is_moving;
			}
			set
			{
				this.is_moving = value;
			}
		}

		public Units mTarget
		{
			get
			{
				return this.m_target;
			}
			set
			{
				this.m_target = value;
			}
		}

		public static Transform m_NavTargets
		{
			get
			{
				if (NavgationAgent.m_navTargets == null)
				{
					GameObject gameObject = GameObject.Find("NavTargets");
					if (gameObject != null)
					{
						NavgationAgent.m_navTargets = gameObject.transform;
					}
					else
					{
						NavgationAgent.m_navTargets = new GameObject("NavTargets").transform;
					}
				}
				return NavgationAgent.m_navTargets;
			}
		}

		public static Dictionary<string, Transform> m_NavTargetList
		{
			get
			{
				if (NavgationAgent.m_targetList == null)
				{
					NavgationAgent.m_targetList = new Dictionary<string, Transform>();
				}
				return NavgationAgent.m_targetList;
			}
		}

		public NavgationAgent()
		{
		}

		public NavgationAgent(Units self) : base(self)
		{
		}

		public override void OnInit()
		{
			this.CreateNavTarget();
			this.InitPath();
			this.CreateObstacle();
			this.m_NavgationState = NavgationState.None;
		}

		public override void OnStart()
		{
		}

		public override void OnStop()
		{
			this.StopMove();
			this.EnableObstacle(false);
			this.DestroyPath();
		}

		public override void OnUpdate(float deltaTime)
		{
		}

		public override void OnDeath(Units attacker)
		{
			this.StopMove();
			this.EnableObstacle(false);
			this.DestroyPath();
		}

		public override void OnExit()
		{
			this.DestroyPath();
			this.DestroyNavTarget();
			this.DestroyObstacle();
		}

		protected void CreateNavTarget()
		{
			string childName = string.Format("{0}_target", this.self.unique_id);
			this.AddNavTarget(NavgationAgent.m_NavTargets, childName);
		}

		protected void AddNavTarget(Transform parentNode, string childName)
		{
			if (!NavgationAgent.m_NavTargetList.ContainsKey(childName))
			{
				GameObject gameObject = new GameObject();
				Transform transform = gameObject.transform;
				transform.name = childName;
				transform.parent = NavgationAgent.m_NavTargets;
				transform.position = this.self.mTransform.position;
				NavgationAgent.m_NavTargetList.Add(childName, transform);
				this.m_NavTarget = transform;
			}
			else
			{
				this.m_NavTarget = NavgationAgent.m_NavTargetList[childName];
			}
		}

		protected void DestroyNavTarget()
		{
			if (this.m_NavTarget != null)
			{
				if (NavgationAgent.m_NavTargetList.ContainsKey(this.m_NavTarget.name))
				{
					NavgationAgent.m_NavTargetList.Remove(this.m_NavTarget.name);
				}
				UnityEngine.Object.Destroy(this.m_NavTarget.gameObject);
				this.m_NavTarget = null;
			}
		}

		protected void ClearAllNavTargets()
		{
			if (NavgationAgent.m_NavTargetList != null && NavgationAgent.m_NavTargetList.Count > 0)
			{
				NavgationAgent.m_NavTargetList.Clear();
			}
		}

		public bool isReachedTarget()
		{
			return this.CheckReached();
		}

		public void MoveToTarget(Units target, float stop_distance)
		{
			if (this.self == null || !this.self.isLive)
			{
				return;
			}
			if (this.m_NavTarget == null || target == null)
			{
				return;
			}
			this.mTarget = target;
			if (!this.mTarget.isMoving && this.m_DistanceToTargetType == DistanceToTargetType.NearByTarget)
			{
				Vector3 position = this.mTarget.mTransform.position;
				if (this.self.isPlayer)
				{
				}
				this.m_NavTarget.position = position;
				this.m_StopDistance = stop_distance;
				this.MoveToPoint(this.m_NavTarget, 0f);
			}
			else
			{
				this.isSearchNearTarget = false;
				this.m_NavTarget.position = target.mTransform.position;
				this.m_StopDistance = stop_distance;
				this.MoveToPoint(this.m_NavTarget, this.m_StopDistance);
			}
		}

		public void MoveToPoint(Vector3? point, float stop_distance)
		{
			if (this.self == null || !this.self.isLive)
			{
				return;
			}
			if (this.m_NavTarget == null || !point.HasValue)
			{
				return;
			}
			this.m_StopDistance = stop_distance;
			if (this.m_StopDistance <= 0.02f)
			{
				this.m_StopDistance = 0.02f;
			}
			this.mTarget = null;
			this.m_NavTarget.position = point.Value;
			this.MoveToPoint(this.m_NavTarget, this.m_StopDistance);
		}

		public void MoveWithPath(int targetId, Vector3 point, float stop_distance, Path p)
		{
			if (this.self == null || !this.self.isLive)
			{
				return;
			}
			if (this.m_NavTarget == null)
			{
				return;
			}
			this.m_StopDistance = stop_distance;
			if (this.m_StopDistance <= 0.02f)
			{
				this.m_StopDistance = 0.02f;
			}
			this.mTarget = ((targetId == 0) ? null : MapManager.Instance.GetUnit(targetId));
			this.m_NavTarget.position = point;
			this.MoveWithPath(this.m_NavTarget, this.m_StopDistance, p);
		}

		private void MoveToPoint(Transform t, float stop_distance)
		{
			if (this.SearchPath(t, stop_distance))
			{
				this.OnSearchPath();
			}
		}

		private void MoveWithPath(Transform t, float stop_distance, Path p)
		{
			this.SearchPath(t, stop_distance, p);
			this.OnSearchPath();
		}

		public void StopMove()
		{
			if (!this.isMoving)
			{
				return;
			}
			this.isSearchNearTarget = false;
			this.mTarget = null;
			this.StopPath();
			this.OnStopPath();
		}

		protected virtual void SyncMoveState()
		{
			if (this.isMoving)
			{
				this.self.PlayAnim(AnimationType.Move, true, 0, true, false);
			}
			else
			{
				this.self.PlayAnim(AnimationType.Move, false, 0, true, false);
			}
		}

		protected virtual void OnSearchPath()
		{
			this.isMoving = true;
			if (this.m_NavgationState != NavgationState.Arrival)
			{
				this.m_NavgationState = NavgationState.SearchPath;
			}
			if (this.OnSearchPathCallback != null)
			{
				this.OnSearchPathCallback();
			}
		}

		protected virtual void OnStopPath()
		{
			this.isMoving = false;
			this.m_NavgationState = NavgationState.StopPath;
			if (this.OnStopPathCallback != null)
			{
				this.OnStopPathCallback();
			}
			TriggerParamNavigation triggerParamNavigation = new TriggerParamNavigation();
			triggerParamNavigation.EventID = 1;
			triggerParamNavigation.Trigger = this;
			triggerParamNavigation.IsPlayer = this.self.isPlayer;
			TriggerManager2.Instance.Trigger2(triggerParamNavigation);
		}

		protected abstract bool InitPath();

		protected abstract bool SearchPath(Transform t, float stop_distance);

		protected abstract void SearchPath(Transform t, float stop_distance, Path p);

		protected abstract void StopPath();

		public abstract bool CheckReached();

		protected abstract bool CheckObstacled(float deltaTime);

		protected abstract void DestroyPath();

		public virtual void EnableRotAndMove(bool enabled)
		{
		}

		protected virtual void CreateObstacle()
		{
		}

		protected virtual void EnableObstacle(bool enabled)
		{
		}

		protected virtual void DestroyObstacle()
		{
		}

		public virtual int GetTagsChange()
		{
			return 0;
		}

		public virtual void SetTagsChange(int tag)
		{
		}

		public virtual void ResetTagsChange()
		{
		}
	}
}
