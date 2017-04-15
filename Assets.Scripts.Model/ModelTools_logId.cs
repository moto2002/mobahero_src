using System;

namespace Assets.Scripts.Model
{
	public static class ModelTools_logId
	{
		public static long Get_LogId_X(this ModelManager mmng)
		{
			return mmng.GetGet_LogId();
		}

		private static long GetGet_LogId(this ModelManager mmng)
		{
			long result = 0L;
			if (mmng != null && mmng.ValidData(EModelType.Model_logId))
			{
				result = mmng.GetData<long>(EModelType.Model_logId);
			}
			return result;
		}
	}
}
