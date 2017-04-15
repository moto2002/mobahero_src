using Com.Game.Module;
using System;

namespace MobaFrame.SkillAction
{
	public class MorphSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.BianShen.IsInState;
			}
		}

		protected override void SetState()
		{
			bool isHideEffect = this.data.param2 == 1f;
			bool isHideHUDText = this.data.param3 == 1f;
			base.SetState();
			this.targetUnit.SetLockCharaEffect(true);
			this.targetUnit.BianShen.Add();
			base.EnableAction(this.targetUnit, false, this.data.param1);
			this.targetUnit.EnableRender(false, isHideEffect, isHideHUDText);
			this.targetUnit.InterruptAction(SkillInterruptType.Passive);
			base.PlayEffects(this.targetUnit);
			if (StringUtils.CheckValid(this.data.strParam1))
			{
				this.AddAction(ActionManager.PlayEffect(this.data.strParam1, this.targetUnit, null, null, true, string.Empty, base.unit));
			}
			if (this.targetUnit.isPlayer)
			{
				Singleton<SkillView>.Instance.SetForbidMask(true, string.Empty);
				this.targetUnit.skillManager.UpdateSkillUI();
			}
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.SetLockCharaEffect(false);
			this.targetUnit.EnableRender(true, true, true);
			this.targetUnit.BianShen.Remove();
			base.EnableAction(this.targetUnit, true, this.data.param1);
			if (this.targetUnit.isPlayer)
			{
				Singleton<SkillView>.Instance.SetForbidMask(false, string.Empty);
				this.targetUnit.skillManager.UpdateSkillUI();
			}
		}
	}
}
