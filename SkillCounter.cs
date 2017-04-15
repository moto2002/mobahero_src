using Com.Game.Module;
using MobaHeros.Pvp;
using MobaHeros.Spawners;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillCounter : UtilCounter
{
	private SkillAiConfigData _configData;

	private UtilExpData _expData;

	public SkillCounter(UtilType type) : base(type)
	{
	}

	public override void InitCounter()
	{
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			return;
		}
		this._configData = (UtilManager.Instance.GetUtilDataMgr().GetUtilDataByType(UtilDataType.Battle_skill_ai_config, SceneInfo.Current.BattleAttrIndex) as SkillAiConfigData);
		this._expData = (UtilManager.Instance.GetUtilDataMgr().GetUtilDataByType(UtilDataType.Battle_exp, SceneInfo.Current.BattleAttrIndex) as UtilExpData);
		if (this._configData == null || this._expData == null)
		{
			Debug.LogError("no configData get");
		}
		this.InitSkillPoints();
	}

	public void OnHeroLevelup(Units hero, int newLv)
	{
		this.AddSkillPoints(hero, newLv);
	}

	private void InitSkillPoints()
	{
		List<Units> mapUnits = MapManager.Instance.GetMapUnits(TargetTag.Hero);
		if (mapUnits != null)
		{
			foreach (Units current in mapUnits)
			{
				if (!current.isPlayer)
				{
					this.AddSkillPoints(current, 1);
				}
			}
		}
	}

	private void AddSkillPoints(Units hero, int curLv)
	{
		if (curLv < 1 || curLv > this._expData.MaxLv)
		{
			return;
		}
		int indexToLevelup = this._configData.GetIndexToLevelup(hero.npc_id, curLv);
		if (hero.skillManager.skills_index.ContainsKey(indexToLevelup - 1))
		{
			string skillID = hero.skillManager.skills_index[indexToLevelup - 1];
			hero.skillManager.UpgradeSkillLevel(skillID);
		}
	}
}
