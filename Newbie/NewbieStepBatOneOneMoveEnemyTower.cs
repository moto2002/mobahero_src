using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepBatOneOneMoveEnemyTower : NewbieStepBase
	{
		private string _voiceResId = "2033";

		private float _loopTime = 10f;

		private Vector3 _centerPos = new Vector3(11.24f, 0.3f, -0.64f);

		public NewbieStepBatOneOneMoveEnemyTower()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_MoveEnemyTower;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopCheckMoveEnemyTower();
			NewbieManager.Instance.StopVoiceHintLoop();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.StartCheckMoveEnemyTower();
			NewbieManager.Instance.ShowMoveGuideLine(this._centerPos);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
