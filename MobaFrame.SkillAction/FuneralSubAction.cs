using System;

namespace MobaFrame.SkillAction
{
	public class FuneralSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.BoZang.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.BoZang.Add();
			base.PlayEffects(this.targetUnit);
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.BoZang.Remove();
		}
	}
}
