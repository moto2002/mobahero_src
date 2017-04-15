using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroKillData
{
	public enum KillState
	{
		State_Kill_Hero,
		State_Killed_By_Hero,
		State_Killed_By_Not_Hero
	}

	private int _uid;

	private int _continusKillCount;

	private int _deathCount;

	private List<float> _deathRecord;

	public int ContinusKillCount
	{
		get
		{
			if (this._continusKillCount < 10)
			{
				return this._continusKillCount;
			}
			return 10;
		}
	}

	public int DeathCount
	{
		get
		{
			if (this._deathCount < 7)
			{
				return this._deathCount;
			}
			return 7;
		}
	}

	public HeroKillData(int id)
	{
		this._uid = id;
		this._deathRecord = new List<float>();
	}

	public void UpdateState(HeroKillData.KillState state)
	{
		switch (state)
		{
		case HeroKillData.KillState.State_Kill_Hero:
			this._continusKillCount++;
			break;
		case HeroKillData.KillState.State_Killed_By_Hero:
			this._continusKillCount = 0;
			this._deathCount++;
			this._deathRecord.Add(Time.time);
			break;
		case HeroKillData.KillState.State_Killed_By_Not_Hero:
			this._deathCount++;
			this._deathRecord.Add(Time.time);
			break;
		}
	}
}
