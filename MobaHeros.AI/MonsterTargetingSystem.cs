using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MobaHeros.AI
{
	public class MonsterTargetingSystem : MobaTargetingSystem
	{
		private Dictionary<Units, float> tarDic = new Dictionary<Units, float>();

		private float interval = 3f;

		public MonsterTargetingSystem(Units owner, SensoryMemory memory) : base(owner, memory)
		{
			this.tarDic.Clear();
		}

		protected override Units GetNextTarget()
		{
			Units monsterFirstTarget = this.GetMonsterFirstTarget();
			if (monsterFirstTarget != null)
			{
				return monsterFirstTarget;
			}
			List<Units> listOfRecentlySensedOpponents = this.m_SensoryMemory.GetListOfRecentlySensedOpponents(Relation.Hostility, TargetTag.All, true, true, SortType.Distance, FindType.None, null);
			if (listOfRecentlySensedOpponents != null && listOfRecentlySensedOpponents.Count > 0)
			{
				Units units = null;
				foreach (Units current in listOfRecentlySensedOpponents)
				{
					if (this.IfChoose(current))
					{
						if (units == null)
						{
							units = current;
						}
						return current;
					}
				}
				return units;
			}
			return null;
		}

		protected override bool IfNeedChangeTarget()
		{
			Units attackTarget = base.GetAttackTarget();
			if (attackTarget == null || attackTarget.isHero || attackTarget.isBuilding)
			{
				return true;
			}
			Units monsterFirstTarget = this.GetMonsterFirstTarget();
			if (monsterFirstTarget != null && monsterFirstTarget != attackTarget)
			{
				return true;
			}
			bool flag = this.IfChoose(attackTarget);
			return !flag;
		}

		public bool SelectGuideTarget()
		{
			Monster monster = this.m_Owner as Monster;
			if (monster != null && monster.FromNeutralMonster)
			{
				return false;
			}
			Transform nearestGuidePoint = TargetSelectHelper.GetNearestGuidePoint(this.m_Owner);
			return nearestGuidePoint != null;
		}

		public override bool SelectMapTarget()
		{
			if (this.SelectGuideTarget())
			{
				return false;
			}
			Units targetsOfPorityAndDistance = this.GetTargetsOfPorityAndDistance();
			if (targetsOfPorityAndDistance != null && this.IfChoose(targetsOfPorityAndDistance))
			{
				base.SetInputTarget(InputTargetType.AttackTarget, targetsOfPorityAndDistance);
				return true;
			}
			Units nearestEnemyTowerOfSelf = TargetSelectHelper.GetNearestEnemyTowerOfSelf(this.m_Owner);
			if (nearestEnemyTowerOfSelf != null)
			{
				base.SetInputTarget(InputTargetType.AttackTarget, nearestEnemyTowerOfSelf);
				return true;
			}
			base.SetInputTarget(InputTargetType.AttackTarget, null);
			return false;
		}

		private Units GetMonsterFirstTarget()
		{
			List<Units> list = new List<Units>();
			IList<Units> mapUnits = MapManager.Instance.GetMapUnits((TeamType)this.m_Owner.teamType, TargetTag.Hero);
			if (mapUnits != null)
			{
				foreach (Units current in mapUnits)
				{
					AIManager aiManager = current.aiManager;
					foreach (Units current2 in aiManager.GetRecentAttacker(3f))
					{
						if (current2 != null && current2.isLive && current2.isHero && this.IfChoose(current2))
						{
							list.Add(current2);
						}
					}
				}
			}
			if (list != null && list.Count > 0)
			{
				return list[0];
			}
			return null;
		}

		private Units GetTargetsOfPorityAndDistance()
		{
			List<Units> list = MapManager.Instance.EnumEnemyMapUnits(this.m_Owner.TeamType, TargetTag.All).ToList<Units>();
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

		private bool IfChoose(Units target)
		{
			return !(target == null) && (!target.isMonster || target.teamType != 2) && (target.isBuilding || UnitFeature.DistanceToTarget(this.m_Owner.transform, target.transform) <= this.m_Owner.warning_range) && (TeamManager.CanAttack(this.m_Owner, target) && target.CanBeSelected && target.isLive && !target.isItem);
		}

		private bool NeutralMonsterTarget()
		{
			Monster monster = this.m_Owner as Monster;
			if (monster == null)
			{
				return false;
			}
			if (!monster.FromNeutralMonster)
			{
				return false;
			}
			Units attackTarget = monster.GetAttackTarget();
			if (attackTarget == null || !attackTarget.isLive)
			{
				return false;
			}
			if (!this.tarDic.ContainsKey(attackTarget))
			{
				this.tarDic.Add(attackTarget, Time.time);
				return true;
			}
			float num = this.tarDic[attackTarget];
			if (Time.time - num >= this.interval)
			{
				this.tarDic[attackTarget] = Time.time;
				return false;
			}
			return true;
		}
	}
}
