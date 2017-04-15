using System;

namespace MobaMessageData
{
	public class MsgData_WaitServerResponsTimeout
	{
		private MobaMessageType _type;

		private int _msgID;

		public MobaMessageType MobaMsgType
		{
			get
			{
				return this._type;
			}
		}

		public int MsgID
		{
			get
			{
				return this._msgID;
			}
		}

		public MsgData_WaitServerResponsTimeout(MobaMessageType type, int msgID)
		{
			this._type = type;
			this._msgID = msgID;
		}
	}
}
