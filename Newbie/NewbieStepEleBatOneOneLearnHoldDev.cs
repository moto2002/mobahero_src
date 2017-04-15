using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneLearnHoldDev : NewbieStepBase
	{
		private string _voiceHintResId = "2004";

		public NewbieStepEleBatOneOneLearnHoldDev()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_LearnHoldDevice;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintOnce();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.ShowHudView();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.75f));
			Singleton<NewbieView>.Instance.ShowGestureAnimation(true);
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceHintResId);
			base.AutoMoveNextStepWithDelay(7.5f);
		}
	}
}
