using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class GameItem_Rune : Sub_DropItemBase
	{
		public GameItem_Rune() : base(DropItemID.Rune)
		{
		}

		public override void Init(DropItemData data)
		{
			base.Init(data);
		}

		public override void SetData()
		{
			MobaMessageManagerTools.GetItems_Rune(base.ItemID);
		}
	}
}
