using Com.Game.Utils;
using GUIFramework;
using MobaHeros;
using MobaHeros.Pvp;
using Newbie;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class PvpWaitView : BaseView<PvpWaitView>
	{
		private UILabel _timeLabel;

		private UILabel _timeOnButton;

		private Transform _btnPlay;

		private Transform _btnPause;

		private Transform _newbieFakeMatchFiveMatch;

		private Task _task;

		public PvpWaitView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Pvp/PvpWaitView");
		}

		public override void Init()
		{
			base.Init();
			this.gameObject.layer = LayerMask.NameToLayer("UIEffect");
			this._timeLabel = this.transform.TryGetComp("Anchor/Panel/Content");
			this._timeOnButton = this.transform.TryGetComp("Anchor/Panel/Btns/Btn_Cancel/Time");
			this._btnPause = this.transform.TryFindChild("Anchor/Panel/Btns/Btn_Cancel");
			this._btnPlay = this.transform.TryFindChild("Anchor/Panel/Btns/Btn_OK");
			this._newbieFakeMatchFiveMatch = this.transform.Find("Anchor/Panel/NewbieFakeMatchFiveMatch");
			UIEventListener.Get(this._btnPlay.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickPlay);
			UIEventListener.Get(this._btnPause.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickPause);
			UIEventListener.Get(this._newbieFakeMatchFiveMatch.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickPlay);
		}

		private void OnClickPlay(GameObject go)
		{
			CtrlManager.CloseWindow(WindowID.PvpWaitView);
			PvpMatchMgr.Instance.ForceSelectHero();
		}

		private void OnClickPause(GameObject go)
		{
			CtrlManager.CloseWindow(WindowID.PvpWaitView);
			Singleton<MenuTopBarView>.Instance.TryShowTimeTips();
		}

		public override void HandleAfterOpenView()
		{
			base.HandleAfterOpenView();
			AudioMgr.PlayUI("Play_UI_Match", null, false, false);
			this._task = new Task(this.UpdateTime_Coroutine(), true);
			this._newbieFakeMatchFiveMatch.gameObject.SetActive(false);
			NewbieManager.Instance.TryHandleMatchedSuc();
			AutoTestController.InvokeTestLogic(AutoTestTag.EnterPvp, delegate
			{
				this.OnClickPlay(null);
			}, 1f);
			Singleton<MenuTopBarView>.Instance.SetActiveOrNot(true);
		}

		public override void HandleBeforeCloseView()
		{
			base.HandleBeforeCloseView();
			Task.Clear(ref this._task);
		}

		[DebuggerHidden]
		private IEnumerator UpdateTime_Coroutine()
		{
			PvpWaitView.<UpdateTime_Coroutine>c__Iterator171 <UpdateTime_Coroutine>c__Iterator = new PvpWaitView.<UpdateTime_Coroutine>c__Iterator171();
			<UpdateTime_Coroutine>c__Iterator.<>f__this = this;
			return <UpdateTime_Coroutine>c__Iterator;
		}

		private void SetTimeLeft(int time)
		{
			if (!base.IsOpen)
			{
				return;
			}
			this._timeLabel.text = time.ToString();
			this._timeOnButton.text = LanguageManager.Instance.FormatString("PlayUI_Button_TemporarySuspension", new object[]
			{
				time
			});
		}

		public void NewbieFakeMatchFiveShowMatch()
		{
			this._newbieFakeMatchFiveMatch.gameObject.SetActive(true);
		}
	}
}
