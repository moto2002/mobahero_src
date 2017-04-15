using Assets.Scripts.GUILogic.View.BattleEquipment;
using Com.Game.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Model
{
	public class SItemData : IEquatable<SItemData>, IBEItem
	{
		private int realPrice;

		private bool afford;

		private int possessedNum;

		private bool choose;

		private bool hConnect;

		private bool lConnect;

		private SysBattleItemsVo config;

		private Dictionary<string, SItemData> hItems;

		private Dictionary<string, SItemData> lItems;

		private List<string> cItems;

		public string ID
		{
			get
			{
				return this.config.items_id;
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
			private set
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

		public List<string> CItems
		{
			get
			{
				List<string> arg_1B_0;
				if ((arg_1B_0 = this.cItems) == null)
				{
					arg_1B_0 = (this.cItems = new List<string>());
				}
				return arg_1B_0;
			}
		}

		public bool Afford
		{
			get
			{
				return this.afford;
			}
			private set
			{
				this.afford = value;
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
		}

		public bool Cheaper
		{
			get
			{
				return this.realPrice < this.config.sell;
			}
		}

		public bool ChooseState
		{
			get
			{
				return this.choose;
			}
			set
			{
				this.choose = value;
			}
		}

		public bool LConnect
		{
			get
			{
				return this.lConnect;
			}
		}

		public bool HConnect
		{
			get
			{
				return this.hConnect;
			}
		}

		public Dictionary<string, SItemData> HItems
		{
			get
			{
				return this.hItems;
			}
		}

		public Dictionary<string, SItemData> LItems
		{
			get
			{
				return this.lItems;
			}
		}

		public SItemData(SysBattleItemsVo vo)
		{
			this.config = vo;
			this.cItems = BattleEquipTools_config.StringToStringList(this.config.consumption, ',', "[]");
		}

		public bool Equals(SItemData other)
		{
			return other != null && other.ID.Equals(this.ID);
		}

		public bool HasHItems()
		{
			return this.hItems != null && this.hItems.Count > 0;
		}

		public bool HasLItems()
		{
			return this.lItems != null && this.lItems.Count > 0;
		}

		public void Update_rel(Dictionary<ColumnType, Dictionary<string, SItemData>> sItems)
		{
			ColumnType level = (ColumnType)this.config.level;
			if (level + 1 <= ColumnType.three)
			{
				Dictionary<string, SItemData> dictionary = sItems[level + 1];
				Dictionary<string, SItemData>.Enumerator enumerator = dictionary.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, SItemData> current = enumerator.Current;
					SItemData value = current.Value;
					if (value.CItems.Contains(this.ID))
					{
						this.Add(value, true);
					}
				}
			}
			if (level - ColumnType.one >= 1)
			{
				Dictionary<string, SItemData> dictionary2 = sItems[level - 1];
				Dictionary<string, SItemData>.Enumerator enumerator2 = dictionary2.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					KeyValuePair<string, SItemData> current2 = enumerator2.Current;
					SItemData value2 = current2.Value;
					if (this.cItems.Contains(value2.ID))
					{
						this.Add(value2, false);
					}
				}
			}
		}

		public void Update_possess(List<string> pItems)
		{
			this.possessedNum = ((pItems == null) ? 0 : pItems.Count(new Func<string, bool>(this.Selector)));
		}

		public void Update_afford(int money)
		{
			this.Afford = (money >= this.RealPrice);
		}

		public void Update_realPrice(GetRealPrice priceFunc)
		{
			if (priceFunc == null)
			{
				priceFunc = new GetRealPrice(this);
			}
			else
			{
				priceFunc.Reset(this);
			}
			this.Travers_toL(this, priceFunc, 0);
			this.RealPrice = (int)priceFunc.Ret;
		}

		public void Update_route(List<List<SItemData>>[] sections, bool l, bool h, bool show = true)
		{
			if (this.HasHItems())
			{
				this.hConnect = show;
				Dictionary<string, SItemData>.Enumerator enumerator = this.hItems.GetEnumerator();
				while (h && enumerator.MoveNext())
				{
					KeyValuePair<string, SItemData> current = enumerator.Current;
					current.Value.Update_route(sections, false, true, show);
				}
				if (show && h)
				{
					this.AddSection(true, sections);
				}
			}
			if (this.HasLItems())
			{
				this.lConnect = show;
				Dictionary<string, SItemData>.Enumerator enumerator2 = this.lItems.GetEnumerator();
				while (l && enumerator2.MoveNext())
				{
					KeyValuePair<string, SItemData> current2 = enumerator2.Current;
					current2.Value.Update_route(sections, true, false, show);
				}
				if (show && l)
				{
					this.AddSection(false, sections);
				}
			}
		}

		private void Add(SItemData d, bool h = true)
		{
			if (d != null)
			{
				if (h)
				{
					if (this.hItems == null)
					{
						this.hItems = new Dictionary<string, SItemData>();
					}
					if (!this.hItems.ContainsKey(d.ID))
					{
						this.hItems.Add(d.ID, d);
					}
				}
				else
				{
					if (this.lItems == null)
					{
						this.lItems = new Dictionary<string, SItemData>();
					}
					if (!this.lItems.ContainsKey(d.ID))
					{
						this.lItems.Add(d.ID, d);
					}
				}
			}
		}

		private bool Selector(string target)
		{
			return this.ID.Equals(target);
		}

		private void Clear(bool front = true)
		{
			if (front)
			{
				if (this.hItems != null)
				{
					this.hItems.Clear();
				}
			}
			else if (this.lItems != null)
			{
				this.lItems.Clear();
			}
		}

		private void Travers_toL(SItemData target, IBECallback ic, int depth = 0)
		{
			if (target != null)
			{
				if (ic != null && ic.Callback(target, depth + 1))
				{
					if (target.HasLItems())
					{
						Dictionary<string, SItemData> dictionary = target.LItems;
						Dictionary<string, SItemData>.Enumerator enumerator = dictionary.GetEnumerator();
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, SItemData> current = enumerator.Current;
							this.Travers_toL(current.Value, ic, depth + 1);
						}
					}
				}
			}
		}

		private void AddSection(bool h, List<List<SItemData>>[] sections)
		{
			int num = -1;
			int level = (int)this.Level;
			int num2 = level + ((!h) ? -1 : 1);
			Dictionary<string, SItemData> dictionary = (!h) ? this.lItems : this.hItems;
			if (level + num2 == 3)
			{
				num = 0;
			}
			else if (level + num2 == 5)
			{
				num = 1;
			}
			if (num != -1)
			{
				if (sections[num] == null)
				{
					sections[num] = new List<List<SItemData>>();
				}
				if (dictionary != null)
				{
					List<SItemData> list = dictionary.Values.ToList<SItemData>();
					list.Add(this);
					sections[num].Add(list);
				}
			}
		}
	}
}
