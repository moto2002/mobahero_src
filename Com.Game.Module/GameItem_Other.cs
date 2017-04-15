using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class GameItem_Other : Sub_DropItemBase
	{
		public GameItem_Other() : base(DropItemID.OtherGameitem)
		{
		}

		public override void Init(DropItemData data)
		{
			base.Init(data);
		}

		public override void SetData()
		{
			MobaMessageManagerTools.GetItems_GameItem(base.ItemID.ToString());
		}
	}
}
