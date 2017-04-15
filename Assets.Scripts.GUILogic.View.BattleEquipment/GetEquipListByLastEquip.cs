using Com.Game.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public class GetEquipListByLastEquip : ITraversCallback
	{
		private List<string> _recordList;

		private List<string> _limitList;

		private int _depth;

		private bool _addFirst;

		public Func<SysBattleItemsVo, int, bool> TraversingCallback
		{
			get
			{
				return new Func<SysBattleItemsVo, int, bool>(this.callback);
			}
		}

		public object Result
		{
			get
			{
				return this._recordList;
			}
		}

		public GetEquipListByLastEquip(bool addFirst = true, int depth = 3, List<string> limitList = null)
		{
			this._recordList = new List<string>();
			this._limitList = ((limitList != null) ? new List<string>(limitList) : null);
			this._depth = depth;
			this._addFirst = addFirst;
		}

		private bool callback(SysBattleItemsVo info, int depth)
		{
			bool result = depth < this._depth;
			if (depth == 0)
			{
				this._recordList.Clear();
				if (!this._addFirst)
				{
					return result;
				}
			}
			if (info == null)
			{
				result = false;
			}
			else if (!this._recordList.Contains(info.items_id))
			{
				if (this._limitList == null || this._limitList.Contains(info.items_id))
				{
					this._recordList.Add(info.items_id);
				}
			}
			return result;
		}
	}
}
