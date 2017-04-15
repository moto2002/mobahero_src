using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveHintPicSecondGoNext : NewbieStepBase
	{
		public NewbieStepEleBatFiveHintPicSecondGoNext()
		{
			this._stepType = ENewbieStepType.EleBatFive_HintPicSecondGoNext;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideEleBatFiveHintMoveNextBtn();
			Singleton<NewbieView>.Instance.HideEleBatFiveHintPicSecond();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			Singleton<NewbieView>.Instance.ShowEleBatFiveHintMoveNextBtn();
		}
	}
}
