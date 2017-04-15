using System;

namespace MobaFrame.SkillAction
{
	public class PlayEffectPerformSubAction : BaseStateAction
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
			if (this.data.param2 > 0f)
			{
				base.EnableAction(this.targetUnit, false, this.data.param2);
			}
			if (this.performIds != null)
			{
				for (int i = 0; i < this.performIds.Length; i++)
				{
					this.AddAction(ActionManager.PlayAnim(this.performIds[i], this.targetUnit, true));
					this.AddAction(ActionManager.PlayEffect(this.performIds[i], this.targetUnit, null, null, true, string.Empty, null));
				}
			}
		}

		protected override void RevertState()
		{
			base.RevertState();
			if (this.data.param2 > 0f)
			{
				base.EnableAction(this.targetUnit, true, this.data.param2);
			}
		}
	}
}
