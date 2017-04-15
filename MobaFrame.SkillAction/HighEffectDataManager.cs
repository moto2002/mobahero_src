using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class HighEffectDataManager : Singleton<HighEffectDataManager>
	{
		private Dictionary<string, HighEffectData> _dataVos = new Dictionary<string, HighEffectData>();

		private bool isParseTable;

		public void ParseTables()
		{
			if (!this.isParseTable)
			{
				Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysSkillHigheffVo>();
				if (dicByType == null)
				{
					Debug.LogError("==> SysSkillHigheffVo is NULL !!");
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
					SysSkillHigheffVo higheff_vo = current2.Value as SysSkillHigheffVo;
					HighEffectData value = new HighEffectData(key, higheff_vo);
					this._dataVos.Add(key, value);
				}
			}
		}

		public HighEffectData GetVo(string id)
		{
			if (StringUtils.CheckValid(id) && this._dataVos.ContainsKey(id))
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
