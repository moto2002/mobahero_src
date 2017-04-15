using Com.Game.Module;
using MobaClient;
using System;

namespace MobaHeros.Pvp.State
{
	public class PvpStateInQueue : PvpStateBase
	{
		public PvpStateInQueue() : base(PvpStateCode.PvpInQueue)
		{
		}

		protected override void OnDisconnectServer(MobaPeerType type)
		{
			base.OnDisconnectServer(type);
			if (type == MobaPeerType.C2GateServer)
			{
				PvpStateManager.Instance.ChangeState(new PvpStateRecover(PvpStateCode.PvpInQueue));
			}
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			ArenaModeView.ShowMatchingState(true);
			CtrlManager.CloseWindow(WindowID.PvpSelectHeroView);
		}
	}
}
