using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace MobaHeros.AI
{
	public class HeroTargetingSystem : MobaTargetingSystem
	{
		private Dictionary<int, List<Units>> _targetsMap = new Dictionary<int, List<Units>>();

		private int length;

		private EffData effData;

		private Task delayClear;

		public HeroTargetingSystem(Units owner, SensoryMemory memory) : base(owner, memory)
		{
		}

		public override void InitTargets()
		{
			base.InitTargets();
			this.InitTargetsMap();
			this.effData = new EffData();
		}

		public override void ClearTargets()
		{
			base.ClearTargets();
			this.ClearTargetsMap();
			this.delayClear = null;
		}

		private void InitTargetsMap()
		{
			this.length = Enum.GetNames(Type.GetType("MobaHeros.AI.TargetPriority")).Length;
			for (int i = 1; i <= this.length; i++)
			{
				this._targetsMap.Add(i, new List<Units>());
			}
		}

		private void ClearTargetsMap()
		{
			for (int i = 1; i <= this.length; i++)
			{
				this._targetsMap.Clear();
			}
		}

		public override void OnSpecialSKillEffBegin(SpecialSkillEff eff)
		{
			this.effData.SetState(eff, true);
		}

		public override void OnSpecialSkillEffEnd(SpecialSkillEff eff)
		{
			this.effData.SetState(eff, false);
		}

		public override bool SelectVisiableTarget()
		{
			bool flag = !StrategyManager.Instance.IsAuto() && this.m_Owner.isPlayer;
			if (flag)
			{
				List<Units> list = FindTargetHelper.findTargets(this.m_Owner, this.m_Owner.trans.position, SkillTargetCamp.AttackTarget, TargetTag.HeroAndMonster, (this.m_Owner.atk_type != 1) ? (this.m_Owner.attack_range + 2f) : 6.5f);
				FindTargetHelper.SortTargets(this.m_Owner, GlobalSettings.Instance.AttackSortType, ref list);
				if (list != null && list.Count > 0)
				{
					GoalTarget inputTarget = base.GetInputTarget(InputTargetType.AttackTarget);
					if (inputTarget == null || (inputTarget != null && inputTarget.m_Unit.isBuilding))
					{
						base.SetInputTarget(InputTargetType.AttackTarget, list[0]);
						base.SetSelectTarget(list[0]);
						return true;
					}
				}
				return false;
			}
			if (!this.IfNeedChangeTarget())
			{
				return true;
			}
			Units nextTarget = this.GetNextTarget();
			if (nextTarget == null)
			{
				base.SetInputTarget(InputTargetType.AttackTarget, null);
				return false;
			}
			base.SetInputTarget(InputTargetType.AttackTarget, nextTarget);
			return true;
		}

		public override bool SelectMapTarget()
		{
			bool flag = !StrategyManager.Instance.IsAuto() && this.m_Owner.isPlayer;
			if (flag)
			{
				return true;
			}
			Units recentAttackTower = TargetSelectHelper.GetRecentAttackTower(this.m_Owner);
			if (recentAttackTower != null)
			{
				base.SetInputTarget(InputTargetType.AttackTarget, recentAttackTower);
				return true;
			}
			Units units = null;
			UtilCounter counter = UtilManager.Instance.GetCounter(UtilType.Tower);
			if (counter != null)
			{
				TowerCounter towerCounter = counter as TowerCounter;
				units = towerCounter.GetTowerOfLowestPriority(this.m_Owner);
			}
			if (units != null)
			{
				base.SetInputTarget(InputTargetType.AttackTarget, units);
				return true;
			}
			return false;
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
			bool flag = HeroTargetingSystem.IfChooseTarget(this.m_Owner, target, false, 0f);
			return !flag;
		}

		private void SetKillTarget()
		{
			List<Units> list = new List<Units>();
			GoalTarget inputTarget = base.GetInputTarget(InputTargetType.KillTarget);
			if (inputTarget != null && inputTarget.m_Unit != null && HeroTargetingSystem.IfChooseTarget(this.m_Owner, inputTarget.m_Unit, true, 0f) && inputTarget.m_Unit.isHero)
			{
				return;
			}
			List<Units> allians = TargetSelectHelper.GetAllians(this.m_Owner, this.m_Owner.warning_range + 3f, TargetTag.HeroAndMonster, false);
			foreach (Units current in allians)
			{
				if (current.hp / current.hp_max <= 0.3f)
				{
					list.Add(current);
				}
			}
			foreach (Units current2 in list)
			{
				if (current2.isLive && HeroTargetingSystem.IfChooseTarget(this.m_Owner, current2, true, 0f))
				{
					base.SetInputTarget(InputTargetType.KillTarget, current2);
					return;
				}
			}
			base.SetInputTarget(InputTargetType.KillTarget, null);
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

		protected override Units GetNextTarget()
		{
			this.SetKillTarget();
			Units priorityTarget = this.GetPriorityTarget();
			if (priorityTarget == null)
			{
				return this.GetNormalTarget();
			}
			return priorityTarget;
		}

		private Units GetPriorityTarget()
		{
			IEnumerable<Units> recentAttacker = this.m_Owner.aiManager.GetRecentAttacker(3f);
			foreach (Units current in recentAttacker)
			{
				if (HeroTargetingSystem.IfChooseTarget(this.m_Owner, current, false, 0f) && current.isHero)
				{
					return current;
				}
			}
			return null;
		}

		private Units GetNormalTarget()
		{
			List<Units> list = MapManager.Instance.EnumEnemyMapUnits(this.m_Owner.TeamType, TargetTag.HeroAndMonster).ToList<Units>();
			if (list.Count > 0)
			{
				list.Sort(new Comparison<Units>(this.SortFunc));
				foreach (Units current in list)
				{
					if (HeroTargetingSystem.IfChooseTarget(this.m_Owner, current, false, 0f))
					{
						return current;
					}
				}
			}
			return null;
		}

		public static bool IfChooseTarget(Units owner, Units target, bool isKillTarget = false, float range = 0f)
		{
			if (target == null || !target.isLive || !TeamManager.CanAttack(owner, target) || !target.CanBeSelected || target.isBuilding || target.isItem || (target.isMonster && target.teamType == 2) || !owner.CanBeSelected)
			{
				return false;
			}
			if (!isKillTarget && target.isHero && UnitFeature.DistanceToTarget(owner.transform, target.transform) > owner.warning_range)
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
			if (range != 0f && !HeroTargetingSystem.IsInrange(owner.mTransform.position, target.mTransform.position, range))
			{
				return false;
			}
			bool arg_10E_0 = !StrategyManager.Instance.IsAuto() && owner.isPlayer;
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
				if (HeroTargetingSystem.IsInrange(target.transform.position, recoveryPos, 3f))
				{
					return false;
				}
			}
			Units nearestEnemyTowerOfTarget = TargetSelectHelper.GetNearestEnemyTowerOfTarget(target);
			if (nearestEnemyTowerOfTarget != null)
			{
				float attackRange = nearestEnemyTowerOfTarget.GetAttackRange(owner);
				if (HeroTargetingSystem.IsInrange(target.transform.position, nearestEnemyTowerOfTarget.transform.position, attackRange))
				{
					float num3 = UnitFeature.DistanceToTarget(target, nearestEnemyTowerOfTarget);
					if (owner.GetAttackRange(target) + num3 > attackRange && !HeroTargetingSystem.IsInrange(owner.transform.position, nearestEnemyTowerOfTarget.transform.position, attackRange))
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

		public static bool InAnyOfTowersRange(Vector3 self, List<Units> towers)
		{
			if (towers == null)
			{
				return false;
			}
			foreach (Units current in towers)
			{
				if (HeroTargetingSystem.IsInrange(self, current.mTransform.position, current.attack_range + 1f))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsInrange(Vector3 pos1, Vector3 pos2, float range)
		{
			float sqrMagnitude = (pos1 - pos2).sqrMagnitude;
			float num = range * range;
			return sqrMagnitude <= num;
		}

		public override void DefendTower(Units attacker, Units tower)
		{
			GoalTarget inputTarget = base.GetInputTarget(InputTargetType.GuardTarget);
			if (inputTarget == null || inputTarget.m_Unit == null || !inputTarget.m_Unit.isLive)
			{
				base.SetInputTarget(InputTargetType.GuardTarget, attacker);
				if (this.delayClear == null)
				{
					this.delayClear = new Task(this.DelayClearGuardTarget(attacker, tower), true);
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayClearGuardTarget(Units attacker, Units tower)
		{
			HeroTargetingSystem.<DelayClearGuardTarget>c__Iterator28 <DelayClearGuardTarget>c__Iterator = new HeroTargetingSystem.<DelayClearGuardTarget>c__Iterator28();
			<DelayClearGuardTarget>c__Iterator.attacker = attacker;
			<DelayClearGuardTarget>c__Iterator.tower = tower;
			<DelayClearGuardTarget>c__Iterator.<$>attacker = attacker;
			<DelayClearGuardTarget>c__Iterator.<$>tower = tower;
			<DelayClearGuardTarget>c__Iterator.<>f__this = this;
			return <DelayClearGuardTarget>c__Iterator;
		}
	}
}
