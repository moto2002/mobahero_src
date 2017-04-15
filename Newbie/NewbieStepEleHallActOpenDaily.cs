using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleHallActOpenDaily : NewbieStepBase
	{
		private string _voiceResId = "2304";

		private float _loopTime = 10f;

		public NewbieStepEleHallActOpenDaily()
		{
			this._stepType = ENewbieStepType.EleHallAct_OpenDaily;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideEleHallActOpenDaily();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.NotifyServerNewbieGuidePhase(ENewbiePhaseType.End);
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowEleHallActOpenDaily();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
