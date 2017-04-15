using Assets.Scripts.Model;
using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class Unique_HeadPortrait : Sub_DropItemBase
	{
		public Unique_HeadPortrait() : base(DropItemID.Headportrait)
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
				MobaMessageManagerTools.GetItems_Exchange(ItemType.HeadPortrait, base.ItemCount.ToString(), true);
			}
			else
			{
				MobaMessageManagerTools.GetItems_HeadPortrait(base.ItemCount);
				ModelManager.Instance.GetNewAvatar("3", base.ItemCount.ToString());
			}
		}
	}
}
