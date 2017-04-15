using System;

namespace MobaMessageData
{
	public class MsgData_VedioCallback
	{
		public int playerID;

		public string vedioName;

		public MsgData_VedioCallback(int id, string name)
		{
			this.playerID = id;
			this.vedioName = name;
		}
	}
}
