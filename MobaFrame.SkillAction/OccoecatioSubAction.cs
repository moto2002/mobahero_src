using System;

namespace MobaFrame.SkillAction
{
	public class OccoecatioSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.ZhiMang.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.ZhiMang.Add();
			base.PlayEffects(this.targetUnit);
			this.targetUnit.ShowDebuffText(true, 131);
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.ZhiMang.Remove();
			this.targetUnit.ShowDebuffText(false, 131);
		}
	}
}
