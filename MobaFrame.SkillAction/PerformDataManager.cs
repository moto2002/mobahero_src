using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class PerformDataManager : Singleton<PerformDataManager>
	{
		private Dictionary<string, PerformData> _dataVos = new Dictionary<string, PerformData>();

		private bool isParseTable;

		public void ParseTables()
		{
			if (!this.isParseTable)
			{
				Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysSkillPerformVo>();
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
					SysSkillPerformVo perform_vo = current2.Value as SysSkillPerformVo;
					PerformData value = new PerformData(key, perform_vo);
					this._dataVos.Add(key, value);
				}
			}
		}

		public PerformData GetVo(string id)
		{
			PerformData result;
			if (this._dataVos.TryGetValue(id, out result))
			{
				return result;
			}
			return null;
		}

		public void Clear()
		{
			this._dataVos.Clear();
		}
	}
}
