using System;

namespace MobaClient
{
	public class PeerDisconnectedMessage
	{
		public MobaPeerType PeerType;

		public MobaDisconnectedType DisconnectedType;

		public PeerDisconnectedMessage(MobaPeerType pType, MobaDisconnectedType dType)
		{
			this.PeerType = pType;
			this.DisconnectedType = dType;
		}
	}
}
