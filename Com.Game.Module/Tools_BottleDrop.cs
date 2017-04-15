using Assets.Scripts.Model;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Com.Game.Module
{
	public static class Tools_BottleDrop
	{
		public static void ParseDetail(List<DropItemData> didList)
		{
			for (int i = 0; i < didList.Count; i++)
			{
				Tools_BottleDrop.ReturnItems(didList[i]);
			}
		}

		public static void ReturnItems(DropItemData did)
		{
			if (did == null)
			{
				return;
			}
			int itemId = did.itemId;
			int itemType = did.itemType;
			int itemCount = did.itemCount;
			DropItemBase dropItemBase = null;
			switch (itemType)
			{
			case 1:
				dropItemBase = new DropItem_Currency();
				break;
			case 2:
				dropItemBase = new DropItem_GameItem();
				break;
			case 3:
				dropItemBase = new DropItem_Unique();
				break;
			case 6:
				dropItemBase = new DropItem_GameBuff();
				break;
			}
			if (dropItemBase == null)
			{
				ClientLogger.Error("配置错误,找不到对应类型:itemtype=1，2，3，6");
			}
			else
			{
				dropItemBase.Init(did);
			}
		}

		public static void SetModelData()
		{
			DrawCardModelData drawData = ModelManager.Instance.GetDrawData();
			List<EquipmentInfoData> list = ModelManager.Instance.Get_equipmentList_X();
			List<EquipmentInfoData> list2 = new List<EquipmentInfoData>();
			list2 = drawData.equipList;
			if (list2 != null && list2.Count != 0)
			{
				foreach (EquipmentInfoData item in list2)
				{
					EquipmentInfoData equipmentInfoData = list.Find((EquipmentInfoData obj) => obj.EquipmentId == item.EquipmentId);
					if (equipmentInfoData != null)
					{
						if (equipmentInfoData.ModelId == 7777)
						{
						}
						equipmentInfoData.Count += item.Count;
					}
					else
					{
						list.Add(item);
					}
				}
			}
		}
	}
}
