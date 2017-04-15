using Com.Game.Data;
using Com.Game.Manager;
using MobaHeros;
using System;
using System.Collections.Generic;

namespace MobaFrame.SkillAction
{
	public class SpawnMonsterAction : BaseHighEffAction
	{
		protected override void StartHighEff()
		{
			SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(this.data.strParam1);
			Dictionary<DataType, object> dictionary = new Dictionary<DataType, object>();
			if (this.data.param1 == 1f)
			{
				dictionary.Add(DataType.NameId, this.data.strParam1);
				dictionary.Add(DataType.ModelId, monsterMainData.model_id);
				dictionary.Add(DataType.TeamType, base.unit.teamType);
				dictionary.Add(DataType.AttrFactor, GameManager.Instance.Spawner.GetAttrFactor((TeamType)base.unit.teamType));
				dictionary.Add(DataType.AIType, (int)this.data.param2);
				Units units = MapManager.Instance.SpawnMonster(dictionary, null, base.unit.mTransform.position, base.unit.mTransform.rotation);
			}
			else if (this.data.param1 != 2f)
			{
				if (this.data.param1 != 3f)
				{
					if (this.data.param1 != 4f)
					{
						if (this.data.param1 == 5f)
						{
							dictionary.Add(DataType.NameId, this.data.strParam1);
							dictionary.Add(DataType.TeamType, base.unit.teamType);
							Units units = MapManager.Instance.SpawnBuffItem(dictionary, null, base.unit.mTransform.position, base.unit.mTransform.rotation, UnitControlType.None);
						}
					}
				}
			}
		}

		protected override void StopHighEff()
		{
		}
	}
}
