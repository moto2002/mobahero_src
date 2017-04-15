using MobaHeros;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace MobaFrame.SkillAction
{
	public abstract class BaseDataAction : BaseAction
	{
		public DataUpdateInfo dataUpdateInfo = new DataUpdateInfo();

		protected List<int> dataKeys = new List<int>();

		protected List<float> dataValues = new List<float>();

		protected virtual void GetKeyData(Units target, ref List<int> dataKeys, ref List<float> dataValues)
		{
			dataKeys.Clear();
			dataValues.Clear();
			float item = target.hp;
			float item2 = target.mp;
			if (!target.isLive)
			{
				item = 0f;
				item2 = 0f;
			}
			dataKeys.Add(1);
			dataValues.Add(item);
			dataKeys.Add(2);
			dataValues.Add(item2);
		}

		protected void DoHuiGuangFanZhao(Units target, List<DataInfo> infos)
		{
			if (!target.HuiGuangFanZhao.IsInState)
			{
				return;
			}
			for (int i = 0; i < infos.Count; i++)
			{
				if (infos[i].key == AttrType.Hp && infos[i].value < 0f)
				{
					infos[i].value = -infos[i].value;
				}
			}
		}

		protected void DoHuiGuangFanZhao(Units target, List<short> keys, ref List<float> values)
		{
			if (!target.HuiGuangFanZhao.IsInState)
			{
				return;
			}
			for (int i = 0; i < keys.Count; i++)
			{
				if (keys[i] == 1 && values[i] < 0f)
				{
					values[i] = -values[i];
				}
			}
		}
	}
}
