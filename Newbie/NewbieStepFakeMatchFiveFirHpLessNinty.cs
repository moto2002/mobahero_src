using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepFakeMatchFiveFirHpLessNinty : NewbieStepBase
	{
		public NewbieStepFakeMatchFiveFirHpLessNinty()
		{
			this._stepType = ENewbieStepType.FakeMatchFive_FirHpLessNinty;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.EnableFakeMatchFiveTrigger();
			NewbieManager.Instance.StopFakeMatchFiveCheckFirHpLessNintyEnd();
			Singleton<NewbieView>.Instance.HideFakeMatchFiveUseRecoveryHint();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.DisableFakeMatchFiveTrigger();
			NewbieManager.Instance.StartFakeMatchFiveCheckFirHpLessNintyEnd();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowFakeMatchFiveUseRecoveryHint();
		}
	}
}
