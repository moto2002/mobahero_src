using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public static class ModelTools_DrawCardDataList
	{
		public static DrawCardModelData GetDrawData(this ModelManager mmng)
		{
			DrawCardModelData result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_DrawCardDataList))
			{
				result = mmng.GetData<DrawCardModelData>(EModelType.Model_DrawCardDataList);
			}
			return result;
		}

		public static List<DropItemData> Get_DrawData_X(this ModelManager mmng)
		{
			DrawCardModelData drawCardModelData = mmng.GetDrawData();
			if (drawCardModelData == null)
			{
				drawCardModelData = mmng.GetData<DrawCardModelData>(EModelType.Model_DrawCardDataList);
			}
			return drawCardModelData.normalDrop;
		}

		public static List<DropItemData> Get_RepeatData_X(this ModelManager mmng)
		{
			DrawCardModelData drawCardModelData = mmng.GetDrawData();
			if (drawCardModelData == null)
			{
				drawCardModelData = mmng.GetData<DrawCardModelData>(EModelType.Model_DrawCardDataList);
			}
			return drawCardModelData.repeatDrop;
		}
	}
}
