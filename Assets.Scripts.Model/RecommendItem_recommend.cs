using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class RecommendItem_recommend : ITreeTraversCallback
	{
		private List<string> _hasItems;

		private List<RItemData> listRItems;

		private int num;

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
				if (this.listRItems.Count > 2)
				{
					this.listRItems.RemoveRange(2, this.listRItems.Count - 2);
				}
				return this.listRItems;
			}
		}

		public RecommendItem_recommend(List<string> hasItems)
		{
			this.listRItems = new List<RItemData>();
			this._hasItems = new List<string>(hasItems);
			this.num = this._hasItems.Count;
		}

		public void Reset(List<string> hasItems)
		{
			this.listRItems.Clear();
			this._hasItems.Clear();
			this._hasItems.AddRange(hasItems);
			this.num = this._hasItems.Count;
		}

		private bool OnCallback(BEquip_node node, int level)
		{
			bool result = false;
			if (node != null && node.Data != null)
			{
				RItemData data = node.Data;
				int num = this._hasItems.IndexOf(data.ID);
				if (data.Possessed && num != -1)
				{
					this._hasItems.RemoveAt(num);
				}
				else if (!data.Afford)
				{
					result = true;
				}
				else if (this.num == 6 && data.Price <= data.RealPrice)
				{
					result = true;
				}
				else
				{
					this.InsertItem(data);
				}
			}
			return result;
		}

		private void InsertItem(RItemData item)
		{
			if (!this.listRItems.Contains(item))
			{
				if (this.listRItems.Count > 0)
				{
					for (int i = this.listRItems.Count - 1; i >= 0; i--)
					{
						if (this.listRItems[i].CompareTo(item) <= 0)
						{
							this.listRItems.Insert(i + 1, item);
						}
						else if (i == 0)
						{
							this.listRItems.Insert(i, item);
						}
					}
				}
				else
				{
					this.listRItems.Add(item);
				}
			}
		}
	}
}
