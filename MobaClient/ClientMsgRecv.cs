using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;

namespace MobaClient
{
	public class ClientMsgRecv : DelegateContainer<byte>
	{
		private string debugInfo;

		public ClientMsgRecv(string _debugInfo = "")
		{
			this.debugInfo = _debugInfo;
			this.RegistCmds();
		}

		public virtual void OnResponse(OperationResponse operationResponse)
		{
			byte operationCode = operationResponse.OperationCode;
			List<object> list = new List<object>();
			byte b = 0;
			while ((int)b < operationResponse.Parameters.Count)
			{
				object item = null;
				if (operationResponse.Parameters.TryGetValue(b, out item))
				{
					list.Add(item);
				}
				b += 1;
			}
			try
			{
				base.ExecMethod(operationCode, list.ToArray(), this.debugInfo);
			}
			catch (Exception ex)
			{
				throw new ArgumentException(ex.Message, ex);
			}
		}

		public virtual void OnEvent(EventData data)
		{
			byte code = data.Code;
			List<object> list = new List<object>();
			byte b = 0;
			while ((int)b < data.Parameters.Count)
			{
				object item = null;
				if (data.Parameters.TryGetValue(b, out item))
				{
					list.Add(item);
				}
				b += 1;
			}
			try
			{
				base.ExecMethod(code, list.ToArray(), this.debugInfo);
			}
			catch (Exception ex)
			{
				throw new ArgumentException(ex.Message, ex);
			}
		}

		protected virtual void RegistCmds()
		{
		}

		public virtual void OnConnect()
		{
		}

		public virtual void OnReconnect()
		{
		}

		public virtual void OnDisconnect(StatusCode statusCode)
		{
		}
	}
}
