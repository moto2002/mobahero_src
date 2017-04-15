using Com.Game.Module;
using GUIFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace HUD.Module
{
	public class PlayersIndicator : BaseModule
	{
		private UIGrid mGrid;

		private GameObject mEnemyCache;

		private VTrigger mFriendListen;

		private CoroutineManager cMgr = new CoroutineManager();

		private ObjectRecycler<PlayersInfoItem> _playerInfoPool;

		private List<string> _deadEnemy = new List<string>();

		public PlayersIndicator()
		{
			this.module = EHUDModule.PlayersIndicator;
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/HUDModule/PlayersIndicatorModule");
		}

		public override void Init()
		{
			base.Init();
			this.mGrid = this.transform.FindChild("ParentRoot/Grid").GetComponent<UIGrid>();
			this.mEnemyCache = this.transform.FindChild("EnemyCache").gameObject;
			this.mFriendListen = TriggerManager.CreateGameEventTrigger(GameEvent.GameStart, null, new TriggerAction(this.ShowFriendsInfo));
			this._playerInfoPool = new ObjectRecycler<PlayersInfoItem>(new Func<PlayersInfoItem>(this.CreatePlayerInfo), delegate(PlayersInfoItem x)
			{
				UnityEngine.Object.Destroy(x.gameObject);
			});
		}

		public override void HandleAfterOpenModule()
		{
			base.HandleAfterOpenModule();
			MobaMessageManager.RegistMessage((ClientMsg)25035, new MobaMessageFunc(this.DoUnitDead));
		}

		public override void HandleBeforeCloseModule()
		{
			base.HandleBeforeCloseModule();
			MobaMessageManager.UnRegistMessage((ClientMsg)25035, new MobaMessageFunc(this.DoUnitDead));
			this._playerInfoPool.DestroyPool();
		}

		public override void Destroy()
		{
			base.Destroy();
			this.cMgr.StopAllCoroutine();
			if (this.mFriendListen != null)
			{
				TriggerManager.DestroyTrigger(this.mFriendListen);
				this.mFriendListen = null;
			}
		}

		private PlayersInfoItem CreatePlayerInfo()
		{
			return NGUITools.AddChild(this.mGrid.gameObject, this.mEnemyCache).GetComponent<PlayersInfoItem>();
		}

		private void DoUnitDead(MobaMessage msg)
		{
			ParamUnitDeathTime paramUnitDeathTime = msg.Param as ParamUnitDeathTime;
			int uniqueId = paramUnitDeathTime.uniqueId;
			float reliveTime = paramUnitDeathTime.reliveTime;
			Units unit = MapManager.Instance.GetUnit(uniqueId);
			if (unit.isEnemy && !this._deadEnemy.Contains(unit.npc_id) && !unit.MirrorState)
			{
				this._deadEnemy.Add(unit.npc_id);
				this.cMgr.StartCoroutine(this.AddDeathTimer(uniqueId, unit.npc_id, reliveTime), true);
			}
		}

		[DebuggerHidden]
		private IEnumerator AddDeathTimer(int _uniqueId, string _npcId, float _time)
		{
			PlayersIndicator.<AddDeathTimer>c__IteratorDC <AddDeathTimer>c__IteratorDC = new PlayersIndicator.<AddDeathTimer>c__IteratorDC();
			<AddDeathTimer>c__IteratorDC._uniqueId = _uniqueId;
			<AddDeathTimer>c__IteratorDC._npcId = _npcId;
			<AddDeathTimer>c__IteratorDC._time = _time;
			<AddDeathTimer>c__IteratorDC.<$>_uniqueId = _uniqueId;
			<AddDeathTimer>c__IteratorDC.<$>_npcId = _npcId;
			<AddDeathTimer>c__IteratorDC.<$>_time = _time;
			<AddDeathTimer>c__IteratorDC.<>f__this = this;
			return <AddDeathTimer>c__IteratorDC;
		}

		private void ShowFriendsInfo()
		{
			Units player = PlayerControlMgr.Instance.GetPlayer();
			int friendHeroesCount = MapManager.Instance.GetFriendHeroesCount(player.teamType);
			if (friendHeroesCount > 1)
			{
				IList<Units> allHeroes = MapManager.Instance.GetAllHeroes();
				int num = 0;
				foreach (Units current in allHeroes)
				{
					if (current.teamType == player.teamType && current != player)
					{
						if (num < 4)
						{
							this.mGrid.transform.Find("0" + num).gameObject.SetActive(true);
						}
						num++;
					}
				}
			}
			this.mGrid.Reposition();
		}
	}
}
