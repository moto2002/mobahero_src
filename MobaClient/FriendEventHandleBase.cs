using ExitGames.Client.Photon;
using System;

namespace MobaClient
{
	internal class FriendEventHandleBase : ClientResponseRecv, INetEventHandleBase
	{
		protected override void RegistCmds()
		{
			base.Regist(0, new ClientResponseRecv.NetResponseEvent(this.OnAddFriend));
			base.Regist(1, new ClientResponseRecv.NetResponseEvent(this.OnDelFriend));
		}

		private void OnAddFriend(OperationResponse resp)
		{
		}

		private void OnDelFriend(OperationResponse resp)
		{
		}
	}
}
