using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveSettlementThd : NewbieStepBase
	{
		private string _voiceResId = "2211";

		public NewbieStepFakeMatchFiveSettlementThd()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_SettlementThd;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideSettlementContinue();
			Singleton<NewbieView>.Instance.HideMask();
		}

		public override void HandleAction()
		{
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.1f));
			Singleton<NewbieView>.Instance.ShowSettlementContinue();
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
		}
	}
}
