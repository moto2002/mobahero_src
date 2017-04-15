using Assets.Scripts.Model;
using Com.Game.Data;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public class DynItemListToItemList : ITraversListCallback
	{
		private List<ItemInfo> list;

		public object Result
		{
			get
			{
				return this.list;
			}
		}

		public Func<object, int, bool> TraversingCallback
		{
			get
			{
				return new Func<object, int, bool>(this.callback);
			}
		}

		public DynItemListToItemList()
		{
			this.list = new List<ItemInfo>();
		}

		private bool callback(object obj, int index)
		{
			ItemDynData itemDynData = obj as ItemDynData;
			SysBattleItemsVo vo;
			if (itemDynData != null && BattleEquipTools_config.GetBattleItemVo(itemDynData.typeId, out vo))
			{
				this.list.Add(new ItemInfo(this.list.Count, itemDynData.itemOid, itemDynData.count, vo));
			}
			return true;
		}
	}
}
