using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneUseSkill : NewbieStepBase
	{
		private string _voiceResId = "2020";

		private float _loopTime = 10f;

		private string _mainText = "点击图标释放技能";

		public NewbieStepEleBatOneOneUseSkill()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_UseSkillThird;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideTitle();
			Singleton<NewbieView>.Instance.HideUseSkillHint(2);
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.01f));
			Singleton<NewbieView>.Instance.ShowUseSkillHint(2);
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
