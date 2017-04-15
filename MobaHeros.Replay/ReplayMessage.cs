using MobaProtocol;
using System;

namespace MobaHeros.Replay
{
	public struct ReplayMessage
	{
		public PvpCode code;

		public float time;

		public byte[] param;
	}
}
