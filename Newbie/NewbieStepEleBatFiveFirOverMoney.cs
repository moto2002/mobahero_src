using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveFirOverMoney : NewbieStepBase
	{
		private string _mainText = "购买装备，增强实力！";

		private string _voiceResId = "2117";

		public NewbieStepEleBatFiveFirOverMoney()
		{
			this._stepType = ENewbieStepType.EleBatFive_FirOverMoney;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EnableEleBatFiveTriggerCheck();
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideEleBatFiveMiniMapShop();
			Singleton<NewbieView>.Instance.HideTitle();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableEleBatFiveTriggerCheck();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText);
			Singleton<NewbieView>.Instance.ShowEleBatFiveMiniMapShop();
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(10f);
		}
	}
}
