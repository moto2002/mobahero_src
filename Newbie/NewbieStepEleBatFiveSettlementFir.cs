using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatFiveSettlementFir : NewbieStepBase
	{
		private string _voiceResId = "2120";

		public NewbieStepEleBatFiveSettlementFir()
		{
			this._stepType = ENewbieStepType.EleBatFive_SettlementFir;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintOnce();
			NewbieManager.Instance.StopDelayShowSettlementContinue();
			Singleton<NewbieView>.Instance.HideSettlementContinue();
			Singleton<NewbieView>.Instance.HideMask();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.NotifyServerNewbieGuidePhase(ENewbiePhaseType.ElementaryBattleFiveFive);
			NewbieManager.Instance.EleBatFiveStopTriggerChecker();
			NewbieManager.Instance.DestroyEleBatFiveDelayHpLessThirtyEnd();
			NewbieManager.Instance.ClearEleBatFiveLineResources();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.1f));
			NewbieManager.Instance.StartDelayShowSettlementContinue(0.7f);
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
		}
	}
}
