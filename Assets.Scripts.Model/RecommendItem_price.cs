using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class RecommendItem_price : ITreeTraversCallback
	{
		private int realPrice;

		private RItemData _targetItem;

		private List<string> _hasItems;

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
				return this.realPrice;
			}
		}

		public RecommendItem_price(RItemData targetItem, List<string> hasItems)
		{
			this._targetItem = targetItem;
			this.realPrice = targetItem.Config.sell;
			this._hasItems = new List<string>(hasItems);
		}

		public void Reset(RItemData targetItem, List<string> hasItems)
		{
			this._targetItem = targetItem;
			this.realPrice = targetItem.Config.sell;
			this._hasItems.Clear();
			this._hasItems.AddRange(hasItems);
		}

		private bool OnCallback(BEquip_node node, int level)
		{
			bool result = true;
			if (node.Data.Possessed && !node.Data.ID.Equals(this._targetItem.ID))
			{
				int num = this._hasItems.IndexOf(node.Data.ID);
				if (num != -1)
				{
					this._hasItems.RemoveAt(num);
					this.realPrice -= node.Data.Config.sell;
					result = false;
				}
			}
			return result;
		}
	}
}
