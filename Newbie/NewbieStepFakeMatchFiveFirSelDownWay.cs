using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveFirSelDownWay : NewbieStepBase
	{
		private string _voiceResId = "2209";

		public NewbieStepFakeMatchFiveFirSelDownWay()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_FirSelDownWay;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EnableFakeMatchFiveTrigger();
			NewbieManager.Instance.StopFakeMatchFiveCheckFirSelDownWayEnd();
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideFakeMatchFiveSettingDownWay();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableFakeMatchFiveTrigger();
			NewbieManager.Instance.StartFakeMatchFiveCheckFirSelDownWayEnd();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowFakeMatchFiveSettingDownWay();
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
		}
	}
}
