using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleHallActEnd : NewbieStepBase
	{
		public NewbieStepEleHallActEnd()
		{
			this._stepType = ENewbieStepType.EleHallAct_End;
		}

		public override void OnLeave()
		{
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisablePhaseEleHallAct();
			CtrlManager.CloseWindow(WindowID.NewbieView);
			NewbieManager.Instance.StopDisplaySubtitle();
			NewbieManager.Instance.DestroySubtitleCtrl();
			this.ClearResources();
		}

		private void ClearResources()
		{
			NewbieManager.Instance.ClearPhaseEleHallActResources();
		}
	}
}
