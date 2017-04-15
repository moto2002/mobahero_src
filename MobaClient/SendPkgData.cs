using MobaProtocol;
using System;
using System.Collections.Generic;

namespace MobaClient
{
	public class SendPkgData
	{
		public PvpCode code;

		public long timeTick;

		public long firstTimeTick;

		public int resendCnt;

		public Dictionary<byte, object> rsp;

		public SendPkgData()
		{
			this.rsp = new Dictionary<byte, object>();
		}

		public void SaveRsp(Dictionary<byte, object> _rsp)
		{
			this.rsp[0] = _rsp[0];
			this.rsp[1] = _rsp[1];
			this.rsp[2] = _rsp[2];
		}

		public void ResetSendTick()
		{
			this.timeTick = DateTime.Now.Ticks;
			this.resendCnt++;
		}
	}
}
