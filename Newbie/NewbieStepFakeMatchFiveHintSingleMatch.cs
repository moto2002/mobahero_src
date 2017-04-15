using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveHintSingleMatch : NewbieStepBase
	{
		private string _voiceResId = "2202";

		private float _loopTime = 10f;

		public NewbieStepFakeMatchFiveHintSingleMatch()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_HintSingleMatch;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideFakeMatchFiveHintSingleMatch();
		}

		public override void HandleAction()
		{
			Singleton<NewbieView>.Instance.ShowFakeMatchFiveHintSingleMatch();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
