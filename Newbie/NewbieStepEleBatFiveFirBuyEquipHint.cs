using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveFirBuyEquipHint : NewbieStepBase
	{
		private string _voiceResId = "2118";

		public NewbieStepEleBatFiveFirBuyEquipHint()
		{
			this._stepType = ENewbieStepType.EleBatFive_FirBuyEquipHint;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EnableEleBatFiveTriggerCheck();
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideEleBatFiveMiniMapShop();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableEleBatFiveTriggerCheck();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowEleBatFiveMiniMapShop();
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(10f);
		}
	}
}
