using System;

namespace MobaMessageData
{
	public class MsgData_Vedio_creatPlayer
	{
		public int _playerID;

		public bool _create;

		public MsgData_Vedio_creatPlayer(int playerID, bool create = true)
		{
			this._playerID = playerID;
			this._create = create;
		}
	}
}
