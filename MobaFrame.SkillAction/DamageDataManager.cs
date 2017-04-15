using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class DamageDataManager : Singleton<DamageDataManager>
	{
		private Dictionary<int, DamageData> _dataVos = new Dictionary<int, DamageData>();

		private bool isParseTable;

		public void ParseTables()
		{
			if (!this.isParseTable)
			{
				Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysSkillDamageVo>();
				if (dicByType == null)
				{
					Debug.LogError("==> SysSkillDamageVo is NULL !!");
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
					SysSkillDamageVo damage_vo = current2.Value as SysSkillDamageVo;
					DamageData value = new DamageData(key, damage_vo);
					this._dataVos.Add(int.Parse(key), value);
				}
			}
		}

		public DamageData GetVo(int id)
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
