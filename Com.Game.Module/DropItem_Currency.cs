using Com.Game.Utils;
using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class DropItem_Currency : DropItemBase
	{
		public DropItem_Currency() : base(DropItemType.Currency)
		{
		}

		public override void Init(DropItemData data)
		{
			base.Init(data);
			Sub_DropItemBase sub_DropItemBase = null;
			int itemID = base.ItemID;
			switch (itemID)
			{
			case 9:
				sub_DropItemBase = new Currency_Cap();
				goto IL_69;
			case 10:
				IL_25:
				if (itemID == 1)
				{
					sub_DropItemBase = new Currency_Gold();
					goto IL_69;
				}
				if (itemID != 2)
				{
					goto IL_69;
				}
				sub_DropItemBase = new Currency_Diamond();
				goto IL_69;
			case 11:
				sub_DropItemBase = new Currency_Trumpet();
				goto IL_69;
			}
			goto IL_25;
			IL_69:
			if (sub_DropItemBase == null)
			{
				ClientLogger.Error("配置错误,找不到对应货币类型:ItemID=1，2，9，11");
			}
			else
			{
				sub_DropItemBase.Init(data);
				sub_DropItemBase.SetData();
			}
		}
	}
}
