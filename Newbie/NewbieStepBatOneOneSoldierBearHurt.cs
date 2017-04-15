using System;

namespace Newbie
{
	public class NewbieStepBatOneOneSoldierBearHurt : NewbieStepBase
	{
		private string _voiceResId = "2034";

		public NewbieStepBatOneOneSoldierBearHurt()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_SoldierBearHurt;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintOnce();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(5f);
		}
	}
}
