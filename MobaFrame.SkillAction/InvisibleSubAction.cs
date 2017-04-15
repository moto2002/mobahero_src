using System;

namespace MobaFrame.SkillAction
{
	public class InvisibleSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return false;
			}
		}

		protected override void StartHighEff()
		{
			base.StartHighEff();
			TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitAttack, null, new TriggerAction(this.RemoveSelf), this.targetUnit.unique_id);
			TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitConjure, null, new TriggerAction(this.RemoveSelf), this.targetUnit.unique_id);
		}

		private void RemoveSelf()
		{
			ActionManager.RemoveHighEffect(this.higheffId, this.targetUnit, true);
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.SetLockCharaEffect(true);
			this.targetUnit.Invisible.Add();
			this.targetUnit.ChangeLayer("Invisible");
			if (this.targetUnit.isPlayer)
			{
				this.targetUnit.skillManager.UpdateSkillUI();
			}
		}

		protected override void RevertState()
		{
			base.RevertState();
			base.StopActions();
			this.targetUnit.RevertLayer();
			this.targetUnit.SetLockCharaEffect(false);
			this.targetUnit.Invisible.Remove();
			if (this.targetUnit.isPlayer)
			{
				this.targetUnit.skillManager.UpdateSkillUI();
			}
		}
	}
}
