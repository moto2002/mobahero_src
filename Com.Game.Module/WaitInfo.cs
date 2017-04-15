using MobaMessageData;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	internal class WaitInfo
	{
		private int _waitNum;

		private float _initTime;

		private float _waitTime;

		private MobaMessageType _type;

		private int _code;

		private string _key;

		public WaitInfo(MobaMessageType type, int code, string key, float t)
		{
			this._type = type;
			this._code = code;
			this._key = key;
			this._initTime = t;
			this.Add();
			this.ResetStartTime();
			MobaMessageManager.RegistMessage(type, code, new MobaMessageFunc(this.OnReceiveMsg));
		}

		public void OnUpdate()
		{
			this._waitTime -= Time.deltaTime;
			if (this._waitTime <= 0f)
			{
				this._waitTime = 0f;
				this.DispatchTimeOutMsg();
				this.Clear();
			}
		}

		public void OnDestroy()
		{
			MobaMessageManager.UnRegistMessage(this._type, this._code, new MobaMessageFunc(this.OnReceiveMsg));
		}

		private void OnReceiveMsg(MobaMessage msg)
		{
			if (msg.MessageType == MobaMessageType.Client && msg.ID == 23002)
			{
				string a = msg.Param as string;
				if (a == this._key)
				{
					this.Sub();
				}
			}
			else
			{
				this.Sub();
			}
		}

		private void DispatchTimeOutMsg()
		{
			if (this._type != MobaMessageType.Client)
			{
				MsgData_WaitServerResponsTimeout msgParam = new MsgData_WaitServerResponsTimeout(this._type, this._code);
				MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)25012, msgParam, 0f);
				MobaMessageManager.DispatchMsg(message);
			}
		}

		private void ResetStartTime()
		{
			this._waitTime = this._initTime;
		}

		private void Add()
		{
			this._waitNum++;
		}

		private void Sub()
		{
			this._waitNum--;
			if (this._waitNum < 0)
			{
				this._waitNum = 0;
			}
		}

		private void Clear()
		{
			this._waitNum = 0;
		}

		private bool IsEmpty()
		{
			return 0 == this._waitNum;
		}

		public static WaitInfo operator ++(WaitInfo w)
		{
			w.Add();
			w.ResetStartTime();
			return w;
		}

		public static WaitInfo operator --(WaitInfo w)
		{
			w.Sub();
			return w;
		}

		public static bool operator false(WaitInfo w)
		{
			return w.IsEmpty();
		}

		public static bool operator true(WaitInfo w)
		{
			return !w.IsEmpty();
		}
	}
}
