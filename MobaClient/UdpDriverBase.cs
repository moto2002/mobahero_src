using MobaProtocol;
using System;
using System.Collections.Generic;

namespace MobaClient
{
	public class UdpDriverBase
	{
		protected const int maxPkgsPerChannel = 500;

		protected Dictionary<int, UdpPackage[]> pkgMatrix;

		protected Dictionary<int, ChannelInfo> channelArr;

		protected long sendIndex = 0L;

		protected PvpCode lastSentCode;

		protected Dictionary<byte, object> lastSentParam;

		protected int lastSentSeqno = 0;

		protected long lastSentTime = 0L;

		protected int curSeqno = 0;

		public static byte OperationCode_Ack = 1;

		public static byte OperationCode_Other = 2;

		protected Dictionary<int, Dictionary<long, AckInfo>>[] ackDict = null;

		protected int maxAckDictArrLen = 4;

		protected int curAckDictArrIdx = 0;

		public UdpDriverBase()
		{
			this.pkgMatrix = new Dictionary<int, UdpPackage[]>();
			this.channelArr = new Dictionary<int, ChannelInfo>();
			this.ackDict = new Dictionary<int, Dictionary<long, AckInfo>>[this.maxAckDictArrLen];
			for (int i = 0; i < this.maxAckDictArrLen; i++)
			{
				this.ackDict[i] = new Dictionary<int, Dictionary<long, AckInfo>>();
			}
		}

		private void ResetAckDict()
		{
			for (int i = 0; i < this.maxAckDictArrLen; i++)
			{
				Dictionary<int, Dictionary<long, AckInfo>>.Enumerator enumerator = this.ackDict[i].GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, Dictionary<long, AckInfo>> current = enumerator.Current;
					Dictionary<long, AckInfo> value = current.Value;
					value.Clear();
				}
			}
		}

		private void ResetPkgMatrix()
		{
			Dictionary<int, ChannelInfo>.Enumerator enumerator = this.channelArr.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, ChannelInfo> current = enumerator.Current;
				current.Value.firstSeqNo = 0L;
				current = enumerator.Current;
				current.Value.lastSeqNo = 0L;
			}
		}

		protected void Reset()
		{
			this.curSeqno = 0;
			this.lastSentSeqno = 0;
			this.lastSentParam = null;
			this.lastSentTime = 0L;
			this.ResetAckDict();
			this.ResetPkgMatrix();
		}

		protected void OnAck(int seqNo)
		{
			if (seqNo == this.lastSentSeqno)
			{
				this.lastSentParam = null;
				this.lastSentCode = (PvpCode)0;
				this.lastSentSeqno = 0;
			}
		}

		public void OnRecvMsg(byte code, int channelID, long seqNo, long sentSvrTime, object msg)
		{
			UdpPackage udpPackage = UdpPackage.AllocPkg(code, channelID, seqNo, msg);
			if (!this.channelArr.ContainsKey(channelID))
			{
				this.pkgMatrix.Add(channelID, new UdpPackage[500]);
				this.channelArr.Add(channelID, new ChannelInfo(channelID));
			}
			int num = this.SeqnoIdx(seqNo);
			UdpPackage[] array = this.pkgMatrix[channelID];
			UdpPackage udpPackage2 = array[num];
			if (udpPackage2 != null)
			{
				UdpPackage.FreePkg(udpPackage2);
			}
			array[num] = udpPackage;
			udpPackage.code = code;
			udpPackage.seqNo = seqNo;
			udpPackage.sentTime = sentSvrTime;
			this.AlignSeqNo(channelID, seqNo);
			this.AddAckDict(channelID, seqNo, sentSvrTime);
		}

		private void AddAckDict(int channelID, long seqNoRecved, long sentSvrTime)
		{
			Dictionary<long, AckInfo> dictionary = null;
			if (!this.ackDict[this.curAckDictArrIdx].TryGetValue(channelID, out dictionary))
			{
				dictionary = new Dictionary<long, AckInfo>();
				this.ackDict[this.curAckDictArrIdx].Add(channelID, dictionary);
			}
			AckInfo ackInfo = null;
			if (!dictionary.TryGetValue(seqNoRecved, out ackInfo))
			{
				ackInfo = UdpPoolBase<AckInfo>.Alloc();
				dictionary.Add(seqNoRecved, ackInfo);
			}
			ackInfo.channelID = channelID;
			ackInfo.seqNoRecved = seqNoRecved;
			ackInfo.sentTime = sentSvrTime;
		}

		protected void AlignSeqNo(int _channelId, long _seqNo)
		{
			this.channelArr[_channelId].AlignSeqNo(_seqNo);
		}

		protected int SeqnoIdx(long _seqNo)
		{
			return (int)(_seqNo % 500L);
		}

		protected long GetNewestFirstSeqno(int channelID)
		{
			ChannelInfo channelInfo = this.channelArr[channelID];
			long result;
			if (channelInfo != null)
			{
				UdpPackage[] array = this.pkgMatrix[channelInfo.channelID];
				long num = channelInfo.firstSeqNo;
				for (num = channelInfo.firstSeqNo; num < channelInfo.lastSeqNo; num += 1L)
				{
					int num2 = this.SeqnoIdx(num);
					UdpPackage udpPackage = array[num2];
					if (udpPackage == null || udpPackage.seqNo != num)
					{
						break;
					}
				}
				result = num;
			}
			else
			{
				result = 0L;
			}
			return result;
		}
	}
}
