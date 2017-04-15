using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleHallActMagicBottleAwd : NewbieStepBase
	{
		private string _voiceResId = "2313";

		public NewbieStepEleHallActMagicBottleAwd()
		{
			this._stepType = ENewbieStepType.EleHallAct_MagicBottleAwd;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideMask();
		}

		public override void HandleAction()
		{
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			NewbieSubtitleData inSubtitleData;
			inSubtitleData.validNum = 1;
			inSubtitleData.processLength = 9;
			inSubtitleData.firSubtitleId = "Guidance_Letter_D13";
			inSubtitleData.firSubtitleTimeLen = 0.5f;
			inSubtitleData.firProcessIdx = 5;
			inSubtitleData.secSubtitleId = string.Empty;
			inSubtitleData.secSubtitleTimeLen = 1f;
			inSubtitleData.secProcessIdx = 0;
			NewbieManager.Instance.StartDisplaySubtitle(inSubtitleData);
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(5.5f);
		}
	}
}
