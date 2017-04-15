using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Game.Module
{
	public class Unique_Hero : Sub_DropItemBase
	{
		public Unique_Hero() : base(DropItemID.Hero)
		{
		}

		public override void Init(DropItemData data)
		{
			base.Init(data);
		}

		public override void SetData()
		{
			List<HeroInfoData> heroinfoList = ModelManager.Instance.GetDrawData().heroinfoList;
			Dictionary<string, SysHeroMainVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysHeroMainVo>();
			string text = string.Empty;
			if (typeDicByType != null)
			{
				text = typeDicByType.Values.FirstOrDefault((SysHeroMainVo obj) => obj.hero_id == base.ItemCount).npc_id;
			}
			if (base.RepeatList.Exists((DropItemData x) => x.itemCount == base.ItemCount))
			{
				MobaMessageManagerTools.GetItems_Exchange(ItemType.Hero, text, true);
			}
			else
			{
				MobaMessageManagerTools.GetItems_Hero(text);
			}
			if (heroinfoList != null && heroinfoList.Count != 0)
			{
				foreach (HeroInfoData current in ModelManager.Instance.Get_heroInfo_list_X())
				{
					foreach (HeroInfoData current2 in heroinfoList)
					{
						if (current.HeroId != current2.HeroId)
						{
							ModelManager.Instance.Get_heroInfo_list_X().Add(current2);
						}
					}
				}
			}
		}
	}
}
