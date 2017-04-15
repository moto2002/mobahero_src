using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveHintPicFirstGoNext : NewbieStepBase
	{
		public NewbieStepEleBatFiveHintPicFirstGoNext()
		{
			this._stepType = ENewbieStepType.EleBatFive_HintPicFirstGoNext;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideEleBatFiveHintMoveNextBtn();
			Singleton<NewbieView>.Instance.HideEleBatFiveHintPicFirst();
		}

		public override void HandleAction()
		{
			Singleton<NewbieView>.Instance.ShowEleBatFiveHintMoveNextBtn();
			NewbieManager.Instance.EleBatFiveHideHudViewComponents();
		}
	}
}
