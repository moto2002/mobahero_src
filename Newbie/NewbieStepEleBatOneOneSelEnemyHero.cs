using System;

namespace Newbie
{
	public class NewbieStepEleBatOneOneSelEnemyHero : NewbieStepBase
	{
		private string _voiceResId = "2025";

		private float _loopTime = 10f;

		public NewbieStepEleBatOneOneSelEnemyHero()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_SelEnemyHero;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.StartCheckSelEnemyHero();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}
	}
}
