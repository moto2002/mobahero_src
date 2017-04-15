using System;

namespace MobaFrame.SkillAction
{
	public class PetrifactionSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.ShiHua.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.ShiHua.Add();
			this.targetUnit.ShowPetrifaction();
			this.targetUnit.SetFrozenAnimation(true);
			this.targetUnit.SetLockCharaEffect(true);
			base.EnableAction(this.targetUnit, false, this.data.param1);
			this.targetUnit.ShowDebuffIcon(true, 119);
			this.targetUnit.InterruptAction(SkillInterruptType.Passive);
			base.PlayEffects(this.targetUnit);
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.RevertShader();
			this.targetUnit.SetFrozenAnimation(false);
			this.targetUnit.SetLockCharaEffect(false);
			base.EnableAction(this.targetUnit, true, this.data.param1);
			this.targetUnit.ShiHua.Remove();
			this.targetUnit.ShowDebuffIcon(false, 119);
		}
	}
}
