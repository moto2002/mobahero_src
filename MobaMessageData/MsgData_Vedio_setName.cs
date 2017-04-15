using System;

namespace MobaMessageData
{
	public class MsgData_Vedio_setName
	{
		public int _playerID;

		public string _name;

		public MsgData_Vedio_setName(int playerID, string name = "")
		{
			this._playerID = playerID;
			this._name = name;
		}
	}
}
