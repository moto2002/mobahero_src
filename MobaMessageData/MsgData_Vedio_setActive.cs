using System;

namespace MobaMessageData
{
	public class MsgData_Vedio_setActive
	{
		public int _playerID;

		public bool _active;

		public MsgData_Vedio_setActive(int playerID, bool active = true)
		{
			this._playerID = playerID;
			this._active = active;
		}
	}
}
