using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class RollbackInfo
	{
		public int _deltaMoney;

		public List<ItemInfo> _items;

		public List<string> _recommendItems;

		public RollbackInfo(int delaMoney, List<ItemInfo> itemsSnapshoot, List<string> recommendItems)
		{
			this._deltaMoney = delaMoney;
			this._items = itemsSnapshoot;
			this._recommendItems = recommendItems;
		}
	}
}
