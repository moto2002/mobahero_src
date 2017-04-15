using Com.Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaHeros.Spawners
{
	public abstract class BaseSpawner : IGameModule
	{
		public static bool IsDynamicObstacle = true;

		protected SysBattleSceneVo MyScene;

		protected CoroutineManager MyCoroutineManager = new CoroutineManager();

		private bool _isStopSpawn;

		private bool _isEndSpawn;

		private readonly List<BaseSpawnTask> _spawnTasks = new List<BaseSpawnTask>();

		private readonly List<BaseSpawnTask> _doneTasks = new List<BaseSpawnTask>();

		private BaseVictoryChecker _victoryChecker;

		public abstract bool IsReady
		{
			get;
		}

		public TeamType GameResult
		{
			get
			{
				return (this._victoryChecker != null) ? this._victoryChecker.WinnerTeam : TeamType.None;
			}
		}

		public abstract List<EntityVo> GetHeroes(TeamType team);

		protected abstract void InitSpawn();

		protected abstract IEnumerator Preload_Coroutine(MonoBehaviour mono);

		protected abstract void OnRequestSpawn();

		protected abstract void OnSpawnStop();

		protected abstract void OpenViews();

		protected abstract void CloseViews();

		protected abstract void UninitSpawn();

		protected abstract BaseVictoryChecker CreateVictoryChecker();

		protected abstract void SpawnFinished();

		protected void StartSpawnTask(BaseSpawnTask task)
		{
			if (task == null)
			{
				return;
			}
			this._spawnTasks.Add(task);
			task.OnFinished += new Action<BaseSpawnTask, bool>(this.OnTaskFinished);
			task.Start();
		}

		private void OnTaskFinished(BaseSpawnTask task, bool finished)
		{
			task.OnFinished -= new Action<BaseSpawnTask, bool>(this.OnTaskFinished);
			this._spawnTasks.RemoveAll((BaseSpawnTask x) => x == task);
			this._doneTasks.Add(task);
			if (this._spawnTasks.Count == 0)
			{
				this.SpawnFinished();
			}
		}

		private void StopSpawnTasks()
		{
			foreach (BaseSpawnTask current in this._spawnTasks)
			{
				current.Stop();
			}
			foreach (BaseSpawnTask current2 in this._doneTasks)
			{
				current2.Stop();
			}
			this._spawnTasks.Clear();
			this._doneTasks.Clear();
		}

		[DebuggerHidden]
		public IEnumerator StartSpawn(MonoBehaviour mono)
		{
			BaseSpawner.<StartSpawn>c__Iterator1BB <StartSpawn>c__Iterator1BB = new BaseSpawner.<StartSpawn>c__Iterator1BB();
			<StartSpawn>c__Iterator1BB.mono = mono;
			<StartSpawn>c__Iterator1BB.<$>mono = mono;
			<StartSpawn>c__Iterator1BB.<>f__this = this;
			return <StartSpawn>c__Iterator1BB;
		}

		public void StopSpawn()
		{
			if (this._isStopSpawn)
			{
				return;
			}
			if (this._victoryChecker != null)
			{
				this._victoryChecker.StopCheckVictory();
			}
			this.UninitSpawn();
			this.StopSpawnTasks();
			this.OnSpawnStop();
			this.CloseViews();
			this._isStopSpawn = true;
		}

		public void EndSpawn()
		{
			if (this._isEndSpawn)
			{
				return;
			}
			if (!this._isStopSpawn)
			{
				this.StopSpawn();
			}
			this.MyCoroutineManager.StopAllCoroutine();
			this._isEndSpawn = true;
		}

		private void InitReplay()
		{
		}

		public void Init()
		{
		}

		public void Uninit()
		{
			this.EndSpawn();
		}

		public void OnGameStateChange(GameState oldState, GameState newState)
		{
			if (newState == GameState.Game_Playing)
			{
				this._victoryChecker = this.CreateVictoryChecker();
				this._victoryChecker.StartCheckVictory();
			}
			if (newState == GameState.Game_Over)
			{
				this.StopSpawn();
			}
			if (newState == GameState.Game_Exit)
			{
				this.StopSpawn();
			}
		}
	}
}
