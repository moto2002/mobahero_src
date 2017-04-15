using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleHallActOpenAchievement : NewbieStepBase
	{
		private string _voiceResId = "2301";

		private float _loopTime = 10f;

		public NewbieStepEleHallActOpenAchievement()
		{
			this._stepType = ENewbieStepType.EleHallAct_OpenAchievement;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideEleHallActOpenAchieve();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.CloseWindow(WindowID.ArenaModeView);
			CtrlManager.CloseWindow(WindowID.SignView);
			CtrlManager.CloseWindow(WindowID.ActivityView);
			CtrlManager.OpenWindow(WindowID.MenuBottomBarView, null);
			CtrlManager.OpenWindow(WindowID.MenuTopBarView, null);
			CtrlManager.OpenWindow(WindowID.MenuView, null);
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowEleHallActOpenAchieve();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
