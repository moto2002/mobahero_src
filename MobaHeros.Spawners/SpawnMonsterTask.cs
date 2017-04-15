using Com.Game.Data;
using System;
using System.Collections.Generic;

namespace MobaHeros.Spawners
{
	public class SpawnMonsterTask : BaseSpawnTask
	{
		private VTrigger _spawnMonsterTrigger;

		private readonly EntityVoCreator _creator;

		private readonly List<Task> _spawnMonsterTasks = new List<Task>();

		private readonly List<EntityVo> _lmMonster = new List<EntityVo>();

		private readonly List<EntityVo> _blMonster = new List<EntityVo>();

		private int _monsterWaveCount;

		public SpawnMonsterTask(SysBattleSceneVo scene, EntityVoCreator creator) : base(scene)
		{
			this._creator = creator;
		}

		public override void Start()
		{
			this.SpawnMonsters();
			this.MyCoroutineManager.StartCoroutine(base.TriggerFinishEvents(true), true);
		}

		public override void Stop()
		{
			base.Stop();
			this.StopSpawnMonsters();
		}

		private void SpawnMonsters()
		{
			this._spawnMonsterTrigger = TriggerManager.CreatePeriodTimerTrigger(this.MyScene.monster_spawn_time, this.MyScene.monster_spawn_interval, -1f, null, new TriggerAction(this.SpawnMonsterPeriod));
		}

		private void StopSpawnMonsters()
		{
			for (int i = 0; i < this._spawnMonsterTasks.Count; i++)
			{
				this.MyCoroutineManager.StopCoroutine(this._spawnMonsterTasks[i]);
			}
			this._spawnMonsterTasks.Clear();
			if (this._spawnMonsterTrigger != null)
			{
				TriggerManager.DestroyTrigger(this._spawnMonsterTrigger);
			}
		}

		private void SpawnMonsterPeriod()
		{
			if (this.MyScene != null)
			{
				this._lmMonster.Clear();
				this._blMonster.Clear();
				this._monsterWaveCount++;
				string[] array = this.MyScene.super_conditions.Split(new char[]
				{
					'|'
				});
				List<EntityVo> entityVos;
				if (array != null && array[0] == "1" && array.Length >= 3 && (float)((int)GameManager.TotalPlayingSeconds) >= float.Parse(array[2]))
				{
					if (GameManager.Instance.AchieveManager.GetHeroDeadCount(TeamType.BL) >= int.Parse(array[1]))
					{
						string[] waveString = StringUtils.GetWaveString(this.MyScene.super_monster_lm);
						string npcConfig = (waveString == null) ? string.Empty : waveString[0];
						entityVos = this._creator.GetEntityVos(npcConfig, EntityType.Monster, TeamType.LM);
						if (entityVos != null)
						{
							this._lmMonster.AddRange(entityVos);
							UIMessageBox.ShowTextPrompt("1115");
						}
					}
					if (GameManager.Instance.AchieveManager.GetHeroDeadCount(TeamType.LM) >= int.Parse(array[1]))
					{
						string[] waveString2 = StringUtils.GetWaveString(this.MyScene.super_monster_bl);
						string npcConfig2 = (waveString2 == null) ? string.Empty : waveString2[0];
						entityVos = this._creator.GetEntityVos(npcConfig2, EntityType.Monster, TeamType.BL);
						if (entityVos != null)
						{
							this._blMonster.AddRange(entityVos);
							UIMessageBox.ShowTextPrompt("1114");
						}
					}
				}
				string[] waveString3 = StringUtils.GetWaveString(this.MyScene.monster_1);
				string npcConfig3 = (waveString3 == null) ? string.Empty : waveString3[0];
				entityVos = this._creator.GetEntityVos(npcConfig3, EntityType.Monster, TeamType.LM);
				if (entityVos != null)
				{
					this._lmMonster.AddRange(entityVos);
				}
				string[] waveString4 = StringUtils.GetWaveString(this.MyScene.monster_2);
				string npcConfig4 = (waveString4 == null) ? string.Empty : waveString4[0];
				entityVos = this._creator.GetEntityVos(npcConfig4, EntityType.Monster, TeamType.BL);
				if (entityVos != null)
				{
					this._blMonster.AddRange(entityVos);
				}
				string[] array2 = this.MyScene.elite_conditions.Split(new char[]
				{
					'|'
				});
				if (array2 != null && array2[0] == "1" && array2.Length >= 2)
				{
					int num = int.Parse(array2[1]);
					if (num > 0 && this._monsterWaveCount % (num + 1) == 0)
					{
						string[] waveString5 = StringUtils.GetWaveString(this.MyScene.elite_monster_lm);
						string npcConfig5 = (waveString5 == null) ? string.Empty : waveString5[0];
						entityVos = this._creator.GetEntityVos(npcConfig5, EntityType.Monster, TeamType.LM);
						if (entityVos != null)
						{
							this._lmMonster.AddRange(entityVos);
						}
						string[] waveString6 = StringUtils.GetWaveString(this.MyScene.elite_monster_bl);
						string npcConfig6 = (waveString6 == null) ? string.Empty : waveString6[0];
						entityVos = this._creator.GetEntityVos(npcConfig6, EntityType.Monster, TeamType.BL);
						if (entityVos != null)
						{
							this._blMonster.AddRange(entityVos);
						}
					}
				}
				float monster_spawn_delay = this.MyScene.monster_spawn_delay;
				float spawnTime = 0f;
				Task item = this.MyCoroutineManager.StartCoroutine(base.SpawnInstances_Coroutinue(this._lmMonster, "Monster", TeamType.LM, string.Empty, spawnTime, monster_spawn_delay, "[]", null, null), true);
				this._spawnMonsterTasks.Add(item);
				item = this.MyCoroutineManager.StartCoroutine(base.SpawnInstances_Coroutinue(this._blMonster, "Monster", TeamType.BL, string.Empty, spawnTime, monster_spawn_delay, "[]", null, null), true);
				this._spawnMonsterTasks.Add(item);
				if (this._monsterWaveCount >= this.MyScene.monster_maxwave)
				{
					TriggerManager.DestroyTrigger(this._spawnMonsterTrigger);
				}
			}
		}
	}
}
