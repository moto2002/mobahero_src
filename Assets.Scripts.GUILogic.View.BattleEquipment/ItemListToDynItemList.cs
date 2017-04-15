using Assets.Scripts.Model;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public class ItemListToDynItemList : ITraversListCallback
	{
		private List<ItemDynData> dynList;

		public object Result
		{
			get
			{
				return this.dynList;
			}
		}

		public Func<object, int, bool> TraversingCallback
		{
			get
			{
				return new Func<object, int, bool>(this.callback);
			}
		}

		public ItemListToDynItemList()
		{
			this.dynList = new List<ItemDynData>();
		}

		private bool callback(object obj, int index)
		{
			ItemInfo itemInfo = obj as ItemInfo;
			if (itemInfo != null)
			{
				this.dynList.Add(new ItemDynData
				{
					count = itemInfo.Num,
					typeId = itemInfo.ID,
					itemOid = itemInfo.OID
				});
			}
			return true;
		}
	}
}
