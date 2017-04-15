using Assets.Scripts.Model;
using System;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public class FindItem_first_most : ITraversListCallback
	{
		private string _targetID;

		private int _index;

		private ItemInfo _tempItem;

		public object Result
		{
			get
			{
				return this._index;
			}
		}

		public Func<object, int, bool> TraversingCallback
		{
			get
			{
				return new Func<object, int, bool>(this.callback);
			}
		}

		public FindItem_first_most(string targetID)
		{
			this._index = -1;
			this._targetID = targetID;
		}

		private bool callback(object obj, int index)
		{
			ItemInfo itemInfo = obj as ItemInfo;
			if (itemInfo != null && this._targetID == itemInfo.ID && itemInfo.Num < itemInfo.Config.superimposition)
			{
				if (this._index == -1)
				{
					this._index = index;
					this._tempItem = itemInfo;
				}
				else if (itemInfo.Num > this._tempItem.Num)
				{
					this._index = index;
					this._tempItem = itemInfo;
				}
			}
			return true;
		}
	}
}
