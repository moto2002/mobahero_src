using Assets.Scripts.Model;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public class FindItem : ITraversListCallback
	{
		private int _index;

		private bool _first;

		private string _targetID;

		private List<ItemInfo> _items;

		public object Result
		{
			get
			{
				return new object[]
				{
					this._index,
					this._items
				};
			}
		}

		public Func<object, int, bool> TraversingCallback
		{
			get
			{
				return new Func<object, int, bool>(this.callback);
			}
		}

		public FindItem(string targetID, bool first = false)
		{
			this._index = -1;
			this._first = first;
			this._targetID = targetID;
			this._items = new List<ItemInfo>();
		}

		private bool callback(object obj, int index)
		{
			ItemInfo itemInfo = obj as ItemInfo;
			if (itemInfo != null && this._targetID == itemInfo.ID)
			{
				this._items.Add(itemInfo);
				if (this._index == -1)
				{
					this._index = index;
				}
			}
			return !this._first || this._items.Count <= 0;
		}
	}
}
