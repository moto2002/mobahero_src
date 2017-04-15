using System;

namespace MobaClient
{
	public class ChannelInfo
	{
		public int channelID;

		public long firstSeqNo = 0L;

		public long lastSeqNo = 0L;

		public ChannelInfo(int _channelID)
		{
			this.channelID = _channelID;
		}

		public bool Empty()
		{
			return this.firstSeqNo == this.lastSeqNo;
		}

		public void AlignSeqNo(long _seqNo)
		{
			if (_seqNo >= this.lastSeqNo)
			{
				this.lastSeqNo = _seqNo + 1L;
			}
		}
	}
}
