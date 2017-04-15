using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MobaHeros.AI
{
	public class HeroMirrorTargetingSystem : MobaTargetingSystem
	{
		private Units m_FollowTarget;

		private float followRange = 10f;

		public Units FollowTarget
		{
			get;
			private set;
		}

		public HeroMirrorTargetingSystem(Units owner, SensoryMemory memory) : base(owner, memory)
		{
		}

		public override void InitTargets()
		{
			base.InitTargets();
		}

		public override void ClearTargets()
		{
			base.ClearTargets();
		}

		public void SetFollowTarget(Units target)
		{
			this.m_FollowTarget = target;
			base.SetInputTarget(InputTargetType.MoveTarget, this.m_FollowTarget);
		}

		public override bool SelectMapTarget()
		{
			base.SetInputTarget(InputTargetType.MoveTarget, this.m_FollowTarget);
			return true;
		}

		protected override bool IfNeedChangeTarget()
		{
			this.m_timer.interval = 1f;
			if (this.m_timer.CanUpdate())
			{
				return true;
			}
			object outputTarget = base.GetOutputTarget(OutputTargetType.OutputAttackTarget);
			if (outputTarget == null)
			{
				return true;
			}
			Units target = outputTarget as Units;
			bool flag = HeroMirrorTargetingSystem.IfChooseTarget(this.m_Owner, target, 0f);
			return !flag;
		}

		protected override Units GetNextTarget()
		{
			this.SetMoveTarget();
			Units priorityTarget = this.GetPriorityTarget();
			if (priorityTarget == null)
			{
				return this.GetNormalTarget();
			}
			return priorityTarget;
		}

		private void SetMoveTarget()
		{
			GoalTarget inputTarget = base.GetInputTarget(InputTargetType.MoveTarget);
			if (inputTarget == null || inputTarget.m_Unit == null)
			{
				base.SetInputTarget(InputTargetType.MoveTarget, this.m_FollowTarget);
			}
		}

		private Units GetPriorityTarget()
		{
			List<Units> list = new List<Units>();
			Units attackTarget = this.m_FollowTarget.GetAttackTarget();
			if (attackTarget != null)
			{
				list.Add(attackTarget);
			}
			foreach (Units current in this.m_FollowTarget.aiManager.GetRecentAttacker(3f))
			{
				if (current != null && current.isLive)
				{
					list.Add(current);
				}
			}
			if (list.Count > 0)
			{
				foreach (Units current2 in list)
				{
					if (current2.isHero)
					{
						return current2;
					}
				}
				return list[0];
			}
			return null;
		}

		private Units GetNormalTarget()
		{
			Units units = null;
			List<Units> list = MapManager.Instance.EnumEnemyMapUnits(this.m_Owner.TeamType, TargetTag.HeroAndMonster).ToList<Units>();
			if (list.Count > 0)
			{
				list.Sort(new Comparison<Units>(this.SortFunc));
				foreach (Units current in list)
				{
					if (HeroMirrorTargetingSystem.IfChooseTarget(this.m_Owner, current, 0f))
					{
						if (current.isHero)
						{
							return current;
						}
						if (units == null)
						{
							units = current;
						}
					}
				}
				return units;
			}
			return null;
		}

		private int SortFunc(Units enemy1, Units enemy2)
		{
			if (!(enemy1 != null) || !(enemy2 != null) || !enemy1.isLive || !enemy2.isLive)
			{
				return 0;
			}
			Units home = MapManager.Instance.GetHome((TeamType)this.m_Owner.teamType);
			if (home == null)
			{
				return 0;
			}
			float num = UnitFeature.DistanceToTargetSqr(home.mTransform, enemy1.mTransform);
			float num2 = UnitFeature.DistanceToTargetSqr(home.mTransform, enemy2.mTransform);
			if (num < num2)
			{
				return -1;
			}
			if (num == num2)
			{
				return 0;
			}
			return 1;
		}

		public static bool IfChooseTarget(Units owner, Units target, float range = 0f)
		{
			if (target == null || !target.isLive || !TeamManager.CanAttack(owner, target) || !target.CanBeSelected || target.isItem || (target.isMonster && target.teamType == 2) || !owner.CanBeSelected)
			{
				return false;
			}
			if (UnitFeature.DistanceToTarget(owner.transform, target.transform) > owner.warning_range)
			{
				return false;
			}
			Units home = MapManager.Instance.GetHome((TeamType)target.teamType);
			if (home == null)
			{
				return true;
			}
			if (owner.MeiHuo.IsInState)
			{
				return true;
			}
			if (range != 0f && !HeroMirrorTargetingSystem.IsInrange(owner.mTransform.position, target.mTransform.position, range))
			{
				return false;
			}
			UtilCounter counter = UtilManager.Instance.GetCounter(UtilType.Tower);
			if (counter != null)
			{
				TowerCounter towerCounter = counter as TowerCounter;
				Units towerOfLowestPriority = towerCounter.GetTowerOfLowestPriority(owner);
				if (towerOfLowestPriority != null)
				{
					float num = UnitFeature.DistanceToTargetSqr(owner.transform, towerOfLowestPriority.transform);
					float num2 = UnitFeature.DistanceToTargetSqr(owner.transform, target.transform);
					if (num2 > num)
					{
						return false;
					}
				}
			}
			bool flag = StrategyManager.Instance.IsHomeRecovery((TeamType)target.teamType);
			if (flag)
			{
				Vector3 recoveryPos = StrategyManager.Instance.GetRecoveryPos((TeamType)target.teamType);
				if (HeroMirrorTargetingSystem.IsInrange(target.transform.position, recoveryPos, 3f))
				{
					return false;
				}
			}
			Units nearestEnemyTowerOfTarget = TargetSelectHelper.GetNearestEnemyTowerOfTarget(target);
			if (nearestEnemyTowerOfTarget != null)
			{
				float attackRange = nearestEnemyTowerOfTarget.GetAttackRange(owner);
				if (HeroMirrorTargetingSystem.IsInrange(target.transform.position, nearestEnemyTowerOfTarget.transform.position, attackRange))
				{
					float num3 = UnitFeature.DistanceToTarget(target, nearestEnemyTowerOfTarget);
					if (owner.GetAttackRange(target) + num3 > attackRange && !HeroMirrorTargetingSystem.IsInrange(owner.transform.position, nearestEnemyTowerOfTarget.transform.position, attackRange))
					{
						return true;
					}
					Units attackTarget = nearestEnemyTowerOfTarget.GetAttackTarget();
					if (attackTarget != null && attackTarget == owner)
					{
						return false;
					}
					List<Units> allians = TargetSelectHelper.GetAllians(nearestEnemyTowerOfTarget, attackRange, TargetTag.Monster, false);
					return allians != null && allians.Count > 1 && !target.isHero;
				}
			}
			return true;
		}

		public static bool IsInrange(Vector3 pos1, Vector3 pos2, float range)
		{
			float sqrMagnitude = (pos1 - pos2).sqrMagnitude;
			float num = range * range;
			return sqrMagnitude <= num;
		}
	}
}
