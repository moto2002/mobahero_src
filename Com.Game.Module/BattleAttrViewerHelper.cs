using System;

namespace Com.Game.Module
{
	internal class BattleAttrViewerHelper
	{
		private static BattleAttrViewerHelper _instance;

		public static BattleAttrViewerHelper Instance
		{
			get
			{
				if (BattleAttrViewerHelper._instance == null)
				{
					BattleAttrViewerHelper._instance = new BattleAttrViewerHelper();
				}
				return BattleAttrViewerHelper._instance;
			}
		}

		public void CreateSphere(Units unit)
		{
		}

		public void CreateExpFont(Units unit)
		{
		}

		public void CreateHolo()
		{
		}

		public void CreateDurationLight()
		{
		}
	}
}
