using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using System;

namespace Mobaheros.AI.Equipment
{
	public class EquipmentData
	{
		public string EquipmentId
		{
			get;
			private set;
		}

		public int SellGain
		{
			get;
			private set;
		}

		public int BuyCost
		{
			get;
			private set;
		}

		public bool HaveBeenBought
		{
			get;
			private set;
		}

		public bool PosseState
		{
			get;
			private set;
		}

		public int ExtraSpent
		{
			get;
			private set;
		}

		public SysBattleItemsVo itemVo
		{
			get;
			private set;
		}

		public EquipmentData(string id, int sell, int buy)
		{
			this.EquipmentId = id;
			this.SellGain = sell;
			this.BuyCost = buy;
			this.HaveBeenBought = false;
			this.PosseState = false;
			this.CaclExtra();
		}

		public ItemInfo GetItemInfo()
		{
			return new ItemInfo(this.itemVo);
		}

		private void CaclExtra()
		{
			this.itemVo = BaseDataMgr.instance.GetDataById<SysBattleItemsVo>(this.EquipmentId);
			this.ExtraSpent = this.itemVo.sell;
			if (StringUtils.CheckValid(this.itemVo.consumption))
			{
				string[] stringValue = StringUtils.GetStringValue(this.itemVo.consumption, ',');
				if (stringValue != null)
				{
					string[] array = stringValue;
					for (int i = 0; i < array.Length; i++)
					{
						string unikey = array[i];
						SysBattleItemsVo dataById = BaseDataMgr.instance.GetDataById<SysBattleItemsVo>(unikey);
						if (dataById != null)
						{
							this.ExtraSpent -= dataById.sell;
						}
					}
				}
			}
			else
			{
				this.ExtraSpent = 0;
			}
		}

		public void ChangeBoughtState(bool bought)
		{
			this.HaveBeenBought = bought;
		}

		public void ChangePosseState(bool have)
		{
			this.PosseState = have;
		}
	}
}
