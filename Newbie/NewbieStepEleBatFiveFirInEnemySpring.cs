using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveFirInEnemySpring : NewbieStepBase
	{
		private string _mainText = "敌方泉水区也有攻击力";

		private string _voiceResId = "2115";

		public NewbieStepEleBatFiveFirInEnemySpring()
		{
			this._stepType = ENewbieStepType.EleBatFive_FirInEnemySpring;
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
			base.AutoMoveNextStepWithDelay(5.5f);
		}
	}
}
