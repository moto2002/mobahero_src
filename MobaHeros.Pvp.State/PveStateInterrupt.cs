using System;

namespace MobaHeros.Pvp.State
{
	public class PveStateInterrupt : PvpStateBase
	{
		public PveStateInterrupt() : base(PvpStateCode.PveInterrupt)
		{
		}

		protected override void OnEnter()
		{
			NetWorkHelper.Instance.DisconnectPvpServer();
			NetWorkHelper.Instance.ConnectToGateServer();
		}
	}
}
