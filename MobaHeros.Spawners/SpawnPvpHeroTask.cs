using Com.Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MobaHeros.Spawners
{
	public class SpawnPvpHeroTask : BaseSpawnTask
	{
		private readonly Dictionary<TeamType, List<EntityVo>> _heroDict;

		private readonly BaseSpawnTask.OnSpawnUnit _onSpawnAction;

		public SpawnPvpHeroTask(SysBattleSceneVo scene, Dictionary<TeamType, List<EntityVo>> heroDict, BaseSpawnTask.OnSpawnUnit onSpawnAction) : base(scene)
		{
			this._heroDict = heroDict;
			this._onSpawnAction = onSpawnAction;
		}

		public override void Start()
		{
			this.MyScene.hero2_location = "11,12,13,14,15";
			IEnumerator[] array = new IEnumerator[this._heroDict.Count + 1];
			int num = 0;
			foreach (TeamType current in this._heroDict.Keys)
			{
				array[num++] = base.DoSpawnPvpHeroes_Coroutinue(this._heroDict[current], "Hero", current, this.MyScene.hero2_location, 0f, 0f, null, this._onSpawnAction);
			}
			array[num++] = base.TriggerFinishEvents(true);
			this.MyCoroutineManager.StartCoroutine(BaseSpawnTask.ConcatEnum(array), true);
		}
	}
}
