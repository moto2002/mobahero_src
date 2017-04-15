using System;

namespace Newbie
{
	public class NewbieStepBatOneOneSelReliveHero : NewbieStepBase
	{
		private string _voidceResId = "2040";

		private float _loopTime = 10f;

		public NewbieStepBatOneOneSelReliveHero()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_SelReliveHero;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.StartCheckSelEnemyHero();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voidceResId, this._loopTime);
		}
	}
}
