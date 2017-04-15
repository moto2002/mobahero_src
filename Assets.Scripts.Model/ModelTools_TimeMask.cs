using System;

namespace Assets.Scripts.Model
{
	public static class ModelTools_TimeMask
	{
		public static TimeClass GetTimeClass(this ModelManager mmng)
		{
			TimeClass result = new TimeClass();
			if (mmng != null && mmng.ValidData(EModelType.Model_TimeMask))
			{
				result = mmng.GetData<TimeClass>(EModelType.Model_TimeMask);
			}
			return result;
		}
	}
}
