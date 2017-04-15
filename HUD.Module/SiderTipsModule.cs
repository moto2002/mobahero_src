using Com.Game.Module;
using GUIFramework;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace HUD.Module
{
	public class SiderTipsModule : BaseModule
	{
		private GameObject triggerBtn;

		private Transform cacheContainer;

		private CoroutineManager cMgr = new CoroutineManager();

		private Queue<SiderTipCtrl> msgShowQueue = new Queue<SiderTipCtrl>();

		private Queue<SiderTipCtrl> msgFreeQueue = new Queue<SiderTipCtrl>();

		private Queue<SiderTipMsg> msgWaitQueue = new Queue<SiderTipMsg>();

		private SiderTipCtrl speedUpTip;

		private int counter;

		private TweenPosition tPos;

		private Dictionary<string, string> headIconDict = new Dictionary<string, string>();

		public float cellHeight = 130f;

		private SiderTipCtrl siderPrefab;

		public bool isRightSider = true;

		public SiderTipsModule()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/HUDModule/SiderTipsModule");
		}

		public override void Init()
		{
			this.counter = 0;
			this.speedUpTip = null;
			this.triggerBtn = this.transform.Find("Trigger").gameObject;
			this.cacheContainer = this.transform.Find("CacheContainer");
			this.msgShowQueue.Clear();
			this.msgFreeQueue.Clear();
			this.msgWaitQueue.Clear();
			this.tPos = this.cacheContainer.GetComponent<TweenPosition>();
			UIEventListener.Get(this.triggerBtn).onClick = new UIEventListener.VoidDelegate(this.onTriggerMsg);
			if (Singleton<MiniMapView>.Instance.gameObject)
			{
				if (Singleton<HUDModuleManager>.Instance.IsVastMap)
				{
					this.ResetHeight(145f);
				}
				else
				{
					this.ResetHeight(30f + Singleton<MiniMapView>.Instance.GetMapHeight());
				}
			}
		}

		public override void HandleAfterOpenModule()
		{
			if (this.isRightSider)
			{
				this.siderPrefab = ResourceManager.LoadPath<SiderTipCtrl>("Prefab/HUDModule/SiderTip", null, 0);
			}
			else
			{
				this.siderPrefab = ResourceManager.LoadPath<SiderTipCtrl>("Prefab/HUDModule/SiderTip_left", null, 0);
			}
			for (int i = 0; i < 4; i++)
			{
				SiderTipCtrl siderTipCtrl = (SiderTipCtrl)UnityEngine.Object.Instantiate(this.siderPrefab);
				siderTipCtrl.transform.parent = this.cacheContainer;
				siderTipCtrl.transform.localPosition = Vector3.zero;
				siderTipCtrl.transform.localScale = Vector3.one;
				this.msgFreeQueue.Enqueue(siderTipCtrl);
				siderTipCtrl.gameObject.SetActive(false);
			}
		}

		public override void CancelUpdateHandler()
		{
			this.cMgr.StopAllCoroutine();
			this.msgShowQueue.Clear();
			this.msgFreeQueue.Clear();
			this.msgWaitQueue.Clear();
			this.speedUpTip = null;
			this.tPos = null;
		}

		public override void onFlyIn()
		{
		}

		public override void onFlyOut()
		{
		}

		public override void AdaptSkillPanelPivot()
		{
			this.isRightSider = (Singleton<HUDModuleManager>.Instance.skillPanelPivot == SkillPanelPivot.Left || Singleton<HUDModuleManager>.Instance.skillPanelPivot == SkillPanelPivot.Bottom);
			TweenPosition component = this.cacheContainer.GetComponent<TweenPosition>();
			component.from = new Vector3((float)((!this.isRightSider) ? -960 : 960), 0f, 0f);
			component.to = component.from;
			this.cacheContainer.localPosition = component.from;
		}

		public void ResetHeight(float mapHeight)
		{
			if (Singleton<HUDModuleManager>.Instance.IsVastMap)
			{
				this.transform.localPosition = new Vector3(0f, 150f, 0f);
				return;
			}
			this.transform.localPosition = new Vector3(0f, 310f - mapHeight, 0f);
		}

		private void onTriggerMsg(GameObject obj = null)
		{
			this.AddNewMsg(new SiderTipMsg("Liaonida_Avatar", TeamSignalType.Defense));
		}

		public void AddSiderTip_Kill(string npcId1, string npcId2, bool killEnmeyOrDeath, List<string> assistantList, TeamType attackTeam, TeamType victimTeam)
		{
			string spriteNameById = Singleton<HUDModuleManager>.Instance.GetSpriteNameById(npcId1);
			string spriteNameById2 = Singleton<HUDModuleManager>.Instance.GetSpriteNameById(npcId2);
			List<string> list = new List<string>();
			for (int i = 0; i < assistantList.Count; i++)
			{
				list.Add(Singleton<HUDModuleManager>.Instance.GetSpriteNameById(assistantList[i]));
			}
			this.AddNewMsg(new SiderTipMsg(spriteNameById, spriteNameById2, list, killEnmeyOrDeath, attackTeam, victimTeam));
		}

		public void AddSiderTip_Signal(string npcId1, TeamSignalType type)
		{
			string spriteNameById = Singleton<HUDModuleManager>.Instance.GetSpriteNameById(npcId1);
			this.AddNewMsg(new SiderTipMsg(spriteNameById, type));
		}

		private void AddNewMsg(SiderTipMsg _msg)
		{
			if (this.msgShowQueue.Count >= 3)
			{
				this.msgWaitQueue.Enqueue(_msg);
				return;
			}
			SiderTipCtrl siderTipCtrl = this.msgFreeQueue.Dequeue();
			siderTipCtrl.defaultPos = new Vector3(0f, (float)this.counter * -this.cellHeight, 0f);
			this.counter++;
			this.cMgr.StartCoroutine(this.ShowMsg(_msg, siderTipCtrl), true);
			this.msgShowQueue.Enqueue(siderTipCtrl);
		}

		[DebuggerHidden]
		private IEnumerator ShowMsg(SiderTipMsg _msg, SiderTipCtrl _tweenComp)
		{
			SiderTipsModule.<ShowMsg>c__IteratorDE <ShowMsg>c__IteratorDE = new SiderTipsModule.<ShowMsg>c__IteratorDE();
			<ShowMsg>c__IteratorDE._tweenComp = _tweenComp;
			<ShowMsg>c__IteratorDE._msg = _msg;
			<ShowMsg>c__IteratorDE.<$>_tweenComp = _tweenComp;
			<ShowMsg>c__IteratorDE.<$>_msg = _msg;
			<ShowMsg>c__IteratorDE.<>f__this = this;
			return <ShowMsg>c__IteratorDE;
		}

		private void FinishAMsg()
		{
			if (!this.tPos.enabled)
			{
				this.tPos.from = this.tPos.to;
				this.tPos.to = this.tPos.from + new Vector3(0f, this.cellHeight, 0f);
				this.tPos.ResetToBeginning();
				this.tPos.PlayForward();
			}
			else
			{
				this.tPos.SetStartToCurrentValue();
				this.tPos.enabled = false;
				this.tPos.to = this.tPos.to + new Vector3(0f, this.cellHeight, 0f);
				this.tPos.ResetToBeginning();
				this.tPos.PlayForward();
			}
			if (this.msgWaitQueue.Count > 0)
			{
				this.AddNewMsg(this.msgWaitQueue.Dequeue());
			}
		}

		private void RemoveNow()
		{
			this.speedUpTip = this.msgShowQueue.Peek();
			if (this.speedUpTip.state == BattleMsgState.EaseOut)
			{
				this.speedUpTip = null;
			}
		}
	}
}
