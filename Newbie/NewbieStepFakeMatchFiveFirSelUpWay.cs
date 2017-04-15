using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveFirSelUpWay : NewbieStepBase
	{
		private string _voiceResId = "2209";

		public NewbieStepFakeMatchFiveFirSelUpWay()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_FirSelUpWay;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EnableFakeMatchFiveTrigger();
			NewbieManager.Instance.StopFakeMatchFiveCheckFirSelUpWayEnd();
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideFakeMatchFiveSettingUpWay();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableFakeMatchFiveTrigger();
			NewbieManager.Instance.StartFakeMatchFiveCheckFirSelUpWayEnd();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowFakeMatchFiveSettingUpWay();
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
		}
	}
}
