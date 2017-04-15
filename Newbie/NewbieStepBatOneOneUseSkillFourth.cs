using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepBatOneOneUseSkillFourth : NewbieStepBase
	{
		private string _voiceResId = "2041";

		private float _loopTime = 10f;

		public NewbieStepBatOneOneUseSkillFourth()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_UseSkillFourth;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideUseSkillHint(3);
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowUseSkillHint(3);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
