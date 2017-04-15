using Assets.Scripts.GUILogic.View.PropertyView;
using Com.Game.Data;
using Com.Game.Manager;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class ShopItemController : MonoBehaviour
	{
		public UIToggle _discountToggle;

		public UIToggle _newToggle;

		public UIToggle _singlePicToggle;

		public UITable _bottomTable;

		public UISprite _goodsBg;

		public UISprite _propBgPic;

		public UITexture _propPic;

		public UISprite _runePic;

		public UITexture _singlePic;

		public Transform _buyWidget;

		public Transform _buyBtnWidget;

		public Transform _goldenBuyBtnWidget;

		public Transform _ownWidget;

		public Transform _descWidget;

		public Transform _nameWidget;

		public Transform _discountWidget;

		public PriceTagController _priceTagCtrl;

		public GameObject _buyBtn;

		public UILabel _itemName;

		public UILabel _itemDescription;

		public UILabel _discountLabel;

		public GameObject _3_coupon;

		public GameObject _7_coupon;

		public GameObject _altarJumpBtn;

		private bool isOwned;

		private GoodsData mData;

		private CoroutineManager cMgr = new CoroutineManager();

		public static Color32 Green = new Color32(20, 150, 42, 255);

		public static Color32 Blue = new Color32(21, 131, 220, 255);

		public static Color32 Purple = new Color32(193, 10, 239, 255);

		public static Color32 Yellow = new Color32(223, 147, 0, 255);

		public static Color32 Red = new Color32(255, 50, 28, 255);

		public int cost
		{
			get;
			private set;
		}

		public GoodsData data
		{
			get
			{
				return this.mData;
			}
		}

		private void Awake()
		{
			UIEventListener.Get(this._buyBtn).onClick = new UIEventListener.VoidDelegate(this.onClick_Buy);
			UIEventListener.Get(this._altarJumpBtn).onClick = new UIEventListener.VoidDelegate(this.onClick_AltarJump);
		}

		private void OnDestroy()
		{
			this.cMgr.StopAllCoroutine();
		}

		[DebuggerHidden]
		private IEnumerator AsyncLoadPic(UITexture _comp, string _resPath, bool isLoadByResId, bool makePixelPerfect = false)
		{
			ShopItemController.<AsyncLoadPic>c__Iterator17A <AsyncLoadPic>c__Iterator17A = new ShopItemController.<AsyncLoadPic>c__Iterator17A();
			<AsyncLoadPic>c__Iterator17A.isLoadByResId = isLoadByResId;
			<AsyncLoadPic>c__Iterator17A._resPath = _resPath;
			<AsyncLoadPic>c__Iterator17A._comp = _comp;
			<AsyncLoadPic>c__Iterator17A.makePixelPerfect = makePixelPerfect;
			<AsyncLoadPic>c__Iterator17A.<$>isLoadByResId = isLoadByResId;
			<AsyncLoadPic>c__Iterator17A.<$>_resPath = _resPath;
			<AsyncLoadPic>c__Iterator17A.<$>_comp = _comp;
			<AsyncLoadPic>c__Iterator17A.<$>makePixelPerfect = makePixelPerfect;
			return <AsyncLoadPic>c__Iterator17A;
		}

		private void SetItemInfo()
		{
			switch (this.mData.Type)
			{
			case 1:
			{
				this._altarJumpBtn.SetActive(true);
				SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.mData.ElementId);
				if (heroMainData == null)
				{
					return;
				}
				this._nameWidget.gameObject.SetActive(true);
				this._descWidget.gameObject.SetActive(false);
				this._itemName.text = LanguageManager.Instance.GetStringById(heroMainData.name);
				this._itemName.color = Color.white;
				this._itemName.effectColor = new Color32(1, 28, 64, 255);
				this._singlePicToggle.value = true;
				if (this.mData.Picture == "[]")
				{
					this.cMgr.StartCoroutine(this.AsyncLoadPic(this._singlePic, heroMainData.Loading_icon, true, false), true);
				}
				else
				{
					this.cMgr.StartCoroutine(this.AsyncLoadPic(this._singlePic, this.mData.Picture, true, false), true);
					this._singlePic.width = 320;
					this._singlePic.height = 470;
				}
				break;
			}
			case 2:
			{
				this._altarJumpBtn.SetActive(true);
				SysHeroSkinVo dataById = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(this.mData.ElementId);
				if (dataById == null)
				{
					return;
				}
				this._nameWidget.gameObject.SetActive(true);
				this._descWidget.gameObject.SetActive(false);
				this._itemName.text = LanguageManager.Instance.GetStringById(dataById.name);
				this._itemName.color = Color.white;
				this._itemName.effectColor = new Color32(1, 28, 64, 255);
				this._singlePicToggle.value = true;
				if (this.mData.Picture == "[]")
				{
					this.cMgr.StartCoroutine(this.AsyncLoadPic(this._singlePic, dataById.Loading_icon, true, false), true);
				}
				else
				{
					this.cMgr.StartCoroutine(this.AsyncLoadPic(this._singlePic, this.mData.Picture, true, false), true);
					this._singlePic.width = 320;
					this._singlePic.height = 470;
				}
				break;
			}
			case 3:
			{
				this._altarJumpBtn.SetActive(false);
				SysGameItemsVo dataById2 = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(this.mData.ElementId);
				if (dataById2 == null)
				{
					return;
				}
				this._nameWidget.gameObject.SetActive(true);
				this._itemName.text = LanguageManager.Instance.GetStringById(dataById2.name);
				this._itemName.color = this.GetColorByQuality(dataById2.quality);
				this._itemName.effectColor = Color.black;
				this._propPic.transform.localPosition = new Vector3(0f, 96f, 0f);
				if (dataById2.attribute == "[]")
				{
					this._descWidget.gameObject.SetActive(false);
				}
				else
				{
					this._descWidget.gameObject.SetActive(true);
					string[] array = dataById2.attribute.Split(new char[]
					{
						'|'
					});
					string text = (!(array[1].Substring(array[1].Length - 1, 1) == "%")) ? float.Parse(array[1]).ToString(array[2]) : array[1];
					if (dataById2.rune_type != 2)
					{
						this._itemDescription.text = "+" + text + " " + LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName);
					}
					else
					{
						this._itemDescription.text = string.Concat(new string[]
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
					this._singlePicToggle.value = false;
					this._propBgPic.spriteName = "Store_bottom_" + dataById2.quality.ToString("D2");
					if (dataById2.type == 4)
					{
						this._propPic.gameObject.SetActive(false);
						this._runePic.enabled = true;
						this._runePic.spriteName = dataById2.icon;
					}
					else
					{
						this._propPic.gameObject.SetActive(true);
						this._runePic.enabled = false;
						this._propPic.transform.GetChild(0).gameObject.SetActive(false);
						this.cMgr.StartCoroutine(this.AsyncLoadPic(this._propPic, dataById2.icon, true, true), true);
					}
				}
				else
				{
					this._singlePicToggle.value = true;
					this.cMgr.StartCoroutine(this.AsyncLoadPic(this._singlePic, this.mData.Picture, true, false), true);
				}
				break;
			}
			case 4:
			{
				this._altarJumpBtn.SetActive(false);
				SysCurrencyVo dataById3 = BaseDataMgr.instance.GetDataById<SysCurrencyVo>(this.mData.ElementId);
				if (dataById3 == null)
				{
					return;
				}
				this._nameWidget.gameObject.SetActive(true);
				this._descWidget.gameObject.SetActive(false);
				this._itemName.color = Color.white;
				this._itemName.effectColor = Color.black;
				if (dataById3.currency_id == 1)
				{
					this._itemName.text = LanguageManager.Instance.GetStringById(dataById3.name) + " x" + this.mData.Count;
					this._singlePicToggle.value = false;
					this._propBgPic.spriteName = "Store_bottom_gold";
					this._propPic.gameObject.SetActive(true);
					this._runePic.enabled = false;
					this._propPic.transform.GetChild(0).gameObject.SetActive(false);
					this._propPic.transform.localPosition = new Vector3(0f, 72f, 0f);
					this.cMgr.StartCoroutine(this.AsyncLoadPic(this._propPic, "Texture/ShopITem/Shop_DiamondExchangeGold", false, true), true);
				}
				else if (dataById3.currency_id == 2)
				{
					this._itemName.text = LanguageManager.Instance.GetStringById(dataById3.name) + " x" + this.mData.Count;
					this._singlePicToggle.value = true;
					this._runePic.enabled = false;
					this._propPic.transform.GetChild(0).gameObject.SetActive(false);
					this._propPic.transform.localPosition = new Vector3(0f, 0f, 0f);
					this.cMgr.StartCoroutine(this.AsyncLoadPic(this._singlePic, this.mData.Picture, true, false), true);
					this._propPic.width = 320;
					this._propPic.height = 470;
				}
				else if (dataById3.currency_id == 11)
				{
					this._itemName.text = LanguageManager.Instance.GetStringById(dataById3.name);
					this._itemName.color = this.GetColorByQuality(dataById3.quality);
					this._singlePicToggle.value = true;
					this._runePic.enabled = false;
					this.cMgr.StartCoroutine(this.AsyncLoadPic(this._singlePic, this.mData.Picture, true, false), true);
					this._singlePic.width = 320;
					this._singlePic.height = 470;
				}
				break;
			}
			case 5:
			{
				this._altarJumpBtn.SetActive(false);
				SysSummonersHeadportraitVo dataById4 = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(this.mData.ElementId);
				if (dataById4 == null)
				{
					return;
				}
				this._nameWidget.gameObject.SetActive(true);
				this._itemName.text = LanguageManager.Instance.GetStringById(dataById4.headportrait_name);
				this._itemName.color = this.GetColorByQuality(dataById4.headportrait_quality);
				this._itemName.effectColor = Color.black;
				this._propPic.transform.localPosition = new Vector3(0f, 72f, 0f);
				this._descWidget.gameObject.SetActive(false);
				if (this.mData.Picture == "[]")
				{
					this._singlePicToggle.value = false;
					this._propBgPic.spriteName = "Store_bottom_" + dataById4.headportrait_quality.ToString("D2");
					this._propPic.gameObject.SetActive(true);
					this._runePic.enabled = false;
					this.cMgr.StartCoroutine(this.AsyncLoadPic(this._propPic, dataById4.headportrait_icon, true, false), true);
					this._propPic.transform.GetChild(0).gameObject.SetActive(true);
					this._propPic.width = 168;
					this._propPic.height = 168;
				}
				else
				{
					this._singlePicToggle.value = true;
					this.cMgr.StartCoroutine(this.AsyncLoadPic(this._singlePic, this.mData.Picture, true, false), true);
					this._singlePic.SetDimensions(320, 470);
				}
				break;
			}
			case 6:
			{
				this._altarJumpBtn.SetActive(false);
				SysSummonersPictureframeVo dataById5 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(this.mData.ElementId);
				if (dataById5 == null)
				{
					return;
				}
				this._nameWidget.gameObject.SetActive(true);
				this._itemName.text = LanguageManager.Instance.GetStringById(dataById5.pictureframe_name);
				this._itemName.color = this.GetColorByQuality(dataById5.pictureframe_quality);
				this._itemName.effectColor = Color.black;
				this._propPic.transform.localPosition = new Vector3(0f, 72f, 0f);
				this._descWidget.gameObject.SetActive(false);
				if (this.mData.Picture == "[]")
				{
					this._singlePicToggle.value = false;
					this._propBgPic.spriteName = "Store_bottom_" + dataById5.pictureframe_quality.ToString("D2");
					this._propPic.gameObject.SetActive(true);
					this._runePic.enabled = false;
					this.cMgr.StartCoroutine(this.AsyncLoadPic(this._propPic, dataById5.pictureframe_icon, true, false), true);
					this._propPic.transform.GetChild(0).gameObject.SetActive(false);
					this._propPic.SetDimensions(218, 218);
				}
				else
				{
					this._singlePicToggle.value = true;
					this.cMgr.StartCoroutine(this.AsyncLoadPic(this._singlePic, this.mData.Picture, true, false), true);
					this._singlePic.SetDimensions(320, 470);
				}
				break;
			}
			case 7:
			{
				this._altarJumpBtn.SetActive(false);
				SysGameBuffVo dataById6 = BaseDataMgr.instance.GetDataById<SysGameBuffVo>(this.mData.ElementId);
				if (dataById6 == null)
				{
					return;
				}
				this._nameWidget.gameObject.SetActive(true);
				this._descWidget.gameObject.SetActive(false);
				this._itemName.text = LanguageManager.Instance.GetStringById(dataById6.name);
				this._itemName.color = this.GetColorByQuality(dataById6.quality);
				this._itemName.effectColor = Color.black;
				if (this.mData.Picture == "[]")
				{
					this._singlePicToggle.value = true;
					this.cMgr.StartCoroutine(this.AsyncLoadPic(this._singlePic, dataById6.long_icon, true, false), true);
				}
				else
				{
					this._singlePicToggle.value = true;
					this.cMgr.StartCoroutine(this.AsyncLoadPic(this._singlePic, this.mData.Picture, true, false), true);
				}
				break;
			}
			}
		}

		private Color32 GetColorByQuality(int _quality)
		{
			switch (_quality)
			{
			case 1:
				return ShopItemController.Green;
			case 2:
				return ShopItemController.Blue;
			case 3:
				return ShopItemController.Purple;
			case 4:
				return ShopItemController.Yellow;
			case 5:
				return ShopItemController.Red;
			default:
				return Color.white;
			}
		}

		private void onClick_AltarJump(GameObject obj = null)
		{
			Singleton<ShopView>.Instance.SetBackShop();
			Singleton<ShopView>.Instance.ThroughShopItem = this.mData.Id;
			if (this.mData.Type == 1)
			{
				CtrlManager.OpenWindow(WindowID.PropertyView, null);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.sacriviewChangeHero, this.mData.ElementId, false);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewInitToggle, PropertyType.Info, false);
			}
			else if (this.mData.Type == 2)
			{
				SysHeroSkinVo dataById = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(this.mData.ElementId);
				if (dataById != null)
				{
					CtrlManager.OpenWindow(WindowID.PropertyView, null);
					MobaMessageManagerTools.SendClientMsg(ClientV2C.sacriviewChangeHero, dataById.npc_id, false);
					MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewInitToggle, PropertyType.Info, false);
				}
			}
		}

		private void onClick_Buy(GameObject obj = null)
		{
			AudioMgr.PlayUI("Play_Menu_click", null, false, false);
			if (this._ownWidget.gameObject.activeInHierarchy && this.mData.Type != 1)
			{
				return;
			}
			if (this._ownWidget.gameObject.activeInHierarchy && this.mData.Type == 1)
			{
				CtrlManager.OpenWindow(WindowID.PropertyView, null);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.sacriviewChangeHero, this.mData.ElementId, false);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewInitToggle, PropertyType.Info, false);
				return;
			}
			if (this.mData.Type == 2)
			{
				SysHeroSkinVo _skinVo = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(this.mData.ElementId);
				if (CharacterDataMgr.instance.OwenHeros.Find((string obj1) => obj1 == _skinVo.npc_id) == null)
				{
					CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("ShopUI_Title_HeroUnable"), LanguageManager.Instance.GetStringById("ShopUI_Tips_HeroUnable"), new Action<bool>(this.BuyHeroFirst), PopViewType.PopTwoButton, "确定", "取消", null);
					return;
				}
			}
			CtrlManager.OpenWindow(WindowID.PurchasePopupView, null);
			if (this.mData.IsSingle == 1)
			{
				Singleton<PurchasePopupView>.Instance.onSuccess.Add(new Callback(this.RefreshItem));
			}
			Singleton<PurchasePopupView>.Instance.Show(this.mData, false);
		}

		private void BuyHeroFirst(bool isConfirm)
		{
			if (isConfirm)
			{
				SysHeroSkinVo dataById = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(this.mData.ElementId);
				CtrlManager.OpenWindow(WindowID.PurchasePopupView, null);
				Singleton<PurchasePopupView>.Instance.Show(GoodsSubject.Hero, dataById.npc_id, 1, false);
			}
		}

		private void RefreshItem()
		{
			Singleton<ShopView>.Instance.CheckStatic();
			this.SetData(this.mData);
		}

		public void SetData(GoodsData _data)
		{
			this.mData = _data;
			this.SetPriceTag(_data);
			this._newToggle.value = (_data.Mark != 0);
			this.SetItemInfo();
			this._bottomTable.Reposition();
		}

		private void SetPriceTag(GoodsData _data)
		{
			this._buyWidget.gameObject.SetActive(true);
			this._buyBtnWidget.gameObject.SetActive(true);
			this._goldenBuyBtnWidget.gameObject.SetActive(false);
			this._3_coupon.SetActive(false);
			this._7_coupon.SetActive(false);
			this._ownWidget.gameObject.SetActive(false);
			this._discountToggle.value = false;
			this._discountWidget.gameObject.SetActive(false);
			this._goodsBg.color = new Color32(19, 138, 163, 255);
			this.isOwned = false;
			this._priceTagCtrl.SetData(_data.Id, new Callback(this.SetOwned), new Callback<float>(this.SetDiscount), new Callback<byte, int, DiscountCardData>(this.SetCoupon));
		}

		private void SetOwned()
		{
			this.isOwned = true;
			this._ownWidget.gameObject.SetActive(true);
			this._buyWidget.gameObject.SetActive(false);
			this._discountWidget.gameObject.SetActive(false);
			this._discountToggle.value = false;
		}

		private void SetDiscount(float _discount)
		{
			if (this.isOwned)
			{
				return;
			}
			this._discountToggle.value = true;
			this._discountWidget.gameObject.SetActive(true);
			this._discountLabel.text = (_discount * 10f).ToString("F0");
		}

		private void SetCoupon(byte _moneyType, int _couponDiscount, DiscountCardData _cardData)
		{
			if (this.isOwned)
			{
				return;
			}
			this._discountToggle.value = false;
			this._goldenBuyBtnWidget.gameObject.SetActive(true);
			this._buyBtnWidget.gameObject.SetActive(false);
			this._goodsBg.color = new Color32(255, 198, 0, 255);
			GameObject gameObject = null;
			if (_couponDiscount == 7)
			{
				gameObject = this._7_coupon;
			}
			else if (_couponDiscount == 3)
			{
				gameObject = this._3_coupon;
			}
			if (gameObject == null)
			{
				return;
			}
			gameObject.SetActive(true);
			TimeSpan timeSpan = ToolsFacade.Instance.TimeDValueCount(_cardData.endtime);
			if (timeSpan.Days == 0)
			{
				gameObject.transform.FindChild("LastTime").GetComponent<UILabel>().text = timeSpan.Hours.ToString() + "小时";
			}
			else if (timeSpan.Days >= 1)
			{
				gameObject.transform.FindChild("LastTime").GetComponent<UILabel>().text = timeSpan.Days.ToString() + "天";
			}
			if (_moneyType == 1)
			{
				gameObject.transform.FindChild("Tip").GetComponent<UILabel>().text = "金币折扣券";
			}
			else if (_moneyType == 2)
			{
				gameObject.transform.FindChild("Tip").GetComponent<UILabel>().text = "钻石折扣券";
			}
			else if (_moneyType == 9)
			{
				gameObject.transform.FindChild("Tip").GetComponent<UILabel>().text = "瓶盖折扣券";
			}
		}

		public void ClearResources()
		{
			if (this._propPic != null && this._propPic.mainTexture != null)
			{
				this._propPic.mainTexture = null;
			}
			if (this._singlePic != null && this._singlePic.mainTexture != null)
			{
				this._singlePic.mainTexture = null;
			}
		}
	}
}
