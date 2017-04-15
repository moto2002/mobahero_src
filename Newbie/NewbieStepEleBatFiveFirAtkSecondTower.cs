using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveFirAtkSecondTower : NewbieStepBase
	{
		private string _voiceResId = "2116";

		private float _loopTime = 10f;

		public NewbieStepEleBatFiveFirAtkSecondTower()
		{
			this._stepType = ENewbieStepType.EleBatFive_FirAtkSecondTower;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EnableEleBatFiveTriggerCheck();
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideEleBatFiveAttackHint();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableEleBatFiveTriggerCheck();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowEleBatFiveAttackHint();
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
		}
	}
}
