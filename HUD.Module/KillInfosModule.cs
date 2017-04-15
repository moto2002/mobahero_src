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
	public class KillInfosModule : BaseModule
	{
		private class RightMsg
		{
		}

		private class RightKillMsg : KillInfosModule.RightMsg
		{
			public string npc1Id;

			public string npc2Id;

			public float duration;

			public bool killOrDeath;

			public List<string> assistantList;

			public RightKillMsg(string npc1Id, string npc2Id, float duration, bool killOrDeath, List<string> assistantList)
			{
				this.npc1Id = npc1Id;
				this.npc2Id = npc2Id;
				this.duration = duration;
				this.killOrDeath = killOrDeath;
				this.assistantList = assistantList;
			}
		}

		private class RightSignalMsg : KillInfosModule.RightMsg
		{
			public string npcId;

			public float duration;

			public TeamSignalType signal;
		}

		private GameObject mItemCache;

		private Transform mContainer;

		private List<KillInfosModule.RightMsg> rightMsgWaitingList = new List<KillInfosModule.RightMsg>();

		private List<GameObject> rightMsgObjList = new List<GameObject>();

		private CoroutineManager cMgr = new CoroutineManager();

		private static Color32 RedBg = new Color32(129, 0, 0, 255);

		private static Color32 GreenBg = new Color32(0, 127, 0, 255);

		private static Color32 BlueBg = new Color32(0, 41, 103, 255);

		private static Color32 RedRing = new Color32(219, 0, 27, 255);

		private static Color32 GreenRing = new Color32(24, 154, 25, 255);

		public KillInfosModule()
		{
			this.module = EHUDModule.KillInfos;
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/HUDModule/KillInfosModule");
		}

		public override void Init()
		{
			base.Init();
			this.mItemCache = ResourceManager.LoadPath<GameObject>("Prefab/HUDModule/KillInfosItem", null, 0);
			this.mContainer = this.transform.FindChild("Container");
		}

		public override void HandleAfterOpenModule()
		{
			base.HandleAfterOpenModule();
			this.cMgr.StopAllCoroutine();
			if (Singleton<MiniMapView>.Instance.transform != null)
			{
				UITexture component = Singleton<MiniMapView>.Instance.transform.FindChild("Anchor/Panel/Map").GetComponent<UITexture>();
				this.ResetHeight((float)component.height);
			}
		}

		public override void HandleBeforeCloseModule()
		{
			base.HandleBeforeCloseModule();
			this.cMgr.StopAllCoroutine();
		}

		public override void RegisterUpdateHandler()
		{
			base.RegisterUpdateHandler();
		}

		public override void onFlyOut()
		{
		}

		public override void onFlyIn()
		{
		}

		public void ResetHeight(float mapHeight)
		{
			this.mContainer.localPosition = new Vector3(960f, 310f - mapHeight, 0f);
		}

		public void ShowRightKillView_Internal(float duration, string npcId1, string npcId2, bool killEnmeyOrDeath, List<string> assistantList)
		{
			if (this.rightMsgObjList.Count <= 4)
			{
				this.cMgr.StartCoroutine(this.RightKillView_Coroutinue(npcId1, npcId2, duration, killEnmeyOrDeath, assistantList), true);
			}
			else
			{
				this.rightMsgWaitingList.Add(new KillInfosModule.RightKillMsg(npcId1, npcId2, duration, killEnmeyOrDeath, assistantList));
			}
		}

		public void ShowRightSignalView_Internal(float duration, string npcId1, TeamSignalType signal)
		{
			if (this.rightMsgObjList.Count <= 4)
			{
				this.cMgr.StartCoroutine(this.RightSignalView_Coroutinue(npcId1, signal, duration), true);
			}
			else
			{
				this.rightMsgWaitingList.Add(new KillInfosModule.RightSignalMsg
				{
					duration = duration,
					signal = signal,
					npcId = npcId1
				});
			}
		}

		[DebuggerHidden]
		private IEnumerator RightKillView_Coroutinue(string killerId, string deadId, float duration, bool killOrDeath, List<string> assistantList)
		{
			KillInfosModule.<RightKillView_Coroutinue>c__IteratorDA <RightKillView_Coroutinue>c__IteratorDA = new KillInfosModule.<RightKillView_Coroutinue>c__IteratorDA();
			<RightKillView_Coroutinue>c__IteratorDA.killerId = killerId;
			<RightKillView_Coroutinue>c__IteratorDA.deadId = deadId;
			<RightKillView_Coroutinue>c__IteratorDA.duration = duration;
			<RightKillView_Coroutinue>c__IteratorDA.killOrDeath = killOrDeath;
			<RightKillView_Coroutinue>c__IteratorDA.assistantList = assistantList;
			<RightKillView_Coroutinue>c__IteratorDA.<$>killerId = killerId;
			<RightKillView_Coroutinue>c__IteratorDA.<$>deadId = deadId;
			<RightKillView_Coroutinue>c__IteratorDA.<$>duration = duration;
			<RightKillView_Coroutinue>c__IteratorDA.<$>killOrDeath = killOrDeath;
			<RightKillView_Coroutinue>c__IteratorDA.<$>assistantList = assistantList;
			<RightKillView_Coroutinue>c__IteratorDA.<>f__this = this;
			return <RightKillView_Coroutinue>c__IteratorDA;
		}

		[DebuggerHidden]
		private IEnumerator RightSignalView_Coroutinue(string npcId1, TeamSignalType signal, float duration)
		{
			KillInfosModule.<RightSignalView_Coroutinue>c__IteratorDB <RightSignalView_Coroutinue>c__IteratorDB = new KillInfosModule.<RightSignalView_Coroutinue>c__IteratorDB();
			<RightSignalView_Coroutinue>c__IteratorDB.npcId1 = npcId1;
			<RightSignalView_Coroutinue>c__IteratorDB.signal = signal;
			<RightSignalView_Coroutinue>c__IteratorDB.duration = duration;
			<RightSignalView_Coroutinue>c__IteratorDB.<$>npcId1 = npcId1;
			<RightSignalView_Coroutinue>c__IteratorDB.<$>signal = signal;
			<RightSignalView_Coroutinue>c__IteratorDB.<$>duration = duration;
			<RightSignalView_Coroutinue>c__IteratorDB.<>f__this = this;
			return <RightSignalView_Coroutinue>c__IteratorDB;
		}
	}
}
