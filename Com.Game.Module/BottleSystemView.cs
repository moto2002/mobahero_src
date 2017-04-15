using Assets.Scripts.GUILogic.View.BottleSystemView;
using GUIFramework;
using Newbie;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class BottleSystemView : BaseView<BottleSystemView>
	{
		private BottleViewBottomRight bottomRight;

		private BottleViewBottomLeft bottomLeft;

		private BottleViewRightTop topRight;

		private Transform anchor;

		private Transform FxEffects;

		private Transform BookTip;

		public BottleSystemView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Bottle/BottleSystemView");
			this.WindowTitle = "小魔瓶";
		}

		public override void Init()
		{
			this.FxEffects = this.transform.Find("FxEffects");
			this.anchor = this.transform.Find("Anchor");
			this.bottomLeft = this.anchor.Find("RightPanel/BottomLeft").gameObject.AddComponent<BottleViewBottomLeft>();
			this.bottomRight = this.anchor.Find("RightPanel/BottomRight").gameObject.AddComponent<BottleViewBottomRight>();
			this.topRight = this.anchor.Find("RightPanel/TopRight").gameObject.AddComponent<BottleViewRightTop>();
			this.BookTip = this.anchor.Find("LeftPanel/Tip");
			UIEventListener.Get(this.BookTip.gameObject).onClick = new UIEventListener.VoidDelegate(this.OpenBottleBook);
		}

		public override void RegisterUpdateHandler()
		{
			CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
			Singleton<MenuTopBarView>.Instance.DoOpenOtherViewAction(0.25f);
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void HandleAfterOpenView()
		{
			Singleton<MenuTopBarView>.Instance.SetActiveOrNot(true);
			this.FxEffects.gameObject.SetActive(true);
			base.HandleAfterOpenView();
			NewbieManager.Instance.TryHandleOnOpenBottleSystem();
		}

		public override void HandleBeforeCloseView()
		{
			this.FxEffects.gameObject.SetActive(false);
		}

		public override void RefreshUI()
		{
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		public void NewbieEleHallActMagicUseExp(bool isPress)
		{
			if (this.topRight != null)
			{
				this.topRight.NewbieMagicBottleUseExp(isPress);
			}
		}

		public void NewbieEleHallActMagicTaleAwd()
		{
			if (this.bottomRight != null)
			{
				this.bottomRight.NewbieGetMagicBottleTaleAwd();
			}
		}

		public bool NewbieEleHallActIsOwnLegnedBottle()
		{
			return this.bottomRight != null && this.bottomRight.NewbieIsHaveLengendBottle();
		}

		private void OpenBottleBook(GameObject go)
		{
			MobaMessageManagerTools.BeginWaiting_manual("WaittingForBottleBook", "正在解析数据...", true, 15f, true);
			CtrlManager.OpenWindow(WindowID.BottleBookView, null);
		}
	}
}
