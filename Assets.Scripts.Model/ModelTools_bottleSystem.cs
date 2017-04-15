using MobaProtocol.Data;
using System;

namespace Assets.Scripts.Model
{
	public static class ModelTools_bottleSystem
	{
		public static MagicBottleData Get_BottleData(this ModelManager mmng)
		{
			MagicBottleData result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_BottleSystem))
			{
				result = mmng.GetData<MagicBottleData>(EModelType.Model_BottleSystem);
			}
			return result;
		}

		public static long Get_BottleData_BottleID(this ModelManager mmng)
		{
			MagicBottleData magicBottleData = mmng.Get_BottleData();
			return magicBottleData.magicbottleid;
		}

		public static int Get_BottleData_Level(this ModelManager mmng)
		{
			MagicBottleData magicBottleData = mmng.Get_BottleData();
			return magicBottleData.curlevel;
		}

		public static long Get_BottleData_Exp(this ModelManager mmng)
		{
			MagicBottleData magicBottleData = mmng.Get_BottleData();
			return magicBottleData.curexp;
		}

		public static int Get_BottleData_Award(this ModelManager mmng)
		{
			MagicBottleData magicBottleData = mmng.Get_BottleData();
			return magicBottleData.drawaward;
		}

		public static int Get_BottleData_LegendCount(this ModelManager mmng)
		{
			MagicBottleData magicBottleData = mmng.Get_BottleData();
			return magicBottleData.generalbottle;
		}

		public static int Get_BottleData_CollectorCount(this ModelManager mmng)
		{
			MagicBottleData magicBottleData = mmng.Get_BottleData();
			return magicBottleData.classicbottle;
		}
	}
}
