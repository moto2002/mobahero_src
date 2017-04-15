using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleHallActDoDailyHint : NewbieStepBase
	{
		private string _voiceResId = "2306";

		public NewbieStepEleHallActDoDailyHint()
		{
			this._stepType = ENewbieStepType.EleHallAct_DoDailyHint;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(8.5f);
		}
	}
}
