using ExitGames.Client.Photon;
using MobaProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaClient
{
	public class CustomClientReliableChannel
	{
		private PhotonClient client;

		public MobaPeer pvpPeer;

		public int channelID = -1;

		private RecvCacheMgr recvCacheMgr;

		private SendPkgMgr sendPkgMgr;

		private Dictionary<byte, object> waitingSendPkg;

		private PvpCode waitingSendCode;

		public bool needDestroy = false;

		public long destroyTime = 0L;

		public List<ACKInfo> mACKInfo = new List<ACKInfo>();

		public long ping;

		public static void log(string str)
		{
			Debug.LogError(string.Concat(new object[]
			{
				DateTime.Now.Hour,
				":",
				DateTime.Now.Minute,
				":",
				DateTime.Now.Second,
				" ",
				DateTime.Now.Millisecond,
				"   ",
				str
			}));
		}

		public static void log2(string str)
		{
		}

		public CustomClientReliableChannel(MobaPeer _peer, PhotonClient _client, int _channelID)
		{
			this.pvpPeer = _peer;
			this.client = _client;
			this.channelID = _channelID;
			this.recvCacheMgr = new RecvCacheMgr();
			this.sendPkgMgr = new SendPkgMgr(this);
			this.Reset();
		}

		public void Reset()
		{
			this.recvCacheMgr.Reset();
			this.sendPkgMgr.Reset();
			this.needDestroy = false;
			this.destroyTime = 0L;
		}

		public void OnDisconnect()
		{
			this.Reset();
		}

		public void OnConnect()
		{
		}

		public void OnUpdate()
		{
			SendPkgData firstPkgWaitingAck = this.sendPkgMgr.GetFirstPkgWaitingAck();
			if (firstPkgWaitingAck != null && DateTime.Now.Ticks - firstPkgWaitingAck.firstTimeTick > 100000000L)
			{
				this.pvpPeer.Disconnect();
			}
			else
			{
				this.sendPkgMgr.UpdatePing();
				this.sendPkgMgr.RetrySendCachePkg();
				if (this.waitingSendPkg != null && this.sendPkgMgr.GetNumberOfWaitingAck() <= 0)
				{
					this.Send(this.waitingSendCode, this.waitingSendPkg, false);
					this.waitingSendPkg = null;
				}
			}
		}

		private void OnDestroy()
		{
			this.needDestroy = true;
			this.destroyTime = DateTime.Now.Ticks + 200000000L;
		}

		public int GetLastFatchSeqno()
		{
			return this.recvCacheMgr.lastFetchSeqNo;
		}

		public long GetPing()
		{
			this.ping = this.sendPkgMgr.GetPing();
			return this.ping;
		}

		public bool OnRecv(int seqNo, OperationResponse _operationResponse)
		{
			bool result;
			if (_operationResponse.Parameters.Count > 2)
			{
				if (_operationResponse.OperationCode == 240)
				{
					this.OnAck(seqNo);
					result = true;
				}
				else if (_operationResponse.OperationCode == 241)
				{
					this.OnDestroy();
					result = true;
				}
				else
				{
					this.recvCacheMgr.AddRecv(seqNo, _operationResponse);
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool TryProcessAvailableRecv()
		{
			int num = 0;
			int num2 = 0;
			while (true)
			{
				OperationResponse operationResponse = this.recvCacheMgr.FetchReadyRecv();
				if (operationResponse == null)
				{
					break;
				}
				if (operationResponse.OperationCode == 250)
				{
					if (num2 >= num)
					{
						this.client.ExecUnrelivableCmd((PvpCode)operationResponse.OperationCode, operationResponse);
					}
				}
				else
				{
					this.client.ExecUnrelivableCmd((PvpCode)operationResponse.OperationCode, operationResponse);
				}
				num2++;
			}
			return true;
		}

		public void SendAck(int channelID, int seqNo)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[0] = 0;
			dictionary[1] = channelID;
			dictionary[2] = seqNo;
			if (!this.pvpPeer.OpCustom(240, dictionary, false, 0))
			{
			}
		}

		public bool Send(PvpCode code, Dictionary<byte, object> param, bool isResend = false)
		{
			bool result;
			if (!isResend)
			{
				param[1] = this.channelID;
				int nextSeqNo = this.sendPkgMgr.GetNextSeqNo();
				param[2] = nextSeqNo;
				param[3] = 0;
				if (this.sendPkgMgr.GetNumberOfWaitingAck() > 10)
				{
					this.waitingSendPkg = param;
					this.waitingSendCode = code;
					result = true;
					return result;
				}
				this.waitingSendPkg = null;
				this.sendPkgMgr.AddSendPkg(code, nextSeqNo, param);
			}
			bool flag = this.pvpPeer.OpCustom((byte)code, param, false, 1);
			result = flag;
			return result;
		}

		private void OnAck(int seqNo)
		{
			this.sendPkgMgr.OnRecvAck(seqNo);
		}
	}
}
