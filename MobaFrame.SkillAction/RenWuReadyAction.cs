using System;

namespace MobaFrame.SkillAction
{
	public class RenWuReadyAction : SimpleReadySkillAction
	{
		protected override bool doAction()
		{
			Skill_Tunvlang_01 skill_Tunvlang_ = base.unit.getSkillById(this.skillData.skillId) as Skill_Tunvlang_01;
			if (skill_Tunvlang_ == null)
			{
				return false;
			}
			base.DoRatate(true, 0f);
			string[] array = this.skillData.ready_actions;
			if (base.unit.ReadyActions != null)
			{
				array = base.unit.ReadyActions;
			}
			if (array != null)
			{
				int curConjureIndex = skill_Tunvlang_.GetCurConjureIndex();
				if (StringUtils.CheckValid(array[curConjureIndex]))
				{
					PerformAction action = ActionManager.PlayPerform(this.skillKey, array[curConjureIndex], base.unit, this.targetUnits, this.targetPosition, true, null);
					this.AddAction(action);
				}
			}
			base.unit.ReadyActions = null;
			return true;
		}
	}
}
