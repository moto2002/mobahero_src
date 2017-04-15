using System;

namespace Assets.Scripts.Model
{
	public static class ModelTools_loginTime
	{
		private static LoginTime Get_loginTimeData(this ModelManager mmng)
		{
			LoginTime loginTime = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_loginTime))
			{
				loginTime = mmng.GetData<LoginTime>(EModelType.Model_loginTime);
			}
			if (loginTime == null)
			{
				loginTime = new LoginTime();
			}
			return loginTime;
		}

		public static TimeSpan Get_loginTime_diff_X(this ModelManager mmng)
		{
			LoginTime loginTime = mmng.Get_loginTimeData();
			return DateTime.Now - loginTime.loginTime_Local;
		}

		public static DateTime Get_loginTime_DataTime(this ModelManager mmng)
		{
			LoginTime loginTime = mmng.Get_loginTimeData();
			return loginTime.loginTime_Server;
		}

		public static bool Get_ServerTime_IsCorrected(this ModelManager mmng)
		{
			LoginTime loginTime = mmng.Get_loginTimeData();
			return loginTime.IsCorrected;
		}

		public static DateTime Get_ServerTimeCorrected(this ModelManager mmng)
		{
			LoginTime loginTime = mmng.Get_loginTimeData();
			return loginTime.ServerTimeCorrected + (DateTime.Now - loginTime.LocalTimeCorrected);
		}

		public static TimeSpan Get_LastBattleTime(this ModelManager mmng)
		{
			LoginTime loginTime = mmng.Get_loginTimeData();
			return loginTime.TimeInLastBattle;
		}

		public static void Set_Time_ExitBattle(this ModelManager mmng)
		{
			LoginTime loginTime = mmng.Get_loginTimeData();
			loginTime.TimeInLastBattle = ToolsFacade.ServerCurrentTime - loginTime.TimeEnterBattle;
		}

		public static void Set_Time_ClearBattleTimeRecord(this ModelManager mmng)
		{
			LoginTime loginTime = mmng.Get_loginTimeData();
			loginTime.TimeInLastBattle = TimeSpan.Zero;
		}
	}
}
