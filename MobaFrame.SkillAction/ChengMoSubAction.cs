using System;

namespace MobaFrame.SkillAction
{
	public class ChengMoSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.ChengMo.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.ChengMo.Add();
			base.PlayEffects(this.targetUnit);
			this.targetUnit.ShowDebuffIcon(true, 117);
			this.targetUnit.InterruptAction(SkillInterruptType.Passive);
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.ChengMo.Remove();
			this.targetUnit.ShowDebuffIcon(false, 117);
		}
	}
}
