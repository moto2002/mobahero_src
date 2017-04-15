using Assets.Scripts.Model;
using GUIFramework;
using MobaProtocol;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class HomePayView : BaseView<HomePayView>
	{
		public int PayPage = 1;

		public int PayState;

		private Transform FirstPage;

		private Transform SecondPage;

		private Transform Recharge1;

		private Transform Recharge2;

		private Transform BackBtn;

		private Activity_rewardMsgListener _award;

		private Activity_rewardMsgListener _award2;

		private Transform Pack1;

		private Transform Pack1PayBtn;

		private Transform Pack2;

		private Transform Pack2PayBtn;

		private Transform Pack3;

		private Transform Pack3PayBtn;

		public HomePayView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Home/HomePayView");
		}

		public override void Init()
		{
			base.Init();
			this.FirstPage = this.transform.Find("First");
			this.SecondPage = this.transform.Find("Second");
			this.Recharge1 = this.FirstPage.Find("Btn");
			this.Recharge2 = this.SecondPage.Find("Btn");
			this.BackBtn = this.transform.Find("BackBtn");
			this._award = this.FirstPage.GetComponent<Activity_rewardMsgListener>();
			this._award2 = this.SecondPage.GetComponent<Activity_rewardMsgListener>();
			this.Pack1 = this.transform.Find("Pack1");
			this.Pack1PayBtn = this.Pack1.FindChild("Btn");
			this.Pack2 = this.transform.Find("Pack2");
			this.Pack2PayBtn = this.Pack2.FindChild("Btn");
			this.Pack3 = this.transform.Find("Pack3");
			this.Pack3PayBtn = this.Pack3.FindChild("Btn");
			UIEventListener.Get(this.BackBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.CloseThisWindow);
		}

		public override void RegisterUpdateHandler()
		{
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void HandleAfterOpenView()
		{
			this.UpdatePage();
		}

		public override void HandleBeforeCloseView()
		{
			if (this.PayPage <= 2)
			{
				Singleton<MenuView>.Instance.UpdateFirstPay();
			}
		}

		public override void RefreshUI()
		{
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void UpdatePage()
		{
			this.FirstPage.gameObject.SetActive(this.PayPage == 1);
			this.SecondPage.gameObject.SetActive(this.PayPage == 2);
			this.Pack1.gameObject.SetActive(this.PayPage == 3);
			this.Pack2.gameObject.SetActive(this.PayPage == 4);
			this.Pack3.gameObject.SetActive(this.PayPage == 5);
			if (this.PayPage == 3)
			{
				UIEventListener.Get(this.Pack1PayBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClick_ShowGoodsPackage);
				return;
			}
			if (this.PayPage == 4)
			{
				UIEventListener.Get(this.Pack2PayBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClick_ShowGoodsPackage);
				return;
			}
			if (this.PayPage == 5)
			{
				UIEventListener.Get(this.Pack3PayBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClick_ShowGoodsPackage);
				return;
			}
			Transform transform = (!this.FirstPage.gameObject.activeInHierarchy) ? this.Recharge2 : this.Recharge1;
			if (this.PayState == 0)
			{
				transform.Find("Label").GetComponent<UILabel>().text = "立即充值";
				UIEventListener.Get(transform.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickToRecharge);
			}
			else if (this.PayState == 1)
			{
				transform.Find("Label").GetComponent<UILabel>().text = "领取奖励";
				UIEventListener.Get(transform.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickToGetReward);
			}
		}

		private void OnClick_ShowGoodsPackage(GameObject obj = null)
		{
			CtrlManager.OpenWindow(WindowID.PurchasePopupView, null);
			Singleton<PurchasePopupView>.Instance.onSuccess.Add(delegate
			{
				CtrlManager.CloseWindow(WindowID.HomePayView);
			});
			if (this.Pack1.gameObject.activeInHierarchy)
			{
				Singleton<PurchasePopupView>.Instance.Show(ETypicalCommodity.NewbieHeroGiftPackage, false);
			}
			else if (this.Pack2.gameObject.activeInHierarchy)
			{
				Singleton<PurchasePopupView>.Instance.Show(ETypicalCommodity.LuxuriousHeroGiftPackage, false);
			}
			else if (this.Pack3.gameObject.activeInHierarchy)
			{
				Singleton<PurchasePopupView>.Instance.Show(ETypicalCommodity.NewYearGiftPackage, false);
			}
			else
			{
				CtrlManager.CloseWindow(WindowID.PurchasePopupView);
			}
		}

		private void ClickToRecharge(GameObject go)
		{
			Singleton<ShopView>.Instance.ThroughShop = ETypicalShop.Recharge;
			CtrlManager.OpenWindow(WindowID.ShopViewNew, null);
			CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
			this.CloseThisWindow(null);
		}

		private void ClickToGetReward(GameObject go)
		{
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "领取奖励...", true, 15f);
			if (this.PayPage == 1)
			{
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetActivityAward, param, new object[]
				{
					"10101"
				});
				this._award.taskId = 10101;
				this._award.onSucceed = new Callback(this.CloseWindow);
			}
			else
			{
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetActivityAward, param, new object[]
				{
					"10102"
				});
				this._award2.taskId = 10102;
				this._award2.onSucceed = new Callback(this.CloseWindow);
			}
		}

		private void CloseThisWindow(GameObject go = null)
		{
			CtrlManager.CloseWindow(WindowID.HomePayView);
		}

		private void CloseWindow()
		{
			this.CloseThisWindow(null);
		}
	}
}
