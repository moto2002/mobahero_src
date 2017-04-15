using Assets.Scripts.Model;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public class ItemListToIDList : ITraversListCallback
	{
		private List<string> list;

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

		public ItemListToIDList()
		{
			this.list = new List<string>();
		}

		private bool callback(object obj, int index)
		{
			ItemInfo itemInfo = obj as ItemInfo;
			if (itemInfo != null)
			{
				this.list.Add(itemInfo.ID);
			}
			return true;
		}
	}
}
