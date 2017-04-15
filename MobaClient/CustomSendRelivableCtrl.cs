using ExitGames.Client.Photon;
using MobaProtocol;
using System;
using System.Collections.Generic;

namespace MobaClient
{
	public class CustomSendRelivableCtrl
	{
		public PhotonClient client;

		public MobaPeer pvpPeer;

		private Dictionary<int, CustomClientReliableChannel> channelDict = new Dictionary<int, CustomClientReliableChannel>();

		private List<CustomClientReliableChannel> channelPool = new List<CustomClientReliableChannel>();

		private List<ACKInfo> mBackACKInfo = new List<ACKInfo>();

		private long lastPing = 0L;

		public CustomSendRelivableCtrl(PhotonClient _client, MobaPeer _peer)
		{
			this.client = _client;
			this.pvpPeer = _peer;
		}

		public long GetPing()
		{
			long num = 0L;
			int num2 = 0;
			lock (this.channelDict)
			{
				IEnumerator<KeyValuePair<int, CustomClientReliableChannel>> enumerator = this.channelDict.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, CustomClientReliableChannel> current = enumerator.Current;
					long ping = current.Value.GetPing();
					if (ping > 0L)
					{
						num += ping;
						num2++;
					}
				}
			}
			long result;
			if (num2 > 0)
			{
				this.lastPing = num;
				result = num;
			}
			else
			{
				result = this.lastPing;
			}
			return result;
		}

		public bool Send(MobaPeer _pvpPeer, PvpCode code, Dictionary<byte, object> param, int channelID = 0)
		{
			bool result;
			lock (this.channelDict)
			{
				CustomClientReliableChannel channel = this.GetChannel(channelID);
				result = channel.Send(code, param, false);
			}
			return result;
		}

		public void OnUpdate()
		{
			lock (this.channelDict)
			{
				IEnumerator<KeyValuePair<int, CustomClientReliableChannel>> enumerator = this.channelDict.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, CustomClientReliableChannel> current = enumerator.Current;
					CustomClientReliableChannel value = current.Value;
					if (value.needDestroy && DateTime.Now.Ticks > value.destroyTime)
					{
						Dictionary<int, CustomClientReliableChannel> arg_75_0 = this.channelDict;
						current = enumerator.Current;
						arg_75_0.Remove(current.Key);
						this.channelPool.Add(value);
						break;
					}
				}
				IEnumerator<KeyValuePair<int, CustomClientReliableChannel>> enumerator2 = this.channelDict.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					KeyValuePair<int, CustomClientReliableChannel> current = enumerator2.Current;
					current.Value.OnUpdate();
				}
			}
		}

		public void OnRecv(object obj)
		{
			OperationResponse operationResponse = obj as OperationResponse;
			int channelID = (int)operationResponse.Parameters[1];
			int seqNo = (int)operationResponse.Parameters[2];
			lock (this.channelDict)
			{
				CustomClientReliableChannel channel = this.GetChannel(channelID);
				if (operationResponse.OperationCode != 240)
				{
					ACKInfo ackInfo = this.GetAckInfo();
					ackInfo.channelID = channelID;
					ackInfo.seqNo = seqNo;
					ackInfo.code = (PvpCode)operationResponse.OperationCode;
					channel.mACKInfo.Add(ackInfo);
				}
				channel.OnRecv(seqNo, operationResponse);
			}
		}

		public void SendAllAck()
		{
			lock (this.channelDict)
			{
				IEnumerator<KeyValuePair<int, CustomClientReliableChannel>> enumerator = this.channelDict.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, CustomClientReliableChannel> current = enumerator.Current;
					CustomClientReliableChannel value = current.Value;
					List<ACKInfo> mACKInfo = value.mACKInfo;
					int i = mACKInfo.Count;
					int num = i;
					int num2 = 0;
					while (i > 0)
					{
						Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
						dictionary[0] = 0;
						int num3 = 0;
						if (num > 0)
						{
							dictionary[1] = value.channelID;
							dictionary[2] = value.GetLastFatchSeqno();
						}
						for (int j = 1; j < num + 1; j++)
						{
							if (i <= 0)
							{
								break;
							}
							dictionary[(byte)(2 * j + 1)] = mACKInfo[num2].channelID;
							dictionary[(byte)(2 * j + 2)] = mACKInfo[num2].seqNo;
							i--;
							num2++;
							num3++;
						}
						if (num3 > 0)
						{
							this.pvpPeer.OpCustom(240, dictionary, false, 0);
						}
					}
					for (int j = 0; j < mACKInfo.Count; j++)
					{
						this.mBackACKInfo.Add(mACKInfo[j]);
					}
					mACKInfo.Clear();
				}
			}
		}

		private ACKInfo GetAckInfo()
		{
			ACKInfo result;
			if (this.mBackACKInfo.Count > 0)
			{
				ACKInfo aCKInfo = this.mBackACKInfo[0];
				this.mBackACKInfo.RemoveAt(0);
				result = aCKInfo;
			}
			else
			{
				result = new ACKInfo();
			}
			return result;
		}

		private void ReleaseAckInfo(ACKInfo info)
		{
			this.mBackACKInfo.Add(info);
		}

		public void OnDisconnect()
		{
			lock (this.channelDict)
			{
				IEnumerator<KeyValuePair<int, CustomClientReliableChannel>> enumerator = this.channelDict.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, CustomClientReliableChannel> current = enumerator.Current;
					current.Value.OnDisconnect();
				}
			}
		}

		public void TryProcessAvailablePkg()
		{
			lock (this.channelDict)
			{
				IEnumerator<KeyValuePair<int, CustomClientReliableChannel>> enumerator = this.channelDict.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, CustomClientReliableChannel> current = enumerator.Current;
					current.Value.TryProcessAvailableRecv();
				}
			}
		}

		private CustomClientReliableChannel GetChannel(int channelID)
		{
			CustomClientReliableChannel customClientReliableChannel = null;
			CustomClientReliableChannel result;
			if (this.channelDict.TryGetValue(channelID, out customClientReliableChannel))
			{
				result = customClientReliableChannel;
			}
			else if (this.channelPool.Count > 0)
			{
				customClientReliableChannel = this.channelPool[0];
				customClientReliableChannel.Reset();
				customClientReliableChannel.channelID = channelID;
				this.channelPool.RemoveAt(0);
				this.channelDict[channelID] = customClientReliableChannel;
				result = customClientReliableChannel;
			}
			else
			{
				customClientReliableChannel = new CustomClientReliableChannel(this.pvpPeer, this.client, channelID);
				this.channelDict[channelID] = customClientReliableChannel;
				result = customClientReliableChannel;
			}
			return result;
		}
	}
}
