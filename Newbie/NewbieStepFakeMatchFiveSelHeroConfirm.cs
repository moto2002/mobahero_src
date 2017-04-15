using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveSelHeroConfirm : NewbieStepBase
	{
		private string _voiceResId = "2208";

		private float _loopTime = 10f;

		public NewbieStepFakeMatchFiveSelHeroConfirm()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_SelHeroConfirm;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideFakeMatchFiveSelHeroConfirm();
		}

		public override void HandleAction()
		{
			Singleton<NewbieView>.Instance.ShowFakeMatchFiveSelHeroConfirm();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
