using System;

namespace MobaFrame.SkillAction
{
	public class CharmSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.MeiHuo.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.MeiHuo.Add();
			if (this.targetUnit.mAIManager != null)
			{
				this.targetUnit.mAIManager.SpecialEffectBegin(SpecialSkillEff.Skill_Meihuo);
			}
			this.targetUnit.SetLockCharaControl(true);
			TeamType type = (this.targetUnit.teamType != 1) ? TeamType.BL : TeamType.LM;
			this.targetUnit.ChangeTeam(type);
			this.targetUnit.ShowDebuffIcon(true, 125);
			this.targetUnit.InterruptAction(SkillInterruptType.Passive);
			base.PlayEffects(this.targetUnit);
			this.targetUnit.UpdateHUDBar();
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.RevertTeam();
			base.StopActions();
			this.targetUnit.SetLockCharaControl(false);
			this.targetUnit.UpdateHUDBar();
			if (this.targetUnit.mAIManager != null)
			{
				this.targetUnit.mAIManager.SpecialEffectEnd(SpecialSkillEff.Skill_Meihuo);
			}
			this.targetUnit.MeiHuo.Remove();
			this.targetUnit.ShowDebuffIcon(false, 125);
		}

		protected override bool CheckCondition()
		{
			return this.targetUnit.teamType != -1 && this.targetUnit.teamType != 2;
		}
	}
}
