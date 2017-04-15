using ExitGames.Client.Photon;
using System;

namespace MobaClient
{
	public class RecvCacheMgr
	{
		private const int maxRecvCacheSize = 1000;

		public int lastFetchSeqNo = 0;

		private OperationResponse[] recvArr;

		public RecvCacheMgr()
		{
			this.recvArr = new OperationResponse[1000];
			this.Reset();
		}

		public void Reset()
		{
			this.lastFetchSeqNo = 0;
			for (int i = 0; i < this.recvArr.Length; i++)
			{
				this.recvArr[i] = null;
			}
		}

		private OperationResponse Get(int idx)
		{
			return this.recvArr[idx % 1000];
		}

		public void AddRecv(int seqNo, OperationResponse _operationResponse)
		{
			if (seqNo >= this.lastFetchSeqNo)
			{
				this.recvArr[seqNo % 1000] = _operationResponse;
			}
		}

		public OperationResponse FetchReadyRecv()
		{
			OperationResponse operationResponse = this.recvArr[this.lastFetchSeqNo % 1000];
			OperationResponse result;
			if (operationResponse != null)
			{
				this.recvArr[this.lastFetchSeqNo % 1000] = null;
				this.lastFetchSeqNo++;
				result = operationResponse;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public int FetchUseSnapIndex()
		{
			int num = 4;
			int num2 = this.lastFetchSeqNo;
			while (this.recvArr[num2 % 1000] != null)
			{
				num2++;
			}
			int result;
			if (num2 <= this.lastFetchSeqNo + num)
			{
				result = 0;
			}
			else
			{
				int num3 = 0;
				for (int i = num2 - 1; i >= this.lastFetchSeqNo; i--)
				{
					if (this.recvArr[i % 1000].OperationCode == 250)
					{
						num3++;
						if (num3 >= num)
						{
							result = i - this.lastFetchSeqNo;
							return result;
						}
					}
				}
				result = 0;
			}
			return result;
		}
	}
}
