using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;

namespace MobaClient
{
	public class TimeSyncSystem
	{
		public long clientTime = 0L;

		private PhotonClient client;

		public MobaPeer pvpPeer;

		private long realServerTime = 0L;

		private long realServerPing = 0L;

		private long ping = 0L;

		private long deltaTime = 0L;

		private long clientWorldTime = 0L;

		private long firstTickTime = 0L;

		private static int serverTimeSyncSeqno = 0;

		private int waitServerTimeSyncSeqno = 0;

		private long extraDelayTime = 0L;

		private long lastAckNotBackTime = 0L;

		private TimeSyncInfo[] timeSyncInfoReceived = new TimeSyncInfo[20];

		private TimeSyncMsgTimeoutInfo[] timeSyncMsgTimeoutLogArr = new TimeSyncMsgTimeoutInfo[20];

		private long tmpTime = 0L;

		private long lastServerTime = 0L;

		private long pingMsgSentTimeout = 0L;

		private long maxPingInterval = 2000L;

		public long serverTime
		{
			get
			{
				return this.realServerTime - this.extraDelayTime;
			}
		}

		public long ExtraDelayTime
		{
			get
			{
				return this.extraDelayTime;
			}
		}

		public long Ping
		{
			get
			{
				return this.ping;
			}
		}

		public TimeSyncSystem(PhotonClient _client, MobaPeer _peer)
		{
			this.client = _client;
			this.pvpPeer = _peer;
			this.firstTickTime = DateTime.Now.Ticks / 10000L;
			for (int i = 0; i < this.timeSyncInfoReceived.Length; i++)
			{
				this.timeSyncInfoReceived[i] = new TimeSyncInfo();
			}
			for (int i = 0; i < this.timeSyncMsgTimeoutLogArr.Length; i++)
			{
				this.timeSyncMsgTimeoutLogArr[i] = new TimeSyncMsgTimeoutInfo();
				this.timeSyncMsgTimeoutLogArr[i].isAckBack = true;
			}
		}

		private void PreUpdate()
		{
			long num = DateTime.Now.Ticks / 10000L - this.firstTickTime;
			this.deltaTime = num - this.clientWorldTime;
			this.clientWorldTime = num;
			if (this.realServerTime > 0L)
			{
				this.realServerTime += this.deltaTime;
			}
		}

		public void Update()
		{
			this.PreUpdate();
			this.OnUpdate(this.deltaTime);
			this.PostUpdate(this.deltaTime);
		}

		private void OnUpdate(long deltaTime)
		{
			if (this.pvpPeer != null)
			{
				this.tmpTime += deltaTime;
				if (this.tmpTime > 1000L)
				{
					this.lastServerTime = this.serverTime;
					this.tmpTime = 0L;
				}
				this.SendPingMsg(deltaTime);
				this.extraDelayTime = (long)(this.GetExtraTimeDelayRatio() * (float)this.pvpPeer.RoundTripTime);
				if (this.clientWorldTime - this.lastAckNotBackTime < 10000L)
				{
					this.extraDelayTime += (long)(this.pvpPeer.RoundTripTime / 2);
				}
			}
		}

		private void PostUpdate(long deltaTime)
		{
			if (this.clientTime == 0L && this.realServerTime != 0L)
			{
				this.clientTime = this.serverTime;
			}
		}

		private void SendPingMsg(long deltaTime)
		{
			this.pingMsgSentTimeout += deltaTime;
			if (this.pingMsgSentTimeout >= this.maxPingInterval)
			{
				this.SendServerTimeReq();
				this.pingMsgSentTimeout = 0L;
			}
		}

