using GUIFramework;
using MobaMessageData;
using Newbie;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class NewbieLoadView : BaseView<NewbieLoadView>
	{
		private LoadController m_LoadController;

		private UILabel txtLabel;

		private UISprite _bgPic;

		private UISprite _loadBlueBg;

		private int displayProgress;

		private int toProgress;

		private float interval = 1f;

		private System.Random _randInst = new System.Random();

		public NewbieLoadView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/NewbieLoadView");
		}

		public override void Init()
		{
			base.Init();
			this.m_LoadController = this.gameObject.GetComponent<LoadController>();
			if (this.m_LoadController == null)
			{
				this.m_LoadController = this.gameObject.AddComponent<LoadController>();
			}
			this.txtLabel = this.transform.Find("Anchor/Text").GetComponent<UILabel>();
			this._bgPic = this.transform.Find("Anchor/Background").GetComponent<UISprite>();
			this._loadBlueBg = this.transform.Find("Anchor/Bluebg").GetComponent<UISprite>();
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
			this.ClearResources();
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
			if (this.toProgress >= 99)
			{
				this.toProgress = 100;
				this.displayProgress = 100;
				this.SetLoadingPercentage(this.displayProgress);
			}
		}

		private void SetToProgress(int target)
		{
			this.toProgress = target;
			if (this.toProgress >= 99)
			{
				this.toProgress = 100;
				this.displayProgress = 100;
				this.SetLoadingPercentage(this.displayProgress);
			}
		}

		[DebuggerHidden]
		private IEnumerator DoAddProgress()
		{
			NewbieLoadView.<DoAddProgress>c__Iterator126 <DoAddProgress>c__Iterator = new NewbieLoadView.<DoAddProgress>c__Iterator126();
			<DoAddProgress>c__Iterator.<>f__this = this;
			return <DoAddProgress>c__Iterator;
		}

		private void SetLoadingPercentage(int progress)
		{
			if (this.m_LoadController != null)
			{
				this.m_LoadController.SetLoadingPercentage(progress);
			}
		}

		private void ClearResources()
		{
			if (this._bgPic != null)
			{
				HomeGCManager.Instance.ClearUIAtlasResourceImmediately(this._bgPic.atlas);
			}
			if (this._loadBlueBg != null)
			{
				HomeGCManager.Instance.ClearUIAtlasResourceImmediately(this._loadBlueBg.atlas);
			}
			this.uiWindow.StopAllCoroutines();
		}

		public void SetBgByNewbiePhase(ENewbiePhaseType inPhaseType)
		{
			if (inPhaseType == ENewbiePhaseType.ElementaryBattleOneOne)
			{
				GameObject gameObject = Resources.Load<GameObject>("Prefab/UI/NewbieLoadEleBatOneAtlas");
				UIAtlas uIAtlas = (!(gameObject != null)) ? null : gameObject.GetComponent<UIAtlas>();
				if (uIAtlas != null)
				{
					this._bgPic.atlas = uIAtlas;
					this._bgPic.spriteName = "Guide_loading_bg";
				}
			}
			else if (inPhaseType == ENewbiePhaseType.ElementaryBattleFiveFive)
			{
				GameObject gameObject2 = Resources.Load<GameObject>("Prefab/UI/NewbieLoadEleBatFiveAtlas");
				UIAtlas uIAtlas2 = (!(gameObject2 != null)) ? null : gameObject2.GetComponent<UIAtlas>();
				if (uIAtlas2 != null)
				{
					this._bgPic.atlas = uIAtlas2;
					this._bgPic.spriteName = "Guide_loading_bg";
				}
			}
		}
	}
}
