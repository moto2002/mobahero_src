using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepBatOneOneLearnSkillFourth : NewbieStepBase
	{
		private string _voiceResId = "2038";

		private float _loopTime = 10f;

		public NewbieStepBatOneOneLearnSkillFourth()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_LearnSkillFourth;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideLearnSkillHint(3);
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowLearnSkillHint(3);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
