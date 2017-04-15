using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;

namespace MobaFrame.SkillAction
{
	public class EndSkillAction : BaseSkillAction
	{
		protected override void OnSendStart()
		{
			if (this.skillData.end_actions != null)
			{
				PvpEvent.SendEndSkillEvent(new EndSkillInfo
				{
					unitId = base.unit.unique_id,
					skillId = this.skillKey.SkillID
				});
			}
		}

		protected override bool doAction()
		{
			if (this.skill != null)
			{
				this.skill.CastPhase = SkillCastPhase.Cast_End;
			}
			if (this.skillData == null)
			{
				return false;
			}
			if (this.skillData.end_actions != null)
			{
				for (int i = 0; i < this.skillData.end_actions.Length; i++)
				{
					if (StringUtils.CheckValid(this.skillData.end_actions[i]))
					{
						PerformAction action = ActionManager.PlayPerform(this.skillKey, this.skillData.end_actions[i], base.unit, null, null, true, null);
						this.AddAction(action);
					}
				}
				this.AddActionToSkill(SkillCastPhase.Cast_End, this);
				return true;
			}
			return false;
		}
	}
}
