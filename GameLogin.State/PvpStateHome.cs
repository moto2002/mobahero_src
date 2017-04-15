using Com.Game.Module;
using MobaHeros.Pvp;
using MobaHeros.Pvp.State;
using System;

namespace GameLogin.State
{
	public class PvpStateHome : PvpStateBase
	{
		public PvpStateHome() : base(PvpStateCode.Home)
		{
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			PvpMatchMgr.Instance.QuitMatch(false);
			if (!NetWorkHelper.Instance.GateReconnection.Available)
			{
				NetWorkHelper.Instance.GateReconnection.Begin();
			}
			NetWorkHelper.Instance.DisconnectLobbyServer();
			NetWorkHelper.Instance.DisconnectPvpServer();
			ArenaModeView.ShowMatchingState(false);
			CtrlManager.CloseWindow(WindowID.PvpSelectHeroView);
			CtrlManager.CloseWindow(WindowID.PvpWaitView);
			PvpUtils.GoHome();
			Singleton<PvpManager>.Instance.IsContinuedBattle = false;
			Singleton<MenuView>.Instance.UpdateFreeActive();
		}
	}
}
