using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaHeros.Pvp;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class PurchasePopupView : BaseView<PurchasePopupView>
	{
		public const byte MONEY_COIN = 1;

		public const byte MONEY_DIAMOND = 2;

		public const byte MONEY_CAP = 9;

		private Transform mBg;

		private Transform mAlternativeAnchor;

		private Transform aIcon;

		private UITexture aIconTexture;

		private UISprite aIconSprite;

		private UILabel aItemTitle;

		private UILabel aItemName;

		private UITable aTable;

		private Transform aItemCounter;

		private Transform aDiamondPay;

		private Transform aCoinPay;

		private Transform aCoupon;

		private UIButton aCoinBtn;

		private UIButton aDiamondBtn;

		private UILabel aCountLabel;

		private GameObject aDiscountTag;

		private Transform mSingleAnchor;

		private UILabel sItemName;

		private UILabel sItemDesc;

		private Transform sIcon;

		private UITexture sIconTexture;

		private UISprite sIconSprite;

		private UITable sTable;

		private Transform sItemCounter;

		private Transform sDiamondPay;

		private Transform sCoinPay;

		private Transform sCoupon;

		private Transform sCapPay;

		private UIButton sConfirmBtn;

		private UILabel sCountLabel;

		private GameObject sDiscountTag;

		private GoodsData mData;

		private Dictionary<byte, float[]> mPriceDict = new Dictionary<byte, float[]>();

		public int quantity = 1;

		private static string rawPriceStr = LanguageManager.Instance.GetStringById("ShopUI_Cost") + ":[s]*[-]";

		private static string mobaPriceStr = LanguageManager.Instance.GetStringById("ShopUI_MobaCost") + ":[fec947]*[-]";

		private static string normalPriceStr = LanguageManager.Instance.GetStringById("ShopUI_SalePrice") + ":[fec947]*[-]";

		private Transform activePriceTag1;

		private Transform activePriceTag2;

		private int? price_payment1;

		private double discount_payment1 = 1.0;

		private int? price_payment2;

		private double discount_payment2 = 1.0;

		private byte payment;

		private byte couponMoneyType;

		private int couponDiscount = 10;

		private DiscountCardData cardData;

		private long coinRecord;

		private long diamondRecord;

		private int capRecord;

		private int offeredMaxNum = 999;

		private CoroutineManager cMgr = new CoroutineManager();

		private Task longPressTask;

		public List<Callback> onSuccess = new List<Callback>();

		private bool isBlockPageJump;

		private bool isCoinShortage;

		private bool isDiamondShortage;

		private bool isCapShortage;

		public GoodsData data
		{
			get
			{
				return this.mData;
			}
		}

		public PurchasePopupView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Shop/PurchasePopupView");
		}

		public override void Init()
		{
			base.Init();
			this.mBg = this.transform.FindChild("Bg");
			this.mAlternativeAnchor = this.transform.FindChild("AlternativeAnchor");
			this.mSingleAnchor = this.transform.FindChild("SingleAnchor");
			this.aItemTitle = this.mAlternativeAnchor.FindChild("ItemName").GetComponent<UILabel>();
			this.aIcon = this.mAlternativeAnchor.FindChild("ItemFrame");
			this.aIconTexture = this.aIcon.FindChild("Texture").GetComponent<UITexture>();
			this.aIconSprite = this.aIcon.FindChild("IconSprite").GetComponent<UISprite>();
			this.aItemName = this.mAlternativeAnchor.FindChild("ItemName/ItemDesc").GetComponent<UILabel>();
			this.aTable = this.mAlternativeAnchor.FindChild("PriceContainer").GetComponent<UITable>();
			this.aDiamondPay = this.aTable.transform.FindChild("Diamond");
			this.aCoinPay = this.aTable.transform.FindChild("Coin");
			this.aCoupon = this.aTable.transform.FindChild("Coupon");
			this.aCoinBtn = this.mAlternativeAnchor.FindChild("CoinConfirmBtn").GetComponent<UIButton>();
			this.aDiamondBtn = this.mAlternativeAnchor.FindChild("DiamondConfirmBtn").GetComponent<UIButton>();
			this.aItemCounter = this.aTable.transform.FindChild("Counter");
			this.aCountLabel = this.aItemCounter.FindChild("Num").GetComponent<UILabel>();
			this.aDiscountTag = this.aIcon.FindChild("Discount").gameObject;
			this.sItemName = this.mSingleAnchor.FindChild("ItemName").GetComponent<UILabel>();
			this.sItemDesc = this.mSingleAnchor.FindChild("ItemName/ItemDesc").GetComponent<UILabel>();
			this.sIcon = this.mSingleAnchor.FindChild("ItemFrame");
			this.sIconTexture = this.sIcon.FindChild("Texture").GetComponent<UITexture>();
			this.sIconSprite = this.sIcon.FindChild("IconSprite").GetComponent<UISprite>();
			this.sTable = this.mSingleAnchor.FindChild("PriceContainer").GetComponent<UITable>();
			this.sItemCounter = this.sTable.transform.FindChild("Counter");
			this.sDiamondPay = this.sTable.transform.FindChild("Diamond");
			this.sCoinPay = this.sTable.transform.FindChild("Coin");
			this.sCoupon = this.sTable.transform.FindChild("Coupon");
			this.sCapPay = this.sTable.transform.FindChild("Cap");
			this.sConfirmBtn = this.mSingleAnchor.FindChild("ConfirmBtn").GetComponent<UIButton>();
			this.sCountLabel = this.sItemCounter.FindChild("Num").GetComponent<UILabel>();
			this.sDiscountTag = this.sIcon.FindChild("Discount").gameObject;
			UIEventListener.Get(this.mAlternativeAnchor.FindChild("backBtn").gameObject).onClick = new UIEventListener.VoidDelegate(this.onClick_Close);
			UIEventListener.Get(this.mSingleAnchor.FindChild("backBtn").gameObject).onClick = new UIEventListener.VoidDelegate(this.onClick_Close);
			UIEventListener.Get(this.aTable.transform.FindChild("Counter/Add").gameObject).onPress = new UIEventListener.BoolDelegate(this.onPress_AddNum);
			UIEventListener.Get(this.aTable.transform.FindChild("Counter/Minus").gameObject).onPress = new UIEventListener.BoolDelegate(this.onPress_MinusNum);
			UIEventListener.Get(this.sTable.transform.FindChild("Counter/Add").gameObject).onPress = new UIEventListener.BoolDelegate(this.onPress_AddNum);
			UIEventListener.Get(this.sTable.transform.FindChild("Counter/Minus").gameObject).onPress = new UIEventListener.BoolDelegate(this.onPress_MinusNum);
			UIEventListener.Get(this.aCoinBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.onClick_ConfirmBtn_Coin);
			UIEventListener.Get(this.aDiamondBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.onClick_ConfirmBtn_Diamond);
			UIEventListener.Get(this.sConfirmBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.onClick_ConfirmBtn);
		}

		public override void HandleAfterOpenView()
		{
			this.onSuccess.Clear();
			this.offeredMaxNum = 999;
			if (this.cMgr == null)
			{
				this.cMgr = new CoroutineManager();
			}
		}

		public override void HandleBeforeCloseView()
		{
			if (this.cMgr != null)
			{
				this.cMgr.StopAllCoroutine();
				this.cMgr = null;
			}
		}

		public override void RegisterUpdateHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.BuyShopGoodsNew, new MobaMessageFunc(this.onBuyItemCallback));
		}

		public override void CancelUpdateHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.BuyShopGoodsNew, new MobaMessageFunc(this.onBuyItemCallback));
		}

		private void onClick_Close(GameObject obj = null)
		{
			CtrlManager.CloseWindow(WindowID.PurchasePopupView);
		}

		[DebuggerHidden]
		private IEnumerator LongPressEvent(int _stepNum)
		{
			PurchasePopupView.<LongPressEvent>c__Iterator179 <LongPressEvent>c__Iterator = new PurchasePopupView.<LongPressEvent>c__Iterator179();
			<LongPressEvent>c__Iterator._stepNum = _stepNum;
			<LongPressEvent>c__Iterator.<$>_stepNum = _stepNum;
			<LongPressEvent>c__Iterator.<>f__this = this;
			return <LongPressEvent>c__Iterator;
		}

		private void onPress_AddNum(GameObject obj, bool isPress)
		{
			if (isPress)
			{
				this.onClick_AddNum(obj);
				this.longPressTask = this.cMgr.StartCoroutine(this.LongPressEvent(1), true);
			}
			else if (this.longPressTask != null)
			{
				this.cMgr.StopCoroutine(this.longPressTask);
			}
		}

		private void onPress_MinusNum(GameObject obj, bool isPress)
		{
			if (isPress)
			{
				this.onClick_MinusNum(obj);
				this.longPressTask = this.cMgr.StartCoroutine(this.LongPressEvent(-1), true);
			}
			else if (this.longPressTask != null)
			{
				this.cMgr.StopCoroutine(this.longPressTask);
			}
		}

		private bool IsMoneyShorage(byte _payment)
		{
			if (_payment == 1)
			{
				return this.isCoinShortage;
			}
			if (_payment != 2)
			{
				return _payment != 9 || this.isCapShortage;
			}
			return this.isDiamondShortage;
		}

		private void CheckCurrencyLimit(Transform _priceTagRoot, UILabel _priceLabel, int _price)
		{
			if (_priceTagRoot == this.sCoinPay || _priceTagRoot == this.aCoinPay)
			{
				this.isCoinShortage = ((long)_price > this.coinRecord);
				if (this.isCoinShortage)
				{
					_priceLabel.text = _priceLabel.text.Replace("fec947", "ff0000");
				}
				else
				{
					_priceLabel.text = _priceLabel.text.Replace("ff0000", "fec947");
				}
			}
			if (_priceTagRoot == this.sDiamondPay || _priceTagRoot == this.aDiamondPay)
			{
				this.isDiamondShortage = ((long)_price > this.diamondRecord);
				if (this.isDiamondShortage)
				{
					_priceLabel.text = _priceLabel.text.Replace("fec947", "ff0000");
				}
				else
				{
					_priceLabel.text = _priceLabel.text.Replace("ff0000", "fec947");
				}
			}
			if (_priceTagRoot == this.sCapPay)
			{
				this.isCapShortage = (_price > this.capRecord);
				if (this.isCapShortage)
				{
					_priceLabel.text = _priceLabel.text.Replace("fec947", "ff0000");
				}
				else
				{
					_priceLabel.text = _priceLabel.text.Replace("ff0000", "fec947");
				}
			}
		}

		private void RefreshPriceTag()
		{
			this.sCountLabel.text = this.quantity.ToString();
			this.aCountLabel.text = this.quantity.ToString();
			if (null != this.activePriceTag1)
			{
				bool activeInHierarchy = this.activePriceTag1.FindChild("Table/RawPrice").gameObject.activeInHierarchy;
				int num = this.price_payment1.Value * this.quantity;
				if (activeInHierarchy)
				{
					this.activePriceTag1.FindChild("Table/RawPrice").GetComponent<UILabel>().text = PurchasePopupView.rawPriceStr.Replace("*", num.ToString());
					this.activePriceTag1.FindChild("Table/MobaPrice").GetComponent<UILabel>().text = PurchasePopupView.mobaPriceStr.Replace("*", Convert.ToInt32((double)num * this.discount_payment1).ToString());
					this.CheckCurrencyLimit(this.activePriceTag1, this.activePriceTag1.FindChild("Table/MobaPrice").GetComponent<UILabel>(), Convert.ToInt32((double)num * this.discount_payment1));
				}
				else
				{
					this.activePriceTag1.FindChild("Table/MobaPrice").GetComponent<UILabel>().text = PurchasePopupView.normalPriceStr.Replace("*", num.ToString());
					this.CheckCurrencyLimit(this.activePriceTag1, this.activePriceTag1.FindChild("Table/MobaPrice").GetComponent<UILabel>(), num);
				}
			}
			if (null != this.activePriceTag2)
			{
				bool activeInHierarchy = this.activePriceTag1.FindChild("Table/RawPrice").gameObject.activeInHierarchy;
				int num = this.price_payment2.Value * this.quantity;
				if (activeInHierarchy)
				{
					this.activePriceTag2.FindChild("Table/RawPrice").GetComponent<UILabel>().text = PurchasePopupView.rawPriceStr.Replace("*", num.ToString());
					this.activePriceTag2.FindChild("Table/MobaPrice").GetComponent<UILabel>().text = PurchasePopupView.mobaPriceStr.Replace("*", Convert.ToInt32((double)num * this.discount_payment2).ToString());
					this.CheckCurrencyLimit(this.activePriceTag2, this.activePriceTag2.FindChild("Table/MobaPrice").GetComponent<UILabel>(), Convert.ToInt32((double)num * this.discount_payment2));
				}
				else
				{
					this.activePriceTag2.FindChild("Table/MobaPrice").GetComponent<UILabel>().text = PurchasePopupView.normalPriceStr.Replace("*", Convert.ToInt32((double)num * this.discount_payment2).ToString());
					this.CheckCurrencyLimit(this.activePriceTag2, this.activePriceTag2.FindChild("Table/MobaPrice").GetComponent<UILabel>(), num);
				}
			}
		}

		private void onClick_AddNum(GameObject obj = null)
		{
			if (this.quantity >= this.mData.BuyMaxNum)
			{
				return;
			}
			this.quantity++;
			AudioMgr.PlayUI("Play_UI_Coin", null, false, false);
			this.RefreshPriceTag();
		}

		private void onClick_MinusNum(GameObject obj = null)
		{
			if (this.quantity <= 1)
			{
				if (this.offeredMaxNum <= 0)
				{
					this.quantity = 1;
				}
				else
				{
					this.quantity = Mathf.Min(this.mData.BuyMaxNum, this.offeredMaxNum);
				}
			}
			else
			{
				this.quantity--;
			}
			AudioMgr.PlayUI("Play_UI_Coin", null, false, false);
			this.RefreshPriceTag();
		}

		private void onClick_ConfirmBtn(GameObject obj = null)
		{
			if (this.IsMoneyShorage(this.payment))
			{
				this.ShowTipWhenMoneyShortage(this.payment);
				return;
			}
			long summonerId = ModelManager.Instance.Get_userData_X().SummonerId;
			long num = 0L;
			if (this.mData.Type == 2)
			{
				SysHeroSkinVo _skinVo = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(this.mData.ElementId);
				if (_skinVo == null)
				{
					return;
				}
				HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_list_X().Find((HeroInfoData obj1) => obj1.ModelId == _skinVo.npc_id);
				if (heroInfoData == null)
				{
					return;
				}
				num = heroInfoData.HeroId;
			}
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("ServerResponse_Waiting"), true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.BuyShopGoodsNew, param, new object[]
			{
				this.payment,
				this.mData.Id,
				this.quantity,
				summonerId,
				num
			});
		}

		private void onClick_ConfirmBtn_Coin(GameObject obj = null)
		{
			if (this.IsMoneyShorage(1))
			{
				this.ShowTipWhenMoneyShortage(1);
				return;
			}
			long summonerId = ModelManager.Instance.Get_userData_X().SummonerId;
			long num = 0L;
			if (this.mData.Type == 2)
			{
				SysHeroSkinVo _skinVo = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(this.mData.ElementId);
				if (_skinVo == null)
				{
					return;
				}
				HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_list_X().Find((HeroInfoData obj1) => obj1.ModelId == _skinVo.npc_id);
				if (heroInfoData == null)
				{
					return;
				}
				num = heroInfoData.HeroId;
			}
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("ServerResponse_Waiting"), true, 15f);
			this.payment = 1;
			SendMsgManager.Instance.SendMsg(MobaGameCode.BuyShopGoodsNew, param, new object[]
			{
				1,
				this.mData.Id,
				this.quantity,
				summonerId,
				num
			});
		}

		private void onClick_ConfirmBtn_Diamond(GameObject obj = null)
		{
			if (this.IsMoneyShorage(2))
			{
				this.ShowTipWhenMoneyShortage(2);
				return;
			}
			long summonerId = ModelManager.Instance.Get_userData_X().SummonerId;
			long num = 0L;
			if (this.mData.Type == 2)
			{
				SysHeroSkinVo _skinVo = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(this.mData.ElementId);
				if (_skinVo == null)
				{
					return;
				}
				HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_list_X().Find((HeroInfoData obj1) => obj1.ModelId == _skinVo.npc_id);
				if (heroInfoData == null)
				{
					return;
				}
				num = heroInfoData.HeroId;
			}
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("ServerResponse_Waiting"), true, 15f);
			this.payment = 2;
			bool flag = SendMsgManager.Instance.SendMsg(MobaGameCode.BuyShopGoodsNew, param, new object[]
			{
				2,
				this.mData.Id,
				this.quantity,
				summonerId,
				num
			});
		}

		private void onBuyItemCallback(MobaMessage msg)
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
				if (mobaErrorCode != MobaErrorCode.MoneyShortage)
				{
					if (mobaErrorCode != MobaErrorCode.BuyPurchaseLimit)
					{
						if (mobaErrorCode != MobaErrorCode.ItemExist)
						{
							Singleton<TipView>.Instance.ShowViewSetText("商店出错：未知错误代码 - " + num.ToString(), 1f);
							ClientLogger.Error(string.Format("[GameShop]Unexpected Error Code: {0}, From User: {1}, Shop Item Id: {2}", num.ToString(), ModelManager.Instance.Get_userData_filed_X("UserId"), this.mData.Id));
							CtrlManager.CloseWindow(WindowID.PurchasePopupView);
						}
						else
						{
							Singleton<TipView>.Instance.ShowViewSetText("已拥有！无需重复购买", 1f);
							CtrlManager.CloseWindow(WindowID.PurchasePopupView);
						}
					}
					else
					{
						string stringById = LanguageManager.Instance.GetStringById("ShopUI_PurchaseTimesToday");
						if (stringById != null)
						{
							Singleton<TipView>.Instance.ShowViewSetText(stringById.Replace("*", this.mData.DayBuyCount.ToString()), 1f);
						}
						else
						{
							Singleton<TipView>.Instance.ShowViewSetText("已达每日购买次数限制，请明天再来。", 1f);
						}
						CtrlManager.CloseWindow(WindowID.PurchasePopupView);
					}
				}
				else
				{
					this.ShowTipWhenMoneyShortage(this.payment);
				}
			}
			else
			{
				Singleton<MenuTopBarView>.Instance.RefreshUI();
				Singleton<MenuView>.Instance.CheckHeroState();
				CharacterDataMgr.instance.UpdateHerosData();
				if (Singleton<ShopView>.Instance.IsOpen || Singleton<HomePayView>.Instance.IsOpen)
				{
					if (!this.CallGetItemsView(operationResponse))
					{
						Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("ShopUI_Tips_PurchaseSuccess"), 1f);
					}
					Singleton<MenuView>.Instance.UpdatePayTable();
				}
				else
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("ShopUI_Tips_PurchaseSuccess"), 1f);
				}
				if (this.onSuccess.Count != 0)
				{
					foreach (Callback current in this.onSuccess)
					{
						current();
					}
				}
				CtrlManager.CloseWindow(WindowID.PurchasePopupView);
			}
		}

		private bool CallGetItemsView(OperationResponse operationResponse)
		{
			switch ((int)operationResponse.Parameters[242])
			{
			case 1:
			{
				HeroInfoData heroInfoData = SerializeHelper.Deserialize<HeroInfoData>(operationResponse.Parameters[88] as byte[]);
				CtrlManager.OpenWindow(WindowID.GetItemsView, null);
				MobaMessageManagerTools.GetItems_Hero(heroInfoData.ModelId);
				Singleton<GetItemsView>.Instance.Play();
				break;
			}
			case 2:
			{
				SummSkinData summSkinData = SerializeHelper.Deserialize<SummSkinData>(operationResponse.Parameters[209] as byte[]);
				CtrlManager.OpenWindow(WindowID.GetItemsView, null);
				MobaMessageManagerTools.GetItems_HeroSkin(summSkinData.SkinId);
				Singleton<GetItemsView>.Instance.Play();
				break;
			}
			case 3:
			{
				EquipmentInfoData equipmentInfoData = SerializeHelper.Deserialize<EquipmentInfoData>(operationResponse.Parameters[90] as byte[]);
				SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(equipmentInfoData.ModelId.ToString());
				if (dataById.type == 4)
				{
					CtrlManager.OpenWindow(WindowID.GetItemsView, null);
					MobaMessageManagerTools.GetItems_Rune(int.Parse(dataById.items_id));
					Singleton<GetItemsView>.Instance.Play();
				}
				else
				{
					if (dataById == null)
					{
						return false;
					}
					CtrlManager.OpenWindow(WindowID.GetItemsView, null);
					MobaMessageManagerTools.GetItems_GameItem(dataById.items_id);
					Singleton<GetItemsView>.Instance.Play();
				}
				break;
			}
			case 4:
			{
				int num = (int)operationResponse.Parameters[61];
				string text = (string)operationResponse.Parameters[11];
				CtrlManager.OpenWindow(WindowID.GetItemsView, null);
				if (text.Equals("1"))
				{
					MobaMessageManagerTools.GetItems_Coin(num);
				}
				else if (text.Equals("11"))
				{
					MobaMessageManagerTools.GetItems_Speaker(num);
				}
				Singleton<GetItemsView>.Instance.Play();
				return true;
			}
			case 5:
			{
				string value = (string)operationResponse.Parameters[11];
				CtrlManager.OpenWindow(WindowID.GetItemsView, null);
				MobaMessageManagerTools.GetItems_HeadPortrait(Convert.ToInt32(value));
				Singleton<GetItemsView>.Instance.Play();
				return true;
			}
			case 6:
			{
				string frameId = (string)operationResponse.Parameters[11];
				CtrlManager.OpenWindow(WindowID.GetItemsView, null);
				MobaMessageManagerTools.GetItems_PortraitFrame(frameId);
				Singleton<GetItemsView>.Instance.Play();
				return true;
			}
			case 7:
			{
				int modelId = (int)operationResponse.Parameters[11];
				CtrlManager.OpenWindow(WindowID.GetItemsView, null);
				MobaMessageManagerTools.GetItems_GameBuff(modelId);
				Singleton<GetItemsView>.Instance.Play();
				return true;
			}
			case 8:
			{
				List<EquipmentInfoData> listEquip = SerializeHelper.Deserialize<List<EquipmentInfoData>>(operationResponse.Parameters[202] as byte[]);
				List<HeroInfoData> listHero = SerializeHelper.Deserialize<List<HeroInfoData>>(operationResponse.Parameters[88] as byte[]);
				List<DropItemData> listDropItem = SerializeHelper.Deserialize<List<DropItemData>>(operationResponse.Parameters[246] as byte[]);
				List<DropItemData> listRepeatItem = SerializeHelper.Deserialize<List<DropItemData>>(operationResponse.Parameters[146] as byte[]);
				ToolsFacade.Instance.GetRewards_WriteInModels(listEquip, listHero, listDropItem, listRepeatItem, null);
				return true;
			}
			}
			return true;
		}

		private void AddCallback(bool isconfirm)
		{
			if (isconfirm)
			{
				if (!Singleton<ShopView>.Instance.IsOpen)
				{
					Singleton<ShopView>.Instance.ThroughShop = ETypicalShop.Recharge;
					CtrlManager.OpenWindow(WindowID.ShopViewNew, null);
					if (Singleton<MenuBottomBarView>.Instance.IsOpen)
					{
						CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
					}
					Singleton<MenuTopBarView>.Instance.SetActiveOrNot(true);
				}
				else
				{
					Singleton<ShopView>.Instance.SetShopType(6);
				}
				this.onClick_Close(null);
			}
		}

		private void SetData_SingleAnchor()
		{
			switch (this.mData.Type)
			{
			case 1:
			{
				SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.mData.ElementId);
				if (heroMainData == null)
				{
					return;
				}
				this.sItemName.text = LanguageManager.Instance.GetStringById("BattleSettlement_Hero");
				this.sItemDesc.text = LanguageManager.Instance.GetStringById(heroMainData.name);
				this.sIcon.FindChild("Sprite").gameObject.SetActive(false);
				this.sIconTexture.width = 320;
				this.sIconTexture.height = 470;
				this.sIconTexture.enabled = true;
				this.sIconSprite.enabled = false;
				if (this.mData.Picture == "[]")
				{
					this.sIconTexture.mainTexture = ResourceManager.Load<Texture>(heroMainData.Loading_icon, true, true, null, 0, false);
				}
				else
				{
					this.sIconTexture.mainTexture = ResourceManager.Load<Texture>(this.mData.Picture, true, true, null, 0, false);
				}
				break;
			}
			case 2:
			{
				SysHeroSkinVo dataById = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(this.mData.ElementId);
				if (dataById == null)
				{
					return;
				}
				this.sItemName.text = LanguageManager.Instance.GetStringById("BattleSettlement_Skin");
				this.sItemDesc.text = LanguageManager.Instance.GetStringById(dataById.name);
				this.sIcon.FindChild("Sprite").gameObject.SetActive(false);
				this.sIconTexture.width = 320;
				this.sIconTexture.height = 470;
				this.sIconTexture.enabled = true;
				this.sIconSprite.enabled = false;
				if (this.mData.Picture == "[]")
				{
					this.sIconTexture.mainTexture = ResourceManager.Load<Texture>(dataById.Loading_icon, true, true, null, 0, false);
				}
				else
				{
					this.sIconTexture.mainTexture = ResourceManager.Load<Texture>(this.mData.Picture, true, true, null, 0, false);
				}
				break;
			}
			case 3:
			{
				SysGameItemsVo dataById2 = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(this.mData.ElementId);
				if (dataById2 == null)
				{
					return;
				}
				this.sItemName.text = LanguageManager.Instance.GetStringById(dataById2.name);
				if (dataById2.attribute == "[]")
				{
					this.sItemDesc.text = LanguageManager.Instance.GetStringById(dataById2.role);
				}
				else
				{
					string[] array = dataById2.attribute.Split(new char[]
					{
						'|'
					});
					string text = (!(array[1].Substring(array[1].Length - 1, 1) == "%")) ? float.Parse(array[1]).ToString(array[2]) : array[1];
					if (dataById2.rune_type != 2)
					{
						this.sItemDesc.text = "+" + text + " " + LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName);
					}
					else
					{
						this.sItemDesc.text = string.Concat(new string[]
						{
							"+",
							text,
							" ",
							LanguageManager.Instance.GetStringById("HeroRunsUI_GrowthRuns"),
							LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName)
						});
					}
				}
				if (this.mData.Picture == "[]")
				{
					this.sIcon.FindChild("Sprite").gameObject.SetActive(true);
					this.sIcon.FindChild("Sprite").GetComponent<UISprite>().spriteName = "Settlement_rune_bottom";
					if (dataById2.type == 4)
					{
						this.sIconTexture.enabled = false;
						this.sIconSprite.enabled = true;
						this.sIconSprite.spriteName = dataById2.icon;
					}
					else
					{
						this.sIconSprite.enabled = false;
						this.sIconTexture.enabled = true;
						this.sIconTexture.mainTexture = ResourceManager.Load<Texture>(dataById2.icon, true, true, null, 0, false);
						this.sIconTexture.MakePixelPerfect();
					}
				}
				else
				{
					this.sIcon.FindChild("Sprite").gameObject.SetActive(false);
					this.sIconSprite.enabled = false;
					this.sIconTexture.enabled = true;
					this.sIconTexture.mainTexture = ResourceManager.Load<Texture>(this.mData.Picture, true, true, null, 0, false);
					this.sIconTexture.width = 320;
					this.sIconTexture.height = 470;
				}
				break;
			}
			case 4:
			{
				SysCurrencyVo dataById3 = BaseDataMgr.instance.GetDataById<SysCurrencyVo>(this.mData.ElementId);
				if (dataById3 == null || dataById3.currency_id == 1)
				{
					this.sItemName.text = "货币兑换";
					this.sItemDesc.text = LanguageManager.Instance.GetStringById("Currency_Gold") + " x" + this.mData.Count;
					this.sIcon.FindChild("Sprite").gameObject.SetActive(false);
					this.sIconSprite.enabled = false;
					this.sIconTexture.enabled = true;
					this.sIconTexture.mainTexture = Resources.Load<Texture>("Texture/ShopItem/Shop_Ensure_DiamondExchangeGold");
					this.sIconTexture.width = 320;
					this.sIconTexture.height = 470;
				}
				else if (dataById3.currency_id == 11)
				{
					this.sItemName.text = LanguageManager.Instance.GetStringById(dataById3.name);
					this.sItemDesc.text = LanguageManager.Instance.GetStringById(dataById3.description);
					this.sIcon.FindChild("Sprite").gameObject.SetActive(false);
					this.sIconSprite.enabled = false;
					this.sIconTexture.enabled = true;
					this.sIconTexture.mainTexture = ResourceManager.Load<Texture>("Shop_little_trumpet_card", true, true, null, 0, false);
					this.sIconTexture.width = 320;
					this.sIconTexture.height = 470;
				}
				break;
			}
			case 5:
			{
				SysSummonersHeadportraitVo dataById4 = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(this.mData.ElementId);
				this.sItemName.text = LanguageManager.Instance.GetStringById(dataById4.headportrait_name);
				this.sItemDesc.text = "头像";
				this.sIconSprite.enabled = false;
				this.sIconTexture.enabled = true;
				this.sIcon.FindChild("Sprite").gameObject.SetActive(true);
				this.sIcon.FindChild("Sprite").GetComponent<UISprite>().spriteName = "Shop_get_head_bg";
				this.sIconTexture.mainTexture = ResourceManager.Load<Texture>(dataById4.headportrait_icon, true, true, null, 0, false);
				this.sIconTexture.SetDimensions(168, 168);
				break;
			}
			case 6:
			{
				SysSummonersPictureframeVo dataById5 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(this.mData.ElementId);
				this.sItemName.text = LanguageManager.Instance.GetStringById(dataById5.pictureframe_name);
				this.sItemDesc.text = "头像框";
				this.sIconSprite.enabled = false;
				this.sIconTexture.enabled = true;
				this.sIcon.FindChild("Sprite").gameObject.SetActive(true);
				this.sIcon.FindChild("Sprite").GetComponent<UISprite>().spriteName = "Shop_get_head_bg";
				this.sIconTexture.mainTexture = ResourceManager.Load<Texture>(dataById5.pictureframe_icon, true, true, null, 0, false);
				this.sIconTexture.SetDimensions(218, 218);
				break;
			}
			case 7:
			{
				SysGameBuffVo dataById6 = BaseDataMgr.instance.GetDataById<SysGameBuffVo>(this.mData.ElementId);
				if (dataById6 == null)
				{
					return;
				}
				this.sItemDesc.text = LanguageManager.Instance.GetStringById(dataById6.describe);
				this.sItemName.text = LanguageManager.Instance.GetStringById(dataById6.name);
				this.sIconSprite.enabled = false;
				this.sIconTexture.enabled = true;
				this.sIconTexture.mainTexture = ResourceManager.Load<Texture>(dataById6.long_icon, true, true, null, 0, false);
				this.sIconTexture.width = 320;
				this.sIconTexture.height = 470;
				break;
			}
			case 8:
				if (this.mData.Id == 80000)
				{
					this.sItemName.text = LanguageManager.Instance.GetStringById("NewbiePackage", "萌新特惠套餐");
					this.sItemDesc.text = LanguageManager.Instance.GetStringById("NewbiePackageHero", "包含内容如左图");
				}
				else if (this.mData.Id == 80001)
				{
					this.sItemName.text = LanguageManager.Instance.GetStringById("LuxuriousPackage", "超级豪华套餐");
					this.sItemDesc.text = LanguageManager.Instance.GetStringById("LuxuriousPackageHero", "包含内容如左图");
				}
				else if (this.mData.Id == 80003)
				{
					this.sItemName.text = LanguageManager.Instance.GetStringById("2017Newyear", "春节限定豪华套餐");
					this.sItemDesc.text = LanguageManager.Instance.GetStringById("2017NewyearPackage", "包含内容如左图");
				}
				this.sIconSprite.enabled = false;
				this.sIconTexture.enabled = true;
				this.sIconTexture.mainTexture = ResourceManager.Load<Texture>(this.mData.Picture, true, true, null, 0, false);
				this.sIconTexture.width = 320;
				this.sIconTexture.height = 470;
				break;
			}
		}

		private void SetData_AlternativeAnchor()
		{
			switch (this.mData.Type)
			{
			case 1:
			{
				SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.mData.ElementId);
				if (heroMainData == null)
				{
					return;
				}
				this.aItemTitle.text = LanguageManager.Instance.GetStringById("BattleSettlement_Hero");
				this.aItemName.text = LanguageManager.Instance.GetStringById(heroMainData.name);
				this.aIcon.FindChild("Sprite").gameObject.SetActive(false);
				this.aIconTexture.width = 320;
				this.aIconTexture.height = 470;
				this.aIconTexture.enabled = true;
				this.aIconSprite.enabled = false;
				if (this.mData.Picture == "[]")
				{
					this.aIconTexture.mainTexture = ResourceManager.Load<Texture>(heroMainData.Loading_icon, true, true, null, 0, false);
				}
				else
				{
					this.aIconTexture.mainTexture = ResourceManager.Load<Texture>(this.mData.Picture, true, true, null, 0, false);
				}
				break;
			}
			case 2:
			{
				SysHeroSkinVo dataById = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(this.mData.ElementId);
				if (dataById == null)
				{
					return;
				}
				this.aItemTitle.text = LanguageManager.Instance.GetStringById("BattleSettlement_Skin");
				this.aItemName.text = LanguageManager.Instance.GetStringById(dataById.name);
				this.aIcon.FindChild("Sprite").gameObject.SetActive(false);
				this.aIconTexture.width = 320;
				this.aIconTexture.height = 470;
				this.aIconTexture.enabled = true;
				this.aIconSprite.enabled = false;
				if (this.mData.Picture == "[]")
				{
					this.aIconTexture.mainTexture = ResourceManager.Load<Texture>(dataById.Loading_icon, true, true, null, 0, false);
				}
				else
				{
					this.aIconTexture.mainTexture = ResourceManager.Load<Texture>(this.mData.Picture, true, true, null, 0, false);
				}
				break;
			}
			case 3:
			{
				SysGameItemsVo dataById2 = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(this.mData.ElementId);
				if (dataById2 == null)
				{
					return;
				}
				this.aItemTitle.text = LanguageManager.Instance.GetStringById(dataById2.name);
				if (dataById2.attribute == "[]")
				{
					this.aItemName.text = LanguageManager.Instance.GetStringById(dataById2.describe);
				}
				else
				{
					string[] array = dataById2.attribute.Split(new char[]
					{
						'|'
					});
					string text = (!(array[1].Substring(array[1].Length - 1, 1) == "%")) ? float.Parse(array[1]).ToString(array[2]) : array[1];
					if (dataById2.rune_type != 2)
					{
						this.aItemName.text = "+" + text + " " + LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName);
					}
					else
					{
						this.aItemName.text = string.Concat(new string[]
						{
							"+",
							text,
							" ",
							LanguageManager.Instance.GetStringById("HeroRunsUI_GrowthRuns"),
							LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName)
						});
					}
				}
				this.aIcon.FindChild("Sprite").gameObject.SetActive(true);
				if (dataById2.type == 4)
				{
					this.aIconTexture.enabled = false;
					this.aIconSprite.enabled = true;
					this.aIconSprite.spriteName = dataById2.icon;
				}
				else
				{
					this.aIconSprite.enabled = false;
					this.aIconTexture.enabled = true;
					this.aIconTexture.mainTexture = ResourceManager.Load<Texture>(dataById2.icon, true, true, null, 0, false);
					this.aIconTexture.width = 320;
					this.aIconTexture.height = 470;
				}
				break;
			}
			case 5:
			{
				SysSummonersHeadportraitVo dataById3 = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(this.mData.ElementId);
				this.aItemName.text = LanguageManager.Instance.GetStringById(dataById3.headportrait_name);
				this.aItemTitle.text = "头像";
				this.aIconSprite.enabled = false;
				this.aIconTexture.enabled = true;
				this.aIcon.FindChild("Sprite").gameObject.SetActive(true);
				this.aIcon.FindChild("Sprite").GetComponent<UISprite>().spriteName = "Shop_get_head_bg";
				this.aIconTexture.mainTexture = ResourceManager.Load<Texture>(dataById3.headportrait_icon, true, true, null, 0, false);
				this.aIconTexture.SetDimensions(168, 168);
				break;
			}
			case 6:
			{
				SysSummonersPictureframeVo dataById4 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(this.mData.ElementId);
				this.aItemName.text = LanguageManager.Instance.GetStringById(dataById4.pictureframe_name);
				this.aItemTitle.text = "头像框";
				this.aIconSprite.enabled = false;
				this.aIconTexture.enabled = true;
				this.aIcon.FindChild("Sprite").gameObject.SetActive(true);
				this.aIcon.FindChild("Sprite").GetComponent<UISprite>().spriteName = "Shop_get_head_bg";
				this.aIconTexture.mainTexture = ResourceManager.Load<Texture>(dataById4.pictureframe_icon, true, true, null, 0, false);
				this.aIconTexture.SetDimensions(218, 218);
				break;
			}
			case 7:
			{
				SysGameBuffVo dataById5 = BaseDataMgr.instance.GetDataById<SysGameBuffVo>(this.mData.ElementId);
				if (dataById5 == null)
				{
					return;
				}
				this.aItemName.text = LanguageManager.Instance.GetStringById(dataById5.describe);
				this.aItemTitle.text = LanguageManager.Instance.GetStringById(dataById5.name);
				this.aIconSprite.enabled = false;
				this.aIconTexture.enabled = true;
				this.aIconTexture.mainTexture = ResourceManager.Load<Texture>(dataById5.long_icon, true, true, null, 0, false);
				this.aIconTexture.width = 320;
				this.aIconTexture.height = 470;
				break;
			}
			}
		}

		private void SetPriceTag_SingleAnchor()
		{
			bool flag = this.mData.BuyMaxNum > 1;
			this.sItemCounter.gameObject.SetActive(flag);
			if (flag)
			{
				this.sCountLabel.text = "1";
			}
			this.quantity = 1;
			bool flag2 = this.mPriceDict.ContainsKey(1);
			bool flag3 = this.mPriceDict.ContainsKey(2);
			bool flag4 = this.mPriceDict.ContainsKey(9);
			byte b = (!flag3) ? ((!flag4) ? 1 : 9) : 2;
			float num = 0f;
			float[] array = null;
			if (!this.mPriceDict.TryGetValue(b, out array))
			{
				Singleton<TipView>.Instance.ShowViewSetText("未知错误", 1f);
				ClientLogger.Error("PurchasePopupView: 未知错误-1");
				return;
			}
			this.price_payment1 = new int?((int)array[0]);
			this.price_payment2 = null;
			this.discount_payment1 = (double)(array[1] / array[0]);
			this.discount_payment2 = 1.0;
			this.payment = b;
			byte b2 = b;
			if (b2 != 1)
			{
				if (b2 != 2)
				{
					if (b2 == 9)
					{
						this.sCoinPay.gameObject.SetActive(false);
						this.sDiamondPay.gameObject.SetActive(false);
						this.sCapPay.gameObject.SetActive(true);
						this.activePriceTag1 = this.sCapPay;
						this.activePriceTag2 = null;
						num = (float)ModelManager.Instance.Get_userData_X().SmallCap;
					}
				}
				else
				{
					this.sCoinPay.gameObject.SetActive(false);
					this.sDiamondPay.gameObject.SetActive(true);
					this.sCapPay.gameObject.SetActive(false);
					this.activePriceTag1 = this.sDiamondPay;
					this.activePriceTag2 = null;
					num = (float)ModelManager.Instance.Get_userData_X().Diamonds;
				}
			}
			else
			{
				this.sCoinPay.gameObject.SetActive(true);
				this.sDiamondPay.gameObject.SetActive(false);
				this.sCapPay.gameObject.SetActive(false);
				this.activePriceTag1 = this.sCoinPay;
				this.activePriceTag2 = null;
				num = (float)ModelManager.Instance.Get_userData_X().Money;
			}
			if (this.discount_payment1 < 1.0)
			{
				if (this.activePriceTag1 != null)
				{
					this.activePriceTag1.FindChild("Table/RawPrice").gameObject.SetActive(true);
					this.activePriceTag1.FindChild("Table/RawPrice").GetComponent<UILabel>().text = PurchasePopupView.rawPriceStr.Replace("*", array[0].ToString("F0"));
					this.activePriceTag1.FindChild("Table/MobaPrice").GetComponent<UILabel>().text = PurchasePopupView.mobaPriceStr.Replace("*", array[1].ToString("F0"));
				}
				this.activePriceTag1.FindChild("Table").GetComponent<UITable>().Reposition();
				this.CheckCurrencyLimit(this.activePriceTag1, this.activePriceTag1.FindChild("Table/MobaPrice").GetComponent<UILabel>(), Convert.ToInt32(array[1]));
				this.offeredMaxNum = (int)Mathf.Floor(num / array[1]);
			}
			else
			{
				if (this.activePriceTag1 != null)
				{
					this.activePriceTag1.FindChild("Table/RawPrice").gameObject.SetActive(false);
					this.activePriceTag1.FindChild("Table/MobaPrice").GetComponent<UILabel>().text = PurchasePopupView.normalPriceStr.Replace("*", array[0].ToString("F0"));
				}
				this.activePriceTag1.FindChild("Table").GetComponent<UITable>().Reposition();
				this.CheckCurrencyLimit(this.activePriceTag1, this.activePriceTag1.FindChild("Table/MobaPrice").GetComponent<UILabel>(), Convert.ToInt32(array[0]));
				this.offeredMaxNum = (int)Mathf.Floor(num / array[0]);
			}
			if (this.cardData != null)
			{
				this.sCoupon.gameObject.SetActive(true);
				if (this.couponDiscount == 7)
				{
					this.sCoupon.GetComponent<UISprite>().spriteName = "Store_discount_bottom_03";
					this.sCoupon.FindChild("Title").GetComponent<UISprite>().spriteName = "Store_discount_7";
				}
				else if (this.couponDiscount == 3)
				{
					this.sCoupon.GetComponent<UISprite>().spriteName = "Store_discount_bottom_04";
					this.sCoupon.FindChild("Title").GetComponent<UISprite>().spriteName = "Store_discount_3";
				}
				TimeSpan timeSpan = ToolsFacade.Instance.TimeDValueCount(this.cardData.endtime);
				if (timeSpan.Days == 0)
				{
					this.sCoupon.transform.FindChild("Widget/LastTime").GetComponent<UILabel>().text = timeSpan.Hours.ToString() + "小时";
				}
				else if (timeSpan.Days >= 1)
				{
					this.sCoupon.transform.FindChild("Widget/LastTime").GetComponent<UILabel>().text = timeSpan.Days.ToString() + "天";
				}
				if (this.couponMoneyType == 1)
				{
					this.sCoupon.transform.FindChild("Title/Label").GetComponent<UILabel>().text = "金币折扣券";
				}
				else if (this.couponMoneyType == 2)
				{
					this.sCoupon.transform.FindChild("Title/Label").GetComponent<UILabel>().text = "钻石折扣券";
				}
				else if (this.couponMoneyType == 9)
				{
					this.sCoupon.transform.FindChild("Title/Label").GetComponent<UILabel>().text = "瓶盖折扣券";
				}
			}
		}

		private void SetPriceTag_AlternativeAnchor()
		{
			bool flag = this.mData.BuyMaxNum > 1;
			this.aItemCounter.gameObject.SetActive(flag);
			if (flag)
			{
				this.aCountLabel.text = "1";
			}
			this.quantity = 1;
			int num = Convert.ToInt32(this.mPriceDict[1][0]);
			double num2 = Convert.ToDouble(this.mPriceDict[1][1] / this.mPriceDict[1][0]);
			int num3 = Convert.ToInt32(this.mPriceDict[2][0]);
			double num4 = Convert.ToDouble(this.mPriceDict[2][1] / this.mPriceDict[2][0]);
			this.price_payment1 = new int?(num);
			this.discount_payment1 = num2;
			this.price_payment2 = new int?(num3);
			this.discount_payment2 = num4;
			this.activePriceTag1 = this.aCoinPay;
			this.activePriceTag2 = this.aDiamondPay;
			if (num2 < 1.0)
			{
				if (this.activePriceTag1 != null)
				{
					this.activePriceTag1.FindChild("Table/RawPrice").gameObject.SetActive(true);
					this.activePriceTag1.FindChild("Table/RawPrice").GetComponent<UILabel>().text = PurchasePopupView.rawPriceStr.Replace("*", num.ToString());
					this.activePriceTag1.FindChild("Table/MobaPrice").GetComponent<UILabel>().text = PurchasePopupView.mobaPriceStr.Replace("*", Convert.ToInt32((double)num * num2).ToString());
				}
				this.activePriceTag1.FindChild("Table").GetComponent<UITable>().Reposition();
				this.CheckCurrencyLimit(this.activePriceTag1, this.activePriceTag1.FindChild("Table/MobaPrice").GetComponent<UILabel>(), Convert.ToInt32((double)num * num2));
			}
			else
			{
				if (this.activePriceTag1 != null)
				{
					this.activePriceTag1.FindChild("Table/RawPrice").gameObject.SetActive(false);
					this.activePriceTag1.FindChild("Table/MobaPrice").GetComponent<UILabel>().text = PurchasePopupView.normalPriceStr.Replace("*", num.ToString());
				}
				this.activePriceTag1.FindChild("Table").GetComponent<UITable>().Reposition();
				this.CheckCurrencyLimit(this.activePriceTag1, this.activePriceTag1.FindChild("Table/MobaPrice").GetComponent<UILabel>(), num);
			}
			if (num4 < 1.0)
			{
				if (this.activePriceTag2 != null)
				{
					this.activePriceTag2.FindChild("Table/RawPrice").gameObject.SetActive(true);
					this.activePriceTag2.FindChild("Table/RawPrice").GetComponent<UILabel>().text = PurchasePopupView.rawPriceStr.Replace("*", num3.ToString());
					this.activePriceTag2.FindChild("Table/MobaPrice").GetComponent<UILabel>().text = PurchasePopupView.mobaPriceStr.Replace("*", Convert.ToInt32((double)num3 * num4).ToString());
				}
				this.activePriceTag2.FindChild("Table").GetComponent<UITable>().Reposition();
				this.CheckCurrencyLimit(this.activePriceTag2, this.activePriceTag2.FindChild("Table/MobaPrice").GetComponent<UILabel>(), Convert.ToInt32((double)num3 * num4));
				float num5 = (float)ModelManager.Instance.Get_userData_X().Diamonds;
				this.offeredMaxNum = (int)Mathf.Floor(num5 / (float)((double)num * num2));
			}
			else
			{
				if (this.activePriceTag2 != null)
				{
					this.activePriceTag2.FindChild("Table/RawPrice").gameObject.SetActive(false);
					this.activePriceTag2.FindChild("Table/MobaPrice").GetComponent<UILabel>().text = PurchasePopupView.normalPriceStr.Replace("*", num3.ToString());
				}
				this.activePriceTag2.FindChild("Table").GetComponent<UITable>().Reposition();
				this.CheckCurrencyLimit(this.activePriceTag2, this.activePriceTag2.FindChild("Table/MobaPrice").GetComponent<UILabel>(), num3);
				float num5 = (float)ModelManager.Instance.Get_userData_X().Diamonds;
				this.offeredMaxNum = (int)Mathf.Floor(num5 / (float)num);
			}
			if (this.cardData != null)
			{
				this.aCoupon.gameObject.SetActive(true);
				if (this.couponDiscount == 7)
				{
					this.aCoupon.GetComponent<UISprite>().spriteName = "Store_discount_bottom_03";
					this.aCoupon.FindChild("Title").GetComponent<UISprite>().spriteName = "Store_discount_7";
				}
				else if (this.couponDiscount == 3)
				{
					this.aCoupon.GetComponent<UISprite>().spriteName = "Store_discount_bottom_04";
					this.aCoupon.FindChild("Title").GetComponent<UISprite>().spriteName = "Store_discount_3";
				}
				TimeSpan timeSpan = ToolsFacade.Instance.TimeDValueCount(this.cardData.endtime);
				if (timeSpan.Days == 0)
				{
					this.aCoupon.transform.FindChild("Widget/LastTime").GetComponent<UILabel>().text = timeSpan.Hours.ToString() + "小时";
				}
				else if (timeSpan.Days >= 1)
				{
					this.aCoupon.transform.FindChild("Widget/LastTime").GetComponent<UILabel>().text = timeSpan.Days.ToString() + "天";
				}
				if (this.couponMoneyType == 1)
				{
					this.aCoupon.transform.FindChild("Title/Label").GetComponent<UILabel>().text = "金币折扣券";
				}
				else if (this.couponMoneyType == 2)
				{
					this.aCoupon.transform.FindChild("Title/Label").GetComponent<UILabel>().text = "钻石折扣券";
				}
				else if (this.couponMoneyType == 9)
				{
					this.aCoupon.transform.FindChild("Title/Label").GetComponent<UILabel>().text = "瓶盖折扣券";
				}
			}
		}

		private void SetRepeat()
		{
			Singleton<TipView>.Instance.ShowViewSetText("已拥有", 1f);
			this.onClick_Close(null);
		}

		private void SetDiscount(float _discount)
		{
			this.sDiscountTag.SetActive(true);
			this.aDiscountTag.SetActive(true);
			this.sDiscountTag.transform.FindChild("Label").GetComponent<UILabel>().text = (_discount * 10f).ToString("F0");
			this.aDiscountTag.transform.FindChild("Label").GetComponent<UILabel>().text = (_discount * 10f).ToString("F0");
		}

		private void SetCoupon(byte _moneyType, int _coupon, DiscountCardData _cardData)
		{
			this.sCoupon.gameObject.SetActive(true);
			this.aCoupon.gameObject.SetActive(true);
			this.couponMoneyType = _moneyType;
			this.couponDiscount = _coupon;
			this.cardData = _cardData;
		}

		private void RecordCurrencyValue()
		{
			this.coinRecord = ModelManager.Instance.Get_userData_filed_X("Money");
			this.diamondRecord = ModelManager.Instance.Get_userData_filed_X("Diamonds");
			this.capRecord = ModelManager.Instance.Get_userData_filed_X("SmallCap");
		}

		private void CallPaySDK(GoodsData _data, float _price)
		{
			int id = _data.Id;
			int count = _data.Count;
			InitSDK.instance.StartSDKPay((int)_price, id.ToString(), count);
		}

		private void InitData(GoodsData _data)
		{
			if (_data == null)
			{
				Singleton<TipView>.Instance.ShowViewSetText("错误：该物品不售卖。", 1f);
				this.onClick_Close(null);
				return;
			}
			if (_data.Type == 1 && PvpMatchMgr.State != PvpMatchState.None)
			{
				Singleton<TipView>.Instance.ShowViewSetText("匹配对战时无法购买新英雄", 1f);
				this.onClick_Close(null);
				return;
			}
			this.sDiscountTag.SetActive(false);
			this.aDiscountTag.SetActive(false);
			this.sCoupon.gameObject.SetActive(false);
			this.aCoupon.gameObject.SetActive(false);
			this.mData = _data;
			this.cardData = null;
			this.mPriceDict = ModelManager.Instance.Get_ShopGoodsPrice(this.mData, new Callback(this.SetRepeat), new Callback<float>(this.SetDiscount), new Callback<byte, int, DiscountCardData>(this.SetCoupon));
			if (this.mPriceDict == null || (this.mPriceDict.ContainsKey(0) && this.mPriceDict[0] != null))
			{
				Singleton<TipView>.Instance.ShowViewSetText("发生了未知错误", 1f);
				ClientLogger.Error("拿到了一个空的价格，goodId:" + this.mData.Id);
				return;
			}
			if (this.mPriceDict.ContainsKey(10) && this.mPriceDict[10].Length >= 2)
			{
				float price = this.mPriceDict[10][1];
				this.CallPaySDK(_data, price);
				this.onClick_Close(null);
				return;
			}
			this.RecordCurrencyValue();
			this.mBg.gameObject.SetActive(true);
			if (this.mPriceDict.ContainsKey(100) && this.mPriceDict[100][0] < 2f)
			{
				this.mSingleAnchor.gameObject.SetActive(true);
				this.mAlternativeAnchor.gameObject.SetActive(false);
				this.SetData_SingleAnchor();
				this.SetPriceTag_SingleAnchor();
			}
			else
			{
				this.mSingleAnchor.gameObject.SetActive(false);
				this.mAlternativeAnchor.gameObject.SetActive(true);
				this.SetData_AlternativeAnchor();
				this.SetPriceTag_AlternativeAnchor();
			}
		}

		private void ShowTipWhenMoneyShortage(byte _payment)
		{
			byte b = this.payment;
			if (b != 1)
			{
				if (b != 2)
				{
					if (b != 9)
					{
						Singleton<TipView>.Instance.ShowViewSetText("货币数量不足！", 1f);
					}
					else if (this.isBlockPageJump)
					{
						Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("MagicBottle_Title_NotEnough"), 1f);
					}
					else
					{
						CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("MagicBottle_Title_NotEnough"), LanguageManager.Instance.GetStringById("MagicBottle_Content_NotEnough"), delegate
						{
						}, PopViewType.PopOneButton, "确定", "取消", null);
					}
				}
				else if (this.isBlockPageJump)
				{
					Singleton<TipView>.Instance.ShowViewSetText("钻石不足无法购买", 1f);
				}
				else
				{
					CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("Recharge_Title_DiamondRecharge"), LanguageManager.Instance.GetStringById("Recharge_Content_DiamondRecharge"), new Action<bool>(this.AddCallback), PopViewType.PopTwoButton, "确定", "取消", null);
				}
			}
			else if (this.isBlockPageJump)
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("Gold_Title_NotEnough"), 1f);
			}
			else
			{
				CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("Gold_Title_NotEnough"), LanguageManager.Instance.GetStringById("Gold_Content_NotEnough"), delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
			}
		}

		public void Show(ETypicalCommodity _id, bool _isBlockPageJump = false)
		{
			this.isBlockPageJump = _isBlockPageJump;
			int _idInt = (int)_id;
			this.InitData(ModelManager.Instance.Get_ShopGoodsList().Find((GoodsData obj) => obj.Id == _idInt));
		}

		public void Show(GoodsData _data, bool _isBlockPageJump = false)
		{
			this.isBlockPageJump = _isBlockPageJump;
			this.InitData(_data);
		}

		public void Show(GoodsSubject _type, string _itemId, int _itemNum, bool _isBlockPageJump = false)
		{
			this.isBlockPageJump = _isBlockPageJump;
			this.InitData(ModelManager.Instance.Get_ShopGoodsList().Find((GoodsData obj) => obj.Type == (int)_type && obj.ElementId == _itemId && obj.Count == _itemNum));
		}
	}
}
