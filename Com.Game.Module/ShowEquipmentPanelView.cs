using Assets.Scripts.GUILogic.View.BattleEquipment;
using Assets.Scripts.Model;
using Com.Game.Utils;
using GUIFramework;
using MobaHeros;
using MobaMessageData;
using Newbie;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class ShowEquipmentPanelView : BaseView<ShowEquipmentPanelView>
	{
		private UIPanel panel;

		private UIGrid grid_bag;

		private UIGrid grid_recommend;

		private Transform btn_shop;

		private UILabel label_money;

		private UILabel label_money2;

		private Transform TopAnchor;

		private Transform DAnchor;

		private Transform SAnchor;

		private Transform RAnchor;

		private UIGrid grid_attri;

		private GameObject buyBtnEffect;

		private GameObject buyBtnEffect_s;

		private Dictionary<AttrType, BattleEquip_AttriItem> dicAttriCom;

		private Dictionary<string, BattleEquip_recommendItem> dicRecommendItemCom;

		private List<BattleEquip_PossessItem> listPItemsCom;

		private BattleEquip_AttriItem template_attri;

		private BattleEquip_PossessItem template_possessItem;

		private BattleEquip_recommendItem template_recommendItem;

		private Task task_attriDetailsTip;

		private Task task_delaySetStatic;

		private HeroDetailedAttr heroAttr;

		private List<ItemInfo> possessItems;

		private List<RItemData> recommendItemsSub;

		private Vector2[] RAchorOffset;

		private object[] msgs;

		private int money;

		private IBEItem handlingRItem;

		private bool showDetail;

		private int waitFrameCounter;

		private bool widgetState;

		public ShowEquipmentPanelView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/BattleEquipment/ShowEquipmentPanel");
		}

		public override void Init()
		{
			base.Init();
			this.template_possessItem = Resources.Load<BattleEquip_PossessItem>("Prefab/UI/BattleEquipment/PossessItem");
			this.template_attri = Resources.Load<BattleEquip_AttriItem>("Prefab/UI/BattleEquipment/AttriItem");
			this.template_recommendItem = Resources.Load<BattleEquip_recommendItem>("Prefab/UI/BattleEquipment/RecommendItem");
			this.panel = this.transform.GetComponent<UIPanel>();
			this.grid_recommend = this.transform.FindChild("Anchor/RAnchor/RecommendGrid").GetComponent<UIGrid>();
			this.grid_bag = this.transform.Find("Anchor/DAnchor/EquipmentGrid").GetComponent<UIGrid>();
			this.btn_shop = this.transform.Find("Anchor/DAnchor/Btn");
			this.label_money = this.transform.Find("Anchor/DAnchor/Btn/Money").GetComponent<UILabel>();
			this.label_money2 = this.transform.Find("Anchor/SAnchor/Money").GetComponent<UILabel>();
			this.TopAnchor = this.transform.Find("Anchor/TopAnchor");
			this.DAnchor = this.transform.FindChild("Anchor/DAnchor");
			this.SAnchor = this.transform.FindChild("Anchor/SAnchor");
			this.RAnchor = this.transform.FindChild("Anchor/RAnchor");
			UIEventListener.Get(this.btn_shop.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClick_ownShop);
			UIEventListener.Get(this.SAnchor.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClick_ownShop);
			this.grid_attri = this.transform.Find("Anchor/DAnchor/NatureGrid").GetComponent<UIGrid>();
			this.buyBtnEffect = this.transform.FindChild("Anchor/DAnchor/Btn/Fx_ui_MoneyEnough").gameObject;
			this.buyBtnEffect_s = this.transform.FindChild("Anchor/SAnchor/Fx_ui_MoneyEnough").gameObject;
			this.dicAttriCom = new Dictionary<AttrType, BattleEquip_AttriItem>();
			this.dicRecommendItemCom = new Dictionary<string, BattleEquip_recommendItem>();
			this.listPItemsCom = new List<BattleEquip_PossessItem>();
			this.recommendItemsSub = new List<RItemData>();
			this.msgs = new object[]
			{
				ClientC2V.BattleShop_possessItemsChanged,
				ClientC2V.BattleShop_walletChanged,
				ClientC2C.HeroAttrChange,
				ClientC2V.BattleShop_recommendItemsSubChanged,
				ClientC2V.BattleShop_inShopAreaChanged,
				ClientC2V.BattleShop_playerAliveChanged,
				ClientV2V.DetailedShopToggle,
				ClientV2V.SetSkillPanelPivot
			};
			this.template_possessItem.RootGoObj = this.gameObject;
			this.template_recommendItem.RootGoObj = this.gameObject;
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(this.DAnchor);
			this.RAchorOffset = new Vector2[]
			{
				new Vector2(0f, bounds.size.y + 10f),
				new Vector2(bounds.size.x + 10f, 0f)
			};
		}

		public override void RegisterUpdateHandler()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, true, "OnMsg_");
		}

		public override void CancelUpdateHandler()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, false, "OnMsg_");
		}

		public override void HandleAfterOpenView()
		{
			base.HandleAfterOpenView();
			this.InitData();
			this.RefreshUI();
			this.RefreshUI_initPossessItems();
			this.RefreshUI_initAttriList();
			this.RefreshUI_money();
			this.LoadHeroAttriMain();
			this.RefreshUI_rItemsPos();
		}

		public override void HandleBeforeCloseView()
		{
			this.possessItems = null;
			this.dicAttriCom.Clear();
		}

		public override void RefreshUI()
		{
			this.TopAnchor.gameObject.SetActive(false);
			this.showDetail = ModelManager.Instance.Get_SettingData().detailedShop;
			this.RefreshUI_detailShop();
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void InitData()
		{
			this.possessItems = ModelManager.Instance.Get_BattleShop_pItems();
			this.money = ModelManager.Instance.Get_BattleShop_money();
			this.LoadHeroAttri();
		}

		private void LoadHeroAttri()
		{
			Units player = MapManager.Instance.GetPlayer();
			if (null != player)
			{
				this.heroAttr = ((Hero)player).getDetailedAttr();
			}
		}

		private void LoadHeroAttriMain()
		{
			Units player = MapManager.Instance.GetPlayer();
			if (null != player)
			{
				Dictionary<AttrType, BattleEquip_AttriItem>.Enumerator enumerator = this.dicAttriCom.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<AttrType, BattleEquip_AttriItem> current = enumerator.Current;
					BattleEquip_AttriItem arg_4C_0 = current.Value;
					Units arg_47_0 = player;
					KeyValuePair<AttrType, BattleEquip_AttriItem> current2 = enumerator.Current;
					arg_4C_0.AttriValue = arg_47_0.GetAttr(current2.Key);
				}
			}
		}

		private void OnMsg_BattleShop_possessItemsChanged(MobaMessage msg)
		{
			this.possessItems = (msg.Param as List<ItemInfo>);
			this.RefreshUI_initPossessItems();
			this.RefreshUI_newItem();
		}

		private void OnMsg_BattleShop_walletChanged(MobaMessage msg)
		{
			if (this.money != (int)msg.Param)
			{
				this.money = (int)msg.Param;
				this.RefreshUI_money();
			}
		}

		private void OnMsg_BattleShop_recommendItemsSubChanged(MobaMessage msg)
		{
			List<RItemData> list = msg.Param as List<RItemData>;
			bool itemsChanged = BattleEquipTools_config.IsChanged(this.recommendItemsSub, list);
			this.recommendItemsSub.Clear();
			this.recommendItemsSub.AddRange(list);
			this.RefreshUI_initRecommendItems(itemsChanged);
		}

		private void OnMsg_BattleShop_inShopAreaChanged(MobaMessage msg)
		{
			this.RefreshUI_RecommendAlpha();
		}

		private void OnMsg_BattleShop_playerAliveChanged(MobaMessage msg)
		{
			this.RefreshUI_RecommendAlpha();
		}

		private void OnMsg_DetailedShopToggle(MobaMessage msg)
		{
			this.showDetail = (bool)msg.Param;
			this.RefreshUI_detailShop();
			this.RefreshUI_money();
			if (this.showDetail)
			{
				this.RefreshUI_initAttriList();
				this.LoadHeroAttriMain();
			}
			else
			{
				this.handlingRItem = null;
			}
		}

		private void OnMsg_SetSkillPanelPivot(MobaMessage msg)
		{
			SkillPanelPivot skillPanelPivot = (SkillPanelPivot)((int)msg.Param);
			this.RefreshUI_rItemsPos();
		}

		private void OnMsg_HeroAttrChange(MobaMessage msg)
		{
			MsgData_AttrChangeData msgData_AttrChangeData = msg.Param as MsgData_AttrChangeData;
			if (msgData_AttrChangeData != null)
			{
				this.RefreshUI_heroAttri(msgData_AttrChangeData);
			}
		}

		private void OnClick_ownShop(GameObject obj)
		{
			this.handlingRItem = null;
			EBattleShopType nearestShopType = BattleEquipTools_op.GetNearestShopType();
			MobaMessageManagerTools.BattleShop_openBattleShop(nearestShopType, EBattleShopOpenType.eFromButton);
		}

		private void OnClick_bagItem(BattleEquip_PossessItem com)
		{
			if (com.Initiative)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.BattleShop_clickItemInBag, com.ItemData, false);
			}
		}

		private void OnClick_recommendItem(BattleEquip_recommendItem com)
		{
			if (this.showDetail)
			{
				this.handlingRItem = com.RecommendItem;
			}
			MobaMessageManagerTools.SendClientMsg(ClientV2C.BattleShop_clickRItem, com.RecommendItem, false);
			NewbieManager.Instance.TryTriggerBuyEquipHint();
		}

		[DebuggerHidden]
		private IEnumerator coroutine_delaySetStatic()
		{
			ShowEquipmentPanelView.<coroutine_delaySetStatic>c__IteratorFE <coroutine_delaySetStatic>c__IteratorFE = new ShowEquipmentPanelView.<coroutine_delaySetStatic>c__IteratorFE();
			<coroutine_delaySetStatic>c__IteratorFE.<>f__this = this;
			return <coroutine_delaySetStatic>c__IteratorFE;
		}

		private void RefreshUI_setStatic()
		{
			this.panel.widgetsAreStatic = false;
			if (this.waitFrameCounter < 5)
			{
				this.waitFrameCounter = 5;
			}
			if (this.task_delaySetStatic == null)
			{
				this.task_delaySetStatic = new Task(this.coroutine_delaySetStatic(), false);
				this.task_delaySetStatic.Finished += new Task.FinishedHandler(this.OnTaskFinish_delaySetStatic);
				this.task_delaySetStatic.Start();
			}
		}

		private void OnTaskFinish_delaySetStatic(bool manual)
		{
			this.task_delaySetStatic = null;
			this.panel.widgetsAreStatic = true;
		}

		private void RefreshUI_initPossessItems()
		{
			this.RefreshUI_setStatic();
			this.listPItemsCom.Clear();
			List<ItemInfo> list = this.possessItems;
			GridHelper.FillGrid<BattleEquip_PossessItem>(this.grid_bag, this.template_possessItem, (list == null) ? 0 : list.Count, delegate(int idx, BattleEquip_PossessItem comp)
			{
				comp.ItemData = list[idx];
				comp.ChooseState = false;
				comp.name = list[idx].ID;
				comp.TurnOnCD = true;
				comp.OnClickItem = new Callback<BattleEquip_PossessItem>(this.OnClick_bagItem);
				comp.transform.localScale = new Vector3(1f, 1f, 1f);
				comp.gameObject.SetActive(true);
				this.listPItemsCom.Add(comp);
			});
			this.grid_bag.Reposition();
		}

		private void RefreshUI_initAttriList()
		{
			this.dicAttriCom.Clear();
			List<AttrType> list = new List<AttrType>(BattleEquipTools_config.dicAttriShowInfo.Keys);
			GridHelper.FillGrid<BattleEquip_AttriItem>(this.grid_attri, this.template_attri, (list == null) ? 0 : list.Count, delegate(int idx, BattleEquip_AttriItem comp)
			{
				comp.AttriTypeP = list[idx];
				comp.AttriValue = 0f;
				comp.name = list[idx].ToString();
				comp.gameObject.SetActive(true);
				this.dicAttriCom.Add(list[idx], comp);
			});
			this.grid_attri.Reposition();
		}

		private void RefreshUI_initRecommendItems(bool itemsChanged)
		{
			if (itemsChanged)
			{
				this.RefreshUI_setStatic();
			}
			this.dicRecommendItemCom.Clear();
			List<RItemData> list = this.recommendItemsSub;
			GridHelper.FillGrid<BattleEquip_recommendItem>(this.grid_recommend, this.template_recommendItem, (list == null) ? 0 : list.Count, delegate(int idx, BattleEquip_recommendItem comp)
			{
				RItemData rItemData = list[idx];
				comp.RecommendItem = rItemData;
				comp.name = rItemData.ID;
				comp.ShowDetail = itemsChanged;
				comp.Callback_ClickItem = new Callback<BattleEquip_recommendItem>(this.OnClick_recommendItem);
				comp.gameObject.SetActive(true);
				this.dicRecommendItemCom.Add(rItemData.ID, comp);
			});
			this.grid_recommend.Reposition();
			this.RefreshUI_RecommendAlpha();
			this.widgetState = true;
		}

		private void RefreshUI_heroAttri(MsgData_AttrChangeData data)
		{
			AttrType nType = (AttrType)data.nType;
			if (this.dicAttriCom.ContainsKey(nType))
			{
				this.dicAttriCom[nType].AttriValue = data.fValue;
			}
		}

		public void RefreshUI_money()
		{
			if (this.label_money.gameObject.activeInHierarchy)
			{
				this.label_money.text = this.money.ToString();
			}
			else if (this.label_money2.gameObject.activeInHierarchy)
			{
				this.label_money2.text = this.money.ToString();
			}
		}

		private void RefreshUI_RecommendAlpha()
		{
			bool flag = ModelManager.Instance.Get_BattleShop_playerAlive();
			bool active = false;
			if (this.recommendItemsSub != null)
			{
				foreach (RItemData current in this.recommendItemsSub)
				{
					bool flag2 = !flag || current.InShopArea();
					if (flag2)
					{
						active = flag2;
					}
					this.dicRecommendItemCom[current.ID].Alpha = ((!flag2) ? 0.5f : 1f);
				}
			}
			this.buyBtnEffect.gameObject.SetActive(active);
			this.buyBtnEffect_s.gameObject.SetActive(active);
		}

		private void RefreshUI_detailShop()
		{
			this.DAnchor.gameObject.SetActive(this.showDetail);
			this.SAnchor.gameObject.SetActive(!this.showDetail);
		}

		private void RefreshUI_newItem()
		{
			if (this.handlingRItem != null)
			{
				if (this.possessItems != null)
				{
					int num = this.possessItems.LastIndexOf(new ItemInfo(this.handlingRItem.Config));
					if (num >= 0 && num < this.listPItemsCom.Count)
					{
						this.listPItemsCom[num].PlayNewItemAnimator(true);
					}
				}
				this.handlingRItem = null;
			}
		}

		private void RefreshUI_rItemsPos()
		{
			UIAnchor component = this.RAnchor.GetComponent<UIAnchor>();
			SkillPanelPivot skillPanelPivot = (SkillPanelPivot)ModelManager.Instance.Get_SettingData().skillPanelPivot;
			component.pixelOffset = ((skillPanelPivot != SkillPanelPivot.Left) ? this.RAchorOffset[0] : this.RAchorOffset[1]);
			component.enabled = true;
			this.RefreshUI_setStatic();
		}

		public void NewbieBuyRecommendItem()
		{
			if (this.grid_recommend == null)
			{
				return;
			}
			Transform transform = this.grid_recommend.transform;
			if (transform == null)
			{
				return;
			}
			int childCount = transform.childCount;
			if (childCount > 0)
			{
				Transform child = transform.GetChild(0);
				if (child != null)
				{
					BattleEquip_recommendItem component = child.gameObject.GetComponent<BattleEquip_recommendItem>();
					if (component != null)
					{
						MobaMessageManagerTools.SendClientMsg(ClientV2C.BattleShop_clickRItem, component.RecommendItem, false);
					}
				}
			}
		}

		private bool CheckWidgeReady()
		{
			return this.widgetState;
		}
	}
}
