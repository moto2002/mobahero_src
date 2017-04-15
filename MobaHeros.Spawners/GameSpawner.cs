using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaHeros.Pvp;
using Newbie;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaHeros.Spawners
{
	public class GameSpawner : BaseSpawner
	{
		private class ExtraXld
		{
			public int NpcLevelLm = 1;

			public int NpcLevelBl = 1;
		}

		protected EntityVoCreator MyEntityVoCreator;

		private GameSpawner.ExtraXld _battleExtraXld;

		private readonly Dictionary<TeamType, List<EntityVo>> _allHeroes = new Dictionary<TeamType, List<EntityVo>>();

		private VTrigger _gamePlayTrigger;

		private bool _isReady;

		public int CurrentBattleType
		{
			get
			{
				return LevelManager.CurBattleType;
			}
		}

		private SceneInfo CurrentScene
		{
			get;
			set;
		}

		public override bool IsReady
		{
			get
			{
				return this._isReady;
			}
		}

		private void RealFinishLevel()
		{
			BgmPlayer.Instance.StopExBG();
			bool flag = base.GameResult == TeamType.LM;
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				flag = GameManager.IsVictory.Value;
			}
			if (!AudioMgr.Instance.isEffMute())
			{
				if (flag)
				{
					AudioMgr.PlayUI("Play_Victory", null, false, false);
				}
				else
				{
					AudioMgr.PlayUI("Play_Defeat", null, false, false);
				}
			}
			this.MyCoroutineManager.StopAllCoroutine();
			this.MyCoroutineManager.StartCoroutine(this.FinishLevel(), true);
		}

		[DebuggerHidden]
		private IEnumerator FinishLevel()
		{
			GameSpawner.<FinishLevel>c__Iterator1BE <FinishLevel>c__Iterator1BE = new GameSpawner.<FinishLevel>c__Iterator1BE();
			<FinishLevel>c__Iterator1BE.<>f__this = this;
			return <FinishLevel>c__Iterator1BE;
		}

		public override List<EntityVo> GetHeroes(TeamType team)
		{
			List<EntityVo> result;
			this._allHeroes.TryGetValue(team, out result);
			return result;
		}

		protected virtual string GetCorrectModelId(string oldId, EntityType type, TeamType team)
		{
			if (this.CurrentBattleType == 9 && (type == EntityType.Home || type == EntityType.Monster || type == EntityType.Tower))
			{
				return GameSpawner.GetNewModelIDWithLevel(oldId, this._battleExtraXld.NpcLevelBl);
			}
			return oldId;
		}

		[DebuggerHidden]
		protected override IEnumerator Preload_Coroutine(MonoBehaviour mono)
		{
			GameSpawner.<Preload_Coroutine>c__Iterator1BF <Preload_Coroutine>c__Iterator1BF = new GameSpawner.<Preload_Coroutine>c__Iterator1BF();
			<Preload_Coroutine>c__Iterator1BF.mono = mono;
			<Preload_Coroutine>c__Iterator1BF.<$>mono = mono;
			<Preload_Coroutine>c__Iterator1BF.<>f__this = this;
			return <Preload_Coroutine>c__Iterator1BF;
		}

		protected override BaseVictoryChecker CreateVictoryChecker()
		{
			return new DefaultVictoryChecker(this.MyScene);
		}

		protected override void InitSpawn()
		{
			MobaMessageManager.RegistMessage((ClientMsg)25039, new MobaMessageFunc(this.OnBattleEnd));
			this._gamePlayTrigger = TriggerManager.CreateGameEventTrigger(GameEvent.GamePlay, null, new TriggerAction(this.DoSpawnPlay_Trigger));
			string curLevelId = LevelManager.CurLevelId;
			this.MyScene = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(curLevelId);
			this.CurrentScene = new SceneInfo(this.MyScene);
			this.MyEntityVoCreator = new EntityVoCreator(new EntityVoCreator.ResMapper(this.GetCorrectModelId));
			int[] stringToInt = StringUtils.GetStringToInt(this.MyScene.spawn_type, '|');
			Singleton<AttachedPropertyView>.Instance.SceneData = ((!this.CurrentScene.IsOpenAdditionFactor) ? null : this.MyScene);
			this.InitAllHeroes();
			BattleCameraMgr.Instance.InitCamera();
			Singleton<CreepSpawner>.Instance.Initialize();
			AnalyticsToolManager.StartLevel(this.MyScene.scene_id);
		}

		protected override void OnRequestSpawn()
		{
			base.StartSpawnTask(new SpawnHeroTask(this.MyScene, this.GetHeroes(TeamType.LM), this.GetHeroes(TeamType.BL), new BaseSpawnTask.OnSpawnUnit(this.OnHeroUnitSpawned))
			{
				TaskName = "create hereoes"
			});
			base.StartSpawnTask(new SpawnTowerTask(this.MyScene, this.MyEntityVoCreator, null)
			{
				TaskName = "create towers"
			});
			base.StartSpawnTask(new SpawnMonsterTask(this.MyScene, this.MyEntityVoCreator)
			{
				TaskName = "create monsters"
			});
			this.StartSpawnBuffItems();
			string monster_creep = this.MyScene.monster_creep;
			Singleton<CreepSpawner>.Instance.StartSpawn(monster_creep);
		}

		protected override void OnSpawnStop()
		{
			this.StopSpawnBuffItems();
		}

		protected override void UninitSpawn()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)25039, new MobaMessageFunc(this.OnBattleEnd));
			Singleton<CreepSpawner>.Instance.Uninitialize();
			TriggerManager.RemoveAllTriggers();
		}

		protected override void SpawnFinished()
		{
			BattleCameraMgr.Instance.ShowCamera();
			this._isReady = true;
			MobaMessageManager.DispatchMsg(ClientC2C.SpawnFinished, null);
			MobaMessageManager.DispatchMsg(MobaMessageManager.GetMessage((ClientMsg)25011, null, 0f));
		}

		public virtual void OnFinishLevel()
		{
			this.RealFinishLevel();
		}

		protected override void OpenViews()
		{
			bool isObserver = Singleton<PvpManager>.Instance.IsObserver;
			Singleton<CharacterView>.Instance.ShowHpBar(LevelManager.CurBattleType != 10);
			if (!isObserver)
			{
				CtrlManager.OpenWindow(WindowID.CharacterView, null);
			}
			if (LevelManager.CurBattleType == 11)
			{
				CtrlManager.OpenWindow(WindowID.ReplayView, null);
				CtrlManager.OpenWindow(WindowID.GoldView, null);
				Singleton<GoldView>.Instance.SetBattleType(LevelManager.CurBattleType);
				CtrlManager.OpenWindow(WindowID.BuffView, null);
				CtrlManager.OpenWindow(WindowID.MessageView, null);
			}
			else if (LevelManager.CurBattleType == 6)
			{
				CtrlManager.OpenWindow(WindowID.GoldView, null);
				Singleton<GoldView>.Instance.SetBattleType(LevelManager.CurBattleType);
				CtrlManager.OpenWindow(WindowID.MessageView, null);
			}
			else if (LevelManager.CurBattleType != 10)
			{
				if (!isObserver)
				{
					CtrlManager.OpenWindow(WindowID.SkillView, null);
					Singleton<SkillView>.Instance.SetGameObjectActive(false);
				}
			}
			if (LevelManager.CurBattleType == 3)
			{
			}
			if (LevelManager.Instance.IsZyBattleType || LevelManager.Instance.IsPvpBattleType || LevelManager.CurBattleType == 2)
			{
				if (isObserver)
				{
					CtrlManager.OpenWindow(WindowID.ObserveView, null);
				}
				else
				{
					CtrlManager.OpenWindow(WindowID.ShowEquipmentPanelView, null);
				}
			}
			if (Singleton<MiniMapView>.Instance.IsShowMiNiMap(LevelManager.CurLevelId))
			{
				int[] mapVector = Singleton<MiniMapView>.Instance.GetMapVector3(LevelManager.CurLevelId);
				Singleton<MiniMapView>.Instance.SetMap(mapVector[0], mapVector[1], mapVector[2], mapVector[3], mapVector[4], mapVector[5]);
				CtrlManager.OpenWindow(WindowID.MiniMapView, null);
			}
			if (this.CurrentBattleType == 10 || this.CurrentBattleType == 6 || !isObserver)
			{
			}
			if (LevelManager.CurBattleType == 12)
			{
				Singleton<BattleSettlementView>.Instance.IsReplay = GameManager.Instance.ReplayController.IsReplayStart;
				if (!Singleton<BattleSettlementView>.Instance.IsReplay)
				{
					CtrlManager.OpenWindow(WindowID.BarrageEmitterView, null);
					Singleton<BarrageEmitterView>.Instance.sceneType = ((!isObserver) ? BarrageSceneType.BattleIn : BarrageSceneType.WatcherMode);
				}
				CtrlManager.OpenWindow(WindowID.HUDModuleManager, null);
				if (GameManager.Instance.ReplayController.IsReplayStart)
				{
					CtrlManager.OpenWindow(WindowID.ReplayControllerView, null);
				}
			}
		}

		protected override void CloseViews()
		{
			if (LevelManager.CurBattleType == 3)
			{
				CtrlManager.CloseWindow(WindowID.TimeHoleBattleView);
			}
			CtrlManager.CloseWindow(WindowID.HUDModuleManager);
			CtrlManager.CloseWindow(WindowID.MessageView);
			CtrlManager.CloseWindow(WindowID.MiniMapView);
			CtrlManager.CloseWindow(WindowID.CharacterView);
			CtrlManager.CloseWindow(WindowID.GoldView);
			CtrlManager.CloseWindow(WindowID.SkillView);
			CtrlManager.CloseWindow(WindowID.BuffView);
			CtrlManager.CloseWindow(WindowID.AttachedPropertyView);
			CtrlManager.CloseWindow(WindowID.ObserveView);
			CtrlManager.CloseWindow(WindowID.BarrageEmitterView);
			CtrlManager.CloseWindow(WindowID.SurrenderView);
			CtrlManager.CloseWindow(WindowID.StatisticView);
			CtrlManager.CloseWindow(WindowID.BattleEquipmentView);
			NewbieManager.Instance.TryForceHidePartNewbieView();
		}

		private void InitAllHeroes()
		{
			this._allHeroes.Clear();
			int currentBattleType = this.CurrentBattleType;
			if (currentBattleType == 12)
			{
				this._allHeroes[TeamType.BL] = LevelManager.GetHeroes(TeamType.BL);
				this._allHeroes[TeamType.Team_3] = LevelManager.GetHeroes(TeamType.Team_3);
			}
			else if (currentBattleType == 5 || currentBattleType == 9 || currentBattleType == 10)
			{
				this._allHeroes[TeamType.BL] = LevelManager.GetHeroes(TeamType.BL);
			}
			else if (this.CurrentBattleType == 6)
			{
				this._allHeroes[TeamType.BL] = this.GetYzInfo(LevelManager.GetHeroes(TeamType.BL));
			}
			else if (LevelManager.Instance.IsServerZyBattleType)
			{
				this._allHeroes[TeamType.BL] = LevelManager.GetHeroes(TeamType.BL);
			}
			else
			{
				this._allHeroes[TeamType.BL] = this.MyEntityVoCreator.GetEntityVos(this.MyScene.hero_2, EntityType.Hero, TeamType.BL);
			}
			this._allHeroes[TeamType.LM] = LevelManager.GetHeroes(TeamType.LM);
			this._battleExtraXld = null;
		}

		private List<EntityVo> GetYzInfo(List<EntityVo> originInfo)
		{
			List<EntityVo> list = new List<EntityVo>();
			foreach (EntityVo current in originInfo)
			{
				if (current.hp != 0f)
				{
					list.Add(current);
				}
			}
			return list;
		}

		protected void StartSpawnBuffItems()
		{
			GameManager.Instance.BattleRefresh.StartBattleRefresh();
		}

		protected void StopSpawnBuffItems()
		{
			GameManager.Instance.BattleRefresh.StopBattleRefresh();
		}

		private void OnBattleEnd(MobaMessage msg)
		{
			this.OnFinishLevel();
		}

		private void DoSpawnPlay_Trigger()
		{
			if (this.MyScene.hero2_nb_time > 0f)
			{
			}
		}

		protected void OnHeroUnitSpawned(Units unit, EntityVo entityVo)
		{
			if (!unit)
			{
				return;
			}
			if (!GameManager.IsPlaying())
			{
				UnitVisibilityManager.BecomeInvisible(unit);
			}
			unit.SetCanMove(false);
			unit.SetCanAIControl(false);
			StrategyManager.Instance.SetControlState(StrategyManager.ControlState.ManualState, -1f);
			if (NewbieManager.Instance.IsDoNewbieSpawnProcess())
			{
				UnitVisibilityManager.NewbieBecomeInvisible(unit);
			}
		}

		public static string GetNewModelIDWithLevel(string oldId, int level)
		{
			string str = oldId.Substring(0, oldId.Length - 4);
			string s = oldId.Substring(oldId.Length - 4, 4);
			return str + (int.Parse(s) + level - 1).ToString().PadLeft(4, '0');
		}

		public List<string> GetHeroNames(TeamType team)
		{
			List<EntityVo> heroes = this.GetHeroes(team);
			if (heroes != null)
			{
				List<string> list = new List<string>();
				for (int i = 0; i < heroes.Count; i++)
				{
					list.Add(heroes[i].npc_id);
				}
				return list;
			}
			return null;
		}

		public int GetPlayerNum(TeamType team)
		{
			int num = 0;
			List<string> heroNames = this.GetHeroNames(team);
			for (int i = 0; i < heroNames.Count; i++)
			{
				if (heroNames[i] != string.Empty)
				{
					num++;
				}
			}
			return num;
		}

		public float GetAttrFactor(TeamType teamType)
		{
			return SpawnUtility.GetAttrFactor(teamType, this.MyScene);
		}

		public SpawnUtility GetSpawnUtility()
		{
			return new SpawnUtility(this.MyScene);
		}
	}
}
