using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveFirFreeCam : NewbieStepBase
	{
		private string _voiceResId = "2210";

		public NewbieStepFakeMatchFiveFirFreeCam()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_FirFreeCam;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EnableFakeMatchFiveTrigger();
			NewbieManager.Instance.StopFakeMatchFiveCheckFirFreeCamEnd();
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideFakeMatchFiveFreeCamHint();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableFakeMatchFiveTrigger();
			NewbieManager.Instance.StartFakeMatchFiveCheckFirFreeCamEnd();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowFakeMatchFiveFreeCamHint();
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
		}
	}
}
