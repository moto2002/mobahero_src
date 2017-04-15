using Assets.Scripts.Model;
using System;

namespace Newbie
{
	public class NewbieStepNormCastCheckNormalCast : NewbieStepBase
	{
		public NewbieStepNormCastCheckNormalCast()
		{
			this._stepType = ENewbieStepType.NormCast_CheckNormalCast;
		}

		public override void OnLeave()
		{
		}

		public override void HandleAction()
		{
			if (ModelManager.Instance.Get_SettingData().crazyCastingSkill)
			{
				NewbieManager.Instance.NormCastStartCheckLearnSkillFir();
			}
		}
	}
}
