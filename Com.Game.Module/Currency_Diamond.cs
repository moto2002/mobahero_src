using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class Currency_Diamond : Sub_DropItemBase
	{
		public Currency_Diamond() : base(DropItemID.Diamond)
		{
		}

		public override void Init(DropItemData data)
		{
			base.Init(data);
		}

		public override void SetData()
		{
			MobaMessageManagerTools.GetItems_Coin(base.ItemCount);
			base.UserDATA.Diamonds += (long)base.ItemCount;
		}
	}
}
