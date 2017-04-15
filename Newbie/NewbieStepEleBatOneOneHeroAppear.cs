using Com.Game.Module;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneHeroAppear : NewbieStepBase
	{
		public NewbieStepEleBatOneOneHeroAppear()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_HeroAppearance;
		}

		public override void OnLeave()
		{
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			this.DisplayAllObstacleEffect();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.01f));
			base.AutoMoveNextStepWithDelay(0.5f);
		}

		private void DisplayAllObstacleEffect()
		{
			NewbieManager.Instance.DisplayAllEffectObstacles();
		}
	}
}
