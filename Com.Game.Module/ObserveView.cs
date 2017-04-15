using Com.Game.Utils;
using GUIFramework;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class ObserveView : BaseView<ObserveView>
	{
		private VTrigger _changePlayerTrigger;

		private ObserveViewComponents _components;

		private readonly CoroutineManager _coroutineManager = new CoroutineManager();

		private Task _drag;

		public ObserveView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Battle/ObserveView");
		}

		public override void Init()
		{
			base.Init();
			this._components = this.uiWindow.GetComponent<ObserveViewComponents>();
			UIEventListener.Get(this._components.Background).onDragStart = new UIEventListener.VoidDelegate(this.OnDragStart);
			UIEventListener.Get(this._components.Background).onDragEnd = new UIEventListener.VoidDelegate(this.OnDragEnd);
			UIEventListener.Get(this._components.Background).onDrag = new UIEventListener.VectorDelegate(this.OnDrag);
			UIEventListener.Get(this._components.BtnSettings.gameObject).onClick = delegate
			{
				CtrlManager.OpenWindow(WindowID.ReturnView, null);
			};
		}

		private void OnDrag(GameObject go, Vector2 delta)
		{
			Vector3? scenePos = BattleCameraMgr.Instance.GetScenePos();
			if (scenePos.HasValue)
			{
				PvpObserveMgr.FreeObservePos = scenePos.Value;
			}
			PvpObserveMgr.ObserveFree();
		}

		private void OnDragEnd(GameObject go)
		{
		}

		private void OnDragStart(GameObject go)
		{
			if (this._drag != null)
			{
				this._drag.Stop();
			}
			this._drag = null;
			this._coroutineManager.StartCoroutine(this.ObserveFree_Coroutine(), true);
		}

		[DebuggerHidden]
		private IEnumerator ObserveFree_Coroutine()
		{
			return new ObserveView.<ObserveFree_Coroutine>c__Iterator169();
		}

		public override void RegisterUpdateHandler()
		{
			this.RegisterTrigger();
			this._coroutineManager.StopAllCoroutine();
			this._coroutineManager.StartCoroutine(this.UpdateTeamInfo_Coroutine(), true);
		}

		public override void CancelUpdateHandler()
		{
			this.UnRegisterTrigger();
			this._coroutineManager.StopAllCoroutine();
		}

		public override void RefreshUI()
		{
			this.UpdateHeads();
		}

		public override void Destroy()
		{
			this.UnRegisterTrigger();
			base.Destroy();
			this._coroutineManager.StopAllCoroutine();
		}

		private void RegisterTrigger()
		{
			this._changePlayerTrigger = TriggerManager.CreateGameEventTrigger(GameEvent.ChangePlayer, null, new TriggerAction(this.OnChangePlayer));
			MobaMessageManager.RegistMessage((ClientMsg)25030, new MobaMessageFunc(this.OnPvpStartGame));
		}

		private void UnRegisterTrigger()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)25030, new MobaMessageFunc(this.OnPvpStartGame));
			TriggerManager.DestroyTrigger(this._changePlayerTrigger);
			this._changePlayerTrigger = null;
		}

		private void OnPvpStartGame(MobaMessage msg)
		{
			this.RefreshUI();
		}

		private void OnChangePlayer()
		{
			this.RefreshUI();
		}

		public void UpdateHeads()
		{
			List<EntityVo> lmHeros = GameManager.Instance.Spawner.GetHeroes(TeamType.LM);
			List<EntityVo> blHeros = GameManager.Instance.Spawner.GetHeroes(TeamType.BL);
			GridHelper.FillGrid<ObserveHeadItem>(this._components.LmHeroes, this._components.LmHeroSample, lmHeros.Count, delegate(int idx, ObserveHeadItem comp)
			{
				comp.UniqueId = lmHeros[idx].uid;
				comp.Refresh();
			});
			this._components.LmHeroes.Reposition();
			GridHelper.FillGrid<ObserveHeadItem>(this._components.BlHeroes, this._components.BlHeroSample, blHeros.Count, delegate(int idx, ObserveHeadItem comp)
			{
				comp.UniqueId = blHeros[idx].uid;
				comp.Refresh();
			});
			this._components.BlHeroes.Reposition();
		}

		private void RefreshSummaryInfo()
		{
			try
			{
				PvpStatisticMgr.GroupData groupData = Singleton<PvpManager>.Instance.StatisticMgr.GetGroupData(0);
				if (groupData != null)
				{
					this._components.LmTowerCount.text = groupData.TeamTowerDestroy.ToString();
					this._components.LmKillCount.text = groupData.TeamKill.ToString();
					this._components.LmGold.text = Singleton<PvpManager>.Instance.StatisticMgr.GetTotalMeney(TeamType.LM).ToString();
				}
				PvpStatisticMgr.GroupData groupData2 = Singleton<PvpManager>.Instance.StatisticMgr.GetGroupData(1);
				if (groupData2 != null)
				{
					this._components.BlTowerCount.text = groupData2.TeamTowerDestroy.ToString();
					this._components.BlKillCount.text = groupData2.TeamKill.ToString();
					this._components.BlGold.text = Singleton<PvpManager>.Instance.StatisticMgr.GetTotalMeney(TeamType.BL).ToString();
				}
				this._components.ObserverCount.text = Singleton<PvpManager>.Instance.ObserverCount.ToString();
				DateTime? gameStartTime = Singleton<PvpManager>.Instance.GameStartTime;
				TimeSpan timeSpan = default(TimeSpan);
				if (gameStartTime.HasValue)
				{
					timeSpan = DateTime.Now - gameStartTime.Value;
				}
				this._components.Time.text = StringUtils.FormatTimeInMinutes((int)timeSpan.TotalSeconds, true);
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
			}
		}

		[DebuggerHidden]
		private IEnumerator UpdateTeamInfo_Coroutine()
		{
			ObserveView.<UpdateTeamInfo_Coroutine>c__Iterator16A <UpdateTeamInfo_Coroutine>c__Iterator16A = new ObserveView.<UpdateTeamInfo_Coroutine>c__Iterator16A();
			<UpdateTeamInfo_Coroutine>c__Iterator16A.<>f__this = this;
			return <UpdateTeamInfo_Coroutine>c__Iterator16A;
		}

		public static void DoRefresh()
		{
			if (Singleton<ObserveView>.Instance != null && Singleton<ObserveView>.Instance._components)
			{
				Singleton<ObserveView>.Instance._components.Dummy.SetActive(false);
				Singleton<ObserveView>.Instance._components.Dummy.SetActive(true);
			}
		}
	}
}
