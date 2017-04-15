using System;

namespace MobaFrame.SkillAction
{
	public class YunXuanSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.YunXuan.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.YunXuan.Add();
			this.targetUnit.PlayAnim(AnimationType.HitStun, true, 0, true, true);
			this.targetUnit.SetCanRotate(false);
			base.EnableAction(this.targetUnit, false, this.data.param1);
			this.targetUnit.InterruptAction(SkillInterruptType.Passive);
			this.targetUnit.ShowDebuffIcon(true, 116);
			base.PlayEffects(this.targetUnit);
		}

		protected override void RevertState()
		{
			base.RevertState();
			base.StopActions();
			this.targetUnit.SetCanRotate(true);
			base.EnableAction(this.targetUnit, true, this.data.param1);
			this.targetUnit.YunXuan.Remove();
			this.targetUnit.PlayAnim(AnimationType.HitStun, false, 0, true, true);
			this.targetUnit.ShowDebuffIcon(false, 116);
		}
	}
}
