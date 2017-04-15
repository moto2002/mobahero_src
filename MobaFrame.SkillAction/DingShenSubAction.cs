using System;

namespace MobaFrame.SkillAction
{
	public class DingShenSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.DingShen.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.DingShen.Add();
			this.targetUnit.SetCanMove(false);
			this.targetUnit.InterruptAction(SkillInterruptType.Passive);
			base.PlayEffects(this.targetUnit);
			this.targetUnit.ShowDebuffIcon(true, 118);
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.DingShen.Remove();
			this.targetUnit.ShowDebuffIcon(false, 118);
			this.targetUnit.SetCanMove(true);
		}
	}
}
