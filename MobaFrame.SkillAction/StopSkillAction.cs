using Com.Game.Module;
using System;

namespace MobaFrame.SkillAction
{
	public class StopSkillAction : BaseAction
	{
		public SkillDataKey skillKey;

		public SkillInterruptType interruptType;

		public SkillData skillData;

		protected override void OnInit()
		{
			base.OnInit();
			this.skillData = GameManager.Instance.SkillData.GetData(this.skillKey);
		}

		protected override void OnRecordStart()
		{
		}

		protected override void OnSendStart()
		{
		}

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
				this.TryRemoveBornPowerObjSkillDataOnInterrupt(this.skillKey.SkillID);
				skillOrAttackById.Interrupt(true);
				if (base.unit.isPlayer)
				{
					Singleton<SkillView>.Instance.ShowGuideBar(false, 1f, "回城");
				}
				return true;
			}
			this.TryRemoveBornPowerObjSkillDataOnInterrupt(this.skillKey.SkillID);
			skillOrAttackById.Interrupt(false);
			return false;
		}

		protected void TryRemoveBornPowerObjSkillDataOnInterrupt(string inSkillId)
		{
			if (base.unit != null && base.unit.skillManager != null)
			{
				base.unit.skillManager.TryRemoveBornPowerObjSkillDataOnInterrupt(inSkillId);
			}
		}
	}
}
