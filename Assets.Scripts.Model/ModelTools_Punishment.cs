using System;

namespace Assets.Scripts.Model
{
	public static class ModelTools_Punishment
	{
		public static DateTime Get_PunishmentEndTime(this ModelManager mmng)
		{
			if (mmng == null)
			{
				return default(DateTime);
			}
			return mmng.GetData<DateTime>(EModelType.Model_Punishment);
		}

		public static bool IsInPunishmentTime(this ModelManager mmng)
		{
			DateTime targetTime = mmng.Get_PunishmentEndTime();
			return !ToolsFacade.Instance.IsPastTime(targetTime);
		}
	}
}
