using System;

namespace Com.Game.Data
{
	[Serializable]
	public class SysShopItemVo
	{
		public string unikey;

		public int id;

		public int type;

		public string itemID;

		public int itemNum;

		public string itemPrice;

		public int MaxNumberOfPurchases;

		public int dailyPurchaseTimes;

		public int PurchaseTimes;

		public int onlyOne;

		public int weight;

		public int tag;

		public int IsSelling;

		public string StartTime;

		public string EndTime;

		public string icon;
	}
}
