using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveHintSelHero : NewbieStepBase
	{
		private string _voiceResId = "2205";

		private float _loopTime = 10f;

		public NewbieStepFakeMatchFiveHintSelHero()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_HintSelHero;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideFakeMatchFiveHintSelHero();
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.HideMask();
			Singleton<NewbieView>.Instance.ShowFakeMatchFiveHintSelHero();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
