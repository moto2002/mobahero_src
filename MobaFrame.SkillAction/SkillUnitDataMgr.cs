using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class SkillUnitDataMgr : Singleton<SkillUnitDataMgr>
	{
		private Dictionary<string, SkillUnitData> _dataVos = new Dictionary<string, SkillUnitData>();

		private bool isParseTable;

		public void ParseTables()
		{
			if (!this.isParseTable)
			{
				Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysSkillUnitVo>();
				if (dicByType == null)
				{
					Debug.LogError("==> SysSkillUnitVo is NULL !!");
					return;
				}
				this.isParseTable = true;
				this._dataVos.Clear();
				Dictionary<string, object>.Enumerator enumerator = dicByType.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, object> current = enumerator.Current;
					string key = current.Key;
					KeyValuePair<string, object> current2 = enumerator.Current;
					SysSkillUnitVo config = current2.Value as SysSkillUnitVo;
					SkillUnitData value = new SkillUnitData(key, config);
					this._dataVos.Add(key, value);
				}
			}
		}

		public SkillUnitData GetVo(string id)
		{
			if (this._dataVos.ContainsKey(id))
			{
				return this._dataVos[id];
			}
			return null;
		}

		public void Clear()
		{
			this._dataVos.Clear();
		}
	}
}
