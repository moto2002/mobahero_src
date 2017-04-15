using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using GUIFramework;
using MobaHeros;
using MobaHeros.Pvp;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class GoldView : BaseView<GoldView>
	{
		private class SourceData
		{
			public int heroKill;

			public int death;

			public int assist;

			public int monsterKill;

			public bool firstKill;

			public void Clear()
			{
				this.heroKill = 0;
				this.death = 0;
				this.assist = 0;
				this.monsterKill = 0;
				this.firstKill = false;
			}
		}

		private Transform PVEAnchor;

		private Transform Suspend;

		private Transform Setting;

		private Transform Suspend1;

		private UILabel killLabel;

		private UILabel DeathLabel;

		private Transform Kill;

		private Transform Death;

		private Transform PVPAnchor;

		private UILabel PVP_KillLabel;

		private UILabel PVP_Death;

		private UILabel PVP_Assists;

		private UILabel PVP_Boss;

		private UILabel PVP_Time;

		private UILabel PVE_Time;

		private UILabel PVE_Assists;

		private UILabel PVE_Boss;

		private Transform BottomRightAnchor;

		private Transform bottomPvpAnchor;

		private Transform bottomPveAnchor;

		private Transform LeftTopAnchor;

		private Transform SkillPveBtn;

		private Transform SkillPvpBtn;

		private UISprite SkillPveSprite;

		private UISprite SkillPveSprite2;

		private UISprite SkillPvpSprite;

		private UISprite SkillPvpSprite2;

		private Transform StatisticPveBtn;

		private Transform StatisticPvpBtn;

		private Transform StatisticBtn1;

		private Transform B_Game;

		private UILabel FPSLabel;

		private UILabel DelayLabel;

		private Transform BottomAnchor;

		private UIGrid B_Grid;

		private Transform TestBtn;

		private Transform ResetBtn;

		private Transform SpeedBtn;

		private Transform ResetSpeedBtn;

		private Transform LevelBtn;

		public Transform EnableHeroAIBnt;

		public Transform EnableMonsterAIBnt;

		public Transform EnableTowerAIBnt;

		public Transform EnableAllAIBnt;

		public Transform NoMonsterBnt;

		public Transform NoSkillCdBnt;

		public Transform NoSkillCostBnt;

		public Transform WhoIsYourDady;

		public Transform AddBlood;

		public Transform Toggle;

		private Transform DebugObj;

		private UILabel SpeedLabel;

		private Transform skillBackBg;

		private int BattleTypeIndex;

		private Transform heroTargetInfo;

		private string heroName;

		private UITexture heroTexture;

		private UISprite heroBloodSprite;

		private UILabel heroHpNumber;

		private UILabel heroKillInfo;

		private Transform otherTargetInfo;

		private UITexture otherTexture;

		private UISprite otherBloodSprite;

		private UILabel otherHpNumber;

		private UILabel otherName;

		private Texture backTexture;

		private Transform mFriendInfo;

		private UISprite mLockViewSprite;

		private UISprite mUnlockViewSprite;

		private Transform mFriendList;

		private BarrageFriendItem[] mFriendItemArr = new BarrageFriendItem[4];

		private Transform mFPS;

		private bool bInitFriendFinished;

		private VTrigger mFriendListen;

		private UIPanel skillBg;

		public string pveTime = "?:?";

		private VTrigger ChangePlayerTrigger;

		private SkillShowItem skillItem;

		private CoroutineManager m_CoroutineManager = new CoroutineManager();

		private GoldView.SourceData _data = new GoldView.SourceData();

		private float[] pvpSpeeds = new float[]
		{
			1f,
			0.5f,
			0.2f,
			0.1f,
			0.05f,
			0.02f,
			0.01f,
			0.001f
		};

		private int pvpSpeed;

		private VTrigger listener;

		private Task fpsTask;

		private float updateInterval = 0.5f;

		private float accum;

		private float frames;

		private float timeleft = 0.5f;

		private string timeStr = string.Empty;

		public GoldView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "GoldView");
		}

		public override void Init()
		{
			base.Init();
			this.skillItem = ResourceManager.LoadPath<SkillShowItem>("Prefab/UI/PlayHUD/SkillShowItem", null, 0);
			this.ChangePlayerTrigger = TriggerManager.CreateGameEventTrigger(GameEvent.ChangePlayer, null, new TriggerAction(this.ChangePlayer));
			this.listener = TriggerManager.CreateGameEventTrigger(GameEvent.UpdateView, null, new TriggerAction(this.UpdateGoldView));
			this.mFriendListen = TriggerManager.CreateGameEventTrigger(GameEvent.GameStart, null, new TriggerAction(this.OnGameStart));
			this.PVEAnchor = this.transform.Find("PVEAnchor");
			this.Kill = this.transform.Find("PVEAnchor/Kill");
			this.Death = this.transform.Find("PVEAnchor/Death");
			this.killLabel = this.transform.Find("PVEAnchor/Kill/KillNumLabel").GetComponent<UILabel>();
			this.DeathLabel = this.transform.Find("PVEAnchor/Death/DeathNumLabel").GetComponent<UILabel>();
			this.PVE_Boss = this.PVEAnchor.Find("Boss/BossNumLabel").GetComponent<UILabel>();
			this.PVE_Assists = this.PVEAnchor.Find("Assists/AssistsNumLabel").GetComponent<UILabel>();
			this.PVPAnchor = this.transform.Find("PVPAnchor");
			this.PVP_KillLabel = this.PVPAnchor.Find("Kill/KillNumLabel").GetComponent<UILabel>();
			this.PVP_Death = this.PVPAnchor.Find("Death/DeathNumLabel").GetComponent<UILabel>();
			this.PVE_Time = this.PVEAnchor.Find("Time/TimeLabel").GetComponent<UILabel>();
			this.PVP_Time = this.PVPAnchor.Find("Time/TimeLabel").GetComponent<UILabel>();
			this.PVP_Assists = this.PVPAnchor.Find("Assists/AssistsNumLabel").GetComponent<UILabel>();
			this.PVP_Boss = this.PVPAnchor.Find("Boss/BossNumLabel").GetComponent<UILabel>();
			this.BottomRightAnchor = this.transform.Find("BottomRightAnchor");
			this.bottomPveAnchor = this.BottomRightAnchor.Find("Pve");
			this.bottomPvpAnchor = this.BottomRightAnchor.Find("Pvp");
			this.LeftTopAnchor = this.transform.Find("LeftTopAnchor");
			this.SkillPveBtn = this.bottomPveAnchor.Find("SkillBtn");
			this.SkillPvpBtn = this.bottomPvpAnchor.Find("SkillBtn");
			this.SkillPveSprite = this.SkillPveBtn.GetComponent<UISprite>();
			this.SkillPveSprite2 = this.bottomPveAnchor.Find("SkillBtn/SkillBtn2").GetComponent<UISprite>();
			this.SkillPvpSprite = this.SkillPvpBtn.GetComponent<UISprite>();
			this.SkillPvpSprite2 = this.bottomPvpAnchor.Find("SkillBtn/SkillBtn2").GetComponent<UISprite>();
			this.StatisticPveBtn = this.bottomPveAnchor.Find("StatisticBtn");
			this.StatisticPvpBtn = this.bottomPvpAnchor.Find("StatisticBtn");
			this.StatisticBtn1 = this.LeftTopAnchor.Find("StatisticBtn");
			this.B_Game = this.transform.Find("Center/Game");
			this.Suspend = this.bottomPveAnchor.Find("Suspend");
			this.Setting = this.bottomPvpAnchor.Find("Setting");
			this.Suspend1 = this.LeftTopAnchor.Find("Suspend");
			this.FPSLabel = this.transform.Find("Center/Game/FPS/Label").GetComponent<UILabel>();
			this.DelayLabel = this.transform.Find("Center/Game/Delay/Label").GetComponent<UILabel>();
			this.BottomAnchor = this.transform.Find("BottomAnchor");
			this.TestBtn = this.bottomPveAnchor.Find("TestBtn");
			this.DebugObj = this.bottomPvpAnchor.Find("Debug");
			this.ResetBtn = this.bottomPveAnchor.Find("ResetBtn");
			this.SpeedBtn = this.DebugObj.Find("SpeedBtn");
			this.SpeedLabel = this.SpeedBtn.Find("speed").GetComponent<UILabel>();
			this.ResetSpeedBtn = this.DebugObj.Find("ResetSpeedBtn");
			this.skillBackBg = this.transform.Find("BottomAnchor/SkillBg/BackBg");
			this.LevelBtn = this.DebugObj.Find("LevelBtn");
			this.EnableHeroAIBnt = this.DebugObj.Find("Toggle/EnableHeroAI");
			this.EnableMonsterAIBnt = this.DebugObj.Find("Toggle/EnableMonsterAI");
			this.EnableTowerAIBnt = this.DebugObj.Find("Toggle/EnableTowerAI");
			this.EnableAllAIBnt = this.DebugObj.Find("Toggle/EnableAllAI");
			this.NoMonsterBnt = this.DebugObj.Find("Toggle/NoMontser");
			this.NoSkillCdBnt = this.DebugObj.Find("Toggle/NoSkillCd");
			this.NoSkillCostBnt = this.DebugObj.Find("Toggle/NoSkillCost");
			this.WhoIsYourDady = this.DebugObj.Find("Toggle/WhoIsYourDady");
			this.AddBlood = this.DebugObj.Find("Toggle/AddBlood");
			this.Toggle = this.DebugObj.Find("Toggle");
			this.heroTargetInfo = this.transform.Find("TargetBox/HeroTarget");
			this.heroTexture = this.heroTargetInfo.Find("Sprite/Texture").GetComponent<UITexture>();
			this.heroBloodSprite = this.heroTargetInfo.Find("HPBar/SlideBlood").GetComponent<UISprite>();
			this.heroHpNumber = this.heroTargetInfo.Find("HPBar/HPNumber").GetComponent<UILabel>();
			this.heroKillInfo = this.heroTargetInfo.Find("KillInfo/KillNumber").GetComponent<UILabel>();
			this.otherTargetInfo = this.transform.Find("TargetBox/OtherTarget");
			this.otherTexture = this.otherTargetInfo.Find("Sprite/Texture").GetComponent<UITexture>();
			this.backTexture = this.otherTexture.mainTexture;
			this.otherBloodSprite = this.otherTargetInfo.Find("HPBar/HPBar/SlideBlood").GetComponent<UISprite>();
			this.otherHpNumber = this.otherTargetInfo.Find("HPBar/HPBar/HPNumber").GetComponent<UILabel>();
			this.otherName = this.otherTargetInfo.Find("Name/name").GetComponent<UILabel>();
			this.skillBg = this.BottomAnchor.Find("SkillBg").GetComponent<UIPanel>();
			this.B_Grid = this.BottomAnchor.Find("SkillBg/Grid").GetComponent<UIGrid>();
			UIEventListener.Get(this.Suspend.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnSuspend);
			UIEventListener.Get(this.Setting.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnSuspend);
			UIEventListener.Get(this.SkillPveBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickSkillBtn);
			UIEventListener.Get(this.SkillPvpBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickSkillBtn);
			UIEventListener.Get(this.SkillPveSprite2.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickSkillBtn);
			UIEventListener.Get(this.SkillPvpSprite2.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickSkillBtn);
			UIEventListener.Get(this.StatisticPveBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickStatisticBtn);
			UIEventListener.Get(this.StatisticPvpBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickStatisticBtn);
			UIEventListener.Get(this.StatisticBtn1.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickStatisticBtn);
			UIEventListener.Get(this.Suspend1.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnSuspend);
			UIEventListener.Get(this.TestBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnTestBtn);
			UIEventListener.Get(this.ResetBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnResetBtn);
			UIEventListener.Get(this.SpeedBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnSpeedBtn);
			UIEventListener.Get(this.ResetSpeedBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnResetSpeedBtn);
			UIEventListener.Get(this.LevelBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnLevelBtn);
			UIEventListener.Get(this.skillBackBg.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnskillBackBg);
			UIEventListener.Get(this.EnableHeroAIBnt.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.EnableTowerAIBnt.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.EnableMonsterAIBnt.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.EnableAllAIBnt.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.NoMonsterBnt.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.NoSkillCdBnt.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.NoSkillCostBnt.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.WhoIsYourDady.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.AddBlood.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			this.ResetFpsData();
			this.mFriendInfo = this.transform.Find("BattleFriendNotice");
			this.mLockViewSprite = this.transform.Find("BattleFriendNotice/DragPlayerView/LockSprite").GetComponent<UISprite>();
			this.mUnlockViewSprite = this.transform.Find("BattleFriendNotice/DragPlayerView/UnlockSprite").GetComponent<UISprite>();
			this.mFriendList = this.transform.Find("BattleFriendNotice/FriendList");
			UIEventListener.Get(this.mLockViewSprite.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnPressLockView);
			UIEventListener.Get(this.mUnlockViewSprite.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnPressUnlockView);
			Units player = PlayerControlMgr.Instance.GetPlayer();
			this.ShowDebugPvp();
			this.bInitFriendFinished = false;
		}

		private void HideFriendInfo()
		{
			this.mFriendInfo.gameObject.SetActive(false);
		}

		private void ShowFriendInfo(bool lockView = true)
		{
			this.mFriendInfo.gameObject.SetActive(true);
			this.B_Game.gameObject.SetActive(false);
			if (lockView)
			{
				GlobalSettings.Instance.isLockView = true;
				this.mUnlockViewSprite.SetActive(false);
				this.mLockViewSprite.SetActive(true);
			}
			else
			{
				GlobalSettings.Instance.isLockView = false;
				this.mLockViewSprite.SetActive(false);
				this.mUnlockViewSprite.SetActive(true);
			}
			Units player = PlayerControlMgr.Instance.GetPlayer();
			if (player == null)
			{
				return;
			}
			int friendHeroesCount = MapManager.Instance.GetFriendHeroesCount(player.teamType);
			if (this.mFPS != null)
			{
				this.mFPS.gameObject.SetActive(false);
			}
			if (friendHeroesCount <= 1)
			{
				this.mFPS = this.transform.Find("BattleFriendNotice/FPSInfo2");
			}
			else
			{
				this.mFPS = this.transform.Find("BattleFriendNotice/FPSInfo1");
			}
			this.mFPS.gameObject.SetActive(true);
			if (friendHeroesCount > 1)
			{
				IList<Units> allHeroes = MapManager.Instance.GetAllHeroes();
				int num = 0;
				foreach (Units current in allHeroes)
				{
					if (current.teamType == player.teamType && current != player)
					{
						if (num < 4)
						{
							this.transform.Find("BattleFriendNotice/FriendList/Friend" + num).gameObject.SetActive(true);
						}
						num++;
					}
				}
			}
			this.bInitFriendFinished = true;
		}

		public void ShowDebugPvp()
		{
			if (this.DebugObj != null)
			{
				this.DebugObj.gameObject.SetActive(GlobalSettings.Instance.PvpSetting.isDebugSpeed);
			}
		}

		public void OnskillBackBg(GameObject go)
		{
			this.ClickSkillBtn(null);
		}

		private void OnLevelBtn(GameObject go)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
			if (gameObject != null)
			{
				Units component = gameObject.GetComponent<Units>();
				UtilCounter counter = UtilManager.Instance.GetCounter(UtilType.Exp);
				ExpValue expValue = counter.GetValue(PlayerControlMgr.Instance.GetPlayer().unique_id) as ExpValue;
				int lvUpExp = expValue.GetLvUpExp(expValue.CurLv + 1);
				expValue.AddExp((float)lvUpExp);
				SendMsgManager.Instance.SendPvpMsgBase<CheatInfo>(PvpCode.C2P_GMCheat, new CheatInfo
				{
					cheatMsg = "AddExp " + lvUpExp
				});
			}
		}

		public void OnGameCommand(GameObject go)
		{
			if (!(go.name == "NoMontser"))
			{
				if (go.name == "NoSkillCd")
				{
					GlobalSettings.IsNoSkillCD = !GlobalSettings.IsNoSkillCD;
				}
				else if (go.name == "NoSkillCost")
				{
					GlobalSettings.NoCost = !GlobalSettings.NoCost;
				}
				else if (!(go.name == "EnableHeroAI"))
				{
					if (!(go.name == "EnableMonsterAI"))
					{
						if (!(go.name == "EnableTowerAI"))
						{
							if (go.name == "EnableAllAI")
							{
							}
						}
					}
				}
			}
			SendMsgManager.Instance.SendPvpMsgBase<CheatInfo>(PvpCode.C2P_GMCheat, new CheatInfo
			{
				cheatMsg = "GameCommand " + go.name
			});
		}

		private void OnResetCDBtn(GameObject go)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
			if (gameObject != null)
			{
				Units component = gameObject.GetComponent<Units>();
				if (null == component)
				{
					return;
				}
				List<string> skills = component.GetSkills();
				for (int i = 0; i < skills.Count; i++)
				{
					if (component.GetCDTime(skills[i]) > 0f)
					{
						component.SetCDTime(skills[i], 0f);
						Singleton<SkillView>.Instance.UpdateSkillView(skills[i], false);
					}
				}
				SendMsgManager.Instance.SendPvpMsgBase<CheatInfo>(PvpCode.C2P_GMCheat, new CheatInfo
				{
					cheatMsg = "ClearCD"
				});
			}
		}

		private void OnResetBtn(GameObject go)
		{
			if (LevelManager.Instance.CheckSceneIsTest())
			{
				if (LevelManager.Instance.IsServerZyBattleType)
				{
					return;
				}
				GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
				if (gameObject != null)
				{
					Units component = gameObject.GetComponent<Units>();
					UtilCounter counter = UtilManager.Instance.GetCounter(UtilType.Exp);
					ExpValue expValue = counter.GetValue(PlayerControlMgr.Instance.GetPlayer().unique_id) as ExpValue;
					expValue.AddExp(-expValue.CurExp);
					component.skillManager.SkillPointsLeft = 0;
					for (int i = 0; i < 4; i++)
					{
						List<SkillDataKey> skillsByIndex = component.getSkillsByIndex(i);
						if (skillsByIndex != null)
						{
							for (int j = 0; j < skillsByIndex.Count; j++)
							{
								skillsByIndex[j] = new SkillDataKey(skillsByIndex[j].SkillID, 0, skillsByIndex[j].Skin);
								Skill skillById = component.getSkillById(skillsByIndex[j].SkillID);
								if (skillById != null)
								{
									skillById.SetLevel(0);
								}
							}
						}
					}
					SkillCounter skillCounter = UtilManager.Instance.GetCounter(UtilType.Skill) as SkillCounter;
					skillCounter.OnHeroLevelup(component, 0);
					component.level = 1;
					component.UpLevel();
					CtrlManager.CloseWindow(WindowID.SkillView);
					CtrlManager.OpenWindow(WindowID.SkillView, null);
				}
			}
		}

		private void OnTestBtn(GameObject go)
		{
			if (LevelManager.Instance.CheckSceneIsTest())
			{
				if (LevelManager.Instance.IsServerZyBattleType)
				{
					return;
				}
				GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
				if (gameObject != null)
				{
					Units component = gameObject.GetComponent<Units>();
					UtilCounter counter = UtilManager.Instance.GetCounter(UtilType.Exp);
					ExpValue expValue = counter.GetValue(PlayerControlMgr.Instance.GetPlayer().unique_id) as ExpValue;
					int lvUpExp = expValue.GetLvUpExp(expValue.CurLv + 1);
					expValue.AddExp((float)lvUpExp);
				}
			}
		}

		private void OnSpeedBtn(GameObject go)
		{
			this.pvpSpeed++;
			if (this.pvpSpeed >= this.pvpSpeeds.Length)
			{
				this.pvpSpeed = 0;
			}
			this.SetPvpSpeed(this.pvpSpeeds[this.pvpSpeed]);
			SendMsgManager.Instance.SendPvpMsgBase<CheatInfo>(PvpCode.C2P_GMCheat, new CheatInfo
			{
				cheatMsg = "speed " + this.SpeedLabel.text
			});
		}

		private void OnResetSpeedBtn(GameObject go)
		{
			this.pvpSpeed = 0;
			this.SetPvpSpeed(this.pvpSpeeds[this.pvpSpeed]);
			SendMsgManager.Instance.SendPvpMsgBase<CheatInfo>(PvpCode.C2P_GMCheat, new CheatInfo
			{
				cheatMsg = "speed " + this.SpeedLabel.text
			});
		}

		public void SetPvpSpeed(float speed)
		{
			if (this.SpeedLabel != null && this.SpeedLabel.enabled)
			{
				this.SpeedLabel.text = speed.ToString();
			}
			Time.timeScale = speed;
		}

		public override void RegisterUpdateHandler()
		{
			this.ShowUI(this.BattleTypeIndex);
			this.m_CoroutineManager.StartCoroutine(this.RecordTime(), true);
			if (LevelManager.CurBattleType == 11)
			{
				this.SetType(2);
			}
			else
			{
				this.SetType(1);
			}
			this.RefreshUI();
		}

		public void SetType(int type = 1)
		{
			if (type == 1)
			{
				this.LeftTopAnchor.gameObject.SetActive(false);
				this.BottomRightAnchor.gameObject.SetActive(true);
				this.PVEAnchor.Find("Time").gameObject.SetActive(true);
			}
			else
			{
				this.LeftTopAnchor.gameObject.SetActive(true);
				this.BottomRightAnchor.gameObject.SetActive(false);
				this.PVEAnchor.Find("Time").gameObject.SetActive(false);
			}
		}

		public override void CancelUpdateHandler()
		{
			if (this.listener != null)
			{
				TriggerManager.DestroyTrigger(this.listener);
				this.listener = null;
			}
			if (this.mFriendListen != null)
			{
				TriggerManager.DestroyTrigger(this.mFriendListen);
				this.mFriendListen = null;
			}
			if (this.fpsTask != null)
			{
				this.fpsTask.Stop();
			}
		}

		public override void RefreshUI()
		{
			this.UpdateGoldView();
		}

		public override void Destroy()
		{
			this.ChangePlayerTrigger = null;
			this.m_CoroutineManager.StopAllCoroutine();
			this._data.Clear();
			base.Destroy();
		}

		private void UpdateSourceData()
		{
			Units player = PlayerControlMgr.Instance.GetPlayer();
			if (player == null)
			{
				return;
			}
			if (LevelManager.Instance.IsPvpBattleType)
			{
				int unique_id = player.unique_id;
				PvpStatisticMgr.HeroData heroData = Singleton<PvpManager>.Instance.StatisticMgr.GetHeroData(unique_id);
				this._data.heroKill = heroData.HeroKill;
				this._data.monsterKill = heroData.MonsterKill;
				this._data.assist = heroData.Assist;
				this._data.death = heroData.Death;
				this._data.firstKill = heroData.FirstKill;
			}
			else
			{
				AchieveData achieveData = GameManager.Instance.AchieveManager.GetAchieveData(player.unique_id, player.teamType);
				if (achieveData != null)
				{
					this._data.heroKill = achieveData.TotalKill;
					this._data.monsterKill = achieveData.MonsterKillNum;
					this._data.assist = player.assistantNum;
					this._data.death = achieveData.SelfDeathTime;
				}
			}
		}

		public void ShowPVEAnchor()
		{
			this.PVEAnchor.gameObject.SetActive(true);
		}

		private void UpdateGoldView()
		{
			if (this.transform == null)
			{
				return;
			}
			this.UpdateSourceData();
			this.killLabel.text = this._data.heroKill.ToString();
			this.DeathLabel.text = this._data.death.ToString();
			this.PVE_Boss.text = this._data.monsterKill.ToString();
			this.PVP_Boss.text = this._data.monsterKill.ToString();
			this.PVP_KillLabel.text = this._data.heroKill.ToString();
			this.PVP_Death.text = this._data.death.ToString();
			this.PVP_Assists.text = this._data.assist.ToString();
			this.PVE_Assists.text = this._data.assist.ToString();
			if (LevelManager.Instance.IsPvpBattleType)
			{
				this.PVPAnchor.gameObject.SetActive(true);
				this.PVEAnchor.gameObject.SetActive(false);
				this.bottomPvpAnchor.gameObject.SetActive(true);
				this.bottomPveAnchor.gameObject.SetActive(false);
				if (!this.bInitFriendFinished)
				{
					this.B_Game.gameObject.SetActive(true);
				}
				this.Suspend.gameObject.SetActive(false);
			}
			else
			{
				this.PVEAnchor.gameObject.SetActive(true);
				this.bottomPvpAnchor.gameObject.SetActive(false);
				this.bottomPveAnchor.gameObject.SetActive(true);
				this.Suspend.gameObject.SetActive(true);
			}
			if (LevelManager.CurBattleType == 6)
			{
				this.PVEAnchor.gameObject.SetActive(false);
				this.StatisticPveBtn.gameObject.SetActive(false);
			}
			else
			{
				this.StatisticPveBtn.gameObject.SetActive(true);
			}
			if (LevelManager.Instance.CheckSceneIsTest())
			{
				this.TestBtn.gameObject.SetActive(true);
				this.ResetBtn.gameObject.SetActive(true);
			}
		}

		private void OnSuspend(GameObject objct_1 = null)
		{
			CtrlManager.OpenWindow(WindowID.ReturnView, null);
			if (!LevelManager.Instance.IsPvpBattleType)
			{
				GameManager.SetGameState(GameState.Game_Pausing);
			}
		}

		private void OnShowInfo(GameObject objct_1 = null)
		{
			if (Singleton<StatisticView>.Instance != null && Singleton<StatisticView>.Instance.gameObject == null)
			{
				Singleton<StatisticView>.Instance.SetModel(true);
				CtrlManager.OpenWindow(WindowID.StatisticView, null);
			}
		}

		private void ClickSkillBtn(GameObject obj = null)
		{
			this.skillBg.depth = 120;
			this.BottomAnchor.gameObject.SetActive(!this.BottomAnchor.gameObject.activeInHierarchy);
			if (this.BottomAnchor.gameObject.activeInHierarchy)
			{
				Singleton<SkillView>.Instance.SetLevelUpBtns(false);
			}
			else
			{
				Singleton<SkillView>.Instance.SetLevelUpBtns(true);
			}
			if (!this.BottomAnchor.gameObject.activeInHierarchy)
			{
				return;
			}
			this.UpdateSkillStudy();
		}

		public void UpdateSkillStudy()
		{
			Units player = PlayerControlMgr.Instance.GetPlayer();
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(player.npc_id);
			string[] heroSkills = heroMainData.skill_id.Split(new char[]
			{
				','
			});
			GridHelper.FillGrid<SkillShowItem>(this.B_Grid, this.skillItem, (heroSkills.Length <= 4) ? heroSkills.Length : 4, delegate(int idx, SkillShowItem comp)
			{
				SysSkillMainVo skillData = SkillUtility.GetSkillData(heroSkills[idx], -1, -1);
				Texture textue = ResourceManager.Load<Texture>(skillData.skill_icon, true, true, null, 0, false);
				string type = (skillData.skill_trigger != 3) ? "(主动)" : "(被动)";
				string unikey = skillData.skill_id + "_lv0" + ((player.getSkillById(skillData.skill_id).skillLevel != 0) ? player.getSkillById(skillData.skill_id).skillLevel : 1).ToString();
				SysSkillLevelupVo dataById = BaseDataMgr.instance.GetDataById<SysSkillLevelupVo>(unikey);
				comp.Init(textue, skillData.skill_name, type, SkillView.FixSkillTxtInfo(dataById.skill_description2, player), !player.skillManager.IsSkillUnlock(skillData.skill_id), (dataById.cd / (1f + player.GetAttr(AttrType.NormalSkillCooling))).ToString("0.0"));
			});
			this.B_Grid.Reposition();
		}

		private void ResetFpsData()
		{
			this.accum = 0f;
			this.frames = 0f;
			this.timeleft = 0.5f;
		}

		[DebuggerHidden]
		private IEnumerator RecordTime()
		{
			GoldView.<RecordTime>c__IteratorCF <RecordTime>c__IteratorCF = new GoldView.<RecordTime>c__IteratorCF();
			<RecordTime>c__IteratorCF.<>f__this = this;
			return <RecordTime>c__IteratorCF;
		}

		private void ClickStatisticBtn(GameObject obj = null)
		{
			if (Singleton<StatisticView>.Instance != null && Singleton<StatisticView>.Instance.gameObject != null && Singleton<StatisticView>.Instance.gameObject.activeInHierarchy)
			{
				Singleton<StatisticView>.Instance.Close_Statistic(null);
			}
			else
			{
				this.OnShowInfo(null);
			}
		}

		private void ChangePlayer()
		{
			this.RefreshUI();
		}

		private void ShowUI(int battleType)
		{
			this.BottomAnchor.gameObject.SetActive(false);
			switch (battleType)
			{
			case 1:
			case 2:
			case 6:
			case 9:
				this.HideFriendInfo();
				this.PVPAnchor.gameObject.SetActive(false);
				this.PVEAnchor.gameObject.SetActive(true);
				this.Kill.gameObject.SetActive(true);
				this.Death.gameObject.SetActive(true);
				break;
			case 3:
				this.Kill.gameObject.SetActive(false);
				this.Death.gameObject.SetActive(false);
				break;
			case 11:
				this.HideFriendInfo();
				this.PVPAnchor.gameObject.SetActive(false);
				this.PVEAnchor.gameObject.SetActive(true);
				this.Kill.gameObject.SetActive(true);
				this.Death.gameObject.SetActive(true);
				break;
			case 12:
				this.PVPAnchor.gameObject.SetActive(true);
				this.PVEAnchor.gameObject.SetActive(false);
				break;
			}
		}

		public void SetBattleType(int battleType)
		{
			this.BattleTypeIndex = battleType;
		}

		public void ShowTargetInfo(Units target)
		{
			if (target == null)
			{
				if (this.otherTargetInfo == null || this.heroTargetInfo == null)
				{
					return;
				}
				this.otherTargetInfo.gameObject.SetActive(false);
				this.heroTargetInfo.gameObject.SetActive(false);
				return;
			}
			else
			{
				if (target.teamType == PlayerControlMgr.Instance.GetPlayer().teamType)
				{
					return;
				}
				if (target.isHero)
				{
					this.ShowHeroTargetInfo(target);
				}
				else
				{
					this.ShowOtherTargetInfo(target);
				}
				return;
			}
		}

		private void ShowHeroTargetInfo(Units hero)
		{
			if (this.otherTargetInfo.gameObject.activeSelf)
			{
				this.otherTargetInfo.gameObject.SetActive(false);
			}
			if (!this.heroTargetInfo.gameObject.activeSelf)
			{
				this.heroTargetInfo.gameObject.SetActive(true);
			}
			if (this.heroName != hero.name)
			{
				this.heroName = hero.name;
				SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(hero.npc_id);
				if (heroMainData != null)
				{
					this.heroTexture.mainTexture = ResourceManager.Load<Texture>(heroMainData.avatar_icon, true, true, null, 0, false);
				}
			}
			PvpStatisticMgr.HeroData heroData = Singleton<PvpManager>.Instance.StatisticMgr.GetHeroData(hero.unique_id);
			string text = heroData.MonsterKill.ToString();
			string text2 = string.Concat(new object[]
			{
				heroData.HeroKill,
				"/",
				heroData.Death,
				"/",
				heroData.Assist
			});
			int num = text.Length + text2.Length;
			for (int i = 0; i < 12 - num; i++)
			{
				text += " ";
			}
			this.heroKillInfo.text = text + text2;
			this.heroHpNumber.text = (int)hero.hp + "/" + (int)hero.hp_max;
			this.heroBloodSprite.fillAmount = hero.hp / hero.hp_max;
		}

		private void ShowOtherTargetInfo(Units other)
		{
			if (this.heroTargetInfo == null)
			{
				return;
			}
			if (this.heroTargetInfo.gameObject.activeSelf)
			{
				this.heroTargetInfo.gameObject.SetActive(false);
			}
			if (!this.otherTargetInfo.gameObject.activeSelf)
			{
				this.otherTargetInfo.gameObject.SetActive(true);
			}
			if (this.heroName != other.name)
			{
				this.heroName = other.name;
				SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(other.npc_id);
				if (monsterMainData != null)
				{
					if (other.isTower || other.isHome)
					{
						this.otherTexture.mainTexture = ResourceManager.Load<Texture>("Tower_Avatar", true, true, null, 0, false);
					}
					else
					{
						this.otherTexture.mainTexture = ResourceManager.Load<Texture>(monsterMainData.avatar_icon, true, true, null, 0, false);
					}
					if (this.otherTexture.mainTexture == null)
					{
						this.otherTexture.mainTexture = this.backTexture;
					}
					this.otherName.text = LanguageManager.Instance.GetStringById(monsterMainData.name);
				}
			}
			this.otherHpNumber.text = (int)other.hp + "/" + (int)other.hp_max;
			this.otherBloodSprite.fillAmount = other.hp / other.hp_max;
		}

		private void OnGameStart()
		{
			this.ShowFriendInfo(true);
		}

		private void OnPressLockView(GameObject obj = null)
		{
			BattleCameraMgr.Instance.ChangeCameraController(CameraControllerType.Free);
		}

		private void OnPressUnlockView(GameObject obj = null)
		{
			BattleCameraMgr.Instance.ChangeCameraController(CameraControllerType.Center);
		}

		public void SetLockViewIcon(CameraControllerType cameraControllerType)
		{
			if (cameraControllerType == CameraControllerType.Free)
			{
				GlobalSettings.Instance.isLockView = false;
			}
			if (cameraControllerType == CameraControllerType.Center)
			{
				GlobalSettings.Instance.isLockView = true;
			}
			if (this.mUnlockViewSprite == null || this.mLockViewSprite == null)
			{
				return;
			}
			if (cameraControllerType == CameraControllerType.Free)
			{
				GlobalSettings.Instance.isLockView = false;
				this.mUnlockViewSprite.gameObject.SetActive(true);
				this.mLockViewSprite.gameObject.SetActive(false);
			}
			if (cameraControllerType == CameraControllerType.Center)
			{
				GlobalSettings.Instance.isLockView = true;
				this.mUnlockViewSprite.gameObject.SetActive(false);
				this.mLockViewSprite.gameObject.SetActive(true);
			}
		}
	}
}
