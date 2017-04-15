using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveFirHpLessThirty : NewbieStepBase
	{
		public NewbieStepFakeMatchFiveFirHpLessThirty()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_FirHpLessThirty;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EnableFakeMatchFiveTrigger();
			NewbieManager.Instance.StopFakeMatchFiveCheckFirHpLessThirtyEnd();
			Singleton<NewbieView>.Instance.HideFakeMatchFiveUseBackHomeHint();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableFakeMatchFiveTrigger();
			NewbieManager.Instance.StartFakeMatchFiveCheckFirHpLessThirtyEnd();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowFakeMatchFiveUseBackHomeHint();
		}
	}
}
