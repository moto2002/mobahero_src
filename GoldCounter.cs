using MobaHeros.Spawners;
using System;
using System.Collections.Generic;

public class GoldCounter : UtilCounter
{
	public GoldCounter(UtilType type) : base(type)
	{
	}

	public override void InitCounter()
	{
		List<Units> mapUnits = MapManager.Instance.GetMapUnits(TargetTag.Hero);
		if (mapUnits != null)
		{
			BattleConfigData battleConfigData = UtilManager.Instance.GetUtilDataMgr().GetUtilDataByType(UtilDataType.Battle_config, SceneInfo.Current.BattleAttrIndex) as BattleConfigData;
			foreach (Units current in mapUnits)
			{
				this.AddValue(current.unique_id, new GoldValue(current.unique_id, (int)battleConfigData.GetRateByType(BattleConfigData.Rate_Type.Rate_Init, 0)));
			}
		}
		this.IfInit = true;
	}
}
