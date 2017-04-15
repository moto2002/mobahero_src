using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveHintPicSecond : NewbieStepBase
	{
		private string _voiceResId = "2103";

		private float _loopTime = 22f;

		public NewbieStepEleBatFiveHintPicSecond()
		{
			this._stepType = ENewbieStepType.EleBatFive_HintPicSecond;
		}

		public override void OnLeave()
		{
		}

		public override void HandleAction()
		{
			Singleton<NewbieView>.Instance.ShowEleBatFiveHintPicSecond();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
			base.AutoMoveNextStepWithDelay(12.5f);
		}
	}
}
