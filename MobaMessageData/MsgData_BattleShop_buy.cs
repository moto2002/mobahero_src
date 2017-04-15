using Assets.Scripts.Model;
using System;

namespace MobaMessageData
{
	public class MsgData_BattleShop_buy
	{
		private bool valid;

		private RItemData rItem;

		private SItemData sItem;

		private BuyingEquipType buyType;

		private ShopInfo curShop;

		public bool Valid
		{
			get
			{
				return this.valid;
			}
		}

		public ShopInfo CurShop
		{
			get
			{
				return this.curShop;
			}
		}

		public string TargetID
		{
			get
			{
				string result = string.Empty;
				if (this.valid)
				{
					result = ((this.buyType != BuyingEquipType.eRecommend) ? this.sItem.ID : this.rItem.ID);
				}
				return result;
			}
		}

		public int RealPrice
		{
			get
			{
				int result = 0;
				if (this.valid)
				{
					result = ((this.buyType != BuyingEquipType.eRecommend) ? this.sItem.RealPrice : this.rItem.RealPrice);
				}
				return result;
			}
		}

		public bool Cheaper
		{
			get
			{
				bool result = false;
				if (this.valid)
				{
					result = ((this.buyType != BuyingEquipType.eRecommend) ? this.sItem.Cheaper : this.rItem.Cheaper);
				}
				return result;
			}
		}

		public MsgData_BattleShop_buy(RItemData rItem, BuyingEquipType type, ShopInfo curShopInfo)
		{
			this.rItem = rItem;
			this.sItem = null;
			this.buyType = type;
			this.curShop = curShopInfo;
			this.CheckValid();
		}

		public MsgData_BattleShop_buy(SItemData sItem, BuyingEquipType type, ShopInfo curShopInfo)
		{
			this.rItem = null;
			this.sItem = sItem;
			this.buyType = type;
			this.curShop = curShopInfo;
			this.CheckValid();
		}

		private void CheckValid()
		{
			this.valid = false;
			if (this.curShop != null)
			{
				if (this.buyType < BuyingEquipType.eMax && this.buyType > BuyingEquipType.eNone)
				{
					if (this.buyType != BuyingEquipType.eShop || this.sItem != null)
					{
						if (this.buyType != BuyingEquipType.eRecommend || this.rItem != null)
						{
							this.valid = true;
						}
					}
				}
			}
		}
	}
}
