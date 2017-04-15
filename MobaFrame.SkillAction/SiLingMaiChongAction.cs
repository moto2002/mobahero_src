using System;
using System.Collections.Generic;

namespace MobaFrame.SkillAction
{
	public class SiLingMaiChongAction : BaseSkillAction
	{
		protected override bool doAction()
		{
			int skill_index = this.skillData.config.skill_index;
			List<SkillDataKey> skillsByIndex = base.unit.getSkillsByIndex(skill_index);
			if (skillsByIndex == null || skillsByIndex.Count < 3)
			{
				return false;
			}
			if (this.targetUnits == null)
			{
				return false;
			}
			for (int i = 0; i < this.targetUnits.Count; i++)
			{
				List<Units> list = new List<Units>();
				list.Add(this.targetUnits[i]);
				if (TeamManager.CanAttack(base.unit, list[0]))
				{
					StartSkillAction startSkillAction = ActionManager.StartSkill(skillsByIndex[1], base.unit, list, this.targetPosition, true, null);
					startSkillAction.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
					startSkillAction.OnSkillEndCallback = new Callback<BaseSkillAction>(this.OnSkillEnd);
					this.AddAction(startSkillAction);
				}
				else
				{
					StartSkillAction startSkillAction2 = ActionManager.StartSkill(skillsByIndex[2], base.unit, list, this.targetPosition, true, null);
					startSkillAction2.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
					startSkillAction2.OnSkillEndCallback = new Callback<BaseSkillAction>(this.OnSkillEnd);
					this.AddAction(startSkillAction2);
				}
			}
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
