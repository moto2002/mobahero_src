using Com.Game.Utils;
using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class DropItem_GameBuff : DropItemBase
	{
		public DropItem_GameBuff() : base(DropItemType.Gamebuff)
		{
		}

		public override void Init(DropItemData data)
		{
			base.Init(data);
			Sub_DropItemBase sub_DropItemBase = new GameBuff_DoubleCard();
			if (sub_DropItemBase == null)
			{
				ClientLogger.Error("配置错误,找不到对应即时生效物品类型:名前は=DoubleCard");
			}
			else
			{
				sub_DropItemBase.Init(data);
				sub_DropItemBase.SetData();
			}
		}
	}
}
