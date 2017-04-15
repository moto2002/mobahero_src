using Assets.Scripts.GUILogic.View.BattleEquipment;
using MobaMessageData;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Assets.Scripts.Model
{
	internal class Model_battleShop : ModelBase<BattleShopData>
	{
		private const string key_recommededItems = "BattleShop_recommendedItems";

		private List<object> msgList;

		private BattleShopData data;

		private CoroutineManager coroutineMng;

		private Task waitTask;

		private Action onWaitingTaskTimeout;

		private RItemsTool rItemsTool;

		private SItemTool sItemsTool;

		private BE_msg_mng msgMng;

		private BattleEquipType CurMenu
		{
			get
			{
				return this.data.CurMenu;
			}
			set
			{
				this.data.CurMenu = value;
				MobaMessageManagerTools.SendClientMsg(ClientC2V.BattleShop_menuChanged, this.data.CurMenu, true);
			}
		}

		public SItemData CurSItem
		{
			get
			{
				return this.data.CurSItem;
			}
			set
			{
				this.data.CurSItem = value;
				MobaMessageManagerTools.SendClientMsg(ClientC2V.BattleShop_curShopItemChanged, this.data.CurSItem, true);
			}
		}

		public SItemData PreSItem
		{
			get
			{
				return this.data.PreSItem;
			}
			set
			{
				this.data.PreSItem = value;
			}
		}

		public SItemData CurSRItem
		{
			get
			{
				return this.data.CurSRItem;
			}
			set
			{
				this.data.CurSRItem = value;
				MobaMessageManagerTools.SendClientMsg(ClientC2V.BattleShop_curShopItemRouteChanged, this.data.CurSItem, true);
			}
		}

		public SItemData PreSRItem
		{
			get
			{
				return this.data.PreSRItem;
			}
			set
			{
				this.data.PreSRItem = value;
			}
		}

		public ItemInfo CurPItem
		{
			get
			{
				return this.data.CurPItem;
			}
			set
			{
				this.data.CurPItem = value;
				MobaMessageManagerTools.SendClientMsg(ClientC2V.BattleShop_curPossessItemChanged, this.data.CurPItem, true);
			}
		}

		public IBEItem CurTItem
		{
			get
			{
				return this.data.CurTItem;
			}
			set
			{
				this.data.CurTItem = value;
				MobaMessageManagerTools.SendClientMsg(ClientC2V.BattleShop_TItemChanged, this.data.CurTItem, true);
			}
		}

		public string CurHItem
		{
			get
			{
				return this.data.CurHItem;
			}
			set
			{
				this.data.CurHItem = value;
			}
		}

		public Dictionary<ColumnType, Dictionary<string, SItemData>> SItems
		{
			get
			{
				return this.data.SItems;
			}
			set
			{
				this.data.SItems = value;
			}
		}

		public List<ItemInfo> PItems
		{
			get
			{
				return this.data.PItems;
			}
			set
			{
				this.data.PItems = value;
				MobaMessageManagerTools.SendClientMsg(ClientC2V.BattleShop_possessItemsChanged, this.data.PItems, true);
			}
		}

		public List<string> PItemsStr
		{
			get
			{
				return this.data.PItemsStr;
			}
			set
			{
				this.data.PItemsStr = value;
			}
		}

		public List<string> RItems
		{
			get
			{
				return this.data.RItems;
			}
			set
			{
				if (this.data.RItemsSub != null || value != null)
				{
					this.data.RItems = value;
					string text = string.Join(",", this.data.RItems.ToArray());
					if (string.IsNullOrEmpty(text))
					{
						text = "[]";
					}
					C2PSetPlayerVar probufObj = new C2PSetPlayerVar
					{
						key = "BattleShop_recommendedItems",
						val = text
					};
					SendMsgManager.Instance.SendPvpMsgBase<C2PSetPlayerVar>(PvpCode.C2P_SetPlayerVar, probufObj);
				}
			}
		}

		public List<RItemData> RItemsSub
		{
			get
			{
				return this.data.RItemsSub;
			}
			set
			{
				if (this.data.RItemsSub != null || value != null)
				{
					this.data.RItemsSub = value;
					MobaMessageManagerTools.SendClientMsg(ClientC2V.BattleShop_recommendItemsSubChanged, this.data.RItemsSub, false);
				}
			}
		}

		public string SyncRItem
		{
			get
			{
				return this.data.SyncRItem;
			}
			set
			{
				this.data.SyncRItem = value;
			}
		}

		public int Money
		{
			get
			{
				return this.data.Money;
			}
			set
			{
				this.data.Money = value;
				MobaMessageManagerTools.SendClientMsg(ClientC2V.BattleShop_walletChanged, this.data.Money, true);
			}
		}

		private ShopInfo OpenShop
		{
			get
			{
				return this.data.OpenShop;
			}
			set
			{
				this.data.OpenShop = value;
				if (this.data.OpenShop != null)
				{
					this.data.OpenShop.Open = true;
				}
			}
		}

		private ShopInfo PreOpenShop
		{
			get
			{
				return this.data.PreOpenShop;
			}
			set
			{
				this.data.PreOpenShop = value;
				if (this.data.PreOpenShop != null)
				{
					this.data.PreOpenShop.Open = false;
				}
			}
		}

		private ShopInfo DealingShop
		{
			get
			{
				return this.data.DealingShop;
			}
			set
			{
				this.data.DealingShop = value;
			}
		}

		public Dictionary<EBattleShopType, ShopInfo> Shops
		{
			get
			{
				return this.data.DicShops;
			}
			set
			{
				this.data.DicShops = value;
			}
		}

		public bool PlayerAlive
		{
			get
			{
				return this.data.PlayerAlive;
			}
			set
			{
				this.data.PlayerAlive = value;
				MobaMessageManagerTools.SendClientMsg(ClientC2V.BattleShop_playerAliveChanged, this.data.PlayerAlive, true);
			}
		}

		public bool BrawlCanBuy
		{
			get
			{
				return this.data.Brawl_canBuy;
			}
			set
			{
				this.data.Brawl_canBuy = value;
				MobaMessageManagerTools.SendClientMsg(ClientC2V.BattleShop_brawlCanBuyChanged, this.data.Brawl_canBuy, true);
			}
		}

		public bool B_initRItems
		{
			get
			{
				return this.data.B_initRItems;
			}
			set
			{
				this.data.B_initRItems = value;
			}
		}

		public Model_battleShop()
		{
			base.Init(EModelType.Model_battleShop);
			this.data = new BattleShopData();
			base.Data = this.data;
			this.coroutineMng = new CoroutineManager();
			this.msgList = new List<object>
			{
				ClientC2C.BattleShop_initData,
				ClientC2C.BattleShop_setCurMenu,
				ClientC2C.BattleShop_setCurShopItem,
				ClientC2C.BattleShop_setCurPossessItem,
				ClientC2C.BattleShop_setMoney,
				ClientC2C.BattleShop_clearData,
				ClientC2C.HeroEquipChanged,
				ClientC2C.BattleShop_setPlayerAlive,
				ClientC2C.BattleShop_setInShopArea,
				ClientC2C.BattleShop_onOP,
				ClientV2V.RecommendEquipToggle,
				ClientC2C.BattleShop_syncRItems,
				PvpCode.C2P_BuyItem,
				PvpCode.C2P_SellItem,
				PvpCode.C2P_RevertShop
			};
		}

		public override void RegisterMsgHandler()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgList, true, "OnMsg_");
		}

		public override void UnRegisterMsgHandler()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgList, false, "OnMsg_");
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
		}

		private void OnMsg_BattleShop_initData(MobaMessage msg)
		{
			MsgData_BattleShop_initData msgData_BattleShop_initData = msg.Param as MsgData_BattleShop_initData;
			this.msgMng = new BE_msg_mng();
			this.rItemsTool = new RItemsTool(this.data);
			this.sItemsTool = new SItemTool(this.data);
			if (msgData_BattleShop_initData != null)
			{
				this.InitData(msgData_BattleShop_initData);
				this.InitShops(msgData_BattleShop_initData);
				this.InitRecommendItems(msgData_BattleShop_initData);
			}
			else
			{
				MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_err, "初始化商店参数错误", true);
			}
		}

		private void OnMsg_BattleShop_setCurMenu(MobaMessage msg)
		{
			this.Update_menu((BattleEquipType)((int)msg.Param));
			this.Update_sItems();
			this.Update_curSItem(null);
		}

		private void OnMsg_BattleShop_setCurShopItem(MobaMessage msg)
		{
			this.Update_curSItem(msg.Param as SItemData);
		}

		private void OnMsg_BattleShop_setCurPossessItem(MobaMessage msg)
		{
			this.Update_curPItem(msg.Param as ItemInfo);
		}

		private void OnMsg_HeroEquipChanged(MobaMessage msg)
		{
			HeroItemsChangedData heroItemsChangedData = msg.Param as HeroItemsChangedData;
			if (heroItemsChangedData != null)
			{
				Units player = MapManager.Instance.GetPlayer();
				if (player != null && player.unique_id == heroItemsChangedData._uid)
				{
					this.Update_curPItem(null);
					this.Update_pItems(heroItemsChangedData._list);
					this.Update_rItemsSub();
				}
			}
		}

		private void OnMsg_BattleShop_setMoney(MobaMessage msg)
		{
			int num = (int)msg.Param;
			if (this.Money.Equals(num))
			{
				return;
			}
			this.Money = num;
			this.sItemsTool.Update_onMoneyChanged();
			if (this.data.Enable_quickRecommend && (this.DealingShop == null || this.DealingShop.State != EBattleShopState.eIdle))
			{
				this.Update_rItemsSub_forMoney();
			}
		}

		private void OnMsg_BattleShop_setPlayerAlive(MobaMessage msg)
		{
			bool flag = (bool)msg.Param;
			if (this.PlayerAlive != flag)
			{
				this.PlayerAlive = flag;
				Dictionary<EBattleShopType, ShopInfo>.Enumerator enumerator = this.Shops.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<EBattleShopType, ShopInfo> current = enumerator.Current;
					current.Value.OnPlayerLiveChanged(this.PlayerAlive);
				}
				this.Update_brawlCanbuy();
			}
		}

		private void OnMsg_BattleShop_setInShopArea(MobaMessage msg)
		{
			ShopInfo shopInfo;
			bool flag;
			if (this.SetShopArea((Dictionary<EBattleShopType, bool>)msg.Param, out shopInfo, out flag))
			{
				if (this.OpenShop != null && !this.OpenShop.InArea && shopInfo != null && shopInfo != this.OpenShop)
				{
					this.PreOpenShop = this.OpenShop;
					this.OpenShop = shopInfo;
					if (this.PreOpenShop != this.OpenShop)
					{
						this.Update_menu(this.CurMenu);
						SItemData curSItem = this.CurSItem;
						this.Update_curSItem(null);
						this.Update_sItems();
						if (this.SItems != null && curSItem != null && this.SItems.ContainsKey(curSItem.Level) && this.SItems[curSItem.Level].ContainsKey(curSItem.ID))
						{
							this.Update_curSItem(this.SItems[curSItem.Level][curSItem.ID]);
						}
					}
				}
				MobaMessageManagerTools.SendClientMsg(ClientC2V.BattleShop_inShopAreaChanged, flag, false);
				this.Update_brawlCanbuy();
			}
		}

		private void OnMsg_BattleShop_clearData(MobaMessage msg)
		{
			this.data.ResetData();
			this.rItemsTool = null;
			this.sItemsTool = null;
			this.msgMng = null;
		}

		private void OnMsg_C2P_BuyItem(MobaMessage msg)
		{
			P2CBuyItem probufMsg = msg.GetProbufMsg<P2CBuyItem>();
			this.OnEnd_BuyItem(probufMsg);
		}

		private void OnMsg_C2P_SellItem(MobaMessage msg)
		{
			P2CSellItem probufMsg = msg.GetProbufMsg<P2CSellItem>();
			this.OnEnd_SellItem(probufMsg);
		}

		private void OnMsg_C2P_RevertShop(MobaMessage msg)
		{
			RetaMsg probufMsg = msg.GetProbufMsg<RetaMsg>();
			this.OnEnd_RevertShop(probufMsg);
		}

		private void OnMsg_RecommendEquipToggle(MobaMessage msg)
		{
			this.data.Enable_quickRecommend = (bool)msg.Param;
			this.Update_rItemsSub();
		}

		private void OnMsg_BattleShop_syncRItems(MobaMessage msg)
		{
			Dictionary<string, string> dictionary = msg.Param as Dictionary<string, string>;
			if (this.B_initRItems)
			{
				return;
			}
			if (dictionary == null || !dictionary.ContainsKey("BattleShop_recommendedItems"))
			{
				return;
			}
			this.SyncRItem = dictionary["BattleShop_recommendedItems"];
		}

		private void OnMsg_BattleShop_onOP(MobaMessage msg)
		{
			MsgData_BattleShop_onOP msgData_BattleShop_onOP = msg.Param as MsgData_BattleShop_onOP;
			if (msgData_BattleShop_onOP != null)
			{
				switch (msgData_BattleShop_onOP.op)
				{
				case EBattleShopOP.eOpen:
					this.OnOpen(msgData_BattleShop_onOP);
					break;
				case EBattleShopOP.eClose:
					this.OnClose(msgData_BattleShop_onOP);
					break;
				case EBattleShopOP.eBuy:
					this.OnBuy(msgData_BattleShop_onOP);
					break;
				case EBattleShopOP.eSell:
					this.OnSell(msgData_BattleShop_onOP);
					break;
				case EBattleShopOP.eRevert:
					this.OnRevert(msgData_BattleShop_onOP);
					break;
				}
			}
		}

		private void OnOpen(MsgData_BattleShop_onOP param)
		{
			Dictionary<EBattleShopType, ShopInfo> shops = this.Shops;
			if (shops.ContainsKey(param.shopType))
			{
				ShopInfo shopInfo = shops[param.shopType];
				if (this.OpenShop != shopInfo)
				{
					if (this.OpenShop != null)
					{
						this.PreOpenShop = this.OpenShop;
					}
					this.OpenShop = shopInfo;
					if (this.PreOpenShop != this.OpenShop)
					{
						this.Update_menu(BattleEquipType.recommend);
						this.Update_curSItem(null);
						this.Update_sItems();
					}
				}
			}
		}

		private void OnClose(MsgData_BattleShop_onOP param)
		{
			if (this.OpenShop != null)
			{
				this.PreOpenShop = this.OpenShop;
				this.OpenShop = null;
			}
		}

		private void OnBuy(MsgData_BattleShop_onOP param)
		{
			Dictionary<EBattleShopType, ShopInfo> shops = this.Shops;
			if (shops.ContainsKey(param.shopType))
			{
				ShopInfo shopInfo = shops[param.shopType];
				shopInfo.State = EBattleShopState.eBuying;
				this.DealingShop = shopInfo;
				this.CurHItem = param.targetItem;
				this.data.CurRollbackInfo = this.GetCurRollbackInfo(param);
				this.onWaitingTaskTimeout = new Action(this.OnTimeOut);
				this.StartWaitCoroutine();
			}
		}

		private void OnSell(MsgData_BattleShop_onOP param)
		{
			Dictionary<EBattleShopType, ShopInfo> shops = this.Shops;
			if (shops.ContainsKey(param.shopType))
			{
				ShopInfo shopInfo = shops[param.shopType];
				shopInfo.State = EBattleShopState.eSelling;
				this.DealingShop = shopInfo;
				this.CurHItem = param.targetItem;
				this.data.CurRollbackInfo = this.GetCurRollbackInfo(param);
				this.onWaitingTaskTimeout = new Action(this.OnTimeOut);
				this.StartWaitCoroutine();
			}
		}

		private void OnRevert(MsgData_BattleShop_onOP param)
		{
			Dictionary<EBattleShopType, ShopInfo> shops = this.Shops;
			if (shops.ContainsKey(param.shopType))
			{
				ShopInfo shopInfo = shops[param.shopType];
				shopInfo.State = EBattleShopState.eRollBack;
				this.DealingShop = shopInfo;
				this.onWaitingTaskTimeout = new Action(this.OnTimeOut);
				this.StartWaitCoroutine();
			}
		}

		private void OnEnd_BuyItem(P2CBuyItem ret)
		{
			this.StopWaitCoroutine();
			if (ret != null && ret.retaCode == 0)
			{
				List<string> rItems = BattleEquipTools_config.GetRItems(this.RItems, this.CurHItem, this.PItems);
				if (BattleEquipTools_config.IsChanged(this.RItems, rItems))
				{
					this.Update_rItems(rItems);
					this.Update_rItemsSub();
					if (this.CurMenu == BattleEquipType.recommend)
					{
						this.Update_curSItem(null);
						this.Update_sItems();
					}
				}
				if (this.DealingShop != null)
				{
					this.DealingShop.DealCounter++;
					this.DealingShop.RollbackStack.Push(this.data.CurRollbackInfo);
					this.data.CurRollbackInfo = null;
				}
			}
			else
			{
				MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_err, "购买失败,errCode=" + ret.retaCode, false);
			}
			if (this.DealingShop != null)
			{
				this.DealingShop.State = EBattleShopState.eIdle;
				this.DealingShop = null;
			}
			this.CurHItem = null;
		}

		private void OnEnd_SellItem(P2CSellItem ret)
		{
			this.StopWaitCoroutine();
			if (ret != null && ret.retaCode == 0)
			{
				if (this.DealingShop != null)
				{
					this.DealingShop.DealCounter++;
					this.DealingShop.RollbackStack.Push(this.data.CurRollbackInfo);
					this.data.CurRollbackInfo = null;
				}
			}
			else
			{
				MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_err, "卖出失败,errCode=" + ret.retaCode, false);
			}
			if (this.DealingShop != null)
			{
				this.DealingShop.State = EBattleShopState.eIdle;
				this.DealingShop = null;
			}
			this.CurHItem = null;
		}

		private void OnEnd_RevertShop(RetaMsg ret)
		{
			this.StopWaitCoroutine();
			if (ret != null && ret.retaCode == 0)
			{
				if (this.DealingShop != null)
				{
					if (this.DealingShop.RollbackStack.Count > 0)
					{
						RollbackInfo rollbackInfo = this.DealingShop.RollbackStack.Pop();
						this.SetRollbackInfo(rollbackInfo);
					}
					this.DealingShop.DealCounter--;
				}
			}
			else
			{
				MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_err, "撤销失败,errCode=" + ret.retaCode, false);
			}
			if (this.DealingShop != null)
			{
				this.DealingShop.State = EBattleShopState.eIdle;
				this.DealingShop = null;
			}
		}

		private void OnTimeOut()
		{
			if (this.DealingShop != null && this.DealingShop.State != EBattleShopState.eIdle)
			{
				MobaMessageManagerTools.SendClientMsg(ClientC2V.BattleShop_waitServerMsgTimeOut, this.DealingShop.State, false);
				this.DealingShop.ResetShop();
				this.DealingShop = null;
			}
		}

		private void StartWaitCoroutine()
		{
			this.waitTask = this.coroutineMng.StartCoroutine(this.WaitTimeOut(10f), false);
			this.waitTask.Finished += new Task.FinishedHandler(this.OnWaitTimeOut);
			this.waitTask.Start();
		}

		private void StopWaitCoroutine()
		{
			if (this.waitTask != null)
			{
				this.waitTask.Stop();
				this.waitTask = null;
			}
		}

		private void OnWaitTimeOut(bool manual)
		{
			if (!manual && this.onWaitingTaskTimeout != null)
			{
				this.onWaitingTaskTimeout();
			}
		}

		[DebuggerHidden]
		private IEnumerator WaitTimeOut(float waitTime)
		{
			Model_battleShop.<WaitTimeOut>c__Iterator18E <WaitTimeOut>c__Iterator18E = new Model_battleShop.<WaitTimeOut>c__Iterator18E();
			<WaitTimeOut>c__Iterator18E.waitTime = waitTime;
			<WaitTimeOut>c__Iterator18E.<$>waitTime = waitTime;
			return <WaitTimeOut>c__Iterator18E;
		}

		private void Update_menu(BattleEquipType menu)
		{
			this.CurMenu = menu;
		}

		private void Update_curSItem(SItemData itemData)
		{
			this.PreSItem = this.CurSItem;
			this.CurSItem = itemData;
			this.PreSRItem = this.CurSRItem;
			this.CurSRItem = this.CurSItem;
			this.sItemsTool.Update_curSItem();
			this.sItemsTool.Update_curSRItem();
			if (this.CurSItem == null)
			{
				this.CurTItem = ((this.CurPItem == null) ? null : this.CurPItem);
			}
			else
			{
				this.CurTItem = this.CurSItem;
				this.CurPItem = null;
			}
		}

		private void Update_curPItem(ItemInfo itemData)
		{
			this.CurPItem = itemData;
			if (this.CurPItem == null)
			{
				this.CurTItem = ((this.CurSItem == null) ? null : this.CurSItem);
			}
			else
			{
				ColumnType level = this.CurPItem.Level;
				string iD = this.CurPItem.ID;
				this.Update_menu(this.CurPItem.Type);
				this.Update_sItems();
				this.Update_curSItem(null);
				if (this.SItems != null && this.SItems.ContainsKey(level) && this.SItems[level].ContainsKey(iD))
				{
					this.PreSItem = this.CurSItem;
					this.CurSItem = null;
					this.PreSRItem = this.CurSRItem;
					this.CurSRItem = this.SItems[level][iD];
					this.sItemsTool.Update_curSItem();
					this.sItemsTool.Update_curSRItem();
				}
				this.CurTItem = this.CurPItem;
			}
		}

		private void Update_sItems()
		{
			if (this.CurMenu != BattleEquipType.none)
			{
				if (this.CurMenu == BattleEquipType.recommend)
				{
					this.SItems = BattleEquipTools_config.GetShopItems_Recommend(this.SItems, this.RItems);
				}
				else
				{
					this.SItems = BattleEquipTools_config.GetShopItems_common(this.SItems, this.CurMenu, this.OpenShop);
				}
			}
			else
			{
				this.SItems = null;
			}
			this.sItemsTool.Update_sItems();
			MobaMessageManagerTools.SendClientMsg(ClientC2V.BattleShop_shopItemsChanged, this.SItems, false);
		}

		private void Update_pItems(List<ItemDynData> dyList)
		{
			this.PItems = BattleEquipTools_Travers.GetItemList(dyList);
			this.PItemsStr = BattleEquipTools_Travers.GetItemListString(this.PItems);
			if (this.sItemsTool != null)
			{
				this.sItemsTool.Update_onPItemsChanged();
			}
		}

		private void Update_brawlCanbuy()
		{
			bool flag = this.BrawlCanBuy;
			if (this.data.ShopContext == EBattleShopContex.eBrawl)
			{
				if (this.BrawlCanBuy)
				{
					if (this.PlayerAlive)
					{
						bool flag2 = true;
						foreach (KeyValuePair<EBattleShopType, ShopInfo> current in this.Shops)
						{
							if (current.Value.InArea)
							{
								flag2 = false;
								break;
							}
						}
						if (flag2)
						{
							flag = false;
						}
					}
				}
				else if (!this.PlayerAlive)
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (this.BrawlCanBuy != flag)
			{
				this.BrawlCanBuy = flag;
			}
		}

		private void Update_rItems(List<string> list)
		{
			this.RItems = list;
		}

		private void Update_rItemsSub()
		{
			if (!this.data.Enable_quickRecommend)
			{
				if (this.RItemsSub != null && this.RItemsSub.Count > 0)
				{
					this.RItemsSub = null;
				}
			}
			else if (((this.RItems != null && 0 < this.RItems.Count) || (this.rItemsTool != null && this.rItemsTool.ValidTree())) && this.rItemsTool != null)
			{
				this.RItemsSub = this.rItemsTool.Update_rItemsSub();
			}
		}

		private void Update_rItemsSub_forMoney()
		{
			if (!this.data.Enable_quickRecommend)
			{
				return;
			}
			if (!this.rItemsTool.ValidTree())
			{
				return;
			}
			this.RItemsSub = this.rItemsTool.Update_rItemsSub_forMoney();
		}

		private RollbackInfo GetCurRollbackInfo(MsgData_BattleShop_onOP param)
		{
			int num = 0;
			List<ItemInfo> itemsSnapshoot = null;
			if (this.data.ShopContext == EBattleShopContex.ePve)
			{
				num = param.realPrice;
				itemsSnapshoot = BattleEquipTools_Travers.CloneItemsList(this.PItems);
				if (param.op == EBattleShopOP.eBuy)
				{
					num = -num;
				}
				else
				{
					num = param.itemInfo.Config.sale;
				}
			}
			List<string> recommendItems = new List<string>(this.RItems);
			return new RollbackInfo(num, itemsSnapshoot, recommendItems);
		}

		private void SetRollbackInfo(RollbackInfo snapShoot)
		{
			if (snapShoot != null && BattleEquipTools_config.IsChanged(this.RItems, snapShoot._recommendItems))
			{
				this.Update_rItems(snapShoot._recommendItems);
				this.Update_rItemsSub();
				if (this.CurMenu == BattleEquipType.recommend)
				{
					this.Update_curSItem(null);
					this.Update_sItems();
				}
			}
		}

		private void InitData(MsgData_BattleShop_initData param)
		{
			this.data.LevelID = param.levelID;
			this.data.ShopContext = param.eBattleType;
			this.data.Brawl_canBuy = true;
			this.data.Enable_quickRecommend = param.enableQuickR;
		}

		private void InitShops(MsgData_BattleShop_initData param)
		{
			Dictionary<EBattleShopType, ShopInfo> shops;
			if (BattleEquipTools_config.GetShopIDListByScene(param.levelID, param.teamType, out shops))
			{
				this.Shops = shops;
			}
			else
			{
				MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_err, "获取商店配置出错", false);
			}
		}

		private void InitRecommendItems(MsgData_BattleShop_initData param)
		{
			if (string.IsNullOrEmpty(this.SyncRItem))
			{
				Units player = MapManager.Instance.GetPlayer();
				if (player != null)
				{
					List<string> rItems = BattleEquipTools_config.GetRItems(this.data.LevelID, player.npc_id);
					if (BattleEquipTools_config.IsChanged(this.RItems, rItems))
					{
						this.Update_rItems(rItems);
						this.Update_rItemsSub();
						if (this.CurMenu == BattleEquipType.recommend)
						{
							this.Update_curSItem(null);
							this.Update_sItems();
						}
					}
				}
			}
			else
			{
				this.Update_rItems(BattleEquipTools_config.StringToStringList(this.SyncRItem, ',', "[]"));
				this.Update_rItemsSub();
				if (this.CurMenu == BattleEquipType.recommend)
				{
					this.Update_curSItem(null);
					this.Update_sItems();
				}
			}
			this.B_initRItems = true;
		}

		private bool SetShopArea(Dictionary<EBattleShopType, bool> dicInArea, out ShopInfo shopInArea, out bool inOrOut)
		{
			bool result = false;
			shopInArea = null;
			inOrOut = false;
			if (dicInArea != null)
			{
				Dictionary<EBattleShopType, ShopInfo> shops = this.Shops;
				Dictionary<EBattleShopType, bool>.Enumerator enumerator = dicInArea.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Dictionary<EBattleShopType, ShopInfo> arg_32_0 = shops;
					KeyValuePair<EBattleShopType, bool> current = enumerator.Current;
					if (arg_32_0.ContainsKey(current.Key))
					{
						Dictionary<EBattleShopType, ShopInfo> arg_52_0 = shops;
						KeyValuePair<EBattleShopType, bool> current2 = enumerator.Current;
						ShopInfo shopInfo = arg_52_0[current2.Key];
						bool arg_6E_0 = shopInfo.InArea;
						KeyValuePair<EBattleShopType, bool> current3 = enumerator.Current;
						if (arg_6E_0 != current3.Value)
						{
							result = true;
							KeyValuePair<EBattleShopType, bool> current4 = enumerator.Current;
							bool value = current4.Value;
							shopInfo.InArea = value;
							inOrOut = value;
							if (shopInfo.InArea)
							{
								shopInArea = shopInfo;
							}
						}
					}
				}
			}
			return result;
		}
	}
}
