using System;

namespace MobaFrame.SkillAction
{
	public class AnAhaReadyAction : SimpleReadySkillAction
	{
		protected override bool doAction()
		{
			if (this.skillData == null)
			{
				return false;
			}
			if (this.skillData.ready_actions == null)
			{
				return false;
			}
			if (this.skillData.ready_actions.Length < 2)
			{
				return false;
			}
			if (this.targetUnits == null)
			{
				return false;
			}
			if (this.targetUnits.Count == 0)
			{
				return false;
			}
			PerformAction action = ActionManager.PlayPerform(this.skillKey, this.skillData.ready_actions[0], base.unit, null, null, true, null);
			this.AddAction(action);
			PerformAction action2 = ActionManager.PlayPerform(this.skillKey, this.skillData.ready_actions[1], this.targetUnits[0], null, null, true, null);
			this.AddAction(action2);
			return true;
		}
	}
}
