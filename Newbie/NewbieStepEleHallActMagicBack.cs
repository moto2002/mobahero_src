using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleHallActMagicBack : NewbieStepBase
	{
		private string _voiceResId = "2316";

		private float _loopTime = 10f;

		public NewbieStepEleHallActMagicBack()
		{
			this._stepType = ENewbieStepType.EleHallAct_MagicBack;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideEleHallActMagicBack();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.EleHallActDestroyCheckMagicLvThree();
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowEleHallActMagicBack();
			NewbieSubtitleData inSubtitleData;
			inSubtitleData.validNum = 1;
			inSubtitleData.processLength = 9;
			inSubtitleData.firSubtitleId = "Guidance_Letter_D16";
			inSubtitleData.firSubtitleTimeLen = 0.5f;
			inSubtitleData.firProcessIdx = 9;
			inSubtitleData.secSubtitleId = string.Empty;
			inSubtitleData.secSubtitleTimeLen = 1f;
			inSubtitleData.secProcessIdx = 0;
			NewbieManager.Instance.StartDisplaySubtitle(inSubtitleData);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
