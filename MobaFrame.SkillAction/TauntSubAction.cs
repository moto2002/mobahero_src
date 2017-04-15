using System;

namespace MobaFrame.SkillAction
{
	public class TauntSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.ChaoFeng.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.ChaoFeng.Add();
			this.targetUnit.SetLockCharaControl(true);
			this.targetUnit.SetTauntTarget(base.unit);
			this.targetUnit.ShowDebuffIcon(true, 126);
			this.targetUnit.InterruptAction(SkillInterruptType.Passive);
			base.PlayEffects(this.targetUnit);
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.SetLockCharaControl(false);
			this.targetUnit.SetTauntTarget(null);
			this.targetUnit.ChaoFeng.Remove();
			this.targetUnit.ShowDebuffIcon(false, 126);
		}
	}
}
