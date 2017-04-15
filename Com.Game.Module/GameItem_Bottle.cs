using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class GameItem_Bottle : Sub_DropItemBase
	{
		public GameItem_Bottle() : base(DropItemID.Bottle)
		{
		}

		public override void Init(DropItemData data)
		{
			base.Init(data);
		}

		public override void SetData()
		{
			MobaMessageManagerTools.GetItems_Bottle(base.ItemCount);
		}
	}
}
