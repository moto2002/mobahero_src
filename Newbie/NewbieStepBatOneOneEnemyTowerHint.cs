using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepBatOneOneEnemyTowerHint : NewbieStepBase
	{
		private string _voiceResId = "2032";

		public NewbieStepBatOneOneEnemyTowerHint()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_EnemyTowerHint;
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
			base.AutoMoveNextStepWithDelay(4.5f);
		}
	}
}
