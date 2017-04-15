using System;

namespace MobaFrame.SkillAction
{
	public class ImprisonmentSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.Imprisonment.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.Imprisonment.Add();
			this.targetUnit.SetLockAnimState(true);
			this.targetUnit.SetLockCharaEffect(true);
			base.EnableAction(this.targetUnit, false, this.data.param1);
			this.targetUnit.InterruptAction(SkillInterruptType.Passive);
			base.PlayEffects(this.targetUnit);
			if (this.targetUnit.isPlayer)
			{
				this.targetUnit.skillManager.UpdateSkillUI();
			}
		}

		protected override void RevertState()
		{
			base.RevertState();
			base.StopActions();
			this.targetUnit.Imprisonment.Remove();
			if (!this.targetUnit.Imprisonment.IsInState)
			{
				this.targetUnit.SetLockAnimState(false);
				this.targetUnit.SetLockCharaEffect(false);
				base.EnableAction(this.targetUnit, true, this.data.param1);
			}
			if (this.targetUnit.isPlayer)
			{
				this.targetUnit.skillManager.UpdateSkillUI();
			}
		}
	}
}
