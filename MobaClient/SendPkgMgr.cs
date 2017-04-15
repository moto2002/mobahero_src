using MobaProtocol;
using System;
using System.Collections.Generic;

namespace MobaClient
{
	public class SendPkgMgr
	{
		private const int maxSendCacheSize = 1000;

		private const int maxPingSize = 10;

		private SendPkgData[] sendArr;

		private int[] resendDelay = new int[]
		{
			300000,
			500000,
			1500000,
			5000000
		};

		private PingStatisticInfo[] pingInfo;

		private int curPingIdx = 0;

		public int firstSeqNo;

		public int nextSeqNo;

		private List<SendPkgData> dataPool = new List<SendPkgData>();

		private CustomClientReliableChannel channel;

		public SendPkgMgr(CustomClientReliableChannel _channel)
		{
			this.sendArr = new SendPkgData[1000];
			this.pingInfo = new PingStatisticInfo[10];
			for (int i = 0; i < 10; i++)
			{
				this.pingInfo[i] = new PingStatisticInfo();
			}
			this.channel = _channel;
			this.Reset();
		}

		public void Reset()
		{
			this.firstSeqNo = 0;
			this.nextSeqNo = 0;
			for (int i = 0; i < this.sendArr.Length; i++)
			{
				this.sendArr[i] = null;
			}
		}

		public int GetNextSeqNo()
		{
			return this.nextSeqNo;
		}

		public void AddSendPkg(PvpCode code, int seqNo, Dictionary<byte, object> param)
		{
			this.appendToCache(code, seqNo, param);
		}

		public void OnRecvAck(int seqNo)
		{
			int num = seqNo % 1000;
			if (this.sendArr[num] != null)
			{
				this.SetCurPkgPing(seqNo);
				this.ReleasePkgData(this.sendArr[num]);
				this.sendArr[num] = null;
			}
			this.ReflashFirstSeqNo();
		}

		public void UpdatePing()
		{
			SendPkgData sendPkgData = this.sendArr[this.firstSeqNo % 1000];
			if (sendPkgData != null)
			{
				long num = DateTime.Now.Ticks - sendPkgData.firstTimeTick;
				if (num > this.channel.ping)
				{
					PingStatisticInfo pingStatisticInfo = this.pingInfo[9];
					pingStatisticInfo.delayTime = num;
				}
			}
		}

		private void SetCurPkgPing(int seqNo)
		{
			long ticks = DateTime.Now.Ticks;
			SendPkgData sendPkgData = this.sendArr[seqNo % 1000];
			if (sendPkgData != null)
			{
				long delayTime = ticks - sendPkgData.firstTimeTick;
				PingStatisticInfo pingStatisticInfo = this.pingInfo[this.curPingIdx % 9];
				pingStatisticInfo.tick = ticks;
				pingStatisticInfo.delayTime = delayTime;
				this.curPingIdx++;
			}
		}

		public long GetPing()
		{
			long ticks = DateTime.Now.Ticks;
			int num = 0;
			long num2 = 0L;
			for (int i = 0; i < 10; i++)
			{
				PingStatisticInfo pingStatisticInfo = this.pingInfo[i];
				if ((i == 9 && pingStatisticInfo.tick != 0L) || (pingStatisticInfo.tick != 0L && ticks - pingStatisticInfo.tick < 10000000L))
				{
					num2 += pingStatisticInfo.delayTime;
					num++;
				}
			}
			long result;
			if (num == 0)
			{
				result = 0L;
			}
			else
			{
				result = num2 / (long)num;
			}
			return result;
		}

		public void RetrySendCachePkg()
		{
			for (int i = this.firstSeqNo; i < this.nextSeqNo; i++)
			{
				int num = i % 1000;
				SendPkgData sendPkgData = this.sendArr[num];
				if (sendPkgData != null)
				{
					int resendInterval = this.GetResendInterval(sendPkgData.resendCnt);
					long num2 = DateTime.Now.Ticks - sendPkgData.timeTick;
					if (num2 >= (long)resendInterval)
					{
						sendPkgData.ResetSendTick();
						sendPkgData.rsp[3] = sendPkgData.resendCnt;
						this.channel.Send(sendPkgData.code, sendPkgData.rsp, true);
					}
				}
			}
		}

		public int GetNumberOfWaitingAck()
		{
			return this.nextSeqNo - this.firstSeqNo;
		}

		public SendPkgData GetFirstPkgWaitingAck()
		{
			SendPkgData result;
			for (int i = this.firstSeqNo; i < this.nextSeqNo; i++)
			{
				int num = i % 1000;
				SendPkgData sendPkgData = this.sendArr[num];
				if (sendPkgData != null)
				{
					result = sendPkgData;
					return result;
				}
			}
			result = null;
			return result;
		}

		private int GetResendInterval(int sentCnt)
		{
			if (sentCnt >= this.resendDelay.Length)
			{
				sentCnt = this.resendDelay.Length - 1;
			}
			return this.resendDelay[sentCnt] + this.channel.pvpPeer.RoundTripTime * 10000;
		}

		private void ReflashFirstSeqNo()
		{
			int i;
			for (i = this.firstSeqNo; i < this.nextSeqNo; i++)
			{
				int num = i % 1000;
				if (this.sendArr[num] != null)
				{
					break;
				}
			}
			this.firstSeqNo = i;
		}

		private void appendToCache(PvpCode code, int seqNo, Dictionary<byte, object> param)
		{
			SendPkgData sendPkgData = this.AllocPkgData();
			sendPkgData.code = code;
			sendPkgData.SaveRsp(param);
			sendPkgData.timeTick = DateTime.Now.Ticks;
			sendPkgData.firstTimeTick = sendPkgData.timeTick;
			this.sendArr[seqNo % 1000] = sendPkgData;
			if (this.nextSeqNo == seqNo)
			{
				this.nextSeqNo++;
				if (this.nextSeqNo < 0)
				{
					this.nextSeqNo = 0;
				}
			}
		}

		private SendPkgData AllocPkgData()
		{
			SendPkgData result;
			while (this.dataPool.Count > 0)
			{
				SendPkgData sendPkgData = this.dataPool[0];
				this.dataPool.RemoveAt(0);
				if (sendPkgData != null)
				{
					result = sendPkgData;
					return result;
				}
			}
			result = new SendPkgData();
			return result;
		}

		private void ReleasePkgData(SendPkgData data)
		{
			data.resendCnt = 0;
			data.timeTick = 0L;
			this.dataPool.Add(data);
		}
	}
}
