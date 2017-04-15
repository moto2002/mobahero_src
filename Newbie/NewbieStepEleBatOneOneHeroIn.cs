using System;

namespace Newbie
{
	public class NewbieStepEleBatOneOneHeroIn : NewbieStepBase
	{
		private string _voiceResId = "2000";

		public NewbieStepEleBatOneOneHeroIn()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_HeroIn;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintOnce();
			NewbieManager.Instance.HideHeroIn();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.HideHudView();
			NewbieManager.Instance.ShowHeroIn();
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(4.6f);
		}
	}
}
