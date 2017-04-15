using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveFirAtkByTower : NewbieStepBase
	{
		private string _voiceResId = "2109";

		public NewbieStepEleBatFiveFirAtkByTower()
		{
			this._stepType = ENewbieStepType.EleBatFive_FirAtkByTower;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintOnce();
			NewbieManager.Instance.EnableEleBatFiveTriggerCheck();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableEleBatFiveTriggerCheck();
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(5f);
		}
	}
}
