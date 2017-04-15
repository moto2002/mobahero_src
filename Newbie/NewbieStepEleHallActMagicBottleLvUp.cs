using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleHallActMagicBottleLvUp : NewbieStepBase
	{
		private string _voiceResId = "2310";

		private float _loopTime = 10f;

		public NewbieStepEleHallActMagicBottleLvUp()
		{
			this._stepType = ENewbieStepType.EleHallAct_MagicBottleLvUp;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideEleHallActMagicLvUp();
			Singleton<NewbieView>.Instance.HideMask();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.NotifyServerNewbieGuidePhase(ENewbiePhaseType.End);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowEleHallActMagicLvUp();
			NewbieSubtitleData inSubtitleData;
			inSubtitleData.validNum = 1;
			inSubtitleData.processLength = 9;
			inSubtitleData.firSubtitleId = "Guidance_Letter_D10";
			inSubtitleData.firSubtitleTimeLen = 0.5f;
			inSubtitleData.firProcessIdx = 2;
			inSubtitleData.secSubtitleId = string.Empty;
			inSubtitleData.secSubtitleTimeLen = 1f;
			inSubtitleData.secProcessIdx = 0;
			NewbieManager.Instance.StartDisplaySubtitle(inSubtitleData);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
