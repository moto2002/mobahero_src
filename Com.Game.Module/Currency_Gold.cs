using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class Currency_Gold : Sub_DropItemBase
	{
		public Currency_Gold() : base(DropItemID.Gold)
		{
		}

		public override void Init(DropItemData data)
		{
			base.Init(data);
		}

		public override void SetData()
		{
			MobaMessageManagerTools.GetItems_Coin(base.ItemCount);
			base.UserDATA.Money += (long)base.ItemCount;
		}
	}
}
