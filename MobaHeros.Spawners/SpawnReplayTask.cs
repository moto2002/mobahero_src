using Com.Game.Data;
using System;

namespace MobaHeros.Spawners
{
	public class SpawnReplayTask : BaseSpawnTask
	{
		public SpawnReplayTask(SysBattleSceneVo scene, EntityVoCreator creator) : base(scene)
		{
		}

		public override void Start()
		{
			this.MyCoroutineManager.StartCoroutine(base.TriggerFinishEvents(true), true);
		}
	}
}
