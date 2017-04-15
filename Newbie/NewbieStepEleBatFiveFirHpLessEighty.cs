using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveFirHpLessEighty : NewbieStepBase
	{
		private string _mainText = "使用血瓶";

		private string _subText = "可给英雄同时回复生命值和法力值";

		private string _voiceResId = "2110";

		private float _loopTime = 10f;

		public NewbieStepEleBatFiveFirHpLessEighty()
		{
			this._stepType = ENewbieStepType.EleBatFive_FirHpLessEighty;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideEleBatFiveUseRecoveryHint();
			Singleton<NewbieView>.Instance.HideTitle();
			CtrlManager.CloseWindow(WindowID.NewbieView);
			NewbieManager.Instance.EnableEleBatFiveTriggerCheck();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableEleBatFiveTriggerCheck();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText, this._subText);
			Singleton<NewbieView>.Instance.ShowEleBatFiveUseRecoveryHint();
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
		}
	}
}
