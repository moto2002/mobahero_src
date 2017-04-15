using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Tools_Rewards
{
	public static string GetDropItemTypeName(this ToolsFacade facade, Com.Game.Module.ItemType targetType)
	{
		switch (targetType)
		{
		case Com.Game.Module.ItemType.Rune:
			return LanguageManager.Instance.GetStringById("BattleSettlement_Runes");
		case Com.Game.Module.ItemType.Diamond:
			return LanguageManager.Instance.GetStringById("Currency_Diamond");
		case Com.Game.Module.ItemType.Cap:
			return LanguageManager.Instance.GetStringById("Currency_MagicBottle");
		case Com.Game.Module.ItemType.HeadPortrait:
			return LanguageManager.Instance.GetStringById("BattleSettlement_HeadAvatar");
		case Com.Game.Module.ItemType.Hero:
			return LanguageManager.Instance.GetStringById("BattleSettlement_Hero");
		case Com.Game.Module.ItemType.HeroSkin:
			return LanguageManager.Instance.GetStringById("BattleSettlement_Skin");
		case Com.Game.Module.ItemType.Coin:
			return LanguageManager.Instance.GetStringById("Currency_Gold");
		case Com.Game.Module.ItemType.Bottle:
			return LanguageManager.Instance.GetStringById("GameItems_Name_7777");
		case Com.Game.Module.ItemType.Exp:
			return "经验值";
		case Com.Game.Module.ItemType.NormalGameItem:
			return LanguageManager.Instance.GetStringById("Currency_Items");
		case Com.Game.Module.ItemType.Coupon:
			return "打折卡";
		case Com.Game.Module.ItemType.PortraitFrame:
			return LanguageManager.Instance.GetStringById("Currency_PictureFrame");
		case Com.Game.Module.ItemType.GameBuff:
			return "召唤师增益卡";
		case Com.Game.Module.ItemType.Speaker:
			return LanguageManager.Instance.GetStringById("Currency_Horn");
		}
		return string.Empty;
	}

	public static Com.Game.Module.ItemType GetDropItemType(this ToolsFacade facade, DropItemData dropItem)
	{
		switch (dropItem.itemType)
		{
		case 1:
			if (dropItem.itemId == 1)
			{
				return Com.Game.Module.ItemType.Coin;
			}
			if (dropItem.itemId == 2)
			{
				return Com.Game.Module.ItemType.Diamond;
			}
			if (dropItem.itemId == 9)
			{
				return Com.Game.Module.ItemType.Cap;
			}
			if (dropItem.itemId == 11)
			{
				return Com.Game.Module.ItemType.Speaker;
			}
			return Com.Game.Module.ItemType.None;
		case 2:
		{
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(dropItem.itemId.ToString());
			if (dropItem.itemId == 7777)
			{
				return Com.Game.Module.ItemType.Bottle;
			}
			if (dataById != null && dataById.type == 4)
			{
				return Com.Game.Module.ItemType.Rune;
			}
			return Com.Game.Module.ItemType.NormalGameItem;
		}
		case 3:
			if (dropItem.itemId == 1)
			{
				return Com.Game.Module.ItemType.Hero;
			}
			if (dropItem.itemId == 2)
			{
				return Com.Game.Module.ItemType.HeroSkin;
			}
			if (dropItem.itemId == 3)
			{
				return Com.Game.Module.ItemType.HeadPortrait;
			}
			if (dropItem.itemId == 4)
			{
				return Com.Game.Module.ItemType.PortraitFrame;
			}
			if (dropItem.itemId == 5)
			{
				return Com.Game.Module.ItemType.Coupon;
			}
			return Com.Game.Module.ItemType.None;
		case 4:
			if (dropItem.itemId == 1)
			{
				return Com.Game.Module.ItemType.Exp;
			}
			return Com.Game.Module.ItemType.None;
		case 6:
			return Com.Game.Module.ItemType.GameBuff;
		}
		return Com.Game.Module.ItemType.None;
	}

	public static Com.Game.Module.ItemType AnalyzeDropItemById(this ToolsFacade facade, string _dropItemId, out int id, out int count)
	{
		SysDropItemsVo dataById = BaseDataMgr.instance.GetDataById<SysDropItemsVo>(_dropItemId);
		Com.Game.Module.ItemType result = Com.Game.Module.ItemType.None;
		id = 0;
		count = 0;
		if (dataById == null)
		{
			Debug.LogError("Tools_Rewards: 不可处理的空掉落物品" + _dropItemId);
			return result;
		}
		return facade.AnalyzeDropItem(dataById.rewards, out id, out count);
	}

	public static Com.Game.Module.ItemType AnalyzeDropItem(this ToolsFacade facade, string _dropItemStr, out int id, out int count)
	{
		Com.Game.Module.ItemType result = Com.Game.Module.ItemType.None;
		id = 0;
		count = 0;
		if (string.IsNullOrEmpty(_dropItemStr))
		{
			Debug.LogError("Tools_Rewards: 不可处理的空字符串");
			return result;
		}
		string[] array = _dropItemStr.Split(new char[]
		{
			'|'
		});
		if (array == null || array.Length != 3)
		{
			Debug.LogError("Tools_Rewards: 不合规则的错字符串" + _dropItemStr);
			return result;
		}
		int num = int.Parse(array[0]);
		int num2 = int.Parse(array[1]);
		int num3 = int.Parse(array[2]);
		switch (num)
		{
		case 1:
			id = num2;
			count = num3;
			if (num2 == 1)
			{
				result = Com.Game.Module.ItemType.Coin;
			}
			else if (num2 == 2)
			{
				result = Com.Game.Module.ItemType.Diamond;
			}
			else if (num2 == 9)
			{
				result = Com.Game.Module.ItemType.Cap;
			}
			else if (num2 == 11)
			{
				result = Com.Game.Module.ItemType.Speaker;
			}
			break;
		case 2:
		{
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(id.ToString());
			id = num2;
			count = num3;
			if (id == 7777)
			{
				result = Com.Game.Module.ItemType.Bottle;
			}
			else if (dataById != null && dataById.type == 4)
			{
				result = Com.Game.Module.ItemType.Rune;
			}
			else
			{
				result = Com.Game.Module.ItemType.NormalGameItem;
			}
			break;
		}
		case 3:
			count = 1;
			id = num3;
			if (num2 == 1)
			{
				result = Com.Game.Module.ItemType.Hero;
			}
			else if (num2 == 2)
			{
				result = Com.Game.Module.ItemType.HeroSkin;
			}
			else if (num2 == 3)
			{
				result = Com.Game.Module.ItemType.HeadPortrait;
			}
			else if (num2 == 4)
			{
				result = Com.Game.Module.ItemType.PortraitFrame;
			}
			else if (num2 == 5)
			{
				result = Com.Game.Module.ItemType.Coupon;
			}
			break;
		case 4:
			count = num3;
			id = 0;
			if (num2 == 1)
			{
				result = Com.Game.Module.ItemType.Exp;
			}
			break;
		case 6:
			count = 1;
			id = num2;
			result = Com.Game.Module.ItemType.GameBuff;
			break;
		}
		return result;
	}

	public static string[] AnalyseDropRewardsPackage(this ToolsFacade facade, string _packageId)
	{
		string[] result = null;
		if (string.IsNullOrEmpty(_packageId))
		{
			Debug.LogError("Tools_Rewards: 不可处理的空奖励包ID");
			return result;
		}
		SysDropRewardsVo dataById = BaseDataMgr.instance.GetDataById<SysDropRewardsVo>(_packageId);
		if (dataById == null || dataById.drop_items.Equals("[]"))
		{
			return result;
		}
		return dataById.drop_items.Split(new char[]
		{
			','
		});
	}

	public static void GetRewards_WriteInModels(this ToolsFacade facade, List<EquipmentInfoData> listEquip, List<HeroInfoData> listHero, List<DropItemData> listDropItem, List<DropItemData> listRepeatItem, Callback _onFinish = null)
	{
		if (listDropItem == null || listEquip == null || listHero == null || (listDropItem.Count == 0 && listEquip.Count == 0 && listHero.Count == 0))
		{
			ClientLogger.Error("Tools_Rewards: Arguments Illegal");
		}
		CtrlManager.OpenWindow(WindowID.GetItemsView, null);
		Singleton<GetItemsView>.Instance.onFinish = _onFinish;
		UserData userData = ModelManager.Instance.Get_userData_X();
		bool flag = false;
		DropItemData[] array = (from obj in listDropItem
		where obj.itemType == 1 && obj.itemId == 1
		select obj).ToArray<DropItemData>();
		DropItemData[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			DropItemData dropItemData = array2[i];
			userData.Money += (long)dropItemData.itemCount;
			MobaMessageManagerTools.GetItems_Coin(dropItemData.itemCount);
		}
		array = (from obj in listDropItem
		where obj.itemType == 1 && obj.itemId == 2
		select obj).ToArray<DropItemData>();
		DropItemData[] array3 = array;
		for (int j = 0; j < array3.Length; j++)
		{
			DropItemData dropItemData2 = array3[j];
			userData.Diamonds += (long)dropItemData2.itemCount;
			MobaMessageManagerTools.GetItems_Diamond(dropItemData2.itemCount);
		}
		array = (from obj in listDropItem
		where obj.itemType == 1 && obj.itemId == 9
		select obj).ToArray<DropItemData>();
		DropItemData[] array4 = array;
		for (int k = 0; k < array4.Length; k++)
		{
			DropItemData dropItemData3 = array4[k];
			userData.SmallCap += dropItemData3.itemCount;
			MobaMessageManagerTools.GetItems_Caps(dropItemData3.itemCount);
		}
		array = (from obj in listDropItem
		where obj.itemType == 1 && obj.itemId == 11
		select obj).ToArray<DropItemData>();
		DropItemData[] array5 = array;
		for (int l = 0; l < array5.Length; l++)
		{
			DropItemData dropItemData4 = array5[l];
			userData.Speaker += dropItemData4.itemCount;
			MobaMessageManagerTools.GetItems_Speaker(dropItemData4.itemCount);
		}
		EquipmentInfoData[] array6 = listEquip.ToArray();
		EquipmentInfoData[] array7 = array6;
		EquipmentInfoData gameItem;
		for (int m = 0; m < array7.Length; m++)
		{
			gameItem = array7[m];
			if (ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.EquipmentId == gameItem.EquipmentId) == null)
			{
				ModelManager.Instance.Get_equipmentList_X().Add(gameItem);
			}
			else
			{
				ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.EquipmentId == gameItem.EquipmentId).Count += gameItem.Count;
			}
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(gameItem.ModelId.ToString());
			if (gameItem.ModelId == 7777)
			{
				MobaMessageManagerTools.GetItems_Bottle(gameItem.Count);
				Singleton<MenuView>.Instance.UpdateBottleNum();
			}
			else if (dataById.type == 4)
			{
				MobaMessageManagerTools.GetItems_Rune(gameItem.ModelId);
			}
			else
			{
				MobaMessageManagerTools.GetItems_GameItem(gameItem.ModelId.ToString());
			}
		}
		HeroInfoData[] array8 = listHero.ToArray();
		if (array8 != null)
		{
			for (int n = 0; n < array8.Length; n++)
			{
				for (int num = 0; num < listRepeatItem.Count; num++)
				{
					SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(array8[n].ModelId);
					if (listRepeatItem[num].itemType == 3 && listRepeatItem[num].itemId == 1 && listRepeatItem[num].itemCount == heroMainData.hero_id)
					{
						MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.Hero, heroMainData.model_id, true);
						flag = true;
					}
				}
				if (flag)
				{
					flag = false;
				}
				else
				{
					if (!ModelManager.Instance.Get_heroInfo_list_X().Contains(array8[n]))
					{
						ModelManager.Instance.Get_heroInfo_list_X().Add(array8[n]);
					}
					MobaMessageManagerTools.GetItems_Hero(array8[n].ModelId);
				}
			}
			CharacterDataMgr.instance.UpdateHerosData();
			Singleton<MenuView>.Instance.CheckHeroState();
		}
		array = (from obj in listDropItem
		where obj.itemType == 3 && obj.itemId == 2
		select obj).ToArray<DropItemData>();
		DropItemData[] array9 = array;
		for (int num2 = 0; num2 < array9.Length; num2++)
		{
			DropItemData dropItemData5 = array9[num2];
			for (int num3 = 0; num3 < listRepeatItem.Count; num3++)
			{
				if (listRepeatItem[num3].itemType == 3 && listRepeatItem[num3].itemId == 2 && listRepeatItem[num3].itemCount == dropItemData5.itemCount)
				{
					MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.HeroSkin, dropItemData5.itemCount.ToString(), true);
					flag = true;
				}
			}
			if (flag)
			{
				flag = false;
			}
			else
			{
				ModelManager.Instance.GetNewHeroSkin(dropItemData5.itemCount);
				MobaMessageManagerTools.GetItems_HeroSkin(dropItemData5.itemCount);
			}
		}
		array = (from obj in listDropItem
		where obj.itemType == 3 && obj.itemId == 3
		select obj).ToArray<DropItemData>();
		DropItemData[] array10 = array;
		for (int num4 = 0; num4 < array10.Length; num4++)
		{
			DropItemData dropItemData6 = array10[num4];
			for (int num5 = 0; num5 < listRepeatItem.Count; num5++)
			{
				if (listRepeatItem[num5].itemType == 3 && listRepeatItem[num5].itemId == 3 && listRepeatItem[num5].itemCount == dropItemData6.itemCount)
				{
					MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.HeadPortrait, dropItemData6.itemCount.ToString(), true);
					flag = true;
				}
			}
			if (flag)
			{
				flag = false;
			}
			else
			{
				ModelManager.Instance.GetNewAvatar("3", dropItemData6.itemCount.ToString());
				MobaMessageManagerTools.GetItems_HeadPortrait(dropItemData6.itemCount);
			}
		}
		array = (from obj in listDropItem
		where obj.itemType == 3 && obj.itemId == 4
		select obj).ToArray<DropItemData>();
		DropItemData[] array11 = array;
		for (int num6 = 0; num6 < array11.Length; num6++)
		{
			DropItemData dropItemData7 = array11[num6];
			for (int num7 = 0; num7 < listRepeatItem.Count; num7++)
			{
				if (listRepeatItem[num7].itemType == 3 && listRepeatItem[num7].itemId == 4 && listRepeatItem[num7].itemCount == dropItemData7.itemCount)
				{
					MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.PortraitFrame, dropItemData7.itemCount.ToString(), true);
					flag = true;
				}
			}
			if (flag)
			{
				flag = false;
			}
			else
			{
				ModelManager.Instance.GetNewAvatar("4", dropItemData7.itemCount.ToString());
				MobaMessageManagerTools.GetItems_PortraitFrame(dropItemData7.itemCount.ToString());
			}
		}
		array = (from obj in listDropItem
		where obj.itemType == 3 && obj.itemId == 5
		select obj).ToArray<DropItemData>();
		DropItemData[] array12 = array;
		for (int num8 = 0; num8 < array12.Length; num8++)
		{
			DropItemData dropItemData8 = array12[num8];
			for (int num9 = 0; num9 < listRepeatItem.Count; num9++)
			{
				if (listRepeatItem[num9].itemType == 3 && listRepeatItem[num9].itemId == 5 && listRepeatItem[num9].itemCount == dropItemData8.itemCount)
				{
					MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.Coupon, dropItemData8.itemCount.ToString(), true);
					flag = true;
				}
			}
			if (flag)
			{
				flag = false;
			}
			else
			{
				ModelManager.Instance.GetNewCoupon(dropItemData8.itemCount.ToString());
				MobaMessageManagerTools.GetItems_Coupon(dropItemData8.itemCount.ToString());
			}
		}
		array = (from obj in listDropItem
		where obj.itemType == 4 && obj.itemId == 1
		select obj).ToArray<DropItemData>();
		DropItemData[] array13 = array;
		for (int num10 = 0; num10 < array13.Length; num10++)
		{
			DropItemData dropItemData9 = array13[num10];
			MobaMessageManagerTools.GetItems_Exp(dropItemData9.itemCount, userData.Exp);
			userData.Exp += (long)dropItemData9.itemCount;
			CharacterDataMgr.instance.SaveNowUserLevel(userData.Exp);
		}
		array = (from obj in listDropItem
		where obj.itemType == 6
		select obj).ToArray<DropItemData>();
		DropItemData[] array14 = array;
		for (int num11 = 0; num11 < array14.Length; num11++)
		{
			DropItemData dropItemData10 = array14[num11];
			MobaMessageManagerTools.GetItems_GameBuff(dropItemData10.itemId);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetDoubleCard, null, new object[0]);
		}
		Singleton<MenuTopBarView>.Instance.RefreshUI();
		Singleton<GetItemsView>.Instance.Play();
	}

	public static void GetRewards_WriteInModels_WithoutShowEffect(this ToolsFacade facade, List<EquipmentInfoData> listEquip, List<HeroInfoData> listHero, List<DropItemData> listDropItem, List<DropItemData> listRepeatItem, Callback _onFinish = null)
	{
		if (listDropItem == null || listEquip == null || listHero == null || (listDropItem.Count == 0 && listEquip.Count == 0 && listHero.Count == 0))
		{
			ClientLogger.Error("Tools_Rewards: Arguments Illegal");
		}
		UserData userData = ModelManager.Instance.Get_userData_X();
		bool flag = false;
		DropItemData[] array = (from obj in listDropItem
		where obj.itemType == 1 && obj.itemId == 1
		select obj).ToArray<DropItemData>();
		DropItemData[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			DropItemData dropItemData = array2[i];
			userData.Money += (long)dropItemData.itemCount;
		}
		array = (from obj in listDropItem
		where obj.itemType == 1 && obj.itemId == 2
		select obj).ToArray<DropItemData>();
		DropItemData[] array3 = array;
		for (int j = 0; j < array3.Length; j++)
		{
			DropItemData dropItemData2 = array3[j];
			userData.Diamonds += (long)dropItemData2.itemCount;
		}
		array = (from obj in listDropItem
		where obj.itemType == 1 && obj.itemId == 9
		select obj).ToArray<DropItemData>();
		DropItemData[] array4 = array;
		for (int k = 0; k < array4.Length; k++)
		{
			DropItemData dropItemData3 = array4[k];
			userData.SmallCap += dropItemData3.itemCount;
		}
		array = (from obj in listDropItem
		where obj.itemType == 1 && obj.itemId == 11
		select obj).ToArray<DropItemData>();
		DropItemData[] array5 = array;
		for (int l = 0; l < array5.Length; l++)
		{
			DropItemData dropItemData4 = array5[l];
			userData.Speaker += dropItemData4.itemCount;
		}
		EquipmentInfoData[] array6 = listEquip.ToArray();
		EquipmentInfoData[] array7 = array6;
		EquipmentInfoData gameItem;
		for (int m = 0; m < array7.Length; m++)
		{
			gameItem = array7[m];
			if (ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.EquipmentId == gameItem.EquipmentId) == null)
			{
				ModelManager.Instance.Get_equipmentList_X().Add(gameItem);
			}
			else
			{
				ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.EquipmentId == gameItem.EquipmentId).Count += gameItem.Count;
			}
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(gameItem.ModelId.ToString());
			if (gameItem.ModelId == 7777)
			{
				Singleton<MenuView>.Instance.UpdateBottleNum();
			}
		}
		HeroInfoData[] array8 = listHero.ToArray();
		if (array8 != null)
		{
			for (int n = 0; n < array8.Length; n++)
			{
				for (int num = 0; num < listRepeatItem.Count; num++)
				{
					SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(array8[n].ModelId);
					if (listRepeatItem[num].itemType == 3 && listRepeatItem[num].itemId == 1 && listRepeatItem[num].itemCount == heroMainData.hero_id)
					{
						GetItemsView.ExchangeItemData exchangeItemData = new GetItemsView.ExchangeItemData(Com.Game.Module.ItemType.Hero, listRepeatItem[num].itemCount.ToString(), true);
						flag = true;
					}
				}
				if (flag)
				{
					flag = false;
				}
				else if (!ModelManager.Instance.Get_heroInfo_list_X().Contains(array8[n]))
				{
					ModelManager.Instance.Get_heroInfo_list_X().Add(array8[n]);
				}
			}
			CharacterDataMgr.instance.UpdateHerosData();
			Singleton<MenuView>.Instance.CheckHeroState();
		}
		array = (from obj in listDropItem
		where obj.itemType == 3 && obj.itemId == 2
		select obj).ToArray<DropItemData>();
		DropItemData[] array9 = array;
		for (int num2 = 0; num2 < array9.Length; num2++)
		{
			DropItemData dropItemData5 = array9[num2];
			for (int num3 = 0; num3 < listRepeatItem.Count; num3++)
			{
				if (listRepeatItem[num3].itemType == 3 && listRepeatItem[num3].itemId == 2 && listRepeatItem[num3].itemCount == dropItemData5.itemCount)
				{
					GetItemsView.ExchangeItemData exchangeItemData2 = new GetItemsView.ExchangeItemData(Com.Game.Module.ItemType.HeroSkin, dropItemData5.itemCount.ToString(), true);
					flag = true;
				}
			}
			if (flag)
			{
				flag = false;
			}
			else
			{
				ModelManager.Instance.GetNewHeroSkin(dropItemData5.itemCount);
			}
		}
		array = (from obj in listDropItem
		where obj.itemType == 3 && obj.itemId == 3
		select obj).ToArray<DropItemData>();
		DropItemData[] array10 = array;
		for (int num4 = 0; num4 < array10.Length; num4++)
		{
			DropItemData dropItemData6 = array10[num4];
			for (int num5 = 0; num5 < listRepeatItem.Count; num5++)
			{
				if (listRepeatItem[num5].itemType == 3 && listRepeatItem[num5].itemId == 3 && listRepeatItem[num5].itemCount == dropItemData6.itemCount)
				{
					GetItemsView.ExchangeItemData exchangeItemData3 = new GetItemsView.ExchangeItemData(Com.Game.Module.ItemType.HeadPortrait, dropItemData6.itemCount.ToString(), true);
					flag = true;
				}
			}
			if (flag)
			{
				flag = false;
			}
			else
			{
				ModelManager.Instance.GetNewAvatar("3", dropItemData6.itemCount.ToString());
			}
		}
		array = (from obj in listDropItem
		where obj.itemType == 3 && obj.itemId == 4
		select obj).ToArray<DropItemData>();
		DropItemData[] array11 = array;
		for (int num6 = 0; num6 < array11.Length; num6++)
		{
			DropItemData dropItemData7 = array11[num6];
			for (int num7 = 0; num7 < listRepeatItem.Count; num7++)
			{
				if (listRepeatItem[num7].itemType == 3 && listRepeatItem[num7].itemId == 4 && listRepeatItem[num7].itemCount == dropItemData7.itemCount)
				{
					GetItemsView.ExchangeItemData exchangeItemData4 = new GetItemsView.ExchangeItemData(Com.Game.Module.ItemType.PortraitFrame, dropItemData7.itemCount.ToString(), true);
					flag = true;
				}
			}
			if (flag)
			{
				flag = false;
			}
			else
			{
				ModelManager.Instance.GetNewAvatar("4", dropItemData7.itemCount.ToString());
			}
		}
		array = (from obj in listDropItem
		where obj.itemType == 3 && obj.itemId == 5
		select obj).ToArray<DropItemData>();
		DropItemData[] array12 = array;
		for (int num8 = 0; num8 < array12.Length; num8++)
		{
			DropItemData dropItemData8 = array12[num8];
			for (int num9 = 0; num9 < listRepeatItem.Count; num9++)
			{
				if (listRepeatItem[num9].itemType == 3 && listRepeatItem[num9].itemId == 5 && listRepeatItem[num9].itemCount == dropItemData8.itemCount)
				{
					GetItemsView.ExchangeItemData exchangeItemData5 = new GetItemsView.ExchangeItemData(Com.Game.Module.ItemType.Coupon, dropItemData8.itemCount.ToString(), true);
					flag = true;
				}
			}
			if (flag)
			{
				flag = false;
			}
			else
			{
				ModelManager.Instance.GetNewCoupon(dropItemData8.itemCount.ToString());
			}
		}
		array = (from obj in listDropItem
		where obj.itemType == 4 && obj.itemId == 1
		select obj).ToArray<DropItemData>();
		DropItemData[] array13 = array;
		for (int num10 = 0; num10 < array13.Length; num10++)
		{
			DropItemData dropItemData9 = array13[num10];
			userData.Exp += (long)dropItemData9.itemCount;
			CharacterDataMgr.instance.SaveNowUserLevel(userData.Exp);
		}
		array = (from obj in listDropItem
		where obj.itemType == 6
		select obj).ToArray<DropItemData>();
		DropItemData[] array14 = array;
		for (int num11 = 0; num11 < array14.Length; num11++)
		{
			DropItemData dropItemData10 = array14[num11];
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetDoubleCard, null, new object[0]);
		}
		Singleton<MenuTopBarView>.Instance.RefreshUI();
	}
}
