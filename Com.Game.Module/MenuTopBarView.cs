using Assets.Scripts.GUILogic.View.BottleSystemView;
using Assets.Scripts.Model;
using Com.Game.Utils;
using GUIFramework;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class MenuTopBarView : BaseView<MenuTopBarView>
	{
		public static UILabel CoinNumber;

		public static UILabel CrystalNumber;

		public UILabel HPNumber;

		public static UILabel SpecialMoneyNumber;

		public static UILabel SpecialMoneyName;

		public static UISprite SpecialMoneyIcon;

		public static Transform ButtonCoin;

		public static Transform ButtonCoin_rule;

		public static Transform ButtonCrystal;

		public Transform Reset;

		public Transform PingSign;

		public UILabel VIPNumber;

		public Transform HPInfo;

		public UILabel NowTime;

		public UILabel BuyTime;

		public UILabel NextTime;

		public UILabel ResetAll;

		public UILabel ResetTime;

		public UILabel name_text;

		private Transform Item;

		private Transform BackBtnAnchor;

		private Transform BackBtn;

		private UILabel BackBtnLabel;

		private UILabel BottleNumber;

		private Transform ButtonBottle;

		private Transform BottleRule;

		private GameObject Fx_pvpwaittime;

		private SummonerHeadItem summonerItem;

		private Task timer;

		private CoroutineManager coroutineManager = new CoroutineManager();

		public Transform CoinDisplay;

		public Transform DiamondDisplay;

		public Transform SpecialMoneyDisplay;

		private bool needTimer;

		private UILabel _labelTime;

		private Task _animTask;

		private Transform _panelTimeTips;

		private UISprite _expBar;

		private Transform _btnSelectHero;

		private Transform titleBg;

		private Transform doubleCard;

		private TweenAlpha nameBack_ta;

		private TweenPosition nameBack_tp;

		private TweenAlpha item_ta;

		private TweenPosition item_tp;

		private TweenPosition backBtn_tp;

		private TweenAlpha title_ta;

		private GameObject coinEff;

		private GameObject diamondEff;

		private GameObject capEff;

		public MenuTopBarView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "MenuTopBarView");
		}

		public override void Init()
		{
			base.Init();
			MenuTopBarView.CoinNumber = this.transform.Find("Anchor/Coin/CoinNumber").GetComponent<UILabel>();
			MenuTopBarView.CrystalNumber = this.transform.Find("Anchor/Crystal/CrystalNumber").GetComponent<UILabel>();
			MenuTopBarView.SpecialMoneyNumber = this.transform.Find("Anchor/Raid/CoinNumber").GetComponent<UILabel>();
			MenuTopBarView.SpecialMoneyName = this.transform.Find("Anchor/Raid/Name").GetComponent<UILabel>();
			MenuTopBarView.SpecialMoneyIcon = this.transform.Find("Anchor/Raid/CoinTexture").GetComponent<UISprite>();
			this.HPNumber = this.transform.Find("Anchor/HP/HPNumber").GetComponent<UILabel>();
			MenuTopBarView.ButtonCoin = this.transform.Find("Anchor/Coin/Button");
			MenuTopBarView.ButtonCoin_rule = this.transform.Find("Anchor/Coin/Rule");
			MenuTopBarView.ButtonCrystal = this.transform.Find("Anchor/Crystal/Button");
			this.titleBg = this.transform.Find("Anchor/Sprite");
			this.doubleCard = this.transform.Find("Anchor/DoubleCard");
			this.HPInfo = this.transform.Find("Anchor/HP/HPInfo");
			this.NowTime = this.HPInfo.Find("NowTime").GetComponent<UILabel>();
			this.BuyTime = this.HPInfo.Find("BuyTime").GetComponent<UILabel>();
			this.NextTime = this.HPInfo.Find("NextTime").GetComponent<UILabel>();
			this.ResetAll = this.HPInfo.Find("ResetAll").GetComponent<UILabel>();
			this.ResetTime = this.HPInfo.Find("ResetTime").GetComponent<UILabel>();
			this.Reset = this.transform.Find("Anchor/Reset/Button");
			this.PingSign = this.transform.Find("Anchor/PingSign/Button");
			this.VIPNumber = this.transform.Find("Anchor/VIP/VIPNumber").GetComponent<UILabel>();
			this.CoinDisplay = this.transform.Find("Anchor/Coin");
			this.DiamondDisplay = this.transform.Find("Anchor/Crystal");
			this.SpecialMoneyDisplay = this.transform.Find("Anchor/Raid");
			this.name_text = this.transform.Find("UnStaticTra/NameBack/name").GetComponent<UILabel>();
			this.Item = this.transform.Find("UnStaticTra/Item");
			this.summonerItem = this.Item.Find("SummonerItem").GetComponent<SummonerHeadItem>();
			this.BackBtnAnchor = this.transform.Find("UnStaticTra/BackBtn");
			this.BackBtn = this.BackBtnAnchor.Find("backBtnSp");
			this.BackBtnLabel = this.BackBtnAnchor.Find("Name").GetComponent<UILabel>();
			this._panelTimeTips = this.transform.Find("TopAnchor");
			this._labelTime = this.transform.TryFindChild("TopAnchor/WaitTime/Time").GetComponent<UILabel>();
			this._btnSelectHero = this.transform.TryFindChild("TopAnchor/WaitTime/btn");
			this._expBar = this.transform.Find("UnStaticTra/NameBack/ExpBar").GetComponent<UISprite>();
			this.BottleNumber = this.transform.Find("Anchor/MagicBottle/BottleNumber").GetComponent<UILabel>();
			this.ButtonBottle = this.transform.Find("Anchor/MagicBottle/Button");
			this.BottleRule = this.transform.Find("Anchor/MagicBottle/Rule");
			this.Fx_pvpwaittime = this.transform.Find("TopAnchor/WaitTime/Fx_pvpwaittime").gameObject;
			this.nameBack_ta = this.transform.Find("UnStaticTra/NameBack").GetComponent<TweenAlpha>();
			this.nameBack_tp = this.transform.Find("UnStaticTra/NameBack").GetComponent<TweenPosition>();
			this.item_ta = this.Item.GetComponent<TweenAlpha>();
			this.item_tp = this.Item.GetComponent<TweenPosition>();
			this.backBtn_tp = this.BackBtn.GetComponent<TweenPosition>();
			this.title_ta = this.BackBtnLabel.GetComponent<TweenAlpha>();
			this.coinEff = this.transform.FindChild("Anchor/Coin/CoinTexture/Fx_CoinTexture_loop").gameObject;
			this.diamondEff = this.transform.FindChild("Anchor/Crystal/CrystalTexture/Fx_CrystalTexture_loop").gameObject;
			this.capEff = this.transform.FindChild("Anchor/MagicBottle/BottleTexture/Fx_bottleTexture_loop").gameObject;
			UIEventListener.Get(MenuTopBarView.ButtonCoin.gameObject).onClick = new UIEventListener.VoidDelegate(this.OpenCoinRule);
			UIEventListener.Get(MenuTopBarView.ButtonCoin_rule.gameObject).onClick = new UIEventListener.VoidDelegate(this.CloseCoinRule);
			UIEventListener.Get(MenuTopBarView.ButtonCrystal.gameObject).onClick = new UIEventListener.VoidDelegate(this.AddDiamond);
			UIEventListener.Get(this.Reset.gameObject).onClick = new UIEventListener.VoidDelegate(this.ResetBtn);
			UIEventListener.Get(this.PingSign.gameObject).onClick = new UIEventListener.VoidDelegate(this.PingSignBtn);
			UIEventListener.Get(this.Item.gameObject).onClick = new UIEventListener.VoidDelegate(this.openHUDVIP);
			UIEventListener.Get(this.BackBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnReturnWindow);
			UIEventListener.Get(this.ButtonBottle.gameObject).onClick = new UIEventListener.VoidDelegate(this.AddBottle);
			UIEventListener.Get(this.BottleRule.gameObject).onClick = new UIEventListener.VoidDelegate(this.CloseRule);
			UIEventListener.Get(this._btnSelectHero.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickSelectHero);
		}

		private void AddBottle(GameObject obj)
		{
			this.BottleRule.gameObject.SetActive(true);
		}

		private void CloseRule(GameObject go)
		{
			this.BottleRule.gameObject.SetActive(false);
		}

		private void AddCoin(GameObject objct_1 = null)
		{
			CtrlManager.OpenWindow(WindowID.PurchasePopupView, null);
			Singleton<PurchasePopupView>.Instance.Show(ETypicalCommodity.Diamond2Gold, false);
		}

		private void OpenCoinRule(GameObject obj = null)
		{
			MenuTopBarView.ButtonCoin_rule.gameObject.SetActive(true);
		}

		private void CloseCoinRule(GameObject obj = null)
		{
			MenuTopBarView.ButtonCoin_rule.gameObject.SetActive(false);
		}

		private void AddDiamond(GameObject objct_1 = null)
		{
			if (!Singleton<ShopView>.Instance.IsOpen)
			{
				Singleton<ShopView>.Instance.ThroughShop = ETypicalShop.Recharge;
				CtrlManager.OpenWindow(WindowID.ShopViewNew, null);
				CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
			}
			else
			{
				Singleton<ShopView>.Instance.SetShopType(6);
			}
		}

		private void AddHP(GameObject objct_1 = null)
		{
			CtrlManager.OpenWindow(WindowID.GetPowerView, null);
		}

		private void ResetBtn(GameObject object_1 = null)
		{
			CtrlManager.OpenWindow(WindowID.SystemSettingView, null);
		}

		private void PingSignBtn(GameObject object_1 = null)
		{
			this.ShowPingValue();
		}

		private void ShowPingValue()
		{
			this.PingSign.transform.parent.GetComponent<PingSignManage>().ChangeShowValueTra();
		}

		public override void HandleAfterOpenView()
		{
			if (!this.BackBtnAnchor.gameObject.activeInHierarchy)
			{
				this.ShowHUDView(true);
			}
		}

		public override void HandleBeforeCloseView()
		{
			this.ClearResources();
		}

		private void ClearResources()
		{
			if (this.summonerItem != null)
			{
				this.summonerItem.ClearResources();
			}
		}

		public override void RegisterUpdateHandler()
		{
			this.RefreshUI();
			this.coroutineManager.StartCoroutine(this.WaitForDoAction(0f), true);
		}

		public override void CancelUpdateHandler()
		{
		}

		public void DoOpenOtherViewAction(float delayTime = 0.25f)
		{
			if (this.coroutineManager == null)
			{
				this.coroutineManager = new CoroutineManager();
			}
			this.coroutineManager.StartCoroutine(this.WaitForDoOtherAction(delayTime), true);
		}

		[DebuggerHidden]
		public IEnumerator WaitForDoAction(float delayTime = 0f)
		{
			MenuTopBarView.<WaitForDoAction>c__Iterator140 <WaitForDoAction>c__Iterator = new MenuTopBarView.<WaitForDoAction>c__Iterator140();
			<WaitForDoAction>c__Iterator.delayTime = delayTime;
			<WaitForDoAction>c__Iterator.<$>delayTime = delayTime;
			<WaitForDoAction>c__Iterator.<>f__this = this;
			return <WaitForDoAction>c__Iterator;
		}

		[DebuggerHidden]
		public IEnumerator WaitForDoOtherAction(float delayTime = 0f)
		{
			MenuTopBarView.<WaitForDoOtherAction>c__Iterator141 <WaitForDoOtherAction>c__Iterator = new MenuTopBarView.<WaitForDoOtherAction>c__Iterator141();
			<WaitForDoOtherAction>c__Iterator.delayTime = delayTime;
			<WaitForDoOtherAction>c__Iterator.<$>delayTime = delayTime;
			<WaitForDoOtherAction>c__Iterator.<>f__this = this;
			return <WaitForDoOtherAction>c__Iterator;
		}

		public override void RefreshUI()
		{
			UserData userData = ModelManager.Instance.Get_userData_X();
			if (userData == null)
			{
				return;
			}
			MenuTopBarView.CoinNumber.text = userData.Money.ToString();
			MenuTopBarView.CrystalNumber.text = userData.Diamonds.ToString();
			this.BottleNumber.text = userData.SmallCap.ToString();
			this.VIPNumber.text = userData.VIP.ToString();
			long num = ModelManager.Instance.Get_userData_filed_X("Exp");
			if (num == 0L)
			{
				this._expBar.SetActive(false);
			}
			this._expBar.width = (int)(342f * ((float)CharacterDataMgr.instance.GetUserCurrentExp(num) / (float)CharacterDataMgr.instance.GetUserNextLevelExp(num)));
			this.UpdateHUDView();
			this.BottleRule.gameObject.SetActive(false);
		}

		private void openVIP(GameObject objct_1 = null)
		{
			CtrlManager.OpenWindow(WindowID.PassportView, null);
			CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
		}

		public void ShowCoinNUmber(int number)
		{
			MenuTopBarView.CoinNumber.text = number.ToString();
		}

		private void UpdateHUDView()
		{
			UserData userData = ModelManager.Instance.Get_userData_X();
			if (userData != null)
			{
				this.name_text.text = userData.NickName;
				if (userData.CharmRankValue <= 50)
				{
					this.name_text.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(userData.CharmRankValue);
				}
				this.summonerItem.Init(userData.Avatar.ToString(), userData.PictureFrame.ToString(), userData.Exp);
			}
		}

		public void ShowHUDView(bool isShow = true)
		{
			if (isShow)
			{
				this.TweenToSummonerInfo();
			}
		}

		public void ShowBcakBtn(bool isShow = true, string text = "")
		{
			this.ShowHUDView(!isShow);
			this.BackBtnLabel.text = text;
			if (isShow)
			{
				this.TweenToBackBtn();
			}
		}

		public void DelayShowBcakBtn(bool isShow = true, string text = "")
		{
			this.coroutineManager.StartCoroutine(this.DoDelayShowBcakBtn(isShow, text), true);
		}

		[DebuggerHidden]
		private IEnumerator DoDelayShowBcakBtn(bool isShow = true, string text = "")
		{
			MenuTopBarView.<DoDelayShowBcakBtn>c__Iterator142 <DoDelayShowBcakBtn>c__Iterator = new MenuTopBarView.<DoDelayShowBcakBtn>c__Iterator142();
			<DoDelayShowBcakBtn>c__Iterator.isShow = isShow;
			<DoDelayShowBcakBtn>c__Iterator.text = text;
			<DoDelayShowBcakBtn>c__Iterator.<$>isShow = isShow;
			<DoDelayShowBcakBtn>c__Iterator.<$>text = text;
			<DoDelayShowBcakBtn>c__Iterator.<>f__this = this;
			return <DoDelayShowBcakBtn>c__Iterator;
		}

		private void openHUDVIP(GameObject objct_1 = null)
		{
			string text = ModelManager.Instance.Get_userData_filed_X("UserId");
			CtrlManager.OpenWindow(WindowID.SummonerView, null);
			CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
		}

		public void ChangeMoneyDisplay(ShopType shopType)
		{
			bool flag = shopType != ShopType.GameShop && shopType != ShopType.HeroShop && shopType != ShopType.SkinShop;
			if (flag ^ this.SpecialMoneyDisplay.gameObject.activeInHierarchy)
			{
				this.SpecialMoneyDisplay.gameObject.SetActive(flag);
				this.CoinDisplay.gameObject.SetActive(!flag);
				this.DiamondDisplay.gameObject.SetActive(!flag);
			}
			if (flag)
			{
				switch (shopType)
				{
				case ShopType.RaidShop:
					MenuTopBarView.SpecialMoneyName.text = "我的奇袭币：";
					MenuTopBarView.SpecialMoneyNumber.text = ModelManager.Instance.Get_userData_filed_X("ArenaMoney").ToString();
					break;
				case ShopType.CardShop:
					MenuTopBarView.SpecialMoneyName.text = "纸牌屋货币：";
					MenuTopBarView.SpecialMoneyNumber.text = ModelManager.Instance.Get_userData_filed_X("TBCMoney").ToString();
					break;
				case ShopType.LadderShop:
					MenuTopBarView.SpecialMoneyName.text = "我的天梯币：";
					MenuTopBarView.SpecialMoneyNumber.text = ModelManager.Instance.Get_userData_filed_X("LadderMoney").ToString();
					break;
				case ShopType.TeamShop:
					MenuTopBarView.SpecialMoneyName.text = "战队天梯币：";
					break;
				}
			}
		}

		private void OnReturnWindow(GameObject go)
		{
			if (BottleViewCtrl.GetInstance().drawState == DrawState.Drawing)
			{
				return;
			}
			if (null != Singleton<PropertyView>.Instance.transform && Singleton<PropertyView>.Instance.gameObject.activeInHierarchy)
			{
				this.SetActiveOrNot(true);
			}
			CtrlManager.ReturnPreWindow();
		}

		public void SetWindowTitle(string title)
		{
			if (this.BackBtnLabel.text != null)
			{
				this.BackBtnLabel.text = title;
			}
		}

		public void TryShowTimeTips()
		{
			this.ClearTimeTips();
			if (this._panelTimeTips)
			{
				float waitTimeLeft = PvpMatchMgr.Instance.WaitTimeLeft;
				if (waitTimeLeft <= 0f)
				{
					return;
				}
				this._panelTimeTips.gameObject.SetActive(true);
				if (!this.Fx_pvpwaittime.activeInHierarchy)
				{
					this.Fx_pvpwaittime.SetActive(true);
				}
				if (this._animTask != null)
				{
					this._animTask.Stop();
				}
				IEnumerator c = this.AnimeText(1f, LanguageManager.Instance.GetStringById("PlayUI_EnterGameTime"));
				this._animTask = new Task(c, true);
			}
		}

		public void ClearTimeTips()
		{
			Task.Clear(ref this._animTask);
			if (this._panelTimeTips)
			{
				this._panelTimeTips.gameObject.SetActive(false);
			}
		}

		[DebuggerHidden]
		private IEnumerator AnimeText(float dur, string fmt)
		{
			MenuTopBarView.<AnimeText>c__Iterator143 <AnimeText>c__Iterator = new MenuTopBarView.<AnimeText>c__Iterator143();
			<AnimeText>c__Iterator.fmt = fmt;
			<AnimeText>c__Iterator.dur = dur;
			<AnimeText>c__Iterator.<$>fmt = fmt;
			<AnimeText>c__Iterator.<$>dur = dur;
			<AnimeText>c__Iterator.<>f__this = this;
			return <AnimeText>c__Iterator;
		}

		private void OnTimeout()
		{
			this.DoSelectHero();
		}

		private void OnClickSelectHero(GameObject go)
		{
			this.DoSelectHero();
		}

		private void DoSelectHero()
		{
			this.ClearTimeTips();
			PvpMatchMgr.Instance.ForceSelectHero();
		}

		public void SetActiveOrNot(bool isActive)
		{
			if (null == this.transform)
			{
				return;
			}
			if (isActive != this.BackBtnLabel.gameObject.activeInHierarchy)
			{
				this.BackBtnLabel.gameObject.SetActive(isActive);
				this.titleBg.gameObject.SetActive(isActive);
				this.doubleCard.gameObject.SetActive(isActive);
			}
		}

		private void TweenToBackBtn()
		{
			this.nameBack_ta.PlayReverse();
			this.nameBack_tp.PlayReverse();
			this.item_ta.PlayReverse();
			this.item_tp.PlayReverse();
			this.backBtn_tp.PlayForward();
			this.title_ta.PlayForward();
		}

		private void TweenToSummonerInfo()
		{
			this.nameBack_ta.PlayForward();
			this.nameBack_tp.PlayForward();
			this.item_ta.PlayForward();
			this.item_tp.PlayForward();
			this.backBtn_tp.PlayReverse();
			this.title_ta.PlayReverse();
		}

		public void SetPanelVisible(bool isVisible)
		{
			this.transform.GetComponent<UIPanel>().alpha = (float)((!isVisible) ? 0 : 1);
			this.coinEff.SetActive(isVisible);
			this.diamondEff.SetActive(isVisible);
			this.capEff.SetActive(isVisible);
		}

		public void NewbieReturnWindow()
		{
			this.OnReturnWindow(null);
		}
	}
}
