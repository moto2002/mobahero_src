using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class GetRealPrice : IBECallback
	{
		private SItemData _target;

		private int price;

		private Dictionary<string, int> dicUse;

		public Func<object, int, bool> Callback
		{
			get
			{
				return new Func<object, int, bool>(this.callback);
			}
		}

		public object Ret
		{
			get
			{
				return this.price;
			}
		}

		public GetRealPrice(SItemData target)
		{
			this._target = target;
			this.price = this._target.Config.sell;
			this.dicUse = new Dictionary<string, int>();
		}

		public void Reset(SItemData target)
		{
			this._target = target;
			this.price = this._target.Config.sell;
			if (this.dicUse == null)
			{
				this.dicUse = new Dictionary<string, int>();
			}
			else
			{
				this.dicUse.Clear();
			}
		}

		private bool callback(object obj, int depth)
		{
			bool result = true;
			SItemData sItemData = obj as SItemData;
			if (sItemData != null)
			{
				if (sItemData.Possessed && !sItemData.ID.Equals(this._target.ID))
				{
					if (this.dicUse.ContainsKey(sItemData.ID))
					{
						Dictionary<string, int> dictionary;
						Dictionary<string, int> expr_51 = dictionary = this.dicUse;
						string iD;
						string expr_59 = iD = sItemData.ID;
						int num = dictionary[iD];
						expr_51[expr_59] = num + 1;
					}
					else
					{
						this.dicUse.Add(sItemData.ID, 1);
					}
					if (this.dicUse[sItemData.ID] <= sItemData.PossessedNum)
					{
						this.price -= sItemData.Config.sell;
						result = false;
					}
				}
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
