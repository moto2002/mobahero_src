using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleHallActMagicClassicIntro : NewbieStepBase
	{
		private string _voiceResId = "2315";

		public NewbieStepEleHallActMagicClassicIntro()
		{
			this._stepType = ENewbieStepType.EleHallAct_MagicClassicIntro;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideMask();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.NotifyServerNewbieGuidePhase(ENewbiePhaseType.End);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			NewbieSubtitleData inSubtitleData;
			inSubtitleData.validNum = 2;
			inSubtitleData.processLength = 9;
			inSubtitleData.firSubtitleId = "Guidance_Letter_D15a";
			inSubtitleData.firSubtitleTimeLen = 4.5f;
			inSubtitleData.firProcessIdx = 7;
			inSubtitleData.secSubtitleId = "Guidance_Letter_D15b";
			inSubtitleData.secSubtitleTimeLen = 0.5f;
			inSubtitleData.secProcessIdx = 8;
			NewbieManager.Instance.StartDisplaySubtitle(inSubtitleData);
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(11f);
		}
	}
}
