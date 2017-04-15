using System;

namespace MobaHeros.AI
{
	internal class EffData
	{
		private bool _inMeiHuo;

		private bool _inTianBenDiLie;

		public EffData()
		{
			this._inMeiHuo = false;
			this._inTianBenDiLie = false;
		}

		public void SetState(SpecialSkillEff eff, bool isInstate)
		{
			if (eff != SpecialSkillEff.Skill_Meihuo)
			{
				if (eff == SpecialSkillEff.Skill_TianBenDiLie)
				{
					this._inTianBenDiLie = isInstate;
				}
			}
			else
			{
				this._inMeiHuo = isInstate;
			}
		}

		public bool IsInState(SpecialSkillEff eff)
		{
			if (eff != SpecialSkillEff.Skill_Meihuo)
			{
				return eff == SpecialSkillEff.Skill_TianBenDiLie && this._inTianBenDiLie;
			}
			return this._inMeiHuo;
		}
	}
}
