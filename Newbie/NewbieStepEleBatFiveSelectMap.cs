using Com.Game.Module;
using MobaHeros.Pvp;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFiveSelectMap : NewbieStepBase
	{
		public NewbieStepEleBatFiveSelectMap()
		{
			this._stepType = ENewbieStepType.EleBatFive_SelectMap;
		}

		public override void OnLeave()
		{
			Singleton<NewbieView>.Instance.HideHintEleBatFiveToSelectMap();
			Singleton<NewbieView>.Instance.HideHintEnterBatFive();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.InitSetSpecialEnterBattleInfo();
			this.SelectMap();
			base.AutoMoveNextStepWithDelay(0.22f);
		}

		private void SelectMap()
		{
			int num = 90002;
			PvpLevelStorage.SetLevel(MatchType.DP, num.ToString());
			PvpLevelStorage.JoinAsSingle();
			Singleton<PvpManager>.Instance.ChooseSingleGame(num.ToString(), "排队中");
		}
	}
}
