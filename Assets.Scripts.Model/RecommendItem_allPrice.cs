using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class RecommendItem_allPrice : ITreeTraversCallback
	{
		private List<string> _hasItems;

		private RecommendItem_price func_price;

		Func<BEquip_node, int, bool> ITreeTraversCallback.TraversCallback
		{
			get
			{
				return new Func<BEquip_node, int, bool>(this.OnCallback);
			}
		}

		public object Result
		{
			get
			{
				return null;
			}
		}

		public RecommendItem_allPrice(List<string> hasItems)
		{
			this._hasItems = new List<string>(hasItems);
		}

		public void Reset(List<string> hasItems)
		{
			this._hasItems.Clear();
			this._hasItems.AddRange(hasItems);
		}

		private bool OnCallback(BEquip_node node, int level)
		{
			bool result = false;
			if (node != null)
			{
				if (this.func_price == null)
				{
					this.func_price = new RecommendItem_price(node.Data, this._hasItems);
				}
				else
				{
					this.func_price.Reset(node.Data, this._hasItems);
				}
				node.Travers_first(this.func_price);
				node.Data.RealPrice = (int)this.func_price.Result;
				result = true;
			}
			return result;
		}
	}
}
