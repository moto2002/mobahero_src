using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneLearnSkill : NewbieStepBase
	{
		private string _voiceResId = "2018";

		private float _loopTime = 10f;

		private string _mainText = "技能加点与升级";

		public NewbieStepEleBatOneOneLearnSkill()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_LearnSkillThird;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideTitle();
			Singleton<NewbieView>.Instance.HideLearnSkillHint(2);
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowLearnSkillHint(2);
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
