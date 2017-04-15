using Assets.Scripts.Model;
using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public class BattleEquip_Right : MonoBehaviour
	{
		private Transform trans_anchor;

		private Transform btn_rollback;

		private Transform btn_sell;

		private Transform btn_buy;

		private UISprite sp_rollbackText_t;

		private UISprite sp_rollbackText_f;

		private UISprite sp_sellText_t;

		private UISprite sp_sellText_f;

		private UISprite sp_buyText_t;

		private UISprite sp_buyText_f;

		private UILabel label_attributes;

		private UILabel label_introduction;

		private BattleEquip_tipItem com_shopItem;

		private ItemInfo curPosseessItem;

		private SItemData curShopItem;

		private IBEItem tItem;

		private int wallet;

		private bool playerAlive;

		private bool withDistance;

		private int dealCounter;

		private bool brawlCanBuy = true;

		private EBattleShopState shopState;

		private object[] msgs;

		private void Awake()
		{
			this.InitUI();
			this.msgs = new object[]
			{
				ClientC2V.BattleShop_curShopItemChanged,
				ClientC2V.BattleShop_curPossessItemChanged,
				ClientC2V.BattleShop_TItemChanged,
				ClientC2V.BattleShop_walletChanged,
				ClientC2V.BattleShop_inShopAreaChanged,
				ClientC2V.BattleShop_playerAliveChanged,
				ClientC2V.BattleShop_dealCounter,
				ClientC2V.BattleShop_shopStateChanged,
				ClientC2V.BattleShop_brawlCanBuyChanged
			};
		}

		private void Start()
		{
		}

		private void OnDestroy()
		{
		}

		private void OnEnable()
		{
			this.Register();
			this.InitData();
			this.RefreshUI_tip();
			this.RefreshUI_btnBuy();
			this.RefreshUI_btnSell();
			this.RefreshUI_btnRollback();
		}

		private void OnDisable()
		{
			this.Unregister();
		}

		private void Register()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, true, "OnMsg_");
		}

		private void Unregister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, false, "OnMsg_");
		}

		private void InitUI()
		{
			this.trans_anchor = base.transform.FindChild("Anchor");
			this.label_attributes = base.transform.FindChild("Anchor/Nature").GetComponent<UILabel>();
			this.label_introduction = base.transform.FindChild("Anchor/Introduction").GetComponent<UILabel>();
			this.btn_rollback = base.transform.FindChild("Anchor/CancelBtn");
			this.btn_sell = base.transform.FindChild("Anchor/SellBtn");
			this.btn_buy = base.transform.FindChild("Anchor/BuyBtn");
			this.sp_rollbackText_t = base.transform.FindChild("Anchor/CancelBtn/t_text").GetComponent<UISprite>();
			this.sp_rollbackText_f = base.transform.FindChild("Anchor/CancelBtn/f_text").GetComponent<UISprite>();
			this.sp_sellText_t = base.transform.FindChild("Anchor/SellBtn/t_text").GetComponent<UISprite>();
			this.sp_sellText_f = base.transform.FindChild("Anchor/SellBtn/f_text").GetComponent<UISprite>();
			this.sp_buyText_t = base.transform.FindChild("Anchor/BuyBtn/t_text").GetComponent<UISprite>();
			this.sp_buyText_f = base.transform.FindChild("Anchor/BuyBtn/f_text").GetComponent<UISprite>();
			Transform transform = base.transform.FindChild("Anchor/BE_tipItem");
			this.com_shopItem = transform.GetComponent<BattleEquip_tipItem>();
			UIEventListener.Get(this.btn_rollback.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickRollBack);
			UIEventListener.Get(this.btn_sell.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickSell);
			UIEventListener.Get(this.btn_buy.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickBuy);
		}

		private void InitData()
		{
			this.curPosseessItem = ModelManager.Instance.Get_BattleShop_curPItem();
			this.curShopItem = ModelManager.Instance.Get_BattleShop_curSItem();
			this.tItem = ModelManager.Instance.Get_BattleShop_curTItem();
			this.wallet = ModelManager.Instance.Get_BattleShop_money();
			this.playerAlive = ModelManager.Instance.Get_BattleShop_playerAlive();
			ShopInfo shopInfo = ModelManager.Instance.Get_BattleShop_openShop();
			this.brawlCanBuy = ModelManager.Instance.Get_BattleShop_brawlCanBuy();
			if (shopInfo != null)
			{
				this.withDistance = shopInfo.InArea;
				this.dealCounter = shopInfo.DealCounter;
				this.shopState = shopInfo.State;
			}
		}

		private void OnClickBuy(GameObject go)
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.BattleShop_clickBuy, null, false);
		}

		private void OnClickSell(GameObject go)
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.BattleShop_clickSell, null, false);
		}

		private void OnClickRollBack(GameObject go)
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.BattleShop_clickRollback, null, false);
		}

		private void OnMsg_BattleShop_curShopItemChanged(MobaMessage msg)
		{
			this.curShopItem = (msg.Param as SItemData);
			this.RefreshUI_btnBuy();
		}

		private void OnMsg_BattleShop_curPossessItemChanged(MobaMessage msg)
		{
			this.curPosseessItem = (ItemInfo)msg.Param;
			this.RefreshUI_btnSell();
		}

		private void OnMsg_BattleShop_TItemChanged(MobaMessage msg)
		{
			this.tItem = (IBEItem)msg.Param;
			this.RefreshUI_tip();
		}

		private void OnMsg_BattleShop_walletChanged(MobaMessage msg)
		{
			int num = (int)msg.Param;
			if (this.wallet != num)
			{
				this.wallet = num;
			}
		}

		private void OnMsg_BattleShop_inShopAreaChanged(MobaMessage msg)
		{
			ShopInfo shopInfo = ModelManager.Instance.Get_BattleShop_openShop();
			if (shopInfo != null)
			{
				this.withDistance = shopInfo.InArea;
				this.RefreshUI_btnBuy();
				this.RefreshUI_btnSell();
				this.RefreshUI_btnRollback();
			}
		}

		private void OnMsg_BattleShop_playerAliveChanged(MobaMessage msg)
		{
			bool flag = (bool)msg.Param;
			if (this.playerAlive != flag)
			{
				this.playerAlive = flag;
				this.RefreshUI_btnBuy();
				this.RefreshUI_btnSell();
				this.RefreshUI_btnRollback();
			}
		}

		private void OnMsg_BattleShop_dealCounter(MobaMessage msg)
		{
			int num = (int)msg.Param;
			if (this.dealCounter != num)
			{
				this.dealCounter = num;
				this.RefreshUI_btnRollback();
			}
		}

		private void OnMsg_BattleShop_shopStateChanged(MobaMessage msg)
		{
			EBattleShopState eBattleShopState = (EBattleShopState)((int)msg.Param);
			if (this.shopState != eBattleShopState)
			{
				this.shopState = eBattleShopState;
				this.RefreshUI_btnBuy();
				this.RefreshUI_btnSell();
				this.RefreshUI_btnRollback();
			}
		}

		private void OnMsg_BattleShop_brawlCanBuyChanged(MobaMessage msg)
		{
			this.brawlCanBuy = (bool)msg.Param;
			this.RefreshUI_btnBuy();
		}

		private void RefreshUI_tip()
		{
			bool flag = this.tItem != null;
			this.com_shopItem.gameObject.SetActive(flag);
			this.label_attributes.gameObject.SetActive(flag);
			this.label_introduction.gameObject.SetActive(flag);
			if (flag)
			{
				this.com_shopItem.Item = this.tItem;
				this.label_attributes.text = BattleEquipTools_config.GetAttriDes(this.tItem.Config, "\n", 6);
				this.label_introduction.text = LanguageManager.Instance.GetStringById(this.tItem.Config.describe);
			}
		}

		private void RefreshUI_btnBuy()
		{
			this.btn_buy.gameObject.SetActive(null != this.curShopItem);
			bool flag = (!this.playerAlive || this.withDistance) && this.shopState == EBattleShopState.eIdle && this.brawlCanBuy;
			UIButton component = this.btn_buy.GetComponent<UIButton>();
			component.isEnabled = flag;
			this.sp_buyText_t.SetActive(flag);
			this.sp_buyText_f.SetActive(!flag);
		}

		private void RefreshUI_btnSell()
		{
			this.btn_sell.gameObject.SetActive(this.curPosseessItem != null && !string.IsNullOrEmpty(this.curPosseessItem.ID));
			bool flag = (!this.playerAlive || this.withDistance) && this.shopState == EBattleShopState.eIdle;
			UIButton component = this.btn_sell.GetComponent<UIButton>();
			component.isEnabled = flag;
			this.sp_sellText_t.SetActive(flag);
			this.sp_sellText_f.SetActive(!flag);
		}

		private void RefreshUI_btnRollback()
		{
			this.btn_rollback.gameObject.SetActive(this.dealCounter > 0 && (!this.playerAlive || this.withDistance));
			bool flag = this.shopState == EBattleShopState.eIdle;
			UIButton component = this.btn_rollback.GetComponent<UIButton>();
			component.isEnabled = flag;
			this.sp_rollbackText_t.SetActive(flag);
			this.sp_rollbackText_f.SetActive(!flag);
		}
	}
}
