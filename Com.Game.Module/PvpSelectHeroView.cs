using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using Common;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaHeros;
using MobaHeros.Pvp;
using MobaProtocol;
using MobaProtocol.Data;
using Newbie;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Com.Game.Module
{
	public class PvpSelectHeroView : BaseView<PvpSelectHeroView>
	{
		private UISprite _uiConfirmBtn;

		private UISprite _uiConfirmGaryBtn;

		private Transform _uiSelectRandomHeroBtn;

		private UISprite _uiSelectRandomHeroHintPic;

		private UILabel _uiSelectRandomHeroNumText;

		private Transform _uiRandomPointText;

		private UILabel _uiRandomPointContent;

		private Transform _uiMask;

		private UILabel _uiLeftTime;

		private EffectPlayTool _uiLeftTimeEffect;

		private UILabel _uiRightTime;

		private EffectPlayTool _uiRightTimeEffect;

		private UIGrid _uiLeftGrid;

		private UIGrid _uiLeftGrid3;

		private UIGrid _uiRightGrid;

		private UIGrid _uiRightGrid3;

		private UIGrid _uiCenterGrid;

		private UIScrollView _uiCenterScrollView;

		private UILabel _uiLeftTeam;

		private UILabel _uiRightTeam;

		private UILabel _uiTopLabel;

		private Transform _uiSkinPanel;

		private Transform _uiSummonerSkillBtn;

		private UITexture _uiSummonerSkillTex;

		private UILabel _uiTipSelectHero;

		private UISprite _uiSkillNormalFrame;

		private UISprite[] _uiSkillSelectFrames;

		private UISprite[] _uiSkillTexs = new UISprite[4];

		private UILabel _uiHeroName;

		private UILabel _uiHeroFeature;

		private UIToggle _skinButton;

		private UIToggle _heroButton;

		private BoxCollider _skinButtonCollider;

		private UILabel _heroButtonText;

		private UILabel _skinButtonText;

		private Transform _uiSkillDescrip;

		private UILabel _uiSkillIntroName;

		private UILabel _uiSkillIntroType;

		private UILabel _uiSkillIntroText;

		private UISprite _uiLeftSign;

		private UISprite _uiRightSign;

		private Transform _transBottomAnchor;

		private string _heroIdForSkillTips = string.Empty;

		private Transform _uiSwitchHeroInfo;

		private UITexture _uiSelfGetHeroIcon;

		private UITexture _uiOtherGetHeroIcon;

		private Transform _uiSelfGetHeroHint;

		private Transform _uiOtherGetHeroHint;

		private Transform _uiCancelSwitchBtn;

		private Transform _uiAcceptSwitchBtn;

		private Transform _uiRefuseSwitchBtn;

		private Transform _uiSwitchingBg;

		private Transform _uiSwitchResultBg;

		private UILabel _uiSwitchingHintText;

		private Transform _uiHeroAreaBg;

		private Transform _uiChangePic;

		private UILabel _uiSwitchResultText;

		private bool _isClickSkill;

		private List<ReadyPlayerSampleInfo> _lmPlayers;

		private List<ReadyPlayerSampleInfo> _blPlayers;

		private Dictionary<int, NewHeroItem> _itemsDict = new Dictionary<int, NewHeroItem>();

		private readonly Dictionary<string, NewHeroItem> _allHeroes = new Dictionary<string, NewHeroItem>();

		private bool _isObserser;

		private string currentSkillID;

		private int _curNewUidSwitched;

		private NewHeroItem _heroItem;

		private SelectHeroSkillItem _selectSkillItem;

		private BattleType _battleType = BattleType.PVP;

		private readonly List<string> _allHeroIds = new List<string>();

		private readonly List<string> _forrbideHeroIds = new List<string>();

		private readonly CoroutineManager _coroutineManager = new CoroutineManager();

		private bool _isSelfSelected;

		private bool isChooseOK;

		private GameObject _lastSkillBtn;

		private bool register;

		private int leftSelectCount;

		private int rightSelectCount;

		private SkinPanel skinPanel;

		private int protectTime = 100;

		private static string lightSkin = "[00e7fc]" + LanguageManager.Instance.GetStringById("ChooseHeroUI_Paging_Skin");

		private static string darkSkin = "[497478]" + LanguageManager.Instance.GetStringById("ChooseHeroUI_Paging_Skin");

		private static string lightHero = "[00e7fc]" + LanguageManager.Instance.GetStringById("ChooseHeroUI_Paging_Hero");

		private static string darkHero = "[497478]" + LanguageManager.Instance.GetStringById("ChooseHeroUI_Paging_Hero");

		private int saveSkinId;

		private int recordBuySkinId;

		private bool buyAndChoose;

		private string _lastSelectHero = string.Empty;

		private List<int> _curCanSwitchHeroNewUids = new List<int>();

		private int _curRandomSelHeroPoint;

		private int _randomSelHeroCount;

		private int _leftRandomSelHeroPoint;

		private List<int> _curRefusedSwitchHeroNewUids = new List<int>();

		private int _costRandomSelHeroCount;

		private string prevousSelect = string.Empty;

		public PvpSelectHeroView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/NewUI/SelectHero/PvpSelectHeroView");
		}

		public override void Init()
		{
			base.Init();
			this._uiConfirmBtn = this.transform.Find("BottomAnchor/Btn/ConfirmBtn").GetComponent<UISprite>();
			this._uiConfirmGaryBtn = this._uiConfirmBtn.transform.Find("Gray").GetComponent<UISprite>();
			this._uiSelectRandomHeroBtn = this.transform.Find("BottomAnchor/Btn/SelectRandomHeroBtn");
			this._uiSelectRandomHeroHintPic = this._uiSelectRandomHeroBtn.Find("Sprite").GetComponent<UISprite>();
			this._uiSelectRandomHeroNumText = this.transform.Find("BottomAnchor/Btn/SelectRandomHeroNumText").GetComponent<UILabel>();
			this._uiRandomPointText = this.transform.Find("BottomAnchor/Btn/RandomPointText");
			this._uiRandomPointContent = this.transform.Find("BottomAnchor/Btn/RandomPointText/RandomPointContent").GetComponent<UILabel>();
			this._uiMask = this.transform.Find("BottomAnchor/Panel/Mask");
			this._uiLeftSign = this.transform.Find("TopAnchor/Left").GetComponent<UISprite>();
			this._uiRightSign = this.transform.Find("TopAnchor/Right").GetComponent<UISprite>();
			this._uiLeftTime = this.transform.Find("TopAnchor/Left/TimeLabel").GetComponent<UILabel>();
			this._uiLeftTimeEffect = this.transform.FindChild("TopAnchor/Left/Fx_match_number").GetComponent<EffectPlayTool>();
			this._uiRightTime = this.transform.Find("TopAnchor/Right/TimeLabel").GetComponent<UILabel>();
			this._uiRightTimeEffect = this.transform.FindChild("TopAnchor/Right/Fx_match_number").GetComponent<EffectPlayTool>();
			this._uiTopLabel = this.transform.Find("TopAnchor/Sprite/Label").GetComponent<UILabel>();
			this._uiLeftGrid = this.transform.Find("LeftAnchor/Grid").GetComponent<UIGrid>();
			this._uiLeftGrid3 = this.transform.Find("LeftAnchor/Grid3").GetComponent<UIGrid>();
			this._uiRightGrid = this.transform.Find("RightAnchor/Grid").GetComponent<UIGrid>();
			this._uiRightGrid3 = this.transform.Find("RightAnchor/Grid3").GetComponent<UIGrid>();
			this._uiCenterGrid = this.transform.Find("CenterAnchor/Panel/Grid").GetComponent<UIGrid>();
			this._uiCenterScrollView = this.transform.Find("CenterAnchor/Panel").GetComponent<UIScrollView>();
			this._uiLeftTeam = this.transform.Find("LeftAnchor/Team").GetComponent<UILabel>();
			this._uiRightTeam = this.transform.Find("RightAnchor/Team").GetComponent<UILabel>();
			this._uiSkinPanel = this.transform.Find("CenterAnchor/SkinPanel");
			this._uiSkinPanel.gameObject.SetActive(false);
			this._uiSummonerSkillBtn = this.transform.Find("BottomAnchor/Btn/SummonerSkillBtn");
			this._uiSummonerSkillTex = this.transform.Find("BottomAnchor/Btn/SummonerSkillBtn/Skill/Texture").GetComponent<UITexture>();
			this._transBottomAnchor = this.transform.Find("BottomAnchor");
			this._uiTipSelectHero = this.transform.Find("BottomAnchor/Bottom-Bg/Label").GetComponent<UILabel>();
			this._uiSkillNormalFrame = this.transform.Find("BottomAnchor/HeroSkills/HeroName/Grid/skill1/Normal-Frame").GetComponent<UISprite>();
			UISprite component = this.transform.Find("BottomAnchor/HeroSkills/HeroName/Grid/skill1/Normal-Frame/SelectFrame1").GetComponent<UISprite>();
			UISprite component2 = this.transform.Find("BottomAnchor/HeroSkills/HeroName/Grid/skill2/Normal-Frame/SelectFrame2").GetComponent<UISprite>();
			UISprite component3 = this.transform.Find("BottomAnchor/HeroSkills/HeroName/Grid/skill3/Normal-Frame/SelectFrame3").GetComponent<UISprite>();
			UISprite component4 = this.transform.Find("BottomAnchor/HeroSkills/HeroName/Grid/skill4/Normal-Frame/SelectFrame4").GetComponent<UISprite>();
			this._uiSkillSelectFrames = new UISprite[]
			{
				component,
				component2,
				component3,
				component4
			};
			UISprite component5 = this.transform.Find("BottomAnchor/HeroSkills/HeroName/Grid/skill1/Normal-Frame/Sprite1").GetComponent<UISprite>();
			UISprite component6 = this.transform.Find("BottomAnchor/HeroSkills/HeroName/Grid/skill2/Normal-Frame/Sprite2").GetComponent<UISprite>();
			UISprite component7 = this.transform.Find("BottomAnchor/HeroSkills/HeroName/Grid/skill3/Normal-Frame/Sprite3").GetComponent<UISprite>();
			UISprite component8 = this.transform.Find("BottomAnchor/HeroSkills/HeroName/Grid/skill4/Normal-Frame/Sprite4").GetComponent<UISprite>();
			this._uiSkillTexs = new UISprite[]
			{
				component5,
				component6,
				component7,
				component8
			};
			this._uiHeroName = this.transform.Find("BottomAnchor/HeroSkills/HeroName").GetComponent<UILabel>();
			this._uiHeroFeature = this.transform.Find("BottomAnchor/HeroSkills/HeroName/Label").GetComponent<UILabel>();
			this._uiSkillDescrip = this.transform.Find("BottomAnchor/HeroSkills/SkillInfoPanel");
			this._uiSkillIntroName = this._uiSkillDescrip.transform.Find("SkillName").GetComponent<UILabel>();
			this._uiSkillIntroType = this._uiSkillDescrip.transform.Find("SkillType").GetComponent<UILabel>();
			this._uiSkillIntroText = this._uiSkillDescrip.transform.Find("SkillTxt").GetComponent<UILabel>();
			this._skinButton = this.transform.Find("CenterAnchor/Button-Skin").GetComponent<UIToggle>();
			this._skinButtonText = this._skinButton.transform.Find("Label").GetComponent<UILabel>();
			this._heroButton = this.transform.Find("CenterAnchor/Button-Hero").GetComponent<UIToggle>();
			this._skinButtonCollider = this.transform.Find("CenterAnchor/Button-Skin").GetComponent<BoxCollider>();
			this._heroButtonText = this._heroButton.transform.Find("Label").GetComponent<UILabel>();
			this._uiSwitchHeroInfo = this.transform.Find("LeftAnchor/SwitchHeroInfo");
			this._uiSelfGetHeroIcon = this._uiSwitchHeroInfo.Find("group/SelfGetHeroIcon").GetComponent<UITexture>();
			this._uiOtherGetHeroIcon = this._uiSwitchHeroInfo.Find("group/OtherGetHeroIcon").GetComponent<UITexture>();
			this._uiSelfGetHeroHint = this._uiSwitchHeroInfo.Find("group/SelfGetHeroHint");
			this._uiOtherGetHeroHint = this._uiSwitchHeroInfo.Find("group/OtherGetHeroHint");
			this._uiCancelSwitchBtn = this._uiSwitchHeroInfo.Find("group/CancelSwitchBtn");
			this._uiAcceptSwitchBtn = this._uiSwitchHeroInfo.Find("group/AcceptSwitchBtn");
			this._uiRefuseSwitchBtn = this._uiSwitchHeroInfo.Find("group/RefuseSwitchBtn");
			this._uiSwitchingBg = this._uiSwitchHeroInfo.Find("SwitchingBg");
			this._uiSwitchResultBg = this._uiSwitchHeroInfo.Find("SwitchResultBg");
			this._uiSwitchingHintText = this._uiSwitchHeroInfo.Find("group/SwitchingHintText").gameObject.GetComponent<UILabel>();
			this._uiHeroAreaBg = this._uiSwitchHeroInfo.Find("group/HeroAreaBg");
			this._uiChangePic = this._uiSwitchHeroInfo.Find("group/ChangePic");
			this._uiSwitchResultText = this._uiSwitchHeroInfo.Find("SwitchResultBg/SwitchResultText").gameObject.GetComponent<UILabel>();
			UIEventListener.Get(this._uiConfirmBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickConfirmGame);
			UIEventListener.Get(this._uiSelectRandomHeroBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickSelectRandomHero);
			UIEventListener.Get(this._uiSummonerSkillBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickSummonerSkillBtn);
			for (int i = 0; i < this._uiSkillTexs.Length; i++)
			{
				UIEventListener.Get(this._uiSkillTexs[i].gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickSkillBtn);
				UIEventListener.Get(this._uiSkillTexs[i].gameObject).onMobileHover = new UIEventListener.BoolDelegate(this.MobileHoverSkillBtn);
			}
			UIEventListener.Get(this._uiCancelSwitchBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickCancelSwitchBtn);
			UIEventListener.Get(this._uiAcceptSwitchBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickAcceptSwitchBtn);
			UIEventListener.Get(this._uiRefuseSwitchBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickRefuseSwitchBtn);
			EventDelegate.Add(this._skinButton.onChange, new EventDelegate.Callback(this.ClickSkinOrHero));
			EventDelegate.Add(this._heroButton.onChange, new EventDelegate.Callback(this.ClickSkinOrHero));
			AudioMgr.setState(null, "Interface_Music_State", "Hero_Choose");
		}

		public override void HandleAfterOpenView()
		{
			this.SendSkinMsg();
			this.protectTime = 100;
			this._curNewUidSwitched = 0;
			this._lastSelectHero = string.Empty;
			this._curRefusedSwitchHeroNewUids.Clear();
			this._costRandomSelHeroCount = 0;
			this._uiSwitchHeroInfo.gameObject.SetActive(false);
			this._uiSelectRandomHeroBtn.gameObject.SetActive(false);
			this._uiSelectRandomHeroNumText.SetActive(false);
			this._uiRandomPointText.gameObject.SetActive(false);
			if (LevelManager.m_CurLevel.IsRandomSelectHero())
			{
				this._uiConfirmBtn.gameObject.SetActive(false);
				this._uiConfirmBtn.enabled = false;
				this._uiSelectRandomHeroBtn.gameObject.SetActive(true);
				this._uiSelectRandomHeroNumText.SetActive(true);
				this._uiRandomPointText.gameObject.SetActive(true);
				this._uiRandomPointContent.gameObject.SetActive(true);
				this.RefreshSelHeroRandomPointUI();
				this.TryRefreshSwitchHeroBtnStatus();
			}
			else
			{
				this._uiConfirmBtn.enabled = false;
				this._uiConfirmGaryBtn.enabled = true;
			}
			if (LevelManager.m_CurLevel.IsDaLuanDouPvp())
			{
				this._uiTipSelectHero.gameObject.SetActive(false);
			}
			else
			{
				this._uiTipSelectHero.gameObject.SetActive(true);
			}
			this._skinButtonText.text = PvpSelectHeroView.darkSkin;
			this._heroButtonText.text = PvpSelectHeroView.lightHero;
			Singleton<PvpManager>.Instance.SendLobbyMsg(LobbyCode.C2L_ReadySelectHero, new object[0]);
			if (!Singleton<PvpManager>.Instance.IsObserver)
			{
				CtrlManager.OpenWindow(WindowID.BarrageEmitterView, null);
				Singleton<BarrageEmitterView>.Instance.sceneType = BarrageSceneType.SelectHero;
			}
			else
			{
				CtrlManager.OpenWindow(WindowID.BarrageEmitterView, null);
				Singleton<BarrageEmitterView>.Instance.sceneType = BarrageSceneType.WatcherMode_SelectHero;
			}
			if (this._itemsDict != null && Singleton<PvpManager>.Instance != null)
			{
				NewHeroItem newHeroItem = null;
				if (this._itemsDict.TryGetValue(Singleton<PvpManager>.Instance.MyLobbyUserId, out newHeroItem) && newHeroItem != null && newHeroItem.heroData != null && !string.IsNullOrEmpty(newHeroItem.heroData.HeroID))
				{
					this.ShowHeroInfo(newHeroItem.heroData.HeroID);
				}
			}
			NewbieManager.Instance.TryHandleOpenSelHeroView();
			AutoTestController.InvokeTestLogic(AutoTestTag.EnterPvp, delegate
			{
				NewHeroItem value = this._allHeroes.FirstOrDefault((KeyValuePair<string, NewHeroItem> x) => true).Value;
				if (value)
				{
					value.OnSelectHero(value.gameObject);
				}
			}, 1f);
		}

		public override void HandleBeforeCloseView()
		{
			this.ClearResources();
			this._curCanSwitchHeroNewUids.Clear();
			this._curRandomSelHeroPoint = 0;
			this._randomSelHeroCount = 0;
			this._leftRandomSelHeroPoint = 0;
			CtrlManager.CloseWindow(WindowID.SkillView);
			CtrlManager.CloseWindow(WindowID.BarrageEmitterView);
			CtrlManager.CloseWindow(WindowID.BarragePlayerView);
			CtrlManager.CloseWindow(WindowID.PvpChoiceSkillView);
		}

		private void InitResources()
		{
			this._heroItem = Resources.Load<NewHeroItem>("Prefab/NewUI/SelectHero/SelectHeroItem");
			this._selectSkillItem = Resources.Load<SelectHeroSkillItem>("Prefab/NewUI/SelectHero/SelectHeroSkillItem");
		}

		private void ClearResources()
		{
			if (this._uiSummonerSkillTex != null && this._uiSummonerSkillTex.mainTexture != null)
			{
				this._uiSummonerSkillTex.mainTexture = null;
			}
			if (this._uiSelfGetHeroIcon != null && this._uiSelfGetHeroIcon.mainTexture != null)
			{
				this._uiSelfGetHeroIcon.mainTexture = null;
			}
			if (this._uiOtherGetHeroIcon != null && this._uiOtherGetHeroIcon.mainTexture != null)
			{
				this._uiOtherGetHeroIcon.mainTexture = null;
			}
			if (this._heroItem != null)
			{
				this.ClearUITextureRes(this._heroItem.gameObject);
				this._heroItem = null;
			}
			if (this._selectSkillItem != null)
			{
				this.ClearUITextureRes(this._selectSkillItem.gameObject);
				this._selectSkillItem = null;
			}
			if (this._uiLeftGrid != null)
			{
				this.ClearUITextureRes(this._uiLeftGrid.gameObject);
			}
			if (this._uiLeftGrid3 != null)
			{
				this.ClearUITextureRes(this._uiLeftGrid3.gameObject);
			}
			if (this._uiRightGrid != null)
			{
				this.ClearUITextureRes(this._uiRightGrid.gameObject);
			}
			if (this._uiRightGrid3 != null)
			{
				this.ClearUITextureRes(this._uiRightGrid3.gameObject);
			}
			if (this._uiCenterGrid != null)
			{
				this.ClearUITextureRes(this._uiCenterGrid.gameObject);
			}
			CachedRes.ClearResources();
			if (this.skinPanel != null)
			{
				this.skinPanel.ClearResources();
			}
			this._itemsDict.Clear();
			this._allHeroes.Clear();
		}

		private void ClearUITextureRes(GameObject inGo)
		{
			if (inGo == null)
			{
				return;
			}
			UITexture[] componentsInChildren = inGo.GetComponentsInChildren<UITexture>(true);
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					UITexture uITexture = componentsInChildren[i];
					if (uITexture != null && uITexture.mainTexture != null)
					{
						uITexture.mainTexture = null;
					}
				}
			}
		}

		public override void RegisterUpdateHandler()
		{
			this.InitResources();
			if (Singleton<PropertyView>.Instance.IsOpen)
			{
				CtrlManager.CloseWindow(WindowID.PropertyView);
			}
			if (Singleton<InvitationView>.Instance.IsOpen)
			{
				CtrlManager.CloseWindow(WindowID.InvitationView);
			}
			this.RefreshUI();
			this._transBottomAnchor.gameObject.SetActive(!this._isObserser);
			if (!this._isObserser)
			{
				ReadyPlayerSampleInfo arg_B3_0;
				if ((arg_B3_0 = this._lmPlayers.FirstOrDefault((ReadyPlayerSampleInfo obj) => obj.newUid == Singleton<PvpManager>.Instance.MyLobbyUserId)) == null)
				{
					arg_B3_0 = this._blPlayers.FirstOrDefault((ReadyPlayerSampleInfo obj) => obj.newUid == Singleton<PvpManager>.Instance.MyLobbyUserId);
				}
				ReadyPlayerSampleInfo readyPlayerSampleInfo = arg_B3_0;
				if (readyPlayerSampleInfo.selfDefSkillId == string.Empty || readyPlayerSampleInfo.selfDefSkillId == "1")
				{
					string @string = PlayerPrefs.GetString("SummonerSkill_" + ModelManager.Instance.Get_userData_filed_X("SummonerId").ToString());
					if (!string.IsNullOrEmpty(@string))
					{
						readyPlayerSampleInfo.selfDefSkillId = @string;
					}
				}
				this.ShowSummonerSkillBtnTex(this.GetSummonerSkillID(readyPlayerSampleInfo.selfDefSkillId), this.IsShowSummonerSkill(Singleton<PvpManager>.Instance.BattleId.ToString()));
			}
			if (!this.register)
			{
				MobaMessageManager.RegistMessage((ClientMsg)25025, new MobaMessageFunc(this.OnSelectHero));
				MobaMessageManager.RegistMessage((ClientMsg)25026, new MobaMessageFunc(this.OnSelectHeroOk));
				MobaMessageManager.RegistMessage((ClientMsg)25034, new MobaMessageFunc(this.OnPvpSelectHeroSkill));
				MobaMessageManager.RegistMessage((ClientMsg)23065, new MobaMessageFunc(this.OnSelectHeroInfoChanged));
				MobaMessageManager.RegistMessage((ClientMsg)23067, new MobaMessageFunc(this.OnUpdateSwitchHeroBtnStatus));
				MobaMessageManager.RegistMessage((ClientMsg)23068, new MobaMessageFunc(this.OnShowSwitchHeroInfo));
				MobaMessageManager.RegistMessage((ClientMsg)23069, new MobaMessageFunc(this.OnShowSwitchHeroResult));
				MobaMessageManager.RegistMessage((ClientMsg)23070, new MobaMessageFunc(this.OnCancelSwitchHero));
				MobaMessageManager.RegistMessage(LobbyCode.L2C_WaitForSkin, new MobaMessageFunc(this.L2C_WaitForSkin));
				MVC_MessageManager.AddListener_view(MobaGameCode.GetSummSkinList, new MobaMessageFunc(this.GetAllSkinCallBack));
				MobaMessageManager.RegistMessage((ClientMsg)23048, new MobaMessageFunc(this.AfterWearSkinMsg));
				this.register = true;
			}
		}

		private void SendSkinMsg()
		{
			long num = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(false, "正在获取皮肤...", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetSummSkinList, param, new object[]
			{
				num
			});
		}

		public override void CancelUpdateHandler()
		{
			if (this.register)
			{
				MobaMessageManager.UnRegistMessage((ClientMsg)25025, new MobaMessageFunc(this.OnSelectHero));
				MobaMessageManager.UnRegistMessage((ClientMsg)25026, new MobaMessageFunc(this.OnSelectHeroOk));
				MobaMessageManager.UnRegistMessage((ClientMsg)25034, new MobaMessageFunc(this.OnPvpSelectHeroSkill));
				MobaMessageManager.UnRegistMessage((ClientMsg)23065, new MobaMessageFunc(this.OnSelectHeroInfoChanged));
				MobaMessageManager.UnRegistMessage((ClientMsg)23067, new MobaMessageFunc(this.OnUpdateSwitchHeroBtnStatus));
				MobaMessageManager.UnRegistMessage((ClientMsg)23068, new MobaMessageFunc(this.OnShowSwitchHeroInfo));
				MobaMessageManager.UnRegistMessage((ClientMsg)23069, new MobaMessageFunc(this.OnShowSwitchHeroResult));
				MobaMessageManager.UnRegistMessage((ClientMsg)23070, new MobaMessageFunc(this.OnCancelSwitchHero));
				MobaMessageManager.UnRegistMessage(LobbyCode.L2C_WaitForSkin, new MobaMessageFunc(this.L2C_WaitForSkin));
				MVC_MessageManager.RemoveListener_view(MobaGameCode.GetSummSkinList, new MobaMessageFunc(this.GetAllSkinCallBack));
				MobaMessageManager.UnRegistMessage((ClientMsg)23048, new MobaMessageFunc(this.AfterWearSkinMsg));
				this.register = false;
			}
		}

		private void OnPvpSelectHeroSkill(MobaMessage msg)
		{
			ParamSelectHeroSkill paramSelectHeroSkill = msg.Param as ParamSelectHeroSkill;
			this.UpdateSkillView(true);
			if (paramSelectHeroSkill.userId == Singleton<PvpManager>.Instance.MyLobbyUserId)
			{
				this.OnChangeSummonerSkill(ModelManager.Instance.Get_userData_filed_X("SummonerId"), paramSelectHeroSkill.skillId);
			}
		}

		private bool IsSelfHeroSelected()
		{
			if (this._isObserser)
			{
				return true;
			}
			ReadyPlayerSampleInfo readyPlayerSampleInfo = Singleton<PvpManager>.Instance.PvpPlayers.FirstOrDefault((ReadyPlayerSampleInfo x) => x.newUid == Singleton<PvpManager>.Instance.MyLobbyUserId);
			if (readyPlayerSampleInfo == null)
			{
				UnityEngine.Debug.LogError("PvpSelectHeroView.IsSelfHeroSelected null == info, PvpManager.Instance.MyLobbyUserId= " + Singleton<PvpManager>.Instance.MyLobbyUserId);
				return false;
			}
			return readyPlayerSampleInfo.horeSelected;
		}

		public override void RefreshUI()
		{
			this.UpdateHeroData();
			this.UpdateCenterHeroList();
			this.UpdateSelfSelectOk(this.IsSelfHeroSelected());
			this._itemsDict.Clear();
			this.UpdateHeroList(true);
			this.UpdateHeroList(false);
			this.UpdateTeamName();
			this.UpdateTitle();
			this.updateButtonLabel();
			if (!LevelManager.m_CurLevel.IsRandomSelectHero())
			{
				this._skinButtonCollider.enabled = false;
			}
			else
			{
				this._skinButtonCollider.enabled = true;
			}
			this._coroutineManager.StopAllCoroutine();
			this._coroutineManager.StartCoroutine(this.SelectHeroTimer_Coroutine(), true);
			this.UpdateSelectSign();
		}

		private void GetSavedSummonerSkill()
		{
			if (PlayerPrefs.HasKey("SummonerSkill_" + ModelManager.Instance.Get_userData_filed_X("SummonerId").ToString()))
			{
				string @string = PlayerPrefs.GetString("SummonerSkill_" + ModelManager.Instance.Get_userData_filed_X("SummonerId").ToString());
				SendMsgManager.Instance.SendMsg(MobaGameCode.ChangeSummonerSKill, null, new object[]
				{
					ModelManager.Instance.Get_userData_filed_X("SummonerId"),
					@string
				});
			}
		}

		public override void Destroy()
		{
			this._coroutineManager.StopAllCoroutine();
			this._itemsDict = new Dictionary<int, NewHeroItem>();
			this.currentSkillID = null;
			this.leftSelectCount = 0;
			this.rightSelectCount = 0;
			CtrlManager.CloseWindow(WindowID.BarrageEmitterView);
			CtrlManager.CloseWindow(WindowID.BarragePlayerView);
			HomeGCManager.Instance.DoClearTextures();
			base.Destroy();
		}

		private void UpdateHeroData()
		{
			TeamType selfTeamType = Singleton<PvpManager>.Instance.SelfTeamType;
			this._lmPlayers = Singleton<PvpManager>.Instance.GetPlayersByTeam(selfTeamType);
			if (selfTeamType == TeamType.LM || Singleton<PvpManager>.Instance.IsObserver)
			{
				this._blPlayers = Singleton<PvpManager>.Instance.GetPlayersByTeam(TeamType.BL);
			}
			else if (selfTeamType == TeamType.BL)
			{
				this._blPlayers = Singleton<PvpManager>.Instance.GetPlayersByTeam(TeamType.LM);
			}
			else if (selfTeamType == TeamType.Team_3)
			{
				this._blPlayers = Singleton<PvpManager>.Instance.GetPlayersByTeam(TeamType.BL);
			}
			this._isObserser = Singleton<PvpManager>.Instance.IsObserver;
			this._isSelfSelected = this.IsSelfHeroSelected();
			this._allHeroIds.Clear();
			CharacterDataMgr.instance.UpdateHerosData();
			if (!this._isObserser)
			{
				List<string> list = new List<string>();
				for (int i = 0; i < CharacterDataMgr.instance.OwenHeros.Count; i++)
				{
					list.Add(CharacterDataMgr.instance.OwenHeros[CharacterDataMgr.instance.OwenHeros.Count - i - 1]);
				}
				this._allHeroIds.AddRange(list);
				for (int j = 0; j < Singleton<PvpManager>.Instance.freeHeros.Count; j++)
				{
					string item = Singleton<PvpManager>.Instance.freeHeros[Singleton<PvpManager>.Instance.freeHeros.Count - j - 1].Split(new char[]
					{
						','
					})[0];
					if (!this._allHeroIds.Contains(item))
					{
						this._allHeroIds.Add(item);
					}
				}
				for (int k = 0; k < CharacterDataMgr.instance.NoHaveHeros.Count; k++)
				{
					if (!this._allHeroIds.Contains(CharacterDataMgr.instance.NoHaveHeros[CharacterDataMgr.instance.NoHaveHeros.Count - k - 1]))
					{
						this._allHeroIds.Add(CharacterDataMgr.instance.NoHaveHeros[CharacterDataMgr.instance.NoHaveHeros.Count - k - 1]);
					}
				}
			}
			else
			{
				this._allHeroIds.AddRange(CharacterDataMgr.instance.AllHeros);
				List<string> list2 = new List<string>();
				for (int l = 0; l < this._allHeroIds.Count; l++)
				{
					list2.Add(this._allHeroIds[this._allHeroIds.Count - l - 1]);
				}
				this._allHeroIds.Clear();
				this._allHeroIds.AddRange(list2);
			}
		}

		private void UpdateCenterHeroList()
		{
			this._allHeroes.Clear();
			this._forrbideHeroIds.Clear();
			this.CheckSceneMode();
			GridHelper.FillGrid<NewHeroItem>(this._uiCenterGrid, this._heroItem, this._allHeroIds.Count, delegate(int idx, NewHeroItem comp)
			{
				HeroData herosData = this.ReturnHeroData(this._allHeroIds[idx]);
				comp.Init(herosData, NewHeroItem.CardType.HeroAvator, true, true);
				comp.SetSelectState(this.IsHeroSelectedByOurTeam(this._allHeroIds[idx]));
				comp.name = this._allHeroIds[idx];
				comp.OnChangeHeroCallback = new Callback<NewHeroItem>(this.OnClickHeroItem);
				if (this._isObserser)
				{
					comp.OnChangeHeroCallback = null;
				}
				this._allHeroes[this._allHeroIds[idx]] = comp;
				bool flag = false;
				bool flag2 = this._forrbideHeroIds.Contains(this._allHeroIds[idx]);
				for (int k = 0; k < Singleton<PvpManager>.Instance.freeHeros.Count; k++)
				{
					if (Singleton<PvpManager>.Instance.freeHeros[k].Contains(this._allHeroIds[idx]))
					{
						flag = true;
						break;
					}
				}
				if ((CharacterDataMgr.instance.NoHaveHeros.Contains(this._allHeroIds[idx]) && !flag) || flag2)
				{
					comp.transform.Find("Center/Sprite").GetComponent<UITexture>().material = CharacterDataMgr.instance.ReturnMaterialType(10);
					comp.ShowFreeSign(false);
					comp.transform.Find("Frame").GetComponent<UISprite>().spriteName = "PVP_select_hero_03";
					comp.GetComponent<BoxCollider>().enabled = false;
				}
				else if (CharacterDataMgr.instance.OwenHeros.Contains(this._allHeroIds[idx]))
				{
					comp.ShowFreeSign(false);
				}
				else if (flag)
				{
					comp.ShowFreeSign(true);
				}
				if (flag2)
				{
					comp.SetForrbide(flag2);
				}
				HomeGCManager.Instance.AddTexture(comp.HeroTexture.mainTexture);
			});
			for (int i = this._allHeroIds.Count; i < this._allHeroIds.Count + 4; i++)
			{
				GameObject gameObject;
				if (i >= this._allHeroIds.Count)
				{
					gameObject = NGUITools.AddChild(this._uiCenterGrid.gameObject, this._heroItem.gameObject);
				}
				else
				{
					gameObject = this._uiCenterGrid.transform.GetChild(i).gameObject;
				}
				gameObject.GetComponent<NewHeroItem>().Init(null, NewHeroItem.CardType.NullObject, true, true);
				HomeGCManager.Instance.AddTexture(gameObject.GetComponent<NewHeroItem>().HeroTexture.mainTexture);
			}
			for (int j = this._allHeroIds.Count + 4; j < this._allHeroIds.Count; j++)
			{
				this._uiCenterGrid.transform.GetChild(j).gameObject.SetActive(false);
			}
			this._uiCenterGrid.Reposition();
			this._uiCenterScrollView.ResetPosition();
		}

		private void CheckSceneMode()
		{
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(Singleton<PvpManager>.Instance.BattleId.ToString());
			if (dataById.script != string.Empty && dataById.script != "[]")
			{
				string[] array = dataById.script.Split(new char[]
				{
					','
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						'|'
					});
					if (array2[0] == "1")
					{
						for (int j = 1; j < array2.Length; j++)
						{
							this._forrbideHeroIds.Add(array2[j]);
						}
					}
				}
			}
		}

		private void BindHeroItem(NewHeroItem comp, ReadyPlayerSampleInfo info, NewHeroItem.CardType cardType)
		{
			try
			{
				string heroId = info.GetHeroId();
				bool flag = Singleton<PvpManager>.Instance.MyLobbyUserId == info.newUid;
				comp.InitPVP(info, cardType, flag);
				if (!string.IsNullOrEmpty(heroId))
				{
					if (info.horeSelected)
					{
						comp.Init(comp.heroData, cardType, flag, true);
					}
					else
					{
						comp.GrayHeroItem(comp.heroData);
					}
				}
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
			}
		}

		private void UpdateHeroList(bool isLeft)
		{
			UIGrid uIGrid = (!isLeft) ? this._uiRightGrid : this._uiLeftGrid;
			List<ReadyPlayerSampleInfo> players = (!isLeft) ? this._blPlayers : this._lmPlayers;
			NewHeroItem.CardType align = (!isLeft) ? NewHeroItem.CardType.HeroCardRight : NewHeroItem.CardType.HeroCardLeft;
			GridHelper.FillGrid<NewHeroItem>(uIGrid, this._heroItem, 5, delegate(int idx, NewHeroItem comp)
			{
				ClientLogger.AssertNotNull(players, null);
				ClientLogger.AssertNotNull(comp, null);
				if (idx >= players.Count)
				{
					comp.InitPVP(null, NewHeroItem.CardType.Lock, false);
				}
				else
				{
					ReadyPlayerSampleInfo readyPlayerSampleInfo = players[idx];
					ClientLogger.AssertNotNull(readyPlayerSampleInfo, null);
					this.BindHeroItem(comp, readyPlayerSampleInfo, align);
					this._itemsDict[readyPlayerSampleInfo.newUid] = comp;
					this.UpdateSkillView(isLeft);
				}
			});
			uIGrid.Reposition();
		}

		private void UpdateSelfSelectOk(bool selected)
		{
			this._isSelfSelected = selected;
			if (this._uiConfirmBtn != null)
			{
				this._uiConfirmBtn.enabled = !selected;
			}
			if (this._uiConfirmGaryBtn != null)
			{
				this._uiConfirmGaryBtn.enabled = selected;
			}
			if (this._uiMask != null && this._uiMask.gameObject != null)
			{
				this._uiMask.gameObject.SetActive(selected);
			}
		}

		private void UpdateTitle()
		{
			if (this._isObserser)
			{
				this._uiTopLabel.text = LanguageManager.Instance.GetStringById("PVPChooseHeroUI_YouTurnToChooseHero");
			}
		}

		private void UpdateTeamName()
		{
			Color32 color = new Color32(0, 202, 220, 255);
			Color32 color2 = new Color32(182, 0, 4, 255);
			Color lhs = color;
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				this._uiLeftTeam.text = LanguageManager.Instance.GetStringById("GangUpUI_BlueTeam");
				this._uiRightTeam.text = LanguageManager.Instance.GetStringById("GangUpUI_RedTeam");
			}
			else
			{
				ReadyPlayerSampleInfo item = Singleton<PvpManager>.Instance.PvpPlayers.FirstOrDefault((ReadyPlayerSampleInfo x) => x.newUid == Singleton<PvpManager>.Instance.MyLobbyUserId);
				if (this._lmPlayers.Contains(item))
				{
					this._uiLeftTeam.text = LanguageManager.Instance.GetStringById("ChooseHeroUI_YourTeam");
					this._uiRightTeam.text = LanguageManager.Instance.GetStringById("ChooseHeroUI_EnemyTeam");
				}
				else
				{
					this._uiLeftTeam.text = LanguageManager.Instance.GetStringById("ChooseHeroUI_EnemyTeam");
					this._uiRightTeam.text = LanguageManager.Instance.GetStringById("ChooseHeroUI_YourTeam");
					lhs = color2;
				}
			}
			this._uiLeftTeam.color = ((!(lhs == color)) ? color2 : color);
			UIWidget[] componentsInChildren = this._uiLeftTeam.GetComponentsInChildren<UIWidget>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].color = ((!(lhs == color)) ? color2 : color);
			}
			this._uiRightTeam.color = ((!(lhs != color)) ? color2 : color);
			UIWidget[] componentsInChildren2 = this._uiRightTeam.GetComponentsInChildren<UIWidget>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].color = ((!(lhs != color)) ? color2 : color);
			}
		}

		private void OnClickHeroItem(NewHeroItem item)
		{
			if (LevelManager.m_CurLevel.IsRandomSelectHero())
			{
				return;
			}
			if (this._isObserser || this._isSelfSelected)
			{
				return;
			}
			if (this.protectTime <= 10)
			{
				return;
			}
			int myId = Singleton<PvpManager>.Instance.MyLobbyUserId;
			ReadyPlayerSampleInfo info = Singleton<PvpManager>.Instance.PvpPlayers.FirstOrDefault((ReadyPlayerSampleInfo x) => x.newUid == myId);
			if (item.heroData.HeroID == info.GetHeroId())
			{
				return;
			}
			this.RestoreLastSelect();
			string name = item.gameObject.name;
			if (!item.CanSelect)
			{
				Singleton<TipView>.Instance.ShowViewSetText("该英雄已经被选不能被选择", 1f);
				return;
			}
			this._lastSelectHero = name;
			Singleton<PvpManager>.Instance.SelectHero(item.heroData.HeroID);
			string heroId = info.GetHeroId();
			if (!string.IsNullOrEmpty(heroId))
			{
				bool flag = this._allHeroes.ContainsKey(info.GetHeroId()) && !this.IsHeroSelectedByOurTeam(info.GetHeroId());
				this._allHeroes[heroId].SetSelectState(!flag);
			}
			item.SetSelectState(true);
			if (this._itemsDict.ContainsKey(myId))
			{
				this._itemsDict[myId].GrayHeroItem(item.heroData);
			}
			if (!this._skinButtonCollider.enabled)
			{
				this._skinButtonCollider.enabled = true;
			}
			AudioMgr.PlayUI("Play_Menu_Hero_choose", null, true, false);
			AudioMgr.SetLisenerPos_NoDir(AudioMgr.Instance.transform, Vector3.zero, 0);
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(name);
			string music_id = heroMainData.music_id;
			if (!string.IsNullOrEmpty(this.prevousSelect))
			{
				AudioMgr.UnLoadBnk(this.prevousSelect + ".bnk", true);
			}
			if (!string.IsNullOrEmpty(music_id))
			{
				this.prevousSelect = music_id;
				AudioMgr.LoadBnk(music_id + ".bnk");
			}
			HeroVoicePlayer.playHeroVoice(music_id, false, AudioMgr.Instance.exSoundObj);
		}

		public void NewbieAutoSelHero(string inHeroId)
		{
			int myLobbyUserId = Singleton<PvpManager>.Instance.MyLobbyUserId;
			this._lastSelectHero = inHeroId;
			Singleton<PvpManager>.Instance.SelectHero(inHeroId);
			NewHeroItem newHeroItem = null;
			if (this._allHeroes.TryGetValue(inHeroId, out newHeroItem) && newHeroItem != null)
			{
				newHeroItem.SetSelectState(true);
				NewHeroItem newHeroItem2 = null;
				if (this._itemsDict.TryGetValue(myLobbyUserId, out newHeroItem2) && newHeroItem2 != null)
				{
					newHeroItem2.GrayHeroItem(newHeroItem.heroData);
				}
			}
			if (!this._skinButtonCollider.enabled)
			{
				this._skinButtonCollider.enabled = true;
			}
			AudioMgr.PlayUI("Play_Menu_Hero_choose", null, true, false);
			AudioMgr.SetLisenerPos_NoDir(AudioMgr.Instance.transform, Vector3.zero, 0);
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(inHeroId);
			string music_id = heroMainData.music_id;
			if (!string.IsNullOrEmpty(this.prevousSelect))
			{
				AudioMgr.UnLoadBnk(this.prevousSelect + ".bnk", true);
			}
			if (!string.IsNullOrEmpty(music_id))
			{
				this.prevousSelect = music_id;
				AudioMgr.LoadBnk(music_id + ".bnk");
			}
			HeroVoicePlayer.playHeroVoice(music_id, false, AudioMgr.Instance.exSoundObj);
		}

		private void RestoreLastSelect()
		{
			if (this._allHeroes.ContainsKey(this._lastSelectHero))
			{
				this._allHeroes[this._lastSelectHero].SetSelectState(false);
			}
		}

		private void UpdatePlayerState(int newUId, HeroInfo hero)
		{
			if (!this._itemsDict.ContainsKey(newUId))
			{
				UnityEngine.Debug.LogError("UpdatePlayerData: failed to find " + newUId);
				return;
			}
			if (newUId == Singleton<PvpManager>.Instance.MyLobbyUserId)
			{
				this._uiTopLabel.text = LanguageManager.Instance.GetStringById("PVPChooseHeroUI_ConfirmYourChoice");
				this.saveSkinId = 0;
				this.ShowHeroInfo(hero.heroId);
				AutoTestController.InvokeTestLogic(AutoTestTag.EnterPvp, delegate
				{
					this.OnClickConfirmGame(null);
				}, 1f);
			}
			HeroData heroData = new HeroData
			{
				HeroID = hero.heroId,
				Quality = hero.quality,
				LV = CharacterDataMgr.instance.GetLevel(hero.exp),
				Stars = hero.star
			};
			if (this.IsSame(this._itemsDict[newUId].heroData, heroData))
			{
				return;
			}
			this._itemsDict[newUId].GrayHeroItem(heroData);
		}

		private bool IsSame(HeroData heroData1, HeroData heroData2)
		{
			if (heroData1 == null || heroData2 == null)
			{
				return heroData1 == heroData2;
			}
			return !(heroData1.HeroID != heroData2.HeroID) && heroData1.LV == heroData2.LV && heroData1.Quality == heroData2.Quality && heroData1.Stars == heroData2.Stars;
		}

		[DebuggerHidden]
		private IEnumerator SelectHeroTimer_Coroutine()
		{
			PvpSelectHeroView.<SelectHeroTimer_Coroutine>c__Iterator16E <SelectHeroTimer_Coroutine>c__Iterator16E = new PvpSelectHeroView.<SelectHeroTimer_Coroutine>c__Iterator16E();
			<SelectHeroTimer_Coroutine>c__Iterator16E.<>f__this = this;
			return <SelectHeroTimer_Coroutine>c__Iterator16E;
		}

		[DebuggerHidden]
		private IEnumerator ChangeSkinTimer_Coroutine(int seconds)
		{
			PvpSelectHeroView.<ChangeSkinTimer_Coroutine>c__Iterator16F <ChangeSkinTimer_Coroutine>c__Iterator16F = new PvpSelectHeroView.<ChangeSkinTimer_Coroutine>c__Iterator16F();
			<ChangeSkinTimer_Coroutine>c__Iterator16F.seconds = seconds;
			<ChangeSkinTimer_Coroutine>c__Iterator16F.<$>seconds = seconds;
			<ChangeSkinTimer_Coroutine>c__Iterator16F.<>f__this = this;
			return <ChangeSkinTimer_Coroutine>c__Iterator16F;
		}

		private void ConfirmSelectHeroOk()
		{
			if (!this._isSelfSelected)
			{
				this._isSelfSelected = true;
				Singleton<PvpManager>.Instance.SelectHeroFinish();
			}
		}

		private void OnClickConfirmGame(GameObject object1 = null)
		{
			if (this._isObserser)
			{
				return;
			}
			ReadyPlayerSampleInfo readyPlayerSampleInfo = Singleton<PvpManager>.Instance.PvpPlayers.FirstOrDefault((ReadyPlayerSampleInfo x) => x.newUid == Singleton<PvpManager>.Instance.MyLobbyUserId);
			if (readyPlayerSampleInfo == null)
			{
				UnityEngine.Debug.LogError("PvpSelectHeroView.OnClickConfirmGame, playerInfo == null");
				return;
			}
			if (string.IsNullOrEmpty(readyPlayerSampleInfo.GetHeroId()))
			{
				Singleton<TipView>.Instance.ShowViewSetText("没有选择英雄", 1f);
				return;
			}
			NewbieManager.Instance.TryHandleConfirmSelHero();
			NewbieManager.Instance.TryHandleGuideNormCastSkill(readyPlayerSampleInfo.GetHeroId());
			if (object1 != null && this.IsHeroSelectedByOurTeam(readyPlayerSampleInfo.GetHeroId()))
			{
				Singleton<TipView>.Instance.ShowViewSetText("该英雄已经被选，请换一个英雄", 1f);
				return;
			}
			if (!this._itemsDict.ContainsKey(readyPlayerSampleInfo.newUid))
			{
				this._itemsDict[readyPlayerSampleInfo.newUid] = new NewHeroItem();
				this._itemsDict[readyPlayerSampleInfo.newUid].heroData = new HeroData();
				this._itemsDict[readyPlayerSampleInfo.newUid].cardTypeRecord = NewHeroItem.CardType.Lock;
			}
			this._itemsDict[readyPlayerSampleInfo.newUid].Init(this._itemsDict[readyPlayerSampleInfo.newUid].heroData, this._itemsDict[readyPlayerSampleInfo.newUid].cardTypeRecord, true, true);
			this._uiMask.gameObject.SetActive(true);
			this.ConfirmSelectHeroOk();
		}

		private bool IsCanSelectRandomHero()
		{
			return this._randomSelHeroCount - this._costRandomSelHeroCount > 0;
		}

		private void ReduceRandomSelectHeroPoint()
		{
			this._costRandomSelHeroCount++;
			this.RefreshSelHeroRandomPointUI();
		}

		private void OnClickSelectRandomHero(GameObject inObj)
		{
			if (!LevelManager.m_CurLevel.IsRandomSelectHero())
			{
				return;
			}
			if (!this.IsCanSelectRandomHero())
			{
				return;
			}
			Singleton<PvpManager>.Instance.SelectRandomHero();
			this.ReduceRandomSelectHeroPoint();
		}

		private HeroData ReturnHeroData(string heroID)
		{
			HeroData result;
			if (heroID != null)
			{
				HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(heroID);
				if (heroInfoData == null)
				{
					result = new HeroData
					{
						Quality = 0,
						Stars = 0,
						LV = 0,
						HeroID = heroID
					};
				}
				else
				{
					result = new HeroData
					{
						Quality = heroInfoData.Grade,
						Stars = heroInfoData.Level,
						LV = CharacterDataMgr.instance.GetLevel(heroInfoData.Exp),
						HeroID = heroID
					};
				}
			}
			else
			{
				result = new HeroData
				{
					Quality = 0,
					Stars = 0,
					LV = 0,
					HeroID = heroID
				};
			}
			return result;
		}

		private bool IsHeroSelectedByOurTeam(string heroId)
		{
			return Singleton<PvpManager>.Instance.OurPlayers.Find((ReadyPlayerSampleInfo x) => x.horeSelected && x.GetHeroId() == heroId) != null;
		}

		private void DisableSelectedHero(int userId)
		{
			if (!Singleton<PvpManager>.Instance.IsOurPlayer(userId))
			{
				return;
			}
			if (!this._itemsDict.ContainsKey(userId))
			{
				return;
			}
			if (this._itemsDict[userId].heroData == null)
			{
				return;
			}
			if (this._allHeroes != null && this._allHeroes.Keys.Contains(this._itemsDict[userId].heroData.HeroID) && this._allHeroes[this._itemsDict[userId].heroData.HeroID] != null)
			{
				this._allHeroes[this._itemsDict[userId].heroData.HeroID].SetSelectState(true);
			}
		}

		private void L2C_WaitForSkin(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int seconds = (int)operationResponse.Parameters[0];
			this.TryDisableRandomSelectHeroAndSwitchHero();
			this._coroutineManager.StopAllCoroutine();
			this._coroutineManager.StartCoroutine(this.ChangeSkinTimer_Coroutine(seconds), true);
		}

		private void OnSelectHero(MobaMessage msg)
		{
			ParamSelectHero paramSelectHero = msg.Param as ParamSelectHero;
			if (paramSelectHero.heroInfo.Hero == null)
			{
				return;
			}
			this.UpdatePlayerState(paramSelectHero.userId, paramSelectHero.heroInfo);
		}

		private void OnSelectHeroInfoChanged(MobaMessage msg)
		{
			this.UpdateHeroList(true);
			this.UpdateHeroList(false);
		}

		private void OnSelectHeroOk(MobaMessage msg)
		{
			ParamSelectHero paramSelectHero = msg.Param as ParamSelectHero;
			int userId = paramSelectHero.userId;
			bool flag = paramSelectHero.heroInfo != null;
			bool flag2 = userId == Singleton<PvpManager>.Instance.MyLobbyUserId;
			ReadyPlayerSampleInfo readyPlayerSampleInfo = this._lmPlayers.FirstOrDefault((ReadyPlayerSampleInfo obj) => obj.newUid == userId) ?? this._blPlayers.FirstOrDefault((ReadyPlayerSampleInfo obj) => obj.newUid == userId);
			if (!this._itemsDict.ContainsKey(userId))
			{
				return;
			}
			if (!flag)
			{
				if (flag2)
				{
					this.UpdateSelfSelectOk(false);
					Singleton<TipView>.Instance.ShowViewSetText("该英雄已经被同队玩家选了，请换一个英雄", 1f);
					this._itemsDict[userId].InitPVP(readyPlayerSampleInfo, this._itemsDict[userId].cardTypeRecord, true);
				}
				else if (readyPlayerSampleInfo != null)
				{
					this._itemsDict[userId].InitPVP(readyPlayerSampleInfo, this._itemsDict[userId].cardTypeRecord, false);
				}
			}
			else
			{
				this.DisableSelectedHero(userId);
				if (this._itemsDict[userId].cardTypeRecord == NewHeroItem.CardType.HeroCardRight)
				{
					this.rightSelectCount++;
					if (this.rightSelectCount == this._blPlayers.Count)
					{
						this._uiRightSign.spriteName = "PVP_select_hero_sure";
					}
				}
				else
				{
					this.leftSelectCount++;
					if (this.leftSelectCount == this._lmPlayers.Count)
					{
						this._uiLeftSign.spriteName = "PVP_select_hero_sure";
					}
				}
				this._itemsDict[userId].Init(this._itemsDict[userId].heroData, this._itemsDict[userId].cardTypeRecord, true, true);
				if (flag2)
				{
					this.UpdateSelfSelectOk(true);
					this.isChooseOK = true;
					if (!LevelManager.m_CurLevel.IsRandomSelectHero())
					{
						this.ShowSkinPanel(this._itemsDict[userId].heroData.HeroID);
						this.OnSkinChanged(this.skinPanel.selection);
					}
					this._uiTopLabel.text = LanguageManager.Instance.GetStringById("PVPChooseHeroUI_WaitingForOtherPeople");
				}
				if (this.leftSelectCount == this._lmPlayers.Count && this.rightSelectCount == this._blPlayers.Count)
				{
					this._uiTopLabel.text = LanguageManager.Instance.GetStringById("PVPChooseHeroUI_TheBattleIsAboutToBegin");
				}
			}
		}

		private void UpdateSelectSign()
		{
			int userId = Singleton<PvpManager>.Instance.MyLobbyUserId;
			ReadyPlayerSampleInfo arg_4B_0 = this._lmPlayers.FirstOrDefault((ReadyPlayerSampleInfo obj) => obj.newUid == userId) ?? this._blPlayers.FirstOrDefault((ReadyPlayerSampleInfo obj) => obj.newUid == userId);
			for (int i = 0; i < this._lmPlayers.Count; i++)
			{
				if (this._lmPlayers[i].horeSelected)
				{
					this.leftSelectCount++;
					if (this.leftSelectCount == this._lmPlayers.Count)
					{
						this._uiLeftSign.spriteName = "PVP_select_hero_sure";
					}
					if (userId == this._lmPlayers[i].newUid)
					{
						this._uiTopLabel.text = LanguageManager.Instance.GetStringById("PVPChooseHeroUI_WaitingForOtherPeople");
					}
				}
			}
			for (int j = 0; j < this._blPlayers.Count; j++)
			{
				if (this._blPlayers[j].horeSelected)
				{
					this.rightSelectCount++;
					if (this.rightSelectCount == this._blPlayers.Count)
					{
						this._uiRightSign.spriteName = "PVP_select_hero_sure";
					}
					if (userId == this._blPlayers[j].newUid)
					{
						this._uiTopLabel.text = LanguageManager.Instance.GetStringById("PVPChooseHeroUI_WaitingForOtherPeople");
					}
				}
			}
			if (this.leftSelectCount == this._lmPlayers.Count && this.rightSelectCount == this._blPlayers.Count)
			{
				this._uiTopLabel.text = LanguageManager.Instance.GetStringById("PVPChooseHeroUI_TheBattleIsAboutToBegin");
			}
		}

		private void updateButtonLabel()
		{
			if (this._skinButton.value)
			{
				this._skinButtonText.text = PvpSelectHeroView.lightSkin;
				this._heroButtonText.text = PvpSelectHeroView.darkHero;
			}
			else
			{
				this._skinButtonText.text = PvpSelectHeroView.darkSkin;
				this._heroButtonText.text = PvpSelectHeroView.lightHero;
			}
		}

		private void ClickSkinOrHero()
		{
			if (!UIToggle.current.value)
			{
				return;
			}
			if (UIToggle.current.name == "Button-Hero")
			{
				if (this._uiCenterGrid.gameObject.activeInHierarchy)
				{
					return;
				}
				this._uiSkinPanel.gameObject.SetActive(false);
				this._heroButton.value = true;
				this._uiCenterGrid.gameObject.SetActive(true);
				this._uiCenterScrollView.gameObject.SetActive(true);
				this.updateButtonLabel();
			}
			else if (UIToggle.current.name == "Button-Skin")
			{
				if (this._uiSkinPanel.gameObject.activeInHierarchy)
				{
					return;
				}
				if (false || this._itemsDict[Singleton<PvpManager>.Instance.MyLobbyUserId].heroData == null)
				{
					return;
				}
				if (string.IsNullOrEmpty(this._itemsDict[Singleton<PvpManager>.Instance.MyLobbyUserId].heroData.HeroID))
				{
					return;
				}
				this._uiSkinPanel.gameObject.SetActive(true);
				this._skinButton.value = true;
				this.updateButtonLabel();
				this._uiCenterGrid.gameObject.SetActive(false);
				this._uiCenterScrollView.gameObject.SetActive(false);
				this.ShowSkinPanel(this._itemsDict[Singleton<PvpManager>.Instance.MyLobbyUserId].heroData.HeroID);
			}
		}

		private void ShowSkinPanel(string HeroID)
		{
			if (string.IsNullOrEmpty(HeroID))
			{
				return;
			}
			if (this._uiSkinPanel == null)
			{
				return;
			}
			this._uiSkinPanel.gameObject.SetActive(true);
			this._skinButton.value = true;
			this.updateButtonLabel();
			this._uiCenterGrid.gameObject.SetActive(false);
			if (this._uiSkinPanel.childCount != 0)
			{
				this.skinPanel = this._uiSkinPanel.GetComponentInChildren<SkinPanel>();
			}
			else
			{
				this.skinPanel = SkinPanel.genSkinPanel(this._uiSkinPanel);
			}
			HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(this._itemsDict[Singleton<PvpManager>.Instance.MyLobbyUserId].heroData.HeroID);
			if (heroInfoData == null)
			{
				this.skinPanel.setHeroName(HeroID, 0L, 0);
			}
			else
			{
				long heroId = heroInfoData.HeroId;
				int skinid = ModelManager.Instance.Get_heroInfo_curskin_byHeroID_X(heroId);
				this.skinPanel.setHeroName(HeroID, heroId, skinid);
			}
			this.skinPanel.onSkinChanged -= new Action<int>(this.OnSkinChanged);
			this.skinPanel.onSkinChanged += new Action<int>(this.OnSkinChanged);
			this.skinPanel.onBuyBtnEvent -= new Action<int>(this.OnBuySkinEvent);
			this.skinPanel.onBuyBtnEvent += new Action<int>(this.OnBuySkinEvent);
			this.skinPanel.onWearBtnEvent -= new Action<Transform, bool>(this.OnWearEvent);
			this.skinPanel.onWearBtnEvent += new Action<Transform, bool>(this.OnWearEvent);
			this.skinPanel.transform.localPosition = Vector3.zero;
			this.skinPanel.transform.localPosition = this.skinPanel.transform.localPosition + new Vector3(-115f, 0f, 0f);
		}

		private void OnWearEvent(Transform obj = null, bool iswear = false)
		{
			if (this.protectTime <= 2)
			{
				Singleton<TipView>.Instance.ShowViewSetText("倒数两秒不可更换皮肤", 1f);
				return;
			}
			if (null != obj.gameObject)
			{
				obj.gameObject.SetActive(iswear);
			}
			this.SendWearMsg();
		}

		private void AfterWearSkinMsg(MobaMessage msg)
		{
			int num = (int)msg.Param;
			if (SkinPanel.IsPossessSkinId(num) || num == 0)
			{
				Singleton<PvpManager>.Instance.SendLobbyMsg(LobbyCode.C2L_SelectHeroSkin, new object[]
				{
					num.ToString()
				});
			}
			HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(this._itemsDict[Singleton<PvpManager>.Instance.MyLobbyUserId].heroData.HeroID);
			if (heroInfoData == null)
			{
				this.skinPanel.SetWearBtnState(0L, num);
			}
			else
			{
				long heroId = heroInfoData.HeroId;
				this.skinPanel.SetWearBtnState(heroId, num);
			}
		}

		private void SendWearMsg()
		{
			long heroId = ModelManager.Instance.Get_heroInfo_item_byModelID_X(this._itemsDict[Singleton<PvpManager>.Instance.MyLobbyUserId].heroData.HeroID).HeroId;
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(false, "正在穿戴...", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.ChangeSkin, param, new object[]
			{
				heroId.ToString(),
				this.skinPanel.selection.ToString()
			});
		}

		private void OnBuySkinEvent(int index)
		{
			if (this.protectTime <= 2)
			{
				Singleton<TipView>.Instance.ShowViewSetText("倒数两秒不可购买皮肤", 1f);
				return;
			}
			if (CharacterDataMgr.instance.OwenHeros.Contains(this._itemsDict[Singleton<PvpManager>.Instance.MyLobbyUserId].heroData.HeroID))
			{
				this.recordBuySkinId = index;
				CtrlManager.OpenWindow(WindowID.PurchasePopupView, null);
				Singleton<PurchasePopupView>.Instance.onSuccess.Add(new Callback(this.BuySkinCallBack));
				Singleton<PurchasePopupView>.Instance.Show(GoodsSubject.Skin, this.recordBuySkinId.ToString(), 1, true);
			}
			else
			{
				Singleton<TipView>.Instance.ShowViewSetText("请先购买该英雄！", 1f);
			}
		}

		private void BuySkinCallBack()
		{
			Singleton<MenuTopBarView>.Instance.RefreshUI();
			this.SendSkinMsg();
			this.SendWearMsg();
			this.buyAndChoose = true;
		}

		private void GetAllSkinCallBack(MobaMessage msg)
		{
			if (this.skinPanel != null)
			{
				HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(this._itemsDict[Singleton<PvpManager>.Instance.MyLobbyUserId].heroData.HeroID);
				this.skinPanel.RefreshUISkinPanel(this._itemsDict[Singleton<PvpManager>.Instance.MyLobbyUserId].heroData.HeroID);
				if (heroInfoData == null)
				{
					this.skinPanel.ReFreshPrice(0L, 0);
				}
				else
				{
					long heroId = heroInfoData.HeroId;
					int skinid = ModelManager.Instance.Get_heroInfo_curskin_byHeroID_X(heroId);
					this.skinPanel.ReFreshPrice(heroId, skinid);
				}
				if (this.buyAndChoose)
				{
					this.OnSkinChanged(this.skinPanel.selection);
				}
			}
		}

		private void OnSkinChanged(int skinId)
		{
			HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(this._itemsDict[Singleton<PvpManager>.Instance.MyLobbyUserId].heroData.HeroID);
			if (heroInfoData == null)
			{
				this.skinPanel.SetWearBtnState(0L, skinId);
				this.skinPanel.SetBuyBtnState(skinId);
				this.skinPanel.ReFreshPrice(0L, skinId);
			}
			else
			{
				long heroId = heroInfoData.HeroId;
				int skinid = ModelManager.Instance.Get_heroInfo_curskin_byHeroID_X(heroId);
				this.skinPanel.SetWearBtnState(heroId, skinId);
				this.skinPanel.SetBuyBtnState(skinId);
				this.skinPanel.ReFreshPrice(heroId, skinid);
			}
			if (this._isSelfSelected)
			{
				if (this.isChooseOK)
				{
					this.isChooseOK = false;
					long heroid = 0L;
					int skinid2 = 0;
					if (heroInfoData != null)
					{
						heroid = heroInfoData.HeroId;
						skinid2 = ModelManager.Instance.Get_heroInfo_curskin_byHeroID_X(heroid);
					}
					this.skinPanel.SetWearBtnState(heroid, skinid2);
					this.skinPanel.SetBuyBtnState(skinId);
					if (SkinPanel.IsWearSkin(heroid, skinid2))
					{
						Singleton<PvpManager>.Instance.SendLobbyMsg(LobbyCode.C2L_SelectHeroSkin, new object[]
						{
							skinid2.ToString()
						});
					}
				}
			}
			else
			{
				this.saveSkinId = skinId;
			}
		}

		private void OnChangeSummonerSkill(long summonerId, string summonerSkills)
		{
			SendMsgManager.Instance.SendMsg(MobaGameCode.ChangeSummonerSKill, null, new object[]
			{
				summonerId,
				summonerSkills
			});
			this.SaveSummonerSkillLocal(summonerId, summonerSkills);
		}

		private void SaveSummonerSkillLocal(long summonerId, string summonerSkills)
		{
			PlayerPrefs.SetString("SummonerSkill_" + summonerId.ToString(), summonerSkills);
			PlayerPrefs.Save();
		}

		private void ClickSummonerSkillBtn(GameObject obj = null)
		{
			Singleton<PvpChoiceSkillView>.Instance.SetSkillID(this.currentSkillID, new Callback<string>(this.ShowView));
			CtrlManager.OpenWindow(WindowID.PvpChoiceSkillView, null);
			NewbieManager.Instance.TryHandleOpenSummonerSkill();
		}

		private void ShowView(string str)
		{
			if (this.protectTime > 2)
			{
				this.ShowSummonerSkillBtnTex(str, true);
				Singleton<PvpManager>.Instance.SelectSelfDefSkill(str);
			}
		}

		private void ShowSummonerSkillBtnTex(string skillID, bool isShow = true)
		{
			if (!isShow)
			{
				this._uiConfirmBtn.transform.localPosition = Vector3.one;
				this._uiSummonerSkillBtn.gameObject.SetActive(false);
			}
			else
			{
				this._uiConfirmBtn.transform.localPosition = new Vector3(710f, -4f, 0f);
				this._uiSummonerSkillBtn.gameObject.SetActive(true);
				SysSummonersSkillVo dataById2;
				if (skillID == null)
				{
					SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(Singleton<PvpManager>.Instance.BattleId.ToString());
					string unikey = dataById.summoners_skill.Split(new char[]
					{
						','
					})[0];
					dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersSkillVo>(unikey);
				}
				else
				{
					dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersSkillVo>(skillID);
				}
				SysSkillMainVo skillMainData = BaseDataMgr.instance.GetSkillMainData(dataById2.skill_id);
				this._uiSummonerSkillTex.mainTexture = ResourceManager.Load<Texture>(skillMainData.skill_icon, true, true, null, 0, false);
				this.currentSkillID = dataById2.id.ToString();
			}
		}

		public bool IsShowSummonerSkill(string battleId)
		{
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(battleId);
			return dataById != null && !("[]" == dataById.summoners_skill);
		}

		private string GetSummonerSkillID(string skillId)
		{
			if (!string.IsNullOrEmpty(skillId))
			{
				return skillId;
			}
			string text = null;
			string text2 = ModelManager.Instance.Get_userData_filed_X("SummSkills");
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(Singleton<PvpManager>.Instance.BattleId.ToString());
			if (dataById == null)
			{
				UnityEngine.Debug.LogError("SysBattleSceneVo can't find ID:" + Singleton<PvpManager>.Instance.BattleId);
				return text;
			}
			if (text2 == null)
			{
				text = dataById.summoners_skill.Split(new char[]
				{
					','
				})[0];
			}
			else
			{
				text = text2.Split(new char[]
				{
					','
				})[0];
				if (!dataById.summoners_skill.Split(new char[]
				{
					','
				}).ToList<string>().Contains(text))
				{
					text = dataById.summoners_skill.Split(new char[]
					{
						','
					})[0];
				}
			}
			Singleton<PvpManager>.Instance.SelectSelfDefSkill(text);
			return text;
		}

		private void UpdateSkillView(bool isLeft)
		{
			if (BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurLevelId) == null)
			{
				ClientLogger.Error("SysBattleSceneVo中找不到" + LevelManager.CurLevelId);
				return;
			}
			if (!this.IsShowSummonerSkill(Singleton<PvpManager>.Instance.BattleId.ToString()) || (!isLeft && BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurLevelId).show_enemy_squad == 0))
			{
				return;
			}
			List<ReadyPlayerSampleInfo> players = (!isLeft) ? this._blPlayers : this._lmPlayers;
			if (players == null)
			{
				return;
			}
			GridHelper.FillGrid<SelectHeroSkillItem>((!isLeft) ? this._uiRightGrid3 : this._uiLeftGrid3, this._selectSkillItem, players.Count, delegate(int idx, SelectHeroSkillItem comp)
			{
				string text = string.Empty;
				if (players.Count > idx)
				{
					text = players[idx].selfDefSkillId;
				}
				if (!string.IsNullOrEmpty(text))
				{
					comp.ShowSKill(text);
				}
			});
		}

		private int GetSkillIndex(GameObject obj)
		{
			return Array.FindIndex<UISprite>(this._uiSkillTexs, (UISprite x) => x.gameObject == obj);
		}

		private void ClickSkillBtn(GameObject obj)
		{
			this._uiSkillDescrip.GetComponent<UIPanel>().depth = 200;
			if (obj == this._lastSkillBtn)
			{
				this._uiSkillDescrip.gameObject.SetActive(!this._isClickSkill);
				this.ShowSkillFrame(this.GetSkillIndex(obj), !this._isClickSkill);
			}
			else
			{
				this._uiSkillDescrip.gameObject.SetActive(true);
				this.ShowSkillFrame(this.GetSkillIndex(obj), true);
			}
			this.ShowSkillInfo(this.GetSkillIndex(obj));
			this._isClickSkill = !this._isClickSkill;
			this._lastSkillBtn = obj;
			NewbieManager.Instance.TryHandleHeroSkillIntroduction();
		}

		private void MobileHoverSkillBtn(GameObject obj, bool hover)
		{
			if (hover)
			{
				this.ShowSkillInfo(this.GetSkillIndex(obj));
				this._uiSkillDescrip.gameObject.SetActive(true);
				this.ShowSkillFrame(this.GetSkillIndex(obj), true);
			}
			else
			{
				this._uiSkillDescrip.gameObject.SetActive(false);
				this.ShowSkillFrame(this.GetSkillIndex(obj), false);
			}
		}

		private void ShowSkillInfo(int num)
		{
			switch (num)
			{
			case 0:
				this._uiSkillDescrip.transform.localPosition = new Vector3(-466f, -3.5f, 0f);
				break;
			case 1:
				this._uiSkillDescrip.transform.localPosition = new Vector3(-336f, -3.5f, 0f);
				break;
			case 2:
				this._uiSkillDescrip.transform.localPosition = new Vector3(-206f, -3.5f, 0f);
				break;
			case 3:
				this._uiSkillDescrip.transform.localPosition = new Vector3(-76f, -3.5f, 0f);
				break;
			}
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this._heroIdForSkillTips);
			if (heroMainData == null)
			{
				ClientLogger.Error("heroId=" + this._heroIdForSkillTips + "SysHeroMainVo中找不到");
			}
			string[] array = heroMainData.skill_id.Split(new char[]
			{
				','
			});
			SysSkillMainVo skillData = SkillUtility.GetSkillData(array[num], -1, -1);
			if (skillData == null)
			{
				ClientLogger.Error("Skill's Array Index Overflow");
			}
			this._uiSkillIntroText.text = LanguageManager.Instance.GetStringById(skillData.skill_description);
			this._uiSkillIntroName.text = LanguageManager.Instance.GetStringById(skillData.skill_name);
			switch (skillData.skill_trigger)
			{
			case 0:
			case 1:
			case 2:
			case 4:
				this._uiSkillIntroType.text = LanguageManager.Instance.GetStringById("BattleSkillBarUI_ActiveSkill");
				break;
			case 3:
				this._uiSkillIntroType.text = LanguageManager.Instance.GetStringById("BattleSkillBarUI_PassiveSkill");
				break;
			}
		}

		private void SetTitle(string text)
		{
			if (this._uiTopLabel)
			{
				this._uiTopLabel.text = text;
			}
		}

		public void ShowHeroInfo(string heroName)
		{
			this.UpdateSelfSelectOk(this.IsSelfHeroSelected());
			if (this._uiSkillDescrip == null || this._uiSkillDescrip.gameObject == null)
			{
				return;
			}
			this._uiSkillDescrip.gameObject.SetActive(false);
			this.ShowSkillFrame(7, false);
			if (this._uiHeroName == null)
			{
				return;
			}
			if (!this.IsSelfHeroSelected())
			{
				this.SetTitle(LanguageManager.Instance.GetStringById("PVPChooseHeroUI_ConfirmYourChoice"));
			}
			if (this._uiTipSelectHero == null)
			{
				return;
			}
			this._uiTipSelectHero.SetActive(false);
			this._uiHeroName.SetActive(true);
			string text;
			if (heroName == null)
			{
				int myId = Singleton<PvpManager>.Instance.MyLobbyUserId;
				ReadyPlayerSampleInfo info = Singleton<PvpManager>.Instance.PvpPlayers.FirstOrDefault((ReadyPlayerSampleInfo x) => x.newUid == myId);
				text = info.GetHeroId();
			}
			else
			{
				text = heroName;
			}
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(text);
			this._uiHeroName.text = LanguageManager.Instance.GetStringById(heroMainData.name);
			this._uiHeroFeature.text = "[feb300]" + LanguageManager.Instance.GetStringById(heroMainData.hero_feature) + " | " + LanguageManager.Instance.GetStringById(heroMainData.combat_positioning).Split(new char[]
			{
				'|'
			})[1];
			this._heroIdForSkillTips = text;
			string[] array = heroMainData.skill_id.Split(new char[]
			{
				','
			});
			for (int i = 0; i < 4; i++)
			{
				SysSkillMainVo skillMainData = BaseDataMgr.instance.GetSkillMainData(array[i]);
				if (skillMainData == null)
				{
					ClientLogger.Error("skillmain 找不到：" + array[i]);
				}
				else
				{
					string[] array2 = skillMainData.skill_icon.Split(new char[]
					{
						'_'
					});
					this._uiSkillTexs[i].atlas = Resources.Load<UIAtlas>("Texture/Skiller/" + array2[0] + "_" + array2[1]);
					this._uiSkillTexs[i].spriteName = skillMainData.skill_icon;
				}
			}
		}

		private void ShowSkillFrame(int num, bool select)
		{
			for (int i = 0; i < this._uiSkillSelectFrames.Length; i++)
			{
				this._uiSkillSelectFrames[i].SetActive(i == num && select);
			}
		}

		public void InitRandomSelHeroInfo(List<int> inCanSwitchHeroNewUids, int inRandomSelHeroPoint)
		{
			this._curCanSwitchHeroNewUids = inCanSwitchHeroNewUids;
			this._curRandomSelHeroPoint = inRandomSelHeroPoint;
			this.ParseCurRandomSelHeroCountAndLeftPoint(this._curRandomSelHeroPoint);
		}

		private void OnUpdateSwitchHeroBtnStatus(MobaMessage msg)
		{
			ParamGetCanSwitchHeroInfo paramGetCanSwitchHeroInfo = msg.Param as ParamGetCanSwitchHeroInfo;
			if (paramGetCanSwitchHeroInfo == null)
			{
				return;
			}
			this._curCanSwitchHeroNewUids = paramGetCanSwitchHeroInfo.canSwitchHeroNewUids;
			this._curRandomSelHeroPoint = paramGetCanSwitchHeroInfo.randomSelHeroPoint;
			this.ParseCurRandomSelHeroCountAndLeftPoint(this._curRandomSelHeroPoint);
			this.RefreshSelHeroRandomPointUI();
			this.TryRefreshSwitchHeroBtnStatus();
		}

		public void HideAllSwitchHeroBtn()
		{
			foreach (KeyValuePair<int, NewHeroItem> current in this._itemsDict)
			{
				if (current.Value != null)
				{
					current.Value.HideSwitchHeroBtn();
				}
			}
		}

		public void TryRefreshSwitchHeroBtnStatus()
		{
			if (!LevelManager.m_CurLevel.IsRandomSelectHero())
			{
				return;
			}
			foreach (KeyValuePair<int, NewHeroItem> current in this._itemsDict)
			{
				if (current.Key == Singleton<PvpManager>.Instance.MyLobbyUserId)
				{
					if (current.Value != null)
					{
						current.Value.HideSwitchHeroBtn();
					}
				}
				else if (current.Value != null && current.Value.cardTypeRecord == NewHeroItem.CardType.HeroCardRight)
				{
					current.Value.HideSwitchHeroBtn();
				}
				else if (this._curCanSwitchHeroNewUids == null || this._curCanSwitchHeroNewUids.Count < 1)
				{
					if (current.Value != null)
					{
						current.Value.DisableSwitchHeroBtn();
					}
				}
				else if (current.Value != null)
				{
					if (this._curCanSwitchHeroNewUids.Contains(current.Key) && !this._curRefusedSwitchHeroNewUids.Contains(current.Key))
					{
						current.Value.EnableSwitchHeroBtn();
					}
					else
					{
						current.Value.DisableSwitchHeroBtn();
					}
				}
			}
		}

		private void RefreshSelHeroRandomPointUI()
		{
			if (!LevelManager.m_CurLevel.IsRandomSelectHero())
			{
				return;
			}
			if (this.IsCanSelectRandomHero())
			{
				this.EnableRandomSelectHeroBtn();
			}
			else
			{
				this.DisableRandomSelectHeroBtn();
			}
		}

		private void EnableRandomSelectHeroBtn()
		{
			int num = this._randomSelHeroCount - this._costRandomSelHeroCount;
			if (num < 0)
			{
				num = 0;
			}
			this._uiSelectRandomHeroNumText.text = num.ToString();
			this._uiRandomPointContent.text = "[FDF48C]" + this._leftRandomSelHeroPoint.ToString() + "[FFFFFF]/200";
			BoxCollider component = this._uiSelectRandomHeroBtn.GetComponent<BoxCollider>();
			if (component != null)
			{
				component.enabled = true;
			}
			UISprite component2 = this._uiSelectRandomHeroBtn.GetComponent<UISprite>();
			if (component2 != null)
			{
				component2.spriteName = "btn_yellow_qr1";
			}
			this._uiSelectRandomHeroHintPic.spriteName = "PVP_fighter_touzi_01";
		}

		private void DisableRandomSelectHeroBtn()
		{
			int num = this._randomSelHeroCount - this._costRandomSelHeroCount;
			if (num < 0)
			{
				num = 0;
			}
			this._uiSelectRandomHeroNumText.text = num.ToString();
			this._uiRandomPointContent.text = "[FFFFFF]" + this._leftRandomSelHeroPoint.ToString() + "/200";
			BoxCollider component = this._uiSelectRandomHeroBtn.GetComponent<BoxCollider>();
			if (component != null)
			{
				component.enabled = false;
			}
			UISprite component2 = this._uiSelectRandomHeroBtn.GetComponent<UISprite>();
			if (component2 != null)
			{
				component2.spriteName = "btn_yellow_qr3";
			}
			this._uiSelectRandomHeroHintPic.spriteName = "PVP_fighter_touzi_02";
		}

		private void TryDisableRandomSelectHeroAndSwitchHero()
		{
			if (!LevelManager.m_CurLevel.IsRandomSelectHero())
			{
				return;
			}
			this.DisableRandomSelectHeroBtn();
			this.HideAllSwitchHeroBtn();
		}

		private void ParseCurRandomSelHeroCountAndLeftPoint(int inCurRandomSelHeroPoint)
		{
			if (inCurRandomSelHeroPoint >= 600)
			{
				this._randomSelHeroCount = 2;
				this._leftRandomSelHeroPoint = 200;
			}
			else if (inCurRandomSelHeroPoint <= 0)
			{
				this._randomSelHeroCount = 0;
				this._leftRandomSelHeroPoint = 0;
			}
			else
			{
				this._randomSelHeroCount = inCurRandomSelHeroPoint / 200;
				this._leftRandomSelHeroPoint = inCurRandomSelHeroPoint % 200;
			}
		}

		private void OnShowSwitchHeroInfo(MobaMessage msg)
		{
			ParamShowSwitchHeroInfo paramShowSwitchHeroInfo = msg.Param as ParamShowSwitchHeroInfo;
			if (paramShowSwitchHeroInfo == null)
			{
				return;
			}
			this.DisableRandomSelectHeroBtn();
			if (paramShowSwitchHeroInfo.showSwitchHeroInfoType == EShowSwitchHeroInfoType.ReqType)
			{
				this.ShowReqSwitchHeroInfo(paramShowSwitchHeroInfo.newUid);
			}
			else if (paramShowSwitchHeroInfo.showSwitchHeroInfoType == EShowSwitchHeroInfoType.RespType)
			{
				this.ShowRespSwitchHeroInfo(paramShowSwitchHeroInfo.newUid);
			}
		}

		private void OnShowSwitchHeroResult(MobaMessage msg)
		{
			ParamShowSwitchHeroResult paramShowSwitchHeroResult = msg.Param as ParamShowSwitchHeroResult;
			if (paramShowSwitchHeroResult == null)
			{
				return;
			}
			if (this.IsCanSelectRandomHero())
			{
				this.EnableRandomSelectHeroBtn();
			}
			this.TryAddRefusedSwitchHeroNewUid((ESwitchHeroResultType)paramShowSwitchHeroResult.resultVal, paramShowSwitchHeroResult.newUid);
			if (paramShowSwitchHeroResult.resultVal == 1)
			{
				this.HideSwitchHeroInfo();
			}
			else
			{
				this.ShowSwitchHeroResultInfo(paramShowSwitchHeroResult.newUid, (ESwitchHeroResultType)paramShowSwitchHeroResult.resultVal);
				this._coroutineManager.StartCoroutine(this.DelayHideSwitchHeroInfo(1f), true);
			}
		}

		private void OnCancelSwitchHero(MobaMessage msg)
		{
			if (this.IsCanSelectRandomHero())
			{
				this.EnableRandomSelectHeroBtn();
			}
			this.HideSwitchHeroInfo();
			this.TryRefreshSwitchHeroBtnStatus();
		}

		public void SetCurNewUidSwitched(int inNewUid)
		{
			this._curNewUidSwitched = inNewUid;
		}

		private void TryAddRefusedSwitchHeroNewUid(ESwitchHeroResultType inResult, int inNewUid)
		{
			if (inResult == ESwitchHeroResultType.SwitchRefused)
			{
				this._curRefusedSwitchHeroNewUids.Add(inNewUid);
			}
		}

		private void ShowReqSwitchHeroInfo(int inNewUidSwitchTo)
		{
			this.HideAllSwitchHeroBtn();
			this.AjustSwitchHeroInfoReqOrResp();
			UIPanel component = this._uiSwitchHeroInfo.GetComponent<UIPanel>();
			if (component != null)
			{
				component.depth = 140;
			}
			this._uiSwitchHeroInfo.gameObject.SetActive(true);
			this._uiSelfGetHeroIcon.gameObject.SetActive(true);
			this._uiOtherGetHeroIcon.gameObject.SetActive(true);
			this._uiSelfGetHeroHint.gameObject.SetActive(true);
			this._uiOtherGetHeroHint.gameObject.SetActive(true);
			this._uiCancelSwitchBtn.gameObject.SetActive(true);
			this._uiAcceptSwitchBtn.gameObject.SetActive(false);
			this._uiRefuseSwitchBtn.gameObject.SetActive(false);
			this._uiSwitchingBg.gameObject.SetActive(true);
			this._uiSwitchResultBg.gameObject.SetActive(false);
			this._uiSwitchingHintText.text = "交换期间";
			this._uiHeroAreaBg.gameObject.SetActive(true);
			this._uiChangePic.gameObject.SetActive(true);
			NewHeroItem newHeroItem = null;
			if (this._itemsDict.TryGetValue(inNewUidSwitchTo, out newHeroItem) && newHeroItem != null && newHeroItem.heroData != null)
			{
				this.ShowUITextureHeroHeadIcon(this._uiSelfGetHeroIcon, newHeroItem.heroData.HeroID);
			}
			if (this._itemsDict.TryGetValue(Singleton<PvpManager>.Instance.MyLobbyUserId, out newHeroItem) && newHeroItem != null && newHeroItem.heroData != null)
			{
				this.ShowUITextureHeroHeadIcon(this._uiOtherGetHeroIcon, newHeroItem.heroData.HeroID);
			}
		}

		private void ShowRespSwitchHeroInfo(int inNewUidSwitched)
		{
			this._curNewUidSwitched = inNewUidSwitched;
			this.HideAllSwitchHeroBtn();
			this.AjustSwitchHeroInfoReqOrResp();
			UIPanel component = this._uiSwitchHeroInfo.GetComponent<UIPanel>();
			if (component != null)
			{
				component.depth = 140;
			}
			this._uiSwitchHeroInfo.gameObject.SetActive(true);
			this._uiSelfGetHeroIcon.gameObject.SetActive(true);
			this._uiOtherGetHeroIcon.gameObject.SetActive(true);
			this._uiSelfGetHeroHint.gameObject.SetActive(true);
			this._uiOtherGetHeroHint.gameObject.SetActive(true);
			this._uiAcceptSwitchBtn.gameObject.SetActive(true);
			this._uiRefuseSwitchBtn.gameObject.SetActive(true);
			this._uiCancelSwitchBtn.gameObject.SetActive(false);
			this._uiSwitchingBg.gameObject.SetActive(true);
			this._uiSwitchResultBg.gameObject.SetActive(false);
			this._uiSwitchingHintText.text = "交换请求";
			this._uiHeroAreaBg.gameObject.SetActive(true);
			this._uiChangePic.gameObject.SetActive(true);
			NewHeroItem newHeroItem = null;
			if (this._itemsDict.TryGetValue(inNewUidSwitched, out newHeroItem) && newHeroItem != null && newHeroItem.heroData != null)
			{
				this.ShowUITextureHeroHeadIcon(this._uiSelfGetHeroIcon, newHeroItem.heroData.HeroID);
			}
			if (this._itemsDict.TryGetValue(Singleton<PvpManager>.Instance.MyLobbyUserId, out newHeroItem) && newHeroItem != null && newHeroItem.heroData != null)
			{
				this.ShowUITextureHeroHeadIcon(this._uiOtherGetHeroIcon, newHeroItem.heroData.HeroID);
			}
		}

		private void ShowSwitchHeroResultInfo(int inNewUidSwitched, ESwitchHeroResultType inResultType)
		{
			this.HideAllSwitchHeroBtn();
			this.AjustSwitchHeroInfoResult();
			UIPanel component = this._uiSwitchHeroInfo.GetComponent<UIPanel>();
			if (component != null)
			{
				component.depth = 140;
			}
			this._uiSwitchHeroInfo.gameObject.SetActive(true);
			this._uiSelfGetHeroIcon.gameObject.SetActive(false);
			this._uiOtherGetHeroIcon.gameObject.SetActive(false);
			this._uiSelfGetHeroHint.gameObject.SetActive(false);
			this._uiOtherGetHeroHint.gameObject.SetActive(false);
			this._uiAcceptSwitchBtn.gameObject.SetActive(false);
			this._uiRefuseSwitchBtn.gameObject.SetActive(false);
			this._uiCancelSwitchBtn.gameObject.SetActive(false);
			this._uiSwitchingBg.gameObject.SetActive(false);
			this._uiSwitchResultBg.gameObject.SetActive(true);
			this._uiHeroAreaBg.gameObject.SetActive(false);
			this._uiChangePic.gameObject.SetActive(false);
			this._uiSwitchResultText.text = string.Empty;
			if (inResultType == ESwitchHeroResultType.SwitchRefused)
			{
				this._uiSwitchResultText.text = "[FDDC00]" + this.GetSummonerNameByNewUid(inNewUidSwitched) + "\n[FFFFFF]拒绝了你的交换请求";
			}
			else if (inResultType == ESwitchHeroResultType.TargetIsInSwitching)
			{
				this._uiSwitchResultText.text = "对方正在交换中，无法交换。";
			}
		}

		private string GetSummonerNameByNewUid(int inNewUid)
		{
			if (this._lmPlayers != null && this._lmPlayers.Count > 0)
			{
				for (int i = 0; i < this._lmPlayers.Count; i++)
				{
					if (this._lmPlayers[i] != null && this._lmPlayers[i].newUid == inNewUid)
					{
						return this._lmPlayers[i].userName;
					}
				}
			}
			if (this._blPlayers != null && this._blPlayers.Count > 0)
			{
				for (int j = 0; j < this._blPlayers.Count; j++)
				{
					if (this._blPlayers[j] != null && this._blPlayers[j].newUid == inNewUid)
					{
						return this._blPlayers[j].userName;
					}
				}
			}
			return string.Empty;
		}

		private void AjustSwitchHeroInfoReqOrResp()
		{
			Vector3 a = new Vector3(738f, 255f, 0f);
			Vector3 zero = Vector3.zero;
			if (!this.GetSelfOffsetPos(out zero))
			{
				return;
			}
			this._uiSwitchHeroInfo.localPosition = a + zero;
		}

		private void AjustSwitchHeroInfoResult()
		{
			Vector3 a = new Vector3(738f, 255f, 0f);
			Vector3 zero = Vector3.zero;
			if (!this.GetSelfOffsetPos(out zero))
			{
				return;
			}
			this._uiSwitchHeroInfo.localPosition = a + zero;
		}

		private bool GetSelfOffsetPos(out Vector3 outPos)
		{
			outPos = Vector3.zero;
			foreach (KeyValuePair<int, NewHeroItem> current in this._itemsDict)
			{
				if (current.Key == Singleton<PvpManager>.Instance.MyLobbyUserId && current.Value != null)
				{
					outPos = current.Value.gameObject.transform.localPosition;
					return true;
				}
			}
			return false;
		}

		private void ShowUITextureHeroHeadIcon(UITexture inTexture, string inHeroModelId)
		{
			if (inTexture == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(inHeroModelId))
			{
				return;
			}
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(inHeroModelId);
			if (heroMainData == null)
			{
				return;
			}
			inTexture.mainTexture = ResourceManager.Load<Texture>(heroMainData.avatar_icon, true, true, null, 0, false);
		}

		[DebuggerHidden]
		private IEnumerator DelayHideSwitchHeroInfo(float inDelaySeconds)
		{
			PvpSelectHeroView.<DelayHideSwitchHeroInfo>c__Iterator170 <DelayHideSwitchHeroInfo>c__Iterator = new PvpSelectHeroView.<DelayHideSwitchHeroInfo>c__Iterator170();
			<DelayHideSwitchHeroInfo>c__Iterator.inDelaySeconds = inDelaySeconds;
			<DelayHideSwitchHeroInfo>c__Iterator.<$>inDelaySeconds = inDelaySeconds;
			<DelayHideSwitchHeroInfo>c__Iterator.<>f__this = this;
			return <DelayHideSwitchHeroInfo>c__Iterator;
		}

		private void HideSwitchHeroInfo()
		{
			this._uiSwitchHeroInfo.gameObject.SetActive(false);
		}

		private void ClickCancelSwitchBtn(GameObject inGo)
		{
			this.HideSwitchHeroInfo();
			this.TryRefreshSwitchHeroBtnStatus();
			Singleton<PvpManager>.Instance.ReqCancelSwitchHero(this._curNewUidSwitched);
		}

		private void ClickAcceptSwitchBtn(GameObject inGo)
		{
			this.HideSwitchHeroInfo();
			Singleton<PvpManager>.Instance.RespSwitchHero(this._curNewUidSwitched, true);
		}

		private void ClickRefuseSwitchBtn(GameObject inGo)
		{
			this.HideSwitchHeroInfo();
			Singleton<PvpManager>.Instance.RespSwitchHero(this._curNewUidSwitched, false);
		}
	}
}
