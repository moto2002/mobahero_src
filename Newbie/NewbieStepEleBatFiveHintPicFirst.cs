using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatFiveHintPicFirst : NewbieStepBase
	{
		private string _voiceResId = "2102";

		private float _loopTime = 20f;

		public NewbieStepEleBatFiveHintPicFirst()
		{
			this._stepType = ENewbieStepType.EleBatFive_HintPicFirst;
		}

		public override void OnLeave()
		{
		}

		public override void HandleAction()
		{
			CtrlManager.CloseWindow(WindowID.NewbieLoadView);
			NewbieManager.Instance.ShowEleBatFiveLineHint();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.027f, 0.051f, 0.175f, 0.9f));
			Singleton<NewbieView>.Instance.ShowEleBatFiveHintPicFirst();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
			base.AutoMoveNextStepWithDelay(10f);
		}
	}
}
