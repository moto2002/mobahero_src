using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public static class ModelTools_heroInfoList
	{
		public static List<HeroInfoData> Get_heroInfo_list_X(this ModelManager mmng)
		{
			List<HeroInfoData> list = new List<HeroInfoData>();
			return mmng.Get_heroInfo_list();
		}

		public static HeroInfoData Get_heroInfo_item_byModelID_X(this ModelManager mmng, string modelID)
		{
			return mmng.Get_heroInfo_item_byModelID(modelID);
		}

		public static HeroInfoData Get_heroInfo_item_byHeroID_X(this ModelManager mmng, long herolID)
		{
			return mmng.Get_heroInfo_item_byHeroID(herolID);
		}

		public static int Get_heroInfo_curskin_byHeroID_X(this ModelManager mmng, long heroid)
		{
			HeroInfoData heroInfoData = mmng.Get_heroInfo_item_byHeroID(heroid);
			return heroInfoData.CurrSkin;
		}

		public static bool IsWearSkin(this ModelManager mmng, long heroid, int skinid)
		{
			HeroInfoData heroInfoData = mmng.Get_heroInfo_item_byHeroID(heroid);
			return heroInfoData != null && heroInfoData.CurrSkin == skinid;
		}

		public static void Set_HeroRunes(this ModelManager mmng, string heroName, int position, int modelid)
		{
			HeroInfoData heroInfoData = mmng.Get_heroInfo_item_byModelID(heroName);
			if (heroInfoData != null)
			{
				switch (position)
				{
				case 1:
					heroInfoData.Ep_1 = modelid;
					break;
				case 2:
					heroInfoData.Ep_2 = modelid;
					break;
				case 3:
					heroInfoData.Ep_3 = modelid;
					break;
				case 4:
					heroInfoData.Ep_4 = modelid;
					break;
				case 5:
					heroInfoData.Ep_5 = modelid;
					break;
				case 6:
					heroInfoData.Ep_6 = modelid;
					break;
				}
			}
		}

		public static void Set_HeroRunes(this ModelManager mmng, string heroName)
		{
			HeroInfoData heroInfoData = mmng.Get_heroInfo_item_byModelID(heroName);
			if (heroInfoData != null)
			{
				heroInfoData.Ep_1 = 0;
				heroInfoData.Ep_2 = 0;
				heroInfoData.Ep_3 = 0;
				heroInfoData.Ep_4 = 0;
				heroInfoData.Ep_5 = 0;
				heroInfoData.Ep_6 = 0;
			}
		}

		public static bool IsRunesEquipNull(this ModelManager mmng, string heroName)
		{
			HeroInfoData heroInfoData = mmng.Get_heroInfo_item_byModelID(heroName);
			return heroInfoData.Ep_1 == 0 && heroInfoData.Ep_2 == 0 && heroInfoData.Ep_3 == 0 && heroInfoData.Ep_4 == 0 && heroInfoData.Ep_5 == 0 && heroInfoData.Ep_6 == 0;
		}

		private static List<HeroInfoData> Get_heroInfo_list(this ModelManager mmng)
		{
			List<HeroInfoData> result = new List<HeroInfoData>();
			if (mmng != null && mmng.ValidData(EModelType.Model_heroInfoList))
			{
				result = mmng.GetData<List<HeroInfoData>>(EModelType.Model_heroInfoList);
			}
			return result;
		}

		private static List<HeroInfoData> Get_heroInfo_list_old(this ModelManager mmng)
		{
			return NetWorkHelper.Instance.client.m_user.HeroInfoList;
		}

		private static HeroInfoData Get_heroInfo_item_byModelID_old(this ModelManager mmng, string modelID)
		{
			return NetWorkHelper.Instance.client.m_user.HeroInfoList.Find((HeroInfoData obj) => obj.ModelId.ToString() == modelID);
		}

		private static HeroInfoData Get_heroInfo_item_byModelID(this ModelManager mmng, string modelID)
		{
			HeroInfoData result = null;
			List<HeroInfoData> list = mmng.Get_heroInfo_list();
			if (list != null)
			{
				result = list.Find((HeroInfoData obj) => obj.ModelId.ToString() == modelID);
			}
			return result;
		}

		private static HeroInfoData Get_heroInfo_item_byHeroID(this ModelManager mmng, long herolID)
		{
			List<HeroInfoData> list = mmng.Get_heroInfo_list();
			return list.Find((HeroInfoData obj) => obj.HeroId == herolID);
		}

		private static HeroInfoData Get_heroInfo_item_byHeroID_old(this ModelManager mmng, long heroID)
		{
			return NetWorkHelper.Instance.client.m_user.HeroInfoList.Find((HeroInfoData obj) => obj.HeroId == heroID);
		}
	}
}
