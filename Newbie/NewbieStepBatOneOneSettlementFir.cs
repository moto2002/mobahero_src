using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepBatOneOneSettlementFir : NewbieStepBase
	{
		private string _voiceResId = "2044";

		private float _loopTime = 10f;

		public NewbieStepBatOneOneSettlementFir()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_SettlementFir;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			NewbieManager.Instance.StopDelayShowSettlementContinue();
			Singleton<NewbieView>.Instance.HideSettlementContinue();
			Singleton<NewbieView>.Instance.HideMask();
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.1f));
			NewbieManager.Instance.StartDelayShowSettlementContinue(0.7f);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
