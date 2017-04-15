using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections.Generic;

namespace MobaHeros.Spawners
{
	public class PvpSpawner : GameSpawner
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
			Singleton<PvpManager>.Instance.LoadPvpSceneEnd();
		}

		protected override void OnRequestSpawn()
		{
			Dictionary<TeamType, List<EntityVo>> heroDict = new Dictionary<TeamType, List<EntityVo>>
			{
				{
					TeamType.LM,
					this.GetHeroes(TeamType.LM)
				},
				{
					TeamType.BL,
					this.GetHeroes(TeamType.BL)
				},
				{
					TeamType.Team_3,
					this.GetHeroes(TeamType.Team_3)
				}
			};
			base.StartSpawnTask(new SpawnPvpHeroTask(this.MyScene, heroDict, new BaseSpawnTask.OnSpawnUnit(base.OnHeroUnitSpawned))
			{
				TaskName = "create pvp heroes"
			});
			base.StartSpawnTask(new SpawnTowerTask(this.MyScene, this.MyEntityVoCreator, null)
			{
				TaskName = "create pvp towers"
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
				return oldId;
			}
			return oldId;
		}
	}
}
