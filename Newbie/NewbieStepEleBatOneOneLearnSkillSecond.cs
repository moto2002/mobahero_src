using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneLearnSkillSecond : NewbieStepBase
	{
		private string _voiceResId = "2029";

		private float _loopTime = 10f;

		public NewbieStepEleBatOneOneLearnSkillSecond()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_LearnSkillSecond;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideLearnSkillHint(1);
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowLearnSkillHint(1);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
