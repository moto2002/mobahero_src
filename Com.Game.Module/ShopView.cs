using Assets.Scripts.Model;
using GUIFramework;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class ShopView : BaseView<ShopView>
	{
		private GameObject mToggleCache;

		private UIGrid mToggleGrid;

		private GameObject mItemCache;

		private UIGrid mGoodsGrid;

		private UICenterOnChild mGoodsCenterHelper;

		private UIPanel mGoodsPanel;

		private UIScrollBar mScrollBar;

		private int throughShop;

		public int ThroughShopItem;

		private CoroutineManager cMgr = new CoroutineManager();

		private Task _refreshShop;

		private Task _panelStatic;

		private ShopToggleController mCurShop;

		private List<GoodsData> mGoodsList = new List<GoodsData>();

		public ETypicalShop ThroughShop
		{
			set
			{
				this.throughShop = (int)value;
			}
		}

		public ShopView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Shop/ShopView");
			this.WinId = WindowID.ShopViewNew;
			this.WindowTitle = LanguageManager.Instance.GetStringById("ShopUI_Title_Shop");
		}

		public override void Init()
		{
			this.WaitingView_open();
			this.mToggleCache = this.transform.Find("LeftAnchor/Panel/Grid/ToggleCache").gameObject;
			this.mToggleGrid = this.transform.Find("LeftAnchor/Panel/Grid").GetComponent<UIGrid>();
			this.mGoodsGrid = this.transform.Find("RightAnchor/ScrollView/Grid").GetComponent<UIGrid>();
			this.mGoodsPanel = this.transform.Find("RightAnchor/ScrollView").GetComponent<UIPanel>();
			this.mGoodsCenterHelper = this.mGoodsGrid.GetComponent<UICenterOnChild>();
			this.mItemCache = (Resources.Load("Prefab/UI/Shop/ShopItem") as GameObject);
			this.mScrollBar = this.transform.Find("RightAnchor/ProgBar").GetComponent<UIScrollBar>();
		}

		public override void HandleAfterOpenView()
		{
			base.HandleAfterOpenView();
			Singleton<MenuTopBarView>.Instance.SetActiveOrNot(true);
			this.mGoodsList = ModelManager.Instance.Get_ShopGoodsList();
			this.mGoodsGrid.GetComponentInParent<UIPanel>().alpha = 0.01f;
			this.mScrollBar.value = 0f;
			this.cMgr.StartCoroutine(this.InitializeShopToggles(), true);
		}

		public override void HandleBeforeCloseView()
		{
			this.SetAllToggleColliderEnable(false);
			this.ClearResources();
			this.cMgr.StopAllCoroutine();
		}

		private void ClearResources()
		{
			this.mGoodsList = null;
			if (this.mGoodsGrid != null)
			{
				Transform transform = this.mGoodsGrid.transform;
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform child = transform.GetChild(i);
					ShopItemController component = child.gameObject.GetComponent<ShopItemController>();
					if (component != null)
					{
						component.ClearResources();
					}
				}
			}
		}

		public override void RegisterUpdateHandler()
		{
			base.RegisterUpdateHandler();
		}

		public override void CancelUpdateHandler()
		{
			base.CancelUpdateHandler();
		}

		[DebuggerHidden]
		private IEnumerator InitializeShopToggles()
		{
			ShopView.<InitializeShopToggles>c__Iterator17B <InitializeShopToggles>c__Iterator17B = new ShopView.<InitializeShopToggles>c__Iterator17B();
			<InitializeShopToggles>c__Iterator17B.<>f__this = this;
			return <InitializeShopToggles>c__Iterator17B;
		}

		private void SetAllToggleColliderEnable(bool isEnable)
		{
			ShopToggleController[] componentsInChildren = this.mToggleGrid.GetComponentsInChildren<ShopToggleController>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ShopToggleController shopToggleController = componentsInChildren[i];
				shopToggleController.SetColliderEnable(isEnable);
			}
		}

		private int GoodsList_SortByWeight(GoodsData a, GoodsData b)
		{
			if (a == null || b == null)
			{
				return 0;
			}
			int num = b.WeightFactor.CompareTo(a.WeightFactor);
			if (num == 0 && a.Id != b.Id)
			{
				return a.Id.CompareTo(b.Id);
			}
			return num;
		}

		[DebuggerHidden]
		private IEnumerator SetGoodsList(List<GoodsData> _list)
		{
			ShopView.<SetGoodsList>c__Iterator17C <SetGoodsList>c__Iterator17C = new ShopView.<SetGoodsList>c__Iterator17C();
			<SetGoodsList>c__Iterator17C._list = _list;
			<SetGoodsList>c__Iterator17C.<$>_list = _list;
			<SetGoodsList>c__Iterator17C.<>f__this = this;
			return <SetGoodsList>c__Iterator17C;
		}

		private void SetShopType(ShopDataNew _shopData)
		{
			this.WaitingView_open();
			if (this._refreshShop != null)
			{
				this.cMgr.StopCoroutine(this._refreshShop);
			}
			ShopToggleController[] componentsInChildren = this.mToggleGrid.GetComponentsInChildren<ShopToggleController>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ShopToggleController shopToggleController = componentsInChildren[i];
				if (shopToggleController.data.Id != _shopData.Id)
				{
					shopToggleController.transform.GetComponent<UIToggle>().value = false;
				}
				else
				{
					shopToggleController.transform.GetComponent<UIToggle>().value = true;
					this.mCurShop = shopToggleController;
				}
			}
			List<GoodsData> list = new List<GoodsData>();
			string[] array = _shopData.GoodsId.Split(new char[]
			{
				','
			});
			string[] array2 = array;
			string str;
			for (int j = 0; j < array2.Length; j++)
			{
				str = array2[j];
				GoodsData goodsData = this.mGoodsList.Find((GoodsData obj) => obj.Id.ToString() == str);
				if (goodsData != null)
				{
					list.Add(goodsData);
				}
			}
			this._refreshShop = this.cMgr.StartCoroutine(this.SetGoodsList(list), true);
		}

		public void SetShopType(int _shopType)
		{
			List<ShopDataNew> list = ModelManager.Instance.Get_ShopList();
			if (list == null)
			{
				return;
			}
			ShopDataNew shopDataNew = list.Find((ShopDataNew obj) => obj.Type == _shopType);
			if (shopDataNew == null)
			{
				shopDataNew = list[0];
			}
			this.SetShopType(shopDataNew);
		}

		private void onClick_ShopToggle(GameObject obj = null)
		{
			ShopToggleController component = obj.GetComponent<ShopToggleController>();
			if (component == null)
			{
				return;
			}
			if (component == this.mCurShop)
			{
				return;
			}
			this.SetShopType(component.data);
		}

		private void WaitingView_open()
		{
			MobaMessageManagerTools.BeginWaiting_manual("WaitingShop", "加载中...", true, 15f, true);
		}

		private void WaitingView_close()
		{
			MobaMessageManagerTools.EndWaiting_manual("WaitingShop");
		}

		[DebuggerHidden]
		private IEnumerator SetPanelStatic()
		{
			ShopView.<SetPanelStatic>c__Iterator17D <SetPanelStatic>c__Iterator17D = new ShopView.<SetPanelStatic>c__Iterator17D();
			<SetPanelStatic>c__Iterator17D.<>f__this = this;
			return <SetPanelStatic>c__Iterator17D;
		}

		public void SetBackShop()
		{
			this.throughShop = this.mCurShop.data.Id;
		}

		public void CheckStatic()
		{
			if (this.cMgr != null)
			{
				if (this._panelStatic != null)
				{
					this.cMgr.StopCoroutine(this._panelStatic);
				}
				this.mGoodsPanel.widgetsAreStatic = false;
				this._panelStatic = this.cMgr.StartCoroutine(this.SetPanelStatic(), true);
			}
		}
	}
}
