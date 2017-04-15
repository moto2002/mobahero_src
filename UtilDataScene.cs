using System;
using System.Collections.Generic;

public class UtilDataScene
{
	private int _battleAttrIndex;

	private Dictionary<UtilDataType, UtilData> _allData;

	public UtilDataScene(int inBattleAttrIndex)
	{
		this._battleAttrIndex = inBattleAttrIndex;
		this.Init();
	}

	private void Init()
	{
		this._allData = new Dictionary<UtilDataType, UtilData>();
		for (int i = 0; i < Enum.GetNames(typeof(UtilDataType)).Length; i++)
		{
			UtilData value = this.CreateData((UtilDataType)i, this._battleAttrIndex);
			this._allData.Add((UtilDataType)i, value);
		}
	}

	private UtilData CreateData(UtilDataType type, int id)
	{
		UtilData result = null;
		switch (type)
		{
		case UtilDataType.Battle_attr_reward:
			result = new UtilMonsterData(id.ToString());
			break;
		case UtilDataType.Battle_config:
			result = new BattleConfigData(id.ToString());
			break;
		case UtilDataType.Battle_exp:
			result = new UtilExpData(id.ToString());
			break;
		case UtilDataType.Battle_skill_ai_config:
			result = new SkillAiConfigData(id.ToString());
			break;
		}
		return result;
	}

	public UtilData GetDataByType(UtilDataType type)
	{
		UtilData result = null;
		if (!this._allData.TryGetValue(type, out result))
		{
			result = null;
		}
		return result;
	}
}
