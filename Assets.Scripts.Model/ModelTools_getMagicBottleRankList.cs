using MobaProtocol.Data;
using System;

namespace Assets.Scripts.Model
{
	public static class ModelTools_getMagicBottleRankList
	{
		public static MagicBottleRankList Get_GetMagicBottleRankList_X(this ModelManager mmng)
		{
			return mmng.GetMagicBottleRankList();
		}

		private static MagicBottleRankList GetMagicBottleRankList(this ModelManager mmng)
		{
			MagicBottleRankList result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_getMagicBottleRankList))
			{
				MagicBottleRankList data = mmng.GetData<MagicBottleRankList>(EModelType.Model_getMagicBottleRankList);
				if (data != null)
				{
					result = data;
				}
			}
			return result;
		}
	}
}
