using Com.Game.Data;
using MobaProtocol.Data;
using System;
using System.Collections;

namespace MobaHeros.Spawners
{
	public class SpawnTowerTask : BaseSpawnTask
	{
		private readonly EntityVoCreator _entityVoCreator;

		private readonly BaseSpawnTask.OnSpawnUnit _onSpawnAction;

		public SpawnTowerTask(SysBattleSceneVo scene, EntityVoCreator entityVoCreator, BaseSpawnTask.OnSpawnUnit onSpawnAction) : base(scene)
		{
			this._entityVoCreator = entityVoCreator;
			this._onSpawnAction = onSpawnAction;
		}

		private void SpawnTowers()
		{
			if (this.MyScene.guard_id != "[]")
			{
				string[] stringValue = StringUtils.GetStringValue(this.MyScene.guard_id, ',');
				EntityVo entityVo = this._entityVoCreator.GetEntityVo(stringValue[0], EntityType.Home, TeamType.LM, 1);
				Units units = this.SpawnUtility.SpawnInstance(entityVo, "Home", TeamType.LM, 0, "[]", null, UnitControlType.None, UnitType.None);
				this.OnTowerSpawned(units, entityVo);
				if (stringValue.Length > 1)
				{
					EntityVo entityVo2 = this._entityVoCreator.GetEntityVo(stringValue[1], EntityType.Home, TeamType.BL, 51);
					Units units2 = this.SpawnUtility.SpawnInstance(entityVo2, "Home", TeamType.BL, 0, "[]", null, UnitControlType.None, UnitType.None);
					this.OnTowerSpawned(units2, entityVo2);
				}
			}
			IEnumerator enumerator = null;
			if (this.MyScene.tower_1 != "[]")
			{
				enumerator = base.SpawnInstances_Coroutinue(this._entityVoCreator.GetPvpEntityVos(this.MyScene.tower_1, EntityType.Tower, TeamType.LM, 2), "Building", TeamType.LM, string.Empty, 0f, 0f, "[]", null, new BaseSpawnTask.OnSpawnUnit(this.OnTowerSpawned));
			}
			IEnumerator enumerator2 = null;
			if (this.MyScene.tower_2 != "[]")
			{
				enumerator2 = base.SpawnInstances_Coroutinue(this._entityVoCreator.GetPvpEntityVos(this.MyScene.tower_2, EntityType.Tower, TeamType.BL, 52), "Building", TeamType.BL, string.Empty, 0f, 0f, "[]", null, new BaseSpawnTask.OnSpawnUnit(this.OnTowerSpawned));
			}
			IEnumerator enumerator3 = null;
			if (!string.IsNullOrEmpty(this.MyScene.tower_3) && this.MyScene.tower_3 != "[]")
			{
				enumerator3 = base.SpawnInstances_Coroutinue(this._entityVoCreator.GetPvpEntityVos(this.MyScene.tower_3, EntityType.Tower, TeamType.Team_3, 102), "Building", TeamType.Team_3, string.Empty, 0f, 0f, "[]", null, new BaseSpawnTask.OnSpawnUnit(this.OnTowerSpawned));
			}
			this.MyCoroutineManager.StartCoroutine(BaseSpawnTask.ConcatEnum(new IEnumerator[]
			{
				enumerator,
				enumerator2,
				enumerator3,
				base.TriggerFinishEvents(true)
			}), true);
		}

		private void OnTowerSpawned(Units units, EntityVo entityVo)
		{
			if (units != null && (units.tag.Equals("Building") || units.tag.Equals("Home")))
			{
				Tower tower = units as Tower;
				if (tower != null)
				{
					if (StringUtils.CheckValid(entityVo.atk_priority))
					{
						tower.TowerPriority = int.Parse(entityVo.atk_priority);
					}
					tower.Prewarm();
				}
			}
			if (this._onSpawnAction != null)
			{
				this._onSpawnAction(units, entityVo);
			}
		}

		public override void Start()
		{
			this.SpawnTowers();
		}
	}
}
