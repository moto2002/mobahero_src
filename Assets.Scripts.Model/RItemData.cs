using Assets.Scripts.GUILogic.View.BattleEquipment;
using Com.Game.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class RItemData : IComparable, IBEquip_data, IEquatable<RItemData>, IBEItem
	{
		private string itemID;

		private int realPrice;

		private bool affordable;

		private int possessedNum;

		private SysBattleItemsVo config;

		private Dictionary<EBattleShopType, ShopInfo> supportShops;

		private Dictionary<EBattleShopType, ShopInfo> allShops;

		public string ID
		{
			get
			{
				return this.itemID;
			}
			set
			{
				this.itemID = value;
			}
		}

		public SysBattleItemsVo Config
		{
			get
			{
				return this.config;
			}
		}

		public ColumnType Level
		{
			get
			{
				return (ColumnType)this.config.level;
			}
		}

		public BattleEquipType Type
		{
			get
			{
				return (BattleEquipType)this.config.type;
			}
		}

		public int RealPrice
		{
			get
			{
				return this.realPrice;
			}
			set
			{
				this.realPrice = value;
			}
		}

		public int Price
		{
			get
			{
				return this.config.sell;
			}
		}

		public bool Possessed
		{
			get
			{
				return this.possessedNum > 0;
			}
		}

		public int PossessedNum
		{
			get
			{
				return this.possessedNum;
			}
			set
			{
				this.possessedNum = value;
			}
		}

		public bool Afford
		{
			get
			{
				return this.affordable;
			}
			set
			{
				this.affordable = value;
			}
		}

		public Dictionary<EBattleShopType, ShopInfo> SupportShops
		{
			get
			{
				return this.supportShops;
			}
			set
			{
				this.supportShops = value;
			}
		}

		public bool Cheaper
		{
			get
			{
				return this.realPrice < this.config.sell;
			}
		}

		public RItemData(string id, Dictionary<EBattleShopType, ShopInfo> shops)
		{
			this.itemID = id;
			this.allShops = shops;
			this.supportShops = new Dictionary<EBattleShopType, ShopInfo>();
			BattleEquipTools_config.GetBattleItemVo(this.itemID, out this.config);
			this.GetSupportShops();
		}

		private void GetSupportShops()
		{
			this.supportShops.Clear();
			foreach (KeyValuePair<EBattleShopType, ShopInfo> current in this.allShops)
			{
				if (current.Value.ShopItems.Contains(this.itemID))
				{
					this.supportShops.Add(current.Key, current.Value);
				}
			}
		}

		public bool InShopArea()
		{
			bool flag = false;
			foreach (KeyValuePair<EBattleShopType, ShopInfo> current in this.supportShops)
			{
				flag = (flag || current.Value.InArea);
			}
			return flag;
		}

		public ShopInfo GetAvailableShop()
		{
			ShopInfo result = null;
			foreach (KeyValuePair<EBattleShopType, ShopInfo> current in this.supportShops)
			{
				if (current.Value.InArea)
				{
					result = current.Value;
					break;
				}
			}
			return result;
		}

		public ShopInfo GetShopByType(EBattleShopType type)
		{
			ShopInfo result = null;
			if (this.supportShops.ContainsKey(type))
			{
				result = this.supportShops[type];
			}
			return result;
		}

		public bool Equals(RItemData other)
		{
			return other != null && this.itemID.Equals(other.itemID);
		}

		public int CompareTo(object obj)
		{
			RItemData rItemData = obj as RItemData;
			int result;
			if (rItemData == null)
			{
				result = -1;
			}
			else if (this.config.level == rItemData.config.level)
			{
				if (this.realPrice == rItemData.realPrice)
				{
					result = 0;
				}
				else
				{
					result = ((this.realPrice <= rItemData.realPrice) ? 1 : -1);
				}
			}
			else
			{
				result = ((this.config.level <= rItemData.config.level) ? 1 : -1);
			}
			return result;
		}

		public List<RItemData> GenerateChilren(int level)
		{
			List<RItemData> list = new List<RItemData>();
			List<string> list2 = BattleEquipTools_config.StringToStringList(this.config.consumption, ',', "[]");
			for (int i = 0; i < list2.Count; i++)
			{
				RItemData item = new RItemData(list2[i], this.allShops);
				list.Add(item);
			}
			return list;
		}
	}
}
