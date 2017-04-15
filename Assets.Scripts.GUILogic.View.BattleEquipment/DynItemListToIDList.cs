using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public class DynItemListToIDList : ITraversListCallback
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

		public DynItemListToIDList()
		{
			this.list = new List<string>();
		}

		private bool callback(object obj, int index)
		{
			ItemDynData itemDynData = obj as ItemDynData;
			if (itemDynData != null)
			{
				this.list.Add(itemDynData.typeId);
			}
			return true;
		}
	}
}
