using System;

namespace MobaHeros.AI
{
	internal class StrategyHelper
	{
		private const float STAGE_1 = 5f;

		private const float STAGE_2 = 14f;

		public static int GetCurStage(float val)
		{
			if (val < 5f)
			{
				return 1;
			}
			if (val <= 14f && val >= 5f)
			{
				return 2;
			}
			return 3;
		}
	}
}
