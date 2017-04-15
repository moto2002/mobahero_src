using System;
using System.Collections.Generic;

namespace MobaFrame.SkillAction
{
	public class MultiSkillAction : BaseSkillAction
	{
		protected override bool doAction()
		{
			int skill_index = this.skillData.config.skill_index;
			List<SkillDataKey> skillsByIndex = base.unit.getSkillsByIndex(skill_index);
			if (skillsByIndex == null || skillsByIndex.Count < 3)
			{
				return false;
			}
			if (TeamManager.CanAttack(base.unit, this.targetUnits[0]))
			{
				StartSkillAction startSkillAction = ActionManager.StartSkill(skillsByIndex[1], base.unit, this.targetUnits, this.targetPosition, true, null);
				startSkillAction.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
				startSkillAction.OnSkillEndCallback = new Callback<BaseSkillAction>(this.OnSkillEnd);
				this.AddAction(startSkillAction);
			}
			else
			{
				StartSkillAction startSkillAction2 = ActionManager.StartSkill(skillsByIndex[2], base.unit, this.targetUnits, this.targetPosition, true, null);
				startSkillAction2.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
				startSkillAction2.OnSkillEndCallback = new Callback<BaseSkillAction>(this.OnSkillEnd);
				this.AddAction(startSkillAction2);
			}
			return true;
		}
	}
}
