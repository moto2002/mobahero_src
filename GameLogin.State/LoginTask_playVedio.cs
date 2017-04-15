using MobaMessageData;
using System;
using System.Collections.Generic;

namespace GameLogin.State
{
	internal class LoginTask_playVedio : LoginTaskBase
	{
		internal class VideoTimePair
		{
			public bool IsDeafult;

			public string LogoVideoName;

			public string NormalVideoName;

			public string LoopVideoName;

			public DateTime StartTime;

			public DateTime EndTime;

			public static LoginTask_playVedio.VideoTimePair GetCurVideoPair(LoginTask_playVedio.VideoTimePair[] pairArr)
			{
				LoginTask_playVedio.VideoTimePair videoTimePair = null;
				if (pairArr != null)
				{
					DateTime now = DateTime.Now;
					for (int i = 0; i < pairArr.Length; i++)
					{
						if (pairArr[i].IsDeafult && videoTimePair == null)
						{
							videoTimePair = pairArr[i];
						}
						else if (now >= pairArr[i].StartTime && now < pairArr[i].EndTime)
						{
							videoTimePair = pairArr[i];
							break;
						}
					}
				}
				if (videoTimePair == null)
				{
					videoTimePair = new LoginTask_playVedio.VideoTimePair
					{
						LogoVideoName = "xiaomeng",
						NormalVideoName = "login",
						LoopVideoName = "login_loop"
					};
				}
				return videoTimePair;
			}
		}

		private const int Player = 1;

		private LoginTask_playVedio.VideoTimePair currentVideo;

		private readonly LoginTask_playVedio.VideoTimePair[] avaliableVideos = new LoginTask_playVedio.VideoTimePair[]
		{
			new LoginTask_playVedio.VideoTimePair
			{
				IsDeafult = true,
				LogoVideoName = "xiaomeng",
				NormalVideoName = "login",
				LoopVideoName = "login_loop"
			},
			new LoginTask_playVedio.VideoTimePair
			{
				LogoVideoName = "xiaomeng",
				NormalVideoName = "login1",
				LoopVideoName = "login1_loop",
				StartTime = new DateTime(2017, 1, 20, 0, 0, 0),
				EndTime = new DateTime(2017, 2, 12, 0, 0, 0)
			}
		};

		private LoginTask_playVedio.VideoTimePair CurrentVideo
		{
			get
			{
				LoginTask_playVedio.VideoTimePair arg_21_0;
				if ((arg_21_0 = this.currentVideo) == null)
				{
					arg_21_0 = (this.currentVideo = LoginTask_playVedio.VideoTimePair.GetCurVideoPair(this.avaliableVideos));
				}
				return arg_21_0;
			}
		}

		public LoginTask_playVedio() : base(ELoginTask.ePlayVedio, new object[]
		{
			ClientC2C.Vedio_onStop,
			ClientC2C.Vedio_onStart,
			ClientC2C.Vedio_onReady,
			ClientC2C.Login_Action
		})
		{
			base.AddTask(new List<ELoginAction>
			{
				ELoginAction.ePlayLogoMovieFinish
			}, new Action(this.PrepareVedio));
			base.AddTask(new List<ELoginAction>
			{
				ELoginAction.eVedio1Ready
			}, new Action(this.PlayVedio));
		}

		private void PrepareVedio()
		{
			MobaMessageManagerTools.VedioPlay_Vedio_creatPlayer(1, true);
			MobaMessageManagerTools.VedioPlay_Vedio_setName(1, this.CurrentVideo.NormalVideoName);
			MobaMessageManagerTools.VedioPlay_Vedio_loop(1, false);
		}

		private void PlayVedio()
		{
			MobaMessageManagerTools.VedioPlay_Vedio_play(1);
		}

		private void OnMsg_Vedio_onReady(MobaMessage msg)
		{
			MsgData_VedioCallback msgData_VedioCallback = msg.Param as MsgData_VedioCallback;
			if (msgData_VedioCallback != null)
			{
				if (this.CurrentVideo.LogoVideoName.Equals(msgData_VedioCallback.vedioName))
				{
					MobaMessageManagerTools.VedioPlay_Vedio_play(1);
				}
				else if (this.CurrentVideo.NormalVideoName.Equals(msgData_VedioCallback.vedioName))
				{
					base.DoAction(ELoginAction.eVedio1Ready);
				}
				else if (this.CurrentVideo.LoopVideoName.Equals(msgData_VedioCallback.vedioName))
				{
					MobaMessageManagerTools.VedioPlay_Vedio_play(1);
				}
			}
		}

		private void OnMsg_Vedio_onStart(MobaMessage msg)
		{
			MsgData_VedioCallback msgData_VedioCallback = msg.Param as MsgData_VedioCallback;
			if (msgData_VedioCallback != null && this.CurrentVideo.LoopVideoName.Equals(msgData_VedioCallback.vedioName))
			{
				base.DoAction(ELoginAction.eVedio2Start);
			}
		}

		private void OnMsg_Vedio_onStop(MobaMessage msg)
		{
			MsgData_VedioCallback msgData_VedioCallback = msg.Param as MsgData_VedioCallback;
			if (msgData_VedioCallback != null)
			{
				if (this.CurrentVideo.LogoVideoName.Equals(msgData_VedioCallback.vedioName))
				{
					MobaMessageManagerTools.VedioPlay_Vedio_setName(1, this.CurrentVideo.NormalVideoName);
					MobaMessageManagerTools.VedioPlay_Vedio_loop(1, false);
				}
				else if (this.CurrentVideo.NormalVideoName.Equals(msgData_VedioCallback.vedioName))
				{
					MobaMessageManagerTools.VedioPlay_Vedio_setName(1, this.CurrentVideo.LoopVideoName);
					MobaMessageManagerTools.VedioPlay_Vedio_loop(1, true);
					base.DoAction(ELoginAction.eVedio1Finish);
				}
			}
		}
	}
}
