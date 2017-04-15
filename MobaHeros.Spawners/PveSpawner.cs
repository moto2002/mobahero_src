using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections.Generic;

namespace MobaHeros.Spawners
{
	public class PveSpawner : GameSpawner
	{
		private bool _isReady;

		public override bool IsReady
		{
			get
			{
				return this._isReady;
			}
		}

		private void OnStartGame(MobaMessage msg)
		{
			MobaMessageManager.DispatchMsg(MobaMessageManager.GetMessage((ClientMsg)25013, null, 0f));
			this._isReady = true;
		}

		protected override void SpawnFinished()
		{
			base.SpawnFinished();
			Singleton<PveManager>.Instance.LoadPvpSceneEnd();
		}

		protected override void OnRequestSpawn()
		{
			base.StartSpawnTask(new SpawnServerPveHeroTask(this.MyScene, this.GetHeroes(TeamType.LM), this.GetHeroes(TeamType.BL), new BaseSpawnTask.OnSpawnUnit(base.OnHeroUnitSpawned))
			{
				TaskName = "create pve heroes"
			});
			base.StartSpawnTask(new SpawnTowerTask(this.MyScene, this.MyEntityVoCreator, null)
			{
				TaskName = "create pve towers"
			});
		}

		protected override void InitSpawn()
		{
			base.InitSpawn();
			MobaMessageManager.RegistMessage((ClientMsg)25030, new MobaMessageFunc(this.OnStartGame));
		}

		protected override void UninitSpawn()
		{
			base.UninitSpawn();
			MobaMessageManager.UnRegistMessage((ClientMsg)25030, new MobaMessageFunc(this.OnStartGame));
		}

		protected override string GetCorrectModelId(string oldId, EntityType type, TeamType team)
		{
			if (LevelManager.Instance.IsPvpBattleType && (type == EntityType.Home || type == EntityType.Tower))
			{
				KeyValuePair<int, int> pvpTowerLevel = Singleton<PvpManager>.Instance.PvpTowerLevel;
				return GameSpawner.GetNewModelIDWithLevel(oldId, (team != TeamType.LM) ? pvpTowerLevel.Value : pvpTowerLevel.Key);
			}
			return oldId;
		}
	}
}
