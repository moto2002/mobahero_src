using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveHintSelMapFive : NewbieStepBase
	{
		private string _voiceResId = "2203";

		private float _loopTime = 10f;

		public NewbieStepFakeMatchFiveHintSelMapFive()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_HintSelMapFive;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideFakeMatchFiveSelMapFive();
		}

		public override void HandleAction()
		{
			Singleton<NewbieView>.Instance.ShowFakeMatchFiveSelMapFive();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
