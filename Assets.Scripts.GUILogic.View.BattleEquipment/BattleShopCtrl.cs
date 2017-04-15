using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros.Pvp;
using MobaMessageData;
using MobaProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Utils;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public class BattleShopCtrl : BaseGameModule
	{
		private CoroutineManager coroutineMng;

		private bool enable;

		private object[] msgList;

		public override void Init()
		{
			this.enable = !Singleton<PvpManager>.Instance.IsObserver;
			if (this.enable)
			{
				this.InitCom();
				this.Register();
			}
		}

		public override void Uninit()
		{
			if (this.enable)
			{
				this.Unregister();
				this.UnInitCom();
			}
		}

		public override void OnGameStateChange(GameState oldState, GameState newState)
		{
			if (!this.enable)
			{
				return;
			}
			switch (newState)
			{
			case GameState.Game_Playing:
				MobaMessageManagerTools.BattleShop_initData(Singleton<PvpManager>.Instance.IsContinuedBattle, BattleEquipTools_op.GetBattleShopContex(), LevelManager.CurLevelId, BattleEquipTools_op.GetShopTypeByTeamType(), ModelManager.Instance.Get_SettingData().recommendOn);
				this.coroutineMng.StartCoroutine(this.Check(), true);
				this.coroutineMng.StartCoroutine(this.UpdateWallet(), true);
				break;
			case GameState.Game_Over:
				this.CloseView_ShopView();
				this.CloseView_BagView();
				break;
			}
		}

		private void InitCom()
		{
			this.coroutineMng = new CoroutineManager();
			this.msgList = new object[]
			{
				ClientV2C.BattleShop_openShop,
				ClientV2C.BattleShop_closeShop,
				ClientV2C.BattleShop_clickMenu,
				ClientV2C.BattleShop_clickSItem,
				ClientV2C.BattleShop_clickPItem,
				ClientV2C.BattleShop_clickBack,
				ClientV2C.BattleShop_DClickSItem,
				ClientV2C.BattleShop_clickBuy,
				ClientV2C.BattleShop_clickRItem,
				ClientV2C.BattleShop_clickSell,
				ClientV2C.BattleShop_clickRollback,
				ClientC2V.BattleShop_playerAliveChanged,
				ClientC2V.BattleShop_inShopAreaChanged,
				ClientC2V.BattleShop_waitServerMsgTimeOut,
				ClientC2C.BattleShop_err,
				PvpCode.C2P_BuyItem,
				PvpCode.C2P_SellItem,
				PvpCode.C2P_RevertShop
			};
		}

		private void UnInitCom()
		{
			this.msgList = null;
			this.CloseView_BagView();
			this.CloseView_ShopView();
			MobaMessageManagerTools.BattleShop_clearData();
			this.coroutineMng.StopAllCoroutine();
			this.coroutineMng = null;
		}

		private void Register()
		{
			TriggerManager.CreateGameEventTrigger(GameEvent.MainPlayerHitted, null, new TriggerAction(this.OnMainPlayerHitted));
			MobaMessageManagerTools.RegistMsg(this, this.msgList, true, "OnMsg_");
		}

		private void Unregister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgList, false, "OnMsg_");
		}

		private void OnMsg_BattleShop_openShop(MobaMessage msg)
		{
			object[] array = msg.Param as object[];
			this.OpenBattleShop((EBattleShopType)((int)array[0]), (EBattleShopOpenType)((int)array[1]));
		}

		private void OnMsg_BattleShop_closeShop(MobaMessage msg)
		{
			this.CloseView_ShopView();
		}

		private void OnMsg_BattleShop_clickMenu(MobaMessage msg)
		{
			MobaMessageManagerTools.BattleShop_setCurMenu(msg.Param);
		}

		private void OnMsg_BattleShop_clickSItem(MobaMessage msg)
		{
			MobaMessageManagerTools.BattleShop_setCurShopItem(msg.Param);
		}

		private void OnMsg_BattleShop_clickPItem(MobaMessage msg)
		{
			MobaMessageManagerTools.BattleShop_setCurPossessItem(msg.Param);
		}

		private void OnMsg_BattleShop_clickBack(MobaMessage msg)
		{
			this.CloseView_ShopView();
		}

		private void OnMsg_BattleShop_DClickSItem(MobaMessage msg)
		{
			ShopInfo curShopInfo = ModelManager.Instance.Get_BattleShop_openShop();
			SItemData sItem = msg.Param as SItemData;
			MsgData_BattleShop_buy buyInfo = new MsgData_BattleShop_buy(sItem, BuyingEquipType.eShop, curShopInfo);
			this.DoBuy(buyInfo);
		}

		private void OnMsg_BattleShop_clickBuy(MobaMessage msg)
		{
			ShopInfo curShopInfo = ModelManager.Instance.Get_BattleShop_openShop();
			SItemData sItem = ModelManager.Instance.Get_BattleShop_curSItem();
			MsgData_BattleShop_buy buyInfo = new MsgData_BattleShop_buy(sItem, BuyingEquipType.eShop, curShopInfo);
			this.DoBuy(buyInfo);
		}

		private void OnMsg_BattleShop_clickRItem(MobaMessage msg)
		{
			RItemData rItemData = msg.Param as RItemData;
			ShopInfo curShopInfo;
			if (!BattleEquipTools_op.IsPlayerAlive())
			{
				curShopInfo = rItemData.GetShopByType(BattleEquipTools_op.GetShopTypeByTeamType());
			}
			else
			{
				curShopInfo = rItemData.GetAvailableShop();
			}
			MsgData_BattleShop_buy buyInfo = new MsgData_BattleShop_buy(rItemData, BuyingEquipType.eRecommend, curShopInfo);
			this.DoBuy(buyInfo);
		}

		private void OnMsg_BattleShop_clickSell(MobaMessage msg)
		{
			ShopInfo curShopInfo = ModelManager.Instance.Get_BattleShop_openShop();
			ItemInfo item = ModelManager.Instance.Get_BattleShop_curPItem();
			this.DoSell(new MsgData_BattleShop_sell(item, curShopInfo));
		}

		private void OnMsg_BattleShop_clickRollback(MobaMessage msg)
		{
			ShopInfo curShopInfo = ModelManager.Instance.Get_BattleShop_openShop();
			this.DoRollback(new MsgData_BattleShop_rollback(curShopInfo));
		}

		private void OnMsg_BattleShop_playerAliveChanged(MobaMessage msg)
		{
			this.RefreshUI_CloseShopView();
		}

		private void OnMsg_BattleShop_inShopAreaChanged(MobaMessage msg)
		{
			bool flag = (bool)msg.Param;
			if (flag)
			{
				AudioMgr.PlayUI("Play_Shop_Entry", null, false, false);
			}
			else
			{
				AudioMgr.PlayUI("Play_Shop_Leave", null, false, false);
			}
			ShopInfo shopInfo = ModelManager.Instance.Get_BattleShop_openShop();
			if (shopInfo != null)
			{
				this.RefreshUI_CloseShopView();
			}
		}

		private void OnMsg_BattleShop_waitServerMsgTimeOut(MobaMessage msg)
		{
			EBattleShopState eBattleShopState = (EBattleShopState)((int)msg.Param);
			if (eBattleShopState != EBattleShopState.eIdle)
			{
				string message = "服务器消息未返回，等待超时 state=" + eBattleShopState.ToString();
				ClientLogger.Error(message);
			}
		}

		private void OnMsg_BattleShop_err(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				string message = msg.Param as string;
				ClientLogger.Info(message);
				Singleton<TipView>.Instance.ShowViewSetText("操作失败", 1f);
			}
		}

		private void OnMsg_C2P_RevertShop(MobaMessage msg)
		{
			this.DoUpdateWallet();
		}

		private void OnMsg_C2P_BuyItem(MobaMessage msg)
		{
			this.DoUpdateWallet();
		}

		private void OnMsg_C2P_SellItem(MobaMessage msg)
		{
			this.DoUpdateWallet();
		}

		private void OnMainPlayerHitted()
		{
			if (Singleton<BattleEquipmentView>.Instance.IsOpened)
			{
				ShopInfo shopInfo = ModelManager.Instance.Get_BattleShop_openShop();
				if (shopInfo != null)
				{
					EBattleShopType shopType = shopInfo.ShopType;
					if (shopType == EBattleShopType.eNeutral || shopType == EBattleShopType.eNeutral_2)
					{
						this.CloseView_ShopView();
					}
				}
			}
		}

		private void DoRollback(MsgData_BattleShop_rollback rollbackInfo)
		{
			ShopInfo shopInfo = null;
			string text = null;
			if (rollbackInfo == null || rollbackInfo.curShop == null)
			{
				text = BattleEquipTools_config.Notice_outOfShoppingArea;
			}
			else
			{
				shopInfo = rollbackInfo.curShop;
				EBattleShopState state = shopInfo.State;
				if (state != EBattleShopState.eIdle)
				{
					text = BattleEquipTools_config.Notice_ShopBusy;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				MsgData_BattleShop_onOP param = new MsgData_BattleShop_onOP
				{
					op = EBattleShopOP.eRevert,
					shopType = shopInfo.ShopType
				};
				MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_onOP, param, false);
				if (BattleEquipTools_op.IsOnLineBattle())
				{
					BattleEquipTools_op.DoPvpRollback();
				}
				else
				{
					Units player = MapManager.Instance.GetPlayer();
					BattleEquipTools_op.DoPveRollback(shopInfo, player);
				}
			}
			else if (!text.Equals(BattleEquipTools_config.Notice_ShopBusy))
			{
				Singleton<TipView>.Instance.ShowViewSetText(text, 1f);
			}
		}

		private void DoBuy(MsgData_BattleShop_buy buyInfo)
		{
			string text = string.Empty;
			ShopInfo shopInfo = null;
			bool flag = ModelManager.Instance.Get_BattleShop_playerAlive();
			int num = ModelManager.Instance.Get_BattleShop_money();
			List<ItemInfo> list = ModelManager.Instance.Get_BattleShop_pItems();
			string text2 = null;
			if (buyInfo == null || !buyInfo.Valid)
			{
				text2 = BattleEquipTools_config.Notice_outOfShoppingArea;
			}
			else
			{
				shopInfo = buyInfo.CurShop;
				EBattleShopState state = shopInfo.State;
				if (state != EBattleShopState.eIdle)
				{
					text2 = BattleEquipTools_config.Notice_ShopBusy;
				}
				else if (!ModelManager.Instance.Get_BattleShop_brawlCanBuy())
				{
					text2 = BattleEquipTools_config.Notice_brawl;
				}
				else if (num < buyInfo.RealPrice)
				{
					text2 = BattleEquipTools_config.Notice_DeficientMoney;
				}
				else if (list.Count >= 6 && !buyInfo.Cheaper)
				{
					text2 = BattleEquipTools_config.Notice_DeficientSpace;
				}
				else if (flag && !shopInfo.InArea)
				{
					text2 = BattleEquipTools_config.Notice_outOfShoppingArea;
				}
				else
				{
					text = buyInfo.TargetID;
				}
			}
			if (text2 == null)
			{
				AudioMgr.PlayUI("Play_Shop_Buy", null, false, false);
				MsgData_BattleShop_onOP param = new MsgData_BattleShop_onOP
				{
					op = EBattleShopOP.eBuy,
					shopType = shopInfo.ShopType,
					targetItem = text,
					realPrice = buyInfo.RealPrice
				};
				MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_onOP, param, false);
				if (BattleEquipTools_op.IsOnLineBattle())
				{
					BattleEquipTools_op.DoPvpBuy(shopInfo, text);
				}
				else
				{
					Units player = MapManager.Instance.GetPlayer();
					BattleEquipTools_op.DoPveBuy(shopInfo, player, text, list, buyInfo.RealPrice);
				}
			}
			else if (!text2.Equals(BattleEquipTools_config.Notice_ShopBusy))
			{
				Singleton<TipView>.Instance.ShowViewSetText(text2, 1f);
			}
		}

		private void DoSell(MsgData_BattleShop_sell sellInfo)
		{
			string text = null;
			if (sellInfo == null || sellInfo.curShop == null)
			{
				text = BattleEquipTools_config.Notice_SystemError;
			}
			else if (sellInfo.targetItem == null)
			{
				text = BattleEquipTools_config.Notice_SystemError;
			}
			else
			{
				EBattleShopState state = sellInfo.curShop.State;
				if (state != EBattleShopState.eIdle)
				{
					text = BattleEquipTools_config.Notice_ShopBusy;
				}
			}
			if (text == null)
			{
				ItemInfo targetItem = sellInfo.targetItem;
				MsgData_BattleShop_onOP param = new MsgData_BattleShop_onOP
				{
					op = EBattleShopOP.eSell,
					shopType = sellInfo.curShop.ShopType,
					targetItem = targetItem.ID,
					itemInfo = targetItem
				};
				MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_onOP, param, false);
				List<string> recommendItems = ModelManager.Instance.Get_BattleShop_rItems();
				if (BattleEquipTools_op.IsOnLineBattle())
				{
					BattleEquipTools_op.DoPvpSell(sellInfo.curShop, targetItem, recommendItems);
				}
				else
				{
					Units player = MapManager.Instance.GetPlayer();
					List<ItemInfo> possessItemsP = ModelManager.Instance.Get_BattleShop_pItems();
					BattleEquipTools_op.DoPveSell(sellInfo.curShop, player, targetItem, possessItemsP);
				}
			}
			else if (!text.Equals(BattleEquipTools_config.Notice_ShopBusy))
			{
				Singleton<TipView>.Instance.ShowViewSetText(text, 1f);
			}
		}

		private void OpenBattleShop(EBattleShopType type, EBattleShopOpenType openType)
		{
			Units player = MapManager.Instance.GetPlayer();
			ShopInfo shopInfo = ModelManager.Instance.Get_BattleShop_shopInfo(type);
			string text;
			if (BattleEquipTools_op.CanOpenBattleShop(player, shopInfo, openType, out text))
			{
				AudioMgr.PlayUI("Play_Shop_Open", null, false, false);
				MsgData_BattleShop_onOP param = new MsgData_BattleShop_onOP
				{
					shopType = type
				};
				MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_onOP, param, false);
				CtrlManager.OpenWindow(WindowID.BattleEquipmentView, null);
			}
			else if (!string.IsNullOrEmpty(text))
			{
				Singleton<TipView>.Instance.ShowViewSetText(text, 1f);
			}
		}

		private void RefreshUI_CloseShopView()
		{
			if (Singleton<BattleEquipmentView>.Instance.IsOpened)
			{
				bool flag = ModelManager.Instance.Get_BattleShop_playerAlive();
				ShopInfo shopInfo = ModelManager.Instance.Get_BattleShop_openShop();
				bool inArea = shopInfo.InArea;
				EBattleShopType shopType = shopInfo.ShopType;
				if ((shopType == EBattleShopType.eNeutral || shopType == EBattleShopType.eNeutral_2) && (!flag || !inArea))
				{
					this.CloseView_ShopView();
				}
			}
		}

		private void CloseView_ShopView()
		{
			MsgData_BattleShop_onOP param = new MsgData_BattleShop_onOP
			{
				op = EBattleShopOP.eClose
			};
			MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_onOP, param, false);
			CtrlManager.CloseWindow(WindowID.BattleEquipmentView);
		}

		private void CloseView_BagView()
		{
			CtrlManager.CloseWindow(WindowID.ShowEquipmentPanelView);
		}

		[DebuggerHidden]
		private IEnumerator UpdateWallet()
		{
			BattleShopCtrl.<UpdateWallet>c__IteratorFC <UpdateWallet>c__IteratorFC = new BattleShopCtrl.<UpdateWallet>c__IteratorFC();
			<UpdateWallet>c__IteratorFC.<>f__this = this;
			return <UpdateWallet>c__IteratorFC;
		}

		private void DoUpdateWallet()
		{
			Units player = MapManager.Instance.GetPlayer();
			if (null != player)
			{
				int goldById = UtilManager.Instance.GetGoldById(player.unique_id);
				MobaMessageManagerTools.BattleShop_setMoney(goldById);
			}
		}

		[DebuggerHidden]
		private IEnumerator Check()
		{
			BattleShopCtrl.<Check>c__IteratorFD <Check>c__IteratorFD = new BattleShopCtrl.<Check>c__IteratorFD();
			<Check>c__IteratorFD.<>f__this = this;
			return <Check>c__IteratorFD;
		}

		private void CheckPlayerLive()
		{
			Units player = MapManager.Instance.GetPlayer();
			MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_setPlayerAlive, null != player && player.isLive, true);
		}

		private void CheckDistance()
		{
			Dictionary<EBattleShopType, bool> dictionary = new Dictionary<EBattleShopType, bool>(default(EnumEqualityComparer<EBattleShopType>));
			Units player = MapManager.Instance.GetPlayer();
			Dictionary<EBattleShopType, ShopInfo> dictionary2 = ModelManager.Instance.Get_BattleShop_shops();
			Dictionary<EBattleShopType, ShopInfo>.Enumerator enumerator = dictionary2.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<EBattleShopType, ShopInfo> current = enumerator.Current;
				float num;
				bool value = BattleEquipTools_op.IsWithInShopArea(current.Value.Config, player, out num);
				Dictionary<EBattleShopType, bool> arg_69_0 = dictionary;
				KeyValuePair<EBattleShopType, ShopInfo> current2 = enumerator.Current;
				arg_69_0.Add(current2.Key, value);
			}
			MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_setInShopArea, dictionary, true);
		}
	}
}
