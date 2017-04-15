using System;
using System.Collections.Generic;

namespace MobaFrame.SkillAction
{
	public class RenWuAction : BaseSkillAction
	{
		private new Skill_Tunvlang_01 skill;

		protected override bool doAction()
		{
			this.skill = (base.unit.getSkillById(this.skillData.skillId) as Skill_Tunvlang_01);
			if (this.skill == null)
			{
				return false;
			}
			base.DoRatate(true, 0f);
			string[] array = this.skillData.start_actions;
			if (base.unit.StartActions != null)
			{
				array = base.unit.StartActions;
			}
			if (array != null)
			{
				int curConjureIndex = this.skill.GetCurConjureIndex();
				if (this.skill.IsCountOut())
				{
					this.skill.RevertTotalCount();
				}
				else
				{
					this.skill.RemoveCount();
				}
				if (StringUtils.CheckValid(array[curConjureIndex]))
				{
					PerformAction performAction = ActionManager.PlayPerform(this.skillKey, array[curConjureIndex], base.unit, this.targetUnits, this.targetPosition, true, null);
					performAction.OnDamageCallback = new Callback<BaseAction, List<Units>>(this.OnDamage);
					performAction.OnDamageEndCallback = new Callback<BaseAction>(this.OnDamageEnd);
					this.AddAction(performAction);
				}
			}
			base.unit.StartActions = null;
			return true;
		}

		protected override void OnSkillDamage(BaseSkillAction action, List<Units> targets)
		{
			this.AddAction(ActionManager.HitSkill(action.skillKey, base.unit, targets, true));
			SkillData data = GameManager.Instance.SkillData.GetData(this.skillKey);
			string[] highEffects = data.GetHighEffects(SkillPhrase.Hit);
			if (highEffects != null && this.skill.GetPreConjureIndex() < highEffects.Length)
			{
				string text = highEffects[this.skill.GetPreConjureIndex()];
				if (StringUtils.CheckValid(text))
				{
					for (int i = 0; i < targets.Count; i++)
					{
						if (targets[i] != null && targets[i].isLive && !SkillUtility.IsImmunityHighEff(targets[i], text))
						{
							ActionManager.AddHighEffect(text, this.skill.skillMainId, targets[i], base.unit, this.targetPosition, true);
						}
					}
				}
			}
			string[] buffs = data.GetBuffs(SkillPhrase.Hit);
			if (buffs != null && this.skill.GetPreConjureIndex() < buffs.Length)
			{
				string text2 = buffs[this.skill.GetPreConjureIndex()];
				if (StringUtils.CheckValid(text2))
				{
					for (int j = 0; j < targets.Count; j++)
					{
						if (targets[j] != null && targets[j].isLive && !SkillUtility.IsImmunityBuff(targets[j], text2))
						{
							ActionManager.AddBuff(text2, targets[j], base.unit, true, string.Empty);
						}
					}
				}
			}
			base.OnSkillDamage(action, targets);
		}

		protected override void OnSkillEnd(BaseSkillAction action)
		{
			ActionManager.EndSkill(action.skillKey, base.unit, true);
			base.OnSkillEnd(action);
		}
	}
}
