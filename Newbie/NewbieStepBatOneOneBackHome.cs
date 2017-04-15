using Com.Game.Module;
using MobaHeros;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepBatOneOneBackHome : NewbieStepBase
	{
		public NewbieStepBatOneOneBackHome()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_BackHome;
		}

		public override void OnLeave()
		{
			Singleton<NewbieView>.Instance.HideSettlementBackHome();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
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
			Singleton<NewbieView>.Instance.ShowSettlementBackHome();
		}
	}
}
