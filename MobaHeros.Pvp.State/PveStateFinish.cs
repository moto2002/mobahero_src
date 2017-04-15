using System;

namespace MobaHeros.Pvp.State
{
	public class PveStateFinish : PvpStateBase
	{
		public PveStateFinish() : base(PvpStateCode.PveFinish)
		{
		}

		protected override void OnEnter()
		{
			NetWorkHelper.Instance.DisconnectPvpServer();
			NetWorkHelper.Instance.ConnectToGateServer();
		}
	}
}
