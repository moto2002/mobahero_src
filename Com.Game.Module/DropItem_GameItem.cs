using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class DropItem_GameItem : DropItemBase
	{
		public DropItem_GameItem() : base(DropItemType.Gameitem)
		{
		}

		public override void Init(DropItemData data)
		{
			base.Init(data);
			Sub_DropItemBase sub_DropItemBase;
			if (base.ItemID == 7777)
			{
				sub_DropItemBase = new GameItem_Bottle();
			}
			else
			{
				SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(base.ItemID.ToString());
				if (dataById == null)
				{
					return;
				}
				if (dataById.type == 4)
				{
					sub_DropItemBase = new GameItem_Rune();
				}
				else
				{
					sub_DropItemBase = new GameItem_Other();
				}
			}
			if (sub_DropItemBase == null)
			{
				ClientLogger.Error("配置错误,找不到对应gameitem类型:ItemID=7777,gameitem.type=4");
			}
			else
			{
				sub_DropItemBase.Init(data);
				sub_DropItemBase.SetData();
			}
		}
	}
}