		private void SendServerTimeReq()
		{
			if (TimeSyncSystem.serverTimeSyncSeqno == 0)
			{
				TimeSyncSystem.serverTimeSyncSeqno++;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			this.waitServerTimeSyncSeqno = TimeSyncSystem.serverTimeSyncSeqno++;
			dictionary[0] = this.waitServerTimeSyncSeqno;
			dictionary[1] = this.clientWorldTime;
			if (this.pvpPeer != null)
			{
				this.pvpPeer.OpCustom(3, dictionary, false, 0);
			}
			this.timeSyncMsgTimeoutLogArr[this.waitServerTimeSyncSeqno % this.timeSyncMsgTimeoutLogArr.Length].isAckBack = false;
			this.timeSyncMsgTimeoutLogArr[this.waitServerTimeSyncSeqno % this.timeSyncMsgTimeoutLogArr.Length].clientSentTime = this.clientWorldTime;
		}

		public void OnServerTimeRsp(object obj)
		{
			OperationResponse operationResponse = obj as OperationResponse;
			int num = (int)operationResponse.Parameters[0];
			long num2 = (long)operationResponse.Parameters[1];
			long num3 = (long)operationResponse.Parameters[2];
			int num4 = num % this.timeSyncInfoReceived.Length;
			this.timeSyncInfoReceived[num4].ping = this.clientWorldTime - num2;
			this.timeSyncInfoReceived[num4].realServerTime = num3;
			this.timeSyncInfoReceived[num4].clientTimeWhenReceived = this.clientWorldTime;
			if (this.realServerTime == 0L)
			{
				this.realServerTime = num3;
				this.realServerPing = this.timeSyncInfoReceived[num4].ping;
			}
			else
			{
				this.UpdateRealServerTime();
			}
			this.timeSyncMsgTimeoutLogArr[num % this.timeSyncMsgTimeoutLogArr.Length].isAckBack = true;
			this.pingMsgSentTimeout = this.maxPingInterval - 200L;
		}

		private int GetTimeoutCnt(out int ackBackCnt)
		{
			int num = 0;
			ackBackCnt = 0;
			for (int i = 0; i < this.timeSyncMsgTimeoutLogArr.Length; i++)
			{
				TimeSyncMsgTimeoutInfo timeSyncMsgTimeoutInfo = this.timeSyncMsgTimeoutLogArr[i];
				if (timeSyncMsgTimeoutInfo.clientSentTime > this.clientWorldTime - 6000L)
				{
					if (!timeSyncMsgTimeoutInfo.isAckBack)
					{
						if (this.clientWorldTime - timeSyncMsgTimeoutInfo.clientSentTime > this.pingMsgSentTimeout)
						{
							num++;
						}
						if (this.lastAckNotBackTime < timeSyncMsgTimeoutInfo.clientSentTime)
						{
							this.lastAckNotBackTime = timeSyncMsgTimeoutInfo.clientSentTime;
						}
					}
					else
					{
						ackBackCnt++;
					}
				}
			}
			return num;
		}

		private float GetExtraTimeDelayRatio()
		{
			int num = 0;
			int timeoutCnt = this.GetTimeoutCnt(out num);
			float result;
			if (num == 0)
			{
				if (timeoutCnt == 0)
				{
					result = 0f;
				}
				else
				{
					result = 1f;
				}
			}
			else
			{
				float num2 = (float)timeoutCnt * 2f / (float)num;
				result = num2;
			}
			return result;
		}

		private long GetMinServerTime()
		{
			long num = 0L;
			for (int i = 0; i < this.timeSyncInfoReceived.Length; i++)
			{
				TimeSyncInfo timeSyncInfo = this.timeSyncInfoReceived[i];
				if (timeSyncInfo.ping > 0L)
				{
					if (num == 0L)
					{
						num = timeSyncInfo.realServerTime;
					}
					else if (num > timeSyncInfo.realServerTime)
					{
						num = timeSyncInfo.realServerTime;
					}
				}
			}
			return num;
		}

		private void UpdateRealServerTime()
		{
			long minServerTime = this.GetMinServerTime();
			if (minServerTime != 0L)
			{
				long num = 0L;
				int num2 = 0;
				long num3 = 0L;
				for (int i = 0; i < this.timeSyncInfoReceived.Length; i++)
				{
					TimeSyncInfo timeSyncInfo = this.timeSyncInfoReceived[i];
					if (timeSyncInfo.ping > 0L)
					{
						num += timeSyncInfo.realServerTime - minServerTime + this.clientWorldTime - timeSyncInfo.clientTimeWhenReceived;
						num3 += timeSyncInfo.ping;
						num2++;
					}
				}
				if (num2 > 0)
				{
					this.ping = num3 / (long)num2;
					long num4 = num / (long)num2 + minServerTime;
					this.realServerTime = num4;
					this.realServerPing = this.ping;
				}
			}
		}

		private void PrecisionCorrection(TimeSyncInfo info)
		{
			long num = this.realServerTime - info.realServerTime;
			if (num < -20L)
			{
				this.realServerTime += 1L;
			}
			else if (num > 20L)
			{
				this.realServerTime -= 1L;
			}
		}
	}
}
