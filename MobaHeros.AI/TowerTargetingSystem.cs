using System;
using System.Collections.Generic;
using System.Linq;

namespace MobaHeros.AI
{
	public class TowerTargetingSystem : MobaTargetingSystem
	{
		public TowerTargetingSystem(Units owner, SensoryMemory memory) : base(owner, memory)
		{
		}

		public override bool SelectMapTarget()
		{
			Units targetsOfPorityAndDistance = this.GetTargetsOfPorityAndDistance();
			if (targetsOfPorityAndDistance != null && this.IfChoose(targetsOfPorityAndDistance))
			{
				base.SetInputTarget(InputTargetType.AttackTarget, targetsOfPorityAndDistance);
				return true;
			}
			base.SetInputTarget(InputTargetType.AttackTarget, null);
			return false;
		}

		private Units GetTargetsOfPorityAndDistance()
		{
			List<Units> list = null;
			list = MapManager.Instance.EnumEnemyMapUnits(this.m_Owner.TeamType, TargetTag.All).ToList<Units>();
			if (list.Count > 0)
			{
				FindTargetHelper.SortTargets(this.m_Owner, SortType.Distance, ref list);
				foreach (Units current in list)
				{
					if (this.IfChoose(current))
					{
						return current;
					}
				}
			}
			return null;
		}

		protected override Units GetNextTarget()
		{
			Units towerPriorityTarget = this.GetTowerPriorityTarget();
			if (towerPriorityTarget != null)
			{
				return towerPriorityTarget;
			}
			List<Units> list = this.m_SensoryMemory.GetListOfRecentlySensedOpponents(Relation.Hostility, TargetTag.All, true, true, SortType.Distance, FindType.None, null);
			list = FindTargetHelper.FilterTargets(this.m_Owner, list, FindType.Distance, this.m_Owner.GetAttackRange(null));
			if (list != null && list.Count > 0)
			{
				Units units = null;
				foreach (Units current in list)
				{
					if (this.IfChoose(current))
					{
						if (units == null)
						{
							units = current;
						}
						if (!current.isHero)
						{
							return current;
						}
					}
				}
				return units;
			}
			return null;
		}

		protected override bool IfNeedChangeTarget()
		{
			Units attackTarget = base.GetAttackTarget();
			if (attackTarget == null)
			{
				return true;
			}
			Units towerPriorityTarget = this.GetTowerPriorityTarget();
			if (towerPriorityTarget != null && towerPriorityTarget != attackTarget)
			{
				return true;
			}
			bool flag = this.IfChoose(attackTarget);
			return !flag;
		}

		private Units GetTowerPriorityTarget()
		{
			List<Units> list = new List<Units>();
			IList<Units> mapUnits = MapManager.Instance.GetMapUnits((TeamType)this.m_Owner.teamType, TargetTag.Hero);
			if (mapUnits == null)
			{
				return null;
			}
			List<Units> list2 = new List<Units>();
			foreach (Units current in mapUnits)
			{
				if (UnitFeature.TargetInDistance(this.m_Owner.transform, current.transform, this.m_Owner.GetAttackRange(current)))
				{
					list2.Add(current);
				}
			}
			if (mapUnits != null)
			{
				foreach (Units current2 in mapUnits)
				{
					AIManager aiManager = current2.aiManager;
					foreach (Units current3 in aiManager.GetRecentAttacker(2f))
					{
						if (current3 != null && current3.isLive && current3.isHero && this.IfChoose(current3))
						{
							list.Add(current3);
						}
					}
				}
			}
			List<Units> list3 = FindTargetHelper.FilterTargets(this.m_Owner, list, FindType.Distance, this.m_Owner.GetAttackRange(null));
			if (list3 != null && list3.Count > 0)
			{
				foreach (Units current4 in list3)
				{
					if (current4.isHero)
					{
						return current4;
					}
				}
				return list3[0];
			}
			return null;
		}

		private bool IfChoose(Units target)
		{
			return (!target.isMonster || target.teamType != 2) && (TeamManager.CanAttack(this.m_Owner, target) && target.CanBeSelected && !target.isItem && UnitFeature.DistanceToTarget(this.m_Owner.transform, target.transform) <= this.m_Owner.GetAttackRange(target));
		}
	}
}
