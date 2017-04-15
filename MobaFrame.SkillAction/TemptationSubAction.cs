using System;

namespace MobaFrame.SkillAction
{
	public class TemptationSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.Temptation.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.Temptation.Add();
			if (this.targetUnit.mAIManager != null)
			{
				this.targetUnit.mAIManager.SetTauntTarget(base.unit);
			}
			this.targetUnit.SetCanAttack(false);
			this.targetUnit.SetCanSkill(false);
			this.targetUnit.InterruptAction(SkillInterruptType.Passive);
			base.PlayEffects(this.targetUnit);
			this.targetUnit.ShowDebuffIcon(true, 125);
		}

		protected override void RevertState()
		{
			base.RevertState();
			base.StopActions();
			if (this.targetUnit.mAIManager != null)
			{
				this.targetUnit.mAIManager.SetTauntTarget(null);
			}
			this.targetUnit.Temptation.Remove();
			this.targetUnit.SetCanAttack(true);
			this.targetUnit.SetCanSkill(true);
			this.targetUnit.ShowDebuffIcon(false, 125);
		}
	}
}
