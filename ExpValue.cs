using Com.Game.Module;
using MobaHeros.Pvp;
using MobaHeros.Spawners;
using System;
using UnityEngine;

public class ExpValue : CounterValue
{
	private float _curExp;

	private int _onwerId;

	private int _curLv;

	private UtilExpData _expData;

	private float _richerExp;

	public float CurExp
	{
		get
		{
			return this._curExp;
		}
	}

	public float RicherExp
	{
		get
		{
			return this._richerExp;
		}
	}

	public float RicherExpRatio
	{
		get
		{
			int lvUpExp = this.GetLvUpExp(this.CurLv);
			if (lvUpExp == -1)
			{
				return 1f;
			}
			return this._richerExp / (float)lvUpExp;
		}
	}

	public int CurLv
	{
		get
		{
			this._curLv = this.GetLvByExp(this._curExp);
			return this._curLv;
		}
	}

	public ExpValue(int uid, float initExp = 0f)
	{
		this._onwerId = uid;
		this._curExp = initExp;
		this._expData = null;
		if (UtilManager.Instance.GetUtilDataMgr() != null)
		{
			this._expData = (UtilManager.Instance.GetUtilDataMgr().GetUtilDataByType(UtilDataType.Battle_exp, SceneInfo.Current.BattleAttrIndex) as UtilExpData);
		}
	}

	public int GetLvByExp(float exp)
	{
		return (this._expData == null) ? 1 : this._expData.GetLvByTotalExp((int)exp);
	}

	public int GetTotalExpByLv(int lv)
	{
		return (this._expData == null) ? 0 : this._expData.GetTotalExpByLv(lv);
	}

	public int GetLvUpExp(int lv)
	{
		return (this._expData == null) ? -1 : this._expData.GetExp2LevelUp(lv);
	}

	public float GetDeadExp(int inCurLv)
	{
		if (this._expData != null)
		{
			return (float)this._expData.GetKillExp(inCurLv);
		}
		return 0f;
	}

	public void AddExp(float exp)
	{
		if (this._curLv >= this._expData.MaxLv)
		{
			return;
		}
		this._curExp += exp;
		int curLv = this._curLv;
		this._curLv = this.GetLvByExp(this._curExp);
		this.OnLvChange(curLv, this._curLv);
		this.CheckRicherExp(exp);
		Units unit = MapManager.Instance.GetUnit(this._onwerId);
	}

	private void CheckRicherExp(float exp)
	{
		int totalExpByLv = this.GetTotalExpByLv(this._curLv);
		this._richerExp = this._curExp - (float)totalExpByLv;
	}

	private void OnLvChange(int originLv, int newLv)
	{
		int num = newLv - originLv;
		Units player = PlayerControlMgr.Instance.GetPlayer();
		if (player == null)
		{
			return;
		}
		if (num > 0 && originLv > 0 && this._onwerId != player.unique_id)
		{
			SkillCounter skillCounter = UtilManager.Instance.GetCounter(UtilType.Skill) as SkillCounter;
			if (skillCounter == null)
			{
				Debug.LogError("no counter get for UtilType.Skill : " + this._onwerId);
				return;
			}
			Units unit = MapManager.Instance.GetUnit(this._onwerId);
			if (unit == null)
			{
				Debug.LogError("no hero get for id : " + this._onwerId);
				return;
			}
			skillCounter.OnHeroLevelup(unit, newLv);
		}
		if (num > 0)
		{
			this.ShowLevelUpEffect(this._onwerId);
			Units unit2 = MapManager.Instance.GetUnit(this._onwerId);
			if (null != unit2 && unit2.isHero)
			{
				unit2.level = newLv;
			}
			if (this._onwerId == player.unique_id && !Singleton<PvpManager>.Instance.IsInPvp)
			{
				Singleton<SkillView>.Instance.AddSkillPoint(num);
			}
		}
	}

	private void ShowLevelUpEffect(int uid)
	{
		Units unit = MapManager.Instance.GetUnit(uid);
		unit.StartLevelEffect();
		if (unit.isPlayer)
		{
			unit.playVoice("onlevelup");
		}
	}

	public override void Update()
	{
	}
}
