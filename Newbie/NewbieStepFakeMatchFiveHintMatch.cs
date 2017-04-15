using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveHintMatch : NewbieStepBase
	{
		private string _voiceResId = "2204";

		private float _loopTime = 10f;

		public NewbieStepFakeMatchFiveHintMatch()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_HintMatch;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
		}

		public override void HandleAction()
		{
			if (Singleton<PvpWaitView>.Instance != null)
			{
				Singleton<PvpWaitView>.Instance.NewbieFakeMatchFiveShowMatch();
			}
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
