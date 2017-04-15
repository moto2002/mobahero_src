using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatOneOneUseSkillFirst : NewbieStepBase
	{
		private string _voiceResId = "2026";

		private float _loopTime = 10f;

		public NewbieStepEleBatOneOneUseSkillFirst()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_UseSkillFirst;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideUseSkillHint(0);
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowUseSkillHint(0);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
