using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepBatOneOneKillHome : NewbieStepBase
	{
		private string _voiceResId = "2043";

		private float _loopTime = 10f;

		private Vector3 _centerPos = new Vector3(19.89f, 0.3f, -0.3f);

		public NewbieStepBatOneOneKillHome()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_KillHome;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			NewbieManager.Instance.HideMoveGuideLine();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.ShowMoveGuideLine(this._centerPos);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
