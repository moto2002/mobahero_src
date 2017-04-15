using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleHallActMagicBottleLvThree : NewbieStepBase
	{
		private string _voiceResId = "2311";

		private float _loopTime = 10f;

		public NewbieStepEleHallActMagicBottleLvThree()
		{
			this._stepType = ENewbieStepType.EleHallAct_MagicBottleLvThree;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EleHallActStopCheckMagicLvThree();
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideEleHallActMagicLvThree();
			Singleton<NewbieView>.Instance.HideMask();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.EleHallActStartCheckMagicLvThree();
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowEleHallActMagicLvThree();
			NewbieSubtitleData inSubtitleData;
			inSubtitleData.validNum = 1;
			inSubtitleData.processLength = 9;
			inSubtitleData.firSubtitleId = "Guidance_Letter_D11";
			inSubtitleData.firSubtitleTimeLen = 0.5f;
			inSubtitleData.firProcessIdx = 3;
			inSubtitleData.secSubtitleId = string.Empty;
			inSubtitleData.secSubtitleTimeLen = 1f;
			inSubtitleData.secProcessIdx = 0;
			NewbieManager.Instance.StartDisplaySubtitle(inSubtitleData);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
