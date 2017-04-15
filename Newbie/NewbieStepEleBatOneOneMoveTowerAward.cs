using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneMoveTowerAward : NewbieStepBase
	{
		private string _voiceResId = "2014";

		public NewbieStepEleBatOneOneMoveTowerAward()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_MoveTowerAward;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HidePlayNiceEffect();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.CloseWindow(WindowID.BattleEquipmentView);
			CtrlManager.CloseWindow(WindowID.StatisticView);
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.01f));
			Singleton<NewbieView>.Instance.ShowPlayNiceEffect();
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(9f);
		}
	}
}
