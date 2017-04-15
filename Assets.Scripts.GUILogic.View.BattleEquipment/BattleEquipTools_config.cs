using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using MobaHeros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public static class BattleEquipTools_config
	{
		public class AttriShowInfo
		{
			public AttrType _type;

			public string _format;

			public string _baseValue;

			public string _extraValue;

			public string _iconName;

			public AttriShowInfo(AttrType type, string format, string baseValue, string extraValue, string iconName)
			{
				this._type = type;
				this._format = format;
				this._baseValue = baseValue;
				this._extraValue = extraValue;
				this._iconName = iconName;
			}
		}

		public class AttriDetailInfo
		{
			public AttrType _attriType;

			public string _baseValue;

			public string _extraValue;

			public string _attriName;

			public string _extraColor;

			public string _format;

			public AttriDetailInfo(AttrType attriType, string baseValue, string extraValue, string format, string attriName, string extraColor)
			{
				this._attriType = attriType;
				this._baseValue = baseValue;
				this._extraValue = extraValue;
				this._attriName = attriName;
				this._extraColor = extraColor;
				this._format = format;
			}
		}

		public static readonly string Notice_SystemError = "系统错误";

		public static readonly string Notice_ShopBusy = "点击太频繁";

		public static readonly string Notice_EqualUpperLimit = "达到购买上限，无法购买";

		public static readonly string Notice_DeficientMoney = "金钱不够，无法购买";

		public static readonly string Notice_DeficientSpace = "道具已满，无法购买";

		public static readonly string Notice_outOfShoppingArea = "不在购买范围内，无法购买";

		public static readonly string Notice_brawl = "大乱斗中每条命只能买一次装备";

		public static readonly string AttriItemValueColor = "[00ff37]";

		private static BattleEquipType[] typeList = new BattleEquipType[]
		{
			BattleEquipType.recommend,
			BattleEquipType.attack,
			BattleEquipType.magic,
			BattleEquipType.defense,
			BattleEquipType.assist
		};

		private static Dictionary<BattleEquipType, string> dicEquipTypeName = new Dictionary<BattleEquipType, string>
		{
			{
				BattleEquipType.recommend,
				LanguageManager.Instance.GetStringById("BattleShopUI_Paging_Recommend")
			},
			{
				BattleEquipType.attack,
				LanguageManager.Instance.GetStringById("BattleShopUI_Paging_Attack")
			},
			{
				BattleEquipType.magic,
				LanguageManager.Instance.GetStringById("BattleShopUI_Paging_Magic")
			},
			{
				BattleEquipType.defense,
				LanguageManager.Instance.GetStringById("BattleShopUI_Paging_Defense")
			},
			{
				BattleEquipType.assist,
				LanguageManager.Instance.GetStringById("BattleShopUI_Paging_Assist")
			}
		};

		private static Dictionary<BattleEquipType, string> dicMenuTypeSpriteStr = new Dictionary<BattleEquipType, string>
		{
			{
				BattleEquipType.recommend,
				"1"
			},
			{
				BattleEquipType.attack,
				"2"
			},
			{
				BattleEquipType.magic,
				"3"
			},
			{
				BattleEquipType.defense,
				"4"
			},
			{
				BattleEquipType.assist,
				"5"
			}
		};

		private static Dictionary<BattleEquipType, string> dicEquipTypeIcon = new Dictionary<BattleEquipType, string>
		{
			{
				BattleEquipType.recommend,
				"HUD_shop_icons_recommend"
			},
			{
				BattleEquipType.attack,
				"HUD_shop_icons_attack"
			},
			{
				BattleEquipType.magic,
				"HUD_shop_icons_magic"
			},
			{
				BattleEquipType.defense,
				"HUD_shop_icons_protect"
			},
			{
				BattleEquipType.assist,
				"HUD_shop_icons_help"
			}
		};

		public static readonly Dictionary<AttrType, BattleEquipTools_config.AttriShowInfo> dicAttriShowInfo = new Dictionary<AttrType, BattleEquipTools_config.AttriShowInfo>
		{
			{
				AttrType.Attack,
				new BattleEquipTools_config.AttriShowInfo(AttrType.Attack, "F0", "atk", "extraAtk", "HUD_equipment_property_01")
			},
			{
				AttrType.MagicPower,
				new BattleEquipTools_config.AttriShowInfo(AttrType.MagicPower, "F0", "magicPower", "extraMagicPower", "HUD_equipment_property_02")
			},
			{
				AttrType.AttackSpeed,
				new BattleEquipTools_config.AttriShowInfo(AttrType.AttackSpeed, "F2", "atkSpd", "extraAtkSpd", "HUD_equipment_property_03")
			},
			{
				AttrType.MoveSpeed,
				new BattleEquipTools_config.AttriShowInfo(AttrType.MoveSpeed, "F2", "spd", "extraSpd", "HUD_equipment_property_04")
			},
			{
				AttrType.Armor,
				new BattleEquipTools_config.AttriShowInfo(AttrType.Armor, "F0", "armor", "extraArmor", "HUD_equipment_property_06")
			},
			{
				AttrType.MagicResist,
				new BattleEquipTools_config.AttriShowInfo(AttrType.MagicResist, "F0", "magicResist", "extraMagicResist", "HUD_equipment_property_05")
			}
		};

		public static readonly Dictionary<AttrType, BattleEquipTools_config.AttriDetailInfo> dicAttriDetailShowInfo = new Dictionary<AttrType, BattleEquipTools_config.AttriDetailInfo>
		{
			{
				AttrType.Power,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.Power, "power", "extraPower", "F0", "力量", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.Attack,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.Attack, "atk", "extraAtk", "F0", "攻击力", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.Agile,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.Agile, "agility", "extraAgility", "F0", "敏捷", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.AttackSpeed,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.AttackSpeed, "atkSpd", "extraAtkSpd", "F2", "攻击速度", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.Intelligence,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.Intelligence, "intelligence", "extraIntelligence", "F0", "智力", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.MoveSpeed,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.MoveSpeed, "spd", "extraSpd", "F2", "移动速度", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.HpRestore,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.HpRestore, "restore", "extraRestore", "F1", "生命回复", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.MpRestore,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.MpRestore, "mpRestore", "extraMpRestore", "F1", "魔法回复", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.Armor,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.Armor, "armor", "extraArmor", "F0", "护甲", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.MagicResist,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.MagicResist, "magicResist", "extraMagicResist", "F0", "魔法抗性", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.AttackRange,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.AttackRange, "attackRange", "extraAttackRange", "F2", "攻击范围", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.MagicPower,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.MagicPower, "magicPower", "extraMagicPower", "F0", "魔法强度", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.PhysicCritProp,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.PhysicCritProp, "physicalCritProp", "extraPhysicalCritProp", "P0", "物理暴击率", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.PhysicCritMag,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.PhysicCritMag, "physicalCritMag", "extraPhysicalCritMag", "P0", "物理暴击倍率", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.ArmorCut,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.ArmorCut, "armorCut", "extraArmorCut", "F0", "护甲削减", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.MagicResistanceCut,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.MagicResistanceCut, "magicResistCut", "extraMagicResistCut", "F0", "魔法抗性削减", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.ArmorCut_Percentage,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.ArmorCut_Percentage, "armorCutPercentage", "extraArmorCutPercentage", "P0", "护甲穿透", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.MagicResistanceCut_Percentage,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.MagicResistanceCut_Percentage, "magicResistCutPercentage", "extraMagicResistCutPercentage", "P0", "魔法抗性穿透", BattleEquipTools_config.AttriItemValueColor)
			},
			{
				AttrType.DodgeProp,
				new BattleEquipTools_config.AttriDetailInfo(AttrType.DodgeProp, "dodgeProp", "extraDodgeProp", "P0", "闪避率", BattleEquipTools_config.AttriItemValueColor)
			}
		};

		public static string GetAttriValueStr(HeroDetailedAttr data, AttrType type)
		{
			string result = string.Empty;
			if (data != null && BattleEquipTools_config.dicAttriShowInfo.ContainsKey(type))
			{
				Type type2 = data.GetType();
				FieldInfo field = type2.GetField(BattleEquipTools_config.dicAttriShowInfo[type]._baseValue);
				float num = (float)field.GetValue(data);
				FieldInfo field2 = type2.GetField(BattleEquipTools_config.dicAttriShowInfo[type]._extraValue);
				float num2 = (float)field2.GetValue(data);
				result = BattleEquipTools_config.AttriItemValueColor + (num + num2).ToString(BattleEquipTools_config.dicAttriShowInfo[type]._format);
			}
			return result;
		}

		public static string GetAttriDetailValueStr(HeroDetailedAttr data, AttrType type)
		{
			string result = string.Empty;
			if (data != null && BattleEquipTools_config.dicAttriDetailShowInfo.ContainsKey(type))
			{
				BattleEquipTools_config.AttriDetailInfo attriDetailInfo = BattleEquipTools_config.dicAttriDetailShowInfo[type];
				Type type2 = data.GetType();
				FieldInfo field = type2.GetField(attriDetailInfo._baseValue);
				float num = (float)field.GetValue(data);
				FieldInfo field2 = type2.GetField(attriDetailInfo._extraValue);
				float num2 = (float)field2.GetValue(data);
				result = string.Concat(new string[]
				{
					attriDetailInfo._attriName,
					":",
					(num + num2).ToString(attriDetailInfo._format),
					"(",
					attriDetailInfo._extraColor,
					"+",
					num2.ToString(attriDetailInfo._format),
					"[-])"
				});
			}
			return result;
		}

		public static bool GetShopIDListByScene(string levelID, EBattleShopType type, out Dictionary<EBattleShopType, ShopInfo> shopIDList)
		{
			bool result = false;
			shopIDList = new Dictionary<EBattleShopType, ShopInfo>();
			SysBattleSceneVo sysBattleSceneVo;
			if (BattleEquipTools_config.GetBattleSceneVo(levelID, out sysBattleSceneVo))
			{
				string shop_id = sysBattleSceneVo.shop_id;
				if (!string.IsNullOrEmpty(shop_id))
				{
					string[] array = shop_id.Split(new char[]
					{
						','
					});
					for (int i = 0; i < array.Length; i++)
					{
						SysBattleShopVo sysBattleShopVo = null;
						if (BattleEquipTools_config.GetBattelShopVo(array[i], out sysBattleShopVo))
						{
							if ((sysBattleShopVo.type == (int)type || sysBattleShopVo.type == 3 || sysBattleShopVo.type == 5) && !shopIDList.ContainsKey((EBattleShopType)sysBattleShopVo.type))
							{
								shopIDList.Add((EBattleShopType)sysBattleShopVo.type, new ShopInfo(sysBattleShopVo, false));
							}
						}
					}
					result = true;
				}
			}
			return result;
		}

		public static bool GetBattelShopVo(string id, out SysBattleShopVo battleShop)
		{
			battleShop = null;
			if (!string.IsNullOrEmpty(id))
			{
				battleShop = BaseDataMgr.instance.GetDataById<SysBattleShopVo>(id);
				if (battleShop == null)
				{
					Reminder.ReminderStr("SysBattleShopVo配置表 找不到levelID=" + id);
				}
			}
			return null != battleShop;
		}

		public static bool GetBattleSceneVo(string levelID, out SysBattleSceneVo battleScene)
		{
			battleScene = null;
			if (!string.IsNullOrEmpty(levelID))
			{
				battleScene = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(levelID);
				if (battleScene == null)
				{
					Reminder.ReminderStr("SysBattleSceneVo配置表 找不到levelID=" + levelID);
				}
			}
			return null != battleScene;
		}

		public static string GetNameByEquipType(BattleEquipType t)
		{
			return (!BattleEquipTools_config.dicMenuTypeSpriteStr.ContainsKey(t)) ? string.Empty : BattleEquipTools_config.dicMenuTypeSpriteStr[t];
		}

		public static string GetIconByEquipType(BattleEquipType t)
		{
			return (!BattleEquipTools_config.dicEquipTypeIcon.ContainsKey(t)) ? string.Empty : BattleEquipTools_config.dicEquipTypeIcon[t];
		}

		public static BattleEquipType GetEquipMenuTypeByIndex(int index)
		{
			return (index < 0 || index >= BattleEquipTools_config.typeList.Length) ? BattleEquipType.none : BattleEquipTools_config.typeList[index];
		}

		public static int GetEquipMenuCount()
		{
			return BattleEquipTools_config.typeList.Length;
		}

		public static bool GetBattleItemVo(string equipID, out SysBattleItemsVo battleItemVo)
		{
			bool result = false;
			battleItemVo = null;
			if (!string.IsNullOrEmpty(equipID))
			{
				battleItemVo = BaseDataMgr.instance.GetDataById<SysBattleItemsVo>(equipID);
				if (battleItemVo == null)
				{
					Reminder.ReminderStr("未找到 表中 ID=" + equipID + " 的装备");
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		public static int GetItemPrice(string equipID)
		{
			SysBattleItemsVo sysBattleItemsVo;
			return (!BattleEquipTools_config.GetBattleItemVo(equipID, out sysBattleItemsVo)) ? 0 : sysBattleItemsVo.sell;
		}

		public static List<string> GetShopItems(SysBattleShopVo shopVo)
		{
			List<string> list = null;
			if (shopVo == null)
			{
				Reminder.ReminderStr("根据ID未获取到商店信息SHopID===>" + shopVo);
			}
			else if (shopVo.items == null)
			{
				Reminder.ReminderStr("商店数据为空");
			}
			else
			{
				list = new List<string>(shopVo.items.Split(new char[]
				{
					','
				}));
			}
			return (list == null) ? new List<string>() : list;
		}

		public static List<string> GetShopItems(List<string> shopData, BattleEquipType type)
		{
			if (shopData != null)
			{
				shopData = shopData.Where(delegate(string obj)
				{
					SysBattleItemsVo sysBattleItemsVo;
					return BattleEquipTools_config.GetBattleItemVo(obj, out sysBattleItemsVo) && sysBattleItemsVo.type == (int)type;
				}).ToList<string>();
			}
			return shopData;
		}

		public static Dictionary<ColumnType, Dictionary<string, SItemData>> GetShopItems_Recommend(Dictionary<ColumnType, Dictionary<string, SItemData>> sItems, List<string> recommendedItems)
		{
			List<string> ids = null;
			if (recommendedItems != null && recommendedItems.Count > 0)
			{
				ids = BattleEquipTools_Travers.GetComposition(recommendedItems[0]);
			}
			BattleEquipTools_config.idsToSItems(ref sItems, ids, BattleEquipType.none);
			return sItems;
		}

		public static Dictionary<ColumnType, Dictionary<string, SItemData>> GetShopItems_common(Dictionary<ColumnType, Dictionary<string, SItemData>> sItems, BattleEquipType menuType, ShopInfo shopInfo)
		{
			if (menuType != BattleEquipType.none && shopInfo != null)
			{
				List<string> shopItems = shopInfo.ShopItems;
				BattleEquipTools_config.idsToSItems(ref sItems, shopItems, menuType);
			}
			return sItems;
		}

		public static void idsToSItems(ref Dictionary<ColumnType, Dictionary<string, SItemData>> sItems, List<string> ids, BattleEquipType menuType = BattleEquipType.none)
		{
			BattleEquipTools_config.RegularSItems(ref sItems, true);
			SysBattleItemsVo sysBattleItemsVo = null;
			if (ids != null)
			{
				for (int i = 0; i < ids.Count; i++)
				{
					if (BattleEquipTools_config.GetBattleItemVo(ids[i], out sysBattleItemsVo) && (menuType == BattleEquipType.none || sysBattleItemsVo.type == (int)menuType) && sItems.ContainsKey((ColumnType)sysBattleItemsVo.level))
					{
						sItems[(ColumnType)sysBattleItemsVo.level].Add(sysBattleItemsVo.items_id, new SItemData(sysBattleItemsVo));
					}
				}
			}
		}

		public static Dictionary<string, string> GetRItemsDic(string heroID)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			SysHeroMainVo sysHeroMainVo;
			if (BattleEquipTools_config.GetHeroMainVo(heroID, out sysHeroMainVo))
			{
				List<string> list = BattleEquipTools_config.StringToStringList(sysHeroMainVo.recommend_equip, ',', "[]");
				for (int i = 0; i < list.Count; i++)
				{
					List<string> list2 = BattleEquipTools_config.StringToStringList(list[i], '|', "[]");
					if (list2.Count == 2)
					{
						dictionary.Add(list2[0], list2[1]);
					}
				}
			}
			return dictionary;
		}

		public static bool GetRecommendEquipmentVo(string key, out SysRecommendEquipmentVo vo)
		{
			vo = null;
			if (!string.IsNullOrEmpty(key))
			{
				vo = BaseDataMgr.instance.GetDataById<SysRecommendEquipmentVo>(key);
				if (vo == null)
				{
					Reminder.ReminderStr("SysRecommendEquipmentVo  is Null key=" + key);
				}
			}
			return vo != null;
		}

		public static bool GetHeroMainVo(string heroID, out SysHeroMainVo vo)
		{
			vo = null;
			if (!string.IsNullOrEmpty(heroID))
			{
				vo = BaseDataMgr.instance.GetHeroMainData(heroID);
				if (vo == null)
				{
					Reminder.ReminderStr("SysHeroMainVo  is Null heroID=" + heroID);
				}
			}
			return vo != null;
		}

		public static bool GetBattleShopVo(string shopID, out SysBattleShopVo vo)
		{
			vo = null;
			if (!string.IsNullOrEmpty(shopID))
			{
				vo = BaseDataMgr.instance.GetDataById<SysBattleShopVo>(shopID);
				if (vo == null)
				{
					Reminder.ReminderStr("battleShop  is Null shopId" + shopID);
				}
			}
			return vo != null;
		}

		public static string GetAttriDes(SysBattleItemsVo vo, string seporator = "\n", int maxNum = 6)
		{
			string text = string.Empty;
			if (vo != null)
			{
				if (!string.IsNullOrEmpty(vo.attribute))
				{
					string[] array = vo.attribute.Split(new char[]
					{
						','
					});
					int num = 0;
					while (num < array.Length && num < maxNum)
					{
						if (!string.IsNullOrEmpty(array[num]) && !("[]" == array[num]))
						{
							string[] array2 = array[num].Split(new char[]
							{
								'|'
							});
							if (array2.Length < 3)
							{
								Reminder.ReminderStr(vo.items_id + " in battle_items.attrbuite.length < 3");
							}
							else
							{
								string format = array2[2];
								AttrType type = (AttrType)int.Parse(array2[0]);
								float num2;
								if (array2[1].Contains("%"))
								{
									string text2 = array2[1].Trim();
									num2 = float.Parse(text2.Substring(0, text2.Length - 1)) / 100f;
								}
								else
								{
									num2 = float.Parse(array2[1]);
								}
								string text3 = text;
								text = string.Concat(new string[]
								{
									text3,
									"+",
									num2.ToString(format),
									" ",
									CharacterDataMgr.instance.GetChinaName((int)type),
									seporator
								});
							}
						}
						num++;
					}
				}
			}
			return text;
		}

		public static void GetItemsAttri(List<string> equipList, out Dictionary<AttrType, float> add, out Dictionary<AttrType, float> mul)
		{
			add = new Dictionary<AttrType, float>();
			mul = new Dictionary<AttrType, float>();
			if (equipList != null)
			{
				for (int i = 0; i < equipList.Count; i++)
				{
					BattleEquipTools_config.GetItemAttri(equipList[i], ref add, ref mul);
				}
			}
		}

		public static void GetItemAttri(string equipID, ref Dictionary<AttrType, float> add, ref Dictionary<AttrType, float> mul)
		{
			SysBattleItemsVo sysBattleItemsVo;
			if (BattleEquipTools_config.GetBattleItemVo(equipID, out sysBattleItemsVo))
			{
				if (!string.IsNullOrEmpty(sysBattleItemsVo.attribute))
				{
					string[] array = sysBattleItemsVo.attribute.Split(new char[]
					{
						','
					});
					for (int i = 0; i < array.Length; i++)
					{
						if (!string.IsNullOrEmpty(array[i]) && !(array[i] == "[]"))
						{
							string[] array2 = array[i].Split(new char[]
							{
								'|'
							});
							int num;
							if (int.TryParse(array2[0], out num))
							{
								AttrType attrType = (AttrType)num;
								if (array2[1].Contains("%"))
								{
									if (!mul.ContainsKey(attrType))
									{
										mul.Add(attrType, 0f);
									}
									string text = array2[1].Trim();
									Dictionary<AttrType, float> dictionary;
									Dictionary<AttrType, float> expr_CF = dictionary = mul;
									AttrType key;
									AttrType expr_D4 = key = attrType;
									float num2 = dictionary[key];
									expr_CF[expr_D4] = num2 + float.Parse(text.Substring(0, text.Length - 1)) / 100f;
								}
								else
								{
									if (!add.ContainsKey(attrType))
									{
										add.Add(attrType, 0f);
									}
									Dictionary<AttrType, float> dictionary2;
									Dictionary<AttrType, float> expr_129 = dictionary2 = add;
									AttrType key;
									AttrType expr_12E = key = attrType;
									float num2 = dictionary2[key];
									expr_129[expr_12E] = num2 + float.Parse(array2[1]);
								}
							}
						}
					}
				}
			}
		}

		public static List<string> GetRItems(List<string> recommendList, string newItem, List<ItemInfo> possessItems)
		{
			List<string> itemListString = BattleEquipTools_Travers.GetItemListString(possessItems);
			List<string> list = new List<string>(recommendList);
			if (list != null && !string.IsNullOrEmpty(newItem))
			{
				List<string> composition = BattleEquipTools_Travers.GetComposition(newItem);
				while (list != null && list.Count != 0)
				{
					if (!(list[0] == newItem) && !composition.Contains(list[0]) && !itemListString.Contains(list[0]))
					{
						break;
					}
					list.RemoveAt(0);
				}
			}
			return list;
		}

		public static List<string> GetRItems(string levelID, string heroID)
		{
			List<string> result = null;
			SysBattleSceneVo sysBattleSceneVo;
			if (BattleEquipTools_config.GetBattleSceneVo(levelID, out sysBattleSceneVo))
			{
				Dictionary<string, string> rItemsDic = BattleEquipTools_config.GetRItemsDic(heroID);
				string key;
				if (rItemsDic.ContainsKey(sysBattleSceneVo.scene_map_id))
				{
					key = rItemsDic[sysBattleSceneVo.scene_map_id];
				}
				else
				{
					if (!rItemsDic.ContainsKey("default"))
					{
						return result;
					}
					key = rItemsDic["default"];
				}
				SysRecommendEquipmentVo sysRecommendEquipmentVo;
				if (BattleEquipTools_config.GetRecommendEquipmentVo(key, out sysRecommendEquipmentVo))
				{
					result = BattleEquipTools_config.StringToStringList(sysRecommendEquipmentVo.equipments, ',', "[]");
				}
			}
			return result;
		}

		public static void RegularSItems(ref Dictionary<ColumnType, Dictionary<string, SItemData>> dic, bool clear = false)
		{
			if (dic == null)
			{
				dic = new Dictionary<ColumnType, Dictionary<string, SItemData>>();
			}
			ColumnType[] array = (ColumnType[])Enum.GetValues(typeof(ColumnType));
			ColumnType[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				ColumnType key = array2[i];
				if (!dic.ContainsKey(key) || dic[key] == null)
				{
					dic.Add(key, new Dictionary<string, SItemData>());
				}
				else if (clear)
				{
					dic[key].Clear();
				}
			}
		}

		public static List<string> StringToStringList(string res, char seprator = ',', string emptyStr = "[]")
		{
			List<string> list = new List<string>();
			if (!string.IsNullOrEmpty(res) && !res.Equals(emptyStr))
			{
				string[] array = res.Split(new char[]
				{
					seprator
				});
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrEmpty(array[i]) && !res.Equals(emptyStr))
					{
						list.Add(array[i]);
					}
				}
			}
			return list;
		}

		public static bool IsChanged(List<string> oldObj, List<string> newObj)
		{
			bool result = true;
			if (newObj == null && oldObj == null)
			{
				result = false;
			}
			else if (newObj != null && oldObj != null && newObj.Count == oldObj.Count)
			{
				int i;
				for (i = 0; i < newObj.Count; i++)
				{
					if (newObj[i] != oldObj[i])
					{
						break;
					}
				}
				result = (i != newObj.Count);
			}
			return result;
		}

		public static bool IsChanged(List<RItemData> a, List<RItemData> b)
		{
			bool result = true;
			if (a == null && b == null)
			{
				result = false;
			}
			else if (a == null)
			{
				result = (b.Count != 0);
			}
			else if (b == null)
			{
				result = (a.Count != 0);
			}
			else if (a != null && b != null && a.Count == b.Count)
			{
				int i;
				for (i = 0; i < a.Count; i++)
				{
					if (!a[i].ID.Equals(b[i].ID))
					{
						break;
					}
				}
				result = (i != a.Count);
			}
			return result;
		}

		public static TeamType ShopType2TeamType(EBattleShopType e)
		{
			TeamType result = TeamType.None;
			switch (e)
			{
			case EBattleShopType.eLM:
				result = TeamType.LM;
				break;
			case EBattleShopType.eBL:
				result = TeamType.BL;
				break;
			case EBattleShopType.eNeutral:
			case EBattleShopType.eNeutral_2:
				result = TeamType.Neutral;
				break;
			case EBattleShopType.eTeam3:
				result = TeamType.Team_3;
				break;
			}
			return result;
		}
	}
}
