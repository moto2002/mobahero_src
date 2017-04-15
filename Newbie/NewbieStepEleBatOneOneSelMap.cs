using Com.Game.Module;
using MobaHeros.Pvp;
using System;

namespace Newbie
{
	public class NewbieStepEleBatOneOneSelMap : NewbieStepBase
	{
		public NewbieStepEleBatOneOneSelMap()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_SelectMap;
		}

		public override void OnLeave()
		{
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.InitSetSpecialEnterBattleInfo();
			this.SelectMap();
			CtrlManager.OpenWindow(WindowID.NewbieLoadView, null);
			Singleton<NewbieLoadView>.Instance.SetBgByNewbiePhase(ENewbiePhaseType.ElementaryBattleOneOne);
			base.AutoMoveNextStepWithDelay(0.3f);
		}

		private void SelectMap()
		{
			int num = 90001;
			PvpLevelStorage.SetLevel(MatchType.DP, num.ToString());
			PvpLevelStorage.JoinAsSingle();
			Singleton<PvpManager>.Instance.ChooseSingleGame(num.ToString(), string.Empty);
		}
	}
}
