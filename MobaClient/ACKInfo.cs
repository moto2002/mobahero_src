using MobaProtocol;
using System;

namespace MobaClient
{
	public class ACKInfo
	{
		public PvpCode code;

		public int channelID;

		public int seqNo;
	}
	public class AckInfo : UdpPoolBase<AckInfo>
	{
		public int channelID;

		public long seqNoRecved;

		public long sentTime;

		protected override void Reset()
		{
			this.channelID = 0;
			this.sentTime = 0L;
		}
	}
}
