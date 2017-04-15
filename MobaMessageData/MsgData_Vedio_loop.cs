using System;

namespace MobaMessageData
{
	public class MsgData_Vedio_loop
	{
		public int _playerID;

		public bool _loop;

		public MsgData_Vedio_loop(int playerID, bool loop)
		{
			this._playerID = playerID;
			this._loop = loop;
		}
	}
}
