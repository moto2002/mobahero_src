using Com.Game.Data;
using Com.Game.Manager;
using System;

namespace MobaHeros.Spawners
{
	public class SceneInfo
	{
		public readonly bool IsOpenAdditionFactor;

		public readonly int BattleAttrIndex = 101;

		private static SceneInfo _cachedInfo;

		private static string _sceneId;

		public static SceneInfo Current
		{
			get
			{
				string curLevelId = LevelManager.CurLevelId;
				if (SceneInfo._sceneId != curLevelId)
				{
					SceneInfo._sceneId = curLevelId;
					SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(curLevelId);
					SceneInfo._cachedInfo = new SceneInfo(dataById);
				}
				return SceneInfo._cachedInfo;
			}
		}

		public SceneInfo(SysBattleSceneVo scene)
		{
			this.IsOpenAdditionFactor = (scene.isOpenAddiionFactor >= 101);
			if (this.IsOpenAdditionFactor)
			{
				this.BattleAttrIndex = scene.isOpenAddiionFactor;
			}
		}
	}
}
