using GUIFramework;
using MobaMessageData;
using System;
using System.Collections;
using System.Diagnostics;

namespace Com.Game.Module
{
	public class LoadView : BaseView<LoadView>
	{
		private int m_Style = 1;

		private LoadController m_LoadController;

		private UICenterHelper cenHelper;

		private int displayProgress;

		private int toProgress;

		private float interval = 0.01f;

		public LoadView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/LoadView");
		}

		public override void Init()
		{
			base.Init();
			this.m_LoadController = this.gameObject.GetComponent<LoadController>();
			if (this.m_LoadController == null)
			{
				this.m_LoadController = this.gameObject.AddComponent<LoadController>();
			}
			this.cenHelper = this.transform.Find("Anchor/Container").GetComponent<UICenterHelper>();
			this.InitValue();
		}

		public override void RegisterUpdateHandler()
		{
			MobaMessageManager.RegistMessage((ClientMsg)23008, new MobaMessageFunc(this.OnSetProgress));
		}

		public override void HandleAfterOpenView()
		{
			this.uiWindow.StartCoroutine(this.DoAddProgress());
		}

		public override void CancelUpdateHandler()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)23008, new MobaMessageFunc(this.OnSetProgress));
		}

		public override void HandleBeforeCloseView()
		{
		}

		public override void RefreshUI()
		{
			this.UpdateLoadView();
		}

		public override void Destroy()
		{
			this.m_LoadController = null;
			base.Destroy();
		}

		private void InitValue()
		{
			this.SetLoadingPercentage(0);
			this.displayProgress = 0;
			this.toProgress = 0;
		}

		private void UpdateLoadView()
		{
			if (this.m_LoadController != null)
			{
				this.m_LoadController.SetLoadingBackground(this.m_Style);
			}
		}

		private void OnSetProgress(MobaMessage msg)
		{
			MsgData_LoadView_setProgress msgData_LoadView_setProgress = msg.Param as MsgData_LoadView_setProgress;
			if (msgData_LoadView_setProgress != null)
			{
				if (msgData_LoadView_setProgress.AddType == MsgData_LoadView_setProgress.SetType.addNum)
				{
					this.AddToProgress(msgData_LoadView_setProgress.Num);
				}
				else if (msgData_LoadView_setProgress.AddType == MsgData_LoadView_setProgress.SetType.targetNum)
				{
					this.SetToProgress(msgData_LoadView_setProgress.Num);
				}
			}
		}

		private void AddToProgress(int delta)
		{
			this.toProgress += delta;
		}

		private void SetToProgress(int target)
		{
			this.toProgress = target;
		}

		[DebuggerHidden]
		private IEnumerator DoAddProgress()
		{
			LoadView.<DoAddProgress>c__Iterator123 <DoAddProgress>c__Iterator = new LoadView.<DoAddProgress>c__Iterator123();
			<DoAddProgress>c__Iterator.<>f__this = this;
			return <DoAddProgress>c__Iterator;
		}

		private void SetLoadingPercentage(int progress)
		{
			if (this.m_LoadController != null)
			{
				this.m_LoadController.SetLoadingPercentage(progress);
				this.cenHelper.Reposition();
			}
			if (progress % 10 == 0)
			{
				MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)21007, progress, 0f);
				MobaMessageManager.ExecuteMsg(message);
			}
		}
	}
}
