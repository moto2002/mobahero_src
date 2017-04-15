using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Common;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaHeros;
using MobaMessageData;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Com.Game.Module
{
	public class UIPvpEntranceCtrl : BaseView<UIPvpEntranceCtrl>
	{
		private UILabel mTitle;

		private GameObject mSelectEffect;

		private Transform mBack;

		private UIGrid mMapGrid;

		private Transform mRankPanel;

		private Transform mLeaguePanel;

		private GameObject mJZFHT;

		private GameObject mXGXJ;

		private GameObject mMGDLD;

		private GameObject mATLSSM;

		private GameObject mSoloRank;

		private GameObject mTeamRank;

		private GameObject mRankRule;

		private UILabel mRankSeasonLabel;

		private GameObject mLeagueMGDLD;

		private GameObject mLeagueLZXF;

		private UILabel mMapName;

		private UILabel mMapType;

		private UILabel mMyTicket;

		private Transform mBuyTicketBtn;

		private Transform mMyReward;

		private UILabel mMyWinTimes;

		private Transform mLeagueRule;

		private UILabel mTaskNum;

		private GameObject mJZFHTFightRobotDifficultyRoot;

		private GameObject mJZFHTSelEasyBtnRoot;

		private GameObject mJZFHTSelNormalBtnRoot;

		private GameObject mJZFHTSelHardBtnRoot;

		private GameObject mXGXJFightRobotDifficultyRoot;

		private GameObject mXGXJSelEasyBtnRoot;

		private GameObject mXGXJSelNormalBtnRoot;

		private GameObject mXGXJSelHardBtnRoot;

		private GameObject mATLSSMFightRobotDifficultyRoot;

		private GameObject mATLSSMSelEasyBtnRoot;

		private GameObject mATLSSMSelNormalBtnRoot;

		private GameObject mATLSSMSelHardBtnRoot;

		private Transform mRankSeasonReward;

		private Transform mRankSeason_CurrentTarget;

		private UIPvpEntrance_RankRewardBtn mCurrentTarget_BtnCtrl;

		private BottleBookItem[] mCurrentTarget_Items;

		private Transform mRankSeason_GoldTarget;

		private UIPvpEntrance_RankRewardBtn mGoldTarget_BtnCtrl;

		private BottleBookItem mGoldTarget_Item;

		public Dictionary<int, List<MapInfoData>> mMapDict = new Dictionary<int, List<MapInfoData>>();

		private Dictionary<string, SysBattleSceneVo> mBattleSceneDict = new Dictionary<string, SysBattleSceneVo>();

		private MatchType mMatchType;

		private readonly CoroutineManager mCoroutineManager = new CoroutineManager();

		private Task _closeTask;

		public MatchType matchType
		{
			get
			{
				return this.mMatchType;
			}
			set
			{
				this.mMatchType = value;
			}
		}

		public UIPvpEntranceCtrl()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Home/UIPvpEntrance");
		}

		public override void Init()
		{
			base.Init();
			this.BindObject();
			this.InitMapDict();
		}

		private new void BindObject()
		{
			this.mTitle = this.transform.Find("TopTitle/Label").GetComponent<UILabel>();
			this.mBack = this.transform.Find("BackBtn");
			this.mSelectEffect = this.transform.Find("MapPanel/Fx_ui_map_select").gameObject;
			this.mMapGrid = this.transform.Find("MapPanel").GetComponent<UIGrid>();
			this.mRankPanel = this.transform.Find("RankPanel");
			this.mLeaguePanel = this.transform.Find("LeaguePanel");
			this.mJZFHT = this.transform.Find("MapPanel/JZFHT").gameObject;
			this.mXGXJ = this.transform.Find("MapPanel/XGXJ").gameObject;
			this.mMGDLD = this.transform.Find("MapPanel/MGDLD").gameObject;
			this.mATLSSM = this.transform.Find("MapPanel/ATLSSM").gameObject;
			this.mSoloRank = this.mRankPanel.FindChild("SingleBtn").gameObject;
			this.mTeamRank = this.mRankPanel.FindChild("TeamBtn").gameObject;
			this.mRankRule = this.mRankPanel.FindChild("RuleBtn").gameObject;
			this.mRankSeasonLabel = this.mRankPanel.FindChild("SeasonTimeLabel").GetComponent<UILabel>();
			this.mLeagueMGDLD = this.mLeaguePanel.Find("MGDLD").gameObject;
			this.mLeagueLZXF = this.mLeaguePanel.Find("LZXF").gameObject;
			this.mMapName = this.mLeaguePanel.Find("MapName").GetComponent<UILabel>();
			this.mMapType = this.mLeaguePanel.Find("MapType").GetComponent<UILabel>();
			this.mMyTicket = this.mLeaguePanel.Find("Ticket").GetComponent<UILabel>();
			this.mBuyTicketBtn = this.mLeaguePanel.Find("TicketBtn");
			this.mMyReward = this.mLeaguePanel.Find("Reward/RewardState");
			this.mMyWinTimes = this.mLeaguePanel.Find("Reward/WinTimes").GetComponent<UILabel>();
			this.mLeagueRule = this.mLeaguePanel.Find("Rule");
			this.mTaskNum = this.mLeaguePanel.Find("Reward/TaskNum").GetComponent<UILabel>();
			this.mJZFHTFightRobotDifficultyRoot = this.mJZFHT.transform.Find("FightRobotDifficultyRoot").gameObject;
			this.mJZFHTSelEasyBtnRoot = this.mJZFHTFightRobotDifficultyRoot.transform.Find("SelEasyBtnRoot").gameObject;
			this.mJZFHTSelNormalBtnRoot = this.mJZFHTFightRobotDifficultyRoot.transform.Find("SelNormalBtnRoot").gameObject;
			this.mJZFHTSelHardBtnRoot = this.mJZFHTFightRobotDifficultyRoot.transform.Find("SelHardBtnRoot").gameObject;
			this.mXGXJFightRobotDifficultyRoot = this.mXGXJ.transform.Find("FightRobotDifficultyRoot").gameObject;
			this.mXGXJSelEasyBtnRoot = this.mXGXJFightRobotDifficultyRoot.transform.Find("SelEasyBtnRoot").gameObject;
			this.mXGXJSelNormalBtnRoot = this.mXGXJFightRobotDifficultyRoot.transform.Find("SelNormalBtnRoot").gameObject;
			this.mXGXJSelHardBtnRoot = this.mXGXJFightRobotDifficultyRoot.transform.Find("SelHardBtnRoot").gameObject;
			this.mATLSSMFightRobotDifficultyRoot = this.mATLSSM.transform.Find("FightRobotDifficultyRoot").gameObject;
			this.mATLSSMSelEasyBtnRoot = this.mATLSSMFightRobotDifficultyRoot.transform.Find("SelEasyBtnRoot").gameObject;
			this.mATLSSMSelNormalBtnRoot = this.mATLSSMFightRobotDifficultyRoot.transform.Find("SelNormalBtnRoot").gameObject;
			this.mATLSSMSelHardBtnRoot = this.mATLSSMFightRobotDifficultyRoot.transform.Find("SelHardBtnRoot").gameObject;
			this.mRankSeasonReward = this.mRankPanel.FindChild("SeasonReward");
			this.mRankSeason_CurrentTarget = this.mRankSeasonReward.FindChild("CurrentTarget");
			this.mCurrentTarget_BtnCtrl = this.mRankSeason_CurrentTarget.FindChild("BtnContainer").GetComponent<UIPvpEntrance_RankRewardBtn>();
			this.mCurrentTarget_Items = new BottleBookItem[]
			{
				this.mRankSeason_CurrentTarget.FindChild("Grid/BookItem0").GetComponent<BottleBookItem>(),
				this.mRankSeason_CurrentTarget.FindChild("Grid/BookItem1").GetComponent<BottleBookItem>(),
				this.mRankSeason_CurrentTarget.FindChild("Grid/BookItem2").GetComponent<BottleBookItem>()
			};
			this.mRankSeason_GoldTarget = this.mRankSeasonReward.FindChild("NextTarget");
			this.mGoldTarget_BtnCtrl = this.mRankSeason_GoldTarget.FindChild("BtnContainer").GetComponent<UIPvpEntrance_RankRewardBtn>();
			this.mGoldTarget_Item = this.mRankSeason_GoldTarget.FindChild("BookItem").GetComponent<BottleBookItem>();
			UIEventListener.Get(this.mBack.gameObject).onClick = new UIEventListener.VoidDelegate(this.clickBack);
			UIEventListener.Get(this.mJZFHT).onClick = new UIEventListener.VoidDelegate(this.clickItem);
			UIEventListener.Get(this.mXGXJ).onClick = new UIEventListener.VoidDelegate(this.clickItem);
			UIEventListener.Get(this.mMGDLD).onClick = new UIEventListener.VoidDelegate(this.clickItem);
			UIEventListener.Get(this.mATLSSM).onClick = new UIEventListener.VoidDelegate(this.clickItem);
			UIEventListener.Get(this.mSoloRank).onClick = new UIEventListener.VoidDelegate(this.clickRank);
			UIEventListener.Get(this.mTeamRank).onClick = new UIEventListener.VoidDelegate(this.clickRank);
			UIEventListener.Get(this.mRankRule).onClick = new UIEventListener.VoidDelegate(this.clickRankRule);
			UIEventListener.Get(this.mLeagueMGDLD).onClick = new UIEventListener.VoidDelegate(this.clickLeague);
			UIEventListener.Get(this.mLeagueLZXF).onClick = new UIEventListener.VoidDelegate(this.clickLeague);
			UIEventListener.Get(this.mBuyTicketBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.clickBuyTicketBtn);
			UIEventListener.Get(this.mMyReward.gameObject).onClick = new UIEventListener.VoidDelegate(this.clickMyRewardBtn);
			UIEventListener.Get(this.mLeagueRule.gameObject).onClick = new UIEventListener.VoidDelegate(this.clickLeagueRuleBtn);
			UIEventListener.Get(this.mJZFHTSelEasyBtnRoot).onClick = new UIEventListener.VoidDelegate(this.ClickSelFightWithRobotDifficulty);
			UIEventListener.Get(this.mJZFHTSelNormalBtnRoot).onClick = new UIEventListener.VoidDelegate(this.ClickSelFightWithRobotDifficulty);
			UIEventListener.Get(this.mJZFHTSelHardBtnRoot).onClick = new UIEventListener.VoidDelegate(this.ClickSelFightWithRobotDifficulty);
			UIEventListener.Get(this.mXGXJSelEasyBtnRoot).onClick = new UIEventListener.VoidDelegate(this.ClickSelFightWithRobotDifficulty);
			UIEventListener.Get(this.mXGXJSelNormalBtnRoot).onClick = new UIEventListener.VoidDelegate(this.ClickSelFightWithRobotDifficulty);
			UIEventListener.Get(this.mXGXJSelHardBtnRoot).onClick = new UIEventListener.VoidDelegate(this.ClickSelFightWithRobotDifficulty);
			UIEventListener.Get(this.mATLSSMSelEasyBtnRoot).onClick = new UIEventListener.VoidDelegate(this.ClickSelFightWithRobotDifficulty);
			UIEventListener.Get(this.mATLSSMSelNormalBtnRoot).onClick = new UIEventListener.VoidDelegate(this.ClickSelFightWithRobotDifficulty);
			UIEventListener.Get(this.mATLSSMSelHardBtnRoot).onClick = new UIEventListener.VoidDelegate(this.ClickSelFightWithRobotDifficulty);
			UIEventListener.Get(this.mJZFHT).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
			UIEventListener.Get(this.mXGXJ).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
			UIEventListener.Get(this.mMGDLD).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
			UIEventListener.Get(this.mATLSSM).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
			UIEventListener.Get(this.mLeagueMGDLD).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
			UIEventListener.Get(this.mLeagueLZXF).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
			this.mMapGrid.GetComponent<UITweenHelper>().NextPlayDelay = 0.1f;
		}

		public override void RefreshUI()
		{
			switch (this.matchType)
			{
			case MatchType.DP:
				this.mTitle.text = LanguageManager.Instance.GetStringById("PlayUI_ChooseSinglePlayerMap");
				this.mRankPanel.gameObject.SetActive(false);
				this.mLeaguePanel.gameObject.SetActive(false);
				this.mMapGrid.gameObject.SetActive(true);
				this.mTitle.gradientTop = new Color32(27, 162, 251, 255);
				this.mTitle.gradientBottom = new Color32(42, 253, 228, 255);
				this.mJZFHT.gameObject.SetActive(true);
				this.mXGXJ.gameObject.SetActive(true);
				this.mATLSSM.gameObject.SetActive(true);
				this.mMGDLD.gameObject.SetActive(false);
				this.mMapGrid.GetComponent<UITweenHelper>().Play();
				goto IL_48D;
			case MatchType.KH:
				this.mTitle.text = LanguageManager.Instance.GetStringById("PlayUI_ChooseGangUpMap");
				this.mRankPanel.gameObject.SetActive(false);
				this.mLeaguePanel.gameObject.SetActive(false);
				this.mMapGrid.gameObject.SetActive(true);
				this.mTitle.gradientTop = new Color32(27, 162, 251, 255);
				this.mTitle.gradientBottom = new Color32(42, 253, 228, 255);
				this.mJZFHT.gameObject.SetActive(false);
				this.mXGXJ.gameObject.SetActive(true);
				this.mMGDLD.gameObject.SetActive(false);
				this.mATLSSM.gameObject.SetActive(true);
				this.mMapGrid.GetComponent<UITweenHelper>().Play();
				goto IL_48D;
			case MatchType.PWS:
				this.mTitle.text = LanguageManager.Instance.GetStringById("PlayUI_Title_Rank");
				this.mTitle.gradientTop = new Color32(253, 137, 35, 255);
				this.mTitle.gradientBottom = new Color32(237, 199, 44, 255);
				this.mRankPanel.gameObject.SetActive(true);
				this.mLeaguePanel.gameObject.SetActive(false);
				this.mMapGrid.gameObject.SetActive(false);
				this.SetRankInfo(ModelManager.Instance.Get_LadderLevel());
				this.SetRankReward();
				goto IL_48D;
			case MatchType.FightWithRobot:
				this.mTitle.text = LanguageManager.Instance.GetStringById("BattleUI_Title_ChooseRobotMode");
				this.mRankPanel.gameObject.SetActive(false);
				this.mLeaguePanel.gameObject.SetActive(false);
				this.mMapGrid.gameObject.SetActive(true);
				this.mTitle.gradientTop = new Color32(27, 162, 251, 255);
				this.mTitle.gradientBottom = new Color32(42, 253, 228, 255);
				this.mJZFHT.gameObject.SetActive(true);
				this.mXGXJ.gameObject.SetActive(true);
				this.mATLSSM.gameObject.SetActive(true);
				this.mMGDLD.gameObject.SetActive(false);
				this.mMapGrid.GetComponent<UITweenHelper>().Play();
				goto IL_48D;
			case MatchType.JBS:
			{
				this.mTitle.text = "钻石联赛";
				this.mRankPanel.gameObject.SetActive(false);
				this.mMapGrid.gameObject.SetActive(false);
				this.mLeaguePanel.gameObject.SetActive(true);
				this.mTitle.gradientTop = new Color32(27, 162, 251, 255);
				this.mTitle.gradientBottom = new Color32(42, 253, 228, 255);
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在获取奖励信息...", true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.ShowDailyTask, param, new object[0]);
				goto IL_48D;
			}
			}
			this.mTitle.text = LanguageManager.Instance.GetStringById("PlayUI_ChooseSinglePlayerMap");
			IL_48D:
			this.mMapGrid.Reposition();
		}

		[DebuggerHidden]
		private IEnumerator PlayOpenAnim()
		{
			UIPvpEntranceCtrl.<PlayOpenAnim>c__Iterator151 <PlayOpenAnim>c__Iterator = new UIPvpEntranceCtrl.<PlayOpenAnim>c__Iterator151();
			<PlayOpenAnim>c__Iterator.<>f__this = this;
			return <PlayOpenAnim>c__Iterator;
		}

		public override void HandleAfterOpenView()
		{
			this.mJZFHTFightRobotDifficultyRoot.SetActive(false);
			this.mXGXJFightRobotDifficultyRoot.SetActive(false);
			this.mATLSSMFightRobotDifficultyRoot.SetActive(false);
			AutoTestController.InvokeTestLogic(AutoTestTag.EnterPvp, delegate
			{
				this.clickItem(this.mATLSSM);
			}, 1f);
			this.transform.GetComponent<TweenScale>().PlayForward();
			this.transform.GetComponent<TweenAlpha>().PlayForward();
			MobaMessageManager.RegistMessage((ClientMsg)23073, new MobaMessageFunc(this.GetRankRewards));
			this.ShowPressMask(this.mJZFHT, false);
			this.ShowPressMask(this.mXGXJ, false);
			this.ShowPressMask(this.mMGDLD, false);
			this.ShowPressMask(this.mATLSSM, false);
			this.ShowPressMask(this.mLeagueMGDLD, false);
			this.ShowPressMask(this.mLeagueLZXF, false);
		}

		public override void HandleBeforeCloseView()
		{
			MobaMessageManager.RegistMessage((ClientMsg)23073, new MobaMessageFunc(this.GetRankRewards));
			if (this._closeTask != null)
			{
				this.mCoroutineManager.StopCoroutine(this._closeTask);
				this._closeTask = null;
			}
			this.ClearResources();
		}

		private void ClearResources()
		{
			if (!Singleton<PvpRoomView>.Instance.IsOpen)
			{
				HomeGCManager.Instance.ClearChildUiTextureResImmediate(this.gameObject);
			}
		}

		public override void RegisterUpdateHandler()
		{
			MVC_MessageManager.AddListener_view(MobaGameCode.ShowDailyTask, new MobaMessageFunc(this.OnGetMsg_ShowDailyTask));
			MVC_MessageManager.AddListener_view(MobaGameCode.GetDailyTaskAward, new MobaMessageFunc(this.OnGetMsg_GetReward));
			this.RefreshUI();
		}

		public override void CancelUpdateHandler()
		{
			MVC_MessageManager.RemoveListener_view(MobaGameCode.ShowDailyTask, new MobaMessageFunc(this.OnGetMsg_ShowDailyTask));
			MVC_MessageManager.RemoveListener_view(MobaGameCode.GetDailyTaskAward, new MobaMessageFunc(this.OnGetMsg_GetReward));
		}

		private void clickRankRule(GameObject obj = null)
		{
			CtrlManager.OpenWindow(WindowID.RegularView, null);
			Singleton<RegularView>.Instance.AddRule(LanguageManager.Instance.GetStringById("RankUI_RuleTitle_1"), LanguageManager.Instance.GetStringById("RankUI_RuleDesc_1"));
			Singleton<RegularView>.Instance.AddRule(LanguageManager.Instance.GetStringById("RankUI_RuleTitle_2"), LanguageManager.Instance.GetStringById("RankUI_RuleDesc_2"));
			Singleton<RegularView>.Instance.AddRule(LanguageManager.Instance.GetStringById("RankUI_RuleTitle_3"), LanguageManager.Instance.GetStringById("RankUI_RuleDesc_3"));
			Singleton<RegularView>.Instance.AddRule(LanguageManager.Instance.GetStringById("RankUI_RuleTitle_4"), LanguageManager.Instance.GetStringById("RankUI_RuleDesc_4"));
			Singleton<RegularView>.Instance.AddRule(LanguageManager.Instance.GetStringById("RankUI_RuleTitle_5"), LanguageManager.Instance.GetStringById("RankUI_RuleDesc_5"));
			Singleton<RegularView>.Instance.AddRule(LanguageManager.Instance.GetStringById("RankUI_RuleTitle_6"), LanguageManager.Instance.GetStringById("RankUI_RuleDesc_6"));
			Singleton<RegularView>.Instance.ShowRule();
		}

		private void clickLeagueRuleBtn(GameObject go)
		{
			CtrlManager.OpenWindow(WindowID.RegularView, null);
			Singleton<RegularView>.Instance.AddRule(LanguageManager.Instance.GetStringById("DiamondLeague_RuleDesc_Title_1"), LanguageManager.Instance.GetStringById("DiamondLeague_RuleDesc_Text_1"));
			Singleton<RegularView>.Instance.AddRule(LanguageManager.Instance.GetStringById("DiamondLeague_RuleDesc_Title_2"), LanguageManager.Instance.GetStringById("DiamondLeague_RuleDesc_Text_2"));
			Singleton<RegularView>.Instance.AddRule(LanguageManager.Instance.GetStringById("DiamondLeague_RuleDesc_Title_3"), LanguageManager.Instance.GetStringById("DiamondLeague_RuleDesc_Text_3"));
			Singleton<RegularView>.Instance.AddRule(LanguageManager.Instance.GetStringById("DiamondLeague_RuleDesc_Title_4"), LanguageManager.Instance.GetStringById("DiamondLeague_RuleDesc_Text_4"));
			Singleton<RegularView>.Instance.ShowRule();
		}

		private void clickBack(GameObject obj = null)
		{
			this.DelayCloseView();
		}

		public void DelayCloseView()
		{
			if (this.mRankPanel != null)
			{
				this.mRankPanel.FindChild("RankIcon").GetComponent<RankIconEffectPlayerTools>().SetEffectActive(false, 0f);
			}
			if (this._closeTask != null)
			{
				return;
			}
			this.mCoroutineManager.StartCoroutine(this.DelayClose(), true);
			if (this.transform == null)
			{
				return;
			}
			if (this.transform.GetComponent<TweenScale>() == null)
			{
				return;
			}
			this.transform.GetComponent<TweenScale>().PlayReverse();
			this.transform.GetComponent<TweenAlpha>().PlayReverse();
		}

		[DebuggerHidden]
		private IEnumerator DelayClose()
		{
			return new UIPvpEntranceCtrl.<DelayClose>c__Iterator152();
		}

		private void clickItem(GameObject obj = null)
		{
			if (this.matchType == MatchType.FightWithRobot)
			{
				this.OnReqFightWithRobot(obj);
				AutoTestController.InvokeTestLogic(AutoTestTag.EnterPvp, delegate
				{
					this.ClickSelFightWithRobotDifficulty(this.mATLSSMSelEasyBtnRoot);
				}, 1f);
				return;
			}
			this.mCoroutineManager.StartCoroutine(this.clickItemCoroutine(obj), true);
		}

		private void OnReqFightWithRobot(GameObject inObj)
		{
			if (inObj == null)
			{
				return;
			}
			this.mJZFHTFightRobotDifficultyRoot.SetActive(false);
			this.mXGXJFightRobotDifficultyRoot.SetActive(false);
			this.mATLSSMFightRobotDifficultyRoot.SetActive(false);
			if (inObj == this.mJZFHT)
			{
				this.mJZFHTFightRobotDifficultyRoot.SetActive(true);
			}
			else if (inObj == this.mXGXJ)
			{
				this.mXGXJFightRobotDifficultyRoot.SetActive(true);
			}
			else if (inObj == this.mATLSSM)
			{
				this.mATLSSMFightRobotDifficultyRoot.SetActive(true);
			}
		}

		private void ClickSelFightWithRobotDifficulty(GameObject obj = null)
		{
			if (obj == null)
			{
				return;
			}
			bool flag = false;
			ESceneBelongedBattleType inSceneBelongedType = ESceneBelongedBattleType.None;
			int inGroupPlayerCount = 0;
			if (obj == this.mJZFHTSelEasyBtnRoot)
			{
				flag = true;
				inSceneBelongedType = ESceneBelongedBattleType.FightWithRobotEasy;
				inGroupPlayerCount = 1;
			}
			else if (obj == this.mJZFHTSelNormalBtnRoot)
			{
				flag = true;
				inSceneBelongedType = ESceneBelongedBattleType.FightWithRobotNormal;
				inGroupPlayerCount = 1;
			}
			else if (obj == this.mJZFHTSelHardBtnRoot)
			{
				flag = true;
				inSceneBelongedType = ESceneBelongedBattleType.FightWithRobotHard;
				inGroupPlayerCount = 1;
			}
			else if (obj == this.mXGXJSelEasyBtnRoot)
			{
				flag = true;
				inSceneBelongedType = ESceneBelongedBattleType.FightWithRobotEasy;
				inGroupPlayerCount = 3;
			}
			else if (obj == this.mXGXJSelNormalBtnRoot)
			{
				flag = true;
				inSceneBelongedType = ESceneBelongedBattleType.FightWithRobotNormal;
				inGroupPlayerCount = 3;
			}
			else if (obj == this.mXGXJSelHardBtnRoot)
			{
				flag = true;
				inSceneBelongedType = ESceneBelongedBattleType.FightWithRobotHard;
				inGroupPlayerCount = 3;
			}
			else if (obj == this.mATLSSMSelEasyBtnRoot)
			{
				flag = true;
				inSceneBelongedType = ESceneBelongedBattleType.FightWithRobotEasy;
				inGroupPlayerCount = 5;
			}
			else if (obj == this.mATLSSMSelNormalBtnRoot)
			{
				flag = true;
				inSceneBelongedType = ESceneBelongedBattleType.FightWithRobotNormal;
				inGroupPlayerCount = 5;
			}
			else if (obj == this.mATLSSMSelHardBtnRoot)
			{
				flag = true;
				inSceneBelongedType = ESceneBelongedBattleType.FightWithRobotHard;
				inGroupPlayerCount = 5;
			}
			if (!flag)
			{
				return;
			}
			int num = 0;
			if (!this.GetBattleIdBySceneBelongedTypeAndPlayerCount(inSceneBelongedType, inGroupPlayerCount, out num))
			{
				return;
			}
			object[] msgParam = new object[]
			{
				num,
				this.matchType
			};
			MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)26002, msgParam, 0f);
			MobaMessageManager.ExecuteMsg(message);
		}

		private bool GetBattleIdBySceneBelongedTypeAndPlayerCount(ESceneBelongedBattleType inSceneBelongedType, int inGroupPlayerCount, out int outBattleId)
		{
			outBattleId = 0;
			Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysBattleSceneVo>();
			foreach (KeyValuePair<string, object> current in dicByType)
			{
				SysBattleSceneVo sysBattleSceneVo = current.Value as SysBattleSceneVo;
				if (sysBattleSceneVo != null && sysBattleSceneVo.belonged_battletype == (int)inSceneBelongedType && sysBattleSceneVo.hero1_number_cap == inGroupPlayerCount && int.TryParse(sysBattleSceneVo.scene_id, out outBattleId))
				{
					return true;
				}
			}
			return false;
		}

		[DebuggerHidden]
		private IEnumerator clickItemCoroutine(GameObject obj = null)
		{
			UIPvpEntranceCtrl.<clickItemCoroutine>c__Iterator153 <clickItemCoroutine>c__Iterator = new UIPvpEntranceCtrl.<clickItemCoroutine>c__Iterator153();
			<clickItemCoroutine>c__Iterator.obj = obj;
			<clickItemCoroutine>c__Iterator.<$>obj = obj;
			<clickItemCoroutine>c__Iterator.<>f__this = this;
			return <clickItemCoroutine>c__Iterator;
		}

		public void NewbieFakeMatchFiveSelMap()
		{
			this.mCoroutineManager.StartCoroutine(this.NewbieFakeMatchFiveCoroutine(), true);
		}

		[DebuggerHidden]
		private IEnumerator NewbieFakeMatchFiveCoroutine()
		{
			UIPvpEntranceCtrl.<NewbieFakeMatchFiveCoroutine>c__Iterator154 <NewbieFakeMatchFiveCoroutine>c__Iterator = new UIPvpEntranceCtrl.<NewbieFakeMatchFiveCoroutine>c__Iterator154();
			<NewbieFakeMatchFiveCoroutine>c__Iterator.<>f__this = this;
			return <NewbieFakeMatchFiveCoroutine>c__Iterator;
		}

		private void clickRank(GameObject obj = null)
		{
			if (null == obj)
			{
				return;
			}
			MatchType matchType = MatchType.PWS;
			if (obj == this.mSoloRank)
			{
				matchType = MatchType.DP;
			}
			else if (obj == this.mTeamRank)
			{
				matchType = MatchType.KH;
			}
			object[] msgParam = new object[]
			{
				this.GetMapInfoDatasByType(MatchType.PWS)[0].battleId,
				matchType
			};
			MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)26002, msgParam, 0f);
			MobaMessageManager.ExecuteMsg(message);
		}

		private void SetRankInfo(SysRankStageVo vo)
		{
			UserData userData = ModelManager.Instance.Get_userData_X();
			if (userData == null)
			{
				this.mRankSeasonLabel.text = LanguageManager.Instance.GetStringById("RankUI_ChooseMode_Season");
			}
			else
			{
				this.mRankSeasonLabel.text = string.Format("本赛季时间: {0}~{1}", userData.SeasonStartTime.ToString("yyyy.M.d"), userData.SeasonEndTime.ToString("yyyy.M.d"));
			}
			UILabel componentInChildren;
			if (!this.IsHeroEnough())
			{
				this.mSoloRank.GetComponent<BoxCollider>().enabled = false;
				componentInChildren = this.mSoloRank.GetComponentInChildren<UILabel>();
				componentInChildren.gradientTop = Color.white;
				componentInChildren.gradientBottom = new Color32(232, 232, 232, 255);
				componentInChildren.effectColor = new Color32(63, 63, 63, 255);
				this.mTeamRank.GetComponent<BoxCollider>().enabled = false;
				componentInChildren = this.mTeamRank.GetComponentInChildren<UILabel>();
				componentInChildren.gradientTop = Color.white;
				componentInChildren.gradientBottom = new Color32(232, 232, 232, 255);
				componentInChildren.effectColor = new Color32(63, 63, 63, 255);
				this.mRankPanel.FindChild("LockRank").gameObject.SetActive(true);
				this.mRankPanel.FindChild("RankIcon").gameObject.SetActive(false);
				this.mRankPanel.FindChild("RankCenter/Desc").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("RankUI_ChooseMode_EntryRequirements");
				UILabel component = this.mRankPanel.FindChild("RankCenter/Level").GetComponent<UILabel>();
				component.text = LanguageManager.Instance.GetStringById("RankUI_ChooseMode_LimitHero");
				component.fontSize = 48;
				component.applyGradient = false;
				component.color = new Color32(254, 250, 211, 255);
				this.mRankPanel.FindChild("RankCenter/Point").GetComponent<UILabel>().text = "[fc0019]" + CharacterDataMgr.instance.OwenHeros.Count.ToString() + "[-][2bfee4]/5[-]";
				return;
			}
			int rankStage = vo.RankStage;
			Transform transform = this.mRankPanel.FindChild("RankIcon");
			this.mSoloRank.GetComponent<BoxCollider>().enabled = true;
			this.mSoloRank.GetComponent<UIButton>().state = UIButtonColor.State.Normal;
			componentInChildren = this.mSoloRank.GetComponentInChildren<UILabel>();
			componentInChildren.gradientTop = new Color32(254, 254, 253, 255);
			componentInChildren.gradientBottom = new Color32(253, 228, 208, 255);
			componentInChildren.effectColor = new Color32(126, 54, 10, 255);
			this.mTeamRank.GetComponent<BoxCollider>().enabled = true;
			this.mTeamRank.GetComponent<UIButton>().state = UIButtonColor.State.Normal;
			componentInChildren = this.mTeamRank.GetComponentInChildren<UILabel>();
			componentInChildren.gradientTop = new Color32(254, 254, 253, 255);
			componentInChildren.gradientBottom = new Color32(253, 228, 208, 255);
			componentInChildren.effectColor = new Color32(126, 54, 10, 255);
			string text = ModelManager.Instance.Get_userData_X().LadderScore.ToString("f0");
			int num = int.Parse(text);
			this.mRankPanel.FindChild("LockRank").gameObject.SetActive(false);
			transform.gameObject.SetActive(true);
			transform.GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>(vo.StageImg, true, true, null, 0, false);
			this.mRankPanel.FindChild("RankCenter/Desc").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("RankUI_ChooseMode_MyRank");
			this.mRankPanel.FindChild("RankCenter/Level").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(vo.StageName);
			this.mRankPanel.FindChild("RankCenter/Level").GetComponent<UILabel>().applyGradient = true;
			this.mRankPanel.FindChild("RankCenter/Level").GetComponent<UILabel>().gradientTop = ModelManager.Instance.Get_ColorByString_X(vo.GradientTop);
			this.mRankPanel.FindChild("RankCenter/Level").GetComponent<UILabel>().gradientBottom = ModelManager.Instance.Get_ColorByString_X(vo.GradientBottom);
			this.mRankPanel.FindChild("RankCenter/Point").GetComponent<UILabel>().text = text;
			RankIconEffectPlayerTools component2 = transform.GetComponent<RankIconEffectPlayerTools>();
			component2.RankLevel = vo.RankStage;
			component2.SortPanel = this.transform.GetComponent<UIPanel>();
			component2.SortWidget = transform.GetComponent<UITexture>();
			component2.SetEffectActive(true, 0f);
			component2.SetScale(transform.GetComponent<UITexture>().width);
			SysRankStageVo sysRankStageVo = (SysRankStageVo)BaseDataMgr.instance.GetDicByType<SysRankStageVo>().Values.LastOrDefault<object>();
			if (rankStage == sysRankStageVo.RankStage)
			{
				this.mRankPanel.FindChild("Sth.Extra").gameObject.SetActive(true);
				this.mRankPanel.FindChild("RankCenter/ProgBarBg/ProgBarFore").GetComponent<UISprite>().fillAmount = 1f;
			}
			else
			{
				this.mRankPanel.FindChild("Sth.Extra").gameObject.SetActive(false);
				this.mRankPanel.FindChild("RankCenter/ProgBarBg/ProgBarFore").GetComponent<UISprite>().fillAmount = (float)(num - vo.StageScore) / (float)(ModelManager.Instance.Get_NextLadderLevel().StageScore - vo.StageScore);
			}
		}

		private void SetRankReward()
		{
			List<SysRankStageVo> list = BaseDataMgr.instance.GetDicByType<SysRankStageVo>().Values.OfType<SysRankStageVo>().ToList<SysRankStageVo>();
			List<SysRankStageVo> list2 = new List<SysRankStageVo>();
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].activity_task_id.Equals("[]") && !list[i].activity_id.Equals("[]"))
				{
					list2.Add(list[i]);
				}
			}
			bool active = false;
			for (int j = 0; j < list2.Count; j++)
			{
				ActivityTaskData activityTaskData = ModelManager.Instance.Get_Activity_taskData(int.Parse(list2[j].activity_task_id));
				if (activityTaskData != null && (activityTaskData.taskstate != 2 || j + 1 == list2.Count))
				{
					this.SetCurrentReward(activityTaskData, list2[j]);
					active = true;
					break;
				}
			}
			SysRankStageVo sysRankStageVo = list2.Find((SysRankStageVo obj) => obj.RankStage == 3 && obj.RankStageLevel == 1);
			if (sysRankStageVo == null)
			{
				this.mRankSeason_GoldTarget.gameObject.SetActive(false);
			}
			else
			{
				ActivityTaskData activityTaskData = ModelManager.Instance.Get_Activity_taskData(int.Parse(sysRankStageVo.activity_task_id));
				if (activityTaskData != null)
				{
					active = true;
					this.SetGoldReward(activityTaskData, sysRankStageVo);
				}
			}
			this.mRankSeasonReward.gameObject.SetActive(active);
		}

		private void SetCurrentReward(ActivityTaskData _taskData, SysRankStageVo _stageVo)
		{
			if (_taskData == null)
			{
				this.mRankSeason_CurrentTarget.gameObject.SetActive(false);
				return;
			}
			string stringById = LanguageManager.Instance.GetStringById("Rank_Stage_Next_Reward_Desc");
			this.mRankSeason_CurrentTarget.FindChild("Label").GetComponent<UILabel>().text = stringById.Replace("#", LanguageManager.Instance.GetStringById(_stageVo.StageName));
			SysActivityVo _activityVo = BaseDataMgr.instance.GetDataById<SysActivityVo>(_stageVo.activity_id.ToString());
			if (_activityVo == null)
			{
				return;
			}
			if (_taskData.taskstate == 2)
			{
				this.mCurrentTarget_BtnCtrl.SetButtonState(2);
			}
			else
			{
				this.mCurrentTarget_BtnCtrl.SetButtonState(_taskData.taskstate);
				this.mCurrentTarget_BtnCtrl.OnClick = delegate(GameObject obj)
				{
					CtrlManager.OpenWindow(WindowID.ActivityView, null);
					MsgData_Activity_setCurActivity param = new MsgData_Activity_setCurActivity
					{
						activity_typeID = _activityVo.activity_type_id,
						activity_id = _activityVo.id
					};
					MobaMessageManagerTools.SendClientMsg(ClientV2V.Activity_setCurActivity, param, false);
				};
				this.mCurrentTarget_BtnCtrl.SetClickEvent();
			}
			SysActivityTaskVo dataById = BaseDataMgr.instance.GetDataById<SysActivityTaskVo>(_taskData.taskid.ToString());
			string[] array = ToolsFacade.Instance.AnalyseDropRewardsPackage(dataById.reward);
			int num = 0;
			int count = 0;
			for (int i = 0; i < 3; i++)
			{
				if (i < array.Length)
				{
					this.mCurrentTarget_Items[i].gameObject.SetActive(true);
					ItemType itemType = ToolsFacade.Instance.AnalyzeDropItemById(array[i], out num, out count);
					this.mCurrentTarget_Items[i].SetData(itemType, num.ToString(), false, true, count);
				}
				else
				{
					this.mCurrentTarget_Items[i].gameObject.SetActive(false);
				}
			}
		}

		private void SetGoldReward(ActivityTaskData _taskData, SysRankStageVo _stageVo)
		{
			if (_taskData == null)
			{
				this.mRankSeason_GoldTarget.gameObject.SetActive(false);
				return;
			}
			this.mRankSeason_GoldTarget.FindChild("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Rank_Stage_Gold_Reward_Desc");
			SysActivityVo _activityVo = BaseDataMgr.instance.GetDataById<SysActivityVo>(_stageVo.activity_id.ToString());
			if (_activityVo == null)
			{
				return;
			}
			if (_taskData.taskstate == 2)
			{
				this.mGoldTarget_BtnCtrl.SetButtonState(2);
			}
			else
			{
				this.mGoldTarget_BtnCtrl.SetButtonState(_taskData.taskstate);
				this.mGoldTarget_BtnCtrl.OnClick = delegate(GameObject obj)
				{
					CtrlManager.OpenWindow(WindowID.ActivityView, null);
					MsgData_Activity_setCurActivity param = new MsgData_Activity_setCurActivity
					{
						activity_typeID = _activityVo.activity_type_id,
						activity_id = _activityVo.id
					};
					MobaMessageManagerTools.SendClientMsg(ClientV2V.Activity_setCurActivity, param, false);
				};
				this.mGoldTarget_BtnCtrl.SetClickEvent();
			}
			SysActivityTaskVo dataById = BaseDataMgr.instance.GetDataById<SysActivityTaskVo>(_taskData.taskid.ToString());
			string[] array = ToolsFacade.Instance.AnalyseDropRewardsPackage(dataById.reward);
			ItemType itemType = ItemType.None;
			int num = 0;
			int count = 0;
			if (array.Length > 0)
			{
				itemType = ToolsFacade.Instance.AnalyzeDropItemById(array[0], out num, out count);
			}
			this.mGoldTarget_Item.SetData(itemType, num.ToString(), false, true, count);
		}

		private void SetLeagueInfo()
		{
			this.PurchaseSuccess();
			DailyTaskData dailyTaskData = ModelManager.Instance.Get_AchieveAll_X().dailyTaskList.Find((DailyTaskData obj) => obj.taskid == 150);
			if (dailyTaskData == null)
			{
				return;
			}
			SysAchievementDailyVo dataById = BaseDataMgr.instance.GetDataById<SysAchievementDailyVo>(dailyTaskData.taskid.ToString());
			if (dailyTaskData != null && dataById != null)
			{
				this.mTaskNum.text = "/" + dataById.parameter.ToString();
				this.mMyWinTimes.text = dailyTaskData.value.ToString();
				if (dailyTaskData.value >= dataById.parameter && !dailyTaskData.isGetAward)
				{
					this.mMyReward.GetComponent<UISprite>().spriteName = "championships_icons_box";
					this.mMyReward.GetComponent<BoxCollider>().enabled = true;
					this.mMyReward.GetComponent<Animator>().enabled = true;
					this.mMyWinTimes.text = dataById.parameter.ToString();
				}
				else if (dailyTaskData.value < dataById.parameter)
				{
					this.mMyReward.GetComponent<UISprite>().spriteName = "championships_icons_box";
					this.mMyReward.GetComponent<BoxCollider>().enabled = false;
					this.mMyReward.GetComponent<Animator>().enabled = false;
				}
				else if (dailyTaskData.isGetAward)
				{
					this.mMyReward.GetComponent<UISprite>().spriteName = "championships_icons_box_open";
					this.mMyReward.GetComponent<BoxCollider>().enabled = false;
					this.mMyReward.GetComponent<Animator>().enabled = false;
				}
			}
			bool flag = LevelManager.Instance.IsLevelOpen("80007");
			bool flag2 = LevelManager.Instance.IsLevelOpen("80022");
			this.mLeagueLZXF.transform.Find("Texture").gameObject.SetActive(flag);
			this.mLeagueLZXF.transform.Find("DarkTexture").gameObject.SetActive(!flag);
			this.mLeagueLZXF.transform.GetComponent<BoxCollider>().enabled = flag;
			this.mLeagueMGDLD.transform.Find("Texture").gameObject.SetActive(flag2);
			this.mLeagueMGDLD.transform.Find("DarkTexture").gameObject.SetActive(!flag2);
			this.mLeagueMGDLD.transform.GetComponent<BoxCollider>().enabled = flag2;
			if (flag)
			{
				this.mMapName.text = LanguageManager.Instance.GetStringById(this.mBattleSceneDict["80007"].scene_name);
				this.mMapName.gradientTop = new Color32(254, 250, 136, 255);
				this.mMapName.gradientBottom = new Color32(254, 249, 219, 255);
				this.mMapType.text = "3v3v3";
				this.mMapType.gradientTop = new Color32(254, 250, 136, 255);
				this.mMapType.gradientBottom = new Color32(254, 249, 219, 255);
			}
			if (flag2)
			{
				this.mMapName.text = LanguageManager.Instance.GetStringById(this.mBattleSceneDict["80022"].scene_name);
				this.mMapName.gradientTop = new Color32(252, 161, 39, 255);
				this.mMapName.gradientBottom = new Color32(254, 243, 192, 255);
				this.mMapType.text = this.mBattleSceneDict["80022"].hero1_number_cap.ToString() + "v" + this.mBattleSceneDict["80022"].hero1_number_cap.ToString();
				this.mMapType.gradientTop = new Color32(252, 161, 39, 255);
				this.mMapType.gradientBottom = new Color32(254, 243, 192, 255);
			}
		}

		private bool IsHeroEnough()
		{
			return CharacterDataMgr.instance.OwenHeros.Count >= 5;
		}

		private void InitMapDict()
		{
			this.mMapDict.Clear();
			this.mMapDict.Add(1, this.GetMapInfoDatasByType(MatchType.KH));
			this.mMapDict.Add(0, this.GetMapInfoDatasByType(MatchType.DP));
			this.mMapDict.Add(3, this.GetMapInfoDatasByType(MatchType.PWS));
			this.mMapDict.Add(5, this.GetMapInfoDatasByType(MatchType.JBS));
		}

		private void ShowPressMask(GameObject obj, bool isPress)
		{
			if (obj == null)
			{
				return;
			}
			GameObject gameObject = obj.transform.FindChild("PressMask").gameObject;
			if (gameObject == null)
			{
				return;
			}
			gameObject.SetActive(isPress);
		}

		private SysBattleSceneVo GetBattleSceneCfg(string sceneId)
		{
			if (!this.mBattleSceneDict.ContainsKey(sceneId))
			{
				SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(sceneId);
				this.mBattleSceneDict.Add(sceneId, dataById);
			}
			return this.mBattleSceneDict[sceneId];
		}

		private MapInfoData BuildMapInfoData(SysBattleSceneVo arg, string time = "?-?", string reward = "?")
		{
			int id = int.Parse(arg.scene_id);
			string scene_name = arg.scene_name;
			string playersNum = string.Format("{0}V{0}", arg.hero1_number_cap);
			return new MapInfoData(id, time, scene_name, playersNum, reward);
		}

		public List<MapInfoData> GetMapInfoDatasByType(MatchType tp = MatchType.DP)
		{
			List<MapInfoData> list = new List<MapInfoData>();
			switch (tp)
			{
			case MatchType.DP:
				if (GlobalSettings.Instance.PvpSetting.test3v3)
				{
					list.Add(this.BuildMapInfoData(this.GetBattleSceneCfg("80007"), "PlayUI_AverageGameTime3to5", "80"));
				}
				else
				{
					list.Add(this.BuildMapInfoData(this.GetBattleSceneCfg("80001"), "PlayUI_AverageGameTime3to5", "80"));
				}
				list.Add(this.BuildMapInfoData(this.GetBattleSceneCfg("80003"), "PlayUI_AverageGameTime5to10", "80"));
				list.Add(this.BuildMapInfoData(this.GetBattleSceneCfg("800055"), "PlayUI_AverageGameTime5to10", "80"));
				break;
			case MatchType.KH:
				list.Add(this.BuildMapInfoData(this.GetBattleSceneCfg("80001"), "PlayUI_AverageGameTime5to8", "80"));
				list.Add(this.BuildMapInfoData(this.GetBattleSceneCfg("80003"), "PlayUI_AverageGameTime5to10", "80"));
				list.Add(this.BuildMapInfoData(this.GetBattleSceneCfg("800055"), "PlayUI_AverageGameTime5to10", "80"));
				break;
			case MatchType.PWS:
				list.Add(this.BuildMapInfoData(this.GetBattleSceneCfg("80006"), "PlayUI_AverageGameTime5to8", "80"));
				break;
			case MatchType.JBS:
				list.Add(this.BuildMapInfoData(this.GetBattleSceneCfg("80007"), "PlayUI_AverageGameTime5to8", "80"));
				list.Add(this.BuildMapInfoData(this.GetBattleSceneCfg("80022"), "PlayUI_AverageGameTime5to10", "80"));
				break;
			}
			return list;
		}

		private void clickLeague(GameObject go)
		{
			if (null == go)
			{
				return;
			}
			if (ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId == 8000) == null)
			{
				CtrlManager.ShowMsgBox("提示", "您的门票不足，请购买门票", new Action<bool>(this.OpenShopCallback), PopViewType.PopTwoButton, "确定", "取消", null);
			}
			else
			{
				MatchType matchType = MatchType.JBS;
				object[] msgParam;
				if (go.name == this.mLeagueLZXF.name)
				{
					msgParam = new object[]
					{
						this.GetMapInfoDatasByType(MatchType.JBS)[0].battleId,
						matchType
					};
				}
				else
				{
					msgParam = new object[]
					{
						this.GetMapInfoDatasByType(MatchType.JBS)[1].battleId,
						matchType
					};
				}
				MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)26002, msgParam, 0f);
				MobaMessageManager.ExecuteMsg(message);
			}
		}

		private void OpenShopCallback(bool isOK)
		{
			if (isOK)
			{
				CtrlManager.OpenWindow(WindowID.PurchasePopupView, null);
				Singleton<PurchasePopupView>.Instance.onSuccess.Add(new Callback(this.PurchaseSuccess));
				Singleton<PurchasePopupView>.Instance.Show(ETypicalCommodity.DiamondLeagueTicket, false);
			}
		}

		private void clickBuyTicketBtn(GameObject go)
		{
			CtrlManager.OpenWindow(WindowID.PurchasePopupView, null);
			Singleton<PurchasePopupView>.Instance.onSuccess.Add(new Callback(this.PurchaseSuccess));
			Singleton<PurchasePopupView>.Instance.Show(ETypicalCommodity.DiamondLeagueTicket, false);
		}

		private void clickMyRewardBtn(GameObject go)
		{
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在加载信息", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetDailyTaskAward, param, new object[]
			{
				150
			});
		}

		private void PurchaseSuccess()
		{
			EquipmentInfoData equipmentInfoData = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId == 8000);
			this.mMyTicket.text = ((equipmentInfoData != null) ? equipmentInfoData.Count.ToString() : "0");
		}

		private void OnGetMsg_GetReward(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				int num2 = (int)operationResponse.Parameters[123];
				Singleton<MenuBottomBarView>.Instance.RemoveNews(1, num2.ToString());
				this.GetLeagueReward();
			}
		}

		private void GetRankRewards(MobaMessage msg)
		{
			if (this.mRankPanel != null && this.mRankPanel.gameObject.activeInHierarchy)
			{
				this.SetRankReward();
			}
		}

		private void GetLeagueReward()
		{
			List<DailyTaskData> dailyTaskList = ModelManager.Instance.Get_AchieveAll_X().dailyTaskList;
			SysAchievementDailyVo sysAchievementDailyVo = new SysAchievementDailyVo();
			SysDropRewardsVo sysDropRewardsVo = new SysDropRewardsVo();
			SysDropItemsVo sysDropItemsVo = new SysDropItemsVo();
			CtrlManager.OpenWindow(WindowID.GetItemsView, null);
			for (int i = 0; i < dailyTaskList.Count; i++)
			{
				if (dailyTaskList[i].taskid == 150)
				{
					sysAchievementDailyVo = BaseDataMgr.instance.GetDataById<SysAchievementDailyVo>(dailyTaskList[i].taskid.ToString());
					sysDropRewardsVo = BaseDataMgr.instance.GetDataById<SysDropRewardsVo>(sysAchievementDailyVo.reward);
					sysDropItemsVo = BaseDataMgr.instance.GetDataById<SysDropItemsVo>(sysDropRewardsVo.drop_items);
					string[] rewardInfo = sysDropItemsVo.rewards.Split(new char[]
					{
						'|'
					});
					string text = rewardInfo[0];
					switch (text)
					{
					case "1":
						if (rewardInfo[1] == "1")
						{
							MobaMessageManagerTools.GetItems_Coin(int.Parse(rewardInfo[800]));
							ModelManager.Instance.Get_userData_X().Money += (long)int.Parse(rewardInfo[2]);
						}
						else if (rewardInfo[1] == "2")
						{
							MobaMessageManagerTools.GetItems_Diamond(int.Parse(rewardInfo[2]));
							ModelManager.Instance.Get_userData_X().Diamonds += (long)int.Parse(rewardInfo[2]);
						}
						else if (rewardInfo[1] == "9")
						{
							MobaMessageManagerTools.GetItems_Caps(int.Parse(rewardInfo[2]));
							ModelManager.Instance.Get_userData_X().SmallCap += int.Parse(rewardInfo[2]);
						}
						else if (rewardInfo[1] == "11")
						{
							ModelManager.Instance.Get_userData_X().Speaker += int.Parse(rewardInfo[2]);
							MobaMessageManagerTools.GetItems_Speaker(int.Parse(rewardInfo[2]));
						}
						break;
					case "2":
					{
						SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(rewardInfo[1]);
						if (dataById.type == 4)
						{
							MobaMessageManagerTools.GetItems_Rune(int.Parse(rewardInfo[1]));
						}
						if (rewardInfo[1] == "7777")
						{
							MobaMessageManagerTools.GetItems_Bottle(int.Parse(rewardInfo[2]));
						}
						break;
					}
					case "3":
						if (rewardInfo[1] == "1")
						{
							Dictionary<string, SysHeroMainVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysHeroMainVo>();
							string npc_id = typeDicByType.Values.FirstOrDefault((SysHeroMainVo obj) => obj.hero_id == int.Parse(rewardInfo[2])).npc_id;
							MobaMessageManagerTools.GetItems_Hero(npc_id);
						}
						else if (rewardInfo[1] == "2")
						{
							MobaMessageManagerTools.GetItems_HeroSkin(int.Parse(rewardInfo[2]));
						}
						else if (rewardInfo[1] == "3")
						{
							MobaMessageManagerTools.GetItems_HeadPortrait(int.Parse(rewardInfo[2]));
						}
						else if (rewardInfo[1] == "4")
						{
							MobaMessageManagerTools.GetItems_PortraitFrame(rewardInfo[2]);
						}
						else if (rewardInfo[1] == "5")
						{
							MobaMessageManagerTools.GetItems_Coupon(rewardInfo[2]);
						}
						break;
					case "4":
						if (rewardInfo[1] == "1")
						{
							MobaMessageManagerTools.GetItems_Exp(int.Parse(rewardInfo[2]), ModelManager.Instance.Get_userData_X().Exp);
							ModelManager.Instance.Get_userData_X().Exp += (long)int.Parse(rewardInfo[2]);
							CharacterDataMgr.instance.SaveNowUserLevel(ModelManager.Instance.Get_userData_X().Exp);
						}
						break;
					case "6":
						MobaMessageManagerTools.GetItems_GameBuff(Convert.ToInt32(rewardInfo[1]));
						SendMsgManager.Instance.SendMsg(MobaGameCode.GetDoubleCard, null, new object[0]);
						break;
					}
					Singleton<MenuTopBarView>.Instance.RefreshUI();
					dailyTaskList[i].isGetAward = true;
					Singleton<GetItemsView>.Instance.onFinish = new Callback(this.GotReward);
					Singleton<GetItemsView>.Instance.Play();
				}
			}
		}

		private void GotReward()
		{
			this.mMyReward.GetComponent<UISprite>().spriteName = "championships_icons_box_open";
			this.mMyReward.GetComponent<BoxCollider>().enabled = false;
			this.mMyReward.GetComponent<Animator>().enabled = false;
		}

		private void OnGetMsg_ShowDailyTask(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				this.SetLeagueInfo();
			}
		}
	}
}
