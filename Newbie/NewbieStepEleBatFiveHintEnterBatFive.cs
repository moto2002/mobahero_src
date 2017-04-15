using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatFiveHintEnterBatFive : NewbieStepBase
	{
		private string _voiceResId = "2101";

		public NewbieStepEleBatFiveHintEnterBatFive()
		{
			this._stepType = ENewbieStepType.EleBatFive_HintEnterBatFive;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintOnce();
		}

		public override void HandleAction()
		{
			CtrlManager.CloseWindow(WindowID.ArenaModeView);
			CtrlManager.CloseWindow(WindowID.SignView);
			CtrlManager.CloseWindow(WindowID.ActivityView);
			CtrlManager.OpenWindow(WindowID.MenuBottomBarView, null);
			CtrlManager.OpenWindow(WindowID.MenuTopBarView, null);
			CtrlManager.OpenWindow(WindowID.MenuView, null);
			NewbieManager.Instance.NotifyServerNewbieGuidePhase(ENewbiePhaseType.ElementaryBattleFiveFive);
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.027f, 0.051f, 0.175f, 0.9f));
			Singleton<NewbieView>.Instance.ShowHintEnterBatFive();
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(8f);
		}
	}
}
