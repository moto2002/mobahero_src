using Com.Game.Module;
using MobaHeros;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepBatOneOneSettlementThd : NewbieStepBase
	{
		public NewbieStepBatOneOneSettlementThd()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_SettlementThd;
		}

		public override void OnLeave()
		{
			Singleton<NewbieView>.Instance.HideSettlementContinue();
			Singleton<NewbieView>.Instance.HideMask();
		}

		public override void HandleAction()
		{
			if (GlobalSettings.Instance.autoTestSetting.testLevel != AutoTestTag.None)
			{
				NewbieManager.Instance.NotifyServerNewbieGuidePhase(ENewbiePhaseType.End);
			}
			else
			{
				NewbieManager.Instance.NotifyServerNewbieGuidePhase(ENewbiePhaseType.ElementaryBattleOneOne);
			}
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.1f));
			Singleton<NewbieView>.Instance.ShowSettlementContinue();
		}
	}
}
