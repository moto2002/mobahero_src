using Com.Game.Module;
using MobaMessageData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogin.State
{
	internal class LoginTask_loadView : LoginTaskBase
	{
		private bool bDownLoadFinish;

		private bool bLoadFinish;

		private bool bProgressFinish;

		private float bSize;

		private float rSize;

		private float lastCSize;

		private float lastTime;

		private float bProgress;

		private float rProgress;

		private int tNum;

		private int cNum;

		private int n;

		public LoginTask_loadView() : base(ELoginTask.eLoadView, new object[]
		{
			ClientC2C.DownLoadBinDataProgress,
			ClientC2C.DownLoadResourceTotalProgress,
			ClientV2C.LoadView2_subComplete,
			ClientC2C.Login_Action
		})
		{
			this.bSize = 0f;
			this.rSize = 0f;
			this.lastCSize = 0f;
			this.lastTime = 0f;
			this.tNum = -1;
			this.cNum = -2;
			base.AddTask(null, new Action(this.ShowLoadView));
			base.AddTask(new List<ELoginAction>
			{
				ELoginAction.eCheckDownload
			}, new Action(this.OnCheckFinish));
			base.AddTask(new List<ELoginAction>
			{
				ELoginAction.eDownLoadResourceFinish
			}, new Action(this.OnDownLoadFinish));
			base.AddTask(new List<ELoginAction>
			{
				ELoginAction.eInitData
			}, new Action(this.OnLoadFinish));
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void ShowLoadView()
		{
			CtrlManager.OpenWindow(WindowID.LoadView2, null);
			MobaMessageManagerTools.LoadView2_setText2("下载资源...", false);
			MobaMessageManagerTools.LoadView2_setProgress2(0, this.tNum, this.cNum, null, MsgData_LoadView2_setProgress.SetType.targetNum);
			base.DoAction(ELoginAction.eShowLoadView);
		}

		private void OnCheckFinish()
		{
			this.bSize = LoginStateManager.Instance.BSize;
			this.rSize = LoginStateManager.Instance.RSize;
		}

		private void OnDownLoadFinish()
		{
			this.bDownLoadFinish = true;
			MobaMessageManagerTools.LoadView2_setProgress2(100, this.tNum, this.cNum, null, MsgData_LoadView2_setProgress.SetType.targetNum);
			this.LoadResource();
		}

		private void LoadResource()
		{
			if (this.bDownLoadFinish && this.bProgressFinish)
			{
				this.bProgressFinish = false;
				this.cNum = -1;
				string str = "加载资源";
				MobaMessageManagerTools.LoadView2_setProgress2(0, this.tNum, this.cNum, str, MsgData_LoadView2_setProgress.SetType.targetNum);
				base.DoAction(ELoginAction.eBeginLoad);
			}
		}

		private void OnLoadFinish()
		{
			this.bLoadFinish = true;
			MobaMessageManagerTools.LoadView2_setProgress2(10, this.tNum, this.cNum, null, MsgData_LoadView2_setProgress.SetType.targetNum);
			this.EnterGame();
		}

		private void EnterGame()
		{
			LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_success);
			base.Valid = false;
		}

		private void OnMsg_LoadView2_subComplete(MobaMessage msg)
		{
			this.n++;
			this.bProgressFinish = true;
			if (this.n == 1)
			{
				this.LoadResource();
			}
		}

		private void OnMsg_DownLoadBinDataProgress(MobaMessage msg)
		{
			this.bProgress = (float)msg.Param;
		}

		private void OnMsg_DownLoadResourceTotalProgress(MobaMessage msg)
		{
			this.rProgress = (float)msg.Param;
			this.RefreshUI_loadView();
		}

		private void RefreshUI_loadView()
		{
			float num = 1f;
			float num2 = this.bSize + this.rSize;
			float num3 = this.bSize * this.bProgress + this.rSize * this.rProgress;
			float num4 = (num3 - this.lastCSize) / (Time.realtimeSinceStartup - this.lastTime) * 1024f;
			this.lastTime = Time.realtimeSinceStartup;
			this.lastCSize = num3;
			if (num2 != 0f)
			{
				num = num3 / num2;
			}
			string str = string.Format("下载进度：{0}MB/{1}MB  {2}KB/S  ", num3.ToString("F1"), num2.ToString("F1"), num4.ToString("F2"));
			MobaMessageManagerTools.LoadView2_setProgress2((int)(num * 100f), this.tNum, this.cNum, str, MsgData_LoadView2_setProgress.SetType.targetNum);
		}
	}
}
