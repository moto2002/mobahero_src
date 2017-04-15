using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleConfigData : UtilData
{
	public enum Bounus_Type
	{
		Bonus_FirstKill_Monster = 1,
		Bonus_FirstKill_Hero,
		Bonus_Finish_Killing
	}

	public enum Rate_Type
	{
		Rate_Normal,
		Rate_Enemy_Lv_Based,
		Rate_Enemy_Kill_Based,
		Rate_Salary,
		Rate_Bonus,
		Rate_4_Gold,
		Rate_4_Exp,
		Rate_Init,
		Rate_DeathRatio,
		Rate_Exp
	}

	private Dictionary<int, float> _gold4ratio;

	private Dictionary<int, float> _exp4ratio;

	private Dictionary<BattleConfigData.Bounus_Type, float> _bonus;

	private Dictionary<BattleConfigData.Rate_Type, float> _otherRates;

	public BattleConfigData(string id) : base(id)
	{
	}

	protected override void InitConfig()
	{
		this._gold4ratio = new Dictionary<int, float>();
		this._exp4ratio = new Dictionary<int, float>();
		this._bonus = new Dictionary<BattleConfigData.Bounus_Type, float>();
		this._otherRates = new Dictionary<BattleConfigData.Rate_Type, float>();
		SysBattleAttrConfigNewVo dataById = BaseDataMgr.instance.GetDataById<SysBattleAttrConfigNewVo>(this._id);
		if (dataById == null)
		{
			Debug.LogError("no data from battle_attr_config_new for id: " + this._id);
			return;
		}
		this._otherRates.Add(BattleConfigData.Rate_Type.Rate_Normal, (float)dataById.rate_normal);
		this._otherRates.Add(BattleConfigData.Rate_Type.Rate_Enemy_Lv_Based, dataById.rate_kill_hero_ratio);
		this._otherRates.Add(BattleConfigData.Rate_Type.Rate_Enemy_Kill_Based, dataById.rate_title_ratio);
		this._otherRates.Add(BattleConfigData.Rate_Type.Rate_Salary, (float)dataById.salary);
		this._otherRates.Add(BattleConfigData.Rate_Type.Rate_Exp, dataById.exp);
		this._otherRates.Add(BattleConfigData.Rate_Type.Rate_Init, (float)dataById.rate_init);
		this._otherRates.Add(BattleConfigData.Rate_Type.Rate_DeathRatio, dataById.death_ratio);
		if (StringUtils.CheckValid(dataById.rate_bonus))
		{
			string[] stringValues = StringUtils.GetStringValues(dataById.rate_bonus, 1, ',', '|');
			if (stringValues != null)
			{
				this._bonus.Add(BattleConfigData.Bounus_Type.Bonus_FirstKill_Monster, float.Parse(stringValues[0]));
				this._bonus.Add(BattleConfigData.Bounus_Type.Bonus_FirstKill_Hero, float.Parse(stringValues[1]));
				this._bonus.Add(BattleConfigData.Bounus_Type.Bonus_Finish_Killing, float.Parse(stringValues[2]));
			}
		}
		if (StringUtils.CheckValid(dataById.number_ratio))
		{
			string[] stringValues2 = StringUtils.GetStringValues(dataById.number_ratio, 1, ',', '|');
			for (int i = 0; i < stringValues2.Length; i++)
			{
				this._exp4ratio.Add(i + 1, float.Parse(stringValues2[i]));
			}
		}
		if (StringUtils.CheckValid(dataById.asis_ratio))
		{
			string[] stringValues3 = StringUtils.GetStringValues(dataById.asis_ratio, 1, ',', '|');
			for (int j = 0; j < stringValues3.Length; j++)
			{
				this._gold4ratio.Add(j + 1, float.Parse(stringValues3[j]));
			}
		}
	}

	public float GetRateByType(BattleConfigData.Rate_Type type, int idx = 0)
	{
		switch (type)
		{
		case BattleConfigData.Rate_Type.Rate_Normal:
			return this._otherRates[BattleConfigData.Rate_Type.Rate_Normal];
		case BattleConfigData.Rate_Type.Rate_Enemy_Lv_Based:
			return this._otherRates[BattleConfigData.Rate_Type.Rate_Enemy_Lv_Based];
		case BattleConfigData.Rate_Type.Rate_Enemy_Kill_Based:
			return this._otherRates[BattleConfigData.Rate_Type.Rate_Enemy_Kill_Based];
		case BattleConfigData.Rate_Type.Rate_Salary:
			return this._otherRates[BattleConfigData.Rate_Type.Rate_Salary];
		case BattleConfigData.Rate_Type.Rate_Bonus:
		{
			float result = 0f;
			if (!this._bonus.TryGetValue((BattleConfigData.Bounus_Type)idx, out result))
			{
				result = 0f;
			}
			return result;
		}
		case BattleConfigData.Rate_Type.Rate_4_Gold:
		{
			float result2 = 0f;
			if (!this._gold4ratio.TryGetValue(idx, out result2))
			{
				result2 = 0f;
			}
			return result2;
		}
		case BattleConfigData.Rate_Type.Rate_4_Exp:
		{
			float result3 = 0f;
			if (!this._exp4ratio.TryGetValue(idx, out result3))
			{
				result3 = 0f;
			}
			return result3;
		}
		case BattleConfigData.Rate_Type.Rate_Init:
			return this._otherRates[BattleConfigData.Rate_Type.Rate_Init];
		case BattleConfigData.Rate_Type.Rate_DeathRatio:
			return this._otherRates[BattleConfigData.Rate_Type.Rate_DeathRatio];
		case BattleConfigData.Rate_Type.Rate_Exp:
			return this._otherRates[BattleConfigData.Rate_Type.Rate_Exp];
		default:
			return 0f;
		}
	}
}
