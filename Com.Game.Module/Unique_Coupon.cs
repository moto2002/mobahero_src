using Assets.Scripts.Model;
using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class Unique_Coupon : Sub_DropItemBase
	{
		public Unique_Coupon() : base(DropItemID.Coupon)
		{
		}

		public override void Init(DropItemData data)
		{
			base.Init(data);
		}

		public override void SetData()
		{
			if (base.RepeatList.Exists((DropItemData x) => x.itemCount == base.ItemCount))
			{
				MobaMessageManagerTools.GetItems_Exchange(ItemType.Coupon, base.ItemCount.ToString(), true);
			}
			else
			{
				MobaMessageManagerTools.GetItems_Coupon(base.ItemCount.ToString());
				ModelManager.Instance.GetNewCoupon(base.ItemCount.ToString());
			}
		}
	}
}
