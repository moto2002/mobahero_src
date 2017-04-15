using Com.Game.Data;
using Com.Game.Module;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MobaHeros.Spawners
{
	public class SpawnHeroTask : BaseSpawnTask
	{
		private readonly List<EntityVo> _lmHeroList;

		private readonly List<EntityVo> _blHeroList;

		private readonly BaseSpawnTask.OnSpawnUnit _onSpawnAction;

		private readonly List<VTrigger> _respawnTriggers = new List<VTrigger>();

		public SpawnHeroTask(SysBattleSceneVo scene, List<EntityVo> lmHeroList, List<EntityVo> blHeroList, BaseSpawnTask.OnSpawnUnit onSpawnAction) : base(scene)
		{
			this._lmHeroList = lmHeroList;
			this._blHeroList = blHeroList;
			this._onSpawnAction = onSpawnAction;
		}

		private IEnumerator SpawnLmHeros()
		{
			if (this._lmHeroList != null && this._lmHeroList.Count > 0)
			{
				int[] playerPos = new int[]
				{
					11,
					12,
					13,
					14,
					15
				};
				return base.DoSpawnPlayers_Coroutinue(this._lmHeroList, playerPos, 0f, 0f, this.MyScene.hero1_spawn_interval, null, new BaseSpawnTask.OnSpawnUnit(this.OnHeroSpawned));
			}
			return null;
		}

		private IEnumerator SpawnBlHeros()
		{
			if (this._blHeroList != null && this._blHeroList.Count > 0)
			{
				return base.SpawnInstances_Coroutinue(this._blHeroList, "Hero", TeamType.BL, this.MyScene.hero2_location, 0f, 0f, this.MyScene.hero2_spawn_interval, null, new BaseSpawnTask.OnSpawnUnit(this.OnHeroSpawned));
			}
			return null;
		}

		public override void Start()
		{
			this.MyCoroutineManager.StartCoroutine(BaseSpawnTask.ConcatEnum(new IEnumerator[]
			{
				this.SpawnLmHeros(),
				this.SpawnBlHeros(),
				base.TriggerFinishEvents(true)
			}), true);
		}

		private void ShowDeathUI(float spawnInterval)
		{
			UIMessageBox.DeathBox(spawnInterval);
		}

		private void OnHeroSpawned(Units hero, EntityVo entityVo)
		{
			if (hero != null)
			{
				VTrigger item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.RespawnHero), hero.unique_id);
				this._respawnTriggers.Add(item);
			}
			if (this._onSpawnAction != null)
			{
				this._onSpawnAction(hero, entityVo);
			}
		}

		private void RespawnHero()
		{
			if (this.MyScene == null)
			{
				return;
			}
			Units triggerUnit = TriggerManager.GetTriggerUnit();
			if (triggerUnit == null)
			{
				return;
			}
			if (triggerUnit.IsReplayControl)
			{
				return;
			}
			string text = (triggerUnit.TeamType != TeamType.LM) ? this.MyScene.hero2_spawn_interval : this.MyScene.hero1_spawn_interval;
			if ("[]" == text)
			{
				return;
			}
			float[] stringToFloat = StringUtils.GetStringToFloat(text, ',');
			if (stringToFloat == null)
			{
				return;
			}
			float num = stringToFloat[0];
			float num2 = stringToFloat[1];
			float num3 = stringToFloat[2];
			float num4 = 0f;
			float num5 = 45f;
			if (stringToFloat.Length > 3)
			{
				num5 = stringToFloat[3];
			}
			if (stringToFloat.Length > 4)
			{
				num4 = stringToFloat[4];
			}
			float totalPlayingSeconds = GameManager.TotalPlayingSeconds;
			int num6;
			if (triggerUnit.TeamType == TeamType.LM)
			{
				num6 = MyStatistic.Instance.GetDataByTeam(TeamType.BL).heroKill.TotalKill - 1;
			}
			else
			{
				num6 = MyStatistic.Instance.GetDataByTeam(TeamType.LM).heroKill.TotalKill - 1;
			}
			float num7 = num + (float)((int)(totalPlayingSeconds / 30f)) * num2 + (float)num6 * num3 + (float)triggerUnit.level * num4;
			if (num7 < 0f)
			{
				num7 = 5f;
			}
			if (num7 > num5)
			{
				num7 = num5;
			}
			if (triggerUnit.TeamType == TeamType.LM)
			{
				this.ShowDeathUI(num7);
			}
			this.MyCoroutineManager.StartCoroutine(this.SpawnUtility.RespawnPveHero_Coroutinue(triggerUnit, num7, 0f), true);
		}
	}
}
