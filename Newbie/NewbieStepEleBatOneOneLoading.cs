using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatOneOneLoading : NewbieStepBase
	{
		private string _voiceResId = "2002";

		public NewbieStepEleBatOneOneLoading()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_Loading;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopCheckSpecialEnterBattleSuc();
			NewbieManager.Instance.StopVoiceHintOnce();
			CtrlManager.CloseWindow(WindowID.SignView);
			CtrlManager.CloseWindow(WindowID.ActivityView);
			CtrlManager.CloseWindow(WindowID.NewbieLoadView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.StartCheckSpecialEnterBattleSuc(15f, ENewbieStepType.ElementaryBatOneOne_SelectMap);
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
		}
	}
}
