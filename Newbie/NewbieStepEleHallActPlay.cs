using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleHallActPlay : NewbieStepBase
	{
		private string _voiceResId = "2324";

		private float _loopTime = 20f;

		public NewbieStepEleHallActPlay()
		{
			this._stepType = ENewbieStepType.EleHallAct_Play;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideEleHallActPlay();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowEleHallActPlay();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
