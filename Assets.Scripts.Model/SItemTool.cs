using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class SItemTool
	{
		private GetRealPrice priceFunc;

		private BattleShopData data;

		public SItemTool(BattleShopData data)
		{
			this.data = data;
		}

		public void Update_sItems()
		{
			this.Update_allRel();
			this.Update_possess();
			this.Update_realPrice();
			this.Update_afford();
		}

		public void Update_onMoneyChanged()
		{
			this.Update_realPrice();
			this.Update_afford();
		}

		public void Update_onPItemsChanged()
		{
			this.Update_possess();
			this.Update_realPrice();
			this.Update_afford();
		}

		private void Update_allRel()
		{
			Dictionary<ColumnType, Dictionary<string, SItemData>> sItems = this.data.SItems;
			if (sItems != null)
			{
				Dictionary<ColumnType, Dictionary<string, SItemData>>.Enumerator enumerator = sItems.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<ColumnType, Dictionary<string, SItemData>> current = enumerator.Current;
					Dictionary<string, SItemData> value = current.Value;
					if (value != null)
					{
						Dictionary<string, SItemData>.Enumerator enumerator2 = value.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							KeyValuePair<string, SItemData> current2 = enumerator2.Current;
							SItemData value2 = current2.Value;
							value2.Update_rel(sItems);
						}
					}
				}
			}
		}

		private void Update_possess()
		{
			Dictionary<ColumnType, Dictionary<string, SItemData>> sItems = this.data.SItems;
			List<string> pItemsStr = this.data.PItemsStr;
			if (sItems != null)
			{
				Dictionary<ColumnType, Dictionary<string, SItemData>>.Enumerator enumerator = sItems.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<ColumnType, Dictionary<string, SItemData>> current = enumerator.Current;
					Dictionary<string, SItemData> value = current.Value;
					if (value != null)
					{
						Dictionary<string, SItemData>.Enumerator enumerator2 = value.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							KeyValuePair<string, SItemData> current2 = enumerator2.Current;
							SItemData value2 = current2.Value;
							value2.Update_possess(pItemsStr);
						}
					}
				}
			}
		}

		private void Update_afford()
		{
			Dictionary<ColumnType, Dictionary<string, SItemData>> sItems = this.data.SItems;
			int money = this.data.Money;
			if (sItems != null)
			{
				Dictionary<ColumnType, Dictionary<string, SItemData>>.Enumerator enumerator = sItems.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<ColumnType, Dictionary<string, SItemData>> current = enumerator.Current;
					Dictionary<string, SItemData> value = current.Value;
					if (value != null)
					{
						Dictionary<string, SItemData>.Enumerator enumerator2 = value.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							KeyValuePair<string, SItemData> current2 = enumerator2.Current;
							SItemData value2 = current2.Value;
							value2.Update_afford(money);
						}
					}
				}
			}
		}

		private void Update_realPrice()
		{
			Dictionary<ColumnType, Dictionary<string, SItemData>> sItems = this.data.SItems;
			if (sItems != null)
			{
				Dictionary<ColumnType, Dictionary<string, SItemData>>.Enumerator enumerator = sItems.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<ColumnType, Dictionary<string, SItemData>> current = enumerator.Current;
					Dictionary<string, SItemData> value = current.Value;
					if (value != null)
					{
						Dictionary<string, SItemData>.Enumerator enumerator2 = value.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							KeyValuePair<string, SItemData> current2 = enumerator2.Current;
							SItemData value2 = current2.Value;
							value2.Update_realPrice(this.priceFunc);
						}
					}
				}
			}
		}

		public void Update_curSItem()
		{
			SItemData curSItem = this.data.CurSItem;
			SItemData preSItem = this.data.PreSItem;
			if (preSItem != null)
			{
				preSItem.ChooseState = false;
			}
			if (curSItem != null)
			{
				curSItem.ChooseState = true;
			}
		}

		public void Update_curSRItem()
		{
			SItemData curSRItem = this.data.CurSRItem;
			SItemData preSRItem = this.data.PreSRItem;
			if (preSRItem != null)
			{
				preSRItem.Update_route(this.data.Sections, true, true, false);
			}
			this.ResetSection();
			if (curSRItem != null)
			{
				curSRItem.Update_route(this.data.Sections, true, true, true);
			}
		}

		private void ResetSection()
		{
			if (this.data.Sections != null)
			{
				if (this.data.Sections[0] != null)
				{
					this.data.Sections[0].Clear();
				}
				if (this.data.Sections[1] != null)
				{
					this.data.Sections[1].Clear();
				}
			}
		}
	}
}
