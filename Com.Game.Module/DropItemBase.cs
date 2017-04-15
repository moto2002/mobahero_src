using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class DropItemBase
	{
		public DropItemType DropType
		{
			get;
			set;
		}

		public int ItemID
		{
			get;
			set;
		}

		public DropItemBase(DropItemType type)
		{
			this.DropType = type;
		}

		public virtual void Init(DropItemData data)
		{
			if (data == null)
			{
				return;
			}
			this.ItemID = data.itemId;
		}
	}
}
