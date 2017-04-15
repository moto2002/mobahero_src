using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class Currency_Trumpet : Sub_DropItemBase
	{
		public Currency_Trumpet() : base(DropItemID.Trumpet)
		{
		}

		public override void Init(DropItemData data)
		{
			base.Init(data);
		}

		public override void SetData()
		{
			MobaMessageManagerTools.GetItems_Coin(base.ItemCount);
			base.UserDATA.Speaker += base.ItemCount;
		}
	}
}
