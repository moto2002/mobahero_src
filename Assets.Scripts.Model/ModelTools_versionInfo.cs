using System;

namespace Assets.Scripts.Model
{
	public static class ModelTools_versionInfo
	{
		public static ModelVersionInfo Get_versionInfo(this ModelManager mmng)
		{
			return mmng.GetData<ModelVersionInfo>(EModelType.Model_versionInfo);
		}

		public static string Get_versionStr(this ModelManager mmng)
		{
			string result = string.Empty;
			ModelVersionInfo modelVersionInfo = mmng.Get_versionInfo();
			if (modelVersionInfo != null)
			{
				result = modelVersionInfo.versionStr;
			}
			return result;
		}
	}
}
