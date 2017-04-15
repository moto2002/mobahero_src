using System;
using UnityEngine;

namespace GameLogin.State
{
	internal class LoginTask_playLogoMovie : LoginTaskBase
	{
		private float starTime;

		private float waitTime;

		private bool haveShowVideo;

		public LoginTask_playLogoMovie() : base(ELoginTask.ePlayLogoMovie, new object[0])
		{
			base.AddTask(null, new Action(this.PlayMovie));
		}

		public override void Update()
		{
			base.Update();
			if (GlobalSettings.isLoginByHoolaiSDK && !this.haveShowVideo && this.starTime + this.waitTime < Time.realtimeSinceStartup)
			{
				this.haveShowVideo = true;
				this.OnPlayFinish();
			}
		}

		private void PlayMovie()
		{
			if (GlobalSettings.isLoginByHoolaiSDK)
			{
				Handheld.PlayFullScreenMovie("xiaomeng.mp4", Color.black, FullScreenMovieControlMode.Hidden);
				this.waitTime = 3f;
				this.starTime = Time.realtimeSinceStartup;
				base.DoAction(ELoginAction.ePlayLogoMovieStart);
			}
			else
			{
				this.OnPlayFinish();
			}
		}

		private void OnPlayFinish()
		{
			base.DoAction(ELoginAction.ePlayLogoMovieFinish);
			base.Valid = false;
		}
	}
}
