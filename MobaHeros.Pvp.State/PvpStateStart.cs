using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using Common;
using ExitGames.Client.Photon;
using GameLogin.State;
using MobaClient;
using MobaClientCom;
using MobaFrame.SkillAction;
using MobaHeros.Spawners;
using MobaProtocol;
using MobaProtocol.Data;
using Newbie;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaHeros.Pvp.State
{
	public class PvpStateStart : PvpStateBase
	{
		public enum StopSkillType
		{
			StopSkillType_Move,
			StopSkillType_Passive,
			StopSkillType_Skill
		}

		private const string LogTagUnits = "pvp.unit";

		private const int ITEMTYPE_TOWER = 2;

		private const int ITEMTYPE_BOSS = 7;

		private int _dropTime = 2;

		private readonly CoroutineManager _coroutineManager = new CoroutineManager();

		private Task _task;

		private float lastRotateTime;

		private long lastTimeUpdatePlayerMoveStrategy;

		private readonly long sSecondsUpdatePlayerMoveStrategy = 1L;

		private long lastTimeCheckNetworkStatus;

		private PvpStatisticMgr StatisticMgr
		{
			get
			{
				return Singleton<PvpManager>.Instance.StatisticMgr;
			}
		}

		public PvpStateStart(PvpStateCode state = PvpStateCode.PvpStart) : base(state)
		{
		}

		protected virtual void RegMsgs(PvpStateBase.MsgRegFn func)
		{
			func(PvpCode.C2P_QueryInFightInfo, new MobaMessageFunc(this.P2C_QueryInFightInfo));
			func(PvpCode.P2C_QueryUnit, new MobaMessageFunc(this.P2C_QueryUnit));
			func(PvpCode.P2C_SetSceneValue, new MobaMessageFunc(this.P2C_SetSceneValue));
			func(PvpCode.P2C_AddBuffExt, new MobaMessageFunc(this.P2C_AddBuffExt));
			func(PvpCode.C2P_NewbieInBattle, new MobaMessageFunc(this.P2C_NewbieInBattle));
			func(PvpCode.P2C_AddExp, new MobaMessageFunc(this.P2C_AddExp));
			func(PvpCode.P2C_AddMoney, new MobaMessageFunc(this.P2C_AddMoney));
			func(PvpCode.P2C_KillAddMoney, new MobaMessageFunc(this.P2C_KillAddMoney));
			func(PvpCode.P2C_KillHore, new MobaMessageFunc(this.P2C_KillHore));
			func(PvpCode.P2C_CreateHero, new MobaMessageFunc(this.P2C_CreateHero));
			func(PvpCode.P2C_LastKillMonster, new MobaMessageFunc(this.P2C_LastKillMonster));
			func(PvpCode.P2C_ActiveUnit, new MobaMessageFunc(this.P2C_ActiveUnit));
			func(PvpCode.C2P_GetMapItem, new MobaMessageFunc(this.P2C_GetMapItem));
			func(PvpCode.P2C_ReliveHero, new MobaMessageFunc(this.P2C_ReliveHero));
			func(PvpCode.P2C_CreateMapItem, new MobaMessageFunc(this.P2C_CreateMapItem));
			func(PvpCode.P2C_CreateUnits, new MobaMessageFunc(this.P2C_CreateUnits));
			func(PvpCode.C2P_Kill, new MobaMessageFunc(this.P2C_Kill));
			func(PvpCode.P2C_MonsterPseudoDeath, new MobaMessageFunc(this.P2C_MonsterPseudoDeath));
			func(PvpCode.P2C_NotifySpawnSuperSoldier, new MobaMessageFunc(this.P2C_NotifySpawnSuperSoldier));
			func(PvpCode.P2C_NotifyMonsterCreepAiStatus, new MobaMessageFunc(this.P2C_NotifyMonsterCreepAiStatus));
			func(PvpCode.P2C_RemovePlayerControlUnit, new MobaMessageFunc(this.P2C_RemovePlayerControlUnit));
			func(PvpCode.P2C_AddPlayerControlUnit, new MobaMessageFunc(this.P2C_AddPlayerControlUnit));
			func(PvpCode.C2P_UnitsSnap, new MobaMessageFunc(this.P2C_UnitsSnap));
			func(PvpCode.P2C_UnitsVisiableChanged, new MobaMessageFunc(this.P2C_UnitsVisiableChanged));
			func(PvpCode.P2C_RotatoTo, new MobaMessageFunc(this.P2C_RotateTo));
			func(PvpCode.C2P_FlashTo, new MobaMessageFunc(this.P2C_FlashTo));
			func(PvpCode.C2P_ReadySkill, new MobaMessageFunc(this.P2C_ReadySkill));
			func(PvpCode.C2P_ReadySkillCheck, new MobaMessageFunc(this.P2C_ReadySkillCheck));
			func(PvpCode.C2P_StartSkill, new MobaMessageFunc(this.P2C_StartSkill));
			func(PvpCode.C2P_HitSkill, new MobaMessageFunc(this.P2C_HitSkill));
			func(PvpCode.C2P_EndSkill, new MobaMessageFunc(this.P2C_EndSkill));
			func(PvpCode.C2P_StopSkill, new MobaMessageFunc(this.P2C_StopSkill));
			func(PvpCode.C2P_DoSkill, new MobaMessageFunc(this.P2C_DoSkill));
			func(PvpCode.C2P_JumpFont, new MobaMessageFunc(this.P2C_JumpFont));
			func(PvpCode.C2P_DataChange, new MobaMessageFunc(this.P2C_DataChange));
			func(PvpCode.C2P_Wound, new MobaMessageFunc(this.P2C_Wound));
			func(PvpCode.C2P_DataUpdate, new MobaMessageFunc(this.P2C_DataUpdate));
			func(PvpCode.C2P_AddBuff, new MobaMessageFunc(this.P2C_AddBuff));
			func(PvpCode.C2P_DoBuff, new MobaMessageFunc(this.P2C_DoBuff));
			func(PvpCode.C2P_RemoveBuff, new MobaMessageFunc(this.P2C_RemoveBuff));
			func(PvpCode.C2P_AddHighEffect, new MobaMessageFunc(this.P2C_AddHighEffect));
			func(PvpCode.C2P_DoHighEffect, new MobaMessageFunc(this.P2C_DoHighEffect));
			func(PvpCode.C2P_RemoveHighEffect, new MobaMessageFunc(this.P2C_RemoveHighEffect));
			func(PvpCode.C2P_UpSkillLevel, new MobaMessageFunc(this.P2C_UpSkillLevel));
			func(PvpCode.P2C_Rebirth, new MobaMessageFunc(this.P2C_ReBirth));
			func(PvpCode.P2C_RestoreData, new MobaMessageFunc(this.P2C_Restore));
			func(PvpCode.P2C_PlayerReconnect, new MobaMessageFunc(this.P2C_PlayerReconnect));
			func(PvpCode.P2C_PlayerOutline, new MobaMessageFunc(this.P2C_PlayerOutline));
			func(PvpCode.P2C_AIDebugInfo, new MobaMessageFunc(this.P2C_AIDebugInfo));
			func(PvpCode.C2P_NotifyTeamPos, new MobaMessageFunc(this.P2C_NotifyTeamPos));
			func(PvpCode.C2P_NotifyTeamTarget, new MobaMessageFunc(this.P2C_NotifyTeamTarget));
			func(PvpCode.P2C_LastTeamKillMonster, new MobaMessageFunc(this.P2C_LastTeamKillMonster));
			func(PvpCode.P2C_FrameSync, new MobaMessageFunc(this.OnFrameSync));
			func(PvpCode.C2P_GMCheat, new MobaMessageFunc(this.OnGMCheat));
			func(PvpCode.P2C_DebugCollider, new MobaMessageFunc(this.P2C_DebugCollider));
			func(PvpCode.C2P_BuyItem, new MobaMessageFunc(this.P2C_BuyItem));
			func(PvpCode.C2P_SellItem, new MobaMessageFunc(this.P2C_SellItem));
			func(PvpCode.P2C_RemoveItem, new MobaMessageFunc(this.P2C_RemoveItem));
			func(PvpCode.P2C_AddItem, new MobaMessageFunc(this.P2C_AddItem));
			func(PvpCode.P2C_UpdateItem, new MobaMessageFunc(this.P2C_UpdateItem));
			func(PvpCode.P2C_SynSkillInfo, new MobaMessageFunc(this.P2C_SynSkillInfo));
			func(PvpCode.P2C_StartSkillCD, new MobaMessageFunc(this.P2C_StartSkillInfo));
			func(PvpCode.P2C_TipMessage, new MobaMessageFunc(this.P2C_TipMessage));
		}

		protected override void RegistCallbacks()
		{
			this.RegMsgs(new PvpStateBase.MsgRegFn(MobaMessageManager.RegistMessage));
			MobaMessageManager.RegistMessage((ClientMsg)21008, new MobaMessageFunc(this.OnLoadViewComplete));
		}

		protected override void UnregistCallbacks()
		{
			this.RegMsgs(new PvpStateBase.MsgRegFn(MobaMessageManager.UnRegistMessage));
			MobaMessageManager.UnRegistMessage((ClientMsg)21008, new MobaMessageFunc(this.OnLoadViewComplete));
		}

		private void OnLoadViewComplete(MobaMessage msg)
		{
			PvpStateBase.LogState("===>ClientV2C.LoadView_complete");
			this._coroutineManager.StartCoroutine(this.CheckGameAbandon_Coroutine(), true);
		}

		[DebuggerHidden]
		private IEnumerator CheckGameAbandon_Coroutine()
		{
			PvpStateStart.<CheckGameAbandon_Coroutine>c__Iterator4B <CheckGameAbandon_Coroutine>c__Iterator4B = new PvpStateStart.<CheckGameAbandon_Coroutine>c__Iterator4B();
			<CheckGameAbandon_Coroutine>c__Iterator4B.<>f__this = this;
			return <CheckGameAbandon_Coroutine>c__Iterator4B;
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			MobaMessageManager.ExecuteMsg(ClientC2C.PvpStartGame, null, 0f);
			Singleton<MenuView>.Instance.OnPvpStartGame();
			this.ResumeGame();
			ModelManager.Instance.Apply_SettingDataInBattle();
			UnitsSnapReporter.Instance.StartCheckServerTime();
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				PvpObserveMgr.BeginObserve();
				this._coroutineManager.StartCoroutine(this.RefreshCamera(), true);
			}
			if (GlobalSettings.FogMode >= 2)
			{
				FOWSystem.Instance.enableFog(!Singleton<PvpManager>.Instance.IsObserver);
			}
			else if (GlobalSettings.FogMode == 1)
			{
				FogMgr.Instance.enableFog(!Singleton<PvpManager>.Instance.IsObserver);
			}
			if (AutoTestController.UseAI)
			{
				if (Cheat.Instance == null)
				{
					GlobalObject.Instance.gameObject.AddComponent<Cheat>();
				}
				Cheat.Instance.Execute("startai", false);
			}
		}

		protected override void OnExit()
		{
			base.OnExit();
			this._coroutineManager.StopAllCoroutine();
		}

		[DebuggerHidden]
		private IEnumerator RefreshCamera()
		{
			return new PvpStateStart.<RefreshCamera>c__Iterator4C();
		}

		private void ConfirmQuiting()
		{
			CtrlManager.ShowMsgBox("游戏结束", "游戏已经结束，戳确认退出", delegate
			{
				PvpStateManager.Instance.ChangeState(new PvpStateHome());
			}, PopViewType.PopOneButton, "确定", "取消", null);
		}

		protected override void OnDisconnectServer(MobaPeerType type)
		{
			base.OnDisconnectServer(type);
			if (GameManager.Instance.ReplayController.IsReplaying)
			{
				return;
			}
			if (type == MobaPeerType.C2PvpServer && NewbieManager.Instance.IsInNewbiePhase(ENewbiePhaseType.ElementaryBattleOneOne))
			{
				NewbieManager.Instance.StopAllVoiceHint();
				this.ConfirmQuiting();
				return;
			}
			if (type == MobaPeerType.C2PvpServer)
			{
				this.PauseGame();
				PvpStateManager.Instance.ChangeState(new PvpStateRecover(PvpStateCode.PvpStart));
			}
		}

		private void P2C_PlayerReconnect(MobaMessage msg)
		{
			P2CPlayerOutline probufMsg = msg.GetProbufMsg<P2CPlayerOutline>();
			int newUid = probufMsg.newUid;
			if (Singleton<PvpManager>.Instance.IsOurPlayer(newUid))
			{
				UIMessageBox.ShowMessage("玩家" + Singleton<PvpManager>.Instance.GetSummonerName(newUid) + "重连成功", 1.5f, 0);
				Units player = PlayerControlMgr.Instance.GetPlayer();
				if (player != null && newUid != player.unique_id)
				{
					AudioMgr.Play(1011, false);
				}
			}
		}

		private void P2C_PlayerOutline(MobaMessage msg)
		{
			P2CPlayerOutline probufMsg = msg.GetProbufMsg<P2CPlayerOutline>();
			int newUid = probufMsg.newUid;
			if (Singleton<PvpManager>.Instance.IsOurPlayer(newUid))
			{
				UIMessageBox.ShowMessage("玩家" + Singleton<PvpManager>.Instance.GetSummonerName(newUid) + "已经断线", 1.5f, 0);
				Units player = PlayerControlMgr.Instance.GetPlayer();
				if (player != null && newUid != player.unique_id)
				{
					AudioMgr.Play(1010, false);
				}
			}
		}

		private void P2C_ReliveHero(MobaMessage msg)
		{
			P2CReliveHero probufMsg = msg.GetProbufMsg<P2CReliveHero>();
			int unitId = probufMsg.unitId;
			PvpManager.On_ReliveHero(unitId);
			Units units = MapManager.Instance.TryFetchRecycledUnit(unitId);
			if (!units)
			{
				units = MapManager.Instance.GetUnit(unitId);
				if (!units)
				{
					ClientLogger.Error("P2C_ReliveHero: cannot found hero #" + unitId);
					return;
				}
			}
			SpawnUtility spawnUtility = GameManager.Instance.Spawner.GetSpawnUtility();
			spawnUtility.RespawnPvpHero(units);
		}

		private void P2C_CreateMapItem(MobaMessage msg)
		{
			MapItemInfo probufMsg = msg.GetProbufMsg<MapItemInfo>();
			if (probufMsg == null)
			{
				return;
			}
			UnitControlType unitControlType = UnitControlType.PvpAIControl;
			PvpProtocolTools.CreateMapItem(probufMsg, unitControlType);
		}

		private void P2C_CreateHero(MobaMessage msg)
		{
			try
			{
				UnitInfo probufMsg = msg.GetProbufMsg<UnitInfo>();
				Units units = PvpProtocolTools.CreateHeroByUnitInfo(probufMsg);
				if (units != null)
				{
					units.UpdateVisible();
				}
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
			}
		}

		private void P2C_NewbieInBattle(MobaMessage msg)
		{
			NewbieInBattleData probufMsg = msg.GetProbufMsg<NewbieInBattleData>();
			if (probufMsg.msgType == 3)
			{
				NewbieRespSpawnHeroData inData = SerializeHelper.Deserialize<NewbieRespSpawnHeroData>(probufMsg.msgBody);
				NewbieManager.Instance.HandleNewbieRespSpawnHero(inData);
			}
			else if (probufMsg.msgType == 5)
			{
				NewbieMoveToStepData inData2 = SerializeHelper.Deserialize<NewbieMoveToStepData>(probufMsg.msgBody);
				NewbieManager.Instance.HandleNewbieMoveToStep(inData2);
			}
		}

		private void P2C_SetSceneValue(MobaMessage msg)
		{
			P2CSetSceneValue probufMsg = msg.GetProbufMsg<P2CSetSceneValue>();
			string key = probufMsg.key;
			string val = probufMsg.val;
			Dictionary<string, string> states = new Dictionary<string, string>
			{
				{
					key,
					val
				}
			};
			PvpProtocolTools.SyncSceneState(states);
		}

		private void P2C_AddExp(MobaMessage msg)
		{
			P2CAddExp probufMsg = msg.GetProbufMsg<P2CAddExp>();
			int unitId = probufMsg.unitId;
			int add = probufMsg.add;
			int currExp = probufMsg.currExp;
			int level = probufMsg.level;
			PvpStatisticMgr.HeroData heroData = this.StatisticMgr.GetHeroData(unitId);
			int num = level - heroData.CurLv;
			heroData.CurLv = level;
			heroData.CurExp = currExp;
			Units unit = MapManager.Instance.GetUnit(unitId);
			if (unit != null)
			{
				unit.level = level;
			}
			Units player = PlayerControlMgr.Instance.GetPlayer();
			if (player != null && num > 0 && unitId == player.unique_id)
			{
				unit.playVoice("onlevelup");
				unit.StartLevelEffect();
				unit.skillManager.SkillPointsLeft = unit.level - Singleton<SkillView>.Instance.usedSkillPoint;
				Singleton<SkillView>.Instance.SetSkillPointLeft(unit.skillManager.SkillPointsLeft);
				BattleAttrManager.Instance.UpdateSkillView();
			}
		}

		private void P2C_AddMoney(MobaMessage msg)
		{
			P2CAddMoney probufMsg = msg.GetProbufMsg<P2CAddMoney>();
			int unitId = probufMsg.unitId;
			int add = probufMsg.add;
			int currMoney = probufMsg.currMoney;
			byte addType = probufMsg.addType;
			int deadId = probufMsg.deadId;
			PvpStatisticMgr.HeroData heroData = this.StatisticMgr.GetHeroData(unitId);
			heroData.CurGold = currMoney;
			if (add > 0)
			{
				heroData.TotalGold += add;
			}
			Units unit = MapManager.Instance.GetUnit(unitId);
			Units units = null;
			if (deadId != 0)
			{
				units = MapManager.Instance.GetUnit(deadId);
			}
			if (addType == 1 || addType == 8)
			{
				UtilManager.Instance.TryJumpGoldFont(PlayerControlMgr.Instance.GetPlayer(), PlayerControlMgr.Instance.GetPlayer(), AddMoneyType.Kill, add);
				UtilManager.Instance.TryPlaySelfGoldEff(PlayerControlMgr.Instance.GetPlayer());
			}
			else if (addType == 4)
			{
				UtilManager.Instance.TryJumpGoldFont(PlayerControlMgr.Instance.GetPlayer(), PlayerControlMgr.Instance.GetPlayer(), AddMoneyType.Kill, add);
				UtilManager.Instance.TryPlaySelfGoldEff(PlayerControlMgr.Instance.GetPlayer());
			}
			else if (units != null)
			{
				UtilManager.Instance.TryJumpGoldFont(unit, units, (AddMoneyType)addType, add);
				UtilManager.Instance.TryPlayGoldEff(unit, units, (AddMoneyType)addType);
				Units player = PlayerControlMgr.Instance.GetPlayer();
				AudioMgr.Play("Play_FX_Coins", player.gameObject, false, false);
			}
		}

		private void P2C_KillAddMoney(MobaMessage msg)
		{
			P2CKillAddMoney probufMsg = msg.GetProbufMsg<P2CKillAddMoney>();
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int unitId = probufMsg.unitId;
			int add = probufMsg.add;
			int deadId = probufMsg.deadId;
			Units unit = MapManager.Instance.GetUnit(unitId);
			Units unit2 = MapManager.Instance.GetUnit(deadId);
			if (unit != null && unit2 != null && unit2.isVisible)
			{
				unit2.JumpGoldFont(add, unit);
			}
		}

		private void P2C_LastTeamKillMonster(MobaMessage msg)
		{
			TeamKillInfo probufMsg = msg.GetProbufMsg<TeamKillInfo>();
			if (probufMsg == null)
			{
				return;
			}
			int attackerId = probufMsg.attackerId;
			Units unit = MapManager.Instance.GetUnit(attackerId);
			if (null == unit)
			{
				return;
			}
			int teamType = unit.teamType;
			string targetTypeid = probufMsg.targetTypeid;
			PvpStatisticMgr.GroupData groupData = this.StatisticMgr.GetGroupData(teamType);
			SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(targetTypeid);
			if (monsterMainData == null)
			{
				ClientLogger.Error("can't get data for:" + targetTypeid);
				return;
			}
			if (monsterMainData.item_type == 2)
			{
				groupData.TeamTowerDestroy++;
			}
			else if (monsterMainData.item_type == 7)
			{
				groupData.TeamEpicMonsterKill++;
			}
		}

		private void P2C_KillHore(MobaMessage msg)
		{
			KillResult probufMsg = msg.GetProbufMsg<KillResult>();
			PvpStatisticMgr.HeroData heroData = this.StatisticMgr.GetHeroData(probufMsg.attackerId);
			PvpStatisticMgr.HeroData heroData2 = this.StatisticMgr.GetHeroData(probufMsg.targetId);
			if (heroData == null || heroData2 == null)
			{
				return;
			}
			KillType killType = (KillType)probufMsg.killType;
			if (killType == KillType.FirstBoold)
			{
				heroData.FirstKill = true;
			}
			heroData.HeroKill++;
			heroData2.Death++;
			Units units = null;
			if (probufMsg.attackerId != 0)
			{
				units = MapManager.Instance.GetUnit(probufMsg.attackerId);
				if (units)
				{
					if (!units.isTower)
					{
						PvpStatisticMgr.GroupData groupData = this.StatisticMgr.GetGroupData(units.teamType);
						groupData.TeamKill++;
					}
				}
				else
				{
					ClientLogger.Error("cannot found hero #" + probufMsg.attackerId);
				}
			}
			Units unit = MapManager.Instance.GetUnit(probufMsg.targetId);
			if (unit.isHero)
			{
				PvpStatisticMgr.GroupData groupData2 = this.StatisticMgr.GetGroupData(unit.teamType);
				groupData2.TeamDeath++;
				Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.UpdateView);
			}
			List<int> helperList = probufMsg.helperList;
			if (helperList != null)
			{
				foreach (int current in helperList)
				{
					PvpStatisticMgr.HeroData heroData3 = this.StatisticMgr.GetHeroData(current);
					heroData3.Assist++;
					Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitKillAndAssist, MapManager.Instance.GetUnit(current), null, null);
				}
			}
			GameManager.Instance.AchieveManager.BrocastAchievement(probufMsg.attackerId, probufMsg.targetId, killType, probufMsg.helperList, (int)probufMsg.mutiKill, (int)probufMsg.nodeadKillCount, probufMsg.attckerTypeID);
			GameManager.Instance.ChaosFightMgr.TryChangeLeadingTeam(units);
		}

		private void P2C_LastKillMonster(MobaMessage msg)
		{
			P2CLastKillMonster probufMsg = msg.GetProbufMsg<P2CLastKillMonster>();
			int unitId = probufMsg.unitId;
			int killMonsterCount = probufMsg.killMonsterCount;
			PvpStatisticMgr.HeroData heroData = this.StatisticMgr.GetHeroData(unitId);
			heroData.MonsterKill = killMonsterCount;
			Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.UpdateView);
		}

		private void P2C_AddBuffExt(MobaMessage msg)
		{
		}

		private void P2C_RemoveBuffExt(MobaMessage msg)
		{
		}

		protected override void OnAfterBattleEnd(P2CBattleEndInfo info)
		{
			PvpStateManager.Instance.ChangeState(new PvpStateFinish());
		}

		private void P2C_GetMapItem(MobaMessage msg)
		{
			P2CGetMapItem probufMsg = msg.GetProbufMsg<P2CGetMapItem>();
			int attackerId = probufMsg.attackerId;
			int targetId = probufMsg.targetId;
			Units unit = MapManager.Instance.GetUnit(targetId);
			if (unit == null)
			{
				return;
			}
			if (unit is SkillUnits_Blood)
			{
				Units unit2 = MapManager.Instance.GetUnit(attackerId);
				SkillUnits_Blood skillUnits_Blood = unit as SkillUnits_Blood;
				if (skillUnits_Blood != null && unit2 != null)
				{
					skillUnits_Blood.DoAdsorbent(unit2);
				}
				else if (unit2 == null)
				{
					skillUnits_Blood.RemoveSelf(-1f);
				}
			}
			else if (unit is SkillUnit)
			{
				SkillUnit skillUnit = unit as SkillUnit;
				if (skillUnit != null)
				{
					skillUnit.RemoveSelf(-1f);
				}
			}
		}

		private void P2C_ActiveUnit(MobaMessage msg)
		{
			P2CActiveUnit probufMsg = msg.GetProbufMsg<P2CActiveUnit>();
			int unitId = probufMsg.unitId;
			Units unit = MapManager.Instance.GetUnit(unitId);
			if (!unit)
			{
				ClientLogger.Error("P2C_ActiveUnit: unit not found #" + unitId);
				return;
			}
			if (unit.IsMonsterCreep())
			{
				Monster monster = unit as Monster;
				if (monster.Sleeping.IsInState)
				{
					monster.Wakeup(false);
				}
			}
			else
			{
				ClientLogger.Error("P2C_ActiveUnit: only monster can be activated" + unit.name);
			}
		}

		private void P2C_CreateUnits(MobaMessage msg)
		{
			try
			{
				UnitInfo probufMsg = msg.GetProbufMsg<UnitInfo>();
				if (probufMsg != null)
				{
					Units units = PvpProtocolTools.CreateMonsterByUnitInfo(probufMsg);
					if (units != null)
					{
						if (units.npc_id == "54321")
						{
							units.mTransform.eulerAngles = new Vector3(0f, -145f, 0f);
						}
						units.UpdateVisible();
					}
				}
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
			}
		}

		private void P2C_Kill(MobaMessage msg)
		{
			P2CKillInfo probufMsg = msg.GetProbufMsg<P2CKillInfo>();
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int attackerId = probufMsg.attackerId;
			int targetId = probufMsg.targetId;
			long relivetime = probufMsg.relivetime;
			Units unit = MapManager.Instance.GetUnit(targetId);
			Units unit2 = MapManager.Instance.GetUnit(attackerId);
			PvpProtocolTools.ToDie(unit, unit2, relivetime);
			if (unit2 && unit2.isHero && Singleton<PvpManager>.Instance.IsObserver)
			{
				UtilManager.Instance.TryPlayGoldEff(unit2, unit, AddMoneyType.AddMoneyHighEffectAdd);
			}
		}

		private void P2C_MonsterPseudoDeath(MobaMessage msg)
		{
			P2CMonsterPseudoDeath probufMsg = msg.GetProbufMsg<P2CMonsterPseudoDeath>();
			int unitId = probufMsg.unitId;
			int attackerId = probufMsg.attackerId;
			int oldGroup = probufMsg.oldGroup;
			int newGroup = probufMsg.newGroup;
			float hp = probufMsg.hp;
			float mp = probufMsg.mp;
			string typeId = probufMsg.typeId;
			int oldCreepVoId = probufMsg.oldCreepVoId;
			Units unit = MapManager.Instance.GetUnit(unitId);
			Units unit2 = MapManager.Instance.GetUnit(attackerId);
			PvpProtocolTools.DoMonsterPseudoDeath(unit, unit2, oldGroup, newGroup, hp, mp, typeId, oldCreepVoId.ToString());
			int teamType = unit2.teamType;
			PvpStatisticMgr.GroupData groupData = this.StatisticMgr.GetGroupData(teamType);
			SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(unit.model_id.ToString());
			if (monsterMainData == null || groupData == null)
			{
				ClientLogger.Error("can't get data for:" + unitId);
				return;
			}
			if (monsterMainData.item_type == 7)
			{
				groupData.TeamEpicMonsterKill++;
			}
		}

		private void P2C_NotifySpawnSuperSoldier(MobaMessage msg)
		{
			P2CNotifySpawnSuperSoldier probufMsg = msg.GetProbufMsg<P2CNotifySpawnSuperSoldier>();
			if (probufMsg != null)
			{
				PvpProtocolTools.HandleMsgNotifySpawnSuperSoldier((TeamType)probufMsg.group);
			}
		}

		private void P2C_NotifyMonsterCreepAiStatus(MobaMessage msg)
		{
			P2CNotifyMonsterCreepAiStatusData probufMsg = msg.GetProbufMsg<P2CNotifyMonsterCreepAiStatusData>();
			if (probufMsg != null)
			{
				PvpProtocolTools.HandleMsgNotifyMonsterCreepAiStatus(probufMsg.unitId, (EMonsterCreepAiStatus)probufMsg.monsterCreepAiStatus);
			}
		}

		private void P2C_RemovePlayerControlUnit(MobaMessage msg)
		{
		}

		private void P2C_AddPlayerControlUnit(MobaMessage msg)
		{
		}

		private void PauseGame()
		{
			GameTimer.StopTimeScale();
		}

		private void ResumeGame()
		{
			GameTimer.NormalTimeScale();
		}

		private void P2C_UnitsVisiableChanged(MobaMessage msg)
		{
			P2CUnitsVisiableChanged p2CUnitsVisiableChanged = (P2CUnitsVisiableChanged)msg.ProtoMsg;
			if (p2CUnitsVisiableChanged == null)
			{
				p2CUnitsVisiableChanged = msg.GetProbufMsg<P2CUnitsVisiableChanged>();
			}
			Units unit = MapManager.Instance.GetUnit(p2CUnitsVisiableChanged.unitId);
			if (unit == null)
			{
				return;
			}
			unit.m_nServerVisibleState = p2CUnitsVisiableChanged.state;
		}

		private void P2C_UnitsSnap(MobaMessage msg)
		{
			UnitSnapInfo unitSnapInfo = (UnitSnapInfo)msg.ProtoMsg;
			if (unitSnapInfo == null)
			{
				unitSnapInfo = msg.GetProbufMsg<UnitSnapInfo>();
			}
			Units unit = MapManager.Instance.GetUnit(unitSnapInfo.unitId);
			if (unit == null)
			{
				return;
			}
			if (!unit.IsSyncPosition)
			{
				return;
			}
			if (unit != null && unit.OnPvpServerMsg(msg))
			{
				return;
			}
			unit.moveController.P2C_UnitsSnap(unitSnapInfo);
		}

		private void P2C_MoveToPos(MobaMessage msg)
		{
			MoveToPos probufMsg = msg.GetProbufMsg<MoveToPos>();
			Units unit = MapManager.Instance.GetUnit(probufMsg.unitId);
			if (null == unit)
			{
				return;
			}
			if (unit != null && unit.OnPvpServerMsg(msg))
			{
				return;
			}
			unit.moveController.MoveToPoint_Impl(probufMsg.pos, probufMsg.pos, 0f);
		}

		private void P2C_MoveToPosWithPath(MobaMessage msg)
		{
			MoveWithPath probufMsg = msg.GetProbufMsg<MoveWithPath>();
			Units unit = MapManager.Instance.GetUnit(probufMsg.unitId);
			if (null == unit)
			{
				return;
			}
			if (unit != null && unit.OnPvpServerMsg(msg))
			{
				return;
			}
			unit.moveController.MoveToPosWithPath_Impl(probufMsg);
		}

		private void P2C_StopMove(MobaMessage msg)
		{
			StopMove probufMsg = msg.GetProbufMsg<StopMove>();
			Units unit = MapManager.Instance.GetUnit(probufMsg.unitId);
			if (null == unit)
			{
				return;
			}
			if (unit.IsMaster)
			{
				return;
			}
			unit.moveController.StopMove_Impl(probufMsg.pos, probufMsg.rotate);
		}

		private void P2C_RotateTo(MobaMessage msg)
		{
		}

		private void P2C_TipMessage(MobaMessage msg)
		{
			P2CTipMessage probufMsg = msg.GetProbufMsg<P2CTipMessage>();
			string luaStr = StringUtils.GetLuaStr(probufMsg.msg);
			byte type = probufMsg.type;
			if (type == 5)
			{
				if (CtrlManager.IsWindowOpen(WindowID.NewPopView))
				{
					CtrlManager.CloseWindow(WindowID.NewPopView);
				}
				CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("Tips_Hung_Punish"), LanguageManager.Instance.GetStringById("Tips_Hung_Punish_Content"), delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
				return;
			}
			if (type == 4)
			{
				if (CtrlManager.IsWindowOpen(WindowID.NewPopView))
				{
					CtrlManager.CloseWindow(WindowID.NewPopView);
				}
				CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("Tips_Hung"), LanguageManager.Instance.GetStringById("Tips_Hung_Content"), delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
				return;
			}
			if (type == 3)
			{
				AudioMgr.Play("Play_Box_Alert", null, false, false);
				string[] array = luaStr.Split(new char[]
				{
					','
				});
				if (array.Length == 2)
				{
					string s = array[0];
					string s2 = array[1];
					float x = 0f;
					float y = 0f;
					if (float.TryParse(s, out x) && float.TryParse(s2, out y))
					{
						GameManager.Instance.ChaosFightMgr.ShowChestAlert(x, y);
					}
				}
				return;
			}
			if (type == 2)
			{
				AudioMgr.Play("Play_Box_Pick", null, false, false);
				if (luaStr != null)
				{
					string[] array2 = luaStr.Split(new char[]
					{
						'_'
					});
					if (array2.Length == 2)
					{
						string unit_id = array2[0];
						string equip_id = array2[1];
						UIMessageBox.ShowEquipMsg(unit_id, equip_id);
						return;
					}
				}
			}
			if (type == 0)
			{
				Units player = PlayerControlMgr.Instance.GetPlayer();
				if (player.gameObject != null)
				{
					AudioMgr.Play("Play_Coin_Pick", player.gameObject, false, false);
				}
			}
			UIMessageBox.ShowMessage(luaStr, 1.5f, 0);
		}

		private void OnFrameSync(MobaMessage msg)
		{
			long num = (long)msg.Param;
			if (FrameSyncManager.Instance.UseFrame)
			{
				FrameSyncManager.Instance.mFrameTime = (double)num * FrameSyncManager.Instance.OneFrameTime;
			}
			else
			{
				FrameSyncManager.Instance.mFrameTime = (double)num * 1E-07;
			}
		}

		private void OnGMCheat(MobaMessage msg)
		{
			CheatInfo probufMsg = msg.GetProbufMsg<CheatInfo>();
			if (Cheat.Instance != null)
			{
				Cheat.Instance.Execute(probufMsg.cheatMsg, false);
			}
		}

		private void P2C_ReadySkill(MobaMessage msg)
		{
			ReadySkillInfo readySkillInfo = (ReadySkillInfo)msg.ProtoMsg;
			if (readySkillInfo == null)
			{
				readySkillInfo = msg.GetProbufMsg<ReadySkillInfo>();
			}
			if (readySkillInfo == null)
			{
				return;
			}
			int unitId = readySkillInfo.unitId;
			Vector3 zero = Vector3.zero;
			Units unit = MapManager.Instance.GetUnit(unitId);
			if (unit == null)
			{
				return;
			}
			if (unit.OnPvpServerMsg(msg))
			{
				return;
			}
			Skill skillOrAttackById = unit.getSkillOrAttackById(readySkillInfo.skillId);
			if (skillOrAttackById == null)
			{
				return;
			}
			if (unit.isPlayer)
			{
			}
			if (this.IsNormalSkillOutOfView(skillOrAttackById, unit))
			{
				return;
			}
			if (skillOrAttackById != null && unit.isPlayer)
			{
				unit.SetWaitCool(0f, true);
				if (skillOrAttackById.IsSkill && !skillOrAttackById.Data.isCanMoveInSkillCastin)
				{
					unit.SetWaitCool(skillOrAttackById.Data.castBefore, true);
				}
			}
			if (unit.isPlayer && PvpServerStartSkillHeroList.IsStartByServer(unit.npc_id) && readySkillInfo.errorCode != PvpUseSkillErrorCode.OK)
			{
				PvpUseSkillErrorCode errorCode = readySkillInfo.errorCode;
				switch (errorCode)
				{
				case PvpUseSkillErrorCode.InCd:
					UIMessageBox.ShowMessage("技能冷却中", 1.5f, 0);
					break;
				case PvpUseSkillErrorCode.LackMp:
					Singleton<SkillView>.Instance.lanTiao.ShowQueLan();
					break;
				case PvpUseSkillErrorCode.LackHp:
					UIMessageBox.ShowMessage("生命值不足", 1.5f, 0);
					break;
				default:
					if (errorCode != PvpUseSkillErrorCode.InStateZhimang)
					{
						if (errorCode != PvpUseSkillErrorCode.InStateChenmo)
						{
							if (errorCode != PvpUseSkillErrorCode.NoTarget)
							{
								if (errorCode == PvpUseSkillErrorCode.OutOfRange)
								{
									UIMessageBox.ShowMessage("超出施法范围，不能释放！", 1.5f, 0);
								}
							}
							else
							{
								UIMessageBox.ShowMessage("未找到目标，不能释放！", 1.5f, 0);
							}
						}
						else
						{
							UIMessageBox.ShowMessage("被沉默中无法释放技能", 1.5f, 0);
						}
					}
					else
					{
						UIMessageBox.ShowMessage("无法攻击！", 1.5f, 0);
					}
					break;
				}
				if (skillOrAttackById != null)
				{
					if (skillOrAttackById.IsSkill)
					{
						Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitSkillFailed, unit, null, null);
					}
					else
					{
						Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitAttackFailed, unit, null, null);
					}
				}
				HeroAttackController heroAttackController = unit.getAtkController() as HeroAttackController;
				if (heroAttackController != null)
				{
					heroAttackController.sendUseSkills[readySkillInfo.skillId] = false;
				}
				Singleton<TriggerManager>.Instance.SendUnitSkillStateEvent(UnitEvent.UnitSkillCmdFailed, unit, skillOrAttackById);
				return;
			}
			if (skillOrAttackById != null && skillOrAttackById.IsAttack)
			{
				HeroAttackController heroAttackController2 = unit.getAtkController() as HeroAttackController;
				if (heroAttackController2 != null)
				{
					heroAttackController2.IncComboIndex();
				}
				unit.SetAttackTimeLengh(unit.attack_timelenth, false);
			}
			Skill skillOrAttackById2 = unit.getSkillOrAttackById(readySkillInfo.skillId);
			SVector3 targetPosition = readySkillInfo.targetPosition;
			Vector3? vector = null;
			List<Units> list = null;
			if (readySkillInfo.targetUnits != null)
			{
				list = new List<Units>();
				for (int i = 0; i < readySkillInfo.targetUnits.Count; i++)
				{
					Units unit2 = MapManager.Instance.GetUnit((int)readySkillInfo.targetUnits[i]);
					list.Add(unit2);
				}
				if (list[0] != null)
				{
					if (skillOrAttackById2.needTarget)
					{
						vector = new Vector3?(list[0].mTransform.position);
						PlayerControlMgr.Instance.TryChangeTargetOnSkillStart(unit, list);
					}
					if (unit.isTower || unit.isHome)
					{
						Building building = unit as Building;
						if (building)
						{
							building.BuildingAttackTarget = list[0];
						}
					}
				}
			}
			if (!skillOrAttackById2.needTarget && targetPosition != null)
			{
				vector = new Vector3?(new Vector3(targetPosition.x, targetPosition.y, targetPosition.z));
				float y = AstarPath.active.GetPosHeight(vector.Value) + 0.05f;
				vector = new Vector3?(new Vector3(vector.Value.x, y, vector.Value.z));
			}
			if (!GlobalSettings.Instance.ClientGoAhead || !(unit == PlayerControlMgr.Instance.GetPlayer()))
			{
				ActionManager.ReadySkill(new SkillDataKey(readySkillInfo.skillId, skillOrAttackById2.skillLevel, 0), unit, list, vector, skillOrAttackById2, false);
				skillOrAttackById2.attackTargets = list;
				skillOrAttackById2.attackPosition = vector;
				skillOrAttackById2.OnSkillReadyPvp();
			}
			skillOrAttackById2.InitReadySkill();
		}

		private void P2C_ReadySkillCheck(MobaMessage msg)
		{
			ReadySkillCheckInfo probufMsg = msg.GetProbufMsg<ReadySkillCheckInfo>();
			if (probufMsg == null)
			{
				return;
			}
			int unitId = probufMsg.unitId;
			Units unit = MapManager.Instance.GetUnit(unitId);
			if (unit != null && unit.isPlayer)
			{
				HeroAttackController heroAttackController = unit.getAtkController() as HeroAttackController;
				if (heroAttackController != null)
				{
					heroAttackController.ReadySkillCheck(probufMsg);
				}
			}
		}

		private void P2C_StartSkill(MobaMessage msg)
		{
			StartSkillInfo startSkillInfo = (StartSkillInfo)msg.ProtoMsg;
			if (startSkillInfo == null)
			{
				startSkillInfo = msg.GetProbufMsg<StartSkillInfo>();
			}
			if (startSkillInfo != null)
			{
				int unitId = startSkillInfo.unitId;
				Vector3 vector = Vector3.zero;
				Units unit = MapManager.Instance.GetUnit(unitId);
				if (unit == null)
				{
					return;
				}
				if (unit.OnPvpServerMsg(msg))
				{
					return;
				}
				Skill skillOrAttackById = unit.getSkillOrAttackById(startSkillInfo.skillId);
				if (skillOrAttackById == null)
				{
					return;
				}
				if (this.IsNormalSkillOutOfView(skillOrAttackById, unit))
				{
					return;
				}
				if (skillOrAttackById != null && unit.isPlayer && skillOrAttackById.IsSkill)
				{
					if (skillOrAttackById.Data.config.interrupt != 1 && skillOrAttackById.Data.config.interrupt != 3)
					{
						unit.SetWaitCool(skillOrAttackById.Data.castIn, true);
					}
					if (skillOrAttackById.Data.isCanMoveInSkillCastin)
					{
						unit.SetWaitCool(0f, true);
					}
				}
				List<Units> list = null;
				if (startSkillInfo.targetUnits != null && skillOrAttackById.needTarget)
				{
					list = new List<Units>();
					for (int i = 0; i < startSkillInfo.targetUnits.Count; i++)
					{
						Units unit2 = MapManager.Instance.GetUnit((int)startSkillInfo.targetUnits[i]);
						list.Add(unit2);
					}
					if (list[0] != null)
					{
						vector = list[0].mTransform.position;
					}
				}
				else
				{
					SVector3 targetPosition = startSkillInfo.targetPosition;
					vector = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
					float y = AstarPath.active.GetPosHeight(vector) + 0.05f;
					vector = new Vector3(vector.x, y, vector.z);
				}
				ActionManager.StartSkill(new SkillDataKey(startSkillInfo.skillId, skillOrAttackById.skillLevel, 0), unit, list, new Vector3?(vector), false, skillOrAttackById);
				if (PvpServerStartSkillHeroList.IsStartByServer(unit.npc_id))
				{
					Skill skillOrAttackById2 = unit.getSkillOrAttackById(startSkillInfo.skillId);
					if (skillOrAttackById2 != null)
					{
						if (skillOrAttackById2.IsSkill)
						{
							Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitSkillOver, unit, null, null);
						}
						else
						{
							Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitAttackOver, unit, null, null);
						}
						if (skillOrAttackById2.IsGuide && skillOrAttackById2.IsShowGuideBar && unit.isPlayer)
						{
							Singleton<SkillView>.Instance.ShowGuideBar(true, skillOrAttackById2.data.guideTime, LanguageManager.Instance.GetStringById(skillOrAttackById2.skillName));
						}
					}
					HeroAttackController heroAttackController = unit.getAtkController() as HeroAttackController;
					if (heroAttackController != null)
					{
						heroAttackController.sendUseSkills[startSkillInfo.skillId] = false;
					}
				}
			}
		}

		private void P2C_HitSkill(MobaMessage msg)
		{
			HitSkillInfo hitSkillInfo = (HitSkillInfo)msg.ProtoMsg;
			if (hitSkillInfo == null)
			{
				hitSkillInfo = msg.GetProbufMsg<HitSkillInfo>();
			}
			if (hitSkillInfo != null)
			{
				int unitId = hitSkillInfo.unitId;
				Units unit = MapManager.Instance.GetUnit(unitId);
				if (unit != null)
				{
					if (unit.OnPvpServerMsg(msg))
					{
						return;
					}
					if (!unit.isVisibleInCamera)
					{
						return;
					}
					Skill skillOrAttackById = unit.getSkillOrAttackById(hitSkillInfo.skillId);
					if (skillOrAttackById != null)
					{
						if (!hitSkillInfo.bForceDestroy)
						{
							List<Units> list = null;
							if (hitSkillInfo.targetIds != null)
							{
								for (int i = 0; i < hitSkillInfo.targetIds.Count; i++)
								{
									Units unit2 = MapManager.Instance.GetUnit((int)hitSkillInfo.targetIds[i]);
									if (unit2 && unit2.isVisible)
									{
										if (list == null)
										{
											list = ListPool.get<Units>();
										}
										list.Add(unit2);
									}
								}
							}
							if (skillOrAttackById.OnTargetHitCallback != null)
							{
								skillOrAttackById.OnTargetHitCallback(list);
							}
							if (list != null)
							{
								ActionManager.HitSkill(new SkillDataKey(hitSkillInfo.skillId, skillOrAttackById.skillLevel, 0), unit, list, false);
								ListPool.release<Units>(list);
							}
							if (skillOrAttackById.OnPerformHitCallback != null)
							{
								skillOrAttackById.OnPerformHitCallback(hitSkillInfo.performId, hitSkillInfo.index);
							}
						}
						else if (skillOrAttackById.DestroyAction != null)
						{
							skillOrAttackById.DestroyAction(hitSkillInfo.performId, hitSkillInfo.index);
						}
					}
				}
			}
		}

		private void P2C_EndSkill(MobaMessage msg)
		{
			EndSkillInfo endSkillInfo = (EndSkillInfo)msg.ProtoMsg;
			if (endSkillInfo == null)
			{
				endSkillInfo = msg.GetProbufMsg<EndSkillInfo>();
			}
			if (endSkillInfo != null)
			{
				int unitId = endSkillInfo.unitId;
				Units unit = MapManager.Instance.GetUnit(unitId);
				if (unit != null)
				{
					if (unit.OnPvpServerMsg(msg))
					{
						return;
					}
					if (unit.isPlayer)
					{
					}
					Skill skillOrAttackById = unit.getSkillOrAttackById(endSkillInfo.skillId);
					if (skillOrAttackById != null)
					{
						if (this.IsNormalSkillOutOfView(skillOrAttackById, unit))
						{
							return;
						}
						if (unit.isPlayer)
						{
							unit.OnSkillEnd(endSkillInfo.skillId);
						}
						ActionManager.EndSkill(new SkillDataKey(endSkillInfo.skillId, skillOrAttackById.skillLevel, 0), unit, false);
						skillOrAttackById.StartEndTask();
						Singleton<TriggerManager>.Instance.SendUnitSkillStateEvent(UnitEvent.UnitSkillCmdEnd, unit, skillOrAttackById);
					}
				}
			}
		}

		private void P2C_StopSkill(MobaMessage msg)
		{
			StopSkillInfo stopSkillInfo = (StopSkillInfo)msg.ProtoMsg;
			if (stopSkillInfo == null)
			{
				stopSkillInfo = msg.GetProbufMsg<StopSkillInfo>();
			}
			if (stopSkillInfo != null)
			{
				Units unit = MapManager.Instance.GetUnit(stopSkillInfo.unitId);
				if (unit != null)
				{
					if (unit.OnPvpServerMsg(msg))
					{
						return;
					}
					if (unit.isPlayer)
					{
					}
					Skill skillOrAttackById = unit.getSkillOrAttackById(stopSkillInfo.skillId);
					if (this.IsNormalSkillOutOfView(skillOrAttackById, unit))
					{
						return;
					}
					if (stopSkillInfo.stopSkillType == 0 && !unit.IsMonsterCreep())
					{
						unit.animController.PlayAnim(AnimationType.Move, true, 0, true, false);
					}
					if (stopSkillInfo == null)
					{
						return;
					}
					if (skillOrAttackById == null)
					{
						return;
					}
					ActionManager.PVP_StopSkill(new SkillDataKey(stopSkillInfo.skillId, skillOrAttackById.skillLevel, 0), unit, (SkillInterruptType)stopSkillInfo.interruptType, (SkillCastPhase)stopSkillInfo.skillStep, false);
				}
			}
		}

		private void P2C_DoSkill(MobaMessage msg)
		{
			DoSkillInfo probufMsg = msg.GetProbufMsg<DoSkillInfo>();
			if (probufMsg != null)
			{
				Units unit = MapManager.Instance.GetUnit(probufMsg.unitId);
				if (unit != null)
				{
					ActionManager.DoSkill(unit, probufMsg.skillId);
				}
			}
		}

		private void P2C_JumpFont(MobaMessage msg)
		{
			JumpFontInfo probufMsg = msg.GetProbufMsg<JumpFontInfo>();
			if (probufMsg != null)
			{
				int unitId = probufMsg.unitId;
				Units unit = MapManager.Instance.GetUnit(unitId);
				Units unit2 = MapManager.Instance.GetUnit(probufMsg.attackerId);
				if (unit != null)
				{
					unit.jumpFont((JumpFontType)probufMsg.type, probufMsg.text, unit2, true);
				}
			}
		}

		private void P2C_DataChange(MobaMessage msg)
		{
			DataChangeInfo probufMsg = msg.GetProbufMsg<DataChangeInfo>();
			if (probufMsg != null)
			{
				Units unit = MapManager.Instance.GetUnit(probufMsg.unitId);
				Units unit2 = MapManager.Instance.GetUnit(probufMsg.attackerId);
				if (unit2 != null && unit2.OnPvpServerMsg(msg))
				{
					return;
				}
				int[] damageIds = probufMsg.damageIds;
				bool reverse = probufMsg.reverse;
				if (unit != null)
				{
					ActionManager.BuffChange(unit, unit2, probufMsg.buffId, reverse, false);
				}
			}
		}

		private void P2C_DataUpdate(MobaMessage msg)
		{
			DataUpdateInfo probufMsg = msg.GetProbufMsg<DataUpdateInfo>();
			Units unit = MapManager.Instance.GetUnit(probufMsg.unitId);
			if (unit != null && unit.OnPvpServerMsg(msg))
			{
				return;
			}
			ActionManager.DataUpdate(unit, probufMsg, false);
		}

		private void P2C_Wound(MobaMessage msg)
		{
			WoundInfo probufMsg = msg.GetProbufMsg<WoundInfo>();
			if (probufMsg != null)
			{
				Units unit = MapManager.Instance.GetUnit(probufMsg.unitId);
				Units unit2 = MapManager.Instance.GetUnit(probufMsg.attackerId);
				if (unit2 != null && unit2.OnPvpServerMsg(msg))
				{
					return;
				}
				if (unit != null)
				{
					ActionManager.Wound(unit, unit2, probufMsg.dataKeys, probufMsg.dataValues, probufMsg.damageExtInfo == 1, false, probufMsg.damageType);
					unit.data.SetAttrVal(probufMsg.dataKeys, probufMsg.dataAfterValues);
				}
			}
		}

		private void P2C_AddBuff(MobaMessage msg)
		{
			BuffInfo probufMsg = msg.GetProbufMsg<BuffInfo>();
			if (probufMsg != null)
			{
				Units unit = MapManager.Instance.GetUnit(probufMsg.unitId);
				Units unit2 = MapManager.Instance.GetUnit(probufMsg.casterUnitId);
				if (unit2 != null && unit2.OnPvpServerMsg(msg))
				{
					return;
				}
				if (!this.AddBuffExtraFilter(unit2, probufMsg.buffId))
				{
					return;
				}
				if (unit != null)
				{
					ActionManager.AddBuff(probufMsg.buffId, unit, unit2, false, string.Empty);
				}
				else if (Application.isEditor)
				{
					UnityEngine.Debug.LogError("target is null" + probufMsg.unitId);
				}
			}
			else if (Application.isEditor)
			{
				UnityEngine.Debug.LogError("info is null");
			}
		}

		private bool AddBuffExtraFilter(Units actionUnit, string buffId)
		{
			return !(buffId == "Skill_Aier_03") || !(actionUnit != PlayerControlMgr.Instance.GetPlayer());
		}

		private void P2C_DoBuff(MobaMessage msg)
		{
			BuffInfo probufMsg = msg.GetProbufMsg<BuffInfo>();
			if (probufMsg != null)
			{
				Units unit = MapManager.Instance.GetUnit(probufMsg.unitId);
				if (unit != null)
				{
					ActionManager.StartBuff(probufMsg.buffId, unit, false);
				}
			}
		}

		private void P2C_RemoveBuff(MobaMessage msg)
		{
			BuffInfo probufMsg = msg.GetProbufMsg<BuffInfo>();
			if (probufMsg != null)
			{
				Units unit = MapManager.Instance.GetUnit(probufMsg.unitId);
				Units unit2 = MapManager.Instance.GetUnit(probufMsg.casterUnitId);
				if (unit != null && unit.OnPvpServerMsg(msg))
				{
					return;
				}
				if (unit != null)
				{
					ActionManager.RemoveBuff(probufMsg.buffId, unit, unit2, probufMsg.reduce_layers, false);
				}
			}
		}

		private void P2C_AddHighEffect(MobaMessage msg)
		{
			HighEffInfo probufMsg = msg.GetProbufMsg<HighEffInfo>();
			if (probufMsg != null)
			{
				Units unit = MapManager.Instance.GetUnit(probufMsg.casterUnitId);
				if (unit != null && unit.OnPvpServerMsg(msg))
				{
					return;
				}
				List<Units> list = null;
				if (probufMsg.unitIds != null)
				{
					list = new List<Units>();
					for (int i = 0; i < probufMsg.unitIds.Count; i++)
					{
						Units unit2 = MapManager.Instance.GetUnit((int)probufMsg.unitIds[i]);
						if (unit2 != null && unit2.isVisibleInCamera)
						{
							list.Add(unit2);
						}
					}
				}
				if (list != null && list.Count > 0)
				{
					Vector3 value = default(Vector3);
					if (probufMsg.skillPosition != null)
					{
						value.x = probufMsg.skillPosition.x;
						value.y = probufMsg.skillPosition.y;
						value.z = probufMsg.skillPosition.z;
					}
					ActionManager.AddHighEffect(probufMsg.highEffId, probufMsg.skillId, list, unit, new Vector3?(value), false);
				}
			}
		}

		private void P2C_DoHighEffect(MobaMessage msg)
		{
			HighEffInfo highEffInfo = (HighEffInfo)msg.ProtoMsg;
			if (highEffInfo == null)
			{
				highEffInfo = msg.GetProbufMsg<HighEffInfo>();
			}
			if (highEffInfo != null)
			{
				Units unit = MapManager.Instance.GetUnit(highEffInfo.casterUnitId);
				Units unit2 = MapManager.Instance.GetUnit(highEffInfo.ownerUnitId);
				if (unit2 != null && unit2.OnPvpServerMsg(msg))
				{
					return;
				}
				List<Units> list = null;
				if (highEffInfo.unitIds != null)
				{
					list = new List<Units>();
					for (int i = 0; i < highEffInfo.unitIds.Count; i++)
					{
						Units unit3 = MapManager.Instance.GetUnit((int)highEffInfo.unitIds[i]);
						if (unit3 != null && unit3.isVisibleInCamera)
						{
							list.Add(unit3);
						}
					}
				}
				Vector3 value = default(Vector3);
				if (highEffInfo.skillPosition != null)
				{
					value.x = highEffInfo.skillPosition.x;
					value.y = highEffInfo.skillPosition.y;
					value.z = highEffInfo.skillPosition.z;
				}
				StartHighEffectAction action = ActionManager.StartHighEffect(highEffInfo.highEffId, highEffInfo.skillId, list, unit, highEffInfo.rotatoY, new Vector3?(value), unit2, false);
				if (unit2 != null)
				{
					unit2.highEffManager.AddAction(highEffInfo.highEffId, action);
				}
			}
		}

		private void P2C_RemoveHighEffect(MobaMessage msg)
		{
			RemoveHighEffInfo probufMsg = msg.GetProbufMsg<RemoveHighEffInfo>();
			if (probufMsg != null)
			{
				Units unit = MapManager.Instance.GetUnit(probufMsg.unitId);
				if (unit != null)
				{
					if (unit != null && unit.OnPvpServerMsg(msg))
					{
						return;
					}
					ActionManager.RemoveHighEffect(probufMsg.highEffId, unit, false);
				}
			}
		}

		private void P2C_UpSkillLevel(MobaMessage msg)
		{
			P2CUpSkillLevel probufMsg = msg.GetProbufMsg<P2CUpSkillLevel>();
			int unitId = probufMsg.unitId;
			string skillId = probufMsg.skillId;
			int skillLevel = (int)probufMsg.skillLevel;
			Units unit = MapManager.Instance.GetUnit(unitId);
			if (skillLevel == -999)
			{
				if (unit != null && unit.isPlayer)
				{
					Singleton<SkillView>.Instance.ShowCanLevelUp();
				}
				return;
			}
			if (unit != null)
			{
				if (skillLevel != 0)
				{
					unit.skillManager.UpgradeSkillLevel(skillId, skillLevel);
				}
				if (unit.isPlayer)
				{
					Skill skillById = unit.skillManager.getSkillById(skillId);
					if (skillById == null)
					{
						return;
					}
					if (skillLevel != 0)
					{
						Singleton<SkillView>.Instance.GetSkillLevelUpCallBack(skillId, skillLevel);
					}
					Singleton<SkillView>.Instance.UpdateSkillItem(skillById.skillIndex);
					Singleton<SkillView>.Instance.UpdateSkillLock(skillById.skillIndex);
					Singleton<SkillView>.Instance.ShowCanLevelUp();
					Singleton<SkillView>.Instance.CheckIconToGrayByCanUse(unit, skillById.skillIndex);
				}
			}
		}

		private void P2C_ReBirth(MobaMessage msg)
		{
			P2CRebirth probufMsg = msg.GetProbufMsg<P2CRebirth>();
			int unitId = probufMsg.unitId;
			Units unit = MapManager.Instance.GetUnit(unitId);
			if (unit != null)
			{
				unit.Rebirth();
				ActionManager.PVP_Rebirth("HighEff_140", "Skill_Kulouwang_04", new List<Units>
				{
					unit
				}, unit);
			}
		}

		private void P2C_Restore(MobaMessage msg)
		{
			P2CRestoreData probufMsg = msg.GetProbufMsg<P2CRestoreData>();
			int unitId = probufMsg.unitId;
			float hp = probufMsg.hp;
			float mp = probufMsg.mp;
			Units unit = MapManager.Instance.GetUnit(unitId);
			if (unit != null)
			{
				ActionManager.DataRevert(unit, hp, mp);
			}
		}

		private void P2C_NotifyTeamPos(MobaMessage msg)
		{
			NotifyTeamPos probufMsg = msg.GetProbufMsg<NotifyTeamPos>();
			TeamSignalManager.Process(probufMsg);
		}

		private void P2C_NotifyTeamTarget(MobaMessage msg)
		{
			NotifyTeamTarget probufMsg = msg.GetProbufMsg<NotifyTeamTarget>();
			TeamSignalManager.Process(probufMsg);
		}

		private void P2C_QueryUnit(MobaMessage msg)
		{
			P2CQueryUnit probufMsg = msg.GetProbufMsg<P2CQueryUnit>();
			UnitRuntimeInfo unitRuntimeInfo = probufMsg.unitRuntimeInfo;
			int unitId = probufMsg.unitId;
			if (unitRuntimeInfo != null)
			{
				PvpProtocolTools.SyncSingleUnit(unitRuntimeInfo);
			}
			else
			{
				Units unit = MapManager.Instance.GetUnit(unitId);
				if (unit)
				{
					PvpProtocolTools.ToDie(unit, null, 9223372036854775807L);
				}
			}
		}

		private void P2C_QueryInFightInfo(MobaMessage msg)
		{
			InBattleRuntimeInfo probufMsg = msg.GetProbufMsg<InBattleRuntimeInfo>();
			PvpStateBase.LogState("receive P2C_QueryInFightInfo " + StringUtils.DumpObject(probufMsg));
			FrameSyncManager.Instance.ResetInfoOnOnBattleStart();
			if (probufMsg == null)
			{
				PvpStateBase.LogState("no fight info");
			}
			else
			{
				if (probufMsg.roomState == 3)
				{
					this.ConfirmQuiting();
					return;
				}
				PvpProtocolTools.SyncFightInfo(probufMsg);
			}
		}

		private void P2C_FlashTo(MobaMessage msg)
		{
			FlashToInfo probufMsg = msg.GetProbufMsg<FlashToInfo>();
			Units unit = MapManager.Instance.GetUnit(probufMsg.unitId);
			if (unit != null)
			{
				if (unit != null && unit.OnPvpServerMsg(msg))
				{
					return;
				}
				if (Singleton<PvpManager>.Instance.IsInPvp)
				{
					if (unit.moveController == null)
					{
						return;
					}
					unit.moveController.PauseMoveInPvp = false;
				}
				unit.serverPos = MoveController.SVectgor3ToVector3(probufMsg.pos);
				unit.serverPos.y = unit.mTransform.position.y;
				if (unit.npc_id == "Boxxinshi")
				{
					unit.transform.LookAt(probufMsg.pos.ToVector3());
				}
				if (unit.moveController != null)
				{
					Units unit2 = MapManager.Instance.GetUnit(probufMsg.targUnitId);
					if (unit2 != null)
					{
						unit.moveController.SpecialMove(unit2, probufMsg.stopDist, probufMsg.speed);
					}
					else
					{
						unit.moveController.SpecialMove(unit.serverPos, probufMsg.speed);
					}
				}
			}
		}

		private void P2C_BuyItem(MobaMessage msg)
		{
		}

		private void P2C_SellItem(MobaMessage msg)
		{
		}

		private void P2C_RemoveItem(MobaMessage msg)
		{
			P2CRemoveItem probufMsg = msg.GetProbufMsg<P2CRemoveItem>();
			int unitId = probufMsg.unitId;
			int itemoid = probufMsg.itemoid;
			Units unit = MapManager.Instance.GetUnit(unitId);
			if (unit)
			{
				((Hero)unit).EquipPackage.RemoveEquip(itemoid);
			}
		}

		private void P2C_AddItem(MobaMessage msg)
		{
			P2CAddItem probufMsg = msg.GetProbufMsg<P2CAddItem>();
			int unitId = probufMsg.unitId;
			ItemDynData data = probufMsg.data;
			Units unit = MapManager.Instance.GetUnit(unitId);
			if (unit)
			{
				((Hero)unit).EquipPackage.AddEquip(data);
			}
		}

		private void P2C_UpdateItem(MobaMessage msg)
		{
			P2CUpdateItem probufMsg = msg.GetProbufMsg<P2CUpdateItem>();
			int unitId = probufMsg.unitId;
			ItemDynData itemdata = probufMsg.itemdata;
			Units unit = MapManager.Instance.GetUnit(unitId);
			if (unit)
			{
				((Hero)unit).EquipPackage.UpdateEquip(itemdata);
			}
		}

		private void P2C_SynSkillInfo(MobaMessage msg)
		{
			SynSkillInfo probufMsg = msg.GetProbufMsg<SynSkillInfo>();
			Units unit = MapManager.Instance.GetUnit((int)probufMsg.unitId);
			if (unit != null)
			{
				Skill skillById = unit.getSkillById(probufMsg.skillId);
				if (skillById != null)
				{
					skillById.SynInfo(probufMsg);
				}
			}
		}

		private void P2C_StartSkillInfo(MobaMessage msg)
		{
			StartCDInfo probufMsg = msg.GetProbufMsg<StartCDInfo>();
			Units unit = MapManager.Instance.GetUnit(probufMsg.unitId);
			if (unit != null)
			{
				Skill skillById = unit.getSkillById(probufMsg.skillId);
				if (skillById != null)
				{
					skillById.StartCDTime(probufMsg.CD, true);
				}
			}
		}

		private void P2C_AIDebugInfo(MobaMessage msg)
		{
			AIDebugInfo probufMsg = msg.GetProbufMsg<AIDebugInfo>();
			Units unit = MapManager.Instance.GetUnit(probufMsg.unitId);
			if (null == unit)
			{
				return;
			}
			unit.ShowAiState(probufMsg.state);
		}

		private void P2C_DebugCollider(MobaMessage msg)
		{
			GameObject gameObject = GameObject.Find("GlobalObject/Tools");
			if (gameObject != null)
			{
				ServerColliderManager component = gameObject.GetComponent<ServerColliderManager>();
				if (component == null)
				{
					gameObject.AddComponent<ServerColliderManager>();
				}
				if (component != null)
				{
					ColliderInfo probufMsg = msg.GetProbufMsg<ColliderInfo>();
					component.DebugCollider(probufMsg);
				}
			}
		}

		public override void OnUpdate()
		{
			if (DateTime.Now.Ticks - this.lastTimeCheckNetworkStatus > 100000000L)
			{
				this.lastTimeCheckNetworkStatus = DateTime.Now.Ticks;
				if (NetWorkHelper.Instance != null && NetWorkHelper.Instance.client != null && NetWorkHelper.Instance.client.pvpserver_peer != null)
				{
					MobaPvpServerClientPeer pvpserver_peer = NetWorkHelper.Instance.client.pvpserver_peer;
					int windowSize = 0;
					if (NetWorkHelper.Instance.client.timeSyncSystem != null)
					{
						windowSize = (int)NetWorkHelper.Instance.client.timeSyncSystem.ExtraDelayTime;
					}
					NetworkStatusInfo data = new NetworkStatusInfo
					{
						packageLossByCrc = pvpserver_peer.PacketLossByCrc,
						resentReliableCommands = pvpserver_peer.ResentReliableCommands,
						roundTripTime = pvpserver_peer.RoundTripTime,
						roundTripTimeVariance = pvpserver_peer.RoundTripTimeVariance,
						windowSize = windowSize
					};
					SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_NetworkStatus, SerializeHelper.Serialize<NetworkStatusInfo>(data));
				}
			}
			base.OnUpdate();
		}

		private bool IsNormalSkillOutOfView(Skill skill, Units targetUnits)
		{
			return skill != null && !skill.IsSkill && !targetUnits.isVisibleInCamera;
		}
	}
}
