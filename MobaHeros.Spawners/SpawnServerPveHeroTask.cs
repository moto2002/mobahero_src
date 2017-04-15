using Com.Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MobaHeros.Spawners
{
	public class SpawnServerPveHeroTask : BaseSpawnTask
	{
		private readonly List<EntityVo> _lmHeroList;

		private readonly List<EntityVo> _blHeroList;

		private readonly BaseSpawnTask.OnSpawnUnit _onSpawnAction;

		public SpawnServerPveHeroTask(SysBattleSceneVo scene, List<EntityVo> lmHeroList, List<EntityVo> blHeroList, BaseSpawnTask.OnSpawnUnit onSpawnAction) : base(scene)
		{
			this._lmHeroList = lmHeroList;
			this._blHeroList = blHeroList;
			this._onSpawnAction = onSpawnAction;
		}

		public override void Start()
		{
			this.MyScene.hero2_location = "11,12,13,14,15";
			IEnumerator enumerator = base.DoSpawnPvpHeroes_Coroutinue(this._lmHeroList, "Hero", TeamType.LM, this.MyScene.hero2_location, 0f, 0f, null, this._onSpawnAction);
			IEnumerator enumerator2 = base.DoSpawnPvpHeroes_Coroutinue(this._blHeroList, "Hero", TeamType.BL, this.MyScene.hero2_location, 0f, 0f, null, this._onSpawnAction);
			this.MyCoroutineManager.StartCoroutine(BaseSpawnTask.ConcatEnum(new IEnumerator[]
			{
				enumerator,
				enumerator2,
				base.TriggerFinishEvents(true)
			}), true);
		}
	}
}
