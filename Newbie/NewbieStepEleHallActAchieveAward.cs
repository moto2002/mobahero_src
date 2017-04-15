using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleHallActAchieveAward : NewbieStepBase
	{
		private string _voiceResId = "2302";

		private float _loopTime = 10f;

		public NewbieStepEleHallActAchieveAward()
		{
			this._stepType = ENewbieStepType.EleHallAct_AchieveAward;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideEleHallActAchieveAwd();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.NotifyServerNewbieGuidePhase(ENewbiePhaseType.End);
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowEleHallActAchieveAwd();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
