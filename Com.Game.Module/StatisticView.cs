using Assets.Scripts.GUILogic.View.BattleEquipment;
using Com.Game.Utils;
using GUIFramework;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using Newbie;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class StatisticView : BaseView<StatisticView>
	{
		private Transform BackBtn;

		private Transform HeroDataPanel;

		private string LMName = string.Empty;

		private string BLName = string.Empty;

		private UILabel LMClanName;

		private UILabel BLClanName;

		private UILabel LMKillNum;

		private UILabel BLKillNum;

		private UILabel LMTowerLabel;

		private UILabel BLTowerLabel;

		private UILabel LMBossLabel;

		private UILabel BLBossLabel;

		private Dictionary<string, StatisticItem> statisticItemList = new Dictionary<string, StatisticItem>();

		private int scoreLM;

		private int scoreBL;

		private bool isNeedUpdate;

		private Task updateTask;

		private StatisticItem StatisticItemLM;

		private StatisticItem StatisticItemBL;

		private UIGrid PVP_LeftGrid;

		private UIGrid PVP_RightGrid;

		private List<HeroData> LMHeroDataList;

		private List<HeroData> BLHeroDataList;

		private List<VictPlayerData> LMVictPlayerDataList;

		private List<VictPlayerData> BLVictPlayerDataList;

		private int totalKillHeroLM;

		private int totalKillHeroBL;

		private int totalKillAssistLM;

		private int totalKillAssistBL;

		private int totalDeathLM;

		private int totalDeathBL;

		private Transform BackSprite;

		private ReadyPlayerSampleInfo tempInfo;

		private Dictionary<int, StatisticData> herosEpuipDictionary;

		private StatisticData statisticData = default(StatisticData);

		public List<string> blockedSummonerList = new List<string>();

		private VictPlayerType witchSide;

		public StatisticView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "StatisticView");
		}

		public override void Init()
		{
			base.Init();
			this.BackSprite = this.transform.Find("UIPanel/Btn/BackSprite");
			this.BackBtn = this.transform.Find("UIPanel/Btn/BackBtn");
			this.HeroDataPanel = this.transform.Find("HeroDataPanel");
			this.LMKillNum = this.transform.Find("UIPanel/Left/Data").GetComponent<UILabel>();
			this.BLKillNum = this.transform.Find("UIPanel/Right/Data").GetComponent<UILabel>();
			this.LMTowerLabel = this.transform.Find("UIPanel/Left/TowerLabel").GetComponent<UILabel>();
			this.BLTowerLabel = this.transform.Find("UIPanel/Right/TowerLabel").GetComponent<UILabel>();
			this.LMBossLabel = this.transform.Find("UIPanel/Left/BossLabel").GetComponent<UILabel>();
			this.BLBossLabel = this.transform.Find("UIPanel/Right/BossLabel").GetComponent<UILabel>();
			this.LMClanName = this.transform.Find("UIPanel/Left/ClanName").GetComponent<UILabel>();
			this.BLClanName = this.transform.Find("UIPanel/Right/ClanName").GetComponent<UILabel>();
			this.StatisticItemLM = Resources.Load<StatisticItem>("Prefab/UI/Battle/StatisticItemLM");
			this.StatisticItemBL = Resources.Load<StatisticItem>("Prefab/UI/Battle/StatisticItemBL");
			this.PVP_LeftGrid = this.transform.Find("UIPanel/Left/Grid").GetComponent<UIGrid>();
			this.PVP_RightGrid = this.transform.Find("UIPanel/Right/Grid").GetComponent<UIGrid>();
		}

		public void SaveHerosEquip(Dictionary<int, StatisticData> _herosEpuipDictionary)
		{
			this.herosEpuipDictionary = _herosEpuipDictionary;
		}

		public override void HandleAfterOpenView()
		{
			if (Singleton<HUDModuleManager>.Instance.gameObject)
			{
				Singleton<HUDModuleManager>.Instance.FlyOut();
			}
			if (null != Singleton<BattleSettlementView>.Instance.gameObject)
			{
				this.BackSprite.transform.GetComponent<UISprite>().color = new Color32(0, 37, 42, 247);
			}
			else
			{
				this.BackSprite.transform.GetComponent<UISprite>().color = new Color32(0, 37, 42, 1);
			}
			NewbieManager.Instance.TryHandleOpenStatistic();
		}

		public override void HandleBeforeCloseView()
		{
			if (Singleton<HUDModuleManager>.Instance.gameObject)
			{
				Singleton<HUDModuleManager>.Instance.FlyIn();
			}
			NewbieManager.Instance.TryHandleCloseStatistic();
		}

		public override void RegisterUpdateHandler()
		{
			UIEventListener expr_10 = UIEventListener.Get(this.BackSprite.gameObject);
			expr_10.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_10.onClick, new UIEventListener.VoidDelegate(this.Close_Statistic));
			UIEventListener expr_41 = UIEventListener.Get(this.BackBtn.gameObject);
			expr_41.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_41.onClick, new UIEventListener.VoidDelegate(this.Close_Statistic));
			this.RefreshUI();
			if (Singleton<SkillView>.Instance.gameObject != null)
			{
				Singleton<SkillView>.Instance.FlyOut();
			}
		}

		public override void CancelUpdateHandler()
		{
			UIEventListener expr_10 = UIEventListener.Get(this.BackSprite.gameObject);
			expr_10.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(expr_10.onClick, new UIEventListener.VoidDelegate(this.Close_Statistic));
			UIEventListener expr_41 = UIEventListener.Get(this.BackBtn.gameObject);
			expr_41.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(expr_41.onClick, new UIEventListener.VoidDelegate(this.Close_Statistic));
			if (Singleton<SkillView>.Instance.gameObject != null && Singleton<BattleSettlementView>.Instance.gameObject == null)
			{
				Singleton<SkillView>.Instance.FlyIn();
			}
		}

		public override void RefreshUI()
		{
			this.UpdateStatisticView();
			if (this.isNeedUpdate)
			{
				if (this.updateTask != null)
				{
					this.updateTask.Stop();
				}
				this.updateTask = new Task(this.UpdateInfoTask(), true);
				this.updateTask.Start();
			}
		}

		public void SetModel(bool isNeedUpdate = false)
		{
			this.isNeedUpdate = isNeedUpdate;
		}

		[DebuggerHidden]
		private IEnumerator UpdateInfoTask()
		{
			StatisticView.<UpdateInfoTask>c__IteratorEF <UpdateInfoTask>c__IteratorEF = new StatisticView.<UpdateInfoTask>c__IteratorEF();
			<UpdateInfoTask>c__IteratorEF.<>f__this = this;
			return <UpdateInfoTask>c__IteratorEF;
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void UpdateStatisticView()
		{
			if (GameManager.Instance != null)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				if (this.LMName == string.Empty)
				{
					this.LMClanName.text = "我方";
				}
				if (this.BLName == string.Empty)
				{
					this.BLClanName.text = "敌方";
				}
				if (GameManager.IsGameOver())
				{
					BattleAttrManager.Instance.ClearDeathLastTime();
				}
				this.statisticItemList = new Dictionary<string, StatisticItem>();
				this.LMHeroDataList = new List<HeroData>();
				this.BLHeroDataList = new List<HeroData>();
				this.LMVictPlayerDataList = new List<VictPlayerData>();
				this.BLVictPlayerDataList = new List<VictPlayerData>();
				this.totalKillHeroLM = 0;
				this.totalKillHeroBL = 0;
				this.totalKillAssistLM = 0;
				this.totalKillAssistBL = 0;
				this.totalDeathLM = 0;
				this.totalDeathBL = 0;
				if (GameManager.Instance.DamageStatisticalManager.UnitsDictionary == null)
				{
					ClientLogger.Error("==> StatisticView   GameManager.Instance.DamageStatisticalManager.UnitsDictionary==null");
					return;
				}
				foreach (int current in GameManager.Instance.DamageStatisticalManager.UnitsDictionary.Keys)
				{
					if (GameManager.Instance.DamageStatisticalManager.UnitsDictionary.ContainsKey(current))
					{
						if ((!(GameManager.Instance.DamageStatisticalManager.UnitsDictionary[current].tag != "Hero") || !(GameManager.Instance.DamageStatisticalManager.UnitsDictionary[current].tag != "Player")) && !GameManager.Instance.DamageStatisticalManager.UnitsDictionary[current].MirrorState)
						{
							Units units = GameManager.Instance.DamageStatisticalManager.UnitsDictionary[current];
							if (units == null && this.herosEpuipDictionary != null)
							{
								this.statisticData = this.herosEpuipDictionary[current];
							}
							if (LevelManager.Instance.IsPvpBattleType)
							{
								int unique_id = units.unique_id;
								PvpStatisticMgr.HeroData heroData = Singleton<PvpManager>.Instance.StatisticMgr.GetHeroData(unique_id);
								if (heroData == null)
								{
									return;
								}
								if (GameManager.Instance.DamageStatisticalManager.UnitsDictionary[current].TeamType == TeamType.LM)
								{
									this.LMHeroDataList.Add(new HeroData
									{
										LV = UtilManager.Instance.GetHerolv(units.unique_id),
										Quality = units.quality,
										Stars = units.star,
										HeroID = units.npc_id,
										Unique_id = units.unique_id
									});
									this.tempInfo = Singleton<PvpManager>.Instance.GetPlayersByTeam(TeamType.LM).Find((ReadyPlayerSampleInfo obj) => obj.newUid.ToString() == Math.Abs(units.unique_id).ToString());
									if (this.tempInfo == null)
									{
										this.tempInfo = new ReadyPlayerSampleInfo();
									}
									this.LMVictPlayerDataList.Add(new VictPlayerData
									{
										KillHero = heroData.HeroKill,
										Death = heroData.Death,
										KillAssist = heroData.Assist,
										KillMonster = heroData.MonsterKill,
										Gold = heroData.CurGold,
										FirstKill = heroData.FirstKill,
										SummonerName = (!(this.tempInfo.userName == string.Empty)) ? this.tempInfo.userName : this.statisticData.summerName,
										SummonerId = this.tempInfo.SummerId.ToString(),
										SelfIsDeath = !units.isLive,
										LeftDeathTime = (int)BattleAttrManager.Instance.GetPlayerDeathLastTime(units.unique_id),
										SummonerSkillID = Singleton<PvpManager>.Instance.GetSummonerSkillId(units.unique_id),
										IsControlPlayer = (!(units == null)) ? units.isPlayer : this.statisticData.isPlayer,
										HeroEquipmentList = (!(units == null)) ? BattleEquipTools_op.GetHeroItemsString(units) : this.statisticData.EquipItems,
										NPCID = units.npc_id,
										LastChamRank = this.tempInfo.CharmRankvalue
									});
									num2++;
									this.totalKillHeroLM += heroData.HeroKill;
									this.totalKillAssistLM += units.assistantNum;
									this.totalDeathLM += heroData.Death;
								}
								else
								{
									this.BLHeroDataList.Add(new HeroData
									{
										LV = UtilManager.Instance.GetHerolv(units.unique_id),
										Quality = units.quality,
										Stars = units.star,
										HeroID = units.npc_id,
										Unique_id = units.unique_id
									});
									this.tempInfo = Singleton<PvpManager>.Instance.GetPlayersByTeam(TeamType.BL).Find((ReadyPlayerSampleInfo obj) => obj.newUid.ToString() == Math.Abs(units.unique_id).ToString());
									if (this.tempInfo == null)
									{
										this.tempInfo = new ReadyPlayerSampleInfo();
									}
									this.BLVictPlayerDataList.Add(new VictPlayerData
									{
										KillHero = heroData.HeroKill,
										Death = heroData.Death,
										KillAssist = heroData.Assist,
										KillMonster = heroData.MonsterKill,
										Gold = heroData.CurGold,
										FirstKill = heroData.FirstKill,
										SummonerName = (!(this.tempInfo.userName == string.Empty)) ? this.tempInfo.userName : this.statisticData.summerName,
										SummonerId = this.tempInfo.SummerId.ToString(),
										SelfIsDeath = !units.isLive,
										LeftDeathTime = (int)BattleAttrManager.Instance.GetPlayerDeathLastTime(units.unique_id),
										SummonerSkillID = Singleton<PvpManager>.Instance.GetSummonerSkillId(units.unique_id),
										IsControlPlayer = (!(units == null)) ? units.isPlayer : this.statisticData.isPlayer,
										HeroEquipmentList = (!(units == null)) ? BattleEquipTools_op.GetHeroItemsString(units) : this.statisticData.EquipItems,
										NPCID = units.npc_id,
										LastChamRank = this.tempInfo.CharmRankvalue
									});
									num3++;
									this.totalKillHeroBL += heroData.HeroKill;
									this.totalKillAssistBL += units.assistantNum;
									this.totalDeathBL += heroData.Death;
								}
							}
							else
							{
								AchieveData achieveData = GameManager.Instance.AchieveManager.GetAchieveData(units.unique_id, units.teamType);
								if (GameManager.Instance.DamageStatisticalManager.UnitsDictionary[current].TeamType == TeamType.LM)
								{
									if (achieveData == null)
									{
										achieveData = new AchieveData(0, string.Empty, TeamType.LM);
									}
									this.LMHeroDataList.Add(new HeroData
									{
										LV = UtilManager.Instance.GetHerolv(units.unique_id),
										Quality = units.quality,
										Stars = units.star,
										HeroID = units.npc_id,
										Unique_id = units.unique_id
									});
									this.LMVictPlayerDataList.Add(new VictPlayerData
									{
										KillHero = achieveData.TotalKill,
										Death = achieveData.SelfDeathTime,
										KillAssist = units.assistantNum,
										KillMonster = achieveData.MonsterKillNum,
										SelfIsDeath = !units.isLive,
										LeftDeathTime = (int)BattleAttrManager.Instance.GetPlayerDeathLastTime(units.unique_id),
										SummonerSkillID = Singleton<PvpManager>.Instance.GetSummonerSkillId(units.unique_id),
										IsControlPlayer = units.isPlayer,
										HeroEquipmentList = BattleEquipTools_op.GetHeroItemsString(units),
										NPCID = units.npc_id
									});
									num2++;
									this.totalKillAssistLM += units.assistantNum;
									this.totalKillHeroLM += achieveData.TotalKill;
									this.totalDeathLM += achieveData.SelfDeathTime;
								}
								else
								{
									if (achieveData == null)
									{
										achieveData = new AchieveData(0, string.Empty, TeamType.BL);
									}
									this.BLHeroDataList.Add(new HeroData
									{
										LV = UtilManager.Instance.GetHerolv(units.unique_id),
										Quality = units.quality,
										Stars = units.star,
										HeroID = units.npc_id,
										Unique_id = units.unique_id
									});
									this.BLVictPlayerDataList.Add(new VictPlayerData
									{
										KillHero = achieveData.TotalKill,
										Death = achieveData.SelfDeathTime,
										KillAssist = units.assistantNum,
										KillMonster = achieveData.MonsterKillNum,
										SelfIsDeath = !units.isLive,
										LeftDeathTime = (int)BattleAttrManager.Instance.GetPlayerDeathLastTime(units.unique_id),
										SummonerSkillID = Singleton<PvpManager>.Instance.GetSummonerSkillId(units.unique_id),
										IsControlPlayer = units.isPlayer,
										HeroEquipmentList = BattleEquipTools_op.GetHeroItemsString(units),
										NPCID = units.npc_id
									});
									num3++;
									this.totalKillAssistBL += units.assistantNum;
									this.totalKillHeroBL += achieveData.TotalKill;
									this.totalDeathBL += achieveData.SelfDeathTime;
								}
							}
							num++;
						}
					}
				}
				if (LevelManager.Instance.IsPvpBattleType)
				{
					this.LMKillNum.text = string.Concat(new object[]
					{
						this.totalKillHeroLM,
						"/",
						this.totalDeathLM,
						"/",
						this.totalKillAssistLM
					});
					this.BLKillNum.text = string.Concat(new object[]
					{
						this.totalKillHeroBL,
						"/",
						this.totalDeathBL,
						"/",
						this.totalKillAssistBL
					});
					this.LMTowerLabel.text = UtilManager.Instance.GetTowerDestroyByTeam(TeamType.BL).ToString();
					this.BLTowerLabel.text = UtilManager.Instance.GetTowerDestroyByTeam(TeamType.LM).ToString();
					this.LMBossLabel.text = UtilManager.Instance.GetEpicMonsterKilling(TeamType.LM).ToString();
					this.BLBossLabel.text = UtilManager.Instance.GetEpicMonsterKilling(TeamType.BL).ToString();
					Color32 c = new Color32(225, 2, 0, 255);
					Color32 c2 = new Color32(253, 127, 0, 255);
					Color32 c3 = new Color32(0, 133, 254, 255);
					Color32 c4 = new Color32(0, 251, 228, 255);
					if (this.LMVictPlayerDataList.Find((VictPlayerData item) => item.IsControlPlayer) != null)
					{
						UILabel arg_D4B_0 = this.BLKillNum;
						Color color = c;
						this.BLBossLabel.gradientTop = color;
						color = color;
						this.BLTowerLabel.gradientTop = color;
						arg_D4B_0.gradientTop = color;
						UILabel arg_D7F_0 = this.BLKillNum;
						color = c2;
						this.BLBossLabel.gradientBottom = color;
						color = color;
						this.BLTowerLabel.gradientBottom = color;
						arg_D7F_0.gradientBottom = color;
						UILabel arg_DB3_0 = this.LMKillNum;
						color = c3;
						this.LMBossLabel.gradientTop = color;
						color = color;
						this.LMTowerLabel.gradientTop = color;
						arg_DB3_0.gradientTop = color;
						UILabel arg_DE7_0 = this.LMKillNum;
						color = c4;
						this.LMBossLabel.gradientBottom = color;
						color = color;
						this.LMTowerLabel.gradientBottom = color;
						arg_DE7_0.gradientBottom = color;
					}
					else
					{
						UILabel arg_E20_0 = this.BLKillNum;
						Color color = c3;
						this.BLBossLabel.gradientTop = color;
						color = color;
						this.BLTowerLabel.gradientTop = color;
						arg_E20_0.gradientTop = color;
						UILabel arg_E54_0 = this.BLKillNum;
						color = c4;
						this.BLBossLabel.gradientBottom = color;
						color = color;
						this.BLTowerLabel.gradientBottom = color;
						arg_E54_0.gradientBottom = color;
						UILabel arg_E88_0 = this.LMKillNum;
						color = c;
						this.LMBossLabel.gradientTop = color;
						color = color;
						this.LMTowerLabel.gradientTop = color;
						arg_E88_0.gradientTop = color;
						UILabel arg_EBC_0 = this.LMKillNum;
						color = c2;
						this.LMBossLabel.gradientBottom = color;
						color = color;
						this.LMTowerLabel.gradientBottom = color;
						arg_EBC_0.gradientBottom = color;
					}
				}
				else
				{
					this.LMKillNum.text = string.Concat(new object[]
					{
						this.totalKillHeroLM,
						"/",
						this.totalDeathLM,
						"/",
						this.totalKillAssistLM
					});
					this.BLKillNum.text = string.Concat(new object[]
					{
						this.totalKillHeroBL,
						"/",
						this.totalDeathBL,
						"/",
						this.totalKillAssistBL
					});
					this.LMTowerLabel.text = UtilManager.Instance.GetTowerDestroyByTeam(TeamType.BL).ToString();
					this.BLTowerLabel.text = UtilManager.Instance.GetTowerDestroyByTeam(TeamType.LM).ToString();
					this.LMBossLabel.text = UtilManager.Instance.GetEpicMonsterKilling(TeamType.LM).ToString();
					this.BLBossLabel.text = UtilManager.Instance.GetEpicMonsterKilling(TeamType.BL).ToString();
				}
				this.PVPVictoryView();
			}
		}

		public void ChangeHighestData(List<VictPlayerData> _victPlayerDataList, bool isLM, Dictionary<string, StatisticItem> _victoryPVPItemList = null)
		{
			if (_victoryPVPItemList == null)
			{
				_victoryPVPItemList = this.statisticItemList;
			}
			int value = 0;
			for (int i = 0; i < _victPlayerDataList.Count; i++)
			{
				if (value < _victPlayerDataList[i].KillHero)
				{
					value = _victPlayerDataList[i].KillHero;
				}
				_victPlayerDataList[i].isHighestKillHero = false;
			}
			List<VictPlayerData> list = _victPlayerDataList.FindAll((VictPlayerData obj) => obj.KillHero == value);
			for (int j = 0; j < list.Count; j++)
			{
				list[j].isHighestKillHero = true;
				if (_victoryPVPItemList != null)
				{
					if (isLM)
					{
						if (_victoryPVPItemList.ContainsKey(list[j].NPCID + "LM"))
						{
							_victoryPVPItemList[list[j].NPCID + "LM"].SetIsHighestKillHero(true);
						}
					}
					else if (_victoryPVPItemList.ContainsKey(list[j].NPCID + "BL"))
					{
						_victoryPVPItemList[list[j].NPCID + "BL"].SetIsHighestKillHero(true);
					}
				}
			}
			value = 0;
			for (int k = 0; k < _victPlayerDataList.Count; k++)
			{
				if (value < _victPlayerDataList[k].KillMonster)
				{
					value = _victPlayerDataList[k].KillMonster;
				}
				_victPlayerDataList[k].isHighestKillMonster = false;
			}
			list = _victPlayerDataList.FindAll((VictPlayerData obj) => obj.KillMonster == value);
			for (int l = 0; l < list.Count; l++)
			{
				list[l].isHighestKillMonster = true;
				if (_victoryPVPItemList != null)
				{
					if (isLM)
					{
						if (_victoryPVPItemList.ContainsKey(list[l].NPCID + "LM"))
						{
							_victoryPVPItemList[list[l].NPCID + "LM"].SetIsHighestKillMonster(true);
						}
					}
					else if (_victoryPVPItemList.ContainsKey(list[l].NPCID + "BL"))
					{
						_victoryPVPItemList[list[l].NPCID + "BL"].SetIsHighestKillMonster(true);
					}
				}
			}
		}

		public void PVPVictoryView()
		{
			this.ChangeHighestData(this.LMVictPlayerDataList, true, null);
			for (int i = 0; i < this.LMHeroDataList.Count; i++)
			{
				if (this.LMVictPlayerDataList[i].IsControlPlayer)
				{
					this.witchSide = VictPlayerType.LM;
					break;
				}
				this.witchSide = VictPlayerType.BL;
			}
			GridHelper.FillGrid<StatisticItem>(this.PVP_LeftGrid, this.StatisticItemLM, this.LMHeroDataList.Count, delegate(int idx, StatisticItem comp)
			{
				comp.Init(this.witchSide, this.LMHeroDataList[idx], VictPlayerType.LM, this.LMVictPlayerDataList[idx], this.LMVictPlayerDataList[idx].IsControlPlayer, false);
				if (this.statisticItemList.ContainsKey(this.LMHeroDataList[idx].HeroID + "LM"))
				{
					this.statisticItemList.Add(this.LMHeroDataList[idx].HeroID + idx.ToString() + "LM", comp);
				}
				else
				{
					this.statisticItemList.Add(this.LMHeroDataList[idx].HeroID + "LM", comp);
				}
				comp.SetAddFriendShow(false);
			});
			this.PVP_LeftGrid.Reposition();
			this.ChangeHighestData(this.BLVictPlayerDataList, false, null);
			GridHelper.FillGrid<StatisticItem>(this.PVP_RightGrid, this.StatisticItemBL, this.BLHeroDataList.Count, delegate(int idx, StatisticItem comp)
			{
				comp.Init(this.witchSide, this.BLHeroDataList[idx], VictPlayerType.BL, this.BLVictPlayerDataList[idx], this.BLVictPlayerDataList[idx].IsControlPlayer, false);
				if (this.statisticItemList.ContainsKey(this.BLHeroDataList[idx].HeroID + "BL"))
				{
					this.statisticItemList.Add(this.BLHeroDataList[idx].HeroID + idx.ToString() + "BL", comp);
				}
				else
				{
					this.statisticItemList.Add(this.BLHeroDataList[idx].HeroID + "BL", comp);
				}
				comp.SetAddFriendShow(false);
				comp.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			});
			this.PVP_RightGrid.Reposition();
		}

		private void UpdateStar(int number, Transform Star)
		{
			int childCount = Star.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = Star.GetChild(i);
				if (i <= number - 1)
				{
					child.gameObject.SetActive(true);
					child.localPosition = new Vector3((float)(-(float)(number - 1) * 43 / 2 + (i - 1) * 43), -126f, 0f);
				}
				else
				{
					child.gameObject.SetActive(false);
				}
			}
		}

		public void Close_Statistic(GameObject objct_1 = null)
		{
			if (this.updateTask != null)
			{
				this.updateTask.Stop();
			}
			CtrlManager.CloseWindow(WindowID.StatisticView);
		}

		private void UpdateInfo()
		{
			if (!GameManager.IsGameOver())
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				this.totalKillHeroLM = 0;
				this.totalKillHeroBL = 0;
				this.totalKillAssistLM = 0;
				this.totalKillAssistBL = 0;
				this.totalDeathLM = 0;
				this.totalDeathBL = 0;
				this.LMHeroDataList.Clear();
				this.BLHeroDataList.Clear();
				this.LMVictPlayerDataList.Clear();
				this.BLVictPlayerDataList.Clear();
				foreach (int current in GameManager.Instance.DamageStatisticalManager.UnitsDictionary.Keys)
				{
					if ((!(GameManager.Instance.DamageStatisticalManager.UnitsDictionary[current].tag != "Hero") || !(GameManager.Instance.DamageStatisticalManager.UnitsDictionary[current].tag != "Player")) && !GameManager.Instance.DamageStatisticalManager.UnitsDictionary[current].MirrorState)
					{
						Units units = GameManager.Instance.DamageStatisticalManager.UnitsDictionary[current];
						if (LevelManager.Instance.IsPvpBattleType)
						{
							int unique_id = units.unique_id;
							PvpStatisticMgr.HeroData heroData = Singleton<PvpManager>.Instance.StatisticMgr.GetHeroData(unique_id);
							if (heroData == null)
							{
								return;
							}
							if (GameManager.Instance.DamageStatisticalManager.UnitsDictionary[current].TeamType == TeamType.LM)
							{
								this.LMHeroDataList.Add(new HeroData
								{
									LV = UtilManager.Instance.GetHerolv(units.unique_id),
									Quality = units.quality,
									Stars = units.star,
									HeroID = units.npc_id,
									Unique_id = units.unique_id
								});
								this.tempInfo = Singleton<PvpManager>.Instance.GetPlayersByTeam(TeamType.LM).Find((ReadyPlayerSampleInfo obj) => obj.newUid.ToString() == Math.Abs(units.unique_id).ToString());
								if (this.tempInfo == null)
								{
									this.tempInfo = new ReadyPlayerSampleInfo();
								}
								this.LMVictPlayerDataList.Add(new VictPlayerData
								{
									KillHero = heroData.HeroKill,
									Death = heroData.Death,
									KillAssist = heroData.Assist,
									KillMonster = heroData.MonsterKill,
									Gold = heroData.CurGold,
									FirstKill = heroData.FirstKill,
									SummonerName = this.tempInfo.userName,
									SelfIsDeath = !units.isLive,
									LeftDeathTime = (int)BattleAttrManager.Instance.GetPlayerDeathLastTime(units.unique_id),
									SummonerSkillID = Singleton<PvpManager>.Instance.GetSummonerSkillId(units.unique_id),
									IsControlPlayer = units.isPlayer,
									HeroEquipmentList = BattleEquipTools_op.GetHeroItemsString(units),
									NPCID = units.npc_id,
									LastChamRank = this.tempInfo.CharmRankvalue
								});
								this.statisticItemList[units.npc_id + "LM"].UpdateInfo(this.LMHeroDataList[num2], VictPlayerType.LM, this.LMVictPlayerDataList[num2], this.LMVictPlayerDataList[num2].IsControlPlayer);
								this.statisticItemList[units.npc_id + "LM"].SetAddFriendShow(false);
								num2++;
								this.totalKillAssistLM += heroData.Assist;
								this.totalKillHeroLM += heroData.HeroKill;
								this.totalDeathLM += heroData.Death;
							}
							else
							{
								this.BLHeroDataList.Add(new HeroData
								{
									LV = UtilManager.Instance.GetHerolv(units.unique_id),
									Quality = units.quality,
									Stars = units.star,
									HeroID = units.npc_id,
									Unique_id = units.unique_id
								});
								this.tempInfo = Singleton<PvpManager>.Instance.GetPlayersByTeam(TeamType.BL).Find((ReadyPlayerSampleInfo obj) => obj.newUid.ToString() == Math.Abs(units.unique_id).ToString());
								if (this.tempInfo == null)
								{
									this.tempInfo = new ReadyPlayerSampleInfo();
								}
								this.BLVictPlayerDataList.Add(new VictPlayerData
								{
									KillHero = heroData.HeroKill,
									Death = heroData.Death,
									KillAssist = heroData.Assist,
									KillMonster = heroData.MonsterKill,
									Gold = heroData.CurGold,
									FirstKill = heroData.FirstKill,
									SummonerName = this.tempInfo.userName,
									SelfIsDeath = !units.isLive,
									LeftDeathTime = (int)BattleAttrManager.Instance.GetPlayerDeathLastTime(units.unique_id),
									SummonerSkillID = Singleton<PvpManager>.Instance.GetSummonerSkillId(units.unique_id),
									IsControlPlayer = units.isPlayer,
									HeroEquipmentList = BattleEquipTools_op.GetHeroItemsString(units),
									NPCID = units.npc_id,
									LastChamRank = this.tempInfo.CharmRankvalue
								});
								this.statisticItemList[units.npc_id + "BL"].UpdateInfo(this.BLHeroDataList[num3], VictPlayerType.BL, this.BLVictPlayerDataList[num3], this.BLVictPlayerDataList[num3].IsControlPlayer);
								this.statisticItemList[units.npc_id + "BL"].SetAddFriendShow(false);
								num3++;
								this.totalKillAssistBL += heroData.Assist;
								this.totalKillHeroBL += heroData.HeroKill;
								this.totalDeathBL += heroData.Death;
							}
						}
						else
						{
							AchieveData achieveData = GameManager.Instance.AchieveManager.GetAchieveData(units.unique_id, units.teamType);
							if (GameManager.Instance.DamageStatisticalManager.UnitsDictionary[current].TeamType == TeamType.LM)
							{
								if (achieveData == null)
								{
									achieveData = new AchieveData(0, string.Empty, TeamType.LM);
								}
								this.LMHeroDataList.Add(new HeroData
								{
									LV = UtilManager.Instance.GetHerolv(units.unique_id),
									Quality = units.quality,
									Stars = units.star,
									HeroID = units.npc_id,
									Unique_id = units.unique_id
								});
								this.LMVictPlayerDataList.Add(new VictPlayerData
								{
									KillHero = achieveData.TotalKill,
									Death = achieveData.SelfDeathTime,
									KillAssist = units.assistantNum,
									KillMonster = achieveData.MonsterKillNum,
									SelfIsDeath = !units.isLive,
									LeftDeathTime = (int)BattleAttrManager.Instance.GetPlayerDeathLastTime(units.unique_id),
									SummonerSkillID = Singleton<PvpManager>.Instance.GetSummonerSkillId(units.unique_id),
									IsControlPlayer = units.isPlayer,
									HeroEquipmentList = BattleEquipTools_op.GetHeroItemsString(units),
									NPCID = units.npc_id,
									LastChamRank = this.tempInfo.CharmRankvalue
								});
								this.statisticItemList[units.npc_id + "LM"].UpdateInfo(this.LMHeroDataList[num2], VictPlayerType.LM, this.LMVictPlayerDataList[num2], this.LMVictPlayerDataList[num2].IsControlPlayer);
								this.statisticItemList[units.npc_id + "LM"].SetAddFriendShow(false);
								num2++;
								this.totalKillAssistLM += units.assistantNum;
								this.totalKillHeroLM += achieveData.TotalKill;
								this.totalDeathLM += achieveData.SelfDeathTime;
							}
							else
							{
								if (achieveData == null)
								{
									achieveData = new AchieveData(0, string.Empty, TeamType.BL);
								}
								this.BLHeroDataList.Add(new HeroData
								{
									LV = UtilManager.Instance.GetHerolv(units.unique_id),
									Quality = units.quality,
									Stars = units.star,
									HeroID = units.npc_id,
									Unique_id = units.unique_id
								});
								this.BLVictPlayerDataList.Add(new VictPlayerData
								{
									KillHero = achieveData.TotalKill,
									Death = achieveData.SelfDeathTime,
									KillAssist = units.assistantNum,
									KillMonster = achieveData.MonsterKillNum,
									SelfIsDeath = !units.isLive,
									LeftDeathTime = (int)BattleAttrManager.Instance.GetPlayerDeathLastTime(units.unique_id),
									SummonerSkillID = Singleton<PvpManager>.Instance.GetSummonerSkillId(units.unique_id),
									IsControlPlayer = units.isPlayer,
									HeroEquipmentList = BattleEquipTools_op.GetHeroItemsString(units),
									NPCID = units.npc_id,
									LastChamRank = this.tempInfo.CharmRankvalue
								});
								this.statisticItemList[units.npc_id + "BL"].UpdateInfo(this.BLHeroDataList[num3], VictPlayerType.BL, this.BLVictPlayerDataList[num3], this.BLVictPlayerDataList[num3].IsControlPlayer);
								this.statisticItemList[units.npc_id + "BL"].SetAddFriendShow(false);
								num3++;
								this.totalKillAssistBL += units.assistantNum;
								this.totalKillHeroBL += achieveData.TotalKill;
								this.totalDeathBL += achieveData.SelfDeathTime;
							}
						}
						num++;
					}
				}
				this.ChangeHighestData(this.LMVictPlayerDataList, true, null);
				this.ChangeHighestData(this.BLVictPlayerDataList, false, null);
				if (LevelManager.Instance.IsPvpBattleType)
				{
					this.LMKillNum.text = string.Concat(new object[]
					{
						this.totalKillHeroLM,
						"/",
						this.totalDeathLM,
						"/",
						this.totalKillAssistLM
					});
					this.BLKillNum.text = string.Concat(new object[]
					{
						this.totalKillHeroBL,
						"/",
						this.totalDeathBL,
						"/",
						this.totalKillAssistBL
					});
					this.LMTowerLabel.text = UtilManager.Instance.GetTowerDestroyByTeam(TeamType.BL).ToString();
					this.BLTowerLabel.text = UtilManager.Instance.GetTowerDestroyByTeam(TeamType.LM).ToString();
					this.LMBossLabel.text = UtilManager.Instance.GetEpicMonsterKilling(TeamType.LM).ToString();
					this.BLBossLabel.text = UtilManager.Instance.GetEpicMonsterKilling(TeamType.BL).ToString();
				}
				else
				{
					this.LMKillNum.text = string.Concat(new object[]
					{
						this.totalKillHeroLM,
						"/",
						this.totalDeathLM,
						"/",
						this.totalKillAssistLM
					});
					this.BLKillNum.text = string.Concat(new object[]
					{
						this.totalKillHeroBL,
						"/",
						this.totalDeathBL,
						"/",
						this.totalKillAssistBL
					});
					this.LMTowerLabel.text = UtilManager.Instance.GetTowerDestroyByTeam(TeamType.BL).ToString();
					this.BLTowerLabel.text = UtilManager.Instance.GetTowerDestroyByTeam(TeamType.LM).ToString();
					this.LMBossLabel.text = UtilManager.Instance.GetEpicMonsterKilling(TeamType.LM).ToString();
					this.BLBossLabel.text = UtilManager.Instance.GetEpicMonsterKilling(TeamType.BL).ToString();
				}
			}
		}

		private void AddScore(int kill, TeamType team)
		{
			if (team == TeamType.LM)
			{
				this.scoreLM += kill;
			}
			else if (team == TeamType.BL)
			{
				this.scoreBL += kill;
			}
		}
	}
}
