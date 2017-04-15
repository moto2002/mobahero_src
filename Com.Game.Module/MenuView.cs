using Assets.Scripts.GUILogic.View.PropertyView;
using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaHeros.Pvp;
using MobaProtocol;
using MobaProtocol.Data;
using Newbie;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class MenuView : BaseView<MenuView>
	{
		public static Dictionary<MobaGameCode, float> dicTimes = new Dictionary<MobaGameCode, float>();

		private Transform Task;

		private Transform EachDayTask;

		private Transform Mail;

		private Transform HeroSacrificial;

		private Transform Society;

		private Transform PlayBtn;

		private Transform MBJG;

		private Transform PHB;

		private Transform SD;

		private Transform Sign;

		private UITable PayTable;

		private Transform FirstPay;

		private Transform SecondPay;

		private Transform MengxinPackBtn;

		private Transform HaohuaPackBtn;

		private Transform NewYearPackBtn;

		private UILabel EggLabel;

		private Transform MyHeroes;

		private Transform Achievement;

		private Transform Anouncement;

		private Transform Watch;

		private Transform BBS;

		private Transform Advertise;

		private Transform BottomAnchor;

		private Transform CenterAnchor;

		private Transform Free;

		private Transform Banner;

		private Dictionary<int, List<string>> newsShowDic = new Dictionary<int, List<string>>();

		private Transform btnSprite;

		private UIGrid FreeGrid;

		private Transform Label1;

		private Transform Label2;

		private Transform Label3;

		private Transform Label4;

		private Transform LableChange;

		private Transform banner;

		private UILabel LableChangeText;

		private Transform temp;

		private Transform tempLabel;

		private Transform CSTVIcon;

		private UILabel k_d_a;

		private UILabel KDA;

		private UILabel JDDZ;

		private UILabel JDDZCount;

		private UISprite JDDZBar;

		private UILabel LZXF;

		private UILabel LZXFCount;

		private UISprite LZXFBar;

		private UILabel SDDLD;

		private UILabel SDDLDCount;

		private UISprite SDDLDBar;

		private UILabel JJC;

		private UILabel JJCCount;

		private UISprite JJCBar;

		private UILabel Record;

		private UISprite RankIcon;

		private UISprite BottleIcon;

		private GameObject BottleNumTipObj;

		private UILabel BottleNumTipLabel;

		public UIPanel mainPanel;

		private bool boolMoneyTimer;

		private bool boolStoneTimer;

		public bool HSSRBool;

		public bool SMSDBool;

		private bool showSignViewFirst;

		private Task SignTask;

		private Task RankTask;

		private CoroutineManager coroutineManager = new CoroutineManager();

		private bool isInit;

		private HomeGCManager homeGCManager;

		private readonly List<string> _freeHeroIds = new List<string>();

		private Transform[] path;

		private Transform[] targetpath;

		private float moveSpeed = 2f;

		private Task BannerMove;

		private Task TimeCheckTask;

		private Task CheckHongBaoTask;

		private UICenterOnChild centerChild;

		private HongBaoCtrl hongbao;

		private Transform minddleBg;

		private Transform _activityTip;

		private EffectDelayActive _activityTipEffect;

		private Task textFaceTask;

		private bool isOpenAgin;

		private Transform nowTarget;

		private Transform target;

		private int targetIndex = 1;

		private UIScrollView mPanelScrollView;

		private TweenPosition _point;

		private bool isCanClick = true;

		private Transform mTrans;

		private Transform mTrans1;

		private Transform mTrans2;

		private Transform mTrans3;

		protected UIPanel mPanel;

		private static readonly string[] TEXTFACE = new string[]
		{
			"万事如意",
			"新年快乐",
			"大吉大利",
			"恭喜发财",
			"合家欢乐",
			"财源滚滚",
			"身体健康",
			"新春大吉",
			"吉祥如意",
			"福星高照",
			"鸿运当头",
			"好运连连",
			"恭贺新禧",
			"年年有余",
			"招财进宝",
			"吉星高照",
			"心想事成",
			"花开富贵",
			"幸福安康",
			"十全十美",
			"健康如意",
			"好运常伴",
			"笑口常开",
			"团团圆圆",
			"福禄双喜",
			"六六大顺",
			"事遂心愿"
		};

		public MenuView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "MenuView");
		}

		public override void Init()
		{
			base.Init();
			this.mainPanel = this.gameObject.GetComponent<UIPanel>();
			this.BottomAnchor = this.transform.Find("BottomAnchor");
			this.CenterAnchor = this.transform.Find("CenterAnchor");
			this.Task = this.transform.Find("BottomAnchor/Button/Task");
			this.EachDayTask = this.transform.Find("BottomAnchor/Button/EachDayTask");
			this.Mail = this.transform.Find("BottomAnchor/Button/Mail");
			this.HeroSacrificial = this.transform.Find("BottomAnchor/Button/HeroSacrificial");
			this.Society = this.transform.Find("BottomAnchor/Button/Society");
			this.PlayBtn = this.transform.Find("BottomAnchor/Button/PlayBtn");
			this.MBJG = this.transform.Find("TopAnchor/MBJG");
			this.PHB = this.transform.Find("BottomAnchor/PHB");
			this.SD = this.transform.Find("TopAnchor/SD");
			this.Anouncement = this.transform.Find("TopAnchor/Anouncement/Sprite");
			this.Sign = this.transform.Find("TopAnchor/Sign/Icon");
			this.PayTable = this.transform.Find("TopAnchor/Pay").GetComponent<UITable>();
			this.FirstPay = this.transform.Find("TopAnchor/Pay/Normal");
			this.SecondPay = this.transform.Find("TopAnchor/Pay/FirstPaid");
			this.MengxinPackBtn = this.transform.Find("TopAnchor/Pay/X0_GoodsPack");
			this.HaohuaPackBtn = this.transform.Find("TopAnchor/Pay/X1_GoodsPack");
			this.NewYearPackBtn = this.transform.Find("TopAnchor/Pay/X2_GoodsPack");
			this.EggLabel = this.NewYearPackBtn.FindChild("Egg").GetComponent<UILabel>();
			this.MyHeroes = this.transform.Find("TopAnchor/Pay/SecondPaid");
			this.BBS = this.transform.Find("BottomAnchor/Button/BBS");
			this.Achievement = this.transform.Find("BottomAnchor/Statistics");
			this.Banner = this.transform.Find("TopAnchor/Banner");
			this.Free = this.transform.Find("BottomAnchor/Button/Free");
			this.FreeGrid = this.transform.Find("BottomAnchor/Button/Free/Grid").GetComponent<UIGrid>();
			this.k_d_a = this.Achievement.Find("k_d_a").GetComponent<UILabel>();
			this.KDA = this.Achievement.Find("KDA").GetComponent<UILabel>();
			this.JDDZ = this.Achievement.Find("JDDZ").GetComponent<UILabel>();
			this.JDDZCount = this.Achievement.Find("JDDZ/Count").GetComponent<UILabel>();
			this.JDDZBar = this.Achievement.Find("JDDZ/Sprite").GetComponent<UISprite>();
			this.LZXF = this.Achievement.Find("LZXF").GetComponent<UILabel>();
			this.LZXFCount = this.Achievement.Find("LZXF/Count").GetComponent<UILabel>();
			this.LZXFBar = this.Achievement.Find("LZXF/Sprite").GetComponent<UISprite>();
			this.SDDLD = this.Achievement.Find("SDDLD").GetComponent<UILabel>();
			this.SDDLDCount = this.Achievement.Find("SDDLD/Count").GetComponent<UILabel>();
			this.SDDLDBar = this.Achievement.Find("SDDLD/Sprite").GetComponent<UISprite>();
			this.JJC = this.Achievement.Find("JJC").GetComponent<UILabel>();
			this.JJCCount = this.Achievement.Find("JJC/Count").GetComponent<UILabel>();
			this.JJCBar = this.Achievement.Find("JJC/Sprite").GetComponent<UISprite>();
			this.Record = this.Achievement.Find("Record").GetComponent<UILabel>();
			this.Label1 = this.Banner.Find("Label1");
			this.Label2 = this.Banner.Find("Label2");
			this.Label3 = this.Banner.Find("Label3");
			this.Label4 = this.Banner.Find("Label4");
			this.LableChange = this.Banner.Find("LabelChange");
			this.LableChangeText = this.LableChange.GetComponent<UILabel>();
			this.tempLabel = this.Banner.Find("Label1");
			this.mTrans = this.Banner.Find("Scroll View/GameObject/1");
			this.mTrans1 = this.Banner.Find("Scroll View/GameObject/2");
			this.mTrans2 = this.Banner.Find("Scroll View/GameObject/3");
			this.mTrans3 = this.Banner.Find("Scroll View/GameObject/4");
			this.mPanel = this.Banner.Find("Scroll View").GetComponent<UIPanel>();
			this.centerChild = this.Banner.Find("Scroll View/GameObject").GetComponent<UICenterOnChild>();
			this.BottleIcon = this.MBJG.FindChild("Sprite").GetComponent<UISprite>();
			this.BottleNumTipObj = this.MBJG.FindChild("NumTip").gameObject;
			this.BottleNumTipLabel = this.MBJG.FindChild("NumTip/Label").GetComponent<UILabel>();
			this.RankIcon = this.PHB.FindChild("icon").GetComponent<UISprite>();
			this.CSTVIcon = this.transform.Find("BottomAnchor/Button/CSTV");
			this.hongbao = this.transform.Find("CenterAnchor/Hongbao").GetComponent<HongBaoCtrl>();
			this.minddleBg = this.transform.Find("CenterAnchor/MiddleBg");
			this._activityTip = this.Banner.FindChild("ActivityTips");
			this._activityTipEffect = this._activityTip.FindChild("Fx_ui_daytask").GetComponent<EffectDelayActive>();
			float num = (float)Screen.width / (float)Screen.height;
			if (num <= 1.33333337f)
			{
				this.minddleBg.transform.Find("pic").GetComponent<UITexture>().mainTexture = (Resources.Load("Prefab/UI/Bg/Middle-bg") as Texture);
				this.minddleBg.gameObject.SetActive(true);
			}
			else if (num < 1.57777774f)
			{
				this.minddleBg.transform.Find("pic").GetComponent<UITexture>().mainTexture = (Resources.Load("Prefab/UI/Bg/Middle-bg") as Texture);
				this.minddleBg.gameObject.SetActive(true);
			}
			else if (num < 1.72777784f)
			{
				this.minddleBg.transform.Find("pic").GetComponent<UITexture>().mainTexture = (Resources.Load("Prefab/UI/Bg/Middle-bg") as Texture);
				this.minddleBg.transform.Find("pic").GetComponent<UITexture>().uvRect = new Rect(0f, -0.45f, 1f, 1f);
				this.minddleBg.gameObject.SetActive(true);
			}
			else
			{
				this.minddleBg.gameObject.SetActive(false);
			}
			this.boolMoneyTimer = false;
			this.boolStoneTimer = false;
			UIEventListener.Get(this.MBJG.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickMBJG);
			UIEventListener.Get(this.PHB.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickPHB);
			UIEventListener.Get(this.SD.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickSD);
			UIEventListener.Get(this.Achievement.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickAchievement);
			UIEventListener.Get(this.BBS.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBBS);
			UIEventListener.Get(this.Label1.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBanner);
			UIEventListener.Get(this.Label2.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBanner);
			UIEventListener.Get(this.Label3.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBanner);
			UIEventListener.Get(this.Label4.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBanner);
			UIEventListener.Get(this.mTrans.gameObject).onPress = new UIEventListener.BoolDelegate(this.DragBanner);
			UIEventListener.Get(this.mTrans1.gameObject).onPress = new UIEventListener.BoolDelegate(this.DragBanner);
			UIEventListener.Get(this.mTrans2.gameObject).onPress = new UIEventListener.BoolDelegate(this.DragBanner);
			UIEventListener.Get(this.mTrans3.gameObject).onPress = new UIEventListener.BoolDelegate(this.DragBanner);
			UIEventListener.Get(this.CSTVIcon.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickCSTV);
			UIEventListener.Get(this.Sign.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickSign);
			UIEventListener.Get(this.FirstPay.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFirstPayBtn);
			UIEventListener.Get(this.SecondPay.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFirstPayBtn);
			UIEventListener.Get(this.MyHeroes.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFirstPayBtn);
			UIEventListener.Get(this.MengxinPackBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFirstPayBtn);
			UIEventListener.Get(this.HaohuaPackBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFirstPayBtn);
			UIEventListener.Get(this.NewYearPackBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFirstPayBtn);
			UIEventListener.Get(this.MBJG.gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
			UIEventListener.Get(this.PHB.gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
			UIEventListener.Get(this.SD.gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
			UIEventListener.Get(this.Achievement.gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
			UIEventListener.Get(this.Label1.gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowBannerPressMask);
			UIEventListener.Get(this.Label2.gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowBannerPressMask);
			UIEventListener.Get(this.Label3.gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowBannerPressMask);
			UIEventListener.Get(this.Label4.gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowBannerPressMask);
			Transform[] componentsInChildren = this.Free.FindChild("Grid").GetComponentsInChildren<Transform>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Transform transform = componentsInChildren[i];
				UIEventListener.Get(transform.gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowFreePressMask);
			}
			NotifyTimer.instance.Check();
			this.path = new Transform[]
			{
				this.Label1,
				this.Label2,
				this.Label3,
				this.Label4
			};
			this.targetpath = new Transform[]
			{
				this.mTrans,
				this.mTrans1,
				this.mTrans2,
				this.mTrans3
			};
			if (GlobalSettings.isLoginByHoolaiSDK)
			{
				UIEventListener.Get(this.BBS.gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
			}
			else
			{
				UIEventListener.Get(this.CSTVIcon.gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
				this.CSTVIcon.gameObject.SetActive(true);
				this.BBS.gameObject.SetActive(false);
				this.transform.GetComponent<UITweenHelper>().ExchangeWidget(3, this.CSTVIcon.GetComponent<UISprite>());
			}
			this.transform.GetComponent<UITweenHelper>().NextPlayDelay = 1.2f;
		}

		public override void RegisterUpdateHandler()
		{
			this.UpdateFree();
			this.UpdateRankInfo();
			this.UpdateBottleInfo();
			this.UpdateFirstPay();
			this.ShowStatistics();
			if (!this.isInit)
			{
				this.coroutineManager.StartCoroutine(this.CheckStates(), true);
				ModelManager.Instance.Initialize_SettingData();
				ModelManager.Instance.Apply_SettingData();
				this.isInit = true;
			}
			else
			{
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetShopVersion, null, new object[0]);
			}
			this.NewsManagerInit();
			this.ShowContent();
			Singleton<InvitationView>.Instance.CheackSaveData();
			CtrlManager.OpenWindow(WindowID.MenuBottomBarView, null);
			this.TimeCheckTask = this.coroutineManager.StartCoroutine(this.TimeCheck(), true);
			this.CheckHongBaoTask = this.coroutineManager.StartCoroutine(this.hongbao.CheckHongBaoVisi(), true);
		}

		[DebuggerHidden]
		private IEnumerator TimeCheck()
		{
			MenuView.<TimeCheck>c__Iterator144 <TimeCheck>c__Iterator = new MenuView.<TimeCheck>c__Iterator144();
			<TimeCheck>c__Iterator.<>f__this = this;
			return <TimeCheck>c__Iterator;
		}

		private void TimeCheckSign()
		{
			if (ModelManager.Instance.Get_SignDayRecordTime().Day != ToolsFacade.ServerCurrentTime.Day)
			{
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetSignDay, null, new object[0]);
			}
			SignState signState = ModelManager.Instance.Get_GetSignDay_X();
			if (signState != null && signState.isPass == 1)
			{
				Singleton<MenuView>.Instance.SetNews(6, "0");
			}
		}

		public void ShowContent()
		{
			this.mainPanel.alpha = 1f;
			this.CenterAnchor.gameObject.SetActive(true);
			this.BottomAnchor.gameObject.SetActive(true);
		}

		public override void CancelUpdateHandler()
		{
			for (int i = 0; i < 6; i++)
			{
				UITexture component = this.FreeGrid.transform.GetChild(i).GetComponent<UITexture>();
				component.mainTexture = null;
			}
			MobaMessageManager.UnRegistMessage(MobaChatCode.Chat_ScanPrivateNoRead, new MobaMessageFunc(this.OnGetMsg_GetFriendsMessages));
			if (this.TimeCheckTask != null)
			{
				this.TimeCheckTask.Stop();
				this.TimeCheckTask = null;
			}
			if (this.CheckHongBaoTask != null)
			{
				this.CheckHongBaoTask.Stop();
				this.CheckHongBaoTask = null;
			}
			ModelManager.Instance.Get_GetMyAchievementData_X().HaveFight = false;
			if (this._activityTipEffect.activeInHierarchy)
			{
				this._activityTipEffect.SetActive(false, 0f);
			}
			this._activityTip.gameObject.SetActive(false);
		}

		[DebuggerHidden]
		private IEnumerator CheckStates()
		{
			MenuView.<CheckStates>c__Iterator145 <CheckStates>c__Iterator = new MenuView.<CheckStates>c__Iterator145();
			<CheckStates>c__Iterator.<>f__this = this;
			return <CheckStates>c__Iterator;
		}

		public override void HandleAfterOpenView()
		{
			this.SetBottleNumInfo();
			this.BannerMove = this.coroutineManager.StartCoroutine(this.MoveOnPath(), true);
			this.transform.GetComponent<UITweenHelper>().Play();
		}

		public void UpdateBottleNum()
		{
			if (base.IsOpen)
			{
				this.SetBottleNumInfo();
			}
		}

		private void SetBottleNumInfo()
		{
			EquipmentInfoData equipmentInfoData = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId == 7777);
			if (equipmentInfoData == null || equipmentInfoData.Count == 0)
			{
				this.BottleNumTipObj.SetActive(false);
			}
			else
			{
				this.BottleNumTipObj.SetActive(true);
				this.BottleNumTipLabel.text = equipmentInfoData.Count.ToString();
			}
		}

		public void OpenSummonerRegisterView()
		{
			int num = ModelManager.Instance.Get_userData_filed_X("LoginCount");
			if (num < 1)
			{
				CtrlManager.OpenWindow(WindowID.SummonerRegisterView, null);
			}
			if (num > 0)
			{
				NewbieManager.Instance.TryProcessNewbieGuide();
			}
		}

		private void CheckFriendState()
		{
			MVC_MessageManager.AddListener_view(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg_GetFriendList));
			this.coroutineManager.StartCoroutine(this.TryFriend_GetFriendList(), true);
		}

		private void CheckHongbao()
		{
			MVC_MessageManager.AddListener_view(MobaGameCode.GetRedPacketsInfo, new MobaMessageFunc(this.OnGetMsg_GetRedPacketsInfo));
			MVC_MessageManager.AddListener_view(MobaGameCode.ClientReportOnlineTime, new MobaMessageFunc(this.OnGetMsg_ClientReportOnlineTime));
			this.coroutineManager.StartCoroutine(this.TryGetHongBao(), true);
		}

		[DebuggerHidden]
		private IEnumerator TryFriend_GetFriendList()
		{
			return new MenuView.<TryFriend_GetFriendList>c__Iterator146();
		}

		[DebuggerHidden]
		private IEnumerator TryGetHongBao()
		{
			MenuView.<TryGetHongBao>c__Iterator147 <TryGetHongBao>c__Iterator = new MenuView.<TryGetHongBao>c__Iterator147();
			<TryGetHongBao>c__Iterator.<>f__this = this;
			return <TryGetHongBao>c__Iterator;
		}

		private void OnGetMsg_ClientReportOnlineTime(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				int num = (int)operationResponse.Parameters[1];
				MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
				if (mobaErrorCode != MobaErrorCode.Ok)
				{
					if (mobaErrorCode == MobaErrorCode.LargeTimeDiff)
					{
						DateTime dateTime = new DateTime((long)operationResponse.Parameters[11]);
						SendMsgManager.Instance.SendMsg(MobaGameCode.ClientReportOnlineTime, null, new object[]
						{
							dateTime.Ticks,
							ModelManager.Instance.Get_LastBattleTime().TotalSeconds
						});
					}
				}
				else
				{
					ModelManager.Instance.Set_Time_ClearBattleTimeRecord();
					byte[] buffer = (byte[])operationResponse.Parameters[84];
					RedPacketsData data = SerializeHelper.Deserialize<RedPacketsData>(buffer);
					if (this.hongbao != null)
					{
						this.hongbao.PacketsData(data);
					}
				}
			}
			MVC_MessageManager.RemoveListener_view(MobaGameCode.ClientReportOnlineTime, new MobaMessageFunc(this.OnGetMsg_ClientReportOnlineTime));
		}

		public void OnPvpStartGame()
		{
			this.hongbao.OnPvpStartGame();
		}

		private void OnGetMsg_RichManGiftMgr(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			this.hongbao.OnMsg_RichManGiftMgr(msg);
			MVC_MessageManager.RemoveListener_view(MobaGameCode.RichManGiftMgr, new MobaMessageFunc(this.OnGetMsg_RichManGiftMgr));
		}

		private void OnGetMsg_GetRedPacketsInfo(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				int num = (int)operationResponse.Parameters[1];
				MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
				switch (mobaErrorCode)
				{
				case MobaErrorCode.NotInRedPacketTime:
				case MobaErrorCode.RedPacketHaveNothing:
				case MobaErrorCode.DontHaveRedPacket:
					goto IL_FC;
				case MobaErrorCode.LargeTimeDiff:
				{
					IL_4B:
					if (mobaErrorCode != MobaErrorCode.Ok)
					{
						goto IL_FC;
					}
					byte[] buffer = (byte[])operationResponse.Parameters[84];
					RedPacketsData redPacketsData = SerializeHelper.Deserialize<RedPacketsData>(buffer);
					if (redPacketsData.timeleft == 0 || ModelManager.Instance.Get_LastBattleTime() == TimeSpan.Zero || redPacketsData.id == 0)
					{
						this.hongbao.PacketsData(redPacketsData);
					}
					else
					{
						SendMsgManager.Instance.SendMsg(MobaGameCode.ClientReportOnlineTime, null, new object[]
						{
							Tools_TimeCheck.ServerCurrentTime.Ticks,
							(int)ModelManager.Instance.Get_LastBattleTime().TotalSeconds
						});
					}
					goto IL_117;
				}
				}
				goto IL_4B;
				IL_FC:
				this.hongbao.ZaixianPacket.gameObject.SetActive(false);
			}
			IL_117:
			MVC_MessageManager.RemoveListener_view(MobaGameCode.GetRedPacketsInfo, new MobaMessageFunc(this.OnGetMsg_GetRedPacketsInfo));
		}

		private void OnGetMsg_GetFriendList(MobaMessage msg)
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
				this.GetFriendList(num, operationResponse.DebugMessage, null);
			}
			else
			{
				byte[] buffer = operationResponse.Parameters[27] as byte[];
				List<FriendData> list = SerializeHelper.Deserialize<List<FriendData>>(buffer);
				if (list != null)
				{
					Singleton<MenuBottomBarView>.Instance.SetFriendNum(list.FindAll((FriendData obj) => (int)obj.Status == 1 && (int)obj.GameStatus != 0).Count);
					if (list.Find((FriendData obj) => (int)obj.Status == 3) != null)
					{
						this.SetNews(14, "0");
					}
				}
				this.GetFriendList(num, operationResponse.DebugMessage, list);
			}
		}

		private void GetFriendList(int arg1, string arg2, List<FriendData> arg3)
		{
			MVC_MessageManager.RemoveListener_view(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg_GetFriendList));
			MobaMessageManager.RegistMessage(MobaChatCode.Chat_ScanPrivateNoRead, new MobaMessageFunc(this.OnGetMsg_GetFriendsMessages));
			Dictionary<byte, object> args = new Dictionary<byte, object>();
			NetWorkHelper.Instance.client.SendSessionChannelMessage(5, MobaChannel.Chat, args);
		}

		private void OnGetMsg_GetFriendsMessages(MobaMessage msg)
		{
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					byte[] buffer = (byte[])operationResponse.Parameters[174];
					string[] strArr = SerializeHelper.Deserialize<string[]>(buffer);
					this.GetFriendsMessages(strArr);
				}
			}
		}

		private void GetFriendsMessages(string[] strArr)
		{
			List<FriendData> list = ModelManager.Instance.Get_FriendDataList_X();
			if (strArr != null && strArr.Length > 0)
			{
				int i;
				for (i = 0; i < strArr.Length; i++)
				{
					if (!Singleton<FriendView>.Instance.newMessageList.Contains(long.Parse(strArr[i])) && list.Find((FriendData item) => ToolsFacade.Instance.GetUserIdBySummId(item.SummId).ToString() == strArr[i]) != null)
					{
						Singleton<FriendView>.Instance.newMessageList.Add(long.Parse(strArr[i]));
						this.SetNews(14, "0");
					}
				}
			}
		}

		public override void HandleBeforeCloseView()
		{
			if (null != this.RankIcon)
			{
				this.RankIcon.GetComponent<RankIconEffectPlayerTools>().SetEffectActive(false, 0f);
			}
			this.coroutineManager.StopCoroutine(this.BannerMove);
			this.isOpenAgin = true;
		}

		public override void Destroy()
		{
			this.isInit = false;
			HomeGCManager.Instance.ClearUiTextureResource(this.minddleBg.transform.Find("pic").GetComponent<UITexture>());
			base.Destroy();
		}

		public override void RefreshUI()
		{
		}

		private void ClickSociety(GameObject objct_1 = null)
		{
		}

		private void ClickPlayBtn(GameObject objct_1 = null)
		{
			CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
		}

		private void ClickActivity(GameObject objct_1 = null)
		{
			Singleton<TipView>.Instance.ShowViewSetText("运营活动尚未开启，敬请期待", 1f);
		}

		private void ClickMBJG(GameObject objct_1 = null)
		{
			if (LevelManager.Instance.CheckSceneIsUnLock(7))
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemOpenView, null, false);
			}
		}

		public void UpdateRankInfo()
		{
			if (this.gameObject == null || !this.gameObject.activeInHierarchy)
			{
				return;
			}
			SysRankStageVo sysRankStageVo = ModelManager.Instance.Get_LadderLevel();
			this.RankIcon.spriteName = sysRankStageVo.StageImg;
			RankIconEffectPlayerTools _playerTools = this.RankIcon.GetComponent<RankIconEffectPlayerTools>();
			_playerTools.RankLevel = sysRankStageVo.RankStage;
			_playerTools.SortPanel = this.transform.GetComponent<UIPanel>();
			_playerTools.SortWidget = this.RankIcon;
			if (null != this.RankIcon)
			{
				this.transform.GetComponent<UITweenHelper>().AddTweenFinishCallback(5, delegate
				{
					_playerTools.SetEffectActive(true, 0.25f);
				});
			}
			UILabel component = this.PHB.FindChild("LevelName").GetComponent<UILabel>();
			component.text = LanguageManager.Instance.GetStringById(sysRankStageVo.StageName);
			component.gradientTop = ModelManager.Instance.Get_ColorByString_X(sysRankStageVo.GradientTop);
			component.gradientBottom = ModelManager.Instance.Get_ColorByString_X(sysRankStageVo.GradientBottom);
			this.PHB.FindChild("ScoreLabel").GetComponent<UILabel>().text = ModelManager.Instance.Get_userData_X().LadderScore.ToString("F0");
		}

		private void UpdateBottleInfo()
		{
			int curlevel = ModelManager.Instance.Get_BottleData().curlevel;
			string[] array = LanguageManager.Instance.GetStringById("MainUI_MagicBottle_Dec").Split(new char[]
			{
				'|'
			});
			this.MBJG.Find("Level").GetComponent<UILabel>().text = curlevel.ToString();
			Dictionary<string, SysMagicbottleLevelVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysMagicbottleLevelVo>();
			int num = Tools_ParsePrice.MaxLevelCheck<SysMagicbottleLevelVo>(typeDicByType, false);
			int num2;
			if (curlevel >= num)
			{
				SysMagicbottleLevelVo dataById = BaseDataMgr.instance.GetDataById<SysMagicbottleLevelVo>(num.ToString());
				int lastLevelDefference = dataById.lastLevelDefference;
				num2 = ((curlevel - num) / lastLevelDefference + 1) * lastLevelDefference + num;
			}
			else
			{
				num2 = Tools_ParsePrice.ParseCollectRange(curlevel, true);
			}
			if (array.Length == 2)
			{
				this.MBJG.FindChild("tipLabel1").GetComponent<UILabel>().text = array[0].Replace("*", num2.ToString());
				this.MBJG.FindChild("tipLabel2").GetComponent<UILabel>().text = array[1];
			}
			Dictionary<string, SysMagicbottleExpVo> typeDicByType2 = BaseDataMgr.instance.GetTypeDicByType<SysMagicbottleExpVo>();
			int num3 = Tools_ParsePrice.Level_Check<SysMagicbottleExpVo>(curlevel, typeDicByType2, false);
			SysMagicbottleExpVo dataById2 = BaseDataMgr.instance.GetDataById<SysMagicbottleExpVo>(num3.ToString());
			this.BottleIcon.spriteName = dataById2.largeIcon;
			UISprite component = this.MBJG.FindChild("light/shine").GetComponent<UISprite>();
			if (num2 - curlevel <= 2)
			{
				this.BottleIcon.GetComponent<TweenRotation>().enabled = true;
				this.BottleNumTipObj.GetComponent<TweenScale>().enabled = false;
				if (component != null)
				{
					if (dataById2.largeIcon.Equals("Magic_bottle_06"))
					{
						component.spriteName = "Light_default";
					}
					else if (dataById2.largeIcon.Equals("Magic_bottle_08"))
					{
						component.spriteName = "Light_purple";
					}
					else if (dataById2.largeIcon.Equals("Magic_bottle_12"))
					{
						component.spriteName = "Light_red";
					}
					component.gameObject.SetActive(true);
				}
			}
			else
			{
				this.BottleIcon.GetComponent<TweenRotation>().enabled = false;
				this.BottleNumTipObj.GetComponent<TweenScale>().enabled = true;
				if (component != null && component.gameObject.activeInHierarchy)
				{
					component.gameObject.SetActive(false);
				}
			}
		}

		public void UpdateFirstPay()
		{
			ActivityTaskData activityTaskData = ModelManager.Instance.Get_Activity_taskData(10101);
			ActivityTaskData activityTaskData2 = ModelManager.Instance.Get_Activity_taskData(10102);
			if (activityTaskData != null)
			{
				if (activityTaskData.taskstate == 0 || activityTaskData.taskstate == 1)
				{
					this.FirstPay.gameObject.SetActive(true);
					this.SecondPay.gameObject.SetActive(false);
					this.MyHeroes.gameObject.SetActive(false);
					Singleton<HomePayView>.Instance.PayState = activityTaskData.taskstate;
				}
				else if (activityTaskData2 != null)
				{
					if (activityTaskData2.taskstate == 0 || activityTaskData2.taskstate == 1)
					{
						this.FirstPay.gameObject.SetActive(false);
						this.SecondPay.gameObject.SetActive(true);
						this.MyHeroes.gameObject.SetActive(false);
						Singleton<HomePayView>.Instance.PayState = activityTaskData2.taskstate;
					}
					else
					{
						this.FirstPay.gameObject.SetActive(false);
						this.SecondPay.gameObject.SetActive(false);
						this.MyHeroes.gameObject.SetActive(true);
						this.UpdateHeroNum();
					}
				}
				else
				{
					this.FirstPay.gameObject.SetActive(true);
					this.SecondPay.gameObject.SetActive(false);
					this.MyHeroes.gameObject.SetActive(false);
				}
			}
			else
			{
				this.FirstPay.gameObject.SetActive(true);
				this.SecondPay.gameObject.SetActive(false);
				this.MyHeroes.gameObject.SetActive(false);
			}
			this.UpdatePayTable();
		}

		public void UpdatePayTable()
		{
			if (!Singleton<MenuView>.Instance.IsOpen)
			{
				return;
			}
			List<string> owenHeros = CharacterDataMgr.instance.OwenHeros;
			bool flag = owenHeros.Contains("Baoliezuolun") && owenHeros.Contains("Guanyu") && owenHeros.Contains("Bingnv") && owenHeros.Contains("Timo");
			bool flag2 = owenHeros.Contains("Jiuwei") && owenHeros.Contains("Jiansheng") && owenHeros.Contains("Tunvlang");
			List<EquipmentInfoData> list = ModelManager.Instance.Get_equipmentList_X();
			bool arg_1AD_0;
			if (ToolsFacade.Instance.IsInNewYearPackageTime(ToolsFacade.ServerCurrentTime))
			{
				if (list.Exists((EquipmentInfoData obj1) => obj1.ModelId == 9152017))
				{
					if (list.Exists((EquipmentInfoData obj2) => obj2.ModelId == 9252017))
					{
						if (list.Exists((EquipmentInfoData obj3) => obj3.ModelId == 9352017))
						{
							if (list.Exists((EquipmentInfoData obj3) => obj3.ModelId == 9452017))
							{
								if (list.Exists((EquipmentInfoData obj3) => obj3.ModelId == 9552017) && ModelManager.Instance.IsPossessSkin(101203) && ModelManager.Instance.IsPossessSkin(103901))
								{
									arg_1AD_0 = ModelManager.Instance.IsPossessSkin(104101);
									goto IL_1AA;
								}
							}
						}
					}
				}
				arg_1AD_0 = false;
				IL_1AA:;
			}
			else
			{
				arg_1AD_0 = true;
			}
			bool flag3 = arg_1AD_0;
			this.MengxinPackBtn.gameObject.SetActive(!flag);
			this.HaohuaPackBtn.gameObject.SetActive(!flag2);
			this.NewYearPackBtn.gameObject.SetActive(!flag3);
			if (this.textFaceTask != null)
			{
				this.coroutineManager.StopCoroutine(this.textFaceTask);
			}
			this.textFaceTask = ((!flag3) ? this.coroutineManager.StartCoroutine(this.TextFace(), true) : null);
			this.PayTable.Reposition();
		}

		private void UpdateHeroNum()
		{
			int count = CharacterDataMgr.instance.OwenHeros.Count;
			int count2 = CharacterDataMgr.instance.AllHeros.Count;
			this.MyHeroes.Find("OwnHeroseCount").GetComponent<UILabel>().text = count.ToString();
			this.MyHeroes.Find("AllHeroseCount").GetComponent<UILabel>().text = "/" + count2.ToString();
		}

		private void ClickPHB(GameObject object_1 = null)
		{
			if (LevelManager.Instance.CheckSceneIsUnLock(17))
			{
				this.HideNews(8);
				CtrlManager.OpenWindow(WindowID.RankView, null);
				CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
			}
		}

		private void ClickSD(GameObject object_1 = null)
		{
			if (LevelManager.Instance.CheckSceneIsUnLock(6))
			{
				this.HideNews(9);
				CtrlManager.OpenWindow(WindowID.ShopViewNew, null);
				CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
				Singleton<MenuTopBarView>.Instance.DoOpenOtherViewAction(0.25f);
			}
		}

		private void ClickAchievement(GameObject go)
		{
			CtrlManager.OpenWindow(WindowID.AchievementView, null);
			CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
		}

		private void ClickBBS(GameObject object_1 = null)
		{
			UniWebViewFacade.Instance.OpenUrl("http://tieba.baidu.com/f?kw=魔霸英雄&ie=utf-8&tp=0");
		}

		private void ClickAdvertise(GameObject object_1 = null)
		{
			UniWebViewFacade.Instance.OpenUrl("http://tieba.baidu.com/f?kw=魔霸英雄&ie=utf-8&tp=0");
		}

		private void ClickCSTV(GameObject object_1 = null)
		{
			CtrlManager.OpenWindow(WindowID.GameVideoView, null);
			CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
		}

		private void ClickSign(GameObject object_1 = null)
		{
			CtrlManager.OpenWindow(WindowID.SignView, null);
		}

		private void ClickFirstPayBtn(GameObject go)
		{
			if (go == this.MyHeroes.gameObject)
			{
				this.HideNews(3);
				MobaMessageManagerTools.SendClientMsg(ClientC2V.OpenSacrificial, null, false);
			}
			else
			{
				if (go == this.FirstPay.gameObject)
				{
					Singleton<HomePayView>.Instance.PayPage = 1;
				}
				else if (go == this.SecondPay.gameObject)
				{
					Singleton<HomePayView>.Instance.PayPage = 2;
				}
				else if (go == this.MengxinPackBtn.gameObject)
				{
					Singleton<HomePayView>.Instance.PayPage = 3;
				}
				else if (go == this.HaohuaPackBtn.gameObject)
				{
					Singleton<HomePayView>.Instance.PayPage = 4;
				}
				else if (go == this.NewYearPackBtn.gameObject)
				{
					Singleton<HomePayView>.Instance.PayPage = 5;
				}
				CtrlManager.OpenWindow(WindowID.HomePayView, null);
			}
		}

		private void NewsManagerInit()
		{
			this.ShowNews();
		}

		public void SetDelayCheck(MenuViewNewsType newsType, int timeValue, int id = 0)
		{
			if (id == 0)
			{
				if (this.boolMoneyTimer)
				{
					return;
				}
				this.boolMoneyTimer = true;
			}
			else if (id == 1)
			{
				if (this.boolStoneTimer)
				{
					return;
				}
				this.boolStoneTimer = true;
			}
			this.coroutineManager.StartCoroutine(this.DelayCheck(newsType, timeValue, id), true);
		}

		[DebuggerHidden]
		private IEnumerator DelayCheck(MenuViewNewsType newsType, int timeValue, int id = 0)
		{
			MenuView.<DelayCheck>c__Iterator148 <DelayCheck>c__Iterator = new MenuView.<DelayCheck>c__Iterator148();
			<DelayCheck>c__Iterator.timeValue = timeValue;
			<DelayCheck>c__Iterator.id = id;
			<DelayCheck>c__Iterator.<$>timeValue = timeValue;
			<DelayCheck>c__Iterator.<$>id = id;
			<DelayCheck>c__Iterator.<>f__this = this;
			return <DelayCheck>c__Iterator;
		}

		public void SetNews(int newType, string id)
		{
			if (!this.newsShowDic.ContainsKey(newType))
			{
				this.newsShowDic.Add(newType, new List<string>());
				this.newsShowDic[newType].Add(id);
				if (Singleton<MenuView>.Instance.gameObject != null)
				{
					this.ChangeShow(newType, true);
				}
			}
			else
			{
				if (!this.newsShowDic[newType].Contains(id))
				{
					this.newsShowDic[newType].Add(id);
				}
				if (this.newsShowDic[newType].Count == 1 && Singleton<MenuView>.Instance.gameObject != null)
				{
					this.ChangeShow(newType, true);
				}
			}
		}

		public void CheckHeroState()
		{
			List<string> allCanCallHeros = CharacterDataMgr.instance.AllCanCallHeros;
			if (allCanCallHeros != null && allCanCallHeros.Count > 0)
			{
				this.ChangeShow(3, true);
				return;
			}
			this.ChangeShow(3, false);
		}

		public void UpdateFriendNew()
		{
			bool flag = false;
			List<FriendData> list = ModelManager.Instance.Get_applyList_X();
			if (Singleton<FriendView>.Instance.newMessageList != null && Singleton<FriendView>.Instance.newMessageList.Count > 0)
			{
				flag = true;
			}
			if (list != null && list.Count > 0)
			{
				flag = true;
			}
			if (!flag)
			{
				this.RemoveNews(14, "0");
			}
		}

		public void CheckMailState()
		{
			if (!this.isInit)
			{
				this.RegisterUpdateHandler();
			}
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetMailList, null, new object[0]);
		}

		public void HideNews(int newType)
		{
			if (this.newsShowDic.ContainsKey(newType) && this.newsShowDic[newType].Count == 0)
			{
				this.newsShowDic.Remove(newType);
				this.ChangeShow(newType, false);
			}
		}

		public void RemoveNews(int newType, string id)
		{
			if (this.newsShowDic.ContainsKey(newType))
			{
				if (this.newsShowDic[newType].Contains(id))
				{
					this.newsShowDic[newType].Remove(id);
					this.HideNews(newType);
				}
			}
			else
			{
				this.HideNews(newType);
			}
		}

		private void ShowNews()
		{
			foreach (int current in this.newsShowDic.Keys)
			{
				if (this.newsShowDic[current].Count > 0)
				{
					this.ChangeShow(current, true);
				}
			}
		}

		private void ChangeShow(int newType, bool isShow = true)
		{
			if (Singleton<MenuView>.Instance.gameObject == null)
			{
				return;
			}
			if (newType != 6)
			{
				if (newType == 14)
				{
					Singleton<MenuBottomBarView>.Instance.SetFriendNew(isShow);
				}
			}
			else
			{
				this.SetSignNew(isShow);
			}
		}

		public void SetSignNew(bool isShow)
		{
			this.Sign.parent.Find("Message").gameObject.SetActive(isShow);
		}

		public void UpdateFreeActive()
		{
			if (this.FreeGrid == null)
			{
				return;
			}
			this.FreeGrid.gameObject.SetActive(false);
			this.FreeGrid.gameObject.SetActive(true);
		}

		public void UpdateTuhaoHongBao()
		{
			this.hongbao.TuhaoCount++;
			this.hongbao.UpdateTuhaoHongBao();
		}

		private void UpdateFree()
		{
			this._freeHeroIds.AddRange(Singleton<PvpManager>.Instance.freeHeros);
			int num = 0;
			int num2 = 0;
			while (num < this._freeHeroIds.Count && num2 < 6)
			{
				if (!(this._freeHeroIds[num].Split(new char[]
				{
					','
				})[2] != "1"))
				{
					UITexture component = this.FreeGrid.transform.GetChild(num2).GetComponent<UITexture>();
					string text = this._freeHeroIds[num].Split(new char[]
					{
						','
					})[0];
					component.transform.name = text;
					UIEventListener expr_9D = UIEventListener.Get(component.gameObject);
					expr_9D.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_9D.onClick, new UIEventListener.VoidDelegate(this.OpenPropView));
					SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(text.Trim());
					if (heroMainData == null)
					{
						UnityEngine.Debug.LogError("限免英雄有问题：" + text + "在配置表中为空");
					}
					else
					{
						component.mainTexture = ResourceManager.Load<Texture>(heroMainData.avatar_icon, true, true, null, 0, false);
					}
					num2++;
				}
				num++;
			}
		}

		private void OpenPropView(GameObject obj)
		{
			if (null != obj)
			{
				string name = obj.name;
				CtrlManager.OpenWindow(WindowID.PropertyView, null);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.sacriviewChangeHero, name, false);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewInitToggle, PropertyType.Info, false);
			}
		}

		private void ShowStatistics()
		{
			HomeKDAData myHomeKDA = ModelManager.Instance.Get_GetMyAchievementData_X().myHomeKDA;
			if (myHomeKDA == null || myHomeKDA.battleinfos == null)
			{
				return;
			}
			this.Record.text = string.Concat(new string[]
			{
				"[36fe00]",
				myHomeKDA.wincount.ToString(),
				"[-]/",
				(myHomeKDA.wincount + myHomeKDA.losecount).ToString(),
				"场"
			});
			for (int i = 0; i < myHomeKDA.battleinfos.Length; i++)
			{
				int wincount = myHomeKDA.battleinfos[i].wincount;
				double num = (double)myHomeKDA.battleinfos[i].wincount + (double)myHomeKDA.battleinfos[i].losecount;
				if (myHomeKDA.battleinfos[i].battleid == "800055")
				{
					if (num != 0.0)
					{
						this.JDDZBar.gameObject.SetActive(true);
						this.JDDZBar.width = (int)((double)wincount / num * 208.0);
						this.JDDZ.text = wincount.ToString();
						this.JDDZCount.text = "/" + num.ToString() + "场";
					}
				}
				else if (myHomeKDA.battleinfos[i].battleid == "80001")
				{
					if (num != 0.0)
					{
						this.LZXFBar.gameObject.SetActive(true);
						this.LZXFBar.width = (int)((double)wincount / num * 208.0);
						this.LZXF.text = wincount.ToString();
						this.LZXFCount.text = "/" + num.ToString() + "场";
					}
				}
				else if (myHomeKDA.battleinfos[i].battleid == "80006")
				{
					if (num != 0.0)
					{
						this.SDDLDBar.gameObject.SetActive(true);
						this.SDDLDBar.width = (int)((double)wincount / num * 208.0);
						this.SDDLD.text = wincount.ToString();
						this.SDDLDCount.text = "/" + num.ToString() + "场";
					}
				}
				else if (myHomeKDA.battleinfos[i].battleid == "80003" && num != 0.0)
				{
					this.JJCBar.gameObject.SetActive(true);
					this.JJCBar.width = (int)((double)wincount / num * 208.0);
					this.JJC.text = wincount.ToString();
					this.JJCCount.text = "/" + num.ToString() + "场";
				}
			}
			double num2 = (double)(myHomeKDA.wincount + myHomeKDA.losecount);
			if (num2 == 0.0)
			{
				num2 = 1.0;
			}
			double num3 = (double)myHomeKDA.killcount / num2;
			double num4 = (double)myHomeKDA.deathcount / num2;
			double num5 = (double)myHomeKDA.assistantcount / num2;
			this.k_d_a.text = string.Concat(new string[]
			{
				num3.ToString("F1"),
				"/",
				num4.ToString("F1"),
				"/",
				num5.ToString("F1")
			});
			double num6;
			if (num4 == 0.0)
			{
				num6 = (num3 + num5) / 1.0 * 3.0;
			}
			else
			{
				num6 = (num3 + num5) / num4 * 3.0;
			}
			this.KDA.text = num6.ToString("F1");
		}

		private void ClickBanner(GameObject obj = null)
		{
			if (this.mPanelScrollView == null)
			{
				this.mPanelScrollView = this.mPanel.GetComponent<UIScrollView>();
			}
			if (this.BannerMove != null)
			{
				this.coroutineManager.StopCoroutine(this.BannerMove);
			}
			string name = obj.name;
			switch (name)
			{
			case "Label1":
				this.nowTarget = this.Label1;
				this.centerChild.CenterOn(this.mTrans);
				break;
			case "Label2":
				this.nowTarget = this.Label2;
				this.centerChild.CenterOn(this.mTrans1);
				break;
			case "Label3":
				this.nowTarget = this.Label3;
				this.centerChild.CenterOn(this.mTrans2);
				break;
			case "Label4":
				this.nowTarget = this.Label4;
				this.centerChild.CenterOn(this.mTrans3);
				break;
			}
			this.MoveToTarget(this.nowTarget);
			this.BannerMove = this.coroutineManager.StartCoroutine(this.MoveOnPath(), true);
		}

		private void DragBanner(GameObject obj, bool bol)
		{
			if (this.BannerMove != null)
			{
				this.coroutineManager.StopCoroutine(this.BannerMove);
			}
			this.isOpenAgin = true;
			if (!bol)
			{
				string name = this.centerChild.centeredObject.name;
				switch (name)
				{
				case "1":
					this.nowTarget = this.Label1;
					this.MoveToTarget(this.Label1);
					break;
				case "2":
					this.nowTarget = this.Label2;
					this.MoveToTarget(this.Label2);
					break;
				case "3":
					this.nowTarget = this.Label3;
					this.MoveToTarget(this.Label3);
					break;
				case "4":
					this.nowTarget = this.Label4;
					this.MoveToTarget(this.Label4);
					break;
				}
			}
			this.isCanClick = false;
			this.BannerMove = this.coroutineManager.StartCoroutine(this.MoveOnPath(), true);
		}

		[DebuggerHidden]
		private IEnumerator MoveOnPath()
		{
			MenuView.<MoveOnPath>c__Iterator149 <MoveOnPath>c__Iterator = new MenuView.<MoveOnPath>c__Iterator149();
			<MoveOnPath>c__Iterator.<>f__this = this;
			return <MoveOnPath>c__Iterator;
		}

		private void MoveToTarget(Transform target)
		{
			TweenPosition component = this.LableChange.GetComponent<TweenPosition>();
			component.from = this.LableChange.transform.localPosition;
			component.to = target.transform.localPosition;
			this.LableChangeText.text = target.GetComponent<UILabel>().text;
			target.GetComponent<UILabel>().color = new Color(255f, 255f, 255f, 255f);
			if (this.temp != null)
			{
				this.temp.gameObject.SetActive(false);
			}
			if (target != this.tempLabel)
			{
				this.tempLabel.GetComponent<UILabel>().color = new Color(0f, 255f, 234f, 255f);
			}
			else
			{
				target.GetComponent<UILabel>().color = new Color(255f, 255f, 255f, 255f);
			}
			component.ResetToBeginning();
			component.PlayForward();
			this.temp = this.banner;
			this.tempLabel = target;
		}

		[DebuggerHidden]
		private IEnumerator PanelOnDragFinished()
		{
			MenuView.<PanelOnDragFinished>c__Iterator14A <PanelOnDragFinished>c__Iterator14A = new MenuView.<PanelOnDragFinished>c__Iterator14A();
			<PanelOnDragFinished>c__Iterator14A.<>f__this = this;
			return <PanelOnDragFinished>c__Iterator14A;
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

		private void ShowFreePressMask(GameObject obj, bool isPress)
		{
			if (obj == null)
			{
				return;
			}
			this.Free.FindChild("PressMask").gameObject.SetActive(isPress);
		}

		private void ShowBannerPressMask(GameObject obj, bool isPress)
		{
			if (obj == null)
			{
				return;
			}
			this.Banner.FindChild("PressMask").gameObject.SetActive(isPress);
		}

		[DebuggerHidden]
		private IEnumerator TextFace()
		{
			MenuView.<TextFace>c__Iterator14B <TextFace>c__Iterator14B = new MenuView.<TextFace>c__Iterator14B();
			<TextFace>c__Iterator14B.<>f__this = this;
			return <TextFace>c__Iterator14B;
		}

		private string GetTextFaceRandomly()
		{
			int num = UnityEngine.Random.Range(0, MenuView.TEXTFACE.Length);
			return MenuView.TEXTFACE[num];
		}

		public void CheckActivityState()
		{
			if (ModelManager.Instance.Get_Activity_HasRewards())
			{
				this._activityTip.gameObject.SetActive(true);
				this._activityTipEffect.transform.localPosition = this._activityTip.FindChild("Sprite").localPosition;
				this._activityTipEffect.SetActive(true, 0f);
				this._activityTip.GetComponent<UILabel>().text = "您有未领取的活动奖励哟~";
			}
			else
			{
				this._activityTipEffect.SetActive(false, 0f);
				this._activityTip.gameObject.SetActive(false);
			}
		}
	}
}
