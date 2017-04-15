using System;
using UnityEngine;

namespace MobaHeros.AI
{
	public class TargetingSystem : ITargetingSystem
	{
		protected Units m_Owner;

		protected SensoryMemory m_SensoryMemory;

		protected GoalTarget[] m_Goals;

		private bool isInitTargets;

		private bool _autoTest;

		public WatchTarget Watcher
		{
			get;
			protected set;
		}

		public bool IsSearchTarget
		{
			get;
			set;
		}

		public TargetingSystem(Units owner, SensoryMemory memory)
		{
			this.m_Owner = owner;
			this.m_SensoryMemory = memory;
			this.InitTargets();
			this.IsSearchTarget = false;
			if (AutoTestController.UseAI)
			{
				this._autoTest = true;
			}
		}

		public virtual void Update()
		{
			this.UpdateTargetState();
			this.UpdateBuffTarget();
			if (this.IsSearchTarget || this._autoTest)
			{
				this.UpdateAttackTarget();
			}
		}

		public virtual void UpdateTargetState()
		{
		}

		public virtual void UpdateBuffTarget()
		{
		}

		public virtual void UpdateAttackTarget()
		{
		}

		public virtual bool SelectOutTarget()
		{
			return false;
		}

		public virtual bool SelectVisiableTarget()
		{
			return false;
		}

		public virtual bool SelectMapTarget()
		{
			return false;
		}

		public virtual void InitTargets()
		{
			if (!this.isInitTargets)
			{
				this.m_Goals = new GoalTarget[11];
				this.isInitTargets = true;
			}
		}

		public virtual void ClearTargets()
		{
			for (int i = 0; i < this.m_Goals.Length; i++)
			{
				this.m_Goals[i] = null;
			}
		}

		public virtual void OnSpecialSKillEffBegin(SpecialSkillEff eff)
		{
		}

		public virtual void OnSpecialSkillEffEnd(SpecialSkillEff eff)
		{
		}

		public virtual void DefendTower(Units attacker, Units tower)
		{
		}

		public void ForceUpdate(Units guardTarget)
		{
			if (this.Watcher == null)
			{
				this.Watcher = new WatchTarget(Time.time, guardTarget);
			}
		}

		public bool CheckInputTarget(InputTargetType targetType, GoalTarget goal)
		{
			if (goal != null)
			{
				Units unit = goal.m_Unit;
				if (targetType == InputTargetType.MoveTarget)
				{
					return unit != null && unit.isLive;
				}
				if (unit == null || !unit.isLive || !unit.CanBeSelected || !TeamManager.CanAttack(this.m_Owner, goal.m_Unit) || UnitFeature.DistanceToTarget(this.m_Owner.transform, unit.transform) > this.m_Owner.fog_range)
				{
					return false;
				}
				switch (targetType)
				{
				case InputTargetType.AttackTarget:
				case InputTargetType.KillTarget:
				case InputTargetType.BuffTarget:
				case InputTargetType.FixedTarget:
				case InputTargetType.AttackYouTarget:
				case InputTargetType.TauntTarget:
				case InputTargetType.GuardTarget:
					return true;
				case InputTargetType.SelectTarget:
					return this.m_Owner.isPlayer;
				}
			}
			return false;
		}

		public void SetSelectTarget(Units target)
		{
			this.SetInputTarget(InputTargetType.SelectTarget, target);
		}

		public void SetAttackTarget(Units target)
		{
			this.SetInputTarget(InputTargetType.AttackTarget, target);
		}

		public void SetTauntTarget(Units target)
		{
			this.SetInputTarget(InputTargetType.TauntTarget, target);
		}

		public Units GetAttackTarget()
		{
			return (Units)this.GetOutputTarget(OutputTargetType.OutputAttackTarget);
		}

		public Units GetSelectTarget()
		{
			return (Units)this.GetOutputTarget(OutputTargetType.SelectTarget);
		}

		public Units GetTreeAtkTarget()
		{
			return (Units)this.GetOutputTarget(OutputTargetType.TreeAttackTarget);
		}

		public void SetAttackYouTarget(Units target)
		{
			this.SetInputTarget(InputTargetType.AttackYouTarget, target);
		}

		public void SetGuardTarget(Units target)
		{
			this.SetInputTarget(InputTargetType.GuardTarget, target);
		}

		public Units GetAttackYouTarget()
		{
			GoalTarget inputTarget = this.GetInputTarget(InputTargetType.AttackYouTarget);
			if (inputTarget != null)
			{
				return inputTarget.m_Unit;
			}
			return null;
		}

		public Units GetGuardTarget()
		{
			GoalTarget inputTarget = this.GetInputTarget(InputTargetType.GuardTarget);
			if (inputTarget != null)
			{
				return inputTarget.m_Unit;
			}
			return null;
		}

		protected GoalTarget GetInputTarget(InputTargetType targetType)
		{
			return this.m_Goals[(int)targetType];
		}

		protected void SetInputTarget(InputTargetType targetType, Units target)
		{
			if (target != null)
			{
				if (this.m_Goals[(int)targetType] != null)
				{
					bool flag = this.m_Goals[(int)targetType].m_Unit == target;
					this.m_Goals[(int)targetType].Set(targetType, target);
				}
				else
				{
					GoalTarget goalTarget = new GoalTarget();
					goalTarget.Set(targetType, target);
					this.m_Goals[(int)targetType] = goalTarget;
				}
			}
			else
			{
				this.m_Goals[(int)targetType] = null;
			}
		}

		protected object GetOutputTarget(OutputTargetType targetType)
		{
			return null;
		}

		public bool isTargetPresent(InputTargetType targetType)
		{
			return null != this.m_Goals[(int)targetType];
		}

		public bool isTargetWithinFOV(InputTargetType targetType)
		{
			return false;
		}

		public bool isTargetShootable(InputTargetType targetType)
		{
			return false;
		}

		public Vector3? GetLastRecordedPosition(InputTargetType targetType)
		{
			return null;
		}

		public double GetTimeTargetHasBeenVisible(InputTargetType targetType)
		{
			return 0.0;
		}

		public double GetTimeTargetHasBeenOutOfView(InputTargetType targetType)
		{
			return 0.0;
		}
	}
}
