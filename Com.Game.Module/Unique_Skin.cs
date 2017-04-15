using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class Unique_Skin : Sub_DropItemBase
	{
		public Unique_Skin() : base(DropItemID.Skin)
		{
		}

		public override void Init(DropItemData data)
		{
			base.Init(data);
		}

		public override void SetData()
		{
			if (base.RepeatList.Exists((DropItemData x) => x.itemCount == this.ItemCount))
			{
				MobaMessageManagerTools.GetItems_Exchange(ItemType.HeroSkin, base.ItemCount.ToString(), true);
			}
			else
			{
				MobaMessageManagerTools.GetItems_HeroSkin(base.ItemCount);
			}
			SummSkinData temp = new SummSkinData
			{
				NpcId = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(base.ItemCount.ToString()).npc_id,
				SkinId = base.ItemCount,
				SummId = base.UserDATA.SummonerId
			};
			if (ModelManager.Instance.GetSummSkinList().Find((SummSkinData obj) => obj.SkinId == temp.SkinId) == null)
			{
				ModelManager.Instance.GetSummSkinList().Add(temp);
			}
		}
	}
}
