using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;

namespace MobaClient
{
	public class ClientResponseRecv
	{
		public delegate void NetResponseEvent(OperationResponse resp);

		protected Dictionary<byte, ClientResponseRecv.NetResponseEvent> m_eventTable = new Dictionary<byte, ClientResponseRecv.NetResponseEvent>();

		public ClientResponseRecv()
		{
			this.RegistCmds();
		}

		public bool Regist(byte code, ClientResponseRecv.NetResponseEvent func)
		{
			bool result;
			if (this.m_eventTable.ContainsKey(code))
			{
				result = false;
			}
			else
			{
				this.m_eventTable[code] = func;
				result = true;
			}
			return result;
		}

		public virtual void OnResponse(OperationResponse operationResponse)
		{
			byte operationCode = operationResponse.OperationCode;
			ClientResponseRecv.NetResponseEvent netResponseEvent;
			if (this.m_eventTable.TryGetValue(operationCode, out netResponseEvent))
			{
				try
				{
					netResponseEvent(operationResponse);
				}
				catch (Exception ex)
				{
					throw new ArgumentException(string.Concat(new object[]
					{
						"ClientResponseRecv error,code:",
						operationCode,
						"  ",
						ex.Message
					}));
				}
			}
		}

		protected virtual void RegistCmds()
		{
		}
	}
}
