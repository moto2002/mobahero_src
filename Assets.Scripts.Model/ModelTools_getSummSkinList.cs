using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Model
{
	public static class ModelTools_getSummSkinList
	{
		public static List<SummSkinData> GetSummSkinList(this ModelManager mmng)
		{
			return mmng._getSummSkinList();
		}

		public static bool IsPossessSkin(this ModelManager mmng, int skinId)
		{
			if (mmng.GetSummSkinList() == null)
			{
				return false;
			}
			SummSkinData summSkinData = mmng.GetSummSkinList().Find((SummSkinData obj) => obj.SkinId == skinId);
			return summSkinData != null;
		}

		public static List<int> GetHeroSkinList(this ModelManager mmng, string heroId)
		{
			List<SummSkinData> summSkinList = mmng.GetSummSkinList();
			List<SummSkinData> list = (from obj in summSkinList
			where obj.NpcId == heroId
			select obj).ToList<SummSkinData>();
			List<int> list2 = new List<int>();
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					list2.Add(list[i].SkinId);
				}
			}
			return list2;
		}

		public static bool IsPossessHeroIdSkin(this ModelManager mmng, string heroId, int skinId)
		{
			List<int> heroSkinList = mmng.GetHeroSkinList(heroId);
			return heroSkinList.Contains(skinId);
		}

		private static List<SummSkinData> _getSummSkinList(this ModelManager mmng)
		{
			List<SummSkinData> result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_getSummSkinList))
			{
				result = mmng.GetData<List<SummSkinData>>(EModelType.Model_getSummSkinList);
			}
			return result;
		}

		public static void GetNewHeroSkin(this ModelManager mmng, int skinId)
		{
			SysHeroSkinVo dataById = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(skinId.ToString());
			if (dataById == null)
			{
				ClientLogger.Error("Skin Model: SkinId Illegal.");
				return;
			}
			SummSkinData item = new SummSkinData
			{
				SkinId = skinId,
				NpcId = dataById.npc_id,
				SummId = 0L
			};
			mmng.GetSummSkinList().Add(item);
		}
	}
}
