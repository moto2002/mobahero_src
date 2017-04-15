using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public static class ModelTools_DoubleExpGold
	{
		public static List<DoubleCardData> GetDoubleCardData(this ModelManager mmng)
		{
			List<DoubleCardData> result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_DoubleExpGold))
			{
				result = mmng.GetData<List<DoubleCardData>>(EModelType.Model_DoubleExpGold);
			}
			return result;
		}

		public static DoubleCardData Get_Certain_CardData_X(this ModelManager mmng, int type, int recordtype)
		{
			List<DoubleCardData> doubleCardData = mmng.GetDoubleCardData();
			DoubleCardData result = null;
			if (doubleCardData != null)
			{
				result = doubleCardData.Find((DoubleCardData obj) => obj.type == type && obj.recordtype == recordtype);
			}
			return result;
		}

		public static List<DoubleCardData> Get_All_DoubleCardData_X(this ModelManager mmng)
		{
			List<DoubleCardData> list = mmng.GetDoubleCardData();
			if (list == null)
			{
				list = mmng.GetData<List<DoubleCardData>>(EModelType.Model_DoubleExpGold);
			}
			return list;
		}

		public static void RemoveCertainCardData(this ModelManager mmng, int type, int recordtype)
		{
			List<DoubleCardData> doubleCardData = mmng.GetDoubleCardData();
			if (doubleCardData != null)
			{
				DoubleCardData doubleCardData2 = doubleCardData.Find((DoubleCardData obj) => obj.type == type && obj.recordtype == recordtype);
				if (doubleCardData2 != null)
				{
					doubleCardData.Remove(doubleCardData2);
				}
			}
		}

		public static void SetTestDoubleCardData(this ModelManager mmng, DoubleCardData dcd)
		{
			List<DoubleCardData> doubleCardData = mmng.GetDoubleCardData();
			doubleCardData.Add(dcd);
		}

		public static void RemoveWinCard(this ModelManager mmng)
		{
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				return;
			}
			if (Singleton<PvpManager>.Instance.JoinType == PvpJoinType.SefDefineGame)
			{
				return;
			}
			List<DoubleCardData> doubleCardData = mmng.GetDoubleCardData();
			List<DoubleCardData> list = null;
			if (doubleCardData != null)
			{
				list = doubleCardData.FindAll((DoubleCardData obj) => obj.recordtype == 2);
			}
			if (list != null && list.Count != 0)
			{
				SysGameBuffVo dataById = BaseDataMgr.instance.GetDataById<SysGameBuffVo>(list[0].modelid.ToString());
				if (!dataById.effect_battle_scene.Contains(LevelManager.m_CurLevel.level_id))
				{
					return;
				}
				for (int num = list.Count - 1; num != -1; num--)
				{
					list[num].recordvalue--;
					if (list[num].recordvalue <= 0)
					{
						list.RemoveAt(num);
					}
				}
			}
		}
	}
}
