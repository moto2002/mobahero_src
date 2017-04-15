using System;

namespace MobaFrame.SkillAction
{
	public class ReplaceMaterialSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return false;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			if (this.data.param2 == 0f)
			{
				this.targetUnit.mCharacterEffect.UseGrow();
			}
			this.targetUnit.SetLockCharaEffect(true);
			base.PlayEffects(this.targetUnit);
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.SetLockCharaEffect(false);
			this.targetUnit.RevertShader();
		}
	}
}
