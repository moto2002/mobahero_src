using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveFirNearOutShop : NewbieStepBase
	{
		private string _voiceResId = "2119";

		public NewbieStepEleBatFiveFirNearOutShop()
		{
			this._stepType = ENewbieStepType.EleBatFive_FirNearOutShop;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EnableEleBatFiveTriggerCheck();
			NewbieManager.Instance.StopVoiceHintOnce();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableEleBatFiveTriggerCheck();
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(5.5f);
		}
	}
}
