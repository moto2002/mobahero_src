using Assets.Scripts.Model;
using System;

namespace MobaMessageData
{
	public class MsgData_BattelShop_setShopState
	{
		public TeamType shopType;

		public EBattleShopState shopState;

		public MsgData_BattelShop_setShopState(TeamType type, EBattleShopState state)
		{
			this.shopType = type;
			this.shopState = state;
		}
	}
}
