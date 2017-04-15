using Com.Game.Data;
using Com.Game.Manager;
using System;
using UnityEngine;

[Serializable]
public class BattleRefreshVo
{
	public string br_id;

	public GameEvent trigerCdn;

	public int trigerCdnIntParam1;

	public string trigerCndStrParam1;

	public float trigerCdnFloatParam1;

	public float trigerCdnFloatParam2;

	public float trigerCdnFloatParam3;

	public int delayTime;

	public BattleRefreshWay refreshWay = BattleRefreshWay.Position;

	public int[] posIndex;

	public string npcID;

	public TeamType teamType = TeamType.Neutral;

	public CharacterAIType npcAI = CharacterAIType.AI_Monster;

	public EntityType entityType;

	public int SpawnerType = 1;

	public int refreshNumber = 1;

	public int cycleNumber;

	public int cycleInterval;

	public float curNumber;

	public float curNumber2;

	public bool isDestroy;

	public BattleRefreshVo(string brId)
	{
		this.br_id = brId;
		SysBattleRefreshVo dataById = BaseDataMgr.instance.GetDataById<SysBattleRefreshVo>(brId);
		if (dataById == null)
		{
			Debug.LogError("brVo is null error id:" + brId + " 请检查配置表BattleRefresh");
			return;
		}
		if (dataById.trigger_condition != "[]")
		{
			string[] stringValue = StringUtils.GetStringValue(dataById.trigger_condition, '|');
			this.trigerCdn = (GameEvent)((stringValue == null) ? 0 : int.Parse(stringValue[0]));
			this.SetTrigerCondition(stringValue);
		}
		this.delayTime = dataById.delay_time;
		this.refreshWay = (BattleRefreshWay)dataById.refresh_way;
		if (dataById.locationid != "[]")
		{
			this.posIndex = StringUtils.GetStringToInt(dataById.locationid, ',');
		}
		if (dataById.monster_mainid != "[]")
		{
			string[] stringValue2 = StringUtils.GetStringValue(dataById.monster_mainid, '|');
			this.npcID = stringValue2[0];
			this.SpawnerType = int.Parse(stringValue2[1]);
			this.teamType = (TeamType)int.Parse(stringValue2[2]);
			this.npcAI = (CharacterAIType)int.Parse(stringValue2[3]);
		}
		this.refreshNumber = dataById.refresh_number;
		this.cycleNumber = dataById.cycle_number;
		this.cycleInterval = dataById.cycle_interval;
	}

	private void SetTrigerCondition(string[] cdn)
	{
		if (this.trigerCdn != GameEvent.None)
		{
			if (this.trigerCdn != GameEvent.GameStart)
			{
				if (this.trigerCdn == GameEvent.LMHeroDead)
				{
					this.trigerCdnIntParam1 = int.Parse(cdn[1]);
				}
				else if (this.trigerCdn == GameEvent.BLHeroDead)
				{
					this.trigerCdnIntParam1 = int.Parse(cdn[1]);
				}
				else if (this.trigerCdn == GameEvent.UntiDead)
				{
					this.trigerCndStrParam1 = cdn[1];
				}
				else if (this.trigerCdn == GameEvent.TimeTigger)
				{
					this.trigerCdnFloatParam1 = float.Parse(cdn[1]);
					this.trigerCdnFloatParam2 = float.Parse(cdn[2]);
					this.trigerCdnFloatParam3 = float.Parse(cdn[3]);
				}
			}
		}
	}

	public string GetTag(EntityType type)
	{
		switch (type)
		{
		case EntityType.Hero:
			return "Hero";
		case EntityType.Monster:
			return "Monster";
		case EntityType.Tower:
			return "Building";
		case EntityType.Home:
			return "Home";
		default:
			return "Monster";
		}
	}
}
