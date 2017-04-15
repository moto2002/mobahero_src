using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public static class ModelTools_equipmentList
	{
		public static List<EquipmentInfoData> Get_equipmentList_X(this ModelManager mmng)
		{
			List<EquipmentInfoData> list = mmng.GetEquipmentList();
			if (list == null)
			{
				list = mmng.GetData<List<EquipmentInfoData>>(EModelType.Model_equipmentList);
			}
			return list;
		}

		private static List<EquipmentInfoData> GetEquipmentList(this ModelManager mmng)
		{
			List<EquipmentInfoData> result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_equipmentList))
			{
				result = mmng.GetData<List<EquipmentInfoData>>(EModelType.Model_equipmentList);
			}
			return result;
		}
	}
}
