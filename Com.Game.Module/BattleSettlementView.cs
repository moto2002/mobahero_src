using Assets.Scripts.GUILogic.View.BattleSettlement;
using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using GameLogin.State;
using GUIFramework;
using MobaHeros.Pvp;
using MobaHeros.Pvp.State;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class BattleSettlementView : BaseView<BattleSettlementView>
	{
		private Settlement_Common comp_common;

		private Settlement_Achievement comp_achievement;

		private Settlement_Rank comp_rank;

		private Settlement_Summoner comp_summoner;

		private Settlement_PVE comp_pve;

		private Settlement_NormalRewards comp_normal;

		private Settlement_BattleInfo comp_info;

		private Settlement_Surprise comp_surprise;

		private Settlement_Chaos comp_chaos;

		private List<SettlementPeriod> mDisplayOrder = new List<SettlementPeriod>();

		private CoroutineManager cMgr = new CoroutineManager();

		private bool isVictory = true;

		private int battleType;

		private bool is30Level;

		private bool isReplay;

		private bool orderSetFlag;

		private int clickCount;

		private bool clickAgainFlag;

		public bool IsReplay
		{
			get
			{
				return this.isReplay;
			}
			set
			{
				this.isReplay = value;
			}
		}

		public BattleSettlementView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Victory/BattleSettlementView");
		}

		public override void Init()
		{
			base.Init();
			Transform transform = this.transform.Find("CommonAnchor");
			this.comp_common = transform.gameObject.AddComponent<Settlement_Common>();
			transform = this.transform.Find("AchievementAnchor");
			this.comp_achievement = transform.gameObject.AddComponent<Settlement_Achievement>();
			transform = this.transform.Find("RankAnchor");
			this.comp_rank = transform.gameObject.AddComponent<Settlement_Rank>();
			transform = this.transform.Find("SummonerExpAnchor");
			this.comp_summoner = transform.gameObject.AddComponent<Settlement_Summoner>();
			transform = this.transform.Find("PvEStarAnchor");
			this.comp_pve = transform.gameObject.AddComponent<Settlement_PVE>();
			transform = this.transform.Find("NormalRewardsAnchor");
			this.comp_normal = transform.gameObject.AddComponent<Settlement_NormalRewards>();
			transform = this.transform.Find("BattleInfoAnchor");
			this.comp_info = transform.gameObject.AddComponent<Settlement_BattleInfo>();
			transform = this.transform.Find("SurpriseAnchor");
			this.comp_surprise = transform.gameObject.AddComponent<Settlement_Surprise>();
			transform = this.transform.Find("ChaosAnchor");
			this.comp_chaos = transform.gameObject.AddComponent<Settlement_Chaos>();
		}

		public override void HandleAfterOpenView()
		{
			this.orderSetFlag = false;
			this.clickCount = 0;
			this.mDisplayOrder.Clear();
			this.cMgr.StartCoroutine(this.checkDataReady(), true);
		}

		public override void HandleBeforeCloseView()
		{
			ModelManager.Instance.Clear_SettlementData();
		}

		public override void RegisterUpdateHandler()
		{
			MobaMessageManager.RegistMessage((ClientMsg)21025, new MobaMessageFunc(this.onMsg_ClickContinue));
			MobaMessageManager.RegistMessage((ClientMsg)21032, new MobaMessageFunc(this.onMsg_ForceContinue));
			MobaMessageManager.RegistMessage((ClientMsg)21026, new MobaMessageFunc(this.onMsg_ClickBackToLobby));
			MobaMessageManager.RegistMessage((ClientMsg)21027, new MobaMessageFunc(this.onMsg_ClickTryAgain));
		}

		public override void CancelUpdateHandler()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)21025, new MobaMessageFunc(this.onMsg_ClickContinue));
			MobaMessageManager.UnRegistMessage((ClientMsg)21032, new MobaMessageFunc(this.onMsg_ForceContinue));
			MobaMessageManager.UnRegistMessage((ClientMsg)21026, new MobaMessageFunc(this.onMsg_ClickBackToLobby));
			MobaMessageManager.UnRegistMessage((ClientMsg)21027, new MobaMessageFunc(this.onMsg_ClickTryAgain));
		}

		public override void Destroy()
		{
			this.cMgr.StopAllCoroutine();
			base.Destroy();
		}

		public void BackToLobby()
		{
			if (!NetWorkHelper.Instance.IsGateAvailable)
			{
				this.ReConnectGameServer();
			}
			GameManager.SetGameState(GameState.Game_Exit);
			PvpStateManager.Instance.ChangeState(new PvpStateHome());
		}

		private void BattleAgain()
		{
			Singleton<PvpRoomView>.Instance.SetLevelStorage(ModelManager.Instance.Get_Settle_PvpTeamInfo());
			if (NetWorkHelper.Instance.IsGateAvailable)
			{
				GameManager.SetGameState(GameState.Game_Exit);
				PvpStateManager.Instance.ChangeState(new PvpStateHome());
			}
			else
			{
				this.ReConnectGameServer();
			}
		}

		private void SetDisplayOrder()
		{
			if (this.orderSetFlag)
			{
				return;
			}
			this.mDisplayOrder.Clear();
			if (this.IsReplay)
			{
				return;
			}
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				if (LevelManager.m_CurLevel.Is3V3V3())
				{
					this.mDisplayOrder.Add(SettlementPeriod.Settlement_Chaos);
				}
				else
				{
					this.mDisplayOrder.Add(SettlementPeriod.Settlement_BattleInfo);
				}
				return;
			}
			int num = this.battleType;
			if (num != 1 && num != 2)
			{
				if (num != 11)
				{
					if (num == 12)
					{
						if (LevelManager.m_CurLevel.IsRank())
						{
							this.mDisplayOrder.Add(SettlementPeriod.Settlement_Rank);
						}
						if (!this.is30Level)
						{
							this.mDisplayOrder.Add(SettlementPeriod.Settlement_Summoner);
						}
						this.mDisplayOrder.Add(SettlementPeriod.Settlement_NormalRewards);
						if (this.GetAchievementList().Count > 0)
						{
							this.mDisplayOrder.Add(SettlementPeriod.Settlement_Achievement);
						}
						if (LevelManager.m_CurLevel.Is3V3V3())
						{
							this.mDisplayOrder.Add(SettlementPeriod.Settlement_Chaos);
						}
						else
						{
							this.mDisplayOrder.Add(SettlementPeriod.Settlement_BattleInfo);
						}
					}
				}
				else
				{
					this.mDisplayOrder.Add(SettlementPeriod.Settlement_BattleInfo);
				}
			}
			else
			{
				if (this.isVictory)
				{
					this.mDisplayOrder.Add(SettlementPeriod.Settlement_PVE);
				}
				if (!this.is30Level)
				{
					this.mDisplayOrder.Add(SettlementPeriod.Settlement_Summoner);
				}
				this.mDisplayOrder.Add(SettlementPeriod.Settlement_NormalRewards);
				this.mDisplayOrder.Add(SettlementPeriod.Settlement_BattleInfo);
			}
		}

		private void ReadyForDisplay()
		{
			this.isVictory = GameManager.IsVictory.Value;
			this.battleType = LevelManager.CurBattleType;
			this.is30Level = (ModelManager.Instance.Get_Settle_SummonerExpLevel() == 30);
			this.SetDisplayOrder();
			this.comp_common.gameObject.SetActive(true);
			MobaMessageManagerTools.Settle_showCommon(this.isVictory);
			if (this.isVictory)
			{
				BgmPlayer.PlayWinBG();
			}
			else
			{
				BgmPlayer.PlayLoseBG();
			}
		}

		[DebuggerHidden]
		private IEnumerator checkDataReady()
		{
			BattleSettlementView.<checkDataReady>c__IteratorFF <checkDataReady>c__IteratorFF = new BattleSettlementView.<checkDataReady>c__IteratorFF();
			<checkDataReady>c__IteratorFF.<>f__this = this;
			return <checkDataReady>c__IteratorFF;
		}

		private void ReConnectGameServer()
		{
			NetWorkHelper.Instance.ConnectToGateServer();
		}

		private List<Settlement_Achievement.EAchievementMedalType> GetAchievementList()
		{
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurLevelId);
			if (dataById == null || dataById.isinfightability != 1)
			{
				return new List<Settlement_Achievement.EAchievementMedalType>();
			}
			PvpTeamInfo pvpTeamInfo = ModelManager.Instance.Get_Settle_PvpTeamInfo();
			string userId = ModelManager.Instance.Get_userData_X().UserId;
			PlayerCounter playerCounter = null;
			if (pvpTeamInfo != null)
			{
				pvpTeamInfo.unitCounters.TryGetValue(userId, out playerCounter);
			}
			List<Settlement_Achievement.EAchievementMedalType> list = new List<Settlement_Achievement.EAchievementMedalType>();
			if (playerCounter.extKillCount != null)
			{
				if (playerCounter.extKillCount.ContainsKey(2))
				{
					list.Add(Settlement_Achievement.EAchievementMedalType.DoubleKill);
				}
				if (playerCounter.extKillCount.ContainsKey(3))
				{
					list.Add(Settlement_Achievement.EAchievementMedalType.TripleKill);
				}
				if (playerCounter.extKillCount.ContainsKey(4))
				{
					list.Add(Settlement_Achievement.EAchievementMedalType.QuadraKill);
				}
				if (playerCounter.extKillCount.ContainsKey(5))
				{
					list.Add(Settlement_Achievement.EAchievementMedalType.PentaKill);
				}
				if (playerCounter.extKillCount.ContainsKey(104))
				{
					list.Add(Settlement_Achievement.EAchievementMedalType.Legendray);
				}
				if (ModelManager.Instance.Get_Settle_ImMvp())
				{
					list.Add(Settlement_Achievement.EAchievementMedalType.MVP);
				}
			}
			return list;
		}

		private void onMsg_ClickContinue(MobaMessage msg)
		{
			SettlementPeriod settlementPeriod = SettlementPeriod.Settlement_Common;
			if (this.mDisplayOrder.Count != 0 && this.mDisplayOrder.Count > this.clickCount)
			{
				settlementPeriod = this.mDisplayOrder[this.clickCount];
			}
			switch (settlementPeriod)
			{
			case SettlementPeriod.Settlement_Summoner:
				if (this.comp_pve.gameObject.activeInHierarchy)
				{
					this.comp_pve.gameObject.SetActive(false);
				}
				if (this.comp_rank.gameObject.activeInHierarchy)
				{
					this.comp_rank.gameObject.SetActive(false);
				}
				this.comp_summoner.gameObject.SetActive(true);
				MobaMessageManagerTools.Settle_showSummonerExp();
				goto IL_2C6;
			case SettlementPeriod.Settlement_PVE:
				if (this.comp_rank.gameObject.activeInHierarchy)
				{
					this.comp_rank.gameObject.SetActive(false);
				}
				this.comp_pve.gameObject.SetActive(true);
				MobaMessageManagerTools.Settle_showPve();
				goto IL_2C6;
			case SettlementPeriod.Settlement_NormalRewards:
				if (this.comp_rank.gameObject.activeInHierarchy)
				{
					this.comp_rank.gameObject.SetActive(false);
				}
				if (this.comp_pve.gameObject.activeInHierarchy)
				{
					this.comp_pve.gameObject.SetActive(false);
				}
				if (this.comp_summoner.gameObject.activeInHierarchy)
				{
					this.comp_summoner.gameObject.SetActive(false);
				}
				this.comp_normal.gameObject.SetActive(true);
				MobaMessageManagerTools.Settle_showNormal(this.isVictory);
				goto IL_2C6;
			case SettlementPeriod.Settlement_BattleInfo:
				this.comp_normal.gameObject.SetActive(false);
				this.comp_info.gameObject.SetActive(true);
				if (this.comp_achievement.gameObject.activeInHierarchy)
				{
					this.comp_achievement.gameObject.SetActive(false);
				}
				MobaMessageManagerTools.Settle_showInfo(this.battleType);
				goto IL_2C6;
			case SettlementPeriod.Settlement_Rank:
				this.comp_rank.gameObject.SetActive(true);
				MobaMessageManagerTools.Settle_showRank();
				goto IL_2C6;
			case SettlementPeriod.Settlement_Chaos:
				this.comp_normal.gameObject.SetActive(false);
				this.comp_chaos.gameObject.SetActive(true);
				if (this.comp_achievement.gameObject.activeInHierarchy)
				{
					this.comp_achievement.gameObject.SetActive(false);
				}
				MobaMessageManagerTools.Settle_showChaosInfo();
				goto IL_2C6;
			case SettlementPeriod.Settlement_Achievement:
				this.comp_normal.gameObject.SetActive(false);
				this.comp_achievement.gameObject.SetActive(true);
				MobaMessageManagerTools.Settle_showAchievement(this.GetAchievementList());
				goto IL_2C6;
			}
			if (this.clickAgainFlag)
			{
				this.BattleAgain();
			}
			else
			{
				this.BackToLobby();
			}
			IL_2C6:
			this.clickCount++;
		}

		private void onMsg_ForceContinue(MobaMessage msg)
		{
			this.onMsg_ClickContinue(null);
		}

		private void onMsg_ClickBackToLobby(MobaMessage msg)
		{
			bool flag = true;
			this.clickAgainFlag = false;
			if (flag)
			{
				this.comp_info.gameObject.SetActive(false);
				this.comp_surprise.gameObject.SetActive(true);
				Singleton<GetItemsView>.Instance.onFinish = new Callback(this.BackToLobby);
				MobaMessageManagerTools.Settle_showSurprise();
			}
			else
			{
				this.BackToLobby();
			}
		}

		private void onMsg_ClickTryAgain(MobaMessage msg)
		{
			bool flag = true;
			this.clickAgainFlag = true;
			if (flag)
			{
				this.comp_info.gameObject.SetActive(false);
				this.comp_surprise.gameObject.SetActive(true);
				Singleton<GetItemsView>.Instance.onFinish = new Callback(this.BattleAgain);
				MobaMessageManagerTools.Settle_showSurprise();
			}
			else
			{
				this.BattleAgain();
			}
		}

		public void SetResult(bool isPass)
		{
			this.isVictory = isPass;
		}

		public void ExternalCall()
		{
			this.mDisplayOrder.Clear();
			this.isVictory = true;
			this.gameObject.SetActive(true);
		}

		public void NewbieDestroyResultEffect()
		{
			if (this.comp_common != null)
			{
				this.comp_common.NewbieDestroyResultEffect();
			}
		}
	}
}
