using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepBatOneOneLevelUpSix : NewbieStepBase
	{
		private string _voiceResId = "2037";

		public NewbieStepBatOneOneLevelUpSix()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_LevelUpSix;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.01f));
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(5f);
		}
	}
}
