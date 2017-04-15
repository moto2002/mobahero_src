using MobaHeros;
using System;

namespace MobaFrame.SkillAction
{
	public class MoFaMianYiSubAction : BaseStateAction
	{
		private new EffectDataType data = default(EffectDataType);

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
			base.PlayEffects(this.targetUnit);
			this.data.MagicType = (EffectMagicType)this.data.param2;
			this.data.GainType = (EffectGainType)this.data.param3;
			this.data.ImmuneType = (EffectImmuneType)this.data.param4;
			this.targetUnit.ImmunityManager.AddImmunity(this.data);
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.ImmunityManager.RemoveImmunity(this.data);
		}
	}
}
