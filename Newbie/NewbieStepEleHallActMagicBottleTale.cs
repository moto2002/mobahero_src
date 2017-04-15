using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleHallActMagicBottleTale : NewbieStepBase
	{
		private string _voiceResId = "2312";

		private float _loopTime = 10f;

		public NewbieStepEleHallActMagicBottleTale()
		{
			this._stepType = ENewbieStepType.EleHallAct_MagicBottleTale;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideEleHallActMagicTale();
			Singleton<NewbieView>.Instance.HideMask();
		}

		public override void HandleAction()
		{
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowEleHallActMagicTale();
			NewbieSubtitleData inSubtitleData;
			inSubtitleData.validNum = 1;
			inSubtitleData.processLength = 9;
			inSubtitleData.firSubtitleId = "Guidance_Letter_D12";
			inSubtitleData.firSubtitleTimeLen = 0.5f;
			inSubtitleData.firProcessIdx = 4;
			inSubtitleData.secSubtitleId = string.Empty;
			inSubtitleData.secSubtitleTimeLen = 1f;
			inSubtitleData.secProcessIdx = 0;
			NewbieManager.Instance.StartDisplaySubtitle(inSubtitleData);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
