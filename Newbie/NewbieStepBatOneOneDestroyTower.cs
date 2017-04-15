using System;

namespace Newbie
{
	public class NewbieStepBatOneOneDestroyTower : NewbieStepBase
	{
		private string _voiceResId = "2035";

		private float _loopTime = 10f;

		public NewbieStepBatOneOneDestroyTower()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_DestroyTower;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
