using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneKillHeroAwd : NewbieStepBase
	{
		private string _voiceResId = "2027";

		public NewbieStepEleBatOneOneKillHeroAwd()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_KillHeroAwd;
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
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.01f));
			Singleton<NewbieView>.Instance.ShowPlayNiceEffect();
			NewbieManager.Instance.DestroyEffectObstacleByIdx(3);
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			CtrlManager.CloseWindow(WindowID.BattleEquipmentView);
			CtrlManager.CloseWindow(WindowID.StatisticView);
			base.AutoMoveNextStepWithDelay(1f);
		}
	}
}
