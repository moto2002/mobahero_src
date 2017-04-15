using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneLearnSkillOne : NewbieStepBase
	{
		private string _voiceResId = "2022";

		private float _loopTime = 10f;

		public NewbieStepEleBatOneOneLearnSkillOne()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_LearnSkillOne;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideLearnSkillHint(0);
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowLearnSkillHint(0);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
