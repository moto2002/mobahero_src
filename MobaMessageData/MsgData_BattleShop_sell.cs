using Assets.Scripts.Model;
using System;

namespace MobaMessageData
{
	public class MsgData_BattleShop_sell
	{
		public ItemInfo targetItem;

		public ShopInfo curShop;

		public MsgData_BattleShop_sell(ItemInfo item, ShopInfo curShopInfo)
		{
			this.targetItem = item;
			this.curShop = curShopInfo;
		}
	}
}
