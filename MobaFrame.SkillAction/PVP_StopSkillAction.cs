using Com.Game.Module;
using System;

namespace MobaFrame.SkillAction
{
	public class PVP_StopSkillAction : StopSkillAction
	{
		public SkillCastPhase skillCastPhase;

		protected override bool doAction()
		{
			Skill skillOrAttackById = base.unit.getSkillOrAttackById(this.skillKey.SkillID);
			if (skillOrAttackById == null)
			{
				return false;
			}
			if (skillOrAttackById.IsGuide || this.interruptType == SkillInterruptType.Force || SkillUtility.IsBackHomeSkill(skillOrAttackById))
			{
				if (SkillUtility.IsBackHomeSkill(skillOrAttackById) && skillOrAttackById.IsInSkillCastIn)
				{
					base.unit.jumpFont(JumpFontType.Interrupt, "回城打断!", null, false);
				}
				skillOrAttackById.Interrupt_PVP(SkillCastPhase.Cast_None);
				if (base.unit.isPlayer)
				{
					Singleton<SkillView>.Instance.ShowGuideBar(false, 1f, "回城");
				}
				return true;
			}
			skillOrAttackById.Interrupt_PVP(this.skillCastPhase);
			return false;
		}
	}
}
