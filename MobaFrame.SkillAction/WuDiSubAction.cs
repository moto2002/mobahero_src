using System;

namespace MobaFrame.SkillAction
{
	public class WuDiSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return false;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.WuDi.Add();
			base.PlayEffects(this.targetUnit);
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.WuDi.Remove();
		}
	}
}
