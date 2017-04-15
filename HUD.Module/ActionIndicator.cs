using Com.Game.Module;
using Com.Game.Utils;
using GUIFramework;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace HUD.Module
{
	public class ActionIndicator : BaseModule
	{
		private Transform mTopWidget;

		private Transform mBottomWidget;

		private GameObject mMissing;

		private GameObject mRunning;

		private GameObject mAttack;

		private GameObject mDanger;

		private GameObject mFire;

		private GameObject mDefend;

		private GameObject mMapToggle;

		private TweenPosition tPos_top;

		private TweenPosition tPos_bottom;

		private readonly List<BetterCdMask> _signalCds = new List<BetterCdMask>();

		private float _signalCdLeft;

		private Task _updateCdTask;

		private bool resetBottomTransLock;

		public ActionIndicator()
		{
			this.module = EHUDModule.ActionIndicator;
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/HUDModule/ActionIndicatorModule");
		}

		public override void Init()
		{
			base.Init();
			this.mTopWidget = this.transform.FindChild("TopWidget");
			this.mBottomWidget = this.transform.FindChild("BottomWidget");
			this.mMissing = this.mTopWidget.FindChild("Sign_missing").gameObject;
			this.mRunning = this.mTopWidget.FindChild("Sign_running").gameObject;
			this.mAttack = this.mTopWidget.FindChild("Sign_attack").gameObject;
			this.mDanger = this.mTopWidget.FindChild("Sign_danger").gameObject;
			this.mMapToggle = this.mTopWidget.FindChild("MapToggle").gameObject;
			this.mFire = this.mBottomWidget.FindChild("Sign_fire").gameObject;
			this.mDefend = this.mBottomWidget.FindChild("Sign_defend").gameObject;
			this.tPos_top = this.mTopWidget.GetComponent<TweenPosition>();
			this.tPos_bottom = this.mBottomWidget.GetComponent<TweenPosition>();
			this._signalCds.Add(this.mMissing.transform.TryGetComp("CD"));
			this._signalCds.Add(this.mRunning.transform.TryGetComp("CD"));
			this._signalCds.Add(this.mAttack.transform.TryGetComp("CD"));
			this._signalCds.Add(this.mDanger.transform.TryGetComp("CD"));
			this._signalCds.Add(this.mFire.transform.TryGetComp("CD"));
			this._signalCds.Add(this.mDefend.transform.TryGetComp("CD"));
			UIEventListener.Get(this.mMapToggle).onClick = new UIEventListener.VoidDelegate(this.OnClickMapToggle);
			UIEventListener.Get(this.mMissing).onClick = new UIEventListener.VoidDelegate(this.OnClickMissing);
			UIEventListener.Get(this.mRunning).onClick = new UIEventListener.VoidDelegate(this.OnClickRunning);
			UIEventListener.Get(this.mAttack).onClick = new UIEventListener.VoidDelegate(this.OnClickAttack);
			UIEventListener.Get(this.mDanger).onClick = new UIEventListener.VoidDelegate(this.OnClickDanger);
			UIEventListener.Get(this.mDefend).onClick = new UIEventListener.VoidDelegate(this.OnClickDefend);
			UIEventListener.Get(this.mFire).onClick = new UIEventListener.VoidDelegate(this.OnClickFire);
			this.SetPosByLevelID();
		}

		public override void onFlyOut()
		{
			this.tPos_top.PlayForward();
			this.tPos_bottom.PlayForward();
		}

		public override void onFlyIn()
		{
			this.tPos_top.PlayReverse();
			this.tPos_bottom.PlayReverse();
		}

		public override void HandleAfterOpenModule()
		{
			base.HandleAfterOpenModule();
		}

		private void StartCd(MobaMessage msg)
		{
			this._signalCdLeft = 15f;
			this._signalCds[0].newParent.gameObject.SetActive(true);
			for (int i = 0; i < this._signalCds.Count; i++)
			{
				this._signalCds[i].gameObject.SetActive(true);
			}
			Task.Clear(ref this._updateCdTask);
			this._updateCdTask = new Task(this.UpdateCd_Coroutine(), true);
		}

		[DebuggerHidden]
		private IEnumerator UpdateCd_Coroutine()
		{
			ActionIndicator.<UpdateCd_Coroutine>c__IteratorD1 <UpdateCd_Coroutine>c__IteratorD = new ActionIndicator.<UpdateCd_Coroutine>c__IteratorD1();
			<UpdateCd_Coroutine>c__IteratorD.<>f__this = this;
			return <UpdateCd_Coroutine>c__IteratorD;
		}

		public override void Destroy()
		{
			this.mTopWidget = null;
			this.mBottomWidget = null;
			this.mMissing = null;
			this.mRunning = null;
			this.mAttack = null;
			this.mDanger = null;
			this.mFire = null;
			this.mDefend = null;
			this.mMapToggle = null;
			this.tPos_top = null;
			this.tPos_bottom = null;
			base.Destroy();
		}

		public override void RegisterUpdateHandler()
		{
			base.RegisterUpdateHandler();
			MobaMessageManager.RegistMessage((ClientMsg)25041, new MobaMessageFunc(this.OnTeamSignalStateChanged));
			MobaMessageManager.RegistMessage((ClientMsg)25042, new MobaMessageFunc(this.StartCd));
		}

		public override void CancelUpdateHandler()
		{
			base.CancelUpdateHandler();
			MobaMessageManager.UnRegistMessage((ClientMsg)25041, new MobaMessageFunc(this.OnTeamSignalStateChanged));
			MobaMessageManager.UnRegistMessage((ClientMsg)25042, new MobaMessageFunc(this.StartCd));
		}

		private void OnTeamSignalStateChanged(MobaMessage msg)
		{
			if (!(bool)msg.Param)
			{
				this.mMissing.transform.FindChild("mask").gameObject.SetActive(false);
				this.mRunning.transform.FindChild("mask").gameObject.SetActive(false);
				this.mAttack.transform.FindChild("mask").gameObject.SetActive(false);
				this.mDanger.transform.FindChild("mask").gameObject.SetActive(false);
			}
		}

		private void SetPosByLevelID()
		{
			this.resetBottomTransLock = false;
			Vector3 localScale = new Vector3(0.9f, 0.9f, 1f);
			if (Singleton<HUDModuleManager>.Instance.IsVastMap)
			{
				this.mAttack.transform.localPosition = new Vector3(-350f, -38f, 0f);
				this.mAttack.transform.localScale = localScale;
				this.mDanger.transform.localPosition = new Vector3(-350f, -114f, 0f);
				this.mDanger.transform.localScale = localScale;
				this.mRunning.transform.localPosition = new Vector3(-350f, -190f, 0f);
				this.mRunning.transform.localScale = localScale;
				this.mMissing.transform.localPosition = new Vector3(-350f, -266f, 0f);
				this.mMissing.transform.localScale = localScale;
				this.mFire.transform.localPosition = new Vector3(-426f, 270f, 0f);
				this.mDefend.transform.localPosition = new Vector3(-426f, 195f, 0f);
				this.ResetBottomTrans(304f);
				this.resetBottomTransLock = true;
				this.mMapToggle.SetActive(false);
			}
		}

		public void ResetBottomTrans(float mapHeight)
		{
			if (this.resetBottomTransLock)
			{
				return;
			}
			this.mBottomWidget.localPosition = new Vector3(960f, 460f - mapHeight, 0f);
			this.tPos_bottom.from = new Vector3(960f, 460f - mapHeight, 0f);
			this.tPos_bottom.to = new Vector3(1860f, 460f - mapHeight, 0f);
		}

		public void OnClickMissing(GameObject obj = null)
		{
			TeamSignalManager.Begin(TeamSignalType.Miss);
			this.mMissing.transform.FindChild("mask").gameObject.SetActive(true);
		}

		public void OnClickRunning(GameObject obj = null)
		{
			TeamSignalManager.Begin(TeamSignalType.Goto);
			this.mRunning.transform.FindChild("mask").gameObject.SetActive(true);
		}

		public void OnClickAttack(GameObject obj = null)
		{
			TeamSignalManager.Begin(TeamSignalType.Converge);
			this.mAttack.transform.FindChild("mask").gameObject.SetActive(true);
		}

		public void NewbieClickAttack()
		{
			TeamSignalManager.Begin(TeamSignalType.Converge);
			this.mAttack.transform.FindChild("mask").gameObject.SetActive(true);
		}

		public void OnClickDanger(GameObject obj = null)
		{
			TeamSignalManager.Begin(TeamSignalType.Danger);
			this.mDanger.transform.FindChild("mask").gameObject.SetActive(true);
		}

		public void OnClickMapToggle(GameObject obj = null)
		{
			HUDModuleMsgTools.Get_MinimapToggle();
		}

		public void OnClickFire(GameObject obj = null)
		{
			TeamSignalManager.TrySendTeamPosNotify(TeamSignalType.Fire, PlayerControlMgr.Instance.GetPlayer().transform.position);
		}

		public void OnClickDefend(GameObject obj = null)
		{
			TeamSignalManager.TrySendTeamPosNotify(TeamSignalType.Defense, PlayerControlMgr.Instance.GetPlayer().transform.position);
		}
	}
}
