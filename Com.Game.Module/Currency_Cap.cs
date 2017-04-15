using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class Currency_Cap : Sub_DropItemBase
	{
		public Currency_Cap() : base(DropItemID.Cap)
		{
		}

		public override void Init(DropItemData data)
		{
			base.Init(data);
		}

		public override void SetData()
		{
			MobaMessageManagerTools.GetItems_Coin(base.ItemCount);
			base.UserDATA.SmallCap += base.ItemCount;
		}
	}
}
