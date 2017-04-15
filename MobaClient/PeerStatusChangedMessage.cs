using ExitGames.Client.Photon;
using System;

namespace MobaClient
{
	public class PeerStatusChangedMessage
	{
		public MobaPeerType PeerType;

		public StatusCode StatCode;

		public PeerStatusChangedMessage(MobaPeerType pType, StatusCode sCode)
		{
			this.PeerType = pType;
			this.StatCode = sCode;
		}
	}
}
