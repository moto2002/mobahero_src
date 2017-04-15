using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatOneOneWaitStartPlay : NewbieStepBase
	{
		private string _resId = "2005";

		private float _loopTime = 10f;

		public NewbieStepEleBatOneOneWaitStartPlay()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_WaitStartPlay;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideLearnHoldDevice();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			Singleton<NewbieView>.Instance.ShowStartGameBtn();
			NewbieManager.Instance.PlayVoiceHintLoop(this._resId, this._loopTime);
		}
	}
}
