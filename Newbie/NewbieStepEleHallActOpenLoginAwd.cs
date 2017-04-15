using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleHallActOpenLoginAwd : NewbieStepBase
	{
		private string _voiceResId = "2322";

		private float _loopTime = 10f;

		public NewbieStepEleHallActOpenLoginAwd()
		{
			this._stepType = ENewbieStepType.EleHallAct_OpenLoginAwd;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideEleHallActOpenLoginAwd();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowEleHallActOpenLoginAwd();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
