using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveHintPlay : NewbieStepBase
	{
		private string _voiceResId = "2201";

		private float _loopTime = 10f;

		public NewbieStepFakeMatchFiveHintPlay()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_HintPlay;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideFakeMatchFiveHintPlay();
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
			Singleton<NewbieView>.Instance.ShowFakeMatchFiveHintPlay();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
