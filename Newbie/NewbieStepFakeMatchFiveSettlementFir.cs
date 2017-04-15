using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveSettlementFir : NewbieStepBase
	{
		public NewbieStepFakeMatchFiveSettlementFir()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_SettlementFir;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopDelayShowSettlementContinue();
			Singleton<NewbieView>.Instance.HideSettlementContinue();
			Singleton<NewbieView>.Instance.HideMask();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.NotifyServerNewbieGuidePhase(ENewbiePhaseType.FakeMatchFive);
			NewbieManager.Instance.FakeMatchFiveStopCheckTrigger();
			NewbieManager.Instance.FakeMatchFiveDestroyDelayStepEnd();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.1f));
			NewbieManager.Instance.StartDelayShowSettlementContinue(0.7f);
		}
	}
}
