using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneMoveHomeAward : NewbieStepBase
	{
		private string _voiceResId = "2012";

		public NewbieStepEleBatOneOneMoveHomeAward()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_MoveHomeAward;
		}

		public override void OnLeave()
		{
			Singleton<NewbieView>.Instance.HidePlayNiceEffect();
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.CloseWindow(WindowID.BattleEquipmentView);
			CtrlManager.CloseWindow(WindowID.StatisticView);
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.01f));
			NewbieManager.Instance.NotifyServerNewbieStep(ENewbieStepType.ElementaryBatOneOne_MoveHomeAward);
			NewbieManager.Instance.DestroyEffectObstacleByIdx(1);
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			Singleton<NewbieView>.Instance.ShowPlayNiceEffect();
			base.AutoMoveNextStepWithDelay(3f);
		}
	}
}
