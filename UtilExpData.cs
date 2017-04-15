using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UtilExpData : UtilData
{
	private static UtilExpData _instance;

	private Dictionary<int, int> _levelDics;

	private Dictionary<int, int> _killExpDics;

	private int _maxLv;

	public static UtilExpData Instance
	{
		get
		{
			if (UtilExpData._instance == null)
			{
				UtilExpData._instance = new UtilExpData(string.Empty);
			}
			return UtilExpData._instance;
		}
	}

	public int MaxLv
	{
		get
		{
			return this._maxLv;
		}
	}

	public UtilExpData(string id) : base(id)
	{
	}

	protected override void InitConfig()
	{
		this._levelDics = new Dictionary<int, int>();
		this._killExpDics = new Dictionary<int, int>();
		Dictionary<string, object>.ValueCollection values = BaseDataMgr.instance.GetDicByType<SysBattleAttrLvVo>().Values;
		this._maxLv = values.Count;
		for (int i = 1; i <= this._maxLv; i++)
		{
			SysBattleAttrLvVo dataById = BaseDataMgr.instance.GetDataById<SysBattleAttrLvVo>(i.ToString());
			if (dataById != null)
			{
				this._levelDics.Add(i, (int)dataById.exp_to_next);
				this._killExpDics.Add(i, dataById.exp_to_kill);
			}
			else
			{
				Debug.LogError("no data from SysBattleAttrLvVo with id: " + i);
			}
		}
	}

	public int GetExp2LevelUp(int curLv)
	{
		if (this._levelDics.ContainsKey(curLv))
		{
			return this._levelDics[curLv];
		}
		return -1;
	}

	public int GetLvByTotalExp(int exp)
	{
		int i;
		for (i = 1; i <= this._maxLv; i++)
		{
			int num = this._levelDics[i];
			exp -= num;
			if (exp < 0)
			{
				return i;
			}
		}
		return i;
	}

	public int GetTotalExpByLv(int lv)
	{
		if (lv <= 1)
		{
			return 0;
		}
		if (lv > this._maxLv)
		{
			lv = this._maxLv;
		}
		int num = 0;
		for (int i = 1; i <= lv - 1; i++)
		{
			num += this._levelDics[i];
		}
		return num;
	}

	public int GetKillExp(int curLv)
	{
		int result = 0;
		if (this._killExpDics.TryGetValue(curLv, out result))
		{
			return result;
		}
		return -1;
	}
}
