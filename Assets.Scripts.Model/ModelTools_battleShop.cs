using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public static class ModelTools_battleShop
	{
		private static BattleShopData Get_BattleShopData(this ModelManager mmng)
		{
			BattleShopData battleShopData = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_battleShop))
			{
				battleShopData = mmng.GetData<BattleShopData>(EModelType.Model_battleShop);
			}
			return battleShopData ?? new BattleShopData();
		}

		public static BattleEquipType Get_BattleShop_curMenu(this ModelManager mmng)
		{
			BattleShopData battleShopData = mmng.Get_BattleShopData();
			return battleShopData.CurMenu;
		}

		public static IBEItem Get_BattleShop_curTItem(this ModelManager mmng)
		{
			BattleShopData battleShopData = mmng.Get_BattleShopData();
			return battleShopData.CurTItem;
		}

		public static SItemData Get_BattleShop_curSItem(this ModelManager mmng)
		{
			BattleShopData battleShopData = mmng.Get_BattleShopData();
			return battleShopData.CurSItem;
		}

		public static SItemData Get_BattleShop_preSItem(this ModelManager mng)
		{
			BattleShopData battleShopData = mng.Get_BattleShopData();
			return battleShopData.PreSItem;
		}

		public static ItemInfo Get_BattleShop_curPItem(this ModelManager mmng)
		{
			BattleShopData battleShopData = mmng.Get_BattleShopData();
			return battleShopData.CurPItem;
		}

		public static Dictionary<ColumnType, Dictionary<string, SItemData>> Get_BattleShop_shopitems(this ModelManager mmng)
		{
			BattleShopData battleShopData = mmng.Get_BattleShopData();
			return battleShopData.SItems;
		}

		public static List<ItemInfo> Get_BattleShop_pItems(this ModelManager mmng)
		{
			BattleShopData battleShopData = mmng.Get_BattleShopData();
			return battleShopData.PItems;
		}

		public static List<string> Get_BattleShop_rItems(this ModelManager mmng)
		{
			BattleShopData battleShopData = mmng.Get_BattleShopData();
			return battleShopData.RItems;
		}

		public static List<List<SItemData>>[] Get_BattleShop_sections(this ModelManager mmng)
		{
			BattleShopData battleShopData = mmng.Get_BattleShopData();
			return battleShopData.Sections;
		}

		public static int Get_BattleShop_money(this ModelManager mmng)
		{
			BattleShopData battleShopData = mmng.Get_BattleShopData();
			return battleShopData.Money;
		}

		public static ShopInfo Get_BattleShop_openShop(this ModelManager mmng)
		{
			BattleShopData battleShopData = mmng.Get_BattleShopData();
			return battleShopData.OpenShop;
		}

		public static ShopInfo Get_BattleShop_shopInfo(this ModelManager mmng, EBattleShopType type)
		{
			ShopInfo result = null;
			Dictionary<EBattleShopType, ShopInfo> dictionary = mmng.Get_BattleShop_shops();
			if (dictionary.ContainsKey(type))
			{
				result = dictionary[type];
			}
			return result;
		}

		public static Dictionary<EBattleShopType, ShopInfo> Get_BattleShop_shops(this ModelManager mmng)
		{
			BattleShopData battleShopData = mmng.Get_BattleShopData();
			return battleShopData.DicShops;
		}

		public static bool Get_BattleShop_playerAlive(this ModelManager mmng)
		{
			BattleShopData battleShopData = mmng.Get_BattleShopData();
			return battleShopData.PlayerAlive;
		}

		public static bool Get_BattleShop_brawlCanBuy(this ModelManager mmng)
		{
			BattleShopData battleShopData = mmng.Get_BattleShopData();
			return battleShopData.Brawl_canBuy;
		}

		public static bool Get_BattleShop_initRItems(this ModelManager mmng)
		{
			BattleShopData battleShopData = mmng.Get_BattleShopData();
			return battleShopData.B_initRItems;
		}
	}
}
