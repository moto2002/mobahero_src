using System;
using System.Collections.Generic;

namespace MobaHeros.AI
{
	public class CreepTargetingSystem : MobaTargetingSystem
	{
		public CreepTargetingSystem(Units owner, SensoryMemory memory) : base(owner, memory)
		{
		}

		public override void InitTargets()
		{
			base.InitTargets();
			this.SetCreepCheckPoint();
		}

		protected override Units GetNextTarget()
		{
			return this.GetCreepFirstTarget();
		}

		protected override bool IfNeedChangeTarget()
		{
			Units attackTarget = base.GetAttackTarget();
			bool flag = this.IfChoose(attackTarget);
			return !flag;
		}

		public override bool SelectMapTarget()
		{
			return false;
		}

		private bool IfChoose(Units target)
		{
			return !(target == null) && !target.isMonster && !target.isBuilding && target.CanBeSelected;
		}

		private Units GetCreepFirstTarget()
		{
			List<Units> list = new List<Units>();
			foreach (Units current in this.m_Owner.aiManager.GetRecentAttacker(2f))
			{
				if (current != null && current.isLive && this.IfChoose(current))
				{
					list.Add(current);
				}
			}
			List<Units> list2 = FindTargetHelper.FilterTargets(this.m_Owner, list, FindType.Distance, this.m_Owner.warning_range);
			if (list2 != null && list2.Count > 0)
			{
				return list2[0];
			}
			return null;
		}

		private void SetCreepCheckPoint()
		{
		}
	}
}
