using MobaHeros.Pvp.State;
using System;

namespace GameLogin.State
{
	public class PveStateHome : PvpStateBase
	{
		public PveStateHome() : base(PvpStateCode.PveHome)
		{
		}

		protected override void OnEnter()
		{
			NetWorkHelper.Instance.DisconnectPvpServer();
			NetWorkHelper.Instance.ConnectToGateServer();
		}
	}
}
