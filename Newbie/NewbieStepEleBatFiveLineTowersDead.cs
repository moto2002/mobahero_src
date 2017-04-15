using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveLineTowersDead : NewbieStepBase
	{
		private string _mainText = "胜利就在眼前！";

		private string _voiceResId = "2112";

		public NewbieStepEleBatFiveLineTowersDead()
		{
			this._stepType = ENewbieStepType.EleBatFive_LineTowersDead;
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
			base.AutoMoveNextStepWithDelay(10f);
		}
	}
}
