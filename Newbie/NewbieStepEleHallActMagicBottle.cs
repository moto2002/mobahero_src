using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleHallActMagicBottle : NewbieStepBase
	{
		private string _voiceResId = "2308";

		private float _loopTime = 10f;

		public NewbieStepEleHallActMagicBottle()
		{
			this._stepType = ENewbieStepType.EleHallAct_MagicBottle;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideEleHallActOpenMagicBottle();
			Singleton<NewbieView>.Instance.HideMask();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.NotifyServerNewbieGuidePhase(ENewbiePhaseType.End);
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowEleHallActOpenMagicBottle();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
