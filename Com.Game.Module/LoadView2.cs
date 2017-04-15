using GUIFramework;
using MobaMessageData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Com.Game.Module
{
	public class LoadView2 : BaseView<LoadView2>
	{
		private class ProgressInfo
		{
			public int nextTarget;

			public string nextStr;

			public ProgressInfo(int nt, string ns)
			{
				this.nextTarget = nt;
				this.nextStr = ns;
			}
		}

		private UILabel notice;

		private UIProgressBar progressBar;

		private int displayPro;

		private int curTargetPro;

		private int lastTargetPro;

		private int totalNum = 1;

		private int subNum = -1;

		private float interval = 0.01f;

		private string noticeStr;

		private Queue<LoadView2.ProgressInfo> qTarget;

		public LoadView2()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Login/LoadView2");
		}

		public override void Init()
		{
			base.Init();
			this.qTarget = new Queue<LoadView2.ProgressInfo>();
			this.noticeStr = string.Empty;
			this.progressBar = this.gameObject.transform.FindChild("LoadBar/ProgressBar").GetComponent<UIProgressBar>();
			this.progressBar.value = 0f;
			this.notice = this.gameObject.transform.FindChild("LoadBar/Notice").GetComponent<UILabel>();
		}

		public override void RegisterUpdateHandler()
		{
			MobaMessageManager.RegistMessage(ClientC2V.LoadView_setText2, new MobaMessageFunc(this.OnSetText));
			MobaMessageManager.RegistMessage(ClientC2V.LoadView_setProgress2, new MobaMessageFunc(this.OnSetProgress));
		}

		public override void HandleAfterOpenView()
		{
			Task task = new Task(this.DoAddProgress(), true);
		}

		public override void CancelUpdateHandler()
		{
			MobaMessageManager.UnRegistMessage(ClientC2V.LoadView_setText2, new MobaMessageFunc(this.OnSetText));
			MobaMessageManager.UnRegistMessage(ClientC2V.LoadView_setProgress2, new MobaMessageFunc(this.OnSetProgress));
		}

		public override void HandleBeforeCloseView()
		{
		}

		public override void RefreshUI()
		{
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void OnSetText(MobaMessage msg)
		{
			this.noticeStr = (msg.Param as string);
			this.RefreshUI_notice();
		}

		private void OnSetProgress(MobaMessage msg)
		{
			MsgData_LoadView2_setProgress msgData_LoadView2_setProgress = msg.Param as MsgData_LoadView2_setProgress;
			if (msgData_LoadView2_setProgress != null)
			{
				this.totalNum = msgData_LoadView2_setProgress.totalNum;
				this.subNum = msgData_LoadView2_setProgress.curNum;
				if (msgData_LoadView2_setProgress.setType == MsgData_LoadView2_setProgress.SetType.addNum)
				{
					this.AddToProgress(msgData_LoadView2_setProgress.subProgress, msgData_LoadView2_setProgress.notice);
				}
				else if (msgData_LoadView2_setProgress.setType == MsgData_LoadView2_setProgress.SetType.targetNum)
				{
					this.SetToProgress(msgData_LoadView2_setProgress.subProgress, msgData_LoadView2_setProgress.notice);
				}
			}
		}

		private void AddToProgress(int delta, string notice)
		{
			this.lastTargetPro += delta;
			this.qTarget.Enqueue(new LoadView2.ProgressInfo(this.lastTargetPro, notice));
		}

		private void SetToProgress(int target, string notice)
		{
			this.lastTargetPro = target;
			this.qTarget.Enqueue(new LoadView2.ProgressInfo(this.lastTargetPro, notice));
		}

		[DebuggerHidden]
		private IEnumerator DoAddProgress()
		{
			LoadView2.<DoAddProgress>c__Iterator160 <DoAddProgress>c__Iterator = new LoadView2.<DoAddProgress>c__Iterator160();
			<DoAddProgress>c__Iterator.<>f__this = this;
			return <DoAddProgress>c__Iterator;
		}

		private void RefreshUI_notice()
		{
			string text = string.Empty;
			if (this.subNum >= 0 && this.totalNum >= 0)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					this.subNum,
					"/",
					this.totalNum
				});
			}
			text = text + " " + this.noticeStr;
			text = text + "(" + ((float)this.displayPro / 100f).ToString("p0") + ")...";
			this.notice.text = text;
		}
	}
}
