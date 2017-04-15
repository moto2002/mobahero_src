using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class BuffDataManager : Singleton<BuffDataManager>
	{
		private Dictionary<string, BuffData> _dataVos = new Dictionary<string, BuffData>();

		private bool isParseTable;

		public void ParseTables()
		{
			if (!this.isParseTable)
			{
				Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysSkillBuffVo>();
				if (dicByType == null)
				{
					Debug.LogError("==> SysSkillPerformVo is NULL !!");
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
					SysSkillBuffVo buff_vo = current2.Value as SysSkillBuffVo;
					BuffData value = new BuffData(key, buff_vo);
					this._dataVos.Add(key, value);
				}
			}
		}

		public BuffData GetVo(string id)
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
