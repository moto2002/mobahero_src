using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveLoading : NewbieStepBase
	{
		public NewbieStepEleBatFiveLoading()
		{
			this._stepType = ENewbieStepType.EleBatFive_Loading;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopCheckSpecialEnterBattleSuc();
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.StartCheckSpecialEnterBattleSuc(15f, ENewbieStepType.EleBatFive_SelectMap);
			CtrlManager.OpenWindow(WindowID.NewbieLoadView, null);
			Singleton<NewbieLoadView>.Instance.SetBgByNewbiePhase(ENewbiePhaseType.ElementaryBattleFiveFive);
		}
	}
}
