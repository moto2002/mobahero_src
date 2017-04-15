using System;

namespace MobaFrame.SkillAction
{
	public class SimpleReadySkillAction : BaseSkillAction
	{
		protected override bool doAction()
		{
			string[] array = this.skillData.ready_actions;
			if (base.unit.ReadyActions != null)
			{
				array = base.unit.ReadyActions;
			}
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (StringUtils.CheckValid(array[i]))
					{
						PerformAction action = ActionManager.PlayPerform(this.skillKey, array[i], base.unit, this.targetUnits, this.targetPosition, true, null);
						this.AddAction(action);
					}
				}
			}
			base.unit.ReadyActions = null;
			if (base.unit.isBuilding && base.unit is Tower && this.targetUnits != null && this.targetUnits.Count > 0)
			{
				(base.unit as Tower).SetCurAttackTarget(this.targetUnits[0]);
			}
			return true;
		}
	}
}
