using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros.Pvp;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Model
{
	public static class ModelTools_settlement
	{
		public static SettlementModelData Get_SettlementModelData(this ModelManager mmng)
		{
			SettlementModelData result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_settlement))
			{
				result = mmng.GetData<SettlementModelData>(EModelType.Model_settlement);
			}
			return result;
		}

		public static List<EquipmentInfoData> Get_Settle_Equip(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			return settlementModelData.equipRtnData;
		}

		public static List<HeroInfoData> Get_Settle_Hero(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			return settlementModelData.heroRtnData;
		}

		public static List<DropItemData> Get_Settle_DropItem(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			return settlementModelData.dropRtnData;
		}

		public static PvpTeamInfo Get_Settle_PvpTeamInfo(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			return settlementModelData.pvp_teamInfo;
		}

		public static List<BattlesModel> Get_Settle_BattlesInfo(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			return settlementModelData.pve_battlesInfo;
		}

		public static string Get_Settle_BattleTypeText(this ModelManager mmng)
		{
			string result = string.Empty;
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.m_CurLevel.battle_id);
			if (dataById == null)
			{
				return result;
			}
			switch (dataById.belonged_battletype)
			{
			case 2:
				result = LanguageManager.Instance.GetStringById("BattleSettlement_MatchingMode");
				break;
			case 3:
				result = LanguageManager.Instance.GetStringById("PlayUI_Title_ConfusingCombatants");
				break;
			case 4:
				result = LanguageManager.Instance.GetStringById("PlayUI_Title_Rank");
				break;
			case 5:
				result = LanguageManager.Instance.GetStringById("BattleScene_Name_80007");
				break;
			case 6:
				result = "入门人机对战";
				break;
			case 7:
				result = "一般人机对战";
				break;
			case 8:
				result = "困难人机对战";
				break;
			default:
				result = "魔霸英雄";
				break;
			}
			return result;
		}

		public static string Get_Settle_BattlePlayersText(this ModelManager mmng)
		{
			if (LevelManager.m_CurLevel.Is3V3V3())
			{
				return "3V3V3";
			}
			int hero1_number_cap = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurLevelId).hero1_number_cap;
			if (hero1_number_cap == 0)
			{
				return "?V?";
			}
			return string.Format("{0}V{0}", hero1_number_cap);
		}

		public static string Get_Settle_MapName(this ModelManager mmng)
		{
			string curLevelId = LevelManager.CurLevelId;
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(curLevelId);
			return LanguageManager.Instance.GetStringById(dataById.scene_map_id);
		}

		public static string Get_Settle_Time(this ModelManager mmng)
		{
			if (mmng == null)
			{
				return "N/A";
			}
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			if (settlementModelData == null)
			{
				return "N/A";
			}
			if (settlementModelData.battleUseTime.Equals("N/A") && Singleton<PvpManager>.Instance.IsInPvp)
			{
				return StringUtils.FormatTimeInMinutes((int)GameManager.TotalPlayingSeconds, true);
			}
			return settlementModelData.battleUseTime;
		}

		public static void Set_Settle_Time(this ModelManager mmng, string _secStr)
		{
			if (mmng == null)
			{
				return;
			}
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			if (settlementModelData == null)
			{
				return;
			}
			settlementModelData.battleUseTime = _secStr;
		}

		public static void Set_Settle_DeltaExp(this ModelManager mmng)
		{
			if (mmng == null)
			{
				return;
			}
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			if (settlementModelData == null)
			{
				return;
			}
			if (settlementModelData.dropRtnData == null || settlementModelData.dropRtnData.Count == 0)
			{
				return;
			}
			List<DropItemData> list = (from obj in settlementModelData.dropRtnData
			where obj.itemId == 1 && obj.itemType == 1
			select obj).ToList<DropItemData>();
			settlementModelData.coin_Delta = ((list.Count != 0) ? list[0].itemCount : 0);
			list = (from obj in settlementModelData.dropRtnData
			where obj.itemId == 1 && obj.itemType == 4
			select obj).ToList<DropItemData>();
			settlementModelData.summonerExp_Delta = ((list.Count != 0) ? list[0].itemCount : 0);
			list = (from obj in settlementModelData.dropRtnData
			where obj.itemId == 2 && obj.itemType == 4
			select obj).ToList<DropItemData>();
			settlementModelData.bottleExp_Delta = ((list.Count != 0) ? list[0].itemCount : 0);
		}

		public static void Set_settle_2GetItemsView(this ModelManager mmng)
		{
			if (mmng == null)
			{
				CtrlManager.CloseWindow(WindowID.GetItemsView);
				return;
			}
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			if (settlementModelData == null)
			{
				CtrlManager.CloseWindow(WindowID.GetItemsView);
				return;
			}
			if (settlementModelData.dropRtnData == null || settlementModelData.equipRtnData == null || settlementModelData.heroRtnData == null || (settlementModelData.dropRtnData.Count == 0 && settlementModelData.equipRtnData.Count == 0 && settlementModelData.heroRtnData.Count == 0))
			{
				CtrlManager.CloseWindow(WindowID.GetItemsView);
				return;
			}
			DropItemData dropItemData = settlementModelData.dropRtnData.Find((DropItemData obj) => obj.itemId == 2 && obj.itemType == 1);
			int num = (dropItemData != null) ? dropItemData.itemCount : 0;
			dropItemData = settlementModelData.dropRtnData.Find((DropItemData obj) => obj.itemId == 9 && obj.itemType == 1);
			int num2 = (dropItemData != null) ? dropItemData.itemCount : 0;
			HeroInfoData[] array = settlementModelData.heroRtnData.ToArray();
			DropItemData[] array2 = (from obj in settlementModelData.dropRtnData
			where obj.itemId == 2 && obj.itemType == 3
			select obj).ToArray<DropItemData>();
			EquipmentInfoData[] array3 = settlementModelData.equipRtnData.ToArray();
			DropItemData[] array4 = (from obj in settlementModelData.dropRtnData
			where obj.itemId == 3 && obj.itemType == 3
			select obj).ToArray<DropItemData>();
			DropItemData[] array5 = (from obj in settlementModelData.dropRtnData
			where obj.itemId == 4 && obj.itemType == 3
			select obj).ToArray<DropItemData>();
			DropItemData[] array6 = (from obj in settlementModelData.dropRtnData
			where obj.itemId == 5 && obj.itemType == 3
			select obj).ToArray<DropItemData>();
			DropItemData[] array7 = (from obj in settlementModelData.dropRtnData
			where obj.itemType == 6
			select obj).ToArray<DropItemData>();
			bool flag = false;
			EquipmentInfoData[] array8 = array3;
			for (int i = 0; i < array8.Length; i++)
			{
				EquipmentInfoData equipmentInfoData = array8[i];
				SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(equipmentInfoData.ModelId.ToString());
				if (equipmentInfoData.ModelId == 7777)
				{
					MobaMessageManagerTools.GetItems_Bottle(equipmentInfoData.Count);
				}
				else if (dataById.type == 4)
				{
					MobaMessageManagerTools.GetItems_Rune(equipmentInfoData.ModelId);
				}
				else
				{
					MobaMessageManagerTools.GetItems_GameItem(equipmentInfoData.ModelId.ToString());
				}
			}
			MobaMessageManagerTools.GetItems_Diamond(num);
			MobaMessageManagerTools.GetItems_Caps(num2);
			DropItemData[] array9 = array4;
			for (int j = 0; j < array9.Length; j++)
			{
				DropItemData dropItemData2 = array9[j];
				for (int k = 0; k < settlementModelData.repeatRtnData.Count; k++)
				{
					if (settlementModelData.repeatRtnData[k].itemType == 3 && settlementModelData.repeatRtnData[k].itemId == 3 && settlementModelData.repeatRtnData[k].itemCount == dropItemData2.itemCount)
					{
						MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.HeadPortrait, dropItemData2.itemCount.ToString(), true);
						flag = true;
					}
				}
				if (flag)
				{
					flag = false;
				}
				else
				{
					MobaMessageManagerTools.GetItems_HeadPortrait(dropItemData2.itemCount);
				}
			}
			DropItemData[] array10 = array5;
			for (int l = 0; l < array10.Length; l++)
			{
				DropItemData dropItemData3 = array10[l];
				for (int m = 0; m < settlementModelData.repeatRtnData.Count; m++)
				{
					if (settlementModelData.repeatRtnData[m].itemType == 3 && settlementModelData.repeatRtnData[m].itemId == 4 && settlementModelData.repeatRtnData[m].itemCount == dropItemData3.itemCount)
					{
						MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.PortraitFrame, dropItemData3.itemCount.ToString(), true);
						flag = true;
					}
				}
				if (flag)
				{
					flag = false;
				}
				else
				{
					MobaMessageManagerTools.GetItems_PortraitFrame(dropItemData3.itemCount.ToString());
				}
			}
			HeroInfoData[] array11 = array;
			HeroInfoData item;
			for (int n = 0; n < array11.Length; n++)
			{
				item = array11[n];
				for (int num3 = 0; num3 < settlementModelData.repeatRtnData.Count; num3++)
				{
					if ((long)settlementModelData.repeatRtnData[num3].itemCount == item.HeroId)
					{
						Dictionary<string, SysHeroMainVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysHeroMainVo>();
						string npc_id = typeDicByType.Values.FirstOrDefault((SysHeroMainVo obj) => (long)obj.hero_id == item.HeroId).npc_id;
						MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.Hero, npc_id, true);
						flag = true;
					}
				}
				if (flag)
				{
					flag = false;
				}
				else
				{
					MobaMessageManagerTools.GetItems_Hero(item.ModelId);
				}
			}
			DropItemData[] array12 = array2;
			for (int num4 = 0; num4 < array12.Length; num4++)
			{
				DropItemData dropItemData4 = array12[num4];
				for (int num5 = 0; num5 < settlementModelData.repeatRtnData.Count; num5++)
				{
					if (settlementModelData.repeatRtnData[num5].itemType == 3 && settlementModelData.repeatRtnData[num5].itemId == 2 && settlementModelData.repeatRtnData[num5].itemCount == dropItemData4.itemCount)
					{
						MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.HeroSkin, dropItemData4.itemCount.ToString(), true);
						flag = true;
					}
				}
				if (flag)
				{
					flag = false;
				}
				else
				{
					MobaMessageManagerTools.GetItems_HeroSkin(dropItemData4.itemCount);
				}
			}
			DropItemData[] array13 = array6;
			for (int num6 = 0; num6 < array13.Length; num6++)
			{
				DropItemData dropItemData5 = array13[num6];
				for (int num7 = 0; num7 < settlementModelData.repeatRtnData.Count; num7++)
				{
					if (settlementModelData.repeatRtnData[num7].itemType == 3 && settlementModelData.repeatRtnData[num7].itemId == 5 && settlementModelData.repeatRtnData[num7].itemCount == dropItemData5.itemCount)
					{
						MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.Coupon, dropItemData5.itemCount.ToString(), true);
						flag = true;
					}
				}
				if (flag)
				{
					flag = false;
				}
				else
				{
					MobaMessageManagerTools.GetItems_Coupon(dropItemData5.itemCount.ToString());
				}
			}
			DropItemData[] array14 = array7;
			for (int num8 = 0; num8 < array14.Length; num8++)
			{
				DropItemData dropItemData6 = array14[num8];
				MobaMessageManagerTools.GetItems_GameBuff(dropItemData6.itemId);
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetDoubleCard, null, new object[0]);
			}
			Singleton<GetItemsView>.Instance.Play();
		}

		public static void Set_Settle_SummonerExpRecord(this ModelManager mmng)
		{
			long exp = ModelManager.Instance.Get_userData_X().Exp;
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			settlementModelData.summonerExpRecord_level = CharacterDataMgr.instance.GetUserLevel(exp);
			settlementModelData.summonerExpRecord_curExp = CharacterDataMgr.instance.GetUserCurrentExp(exp);
			settlementModelData.summonerExp_curLevelExpRequired = CharacterDataMgr.instance.GetUserNextLevelExp(exp);
		}

		public static int Get_Settle_SummonerExpCurRequired(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			return settlementModelData.summonerExp_curLevelExpRequired;
		}

		public static int Get_Settle_SummonerExpLevel(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			return settlementModelData.summonerExpRecord_level;
		}

		public static int Get_Settle_SummonerExpCurrent(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			return settlementModelData.summonerExpRecord_curExp;
		}

		public static int Get_Settle_SummonerExpDelta(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			return settlementModelData.summonerExp_Delta;
		}

		public static void Set_Settle_BottleExpRecord(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			settlementModelData.bottleExpRecord_level = mmng.Get_BottleData_Level();
			settlementModelData.bottleExpRecord_curExp = (int)mmng.Get_BottleData_Exp();
		}

		public static int Get_Settle_BottleExpLevel(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			return settlementModelData.bottleExpRecord_level;
		}

		public static int Get_Settle_BottleExpCurRequired(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			int level = settlementModelData.bottleExpRecord_level;
			List<object> source = BaseDataMgr.instance.GetDicByType<SysMagicbottleExpVo>().Values.ToList<object>();
			SysMagicbottleExpVo sysMagicbottleExpVo = (SysMagicbottleExpVo)(from obj in source
			where ((SysMagicbottleExpVo)obj).levelRange <= level
			select obj).LastOrDefault<object>();
			if (sysMagicbottleExpVo != null)
			{
				return sysMagicbottleExpVo.exp;
			}
			return 100;
		}

		public static int Get_Settle_BottleExpCurrent(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			return settlementModelData.bottleExpRecord_curExp;
		}

		public static int Get_Settle_BottleExpDelta(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			return settlementModelData.bottleExp_Delta;
		}

		public static SysHeroAttainmentLevelVo Get_ProficiencyLevel(this ModelManager mmng, int exp)
		{
			List<SysHeroAttainmentLevelVo> source = BaseDataMgr.instance.GetDicByType<SysHeroAttainmentLevelVo>().Values.OfType<SysHeroAttainmentLevelVo>().ToList<SysHeroAttainmentLevelVo>();
			return (from obj in source
			where obj.skilled_scores <= exp
			select obj).LastOrDefault<SysHeroAttainmentLevelVo>();
		}

		public static SysHeroAttainmentLevelVo Get_ProficiencyNextLevel(this ModelManager mmng, int exp)
		{
			List<SysHeroAttainmentLevelVo> source = BaseDataMgr.instance.GetDicByType<SysHeroAttainmentLevelVo>().Values.OfType<SysHeroAttainmentLevelVo>().ToList<SysHeroAttainmentLevelVo>();
			return (from obj in source
			where obj.skilled_scores > exp
			select obj).FirstOrDefault<SysHeroAttainmentLevelVo>();
		}

		public static void Set_Settle_Proficiency(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			if (settlementModelData == null)
			{
				ClientLogger.Error("Settlement: Get Self Null");
				return;
			}
			PvpTeamInfo pvp_teamInfo = settlementModelData.pvp_teamInfo;
			if (pvp_teamInfo == null)
			{
				ClientLogger.Error("Settlement: Get PvpTeamInfo Null");
				settlementModelData.proficiencyExpRecord_curExp = 0;
				settlementModelData.proficiencyExpRecord_Delta = 0;
				settlementModelData.levelScore = 5;
				return;
			}
			Units player = PlayerControlMgr.Instance.GetPlayer();
			if (mmng.Get_GetMyAchievementData_X() == null)
			{
				ClientLogger.Error("Settlement: Get Achievement Data Null");
				settlementModelData.proficiencyExpRecord_curExp = 0;
				settlementModelData.proficiencyExpRecord_Delta = 0;
				settlementModelData.levelScore = 5;
				return;
			}
			List<KdaMyHeroData> myHero = ModelManager.Instance.Get_GetMyAchievementData_X().myHero;
			if (myHero == null)
			{
				settlementModelData.proficiencyExpRecord_curExp = 0;
			}
			else
			{
				foreach (KdaMyHeroData current in myHero)
				{
					if (player != null && current.herouseddata.heroid == player.npc_id)
					{
						settlementModelData.proficiencyExpRecord_curExp = current.herouseddata.useinfo;
					}
				}
			}
			string userId = mmng.Get_userData_X().UserId;
			PlayerCounter playerCounter = null;
			if (!pvp_teamInfo.unitCounters.TryGetValue(userId, out playerCounter))
			{
				return;
			}
			int num = playerCounter.deadCount;
			num = ((num != 0) ? num : 1);
			float num2 = (float)(playerCounter.killHoreCount + playerCounter.helpKillHoreCount) / (float)num * 3f;
			string curLevelId = LevelManager.CurLevelId;
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(curLevelId);
			SysHeroProficiencyPointsVo sysHeroProficiencyPointsVo = mmng.Get_ProficiencyLevel(num2, curLevelId);
			if (sysHeroProficiencyPointsVo == null || dataById == null)
			{
				settlementModelData.proficiencyExpRecord_Delta = 0;
				ClientLogger.Error(string.Concat(new object[]
				{
					"Settlement: Can't find Proficiency Item. KDA:",
					num2,
					" BattleId:",
					curLevelId
				}));
				return;
			}
			SysBattleSceneVo dataById2 = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurBattleId);
			bool flag = LevelManager.m_CurLevel.IsFightWithRobot() || LevelManager.m_CurLevel.IsBattleNewbieGuide() || mmng.Get_Settle_IsSelfDefine();
			if (dataById.hero_proficiency_points == 1 && sysHeroProficiencyPointsVo.type.ToString() == curLevelId && GameManager.IsVictory.HasValue && !flag)
			{
				settlementModelData.proficiencyExpRecord_Delta = ((!GameManager.IsVictory.Value) ? sysHeroProficiencyPointsVo.failure_bonus_point : sysHeroProficiencyPointsVo.victory_bonus_point);
			}
			else
			{
				settlementModelData.proficiencyExpRecord_Delta = 0;
			}
			string evaluate = sysHeroProficiencyPointsVo.evaluate;
			switch (evaluate)
			{
			case "D":
				settlementModelData.levelScore = 1;
				return;
			case "C":
				settlementModelData.levelScore = 2;
				return;
			case "B":
				settlementModelData.levelScore = 3;
				return;
			case "A":
				settlementModelData.levelScore = 4;
				return;
			case "S":
				settlementModelData.levelScore = 5;
				return;
			}
			settlementModelData.levelScore = 0;
		}

		public static SysHeroProficiencyPointsVo Get_ProficiencyLevel(this ModelManager mmng, float _kda, string _battleId)
		{
			List<SysHeroProficiencyPointsVo> source = BaseDataMgr.instance.GetDicByType<SysHeroProficiencyPointsVo>().Values.OfType<SysHeroProficiencyPointsVo>().ToList<SysHeroProficiencyPointsVo>();
			SysHeroProficiencyPointsVo sysHeroProficiencyPointsVo = (from obj in source
			where obj.battle_damage <= _kda && obj.type.ToString() == _battleId
			select obj).LastOrDefault<SysHeroProficiencyPointsVo>();
			if (sysHeroProficiencyPointsVo == null)
			{
				sysHeroProficiencyPointsVo = (from obj1 in source
				where obj1.battle_damage <= _kda
				select obj1).LastOrDefault<SysHeroProficiencyPointsVo>();
			}
			return sysHeroProficiencyPointsVo;
		}

		public static char Get_Settle_LevelScore(this ModelManager mmng)
		{
			switch (mmng.Get_SettlementModelData().levelScore)
			{
			case 1:
				return 'D';
			case 2:
				return 'C';
			case 3:
				return 'B';
			case 4:
				return 'A';
			case 5:
				return 'S';
			default:
				return 'X';
			}
		}

		public static int Get_Settle_ProficiencyExpCurrent(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			return settlementModelData.proficiencyExpRecord_curExp;
		}

		public static int Get_Settle_ProficiencyExpDelta(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			return settlementModelData.proficiencyExpRecord_Delta;
		}

		public static string Get_Settle_MvpUserId(this ModelManager mmng)
		{
			PvpTeamInfo pvpTeamInfo = mmng.Get_Settle_PvpTeamInfo();
			return (pvpTeamInfo != null) ? pvpTeamInfo.mvpuserid : string.Empty;
		}

		public static bool Get_Settle_ImMvp(this ModelManager mmng)
		{
			string value = ModelManager.Instance.Get_userData_filed_X("UserId");
			string text = mmng.Get_Settle_MvpUserId();
			return !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(value) && text.Equals(value);
		}

		public static bool Get_Settle_IsSelfDefine(this ModelManager mmng)
		{
			PvpTeamInfo pvpTeamInfo = mmng.Get_Settle_PvpTeamInfo();
			return pvpTeamInfo != null && pvpTeamInfo.isSelfDefine;
		}

		public static int Get_Settle_CoinDelta(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			return settlementModelData.coin_Delta;
		}

		public static bool Get_Settle_IsDataReady(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			if (settlementModelData.isGetMsg)
			{
				settlementModelData.isGetMsg = false;
				return true;
			}
			return false;
		}

		public static void Get_Settle_SavingData(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			DropItemData dropItemData = settlementModelData.dropRtnData.Find((DropItemData obj) => obj.itemId == 2 && obj.itemType == 1);
			int num = (dropItemData != null) ? dropItemData.itemCount : 0;
			dropItemData = settlementModelData.dropRtnData.Find((DropItemData obj) => obj.itemId == 9 && obj.itemType == 1);
			int num2 = (dropItemData != null) ? dropItemData.itemCount : 0;
			HeroInfoData[] array = settlementModelData.heroRtnData.ToArray();
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (!ModelManager.Instance.Get_heroInfo_list_X().Contains(array[i]))
					{
						ModelManager.Instance.Get_heroInfo_list_X().Add(array[i]);
					}
				}
				CharacterDataMgr.instance.UpdateHerosData();
			}
			EquipmentInfoData[] array2 = settlementModelData.equipRtnData.ToArray();
			DropItemData[] array3 = (from obj in settlementModelData.dropRtnData
			where obj.itemId == 3 && obj.itemType == 3
			select obj).ToArray<DropItemData>();
			DropItemData[] array4 = (from obj in settlementModelData.dropRtnData
			where obj.itemId == 4 && obj.itemType == 3
			select obj).ToArray<DropItemData>();
			DropItemData[] array5 = (from obj in settlementModelData.dropRtnData
			where obj.itemId == 5 && obj.itemType == 3
			select obj).ToArray<DropItemData>();
			if (settlementModelData.coin_Delta > 0)
			{
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetCurrencyCount, null, new object[]
				{
					1
				});
			}
			if (settlementModelData.summonerExp_Delta > 0)
			{
				mmng.Get_userData_X().Exp += (long)settlementModelData.summonerExp_Delta;
				CharacterDataMgr.instance.SaveNowUserLevel(mmng.Get_userData_X().Exp);
			}
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetMagicBottleInfo, null, new object[0]);
			if (num > 0)
			{
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetCurrencyCount, null, new object[]
				{
					2
				});
			}
			if (num2 > 0)
			{
				mmng.Get_userData_X().SmallCap += num2;
			}
			EquipmentInfoData[] array6 = array2;
			EquipmentInfoData runeItem;
			for (int j = 0; j < array6.Length; j++)
			{
				runeItem = array6[j];
				if (mmng.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.EquipmentId == runeItem.EquipmentId) == null)
				{
					mmng.Get_equipmentList_X().Add(runeItem);
				}
				else
				{
					mmng.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.EquipmentId == runeItem.EquipmentId).Count += runeItem.Count;
				}
			}
			DropItemData[] array7 = array3;
			for (int k = 0; k < array7.Length; k++)
			{
				DropItemData dropItemData2 = array7[k];
				mmng.GetNewAvatar("3", dropItemData2.itemCount.ToString());
			}
			DropItemData[] array8 = array4;
			for (int l = 0; l < array8.Length; l++)
			{
				DropItemData dropItemData3 = array8[l];
				mmng.GetNewAvatar("4", dropItemData3.itemCount.ToString());
			}
			DropItemData[] array9 = array5;
			for (int m = 0; m < array9.Length; m++)
			{
				DropItemData dropItemData4 = array9[m];
				mmng.GetNewCoupon(dropItemData4.itemCount.ToString());
			}
			if (LevelManager.m_CurLevel.IsLeague(LevelManager.CurBattleId))
			{
				EquipmentInfoData equipmentInfoData = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId == 8000);
				if (equipmentInfoData == null)
				{
					return;
				}
				if (equipmentInfoData.Count <= 1)
				{
					ModelManager.Instance.Get_equipmentList_X().Remove(equipmentInfoData);
				}
				else
				{
					equipmentInfoData.Count--;
				}
			}
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetTaskList, null, new object[]
			{
				0
			});
			SendMsgManager.Instance.SendMsg(MobaGameCode.ShowDailyTask, null, new object[0]);
			if (GameManager.IsVictory.Value)
			{
				ModelManager.Instance.RemoveWinCard();
			}
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurBattleId);
			bool flag = LevelManager.m_CurLevel.IsFightWithRobot() || LevelManager.m_CurLevel.IsBattleNewbieGuide() || mmng.Get_Settle_IsSelfDefine();
			ModelManager.Instance.Get_GetMyAchievementData_X().HaveFight = !flag;
			if (ModelManager.Instance.Get_GetMyAchievementData_X().HaveFight)
			{
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetKdaMyHeroData, null, new object[0]);
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetHomeTotalRecord, null, new object[0]);
			}
		}

		public static void Clear_SettlementData(this ModelManager mmng)
		{
			SettlementModelData settlementModelData = mmng.Get_SettlementModelData();
			settlementModelData.equipRtnData.Clear();
			settlementModelData.heroRtnData.Clear();
			settlementModelData.dropRtnData.Clear();
			settlementModelData.summonerExpRecord_level = 0;
			settlementModelData.summonerExpRecord_curExp = 0;
			settlementModelData.summonerExp_Delta = 0;
			settlementModelData.summonerExp_curLevelExpRequired = 0;
			settlementModelData.bottleExpRecord_level = 0;
			settlementModelData.bottleExpRecord_curExp = 0;
			settlementModelData.bottleExp_Delta = 0;
			settlementModelData.levelScore = 0;
			settlementModelData.proficiencyExpRecord_curExp = 0;
			settlementModelData.proficiencyExpRecord_Delta = 0;
			settlementModelData.coin_Delta = 0;
			settlementModelData.pvp_teamInfo = null;
			settlementModelData.pve_battlesInfo.Clear();
			settlementModelData.isGetMsg = false;
		}
	}
}
