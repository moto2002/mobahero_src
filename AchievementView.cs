using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class AchievementView : BaseView<AchievementView>
{
	private Transform leftAnchor;

	private Transform centerAnchor;

	private Transform rightAnchor;

	private Transform mainBtn;

	public Transform historyBtn;

	private Transform honorBtn;

	private Transform momentBtn;

	private Transform heroesBtn;

	private Transform abilityBtn;

	private UISprite mainPic;

	private UISprite historyPic;

	private UISprite honorPic;

	private UISprite momentPic;

	private UISprite heroPic;

	private UISprite abilityPic;

	private UILabel mainTex;

	private UILabel historyTex;

	private UILabel honorTex;

	private UILabel momentTex;

	private UILabel heroTex;

	private UILabel abilityTex;

	private UIPanel mainPanel;

	private UIPanel historyPanel;

	private UIPanel honorPanel;

	private UIPanel momentPanel;

	private UIPanel heroesPanel;

	private UIPanel abilityPanel;

	private UITexture myPortrait;

	private UILabel myName;

	private UILabel myRank;

	private UILabel like;

	private UILabel achievCount;

	private UILabel KDA;

	private UILabel K_D_A;

	private UILabel JDDZCount;

	private UILabel LZXFCount;

	private UILabel SDDLDCount;

	private UILabel JJCCount;

	private UILabel JDDZPercent;

	private UILabel LZXFPercent;

	private UILabel SDDLDPercent;

	private UILabel JJCPercent;

	private UISprite JDDZBar;

	private UISprite LZXFBar;

	private UISprite SDDLDBar;

	private UISprite JJCBar;

	private UISprite badge;

	private UILabel scoreNum;

	private UILabel tripleKill;

	private UILabel quataryKill;

	private UILabel pentaKill;

	private UILabel legendary;

	private UITexture hero1;

	private UITexture hero2;

	private UITexture hero3;

	private Transform heroGrid;

	private Transform noHero;

	private Transform VBtnInMain;

	public Transform VBtnInHistory;

	public Transform VBtnPanel;

	private Transform vBtnL;

	private Transform vBtnR;

	private Transform btn5;

	private Transform btn10;

	private Transform btn15;

	private Transform btn20;

	private Transform bg1;

	private Transform nextPage;

	private Transform xiba;

	private Transform noRecord;

	private UISprite bg;

	private UILabel page;

	private UILabel pageNum;

	private Transform btnVR;

	private Transform btnVL;

	private UILabel title;

	private UILabel time;

	private Transform detailBtn;

	private Transform closeDetailBtn;

	private Transform AnalysisPanel;

	private UIGrid dtl_blueGrid;

	private UIGrid dtl_redGrid;

	private UIGrid dtl_greenGrid;

	private UILabel dtl_date;

	private UILabel dtl_startTime;

	private UILabel dtl_timeLength;

	private UILabel dtl_type;

	private UILabel dtl_killCount;

	private UILabel dtl_money;

	public UISprite dtl_vectory;

	private UISprite bg0;

	private UIGrid mainBattleRecordGrid;

	private UIGrid historyAllBattleRecordGrid;

	private UIGrid historyBlueGrid;

	private UIGrid historyRedGrid;

	private UIGrid historyGreenGrid;

	private Transform allBtn;

	private Transform pentaBtn;

	private Transform quataryBtn;

	private Transform tripleBtn;

	private Transform legendaryBtn;

	private UIGrid picGrid;

	private BattleRecord _battleRecordItem;

	private PlayerBattleItem _playerBattleItem;

	private PlayerBattleDetailsItem _playerBattleDetailsItem;

	private MyHeroItem _myHeroItem;

	private ScreenShotItem _myHonorItem;

	private KDAData recordData;

	private SysHeroMainVo herovo;

	private BattleMaxInfo maxInfo = new BattleMaxInfo();

	private List<KdaUserHonorData> honorDataList = new List<KdaUserHonorData>();

	private bool haveClickMain;

	private bool haveClickHistory;

	private int battleCount;

	private double killCount;

	private double deathCount;

	private double assistantCount;

	private double winPercent;

	private int winCount;

	public int vBtn = 1;

	public int vBtnInMain = 1;

	private int btnV;

	public int AllCount;

	public int AllCountInMain;

	public string battleID = string.Empty;

	public int PageNum = 1;

	public int PageNumInMain = 1;

	private Dictionary<int, List<heroRecordInfo>> myBattleDic = new Dictionary<int, List<heroRecordInfo>>();

	public bool isInMainPanel = true;

	public Dictionary<long, heroRecordInfo> heroRecordDic = new Dictionary<long, heroRecordInfo>();

	public Dictionary<long, heroRecordInfo> heroRecordListInMain = new Dictionary<long, heroRecordInfo>();

	public string chooseBattleName = string.Empty;

	private List<MemberBattleInfo> teaminfo = new List<MemberBattleInfo>();

	private List<MemberBattleInfo> team1 = new List<MemberBattleInfo>();

	private List<MemberBattleInfo> team2 = new List<MemberBattleInfo>();

	private List<MemberBattleInfo> team3 = new List<MemberBattleInfo>();

	private Transform myHeros;

	private UILabel hero_all;

	private UILabel hero_own;

	private UILabel skin_all;

	private UILabel skin_own;

	private UIGrid myHero_grid;

	private UIScrollView mHeroPanel_ScrollView;

	private Transform myHeros_noRecord;

	private UILabel topKeepWin;

	private UILabel topKill;

	private UILabel topMoney;

	private UILabel topKillMonster;

	private UILabel topKeepKill;

	private UILabel topAssist;

	private UILabel topDamage;

	private UILabel topBeDamage;

	private UILabel topKda;

	private UILabel totalMvp;

	private Double_Mesh plane;

	private Mesh_Circle line;

	private Transform shareBtn;

	private KdaAbilityGraph abilityGraph;

	private CoroutineManager coroutineManager = new CoroutineManager();

	private List<heroRecordInfo> pvpBattleInfo = new List<heroRecordInfo>();

	private GameObject temp;

	private int count = 1;

	public GameObject oobj;

	private HistoryBattleData currbattleRecord;

	private string lastName;

	public AchievementView()
	{
		this.WinResCfg = new WinResurceCfg(true, false, "Prefab/UI/Achievement/AchievementView");
		this.WindowTitle = LanguageManager.Instance.GetStringById("Combatgains_Describe_Theme", "我的战绩");
	}

	public override void Init()
	{
		base.Init();
		this.leftAnchor = this.transform.Find("LeftAnchor");
		this.centerAnchor = this.transform.Find("CenterAnchor");
		this.rightAnchor = this.transform.Find("RightAnchor");
		this.mainBtn = this.leftAnchor.Find("Panel/Grid/MainAchieve");
		this.historyBtn = this.leftAnchor.Find("Panel/Grid/HistoryAchieve");
		this.honorBtn = this.leftAnchor.Find("Panel/Grid/Honor");
		this.momentBtn = this.leftAnchor.Find("Panel/Grid/Moment");
		this.heroesBtn = this.leftAnchor.Find("Panel/Grid/MyHeroes");
		this.abilityBtn = this.leftAnchor.Find("Panel/Grid/BattleAbility");
		this.mainPic = this.mainBtn.Find("Pic").GetComponent<UISprite>();
		this.historyPic = this.historyBtn.Find("Pic").GetComponent<UISprite>();
		this.honorPic = this.honorBtn.Find("Pic").GetComponent<UISprite>();
		this.momentPic = this.momentBtn.Find("Pic").GetComponent<UISprite>();
		this.heroPic = this.heroesBtn.Find("Pic").GetComponent<UISprite>();
		this.abilityPic = this.abilityBtn.Find("Pic").GetComponent<UISprite>();
		this.mainTex = this.mainBtn.Find("Label").GetComponent<UILabel>();
		this.historyTex = this.historyBtn.Find("Label").GetComponent<UILabel>();
		this.honorTex = this.honorBtn.Find("Label").GetComponent<UILabel>();
		this.momentTex = this.momentBtn.Find("Label").GetComponent<UILabel>();
		this.heroTex = this.heroesBtn.Find("Label").GetComponent<UILabel>();
		this.abilityTex = this.abilityBtn.Find("Label").GetComponent<UILabel>();
		this.mainPanel = this.centerAnchor.Find("MainPanel").GetComponent<UIPanel>();
		this.historyPanel = this.centerAnchor.Find("HistoryPanel").GetComponent<UIPanel>();
		this.honorPanel = this.centerAnchor.Find("HonorPanel").GetComponent<UIPanel>();
		this.momentPanel = this.centerAnchor.Find("MomentPanel").GetComponent<UIPanel>();
		this.heroesPanel = this.centerAnchor.Find("MyHeroesPanel").GetComponent<UIPanel>();
		this.abilityPanel = this.centerAnchor.Find("AbilityPanel").GetComponent<UIPanel>();
		this.myPortrait = this.centerAnchor.Find("MainPanel/SummonerInfo/Texture").GetComponent<UITexture>();
		this.myName = this.centerAnchor.Find("MainPanel/SummonerInfo/Name").GetComponent<UILabel>();
		this.like = this.centerAnchor.Find("MainPanel/SummonerInfo/Like/LikeCount").GetComponent<UILabel>();
		this.myRank = this.centerAnchor.Find("MainPanel/SummonerInfo/Frame/rank").GetComponent<UILabel>();
		this.achievCount = this.centerAnchor.Find("MainPanel/SummonerInfo/BattelRecord/Count").GetComponent<UILabel>();
		this.KDA = this.centerAnchor.Find("MainPanel/30DayKDA/KDA").GetComponent<UILabel>();
		this.K_D_A = this.centerAnchor.Find("MainPanel/30DayKDA/kda").GetComponent<UILabel>();
		this.JDDZCount = this.centerAnchor.Find("MainPanel/BattleType/BarChart/JDDZ/Bg/Count").GetComponent<UILabel>();
		this.JDDZPercent = this.centerAnchor.Find("MainPanel/BattleType/BarChart/JDDZ/Bg/Percent").GetComponent<UILabel>();
		this.JDDZBar = this.centerAnchor.Find("MainPanel/BattleType/BarChart/JDDZ/Bg/Bar").GetComponent<UISprite>();
		this.LZXFCount = this.centerAnchor.Find("MainPanel/BattleType/BarChart/LZXF/Bg/Count").GetComponent<UILabel>();
		this.LZXFPercent = this.centerAnchor.Find("MainPanel/BattleType/BarChart/LZXF/Bg/Percent").GetComponent<UILabel>();
		this.LZXFBar = this.centerAnchor.Find("MainPanel/BattleType/BarChart/LZXF/Bg/Bar").GetComponent<UISprite>();
		this.SDDLDCount = this.centerAnchor.Find("MainPanel/BattleType/BarChart/SDDLD/Bg/Count").GetComponent<UILabel>();
		this.SDDLDPercent = this.centerAnchor.Find("MainPanel/BattleType/BarChart/SDDLD/Bg/Percent").GetComponent<UILabel>();
		this.SDDLDBar = this.centerAnchor.Find("MainPanel/BattleType/BarChart/SDDLD/Bg/Bar").GetComponent<UISprite>();
		this.JJCCount = this.centerAnchor.Find("MainPanel/BattleType/BarChart/JJC/Bg/Count").GetComponent<UILabel>();
		this.JJCPercent = this.centerAnchor.Find("MainPanel/BattleType/BarChart/JJC/Bg/Percent").GetComponent<UILabel>();
		this.JJCBar = this.centerAnchor.Find("MainPanel/BattleType/BarChart/JJC/Bg/Bar").GetComponent<UISprite>();
		this.badge = this.centerAnchor.Find("MainPanel/MyScore/Badge").GetComponent<UISprite>();
		this.scoreNum = this.centerAnchor.Find("MainPanel/MyScore/ScoreNum").GetComponent<UILabel>();
		this.tripleKill = this.centerAnchor.Find("MainPanel/MyHonor/Grid/TripleKill/Count").GetComponent<UILabel>();
		this.quataryKill = this.centerAnchor.Find("MainPanel/MyHonor/Grid/QuataryKill/Count").GetComponent<UILabel>();
		this.pentaKill = this.centerAnchor.Find("MainPanel/MyHonor/Grid/PentaKill/Count").GetComponent<UILabel>();
		this.legendary = this.centerAnchor.Find("MainPanel/MyHonor/Grid/Legendary/Count").GetComponent<UILabel>();
		this.hero1 = this.centerAnchor.Find("MainPanel/Relaty/Grid/Hero1/Texture").GetComponent<UITexture>();
		this.hero2 = this.centerAnchor.Find("MainPanel/Relaty/Grid/Hero2/Texture").GetComponent<UITexture>();
		this.hero3 = this.centerAnchor.Find("MainPanel/Relaty/Grid/Hero3/Texture").GetComponent<UITexture>();
		this.heroGrid = this.centerAnchor.Find("MainPanel/Relaty/Grid");
		this.noHero = this.centerAnchor.Find("MainPanel/Relaty/Label");
		this.vBtnL = this.centerAnchor.Find("MainPanel/RightMain/Bottom/VL");
		this.vBtnR = this.centerAnchor.Find("MainPanel/RightMain/Bottom/VR");
		this.btn5 = this.centerAnchor.Find("MainPanel/RightMain/Bottom/5");
		this.btn10 = this.centerAnchor.Find("MainPanel/RightMain/Bottom/10");
		this.btn15 = this.centerAnchor.Find("MainPanel/RightMain/Bottom/15");
		this.btn20 = this.centerAnchor.Find("MainPanel/RightMain/Bottom/20");
		this.bg1 = this.centerAnchor.Find("MainPanel/RightMain/Sprite");
		this.nextPage = this.centerAnchor.Find("MainPanel/RightMain/Bottom/Page");
		this.VBtnInMain = this.centerAnchor.Find("MainPanel/RightMain/Top/VBtn");
		this.VBtnInHistory = this.centerAnchor.Find("HistoryPanel/Top/VBtn");
		this.VBtnPanel = this.centerAnchor.Find("HistoryPanel/Top/VBtn/Panel");
		this.xiba = this.centerAnchor.Find("HistoryPanel/Bottom");
		this.noRecord = this.centerAnchor.Find("HistoryPanel/Label");
		this.bg = this.centerAnchor.Find("HistoryPanel/RightHistory/Bg").GetComponent<UISprite>();
		this.page = this.centerAnchor.Find("HistoryPanel/Bottom/Page").GetComponent<UILabel>();
		this.pageNum = this.centerAnchor.Find("HistoryPanel/Bottom/PageNum").GetComponent<UILabel>();
		this.btnVR = this.centerAnchor.Find("HistoryPanel/Bottom/VR");
		this.btnVL = this.centerAnchor.Find("HistoryPanel/Bottom/VL");
		this.title = this.centerAnchor.Find("HistoryPanel/RightHistory/Top/Title").GetComponent<UILabel>();
		this.time = this.centerAnchor.Find("HistoryPanel/RightHistory/Top/Title/Time").GetComponent<UILabel>();
		this.mainBattleRecordGrid = this.centerAnchor.Find("MainPanel/RightMain/Grid").GetComponent<UIGrid>();
		this.historyAllBattleRecordGrid = this.centerAnchor.Find("HistoryPanel/Grid").GetComponent<UIGrid>();
		this.historyBlueGrid = this.centerAnchor.Find("HistoryPanel/RightHistory/BlueGrid").GetComponent<UIGrid>();
		this.historyRedGrid = this.centerAnchor.Find("HistoryPanel/RightHistory/RedGrid").GetComponent<UIGrid>();
		this.historyGreenGrid = this.centerAnchor.Find("HistoryPanel/RightHistory/GreenGrid").GetComponent<UIGrid>();
		this._battleRecordItem = Resources.Load<BattleRecord>("Prefab/UI/Achievement/BattleRecord");
		this._playerBattleItem = Resources.Load<PlayerBattleItem>("Prefab/UI/Achievement/PlayerBattleItem");
		this._playerBattleDetailsItem = Resources.Load<PlayerBattleDetailsItem>("Prefab/UI/Achievement/PlayerBattleDetailsItem");
		this._myHeroItem = Resources.Load<MyHeroItem>("Prefab/UI/Achievement/MyHeroItem");
		this._myHonorItem = Resources.Load<ScreenShotItem>("Prefab/UI/Achievement/ScreenShotItem");
		this.detailBtn = this.centerAnchor.Find("HistoryPanel/RightHistory/Top/Sprite");
		this.closeDetailBtn = this.centerAnchor.Find("HistoryPanel/AnalysisPanel/Bg");
		this.AnalysisPanel = this.centerAnchor.Find("HistoryPanel/AnalysisPanel");
		this.dtl_blueGrid = this.AnalysisPanel.Find("BlueGrid").GetComponent<UIGrid>();
		this.dtl_redGrid = this.AnalysisPanel.Find("RedGrid").GetComponent<UIGrid>();
		this.dtl_greenGrid = this.AnalysisPanel.Find("GreenGrid").GetComponent<UIGrid>();
		this.dtl_date = this.AnalysisPanel.Find("Detalis/Date").GetComponent<UILabel>();
		this.dtl_startTime = this.AnalysisPanel.Find("Detalis/StartTime").GetComponent<UILabel>();
		this.dtl_timeLength = this.AnalysisPanel.Find("Detalis/Time").GetComponent<UILabel>();
		this.dtl_type = this.AnalysisPanel.Find("Detalis/Type").GetComponent<UILabel>();
		this.dtl_killCount = this.AnalysisPanel.Find("Detalis/KillCount").GetComponent<UILabel>();
		this.dtl_money = this.AnalysisPanel.Find("Detalis/Money").GetComponent<UILabel>();
		this.dtl_vectory = this.AnalysisPanel.Find("Detalis/Title").GetComponent<UISprite>();
		this.bg0 = this.AnalysisPanel.Find("Bg0").GetComponent<UISprite>();
		this.allBtn = this.centerAnchor.Find("HonorPanel/Grid/All");
		this.pentaBtn = this.centerAnchor.Find("HonorPanel/Grid/Penta");
		this.quataryBtn = this.centerAnchor.Find("HonorPanel/Grid/Quatary");
		this.tripleBtn = this.centerAnchor.Find("HonorPanel/Grid/Triple");
		this.legendaryBtn = this.centerAnchor.Find("HonorPanel/Grid/Legendary");
		this.picGrid = this.centerAnchor.Find("HonorPanel/RightHonor/Scroll View/Grid").GetComponent<UIGrid>();
		this.myHeros = this.centerAnchor.Find("MyHeroesPanel");
		this.hero_all = this.myHeros.Find("Title/Hero/All").GetComponent<UILabel>();
		this.hero_own = this.myHeros.Find("Title/Hero/Own").GetComponent<UILabel>();
		this.skin_all = this.myHeros.Find("Title/Skin/All").GetComponent<UILabel>();
		this.skin_own = this.myHeros.Find("Title/Skin/Own").GetComponent<UILabel>();
		this.myHero_grid = this.myHeros.Find("Scroll View/Grid").GetComponent<UIGrid>();
		this.mHeroPanel_ScrollView = this.myHeros.Find("Scroll View").GetComponent<UIScrollView>();
		this.myHeros_noRecord = this.myHeros.Find("NoRecord");
		this.topAssist = this.centerAnchor.Find("AbilityPanel/RightAbility/Help").GetComponent<UILabel>();
		this.topBeDamage = this.centerAnchor.Find("AbilityPanel/RightAbility/DamageIn").GetComponent<UILabel>();
		this.topDamage = this.centerAnchor.Find("AbilityPanel/RightAbility/DamageOut").GetComponent<UILabel>();
		this.topKda = this.centerAnchor.Find("AbilityPanel/RightAbility/KDA").GetComponent<UILabel>();
		this.topKeepKill = this.centerAnchor.Find("AbilityPanel/RightAbility/KillCombo").GetComponent<UILabel>();
		this.topKill = this.centerAnchor.Find("AbilityPanel/RightAbility/Kill").GetComponent<UILabel>();
		this.topKeepWin = this.centerAnchor.Find("AbilityPanel/RightAbility/Win").GetComponent<UILabel>();
		this.topKillMonster = this.centerAnchor.Find("AbilityPanel/RightAbility/Hit").GetComponent<UILabel>();
		this.topMoney = this.centerAnchor.Find("AbilityPanel/RightAbility/Economy").GetComponent<UILabel>();
		this.totalMvp = this.centerAnchor.Find("AbilityPanel/RightAbility/MVPCount").GetComponent<UILabel>();
		this.plane = this.centerAnchor.Find("AbilityPanel/Poly/Plane").GetComponent<Double_Mesh>();
		this.line = this.centerAnchor.Find("AbilityPanel/Poly/Line").GetComponent<Mesh_Circle>();
		this.shareBtn = this.leftAnchor.Find("Panel/Share");
		UIEventListener expr_CA6 = UIEventListener.Get(this.mainBtn.gameObject);
		expr_CA6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_CA6.onClick, new UIEventListener.VoidDelegate(this.OnClicMainBtn));
		UIEventListener expr_CD7 = UIEventListener.Get(this.historyBtn.gameObject);
		expr_CD7.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_CD7.onClick, new UIEventListener.VoidDelegate(this.OnClicHistoryBtn));
		UIEventListener expr_D08 = UIEventListener.Get(this.honorBtn.gameObject);
		expr_D08.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_D08.onClick, new UIEventListener.VoidDelegate(this.OnClicHonorBtn));
		UIEventListener expr_D39 = UIEventListener.Get(this.momentBtn.gameObject);
		expr_D39.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_D39.onClick, new UIEventListener.VoidDelegate(this.OnClicMomentBtn));
		UIEventListener expr_D6A = UIEventListener.Get(this.heroesBtn.gameObject);
		expr_D6A.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_D6A.onClick, new UIEventListener.VoidDelegate(this.OnClicHeroBtn));
		UIEventListener expr_D9B = UIEventListener.Get(this.abilityBtn.gameObject);
		expr_D9B.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_D9B.onClick, new UIEventListener.VoidDelegate(this.OnClicAbilityBtn));
		UIEventListener expr_DCC = UIEventListener.Get(this.detailBtn.gameObject);
		expr_DCC.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_DCC.onClick, new UIEventListener.VoidDelegate(this.OnClickDetailBtn));
		UIEventListener expr_DFD = UIEventListener.Get(this.closeDetailBtn.gameObject);
		expr_DFD.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_DFD.onClick, new UIEventListener.VoidDelegate(this.OnClickDetailBtn));
		UIEventListener expr_E2E = UIEventListener.Get(this.vBtnL.gameObject);
		expr_E2E.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_E2E.onClick, new UIEventListener.VoidDelegate(this.OnClickChangePageBtnInMain));
		UIEventListener expr_E5F = UIEventListener.Get(this.vBtnR.gameObject);
		expr_E5F.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_E5F.onClick, new UIEventListener.VoidDelegate(this.OnClickChangePageBtnInMain));
		UIEventListener expr_E90 = UIEventListener.Get(this.btn5.gameObject);
		expr_E90.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_E90.onClick, new UIEventListener.VoidDelegate(this.OnClickNumBtn));
		UIEventListener expr_EC1 = UIEventListener.Get(this.btn10.gameObject);
		expr_EC1.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_EC1.onClick, new UIEventListener.VoidDelegate(this.OnClickNumBtn));
		UIEventListener expr_EF2 = UIEventListener.Get(this.btn15.gameObject);
		expr_EF2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_EF2.onClick, new UIEventListener.VoidDelegate(this.OnClickNumBtn));
		UIEventListener expr_F23 = UIEventListener.Get(this.btn20.gameObject);
		expr_F23.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_F23.onClick, new UIEventListener.VoidDelegate(this.OnClickNumBtn));
		UIEventListener expr_F54 = UIEventListener.Get(this.btnVR.gameObject);
		expr_F54.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_F54.onClick, new UIEventListener.VoidDelegate(this.OnClickChangePageBtn));
		UIEventListener expr_F85 = UIEventListener.Get(this.btnVL.gameObject);
		expr_F85.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_F85.onClick, new UIEventListener.VoidDelegate(this.OnClickChangePageBtn));
		UIEventListener expr_FB6 = UIEventListener.Get(this.allBtn.gameObject);
		expr_FB6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_FB6.onClick, new UIEventListener.VoidDelegate(this.OnClickAllBtn));
		UIEventListener expr_FE7 = UIEventListener.Get(this.pentaBtn.gameObject);
		expr_FE7.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_FE7.onClick, new UIEventListener.VoidDelegate(this.OnClickAllBtn));
		UIEventListener expr_1018 = UIEventListener.Get(this.quataryBtn.gameObject);
		expr_1018.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_1018.onClick, new UIEventListener.VoidDelegate(this.OnClickAllBtn));
		UIEventListener expr_1049 = UIEventListener.Get(this.tripleBtn.gameObject);
		expr_1049.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_1049.onClick, new UIEventListener.VoidDelegate(this.OnClickAllBtn));
		UIEventListener expr_107A = UIEventListener.Get(this.legendaryBtn.gameObject);
		expr_107A.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_107A.onClick, new UIEventListener.VoidDelegate(this.OnClickAllBtn));
		UIEventListener.Get(this.shareBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickShareBtn);
	}

	private void OnClickAllBtn(GameObject go)
	{
		string name = go.name;
		switch (name)
		{
		case "All":
			GridHelper.FillGrid<ScreenShotItem>(this.picGrid, this._myHonorItem, this.honorDataList.Count, delegate(int idx, ScreenShotItem comp)
			{
				comp.Init(this.honorDataList[idx]);
			});
			break;
		case "Penta":
		{
			List<KdaUserHonorData> pentalist = new List<KdaUserHonorData>();
			for (int i = 0; i < this.honorDataList.Count; i++)
			{
				if (this.honorDataList[i].killtype == KillType.PentaKill)
				{
					pentalist.Add(this.honorDataList[i]);
				}
			}
			GridHelper.FillGrid<ScreenShotItem>(this.picGrid, this._myHonorItem, pentalist.Count, delegate(int idx, ScreenShotItem comp)
			{
				comp.Init(pentalist[idx]);
			});
			break;
		}
		case "Quatary":
		{
			List<KdaUserHonorData> quatarylist = new List<KdaUserHonorData>();
			for (int j = 0; j < this.honorDataList.Count; j++)
			{
				if (this.honorDataList[j].killtype == KillType.QuadraKill)
				{
					quatarylist.Add(this.honorDataList[j]);
				}
			}
			GridHelper.FillGrid<ScreenShotItem>(this.picGrid, this._myHonorItem, quatarylist.Count, delegate(int idx, ScreenShotItem comp)
			{
				comp.Init(quatarylist[idx]);
			});
			break;
		}
		case "Triple":
		{
			List<KdaUserHonorData> triplelist = new List<KdaUserHonorData>();
			for (int k = 0; k < this.honorDataList.Count; k++)
			{
				if (this.honorDataList[k].killtype == KillType.TripleKill)
				{
					triplelist.Add(this.honorDataList[k]);
				}
			}
			GridHelper.FillGrid<ScreenShotItem>(this.picGrid, this._myHonorItem, triplelist.Count, delegate(int idx, ScreenShotItem comp)
			{
				comp.Init(triplelist[idx]);
			});
			break;
		}
		case "Legendary":
		{
			List<KdaUserHonorData> legendarylist = new List<KdaUserHonorData>();
			for (int l = 0; l < this.honorDataList.Count; l++)
			{
				if (this.honorDataList[l].killtype == KillType.GodLike)
				{
					legendarylist.Add(this.honorDataList[l]);
				}
			}
			GridHelper.FillGrid<ScreenShotItem>(this.picGrid, this._myHonorItem, legendarylist.Count, delegate(int idx, ScreenShotItem comp)
			{
				comp.Init(legendarylist[idx]);
			});
			break;
		}
		}
	}

	private void OnClickChangePageBtnInMain(GameObject obj = null)
	{
		BattleRecord.TempObj = null;
		this.oobj = null;
		if (obj == this.vBtnR.gameObject)
		{
			if ((float)(this.vBtnInMain + 1) > Mathf.Ceil((float)this.AllCountInMain / 5f) || this.vBtnInMain + 1 > 4)
			{
				return;
			}
			this.vBtnInMain++;
		}
		else
		{
			if (this.vBtnInMain - 1 < 1)
			{
				return;
			}
			this.vBtnInMain--;
		}
		if (this.vBtnInMain == 1)
		{
			this.OnClickNumBtn(this.btn5.gameObject);
		}
		else if (this.vBtnInMain == 2)
		{
			this.OnClickNumBtn(this.btn10.gameObject);
		}
		else if (this.vBtnInMain == 3)
		{
			this.OnClickNumBtn(this.btn15.gameObject);
		}
		else if (this.vBtnInMain == 4)
		{
			this.OnClickNumBtn(this.btn20.gameObject);
		}
	}

	private void OnClickChangePageBtn(GameObject obj = null)
	{
		BattleRecord.TempObj = null;
		this.oobj = null;
		if (obj == this.btnVR.gameObject)
		{
			if ((float)(this.vBtn + 1) > Mathf.Ceil((float)this.AllCount / 5f))
			{
				return;
			}
			this.vBtn++;
		}
		else
		{
			if (this.vBtn - 1 < 1)
			{
				return;
			}
			this.vBtn--;
		}
		this.ApplyHeroRecordInfo(this.battleID, this.vBtn);
	}

	private void OnClickNumBtn(GameObject obj)
	{
		if (this.temp == obj)
		{
			return;
		}
		if (this.temp != null)
		{
			this.temp.transform.GetComponent<UILabel>().color = new Color32(0, 115, 156, 255);
		}
		obj.transform.GetComponent<UILabel>().color = new Color32(0, 243, 252, 255);
		string name = obj.name;
		switch (name)
		{
		case "5":
			this.vBtnInMain = 1;
			break;
		case "10":
			this.vBtnInMain = 2;
			break;
		case "15":
			this.vBtnInMain = 3;
			break;
		case "20":
			this.vBtnInMain = 4;
			break;
		}
		this.temp = obj;
		this.ApplyHeroRecordInfo(this.battleID, this.vBtnInMain);
	}

	private void OnClicMainBtn(GameObject obj = null)
	{
		this.isInMainPanel = true;
		BattleTypeList component = this.VBtnInMain.GetComponent<BattleTypeList>();
		if (!this.haveClickMain)
		{
			this.vBtnInMain = 1;
			component.chooseStr = LanguageManager.Instance.GetStringById("Combatgains_Describe_Twenty");
			this.OnClickNumBtn(this.btn5.gameObject);
		}
		this.OpenMainPanel();
	}

	public void OnClicHistoryBtn(GameObject obj = null)
	{
		this.isInMainPanel = false;
		BattleTypeList component = Singleton<AchievementView>.Instance.VBtnInHistory.GetComponent<BattleTypeList>();
		if (!this.haveClickHistory)
		{
			this.vBtn = 1;
			component.chooseStr = LanguageManager.Instance.GetStringById("Combatgains_Describe_All");
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在获得数据", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetHeroRecordInfo, param, new object[]
			{
				string.Empty,
				1
			});
			this.OpenHistoryPanel(true);
		}
		else
		{
			this.OpenHistoryPanelnew();
		}
	}

	private void OnClicHonorBtn(GameObject obj = null)
	{
		Singleton<TipView>.Instance.ShowViewSetText("暂未开放", 1f);
	}

	private void OnClicMomentBtn(GameObject obj = null)
	{
		Singleton<TipView>.Instance.ShowViewSetText("暂未开放", 1f);
	}

	private void OnClicHeroBtn(GameObject obj = null)
	{
		this.OpenMyHeroesPanel();
	}

	private void OnClicAbilityBtn(GameObject obj = null)
	{
		this.OpenAbilityPanel();
	}

	private void OnClickShareBtn(GameObject go)
	{
		GameManager.Instance.DoShareSDK(3, new Rect((float)Screen.width * 0.215f, (float)Screen.height * 0.02f, 1488f * (float)Screen.width / 1920f, 920f * (float)Screen.width / 1920f), null);
	}

	public override void RegisterUpdateHandler()
	{
		this.mainBtn.transform.GetComponent<UIToggle>().value = true;
		Singleton<MenuTopBarView>.Instance.DoOpenOtherViewAction(0.25f);
		MVC_MessageManager.AddListener_view(MobaGameCode.GetTotalRecord, new MobaMessageFunc(this.OnGetMsg_AchieveInfo));
		MVC_MessageManager.AddListener_view(MobaGameCode.GetHeroRecordInfo, new MobaMessageFunc(this.OnGetMsg_PrePageBattleInfo));
		MVC_MessageManager.AddListener_view(MobaGameCode.GetHistoryRecord, new MobaMessageFunc(this.OnGetMsg_HistoryInfo));
		MVC_MessageManager.AddListener_view(MobaGameCode.GetKdaMyHeroData, new MobaMessageFunc(this.OnGetMsg_MyHeroInfo));
		MVC_MessageManager.AddListener_view(MobaGameCode.GetMyFightAbility, new MobaMessageFunc(this.OnGetMsg_MyAbilityInfo));
		MVC_MessageManager.AddListener_view(MobaGameCode.GetUserHonorPic, new MobaMessageFunc(this.OnGetMsg_MyHonorInfo));
		SendMsgManager.Instance.SendMsg(MobaGameCode.GetTotalRecord, null, new object[0]);
		this.RefreshUI();
		SendMsgManager.Instance.SendMsg(MobaGameCode.GetMyFightAbility, null, new object[0]);
	}

	public override void CancelUpdateHandler()
	{
		MVC_MessageManager.RemoveListener_view(MobaGameCode.GetTotalRecord, new MobaMessageFunc(this.OnGetMsg_AchieveInfo));
		MVC_MessageManager.RemoveListener_view(MobaGameCode.GetHeroRecordInfo, new MobaMessageFunc(this.OnGetMsg_PrePageBattleInfo));
		MVC_MessageManager.RemoveListener_view(MobaGameCode.GetHistoryRecord, new MobaMessageFunc(this.OnGetMsg_HistoryInfo));
		MVC_MessageManager.RemoveListener_view(MobaGameCode.GetKdaMyHeroData, new MobaMessageFunc(this.OnGetMsg_MyHeroInfo));
		MVC_MessageManager.RemoveListener_view(MobaGameCode.GetMyFightAbility, new MobaMessageFunc(this.OnGetMsg_MyAbilityInfo));
		MVC_MessageManager.RemoveListener_view(MobaGameCode.GetUserHonorPic, new MobaMessageFunc(this.OnGetMsg_MyHonorInfo));
		this.PageNum = 1;
		this.PageNumInMain = 1;
		this.AnalysisPanel.gameObject.SetActive(false);
		this.historyBlueGrid.GetComponent<UIWidget>().alpha = 0.01f;
		this.historyRedGrid.GetComponent<UIWidget>().alpha = 0.01f;
		this.historyGreenGrid.GetComponent<UIWidget>().alpha = 0.01f;
		if (null != Singleton<MenuTopBarView>.Instance.gameObject)
		{
			Singleton<MenuTopBarView>.Instance.SetPanelVisible(true);
		}
		this.haveClickHistory = false;
		this.haveClickMain = false;
		BattleTypeList component = this.VBtnInMain.GetComponent<BattleTypeList>();
		BattleTypeList component2 = this.VBtnInHistory.GetComponent<BattleTypeList>();
		component.RefreshLabelToggle();
		component2.RefreshLabelToggle();
		for (int i = 0; i < 6; i++)
		{
			this.line.value_in[i] = 0f;
			this.plane.value[i] = 0f;
		}
		this.coroutineManager.StopAllCoroutine();
	}

	public void ApplyHeroRecordInfo(string battleid, int pagenumber)
	{
		if (this.isInMainPanel)
		{
			this.chooseBattleName = battleid;
			this.PageNumInMain = pagenumber;
		}
		this.battleID = battleid;
		this.PageNum = pagenumber;
		SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在获得数据", true, 15f);
		SendMsgManager.Instance.SendMsg(MobaGameCode.GetHeroRecordInfo, param, new object[]
		{
			battleid,
			pagenumber
		});
	}

	public override void RefreshUI()
	{
		this.OnClicMainBtn(null);
		this.btn5.transform.GetComponent<UILabel>().color = new Color32(0, 243, 252, 255);
		this.temp = this.btn5.gameObject;
	}

	private void OnGetMsg_AchieveInfo(MobaMessage msg)
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
		if (mobaErrorCode == MobaErrorCode.Ok || mobaErrorCode == MobaErrorCode.ItemExist)
		{
			this.recordData = ModelManager.Instance.Get_GetMyAchievementData_X().kdaData;
			this.battleCount = this.recordData.wincount + this.recordData.losecount;
			this.UpdateMainAchieve();
		}
	}

	private void OnGetMsg_PrePageBattleInfo(MobaMessage msg)
	{
		this.haveClickMain = true;
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
			int num2 = Convert.ToInt32(operationResponse.Parameters[101]);
			int num3 = Convert.ToInt32(operationResponse.Parameters[10]);
			this.heroRecordDic = ModelManager.Instance.Get_GetMyAchievementData_X().myHeroRecord;
			this.AllCount = num2;
			this.PageNum = num3;
			if (this.isInMainPanel)
			{
				this.heroRecordListInMain = this.heroRecordDic;
				this.AllCountInMain = num2;
				this.PageNumInMain = num3;
			}
			if (this.heroRecordDic == null)
			{
				return;
			}
			this.CreatBattleRecordNew(this.isInMainPanel, num2, num3, this.heroRecordDic);
			if (!this.isInMainPanel)
			{
				if (this.heroRecordDic.Count > 0)
				{
					this.SendMsgToGetHistoryRec(this.heroRecordDic.ElementAt(0).Key);
				}
				else
				{
					this.ShowOrHideRecordLst(false, true);
				}
			}
		}
	}

	private void OnGetMsg_HistoryInfo(MobaMessage msg)
	{
		this.haveClickHistory = true;
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
			this.NewNewUpdateHistoryRecord(ModelManager.Instance.Get_GetMyAchievementData_X().historyBattle);
		}
	}

	private void OnGetMsg_MyHeroInfo(MobaMessage msg)
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
		if (mobaErrorCode == MobaErrorCode.Ok || mobaErrorCode == MobaErrorCode.ItemExist)
		{
			this.OpenMyHeroesPanel();
		}
	}

	private void OnGetMsg_MyAbilityInfo(MobaMessage msg)
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
		if (mobaErrorCode == MobaErrorCode.Ok || mobaErrorCode == MobaErrorCode.ItemExist)
		{
			this.abilityGraph = ModelManager.Instance.Get_GetMyAchievementData_X().abilityGraph;
		}
	}

	private void OnGetMsg_MyHonorInfo(MobaMessage msg)
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
			this.honorDataList = ModelManager.Instance.Get_GetMyAchievementData_X().myHonorData;
			this.OpenHonorPanel();
		}
	}

	private void OnGetMsg_MyHonorInfo1(MobaMessage msg)
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
		if (mobaErrorCode != MobaErrorCode.Ok)
		{
		}
	}

	private void OpenMainPanel()
	{
		this.mainPanel.gameObject.SetActive(true);
		this.historyPanel.gameObject.SetActive(false);
		this.honorPanel.gameObject.SetActive(false);
		this.momentPanel.gameObject.SetActive(false);
		this.heroesPanel.gameObject.SetActive(false);
		this.abilityPanel.gameObject.SetActive(false);
	}

	public void OpenHistoryPanel(bool isClickBtn = true)
	{
		this.mainPanel.gameObject.SetActive(false);
		this.historyPanel.gameObject.SetActive(true);
		this.honorPanel.gameObject.SetActive(false);
		this.momentPanel.gameObject.SetActive(false);
		this.heroesPanel.gameObject.SetActive(false);
		this.abilityPanel.gameObject.SetActive(false);
		if (null != this.VBtnInHistory.Find("Panel/All"))
		{
			this.VBtnInHistory.GetComponent<BattleTypeList>().OnClickLabel(this.VBtnInHistory.Find("Panel/All").gameObject);
		}
		if (isClickBtn)
		{
			this.SelectBattle(false, "All");
			this.pageNum.text = "1";
		}
	}

	private void OpenHistoryPanelnew()
	{
		this.mainPanel.gameObject.SetActive(false);
		this.historyPanel.gameObject.SetActive(true);
		this.honorPanel.gameObject.SetActive(false);
		this.momentPanel.gameObject.SetActive(false);
		this.heroesPanel.gameObject.SetActive(false);
		this.abilityPanel.gameObject.SetActive(false);
	}

	private void OpenHonorPanel()
	{
		this.mainPanel.gameObject.SetActive(false);
		this.historyPanel.gameObject.SetActive(false);
		this.honorPanel.gameObject.SetActive(true);
		this.momentPanel.gameObject.SetActive(false);
		this.heroesPanel.gameObject.SetActive(false);
		this.abilityPanel.gameObject.SetActive(false);
		this.allBtn.GetComponent<UIToggle>().value = true;
	}

	private void OpenMomentPanel()
	{
		this.mainPanel.gameObject.SetActive(false);
		this.historyPanel.gameObject.SetActive(false);
		this.honorPanel.gameObject.SetActive(false);
		this.momentPanel.gameObject.SetActive(true);
		this.heroesPanel.gameObject.SetActive(false);
		this.abilityPanel.gameObject.SetActive(false);
	}

	private void OpenMyHeroesPanel()
	{
		this.mainPanel.gameObject.SetActive(false);
		this.historyPanel.gameObject.SetActive(false);
		this.honorPanel.gameObject.SetActive(false);
		this.momentPanel.gameObject.SetActive(false);
		this.heroesPanel.gameObject.SetActive(true);
		this.abilityPanel.gameObject.SetActive(false);
		this.skin_own.text = ModelManager.Instance.GetSummSkinList().Count.ToString();
		Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysHeroSkinVo>();
		this.skin_all.text = "/" + dicByType.Keys.Count.ToString();
		this.hero_own.text = CharacterDataMgr.instance.OwenHeros.Count.ToString();
		this.hero_all.text = "/" + CharacterDataMgr.instance.AllHeros.Count.ToString();
		List<KdaMyHeroData> myHero = ModelManager.Instance.Get_GetMyAchievementData_X().myHero;
		if (myHero == null)
		{
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetKdaMyHeroData, null, new object[0]);
		}
		myHero.Sort((KdaMyHeroData x, KdaMyHeroData y) => (x.herouseddata.useinfo <= y.herouseddata.useinfo) ? 0 : -1);
		List<KdaMyHeroData> newData = new List<KdaMyHeroData>();
		for (int i = 0; i < myHero.Count; i++)
		{
			if (myHero[i].herouseddata.useinfo <= 0)
			{
				break;
			}
			newData.Add(myHero[i]);
		}
		GridHelper.FillGrid<MyHeroItem>(this.myHero_grid, this._myHeroItem, newData.Count, delegate(int idx, MyHeroItem comp)
		{
			comp.Init(newData[idx].skinlist, newData[idx].herouseddata);
		});
		if (this.myHero_grid.transform.childCount == 0)
		{
			this.myHeros_noRecord.gameObject.SetActive(true);
		}
		else
		{
			this.myHeros_noRecord.gameObject.SetActive(false);
		}
		this.myHero_grid.Reposition();
		this.coroutineManager.StartCoroutine(this.SetPanelState(), true);
	}

	[DebuggerHidden]
	private IEnumerator SetPanelState()
	{
		AchievementView.<SetPanelState>c__IteratorA9 <SetPanelState>c__IteratorA = new AchievementView.<SetPanelState>c__IteratorA9();
		<SetPanelState>c__IteratorA.<>f__this = this;
		return <SetPanelState>c__IteratorA;
	}

	private void OpenAbilityPanel()
	{
		this.mainPanel.gameObject.SetActive(false);
		this.historyPanel.gameObject.SetActive(false);
		this.honorPanel.gameObject.SetActive(false);
		this.momentPanel.gameObject.SetActive(false);
		this.heroesPanel.gameObject.SetActive(false);
		this.abilityPanel.gameObject.SetActive(true);
		KdaUserTopData myTopData = ModelManager.Instance.Get_GetMyAchievementData_X().myTopData;
		this.topAssist.text = myTopData.mostAssist.ToString();
		this.topBeDamage.text = myTopData.mostAllBeDamaged.ToString();
		this.topDamage.text = myTopData.mostAllDamage.ToString();
		this.topKda.text = myTopData.mostKda.ToString();
		this.topKeepKill.text = myTopData.mostKeepKill.ToString();
		this.topKill.text = myTopData.mostKill.ToString();
		this.topKeepWin.text = myTopData.mostWin.ToString();
		this.topKillMonster.text = myTopData.mostKillMonster.ToString();
		this.topMoney.text = ((float)myTopData.mostMoney / 1000f).ToString("F2");
		this.totalMvp.text = myTopData.totalMvp.ToString();
		this.UpdateRadarMesh();
	}

	private void UpdateRadarMesh()
	{
		if (this.abilityGraph == null)
		{
			return;
		}
		SysCompetencyDegreeVo sysCompetencyDegreeVo = new SysCompetencyDegreeVo();
		for (int i = 1; i <= 21; i++)
		{
			sysCompetencyDegreeVo = BaseDataMgr.instance.GetDataById<SysCompetencyDegreeVo>(i.ToString());
			if (sysCompetencyDegreeVo.kill_result > this.abilityGraph.kill || i == 21)
			{
				if (this.abilityGraph.kill == 0f)
				{
					this.line.value_in[0] = 0f;
					this.plane.value[0] = 0f;
				}
				else if (this.line.value_in[0] == 0f)
				{
					this.line.value_in[0] = (float)(i - 1) * 0.1335f;
					this.plane.value[0] = (float)(i - 1) * 0.1335f;
				}
			}
			if (sysCompetencyDegreeVo.money_result > this.abilityGraph.money || i == 21)
			{
				if (this.abilityGraph.money == 0f)
				{
					this.line.value_in[1] = 0f;
					this.plane.value[1] = 0f;
				}
				else if (this.line.value_in[1] == 0f)
				{
					this.line.value_in[1] = (float)(i - 1) * 0.1335f;
					this.plane.value[1] = (float)(i - 1) * 0.1335f;
				}
			}
			if (sysCompetencyDegreeVo.assist_result > this.abilityGraph.assist || i == 21)
			{
				if (this.abilityGraph.assist == 0f)
				{
					this.line.value_in[2] = 0f;
					this.plane.value[2] = 0f;
				}
				else if (this.line.value_in[2] == 0f)
				{
					this.line.value_in[2] = (float)(i - 1) * 0.1335f;
					this.plane.value[2] = (float)(i - 1) * 0.1335f;
				}
			}
			if (sysCompetencyDegreeVo.defense_result > this.abilityGraph.defence || i == 21)
			{
				if (this.abilityGraph.defence == 0f)
				{
					this.line.value_in[3] = 0f;
					this.plane.value[3] = 0f;
				}
				else if (this.line.value_in[3] == 0f)
				{
					this.line.value_in[3] = (float)(i - 1) * 0.1335f;
					this.plane.value[3] = (float)(i - 1) * 0.1335f;
				}
			}
			if (sysCompetencyDegreeVo.survival_result < this.abilityGraph.survival || i == 21)
			{
				if (this.abilityGraph.survival == 0f)
				{
					this.line.value_in[4] = 0f;
					this.plane.value[4] = 0f;
				}
				else if (this.line.value_in[4] == 0f)
				{
					this.line.value_in[4] = (float)(i - 1) * 0.1335f;
					this.plane.value[4] = (float)(i - 1) * 0.1335f;
				}
			}
			if (sysCompetencyDegreeVo.harm_result > this.abilityGraph.output || i == 21)
			{
				if (this.abilityGraph.output == 0f)
				{
					this.line.value_in[5] = 0f;
					this.plane.value[5] = 0f;
				}
				else if (this.line.value_in[5] == 0f)
				{
					this.line.value_in[5] = (float)(i - 1) * 0.1335f;
					this.plane.value[5] = (float)(i - 1) * 0.1335f;
				}
			}
		}
		this.plane.init();
		this.line.Init();
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private string CountWinPercent(int _win, int _all)
	{
		int num = Mathf.CeilToInt((float)_win / (float)((_all != 0) ? _all : 1) * 100f);
		return "胜" + num + "%";
	}

	private int CountWinPercentBar(int _win, int _all)
	{
		return Mathf.CeilToInt((float)_win / (float)((_all != 0) ? _all : 1) * 276f);
	}

	private void UpdateMainAchieve()
	{
		long num = ModelManager.Instance.Get_userData_filed_X("Exp");
		int num2 = ModelManager.Instance.Get_userData_filed_X("Avatar");
		string text = ModelManager.Instance.Get_userData_filed_X("NickName");
		int num3 = ModelManager.Instance.Get_userData_filed_X("CharmRankValue");
		this.myRank.text = CharacterDataMgr.instance.GetUserLevel((long)((int)num)).ToString();
		SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(num2.ToString());
		this.myPortrait.mainTexture = ResourceManager.Load<Texture>(dataById.headportrait_icon, true, true, null, 0, false);
		this.myName.text = text;
		if (num3 <= 50)
		{
			this.myName.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(num3);
		}
		this.like.text = this.recordData.saygood.ToString();
		if (this.battleCount <= 5)
		{
			this.btn10.GetComponent<BoxCollider>().enabled = false;
			this.btn15.GetComponent<BoxCollider>().enabled = false;
			this.btn20.GetComponent<BoxCollider>().enabled = false;
		}
		else if (this.battleCount > 5 && this.battleCount <= 10)
		{
			this.btn15.GetComponent<BoxCollider>().enabled = false;
			this.btn20.GetComponent<BoxCollider>().enabled = false;
		}
		else if (this.battleCount > 10 && this.battleCount <= 15)
		{
			this.btn20.GetComponent<BoxCollider>().enabled = false;
		}
		if (this.battleCount == 0)
		{
			this.achievCount.text = "暂无战绩";
			this.killCount = 0.0;
			this.deathCount = 0.0;
			this.assistantCount = 0.0;
			this.KDA.text = 0.ToString();
			this.K_D_A.text = string.Concat(new string[]
			{
				0.ToString(),
				"/",
				0.ToString(),
				"/",
				0.ToString()
			});
		}
		else
		{
			this.killCount = (double)this.recordData.killcount / (double)this.battleCount;
			this.killCount = Math.Round(this.killCount, 1);
			this.winPercent = (double)this.recordData.wincount / (double)this.battleCount * 100.0;
			this.winPercent = Math.Round(this.winPercent, 1);
			this.achievCount.text = string.Concat(new string[]
			{
				"[59FF2D]",
				this.winPercent.ToString("F1"),
				"%[-][D3F6FF]/",
				this.battleCount.ToString(),
				"场"
			});
			this.deathCount = (double)this.recordData.deathcount / (double)this.battleCount;
			this.deathCount = Math.Round(this.deathCount, 1);
			this.assistantCount = (double)this.recordData.assistantcount / (double)this.battleCount;
			this.assistantCount = Math.Round(this.assistantCount, 1);
			double value;
			if (this.deathCount == 0.0)
			{
				value = (this.killCount + this.assistantCount) / 1.0 * 3.0;
			}
			else
			{
				value = (this.killCount + this.assistantCount) / this.deathCount * 3.0;
			}
			value = Math.Round(value, 1);
			this.KDA.text = value.ToString();
			this.K_D_A.text = string.Concat(new string[]
			{
				this.killCount.ToString(),
				"/",
				this.deathCount.ToString(),
				"/",
				this.assistantCount.ToString()
			});
		}
		if (this.recordData.battleinfos == null)
		{
			this.JDDZBar.gameObject.SetActive(false);
			this.LZXFBar.gameObject.SetActive(false);
			this.SDDLDBar.gameObject.SetActive(false);
			this.JJCBar.gameObject.SetActive(false);
		}
		else
		{
			int[] array = new int[2];
			array = ModelManager.Instance.GetBattleTypeInfo("800055");
			this.JDDZCount.text = array[1].ToString() + "场";
			this.JDDZPercent.text = this.CountWinPercent(array[0], array[1]);
			this.JDDZBar.width = this.CountWinPercentBar(array[0], array[1]);
			array = ModelManager.Instance.GetBattleTypeInfo("80006");
			this.SDDLDCount.text = array[1].ToString() + "场";
			this.SDDLDPercent.text = this.CountWinPercent(array[0], array[1]);
			this.SDDLDBar.width = this.CountWinPercentBar(array[0], array[1]);
			array = ModelManager.Instance.GetBattleTypeInfo("80003");
			this.JJCCount.text = array[1].ToString() + "场";
			this.JJCPercent.text = this.CountWinPercent(array[0], array[1]);
			this.JJCBar.width = this.CountWinPercentBar(array[0], array[1]);
			array = ModelManager.Instance.GetBattleTypeInfo("80001");
			this.LZXFCount.text = array[1].ToString() + "场";
			this.LZXFPercent.text = this.CountWinPercent(array[0], array[1]);
			this.LZXFBar.width = this.CountWinPercentBar(array[0], array[1]);
		}
		string stageImg = ModelManager.Instance.Get_LadderLevel().StageImg;
		if (stageImg == "[]")
		{
			this.badge.spriteName = string.Empty;
		}
		else
		{
			this.badge.spriteName = stageImg;
		}
		this.scoreNum.text = this.recordData.ladderscore.ToString();
		this.tripleKill.text = this.recordData.triplekill.ToString();
		this.quataryKill.text = this.recordData.quadrakill.ToString();
		this.pentaKill.text = this.recordData.pentakill.ToString();
		this.legendary.text = this.recordData.godlike.ToString();
		if (this.recordData.herouseinfos == null)
		{
			this.heroGrid.gameObject.SetActive(false);
			this.noHero.gameObject.SetActive(true);
			this.bg1.gameObject.SetActive(true);
			return;
		}
		UITexture[] componentsInChildren = this.heroGrid.transform.GetComponentsInChildren<UITexture>();
		UILabel[] componentsInChildren2 = this.heroGrid.transform.GetComponentsInChildren<UILabel>();
		List<HeroUsedData> list = this.FindRealyHero();
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		while (num4 < list.Count && num5 < componentsInChildren.Length && num6 < componentsInChildren2.Length)
		{
			this.herovo = BaseDataMgr.instance.GetHeroMainData(list[num4].heroid);
			componentsInChildren[num5].mainTexture = ResourceManager.Load<Texture>(this.herovo.avatar_icon, true, true, null, 0, false);
			componentsInChildren2[num6 + 1].text = ToolsFacade.Instance.GetMillionsSuffix(list[num4].useinfo);
			double value2 = (double)list[num4].wincount / (double)(list[num4].wincount + list[num4].losecount) * 100.0;
			componentsInChildren2[num6 + 3].text = string.Concat(new string[]
			{
				"[36ff00]",
				Math.Round(value2, 1).ToString(),
				"%[-]/",
				(list[num4].wincount + list[num4].losecount).ToString(),
				"场"
			});
			num6 += 4;
			num4++;
			num5++;
		}
		UIGrid component = this.heroGrid.transform.GetComponent<UIGrid>();
		for (int i = this.recordData.herouseinfos.Length; i < component.transform.childCount; i++)
		{
			Transform child = component.transform.GetChild(i);
			NGUITools.SetActive(child.gameObject, false);
		}
	}

	private List<HeroUsedData> FindRealyHero()
	{
		List<HeroUsedData> list = new List<HeroUsedData>();
		HeroUsedData[] herouseinfos = this.recordData.herouseinfos;
		for (int i = 0; i < herouseinfos.Length; i++)
		{
			HeroUsedData item = herouseinfos[i];
			list.Add(item);
		}
		list.Sort((HeroUsedData x, HeroUsedData y) => (!(x.updatetime > y.updatetime)) ? 0 : -1);
		return list;
	}

	[DebuggerHidden]
	private IEnumerator UpdateHistoryRecord(List<MemberBattleInfo> team1, List<MemberBattleInfo> team2, List<MemberBattleInfo> team3)
	{
		AchievementView.<UpdateHistoryRecord>c__IteratorAA <UpdateHistoryRecord>c__IteratorAA = new AchievementView.<UpdateHistoryRecord>c__IteratorAA();
		<UpdateHistoryRecord>c__IteratorAA.team1 = team1;
		<UpdateHistoryRecord>c__IteratorAA.team2 = team2;
		<UpdateHistoryRecord>c__IteratorAA.team3 = team3;
		<UpdateHistoryRecord>c__IteratorAA.<$>team1 = team1;
		<UpdateHistoryRecord>c__IteratorAA.<$>team2 = team2;
		<UpdateHistoryRecord>c__IteratorAA.<$>team3 = team3;
		<UpdateHistoryRecord>c__IteratorAA.<>f__this = this;
		return <UpdateHistoryRecord>c__IteratorAA;
	}

	public void NewNewUpdateHistoryRecord(HistoryBattleData _battleRecord)
	{
		this.currbattleRecord = _battleRecord;
		this.historyBlueGrid.GetComponent<UIWidget>().alpha = 0.01f;
		this.historyRedGrid.GetComponent<UIWidget>().alpha = 0.01f;
		this.historyGreenGrid.GetComponent<UIWidget>().alpha = 0.01f;
		if (_battleRecord == null)
		{
			this.ShowOrHideRecordLst(false, true);
			return;
		}
		this.xiba.gameObject.SetActive(true);
		this.noRecord.gameObject.SetActive(false);
		this.historyBlueGrid.gameObject.SetActive(true);
		this.historyRedGrid.gameObject.SetActive(true);
		this.historyGreenGrid.gameObject.SetActive(true);
		this.bg.width = 1112;
		this.detailBtn.transform.GetComponent<BoxCollider>().enabled = true;
		this.detailBtn.GetComponent<UISprite>().spriteName = "Data_statistics_btn";
		this.title.text = LanguageManager.Instance.GetStringById("Combatgains_Describe_" + _battleRecord.battleid);
		TimeSpan timeSpan = _battleRecord.timend - _battleRecord.timestart;
		this.time.text = "/" + timeSpan.Minutes.ToString() + "分钟";
		if (_battleRecord.selfteaminfo == null || _battleRecord.enemyteaminfo == null)
		{
			ClientLogger.Error("selfteaminfo or enemyteaminfo is null @shaohe && @wangqing");
			return;
		}
		this.teaminfo.Clear();
		this.team1.Clear();
		this.team2.Clear();
		this.team3.Clear();
		this.teaminfo.AddRange(_battleRecord.selfteaminfo);
		this.teaminfo.AddRange(_battleRecord.enemyteaminfo);
		foreach (MemberBattleInfo current in this.teaminfo)
		{
			if (current.gourpId == 0)
			{
				this.team1.Add(current);
			}
			else if (current.gourpId == 1)
			{
				this.team2.Add(current);
			}
			else if (current.gourpId == 3)
			{
				this.team3.Add(current);
			}
		}
		this.coroutineManager.StartCoroutine(this.UpdateHistoryRecord(this.team1, this.team2, this.team3), true);
		this.AccountingMax();
		this.FillTheGrid(this.historyBlueGrid, this.team1);
		this.FillTheGrid(this.historyRedGrid, this.team2);
		this.FillTheGrid(this.historyGreenGrid, this.team3);
	}

	private void FillTheGrid(UIGrid _grid, List<MemberBattleInfo> _team)
	{
		GridHelper.FillGrid<PlayerBattleItem>(_grid, this._playerBattleItem, _team.Count, delegate(int idx, PlayerBattleItem comp)
		{
			comp.init(idx, _team, this.maxInfo);
		});
	}

	public void CreatBattleRecordNew(bool isMain, int battleCount, int currentPage, Dictionary<long, heroRecordInfo> battleRecord)
	{
		if (!isMain)
		{
			float num = Mathf.Ceil((float)battleCount / 5f);
			if (num < 1f)
			{
				num = 1f;
			}
			else
			{
				num = Mathf.Ceil((float)battleCount / 5f);
			}
			this.page.text = "/" + num.ToString();
			this.pageNum.text = currentPage.ToString();
		}
		else if (battleCount <= 5)
		{
			this.btn10.GetComponent<BoxCollider>().enabled = false;
			this.btn15.GetComponent<BoxCollider>().enabled = false;
			this.btn20.GetComponent<BoxCollider>().enabled = false;
		}
		else if (battleCount > 5 && battleCount <= 10)
		{
			this.btn10.GetComponent<BoxCollider>().enabled = true;
			this.btn15.GetComponent<BoxCollider>().enabled = false;
			this.btn20.GetComponent<BoxCollider>().enabled = false;
		}
		else if (battleCount > 10 && battleCount <= 15)
		{
			this.btn10.GetComponent<BoxCollider>().enabled = true;
			this.btn15.GetComponent<BoxCollider>().enabled = true;
			this.btn20.GetComponent<BoxCollider>().enabled = false;
		}
		else
		{
			this.btn10.GetComponent<BoxCollider>().enabled = true;
			this.btn15.GetComponent<BoxCollider>().enabled = true;
			this.btn20.GetComponent<BoxCollider>().enabled = true;
		}
		UIGrid grid = (!isMain) ? this.historyAllBattleRecordGrid : this.mainBattleRecordGrid;
		GridHelper.FillGrid<BattleRecord>(grid, this._battleRecordItem, battleRecord.Count, delegate(int idx, BattleRecord comp)
		{
			grid.transform.GetChild(idx).gameObject.SetActive(true);
			grid.transform.GetChild(idx).name = battleRecord.ElementAt(idx).Key.ToString();
			comp.init(isMain, idx, battleRecord.ElementAt(idx).Value);
		});
		if (!isMain && grid.transform.childCount > 0)
		{
			grid.transform.GetChild(0).transform.Find("Bg1/LightFrame").gameObject.SetActive(true);
			this.lastName = grid.transform.GetChild(0).name;
		}
		grid.repositionNow = true;
		if (isMain)
		{
			if (battleRecord.Count == 0)
			{
				this.ShowOrHideRecordLst(isMain, true);
			}
			else
			{
				this.ShowOrHideRecordLst(isMain, false);
			}
		}
	}

	private void ShowOrHideRecordLst(bool isMain, bool isShow)
	{
		if (isMain)
		{
			this.bg1.gameObject.SetActive(isShow);
		}
		else
		{
			this.xiba.gameObject.SetActive(!isShow);
			this.noRecord.gameObject.SetActive(isShow);
			this.bg.width = 1426;
			this.detailBtn.transform.GetComponent<BoxCollider>().enabled = !isShow;
			this.detailBtn.GetComponent<UISprite>().spriteName = "Data_statistics_btn_gray";
			this.title.text = string.Empty;
			this.time.text = string.Empty;
			this.historyBlueGrid.gameObject.SetActive(!isShow);
			this.historyRedGrid.gameObject.SetActive(!isShow);
			this.historyGreenGrid.gameObject.SetActive(!isShow);
		}
	}

	public void CreatBattleList(Dictionary<long, heroRecordInfo> battleRecord)
	{
		float num = Mathf.Ceil((float)this.AllCount / 5f);
		if (num < 1f)
		{
			num = 1f;
		}
		else
		{
			num = Mathf.Ceil((float)this.AllCount / 5f);
		}
		this.page.text = "/" + num.ToString();
		this.pageNum.text = this.PageNum.ToString();
		GridHelper.FillGrid<BattleRecord>(this.historyAllBattleRecordGrid, this._battleRecordItem, battleRecord.Count, delegate(int idx, BattleRecord comp)
		{
			this.historyAllBattleRecordGrid.transform.GetChild(idx).gameObject.SetActive(true);
			this.historyAllBattleRecordGrid.transform.GetChild(idx).name = battleRecord.ElementAt(idx).Key.ToString();
			comp.init(false, idx, battleRecord.ElementAt(idx).Value);
		});
	}

	public void CreatBattleRecord(bool isMain, int count = 0, string battleType = "All", GameObject go = null)
	{
		this.mainBattleRecordGrid.repositionNow = true;
		this.historyAllBattleRecordGrid.repositionNow = true;
	}

	public void SelectBattle(bool isMain, string battleType)
	{
		this.isInMainPanel = isMain;
		this.battleID = battleType;
		if (battleType == "Twenty" || battleType == "All")
		{
			this.battleID = string.Empty;
		}
		else if (battleType == "Other")
		{
			this.battleID = "80001,80005,80007,80021,80022";
		}
		this.ApplyHeroRecordInfo(this.battleID, 1);
		if (this.isInMainPanel)
		{
			this.OnClickNumBtn(this.btn5.gameObject);
		}
	}

	public void SendMsgToGetHistoryRec(long _logID)
	{
		HistoryBattleData historyBattleData = ModelManager.Instance.CheckBattleRecord(_logID);
		if (historyBattleData != null)
		{
			this.NewNewUpdateHistoryRecord(historyBattleData);
		}
		else
		{
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在获得数据", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetHistoryRecord, param, new object[]
			{
				_logID
			});
		}
	}

	private void OnClickDetailBtn(GameObject obj = null)
	{
		if (this.AnalysisPanel.gameObject.activeInHierarchy)
		{
			Singleton<MenuTopBarView>.Instance.SetPanelVisible(true);
			this.AnalysisPanel.gameObject.SetActive(false);
			while (this.dtl_redGrid.transform.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(this.dtl_redGrid.transform.GetChild(0).gameObject);
			}
			while (this.dtl_blueGrid.transform.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(this.dtl_blueGrid.transform.GetChild(0).gameObject);
			}
			while (this.dtl_greenGrid.transform.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(this.dtl_greenGrid.transform.GetChild(0).gameObject);
			}
			return;
		}
		this.AnalysisPanel.gameObject.SetActive(true);
		Singleton<MenuTopBarView>.Instance.SetPanelVisible(false);
		if (ModelManager.Instance.Get_GetMyAchievementData_X().historyBattle == null)
		{
			this.ShowOrHideRecordLst(false, true);
			return;
		}
		TimeSpan timeSpan = this.currbattleRecord.timend - this.currbattleRecord.timestart;
		this.dtl_date.text = this.currbattleRecord.timestart.Month.ToString() + "-" + this.currbattleRecord.timestart.Day.ToString();
		this.dtl_startTime.text = DateTime.Parse(this.currbattleRecord.timestart.ToString()).ToString("HH:mm");
		this.dtl_timeLength.text = timeSpan.Minutes.ToString() + "分钟";
		this.dtl_type.text = LanguageManager.Instance.GetStringById("Combatgains_Describe_" + this.currbattleRecord.battleid);
		int num = (int)this.maxInfo.blueTeamKill;
		int num2 = (int)this.maxInfo.redTeamKill;
		int num3 = (int)this.maxInfo.greenTeamKill;
		int num4 = (int)this.maxInfo.blueTeamMoney;
		int num5 = (int)this.maxInfo.redTeamMoney;
		int num6 = (int)this.maxInfo.greenTeamMoney;
		if (this.currbattleRecord.teamInfos.Count == 3)
		{
			this.dtl_killCount.text = string.Concat(new string[]
			{
				num.ToString(),
				"/",
				num2.ToString(),
				"/",
				num3.ToString()
			});
			this.dtl_money.text = string.Concat(new string[]
			{
				((double)num4 / 1000.0).ToString("F1"),
				"K/",
				((double)num5 / 1000.0).ToString("F1"),
				"K/",
				((double)num6 / 1000.0).ToString("F1"),
				"K"
			});
		}
		else
		{
			this.dtl_killCount.text = num.ToString() + "/" + num2.ToString();
			this.dtl_money.text = ((double)num4 / 1000.0).ToString("F1") + "K/" + ((double)num5 / 1000.0).ToString("F1") + "K";
		}
		this.coroutineManager.StartCoroutine(this.UpdateHistoryDetail(), true);
	}

	private void TeamCount()
	{
		foreach (MemberBattleInfo current in this.team1)
		{
			if (current.playercounter != null)
			{
				this.maxInfo.blueTeamMoney += (double)current.playercounter.allmoney;
				this.maxInfo.blueTeamDam += current.playercounter.allDamage;
				this.maxInfo.blueTeamBeDam += current.playercounter.allBeDamage;
				double num = (double)(current.playercounter.killHoreCount + current.playercounter.helpKillHoreCount) / ((this.maxInfo.blueTeamKill != 0.0) ? this.maxInfo.blueTeamKill : 1.0) * 100.0;
				this.maxInfo.blue_co_op = ((this.maxInfo.blue_co_op <= num) ? num : this.maxInfo.blue_co_op);
			}
		}
		foreach (MemberBattleInfo current2 in this.team2)
		{
			if (current2.playercounter != null)
			{
				this.maxInfo.redTeamMoney += (double)current2.playercounter.allmoney;
				this.maxInfo.redTeamDam += current2.playercounter.allDamage;
				this.maxInfo.redTeamBeDam += current2.playercounter.allBeDamage;
				double num2 = (double)(current2.playercounter.killHoreCount + current2.playercounter.helpKillHoreCount) / ((this.maxInfo.redTeamKill != 0.0) ? this.maxInfo.redTeamKill : 1.0) * 100.0;
				this.maxInfo.red_co_op = ((this.maxInfo.red_co_op <= num2) ? num2 : this.maxInfo.red_co_op);
			}
		}
		foreach (MemberBattleInfo current3 in this.team3)
		{
			if (current3.playercounter != null)
			{
				this.maxInfo.greenTeamMoney += (double)current3.playercounter.allmoney;
				this.maxInfo.greenTeamDam += current3.playercounter.allDamage;
				this.maxInfo.greenTeamBeDam += current3.playercounter.allBeDamage;
				double num3 = (double)(current3.playercounter.killHoreCount + current3.playercounter.helpKillHoreCount) / ((this.maxInfo.greenTeamKill != 0.0) ? this.maxInfo.greenTeamKill : 1.0) * 100.0;
				this.maxInfo.green_co_op = ((this.maxInfo.green_co_op <= num3) ? num3 : this.maxInfo.green_co_op);
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator UpdateHistoryDetail()
	{
		AchievementView.<UpdateHistoryDetail>c__IteratorAB <UpdateHistoryDetail>c__IteratorAB = new AchievementView.<UpdateHistoryDetail>c__IteratorAB();
		<UpdateHistoryDetail>c__IteratorAB.<>f__this = this;
		return <UpdateHistoryDetail>c__IteratorAB;
	}

	private void FillTheDetlGrid(UIGrid _grid, List<MemberBattleInfo> _team, double co_op, long teamDam, long teamBeDam, double teamKill)
	{
		GridHelper.FillGrid<PlayerBattleDetailsItem>(_grid, this._playerBattleDetailsItem, _team.Count, delegate(int idx, PlayerBattleDetailsItem comp)
		{
			comp.init(idx, _team, this.maxInfo, co_op, teamDam, teamBeDam, teamKill);
		});
	}

	public void UpdateFrameLight(string name)
	{
		if (this.lastName != null && null != this.historyAllBattleRecordGrid.transform.Find(this.lastName))
		{
			BattleRecord.TempObj = this.historyAllBattleRecordGrid.transform.Find(this.lastName).gameObject;
			this.historyAllBattleRecordGrid.transform.Find(this.lastName + "/Bg1/LightFrame").gameObject.SetActive(false);
		}
		BattleRecord.TempObj = this.historyAllBattleRecordGrid.transform.Find(name).gameObject;
		this.historyAllBattleRecordGrid.transform.Find(name + "/Bg1/LightFrame").gameObject.SetActive(true);
		this.lastName = name;
	}

	public void AccountingMax()
	{
		this.ClearMaxInfo();
		foreach (MemberBattleInfo current in this.teaminfo)
		{
			if (current.playercounter != null)
			{
				this.maxInfo.maxMoney = ((this.maxInfo.maxMoney <= current.playercounter.allmoney) ? current.playercounter.allmoney : this.maxInfo.maxMoney);
				this.maxInfo.maxK = ((this.maxInfo.maxK <= current.playercounter.killHoreCount) ? current.playercounter.killHoreCount : this.maxInfo.maxK);
				this.maxInfo.maxD = ((this.maxInfo.maxD >= current.playercounter.deadCount) ? current.playercounter.deadCount : this.maxInfo.maxD);
				this.maxInfo.maxA = ((this.maxInfo.maxA <= current.playercounter.helpKillHoreCount) ? current.playercounter.helpKillHoreCount : this.maxInfo.maxA);
				int killMonsterCount = current.playercounter.killMonsterCount;
				this.maxInfo.maxMonsterKill = ((this.maxInfo.maxMonsterKill <= killMonsterCount) ? killMonsterCount : this.maxInfo.maxMonsterKill);
				this.maxInfo.maxDamage = ((this.maxInfo.maxDamage <= current.playercounter.allDamage) ? current.playercounter.allDamage : this.maxInfo.maxDamage);
				this.maxInfo.maxBedamage = ((this.maxInfo.maxBedamage <= current.playercounter.allBeDamage) ? current.playercounter.allBeDamage : this.maxInfo.maxBedamage);
				this.maxInfo.maxTower = ((this.maxInfo.maxTower <= current.playercounter.killTowerCount) ? current.playercounter.killTowerCount : this.maxInfo.maxTower);
				int num = current.playercounter.deadCount;
				if (num == 0)
				{
					num = 1;
				}
				double num2 = (double)(current.playercounter.killHoreCount + current.playercounter.helpKillHoreCount) / (double)num * 3.0;
				num2 = Math.Round(num2, 1);
				this.maxInfo.maxEvaluate = ((this.maxInfo.maxEvaluate <= num2) ? num2 : this.maxInfo.maxEvaluate);
				if (current.isWin)
				{
					this.maxInfo.win_maxEvaluate = ((this.maxInfo.win_maxEvaluate <= num2) ? num2 : this.maxInfo.win_maxEvaluate);
				}
				if (current.gourpId == 0)
				{
					this.maxInfo.blueTeamKill += (double)current.playercounter.killHoreCount;
				}
				if (current.gourpId == 1)
				{
					this.maxInfo.redTeamKill += (double)current.playercounter.killHoreCount;
				}
				if (current.gourpId == 3)
				{
					this.maxInfo.greenTeamKill += (double)current.playercounter.killHoreCount;
				}
			}
		}
		this.TeamCount();
		List<MemberBattleInfo> list = new List<MemberBattleInfo>();
		if (this.team1.Count > 0 && this.team1[0].isWin)
		{
			list.AddRange(this.team1);
		}
		else if (this.team2.Count > 0 && this.team2[0].isWin)
		{
			list.AddRange(this.team2);
		}
		else
		{
			list.AddRange(this.team3);
		}
		this.maxInfo.maxMVP = this.AccountMVP(this.maxInfo.win_maxEvaluate, list);
	}

	private MemberBattleInfo AccountMVP(double evaluate, List<MemberBattleInfo> teamInfo)
	{
		if (teamInfo == null)
		{
			return null;
		}
		int i = 0;
		while (i < teamInfo.Count)
		{
			if (teamInfo[i].playercounter == null)
			{
				teamInfo.Remove(teamInfo[i]);
			}
			else
			{
				int num = teamInfo[i].playercounter.deadCount;
				if (num == 0)
				{
					num = 1;
				}
				double num2 = (double)(teamInfo[i].playercounter.killHoreCount + teamInfo[i].playercounter.helpKillHoreCount) / (double)num * 3.0;
				num2 = Math.Round(num2, 1);
				if (num2 != evaluate)
				{
					teamInfo.Remove(teamInfo[i]);
				}
				else
				{
					i++;
				}
			}
		}
		if (teamInfo.Count == 0)
		{
			return null;
		}
		teamInfo = (from item in teamInfo
		orderby item.playercounter.allmoney descending, item.playercounter.allDamage descending
		select item).ToList<MemberBattleInfo>();
		return teamInfo[0];
	}

	private void ClearMaxInfo()
	{
		this.maxInfo.maxMoney = 0;
		this.maxInfo.maxK = 0;
		this.maxInfo.maxD = 10000;
		this.maxInfo.maxA = 0;
		this.maxInfo.maxEvaluate = 0.0;
		this.maxInfo.maxMonsterKill = 0;
		this.maxInfo.maxTower = 0;
		this.maxInfo.maxDamage = 0L;
		this.maxInfo.maxBedamage = 0L;
		this.maxInfo.red_co_op = 0.0;
		this.maxInfo.blue_co_op = 0.0;
		this.maxInfo.green_co_op = 0.0;
		this.maxInfo.maxMVP = null;
		this.maxInfo.redTeamDam = 0L;
		this.maxInfo.blueTeamDam = 0L;
		this.maxInfo.greenTeamDam = 0L;
		this.maxInfo.redTeamBeDam = 0L;
		this.maxInfo.blueTeamBeDam = 0L;
		this.maxInfo.greenTeamBeDam = 0L;
		this.maxInfo.redTeamKill = 0.0;
		this.maxInfo.blueTeamKill = 0.0;
		this.maxInfo.greenTeamKill = 0.0;
		this.maxInfo.redTeamMoney = 0.0;
		this.maxInfo.blueTeamMoney = 0.0;
		this.maxInfo.greenTeamMoney = 0.0;
		this.maxInfo.win_maxEvaluate = 0.0;
	}
}
