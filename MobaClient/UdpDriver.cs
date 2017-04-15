using ExitGames.Client.Photon;
using MobaProtocol;
using System;
using System.Collections.Generic;

namespace MobaClient
{
	public class UdpDriver : UdpDriverBase
	{
		public PhotonClient client;

		public MobaPeer pvpPeer;

		public UdpDriver(PhotonClient _client, MobaPeer _peer)
		{
			this.client = _client;
			this.pvpPeer = _peer;
		}

		public void OnUpdate()
		{
			this.TryResend();
		}

		public bool Send(PvpCode code, Dictionary<byte, object> param)
		{
			this.lastSentCode = code;
			this.lastSentSeqno = this.curSeqno;
			this.lastSentParam = param;
			this.lastSentTime = DateTime.Now.Ticks;
			param[1] = this.lastSentSeqno;
			this.curSeqno++;
			return this.pvpPeer.OpCustom((byte)code, param, false);
		}

		public void OnRecv(object obj)
		{
			OperationResponse operationResponse = obj as OperationResponse;
			int num = 1;
			if (operationResponse.OperationCode == UdpDriverBase.OperationCode_Ack)
			{
				base.OnAck((int)operationResponse.Parameters[0]);
			}
			else
			{
				int num2 = (int)operationResponse.Parameters[0];
				int num3 = 4;
				for (int i = 0; i < num2; i++)
				{
					long num4 = (long)operationResponse.Parameters[(byte)(num3 * i + num)];
					int channelID = (int)(num4 / 1000L);
					byte code = (byte)(num4 % 1000L);
					long seqNo = (long)operationResponse.Parameters[(byte)(num3 * i + 1 + num)];
					long sentSvrTime = (long)operationResponse.Parameters[(byte)(num3 * i + 2 + num)];
					object msg = operationResponse.Parameters[(byte)(num3 * i + 3 + num)];
					base.OnRecvMsg(code, channelID, seqNo, sentSvrTime, msg);
				}
			}
		}

		public void TryProcessAvailablePkg()
		{
			Dictionary<int, ChannelInfo>.Enumerator enumerator = this.channelArr.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, ChannelInfo> current = enumerator.Current;
				ChannelInfo value = current.Value;
				if (!value.Empty())
				{
					UdpPackage[] array = this.pkgMatrix[value.channelID];
					for (long num = value.firstSeqNo; num < value.lastSeqNo; num += 1L)
					{
						int num2 = base.SeqnoIdx(num);
						UdpPackage udpPackage = array[num2];
						if (udpPackage == null || udpPackage.seqNo != num)
						{
							break;
						}
						this.client.ExecUnrelivableCmd((PvpCode)udpPackage.code, udpPackage.msg);
						value.firstSeqNo += 1L;
					}
				}
			}
		}

		public void SendAllAck()
		{
			Dictionary<byte, object> dictionary = null;
			byte b = 0;
			int num = 1;
			int num2 = 3;
			for (int i = 0; i < this.maxAckDictArrLen; i++)
			{
				Dictionary<int, Dictionary<long, AckInfo>>.Enumerator enumerator = this.ackDict[i].GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, Dictionary<long, AckInfo>> current = enumerator.Current;
					Dictionary<long, AckInfo> value = current.Value;
					current = enumerator.Current;
					int key = current.Key;
					Dictionary<long, AckInfo>.Enumerator enumerator2 = value.GetEnumerator();
					int num3 = 0;
					while (enumerator2.MoveNext())
					{
						if (b >= 70)
						{
							dictionary[0] = b;
							this.pvpPeer.OpCustom(UdpDriverBase.OperationCode_Ack, dictionary, false, 0);
							b = 0;
							dictionary = null;
						}
						if (dictionary == null)
						{
							dictionary = new Dictionary<byte, object>();
						}
						if (num3 == 0)
						{
							dictionary[(byte)(num2 * (int)b + num)] = key;
							dictionary[(byte)(num2 * (int)b + 1 + num)] = base.GetNewestFirstSeqno(key);
							dictionary[(byte)(num2 * (int)b + 2 + num)] = 0L;
							b += 1;
						}
						KeyValuePair<long, AckInfo> current2 = enumerator2.Current;
						AckInfo value2 = current2.Value;
						dictionary[(byte)(num2 * (int)b + num)] = value2.channelID;
						dictionary[(byte)(num2 * (int)b + 1 + num)] = value2.seqNoRecved;
						dictionary[(byte)(num2 * (int)b + 2 + num)] = value2.sentTime;
						UdpPoolBase<AckInfo>.Free(value2);
						b += 1;
						num3++;
					}
					value.Clear();
				}
			}
			if (b > 0)
			{
				dictionary[0] = b;
				if (!this.pvpPeer.OpCustom(UdpDriverBase.OperationCode_Ack, dictionary, false, 0))
				{
				}
			}
			this.curAckDictArrIdx++;
			this.curAckDictArrIdx %= this.maxAckDictArrLen;
			this.ackDict[this.curAckDictArrIdx].Clear();
		}

		private void TryResend()
		{
			long ticks = DateTime.Now.Ticks;
			if (this.lastSentParam != null && this.lastSentTime + 500000L < ticks)
			{
				this.lastSentTime = ticks;
				this.pvpPeer.OpCustom((byte)this.lastSentCode, this.lastSentParam, false);
			}
		}

		public void OnConnect()
		{
		}

		public void OnDisconnect()
		{
			base.Reset();
		}
	}
}
