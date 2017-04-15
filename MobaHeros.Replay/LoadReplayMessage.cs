using MobaProtocol;
using System;

namespace MobaHeros.Replay
{
	public struct LoadReplayMessage
	{
		public PvpCode code;

		public float time;

		public byte[] param;

		public object protoObj;
	}
}
