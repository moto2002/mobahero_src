using Assets.Scripts.GUILogic.View.BattleEquipment;
using Com.Game.Utils;
using MobaMessageData;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;

public class HeroEquipPackage : StaticUnitComponent
{
	public List<ItemDynData> EquipList
	{
		get;
		private set;
	}

	public HeroEquipPackage()
	{
		this.EquipList = new List<ItemDynData>();
	}

	public string GetModelId(int uniqueId, int itemOid)
	{
		ItemDynData itemDynData = this.EquipList.Find((ItemDynData obj) => obj.itemOid == itemOid);
		if (itemDynData == null)
		{
			ClientLogger.Error("数据中没有这个装备的信息itemOid" + itemOid);
			return null;
		}
		return itemDynData.typeId;
	}

	public int GetItemOid(string typeId)
	{
		ItemDynData itemDynData = this.EquipList.Find((ItemDynData obj) => obj.typeId == typeId);
		if (itemDynData == null)
		{
			ClientLogger.Error("数据中没有这个装备的信息typeId" + typeId);
			return 0;
		}
		return itemDynData.itemOid;
	}

	public void SetEquipList(List<ItemDynData> items)
	{
		items = (items ?? new List<ItemDynData>());
		this.EquipList.Clear();
		this.EquipList.AddRange(items);
		this.ApplyChange();
	}

	public void ApplyChange()
	{
		List<ItemDynData> equipList = this.EquipList;
		IEnumerable<string> source = from x in equipList
		select x.typeId;
		List<string> equips = source.ToList<string>();
		BattleEquipTools_op.ApplyEquipsToHero(equips, (Hero)this.self);
		this.self.SetItemSkill(this.EquipList);
		HeroItemsChangedData msgParam = new HeroItemsChangedData(this.self.unique_id, equipList);
		MobaMessageManager.DispatchMsg(MobaMessageManager.GetMessage((ClientMsg)25043, msgParam, 0f));
	}

	public void RemoveEquip(int itemId)
	{
		this.EquipList.RemoveAll((ItemDynData x) => x.itemOid == itemId);
		this.ApplyChange();
	}

	public void AddEquip(ItemDynData item)
	{
		this.EquipList.Add(item);
		this.ApplyChange();
	}

	public void UpdateEquip(ItemDynData item)
	{
		this.EquipList.RemoveAll((ItemDynData x) => x.itemOid == item.itemOid);
		this.EquipList.Add(item);
		this.ApplyChange();
	}
}
