using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class ShopDataGroup
	{
		public int shopVersion;

		public List<ShopDataNew> shopList = new List<ShopDataNew>();

		public List<GoodsData> goodsList = new List<GoodsData>();
	}
}
