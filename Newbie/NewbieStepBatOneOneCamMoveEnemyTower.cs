using Com.Game.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepBatOneOneCamMoveEnemyTower : NewbieStepBase
	{
		public NewbieStepBatOneOneCamMoveEnemyTower()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_CamMoveEnemyTower;
		}

		public override void OnLeave()
		{
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.01f));
			this.DoCamMoveEnemyTower();
			base.AutoMoveNextStepWithDelay(0.3f);
		}

		private void DoCamMoveEnemyTower()
		{
			IList<Units> tower = MapManager.Instance.GetTower(TeamType.BL);
			BattleCameraMgr.Instance.SetPositionAndMoveTime(tower[0].mTransform.position, 0.7f);
		}
	}
}
