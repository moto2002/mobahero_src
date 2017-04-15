using System;

namespace Assets.Scripts.Model
{
	public class LoginTime
	{
		public DateTime loginTime_Local;

		public DateTime loginTime_Server;

		public TimeSpan TimeSpan_login;

		public bool IsCorrected;

		public DateTime ServerTimeCorrected;

		public DateTime LocalTimeCorrected;

		public DateTime TimeEnterBattle;

		public TimeSpan TimeInLastBattle = TimeSpan.Zero;

		public LoginTime()
		{
			this.loginTime_Local = DateTime.Now;
			this.TimeSpan_login = new TimeSpan(0L);
		}
	}
}
