using System;
using System.Collections.Generic;

namespace MobaFrame.SkillAction
{
	public class SimpleSkillAction : BaseSkillAction
	{
		protected override bool doAction()
		{
			if (this.skillData == null)
			{
				return false;
			}
			string[] array = this.skillData.start_actions;
			if (base.unit.StartActions != null)
			{
				array = base.unit.StartActions;
			}
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (StringUtils.CheckValid(array[i]))
					{
						PerformAction performAction = ActionManager.PlayPerform(this.skillKey, array[i], base.unit, this.targetUnits, this.targetPosition, true, null);
						performAction.OnDamageCallback = new Callback<BaseAction, List<Units>>(this.OnDamage);
						performAction.OnDamageEndCallback = new Callback<BaseAction>(this.OnDamageEnd);
						this.AddAction(performAction);
					}
				}
			}
			base.unit.StartActions = null;
			return true;
		}

		protected override void OnSkillDamage(BaseSkillAction action, List<Units> targets)
		{
			this.AddAction(ActionManager.HitSkill(action.skillKey, base.unit, targets, true));
			base.AddHighEff(action.skillKey, SkillPhrase.Hit, targets, this.targetPosition);
			base.AddBuff(action.skillKey, SkillPhrase.Hit, targets);
			base.OnSkillDamage(action, targets);
		}

		protected override void OnSkillEnd(BaseSkillAction action)
		{
			ActionManager.EndSkill(action.skillKey, base.unit, true);
			base.OnSkillEnd(action);
		}
	}
}
