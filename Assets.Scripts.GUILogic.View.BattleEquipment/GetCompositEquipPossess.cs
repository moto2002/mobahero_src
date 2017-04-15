using Assets.Scripts.Model;
using Com.Game.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public class GetCompositEquipPossess : ITraversCallback
	{
		private List<ItemInfo> equip_has;

		private List<string> equip_need;

		private int _depth;

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
				return this.equip_need;
			}
		}

		public GetCompositEquipPossess(string equipTarget, List<ItemInfo> ownEquip, int depth = 3)
		{
			this.equip_has = new List<ItemInfo>();
			this.equip_need = new List<string>();
			this._depth = depth;
			for (int i = 0; i < ownEquip.Count; i++)
			{
				if (equipTarget != ownEquip[i].ID)
				{
					this.equip_has.Add(ownEquip[i].clone());
				}
			}
		}

		private bool callback(SysBattleItemsVo info, int depth)
		{
			bool result = depth < this._depth;
			if (depth == 0)
			{
				this.equip_need.Clear();
			}
			else if (info != null)
			{
				ItemInfo target;
				int index;
				if (BattleEquipTools_Travers.GetItem_first(this.equip_has, info.items_id, out target, out index))
				{
					this.equip_need.Add(info.items_id);
					BattleEquipTools_op.RemoveItem(this.equip_has, target, index);
					result = false;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
