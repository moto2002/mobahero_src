using System;

namespace MobaHeros.AI
{
	public class MobaTargetingSystem : TargetingSystem
	{
		private VTrigger buffSpawnTrigger;

		private VTrigger buffDespawnTrigger;

		protected MyAiTimer m_timer;

		public MobaTargetingSystem(Units owner, SensoryMemory memory) : base(owner, memory)
		{
			this.m_timer = new MyAiTimer();
		}

		public override void UpdateTargetState()
		{
			for (int i = 0; i < this.m_Goals.Length; i++)
			{
				GoalTarget goalTarget = this.m_Goals[i];
				if (!base.CheckInputTarget((InputTargetType)i, goalTarget))
				{
					if (goalTarget != null && goalTarget.m_Unit != null)
					{
						goalTarget.m_Unit.MarkAsTarget(false);
					}
					base.SetInputTarget((InputTargetType)i, null);
				}
			}
		}

		public override void UpdateAttackTarget()
		{
			if (this.SelectOutTarget())
			{
				return;
			}
			if (this.SelectVisiableTarget())
			{
				return;
			}
			if (this.SelectMapTarget())
			{
				return;
			}
		}

		public override bool SelectOutTarget()
		{
			return false;
		}

		public override bool SelectVisiableTarget()
		{
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
			Units targetsOfPorityAndDistance = this.m_SensoryMemory.GetTargetsOfPorityAndDistance();
			if (targetsOfPorityAndDistance != null && TeamManager.CanAttack(this.m_Owner, targetsOfPorityAndDistance))
			{
				base.SetInputTarget(InputTargetType.FixedTarget, targetsOfPorityAndDistance);
				return true;
			}
			return false;
		}

		protected virtual bool IfNeedChangeTarget()
		{
			return true;
		}

		protected virtual Units GetNextTarget()
		{
			return null;
		}

		public override void InitTargets()
		{
			base.InitTargets();
			this.m_Owner.AddSearchingCallback(new Callback(this.OnSearchPath), new Callback(this.OnStopPath));
			this.buffSpawnTrigger = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSpawn, null, new TriggerAction(this.SpawnBuffTarget), this.m_Owner.teamType, "BuffItem");
			this.buffDespawnTrigger = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDespawn, null, new TriggerAction(this.DespawnBuffTarget), this.m_Owner.teamType, "BuffItem");
		}

		public override void ClearTargets()
		{
			this.m_Owner.RemoveSearchingCallback(new Callback(this.OnSearchPath), new Callback(this.OnStopPath));
			if (this.buffSpawnTrigger != null)
			{
				TriggerManager.DestroyTrigger(this.buffSpawnTrigger);
			}
			if (this.buffDespawnTrigger != null)
			{
				TriggerManager.DestroyTrigger(this.buffDespawnTrigger);
			}
			base.ClearTargets();
		}

		public void OnSearchPath()
		{
		}

		public void OnStopPath()
		{
		}

		private void SpawnBuffTarget()
		{
			Units triggerUnit = TriggerManager.GetTriggerUnit();
			base.SetInputTarget(InputTargetType.BuffTarget, triggerUnit);
			StrategyManager.Instance.UpdateBuffHero((TeamType)this.m_Owner.teamType);
		}

		private void DespawnBuffTarget()
		{
			Units triggerUnit = TriggerManager.GetTriggerUnit();
			GoalTarget inputTarget = base.GetInputTarget(InputTargetType.BuffTarget);
			if (inputTarget != null && inputTarget.m_Unit != null && triggerUnit.unique_id == inputTarget.m_Unit.unique_id)
			{
				base.SetInputTarget(InputTargetType.BuffTarget, null);
			}
		}
	}
}
