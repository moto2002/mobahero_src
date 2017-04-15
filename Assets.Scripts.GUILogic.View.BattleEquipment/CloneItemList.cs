using Assets.Scripts.Model;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public class CloneItemList : ITraversListCallback
	{
		private List<ItemInfo> list;

		public object Result
		{
			get
			{
				return this.list;
			}
		}

		public Func<object, int, bool> TraversingCallback
		{
			get
			{
				return new Func<object, int, bool>(this.callback);
			}
		}

		public CloneItemList()
		{
			this.list = new List<ItemInfo>();
		}

		private bool callback(object obj, int index)
		{
			ItemInfo itemInfo = obj as ItemInfo;
			if (itemInfo != null)
			{
				this.list.Add(itemInfo.clone());
			}
			return true;
		}
	}
}
