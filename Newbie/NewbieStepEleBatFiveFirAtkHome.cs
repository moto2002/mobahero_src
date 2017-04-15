using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveFirAtkHome : NewbieStepBase
	{
		private string _mainText = "胜利在望！";

		private string _voiceResId = "2114";

		public NewbieStepEleBatFiveFirAtkHome()
		{
			this._stepType = ENewbieStepType.EleBatFive_FirAtkHome;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EnableEleBatFiveTriggerCheck();
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideTitle();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableEleBatFiveTriggerCheck();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText);
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(4.5f);
		}
	}
}
