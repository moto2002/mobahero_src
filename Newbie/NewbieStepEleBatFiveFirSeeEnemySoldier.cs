using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveFirSeeEnemySoldier : NewbieStepBase
	{
		private string _mainText = "购买更多强力装备！";

		private string _subText = "击杀小兵、英雄、野怪，都可以获得金币";

		private string _voiceResId = "2106";

		private float _loopTime = 10f;

		public NewbieStepEleBatFiveFirSeeEnemySoldier()
		{
			this._stepType = ENewbieStepType.EleBatFive_FirSeeEnemySoldier;
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
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText, this._subText);
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
		}
	}
}
