using Assets.Scripts.Model;
using System;

namespace MobaMessageData
{
	public class MsgData_BattleShop_rollback
	{
		public ShopInfo curShop;

		public MsgData_BattleShop_rollback(ShopInfo curShopInfo)
		{
			this.curShop = curShopInfo;
		}
	}
}
