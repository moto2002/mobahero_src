using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaClientCom;
using MobaFrame.SkillAction;
using MobaHeros.Pvp;
using MobaHeros.Spawners;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UtilManager
{
	private const int ONLY_ATTACKER = 1;

	private const int SHARE_WITH_LEAGUES = 2;

	private static UtilManager _instance;

	private Dictionary<UtilType, UtilCounter> _allCounters = new Dictionary<UtilType, UtilCounter>();

	private Dictionary<int, HeroKillData> _killDics = new Dictionary<int, HeroKillData>();

	private UtilDataMgr _dataMgr;

	private string FX_GOLD = "Fx_Jinbi";

	public static UtilManager Instance
	{
		get
		{
			if (UtilManager._instance == null)
			{
				UtilManager._instance = new UtilManager();
			}
			return UtilManager._instance;
		}
	}

	public void Init()
	{
		this.Reset();
		this._dataMgr = new UtilDataMgr();
		this._allCounters.Add(UtilType.Exp, new ExpCounter(UtilType.Exp));
		this._allCounters.Add(UtilType.Gold, new GoldCounter(UtilType.Gold));
		this._allCounters.Add(UtilType.Tower, new TowerCounter(UtilType.Tower));
		this._allCounters.Add(UtilType.Skill, new SkillCounter(UtilType.Skill));
		foreach (UtilCounter current in this._allCounters.Values)
		{
			current.InitCounter();
		}
	}

	private void Reset()
	{
		foreach (UtilCounter current in this._allCounters.Values)
		{
			current.Clear();
		}
		this._allCounters.Clear();
		this._killDics.Clear();
	}

	public void Update()
	{
		Dictionary<UtilType, UtilCounter>.Enumerator enumerator = this._allCounters.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<UtilType, UtilCounter> current = enumerator.Current;
			current.Value.Update();
		}
	}

	public UtilCounter GetCounter(UtilType type)
	{
		if (this._allCounters != null && this._allCounters.ContainsKey(type))
		{
			return this._allCounters[type];
		}
		return null;
	}

	public UtilDataMgr GetUtilDataMgr()
	{
		return this._dataMgr;
	}

	public int GetGoldById(int uid)
	{
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			PvpStatisticMgr.HeroData heroData = Singleton<PvpManager>.Instance.StatisticMgr.GetHeroData(uid);
			if (heroData != null)
			{
				return heroData.CurGold;
			}
			return 0;
		}
		else
		{
			GoldCounter goldCounter = this.GetCounter(UtilType.Gold) as GoldCounter;
			if (goldCounter == null)
			{
				return 0;
			}
			GoldValue goldValue = goldCounter.GetValue(uid) as GoldValue;
			if (goldValue == null)
			{
				return 0;
			}
			return goldValue.CurrentGold;
		}
	}

	public int GetEpicMonsterKilling(TeamType team)
	{
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			PvpStatisticMgr.GroupData groupData = Singleton<PvpManager>.Instance.StatisticMgr.GetGroupData((int)team);
			return groupData.TeamEpicMonsterKill;
		}
		if (team == TeamType.LM)
		{
			return AchieveData.EpicMonsterLm;
		}
		if (team == TeamType.BL)
		{
			return AchieveData.EpicMonsterBl;
		}
		return 0;
	}

	public int GetTowerDestroyByTeam(TeamType team)
	{
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			PvpStatisticMgr.GroupData groupData = Singleton<PvpManager>.Instance.StatisticMgr.GetGroupData((int)team);
			return groupData.TeamTowerDestroy;
		}
		if (team == TeamType.LM)
		{
			return AchieveData.TowerDestrouByLm;
		}
		if (team == TeamType.BL)
		{
			return AchieveData.TowerDestroyByBl;
		}
		return 0;
	}

	public int ChangeGoldById(int uid, int changeNum)
	{
		GoldCounter goldCounter = this.GetCounter(UtilType.Gold) as GoldCounter;
		if (goldCounter == null)
		{
			return 0;
		}
		GoldValue goldValue = goldCounter.GetValue(uid) as GoldValue;
		if (goldValue == null)
		{
			ClientLogger.Error("not gold data for this id:" + uid);
			return 0;
		}
		goldValue.ChangeGold(changeNum);
		return goldValue.CurrentGold;
	}

	public void AddDeathGold(Units attacker, Units dead)
	{
		if (attacker == null)
		{
			return;
		}
		if (dead == null)
		{
			return;
		}
		if (LevelManager.IsTestingLevel || Singleton<PvpManager>.Instance.IsInPvp)
		{
			return;
		}
		if (!attacker.isHero)
		{
			return;
		}
		HeroKillData heroKillDataByUnitId = this.GetHeroKillDataByUnitId(dead.unique_id);
		int num = 0;
		float num2 = 0f;
		int num3 = (dead.level <= 1) ? 0 : (dead.level - 1);
		if (dead.isHero)
		{
			BattleConfigData battleConfigData = this._dataMgr.GetUtilDataByType(UtilDataType.Battle_config, SceneInfo.Current.BattleAttrIndex) as BattleConfigData;
			if (battleConfigData == null)
			{
				return;
			}
			int num4 = (heroKillDataByUnitId == null) ? 0 : heroKillDataByUnitId.ContinusKillCount;
			num += this.TryGetExtraGoldForFirstBlood(attacker.unique_id, battleConfigData);
			num += this.TryGetExtraGoldForStopKill(num4, battleConfigData);
			float num5;
			if (num4 > 0)
			{
				num5 = battleConfigData.GetRateByType(BattleConfigData.Rate_Type.Rate_Normal, 0) + battleConfigData.GetRateByType(BattleConfigData.Rate_Type.Rate_Enemy_Lv_Based, 0) * (float)num3 + battleConfigData.GetRateByType(BattleConfigData.Rate_Type.Rate_Enemy_Kill_Based, 0) * (float)(num4 - 1);
			}
			else
			{
				num5 = battleConfigData.GetRateByType(BattleConfigData.Rate_Type.Rate_Normal, 0) + battleConfigData.GetRateByType(BattleConfigData.Rate_Type.Rate_Enemy_Lv_Based, 0) * (float)num3;
				float num6 = battleConfigData.GetRateByType(BattleConfigData.Rate_Type.Rate_DeathRatio, 0) * (float)((heroKillDataByUnitId == null) ? 0 : heroKillDataByUnitId.DeathCount);
				num5 = ((num5 <= num6) ? 0f : (num5 - num6));
			}
			List<Units> list = ListPool.get<Units>();
			dead.GetHeroKillHelper(attacker, list);
			if (list != null && list.Count > 0)
			{
				num2 = num5 * battleConfigData.GetRateByType(BattleConfigData.Rate_Type.Rate_4_Gold, list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] != null)
					{
						this.AddGoldByUnitId(list[i].unique_id, (int)num2);
					}
				}
			}
			ListPool.release<Units>(list);
			num = num + (int)num5 + (int)num2;
			this.AddGoldByUnitId(attacker.unique_id, num);
			if (dead.isVisible)
			{
				dead.JumpGoldFont(num, attacker);
			}
		}
		else if (dead.isMonster || dead.isBuilding)
		{
			UtilMonsterData.UnitReward monsterReward = this.GetMonsterReward(dead.npc_id);
			if (monsterReward == null)
			{
				return;
			}
			this.AddGoldByUnitId(attacker.unique_id, monsterReward.Gold_kill);
			if (monsterReward.Gold_extra > 0)
			{
				IList<Units> mapUnits = MapManager.Instance.GetMapUnits((TeamType)attacker.teamType, global::TargetTag.Hero);
				if (mapUnits != null && mapUnits.Count > 0)
				{
					for (int j = 0; j < mapUnits.Count; j++)
					{
						if (mapUnits[j] != null)
						{
							this.AddGoldByUnitId(mapUnits[j].unique_id, monsterReward.Gold_extra);
						}
					}
				}
			}
			if (dead.isVisible)
			{
				dead.JumpGoldFont(monsterReward.Gold_kill, attacker);
				this.TryPlayGoldEff(attacker, dead, AddMoneyType.Kill);
			}
		}
	}

	private int TryGetExtraGoldForFirstBlood(int inAttackerId, BattleConfigData inBattleConfigDataInst)
	{
		if (StatisticsManager.IsCanGetFirstBloodGold(inAttackerId) && inBattleConfigDataInst != null)
		{
			StatisticsManager.OperationAfterGotFirstBloodGold();
			return (int)inBattleConfigDataInst.GetRateByType(BattleConfigData.Rate_Type.Rate_Bonus, 2);
		}
		return 0;
	}

	private int TryGetExtraGoldForStopKill(int inContinuousKillVal, BattleConfigData inBattleConfigDataInst)
	{
		if (inBattleConfigDataInst != null && inContinuousKillVal >= 3)
		{
			return (int)inBattleConfigDataInst.GetRateByType(BattleConfigData.Rate_Type.Rate_Bonus, 3);
		}
		return 0;
	}

	private HeroKillData GetHeroKillDataByUnitId(int inUnitId)
	{
		HeroKillData result = null;
		if (this._killDics.TryGetValue(inUnitId, out result))
		{
			return result;
		}
		return null;
	}

	private void PlayGoldEff(Units target)
	{
		ActionManager.PlayEffect(this.FX_GOLD, target, null, null, true, string.Empty, null);
	}

	public void TryJumpGoldFont(Units inAttacker, Units inTarget, AddMoneyType inAddType, int inAddOnMoney)
	{
		if (inAttacker == null || inTarget == null)
		{
			return;
		}
		if ((inTarget.isVisible || !inTarget.isLive) && inAttacker.isPlayer && inAddType == AddMoneyType.Kill)
		{
			inTarget.JumpFont("+" + inAddOnMoney + "g", HUDText.goldColor);
		}
	}

	public void TryPlayGoldEff(Units inAttacker, Units inTarget, AddMoneyType inAddType)
	{
		if (inAttacker == null || inTarget == null)
		{
			return;
		}
		if (inTarget.isVisible && inAttacker.isPlayer && inTarget.isMonster && !inTarget.IsMonsterCreep() && inAddType == AddMoneyType.Kill)
		{
			this.PlayGoldEff(inTarget);
		}
	}

	public void TryPlaySelfGoldEff(Units inTarget)
	{
		if (inTarget != null)
		{
			this.PlayGoldEff(inTarget);
		}
	}

	public int GetHerolv(int uid)
	{
		int result = 1;
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			PvpStatisticMgr.HeroData heroData = Singleton<PvpManager>.Instance.StatisticMgr.GetHeroData(uid);
			if (heroData != null)
			{
				result = heroData.CurLv;
			}
		}
		else
		{
			ExpCounter expCounter = this.GetCounter(UtilType.Exp) as ExpCounter;
			if (expCounter != null)
			{
				ExpValue expValue = expCounter.GetValue(uid) as ExpValue;
				if (expValue != null)
				{
					result = expValue.CurLv;
				}
			}
		}
		return result;
	}

	public float GetHeroExpRatio(int uid)
	{
		float result = 0f;
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			PvpStatisticMgr.HeroData heroData = Singleton<PvpManager>.Instance.StatisticMgr.GetHeroData(uid);
			if (heroData != null)
			{
				int exp2LevelUp = UtilExpData.Instance.GetExp2LevelUp(heroData.CurLv);
				if (exp2LevelUp == -1)
				{
					result = 1f;
				}
				else
				{
					result = (float)heroData.CurExp / (float)exp2LevelUp;
				}
			}
		}
		else
		{
			ExpCounter expCounter = this.GetCounter(UtilType.Exp) as ExpCounter;
			if (expCounter != null)
			{
				ExpValue expValue = expCounter.GetValue(uid) as ExpValue;
				if (expValue != null)
				{
					result = expValue.RicherExpRatio;
				}
			}
		}
		return result;
	}

	public void AddExp(Units attacker, Units dead)
	{
		if (attacker == null)
		{
			return;
		}
		if (dead == null)
		{
			return;
		}
		if (LevelManager.IsTestingLevel || Singleton<PvpManager>.Instance.IsInPvp)
		{
			return;
		}
		if (dead.isHero)
		{
			float num = 0f;
			if (!this.GetDeadExpByUnitId(dead.unique_id, dead.level, out num))
			{
				return;
			}
			if (attacker.isHero)
			{
				this.AddExpByUnitId(attacker.unique_id, num);
			}
			List<Units> list = new List<Units>();
			dead.GetHeroKillHelper(attacker, list);
			if (list == null)
			{
				return;
			}
			if (attacker.isHero)
			{
				list.Add(attacker);
			}
			if (list.Count > 0)
			{
				float sharedExpByUnitId = this.GetSharedExpByUnitId(num, list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] != null)
					{
						this.AddExpByUnitId(list[i].unique_id, sharedExpByUnitId);
					}
				}
			}
		}
		else if (dead.isMonster || dead.isBuilding)
		{
			UtilMonsterData.UnitReward monsterReward = this.GetMonsterReward(dead.npc_id);
			if (monsterReward == null)
			{
				return;
			}
			List<Units> list2 = new List<Units>();
			if (monsterReward.Exp_share_type == 2)
			{
				this.GetShareExpRangeUnits(attacker, dead, (float)monsterReward.Exp_share_range, list2);
			}
			if (attacker.isHero)
			{
				list2.Add(attacker);
			}
			if (list2.Count > 0)
			{
				float inExpVal = (float)(monsterReward.Exp_kill / list2.Count);
				for (int j = 0; j < list2.Count; j++)
				{
					if (list2[j] != null)
					{
						this.AddExpByUnitId(list2[j].unique_id, inExpVal);
					}
				}
			}
		}
	}

	public void UpdateKillState(Units attacker, Units dead)
	{
		if (attacker == null || dead == null)
		{
			return;
		}
		this.CheckKillRecord(attacker.unique_id);
		this.CheckKillRecord(dead.unique_id);
		if (attacker.isHero && dead.isHero)
		{
			this._killDics[attacker.unique_id].UpdateState(HeroKillData.KillState.State_Kill_Hero);
			this._killDics[dead.unique_id].UpdateState(HeroKillData.KillState.State_Killed_By_Hero);
		}
		else if (!attacker.isHero && dead.isHero)
		{
			this._killDics[dead.unique_id].UpdateState(HeroKillData.KillState.State_Killed_By_Not_Hero);
		}
	}

	private void CheckKillRecord(int id)
	{
		if (!this._killDics.ContainsKey(id))
		{
			this._killDics.Add(id, new HeroKillData(id));
		}
	}

	private bool GetDeadExpByUnitId(int inUnitId, int inCurLv, out float outDeadExpVal)
	{
		outDeadExpVal = 0f;
		ExpCounter expCounter = this.GetCounter(UtilType.Exp) as ExpCounter;
		if (expCounter == null)
		{
			return false;
		}
		ExpValue expValue = expCounter.GetValue(inUnitId) as ExpValue;
		if (expValue == null)
		{
			return false;
		}
		outDeadExpVal = expValue.GetDeadExp(inCurLv);
		return true;
	}

	private float GetSharedExpByUnitId(float inDeadExp, int inSharedNum)
	{
		if (this._dataMgr == null)
		{
			return 0f;
		}
		BattleConfigData battleConfigData = this._dataMgr.GetUtilDataByType(UtilDataType.Battle_config, SceneInfo.Current.BattleAttrIndex) as BattleConfigData;
		if (battleConfigData == null)
		{
			return 0f;
		}
		return inDeadExp * battleConfigData.GetRateByType(BattleConfigData.Rate_Type.Rate_4_Exp, inSharedNum);
	}

	private UtilMonsterData.UnitReward GetMonsterReward(string inMonsterMainId)
	{
		if (!StringUtils.CheckValid(inMonsterMainId))
		{
			return null;
		}
		if (this._dataMgr == null)
		{
			return null;
		}
		UtilMonsterData utilMonsterData = this._dataMgr.GetUtilDataByType(UtilDataType.Battle_attr_reward, SceneInfo.Current.BattleAttrIndex) as UtilMonsterData;
		if (utilMonsterData == null)
		{
			Debug.LogError("no data for battle_attr_reward");
			return null;
		}
		SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(inMonsterMainId);
		if (monsterMainData == null)
		{
			return null;
		}
		return utilMonsterData.GetReward(monsterMainData.battle_attr_reward);
	}

	private void AddExpByUnitId(int inUnitId, float inExpVal)
	{
		if (inExpVal < 0.1f)
		{
			return;
		}
		ExpCounter expCounter = this.GetCounter(UtilType.Exp) as ExpCounter;
		if (expCounter == null)
		{
			return;
		}
		ExpValue expValue = expCounter.GetValue(inUnitId) as ExpValue;
		if (expValue == null)
		{
			return;
		}
		expValue.AddExp(inExpVal);
	}

	private void AddGoldByUnitId(int inUnitId, int inGoldVal)
	{
		GoldCounter goldCounter = this.GetCounter(UtilType.Gold) as GoldCounter;
		if (goldCounter == null)
		{
			return;
		}
		GoldValue goldValue = goldCounter.GetValue(inUnitId) as GoldValue;
		if (goldValue == null)
		{
			return;
		}
		goldValue.ChangeGold(inGoldVal);
	}

	private void GetShareExpRangeUnits(Units inAttacker, Units inTarget, float range, List<Units> inAllShareExpUnits)
	{
		if (inAllShareExpUnits == null)
		{
			return;
		}
		inAllShareExpUnits.Clear();
		if (inAttacker == null || inTarget == null)
		{
			return;
		}
		IList<Units> mapUnits = MapManager.Instance.GetMapUnits((TeamType)inAttacker.teamType, global::TargetTag.Hero);
		if (mapUnits != null && mapUnits.Count > 0)
		{
			for (int i = 0; i < mapUnits.Count; i++)
			{
				if (!(mapUnits[i] == null))
				{
					if (mapUnits[i].unique_id != inAttacker.unique_id)
					{
						if (mapUnits[i].unique_id != inTarget.unique_id)
						{
							if (UnitFeature.DistanceToPointSqr(mapUnits[i].mTransform.position, inAttacker.mTransform.position) <= range * range)
							{
								inAllShareExpUnits.Add(mapUnits[i]);
							}
						}
					}
				}
			}
		}
	}

	private List<int> GetShareUnits(Units target, float range)
	{
		List<int> list = new List<int>();
		IList<Units> mapUnits = MapManager.Instance.GetMapUnits((TeamType)target.teamType, global::TargetTag.Hero);
		if (mapUnits != null)
		{
			foreach (Units current in mapUnits)
			{
				if (current.unique_id != target.unique_id)
				{
					if (UnitFeature.TargetInDistance(target.mTransform, current.mTransform, range))
					{
						list.Add(current.unique_id);
					}
				}
			}
		}
		return list;
	}
}
