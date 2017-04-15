using System;

namespace MobaFrame.SkillAction
{
	public class FrozenSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.BingDong.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.BingDong.Add();
			this.targetUnit.SetCanRotate(false);
			this.targetUnit.SetFrozenAnimation(true);
			base.EnableAction(this.targetUnit, false, this.data.param1);
			this.targetUnit.ShowDebuffIcon(true, 124);
			this.targetUnit.InterruptAction(SkillInterruptType.Passive);
			base.PlayEffects(this.targetUnit);
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.SetFrozenAnimation(false);
			this.targetUnit.SetCanRotate(true);
			this.targetUnit.RevertShader();
			this.targetUnit.RevertAnimSpeed();
			base.EnableAction(this.targetUnit, true, this.data.param1);
			this.targetUnit.BingDong.Remove();
			this.targetUnit.ShowDebuffIcon(false, 124);
		}
	}
}
