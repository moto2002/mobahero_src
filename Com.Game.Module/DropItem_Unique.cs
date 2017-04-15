using Com.Game.Utils;
using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class DropItem_Unique : DropItemBase
	{
		public DropItem_Unique() : base(DropItemType.Unique)
		{
		}

		public override void Init(DropItemData data)
		{
			base.Init(data);
			Sub_DropItemBase sub_DropItemBase = null;
			switch (base.ItemID)
			{
			case 1:
				sub_DropItemBase = new Unique_Hero();
				break;
			case 2:
				sub_DropItemBase = new Unique_Skin();
				break;
			case 3:
				sub_DropItemBase = new Unique_HeadPortrait();
				break;
			case 4:
				sub_DropItemBase = new Unique_PortraitFrame();
				break;
			case 5:
				sub_DropItemBase = new Unique_Coupon();
				break;
			}
			if (sub_DropItemBase == null)
			{
				ClientLogger.Error("配置错误,找不到对应唯一物品类型:ItemID=1，2，3，4，5");
			}
			else
			{
				sub_DropItemBase.Init(data);
				sub_DropItemBase.SetData();
			}
		}
	}
}
