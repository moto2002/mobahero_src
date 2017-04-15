using Com.Game.Module;
using HUD.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneLearnSkillThdAwd : NewbieStepBase
	{
		private string _voiceResId = "2045";

		public NewbieStepEleBatOneOneLearnSkillThdAwd()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_LearnSkillThdAwd;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideDispSkillInfoHint();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
			if (Singleton<HUDModuleManager>.Instance != null)
			{
				FunctionBtnsModule module = Singleton<HUDModuleManager>.Instance.GetModule<FunctionBtnsModule>(EHUDModule.FunctionBtns);
				if (module != null)
				{
					module.NewbieForceHideSkillInfo();
				}
			}
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.01f));
			Singleton<NewbieView>.Instance.ShowDispSkillInfoHint();
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(7.5f);
		}
	}
}
