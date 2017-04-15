using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Module;
using MobaHeros;
using MobaHeros.Pvp;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public static class BattleEquipTools_op
	{
		public static EBattleShopContex GetBattleShopContex()
		{
			EBattleShopContex result = EBattleShopContex.ePve;
			if (LevelManager.Instance.IsPvpBattleType || LevelManager.Instance.IsServerZyBattleType)
			{
				result = EBattleShopContex.ePvp;
			}
			if (LevelManager.m_CurLevel.IsDaLuanDouPvp())
			{
				result = EBattleShopContex.eBrawl;
			}
			return result;
		}

		public static bool IsOnLineBattle()
		{
			return LevelManager.Instance.IsPvpBattleType || LevelManager.Instance.IsServerZyBattleType || LevelManager.m_CurLevel.IsDaLuanDouPvp();
		}

		public static bool IsPlayerAlive()
		{
			Units player = MapManager.Instance.GetPlayer();
			return player != null && player.isLive;
		}

		public static EBattleShopType GetShopTypeByTeamType()
		{
			EBattleShopType result;
			if (LevelManager.Instance.IsPvpBattleType || LevelManager.Instance.IsServerZyBattleType)
			{
				if (Singleton<PvpManager>.Instance.SelfTeamType == TeamType.LM)
				{
					result = EBattleShopType.eLM;
				}
				else if (Singleton<PvpManager>.Instance.SelfTeamType == TeamType.BL)
				{
					result = EBattleShopType.eBL;
				}
				else
				{
					result = EBattleShopType.eTeam3;
				}
			}
			else
			{
				result = EBattleShopType.eLM;
			}
			return result;
		}

		public static EBattleShopType GetNearestShopType()
		{
			Dictionary<EBattleShopType, ShopInfo> dictionary = ModelManager.Instance.Get_BattleShop_shops();
			List<ShopInfo> list = null;
			EBattleShopType eBattleShopType = EBattleShopType.eNone;
			if (BattleEquipTools_op.IsPlayerAlive())
			{
				list = new List<ShopInfo>();
				foreach (KeyValuePair<EBattleShopType, ShopInfo> current in dictionary)
				{
					if (current.Value.InArea)
					{
						list.Add(current.Value);
					}
				}
				if (list.Count > 0)
				{
					if (list.Count > 1)
					{
						float num = 1E+07f;
						float num2 = 0f;
						Units player = MapManager.Instance.GetPlayer();
						foreach (ShopInfo current2 in list)
						{
							BattleEquipTools_op.IsWithInShopArea(current2.Config, player, out num2);
							if (num2 < num)
							{
								num = num2;
								eBattleShopType = current2.ShopType;
							}
						}
					}
					else
					{
						eBattleShopType = list[0].ShopType;
					}
				}
			}
			if (eBattleShopType == EBattleShopType.eNone)
			{
				eBattleShopType = BattleEquipTools_op.GetShopTypeByTeamType();
			}
			return eBattleShopType;
		}

		public static bool CanOpenBattleShop(Units player, ShopInfo shopInfo, EBattleShopOpenType openType, out string err)
		{
			err = null;
			bool result = false;
			if (shopInfo != null && null != player && openType == EBattleShopOpenType.eFromButton)
			{
				switch (shopInfo.ShopType)
				{
				case EBattleShopType.eLM:
					result = (!BattleEquipTools_op.IsOnLineBattle() || Singleton<PvpManager>.Instance.SelfTeamType == TeamType.LM);
					break;
				case EBattleShopType.eBL:
					result = (BattleEquipTools_op.IsOnLineBattle() && Singleton<PvpManager>.Instance.SelfTeamType == TeamType.BL);
					break;
				case EBattleShopType.eNeutral:
				case EBattleShopType.eNeutral_2:
				{
					bool flag = null != player && BattleEquipTools_op.WithinShopArea(shopInfo, player.transform.position);
					result = flag;
					break;
				}
				case EBattleShopType.eTeam3:
					result = (Singleton<PvpManager>.Instance.SelfTeamType == TeamType.Team_3);
					break;
				}
			}
			return result;
		}

		public static bool WithinShopArea(ShopInfo shopInfo, Vector3 selfVect)
		{
			bool result = false;
			if (shopInfo != null)
			{
				Vector3 a = Vector3.zero;
				TeamType team = BattleEquipTools_config.ShopType2TeamType(shopInfo.ShopType);
				a = MapManager.Instance.GetSpawnPos(team, shopInfo.Config.shop_origin).position;
				float num = (float)shopInfo.Config.shop_range;
				result = (Vector3.Distance(a, selfVect) <= num);
			}
			return result;
		}

		public static bool IsWithInShopArea(SysBattleShopVo shopVo, Units player, out float distance)
		{
			bool result = false;
			distance = 0f;
			Vector3 a = Vector3.zero;
			float num = 0f;
			if (shopVo != null)
			{
				TeamType team = BattleEquipTools_config.ShopType2TeamType((EBattleShopType)shopVo.type);
				Transform spawnPos = MapManager.Instance.GetSpawnPos(team, shopVo.shop_origin);
				if (null != spawnPos)
				{
					a = spawnPos.position;
					num = (float)shopVo.shop_range;
				}
			}
			if (null != player)
			{
				Vector3 position = player.transform.position;
				distance = Vector3.Distance(a, position);
				result = (distance <= num);
			}
			return result;
		}

		public static void DoPvpBuy(ShopInfo shopInfo, string targetItem)
		{
			bool flag = SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_BuyItem, SerializeHelper.Serialize<ShopBuyItemInfo>(new ShopBuyItemInfo
			{
				shopId = shopInfo.ShopID,
				itemTypeId = targetItem
			}));
		}

		public static void DoPvpSell(ShopInfo shopInfo, ItemInfo targetItem, List<string> recommendItems)
		{
			bool flag = SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_SellItem, SerializeHelper.Serialize<ShopSellItemInfo>(new ShopSellItemInfo
			{
				shopId = shopInfo.ShopID,
				itemoid = targetItem.OID
			}));
		}

		public static void DoPvpRollback()
		{
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_RevertShop, null);
		}

		public static void DoPvpUseitem(ItemInfo targetItem)
		{
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_UseItem, SerializeHelper.Serialize<ShopUseItemInfo>(new ShopUseItemInfo
			{
				itemoid = targetItem.OID
			}));
		}

		public static void DoPveBuy(ShopInfo shopInfo, Units unit, string targetItemID, List<ItemInfo> possessItemsP, int realPrice)
		{
			List<ItemInfo> list = new List<ItemInfo>(possessItemsP);
			List<string> composition = BattleEquipTools_Travers.GetComposition(targetItemID, list);
			if (composition != null || composition.Count > 0)
			{
				for (int i = 0; i < composition.Count; i++)
				{
					ItemInfo target;
					int index;
					if (BattleEquipTools_Travers.GetItem_last_least(list, composition[i], out target, out index))
					{
						BattleEquipTools_op.RemoveItem(list, target, index);
					}
				}
			}
			BattleEquipTools_op.AddItem(list, targetItemID);
			BattleEquipTools_op.ChangeHeroMoney(unit.unique_id, -realPrice);
			BattleEquipTools_op.SetHeroItems(unit, list);
			byte[] msgParam = SerializeHelper.Serialize<P2CBuyItem>(new P2CBuyItem
			{
				itemoid = 0,
				retaCode = 0
			});
			MobaMessage message = MobaMessageManager.GetMessage(PvpCode.C2P_BuyItem, msgParam, 0f, null);
			MobaMessageManager.DispatchMsg(message);
		}

		public static void DoPveSell(ShopInfo shopInfo, Units unit, ItemInfo targetItem, List<ItemInfo> possessItemsP)
		{
			List<ItemInfo> items = new List<ItemInfo>(possessItemsP);
			int itemPrice = BattleEquipTools_config.GetItemPrice(targetItem.ID);
			int delta = (int)((float)itemPrice * 0.8f);
			BattleEquipTools_op.RemoveItem(items, targetItem);
			BattleEquipTools_op.ChangeHeroMoney(unit.unique_id, delta);
			BattleEquipTools_op.SetHeroItems(unit, items);
			byte[] msgParam = SerializeHelper.Serialize<P2CSellItem>(new P2CSellItem
			{
				itemoid = 0,
				retaCode = 0
			});
			MobaMessage message = MobaMessageManager.GetMessage(PvpCode.C2P_SellItem, msgParam, 0f, null);
			MobaMessageManager.DispatchMsg(message);
		}

		public static void DoPveRollback(ShopInfo shopInfo, Units unit)
		{
			RollbackInfo rollbackInfo = shopInfo.RollbackStack.Peek();
			if (rollbackInfo != null)
			{
				BattleEquipTools_op.ChangeHeroMoney(unit.unique_id, -rollbackInfo._deltaMoney);
				BattleEquipTools_op.SetHeroItems(unit, rollbackInfo._items);
			}
			byte[] msgParam = SerializeHelper.Serialize<RetaMsg>(new RetaMsg
			{
				retaCode = 0
			});
			MobaMessage message = MobaMessageManager.GetMessage(PvpCode.C2P_RevertShop, msgParam, 0f, null);
			MobaMessageManager.DispatchMsg(message);
		}

		public static void DoPveUseItem(Units unit, ItemInfo targetItem, List<ItemInfo> possessItemsP)
		{
			List<ItemInfo> items = new List<ItemInfo>(possessItemsP);
			BattleEquipTools_op.RemoveItem(items, targetItem);
			BattleEquipTools_op.SetHeroItems(unit, items);
		}

		public static void ChangeHeroMoney(int uid, int delta)
		{
			UtilManager.Instance.ChangeGoldById(uid, delta);
		}

		public static void SetHeroItems(Units unit, List<ItemInfo> items)
		{
			if (items != null && items != null)
			{
				List<ItemDynData> dynItemList = BattleEquipTools_Travers.GetDynItemList(items);
				((Hero)unit).EquipPackage.SetEquipList(dynItemList);
			}
		}

		public static void ApplyEquipsToHero(List<string> equips, Hero hero)
		{
			if (equips != null && hero != null)
			{
				Dictionary<AttrType, float> dictionary;
				Dictionary<AttrType, float> dictionary2;
				BattleEquipTools_config.GetItemsAttri(equips, out dictionary, out dictionary2);
			}
		}

		public static List<string> GetHeroItemsString(Units unit)
		{
			List<ItemDynData> heroItems = BattleEquipTools_op.GetHeroItems(unit);
			return BattleEquipTools_Travers.GetItemListString(heroItems);
		}

		public static List<ItemDynData> GetHeroItems(Units unit)
		{
			List<ItemDynData> list = null;
			if (null != unit)
			{
				list = ((Hero)unit).EquipPackage.EquipList;
			}
			return list ?? new List<ItemDynData>();
		}

		public static bool RemoveItem(List<ItemInfo> items, ItemInfo target)
		{
			if (items != null || target != null)
			{
				int index = items.IndexOf(target);
				return BattleEquipTools_op.RemoveItem(items, target, index);
			}
			return false;
		}

		public static bool RemoveItem(List<ItemInfo> items, ItemInfo target, int index)
		{
			bool result = false;
			if (items != null && target != null)
			{
				if (!(result = BattleEquipTools_op.SubstractItemNum(target)))
				{
					if (result = BattleEquipTools_op.RemoveItem(items, index))
					{
					}
				}
			}
			return result;
		}

		public static bool SubstractItemNum(ItemInfo item)
		{
			if (item != null && item.Num > 1)
			{
				item.Num--;
				return true;
			}
			return false;
		}

		public static bool RemoveItem(List<ItemInfo> items, int index)
		{
			if (items != null && index >= 0 && index < items.Count)
			{
				items.RemoveAt(index);
				return true;
			}
			return false;
		}

		public static void AddItem(List<ItemInfo> items, string targetID)
		{
			int num = -1;
			ItemInfo itemInfo;
			SysBattleItemsVo vo;
			if (BattleEquipTools_Travers.GetItem_first_most(items, targetID, out itemInfo, out num))
			{
				itemInfo.Num++;
			}
			else if (items != null && items.Count < 6 && BattleEquipTools_config.GetBattleItemVo(targetID, out vo))
			{
				items.Add(new ItemInfo(items.Count, 0, 1, vo));
			}
		}
	}
}
