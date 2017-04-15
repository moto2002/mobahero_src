using Assets.Scripts.Model;
using Com.Game.Data;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public static class BattleEquipTools_Travers
	{
		public static void TraverseList<T>(List<T> items, ITraversListCallback it)
		{
			if (items != null)
			{
				for (int i = 0; i < items.Count; i++)
				{
					if (!it.TraversingCallback(items[i], i))
					{
						break;
					}
				}
			}
		}

		private static void TraverseEquipTree(string equipID, ITraversCallback Del, int depth = 0)
		{
			if (Del != null && Del.TraversingCallback != null)
			{
				SysBattleItemsVo sysBattleItemsVo;
				if (BattleEquipTools_config.GetBattleItemVo(equipID, out sysBattleItemsVo))
				{
					if (Del.TraversingCallback(sysBattleItemsVo, depth))
					{
						string[] array = sysBattleItemsVo.consumption.Split(new char[]
						{
							','
						});
						if (array.Length != 0 && !(sysBattleItemsVo.consumption == "[]"))
						{
							for (int i = 0; i < array.Length; i++)
							{
								BattleEquipTools_Travers.TraverseEquipTree(array[i], Del, depth + 1);
							}
						}
					}
				}
			}
		}

		public static List<string> GetComposition(string equipID)
		{
			GetEquipListByLastEquip getEquipListByLastEquip = new GetEquipListByLastEquip(true, 3, null);
			BattleEquipTools_Travers.TraverseEquipTree(equipID, getEquipListByLastEquip, 0);
			return getEquipListByLastEquip.Result as List<string>;
		}

		public static List<string> GetComposition(string equipID, List<ItemInfo> givenList)
		{
			GetCompositEquipPossess getCompositEquipPossess = new GetCompositEquipPossess(equipID, givenList, 3);
			BattleEquipTools_Travers.TraverseEquipTree(equipID, getCompositEquipPossess, 0);
			return getCompositEquipPossess.Result as List<string>;
		}

		public static bool GetItem_first(List<ItemInfo> list, string target, out ItemInfo item, out int index)
		{
			item = null;
			FindItem findItem = new FindItem(target, true);
			BattleEquipTools_Travers.TraverseList<ItemInfo>(list, findItem);
			object[] array = findItem.Result as object[];
			List<ItemInfo> list2 = array[1] as List<ItemInfo>;
			index = (int)array[0];
			if (list2 != null && list2.Count > 0)
			{
				item = list2[0];
			}
			return item != null;
		}

		public static bool GetItem_last_least(List<ItemInfo> list, string target, out ItemInfo item, out int index)
		{
			item = null;
			index = -1;
			FindItem_last_least findItem_last_least = new FindItem_last_least(target);
			BattleEquipTools_Travers.TraverseList<ItemInfo>(list, findItem_last_least);
			index = (int)findItem_last_least.Result;
			if (index >= 0 && index < list.Count)
			{
				item = list[index];
			}
			return item != null && index != -1;
		}

		public static bool GetItem_first_most(List<ItemInfo> list, string target, out ItemInfo item, out int index)
		{
			item = null;
			index = -1;
			FindItem_first_most findItem_first_most = new FindItem_first_most(target);
			BattleEquipTools_Travers.TraverseList<ItemInfo>(list, findItem_first_most);
			index = (int)findItem_first_most.Result;
			if (index >= 0 && index < list.Count)
			{
				item = list[index];
			}
			return item != null && index != -1;
		}

		public static List<ItemDynData> GetDynItemList(List<ItemInfo> list)
		{
			ItemListToDynItemList itemListToDynItemList = new ItemListToDynItemList();
			BattleEquipTools_Travers.TraverseList<ItemInfo>(list, itemListToDynItemList);
			return itemListToDynItemList.Result as List<ItemDynData>;
		}

		public static List<ItemInfo> CloneItemsList(List<ItemInfo> list)
		{
			CloneItemList cloneItemList = new CloneItemList();
			BattleEquipTools_Travers.TraverseList<ItemInfo>(list, cloneItemList);
			List<ItemInfo> list2 = cloneItemList.Result as List<ItemInfo>;
			return list2 ?? new List<ItemInfo>();
		}

		public static List<string> GetItemListString(List<ItemDynData> list)
		{
			DynItemListToIDList dynItemListToIDList = new DynItemListToIDList();
			BattleEquipTools_Travers.TraverseList<ItemDynData>(list, dynItemListToIDList);
			List<string> list2 = dynItemListToIDList.Result as List<string>;
			return list2 ?? new List<string>();
		}

		public static List<string> GetItemListString(List<ItemInfo> list)
		{
			ItemListToIDList itemListToIDList = new ItemListToIDList();
			BattleEquipTools_Travers.TraverseList<ItemInfo>(list, itemListToIDList);
			List<string> list2 = itemListToIDList.Result as List<string>;
			return list2 ?? new List<string>();
		}

		public static List<ItemInfo> GetItemList(List<ItemDynData> list)
		{
			DynItemListToItemList dynItemListToItemList = new DynItemListToItemList();
			BattleEquipTools_Travers.TraverseList<ItemDynData>(list, dynItemListToItemList);
			List<ItemInfo> list2 = dynItemListToItemList.Result as List<ItemInfo>;
			return list2 ?? new List<ItemInfo>();
		}
	}
}
