using System;

namespace MobaFrame.SkillAction
{
	public class WeiyiAction : MultiTargetHighEffAction<WeiyiSubAction>
	{
		protected override bool doAction()
		{
			if (base.doAction())
			{
				base.AddActionToSkill(this);
				return true;
			}
			return false;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.RemoveActionFromSkill(this);
		}
	}
}
