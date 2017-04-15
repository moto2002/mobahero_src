using System;

namespace MobaClient
{
	public class PeerConnectedMessage
	{
		public MobaPeerType PeerType;

		public MobaConnectedType ConnectedType;

		public PeerConnectedMessage(MobaPeerType pType, MobaConnectedType cType)
		{
			this.PeerType = pType;
			this.ConnectedType = cType;
		}
	}
}
