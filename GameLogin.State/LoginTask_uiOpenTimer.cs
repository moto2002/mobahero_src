using Com.Game.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogin.State
{
	internal class LoginTask_uiOpenTimer : LoginTaskBase
	{
		private float startTime;

		private float waitTime = 10f;

		private bool isTimerStart;

		public LoginTask_uiOpenTimer() : base(ELoginTask.eUiOpenTimer, new object[]
		{
			ClientC2C.Login_Action
		})
		{
			base.AddTask(new List<ELoginAction>
			{
				ELoginAction.ePlayLogoMovieFinish
			}, new Action(this.StartTimer));
			base.AddTask(new List<ELoginAction>
			{
				ELoginAction.eVedio2Start
			}, new Action(this.StopTimer));
		}

		public override void Update()
		{
			base.Update();
			if (this.isTimerStart && this.startTime + this.waitTime < Time.realtimeSinceStartup)
			{
				this.OnTimerFinished();
			}
		}

		private void StartTimer()
		{
			this.isTimerStart = true;
			this.startTime = Time.realtimeSinceStartup;
		}

		private void StopTimer()
		{
			this.isTimerStart = false;
			base.Valid = false;
		}

		private void OnTimerFinished()
		{
			this.StopTimer();
			base.DoAction(ELoginAction.eVedio2Start);
			string bgPath = (!ToolsFacade.Instance.IsInNewYearTime(DateTime.Now)) ? "Texture/LoginTex/LoginBg" : "Texture/LoginTex/LoginBg1";
			Singleton<BgView>.Instance.SetBgPath(bgPath);
			CtrlManager.OpenWindow(WindowID.BgView, null);
		}
	}
}
