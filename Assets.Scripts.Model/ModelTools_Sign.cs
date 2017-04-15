using System;

namespace Assets.Scripts.Model
{
	public static class ModelTools_Sign
	{
		public static SignState Get_GetSignDay_X(this ModelManager mmng)
		{
			return mmng.GetSignDay_X();
		}

		private static SignState GetSignDay_X(this ModelManager mmng)
		{
			SignState result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_Sign))
			{
				SignState data = mmng.GetData<SignState>(EModelType.Model_Sign);
				if (data != null)
				{
					result = data;
				}
			}
			return result;
		}

		public static DateTime Get_SignDayRecordTime(this ModelManager mmng)
		{
			SignState signDay_X = mmng.GetSignDay_X();
			return (signDay_X != null) ? signDay_X.dataReceiveTime : default(DateTime);
		}
	}
}
