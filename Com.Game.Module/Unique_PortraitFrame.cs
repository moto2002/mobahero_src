using Assets.Scripts.Model;
using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class Unique_PortraitFrame : Sub_DropItemBase
	{
		public Unique_PortraitFrame() : base(DropItemID.Portraitrame)
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
				MobaMessageManagerTools.GetItems_Exchange(ItemType.PortraitFrame, base.ItemCount.ToString(), true);
			}
			else
			{
				MobaMessageManagerTools.GetItems_PortraitFrame(base.ItemCount.ToString());
				ModelManager.Instance.GetNewAvatar("4", base.ItemCount.ToString());
			}
		}
	}
}
