using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveFirNearGrass : NewbieStepBase
	{
		private string _mainText = "通过插眼获得视野";

		private string _voiceResId = "2113";

		private float _loopTime = 10f;

		public NewbieStepEleBatFiveFirNearGrass()
		{
			this._stepType = ENewbieStepType.EleBatFive_FirNearGrass;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EnableEleBatFiveTriggerCheck();
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideEleBatFiveUseEye();
			Singleton<NewbieView>.Instance.HideTitle();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableEleBatFiveTriggerCheck();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText);
			Singleton<NewbieView>.Instance.ShowEleBatFiveUseEye();
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
		}
	}
}
