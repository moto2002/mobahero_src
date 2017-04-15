using MobaHeros.Spawners;
using System;
using UnityEngine;

public class GoldValue : CounterValue
{
	private int _currentGold;

	private int _ownerId;

	private float _lastUpdateTime;

	private float _interval = 1f;

	public int CurrentGold
	{
		get
		{
			return this._currentGold;
		}
	}

	public GoldValue(int id, int initGold)
	{
		this._ownerId = id;
		this._currentGold = initGold;
	}

	public override void Update()
	{
		if (Time.time - this._lastUpdateTime > this._interval)
		{
			this._lastUpdateTime = Time.time;
			BattleConfigData battleConfigData = UtilManager.Instance.GetUtilDataMgr().GetUtilDataByType(UtilDataType.Battle_config, SceneInfo.Current.BattleAttrIndex) as BattleConfigData;
			this.ChangeGold((int)battleConfigData.GetRateByType(BattleConfigData.Rate_Type.Rate_Salary, 0));
		}
	}

	public void ChangeGold(int addition)
	{
		this._currentGold += addition;
	}
}
