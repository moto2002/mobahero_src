using System;
using System.Collections.Generic;

public class UtilDataMgr
{
	private Dictionary<int, UtilDataScene> _allUtilDataScenes;

	public UtilDataMgr()
	{
		this.Init();
	}

	private void Init()
	{
		this._allUtilDataScenes = new Dictionary<int, UtilDataScene>();
	}

	public UtilData GetUtilDataByType(UtilDataType inType, int inBattleAttrIndex)
	{
		UtilDataScene utilDataSceneById = this.GetUtilDataSceneById(inBattleAttrIndex, true);
		if (utilDataSceneById != null)
		{
			return utilDataSceneById.GetDataByType(inType);
		}
		return null;
	}

	private UtilDataScene GetUtilDataSceneById(int inBattleAttrIndex, bool inCreateIfNotExist)
	{
		UtilDataScene utilDataScene = null;
		if (!this._allUtilDataScenes.TryGetValue(inBattleAttrIndex, out utilDataScene))
		{
			if (inCreateIfNotExist)
			{
				utilDataScene = new UtilDataScene(inBattleAttrIndex);
				this._allUtilDataScenes.Add(inBattleAttrIndex, utilDataScene);
			}
			else
			{
				utilDataScene = null;
			}
		}
		return utilDataScene;
	}
}
