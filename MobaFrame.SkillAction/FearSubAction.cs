using System;

namespace MobaFrame.SkillAction
{
	public class FearSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.KongJu.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.KongJu.Add();
			this.targetUnit.SetLockCharaControl(true);
			this.targetUnit.ShowDebuffIcon(true, 128);
			this.targetUnit.InterruptAction(SkillInterruptType.Passive);
			base.PlayEffects(this.targetUnit);
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.SetLockCharaControl(false);
			this.targetUnit.DingShen.Remove();
			this.targetUnit.ShowDebuffIcon(false, 128);
		}
	}
}
