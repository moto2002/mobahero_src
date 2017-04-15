using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveFirHpLessThirty : NewbieStepBase
	{
		private string _mainText = "如何回城";

		private string _subText = "回城可购买装备，同时回复生命值和法力值";

		private string _voiceResId = "2111";

		public NewbieStepEleBatFiveFirHpLessThirty()
		{
			this._stepType = ENewbieStepType.EleBatFive_FirHpLessThirty;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EnableEleBatFiveTriggerCheck();
			NewbieManager.Instance.StopEleBatFiveDelayHpLessThirtyEnd();
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideEleBatFiveUseBackHome();
			Singleton<NewbieView>.Instance.HideTitle();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableEleBatFiveTriggerCheck();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText, this._subText);
			Singleton<NewbieView>.Instance.ShowEleBatFiveUseBackHome();
			NewbieManager.Instance.StartEleBatFiveDelayHpLessThirtyEnd(10f);
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
		}
	}
}
