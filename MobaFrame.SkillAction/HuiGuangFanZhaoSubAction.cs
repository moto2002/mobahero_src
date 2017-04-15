using System;

namespace MobaFrame.SkillAction
{
	public class HuiGuangFanZhaoSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.HuiGuangFanZhao.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.HuiGuangFanZhao.Add();
			base.PlayEffects(this.targetUnit);
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.HuiGuangFanZhao.Remove();
		}
	}
}
