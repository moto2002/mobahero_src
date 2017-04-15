using System;

namespace MobaClient
{
	public class TimeSyncInfo
	{
		public bool done = false;

		public int seqNo;

		public long ping;

		public long realServerTime;

		public long clientWorldTime;

		public long clientTimeWhenReceived;
	}
}
