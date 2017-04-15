using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.PropertyView
{
	public class PropViewPurchase : MonoBehaviour
	{
		private Transform purchase;

		private Transform purchaseBtn;

		private Transform priceTag;

		private PriceTagController priceController;

		private UILabel description;

		private List<GoodsData> list_goodsdata = new List<GoodsData>();

		private List<GoodsData> goodsdata_hero = new List<GoodsData>();

		private List<GoodsData> hero_data = new List<GoodsData>();

		private object[] mgs;

		private string heroNPC;

		private string strSwitch;

		public string HeroNPC
		{
			get
			{
				return this.heroNPC;
			}
		}

		private void Awake()
		{
			this.mgs = new object[]
			{
				ClientV2C.sacriviewChangeHero,
				ClientV2C.propviewChangeToggle
			};
			this.Initialize();
		}

		private void OnEnable()
		{
			this.Register();
		}

		private void OnDisable()
		{
			this.Unregister();
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void Register()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
		}

		private void Unregister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
		}

		private void Initialize()
		{
			this.purchase = base.transform.Find("Panel/Toggle/Purchase");
			this.priceTag = this.purchase.Find("PriceTag");
			this.purchaseBtn = this.purchase.Find("PurchaseBtn");
			this.description = this.purchase.Find("Description").GetComponent<UILabel>();
			UIEventListener.Get(this.purchaseBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.GetHero);
			this.priceController = this.priceTag.GetComponent<PriceTagController>();
		}

		private void OnMsg_sacriviewChangeHero(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				string modelID = string.Empty;
				modelID = (string)msg.Param;
				this.heroNPC = modelID;
				HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(modelID);
				this.purchase.gameObject.SetActive(null == heroInfoData);
				this.ShowPrice(null == heroInfoData);
				SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.heroNPC);
				this.CheckBtnState(heroInfoData == null && heroMainData != null && heroMainData.source != "1");
			}
		}

		private void OnMsg_propviewChangeToggle(MobaMessage msg)
		{
		}

		private void GetHero(GameObject obj)
		{
			if (obj.name == "PurchaseBtn")
			{
				CtrlManager.OpenWindow(WindowID.PurchasePopupView, null);
				Singleton<PurchasePopupView>.Instance.onSuccess.Add(new Callback(this.BuyHero));
				Singleton<PurchasePopupView>.Instance.Show(GoodsSubject.Hero, this.heroNPC, 1, false);
			}
		}

		private void BuyHero()
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.sacriviewChangeHero, this.heroNPC, false);
		}

		private void CheckBtnState(bool isActive)
		{
			if (isActive)
			{
				SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.heroNPC);
				string text = string.Empty;
				string[] array = heroMainData.source.Split(new char[]
				{
					','
				});
				if (array.Count<string>() > 1)
				{
					if (array[0].CompareTo("1") == 0)
					{
						string[] array2 = heroMainData.source.Split(new char[]
						{
							'|'
						});
						if (array2.Length > 1)
						{
							text = LanguageManager.Instance.GetStringById(array2[1]);
						}
					}
					this.description.gameObject.SetActive(true);
					this.description.text = text;
				}
				this.strSwitch = array[0];
			}
			else if (this.description.gameObject.activeInHierarchy)
			{
				this.description.gameObject.SetActive(false);
			}
		}

		private void SwitchToOtherWindow(GameObject obj)
		{
			if (null != obj)
			{
				string text = this.strSwitch;
				switch (text)
				{
				case "2":
					MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemOpenView, null, false);
					break;
				case "3":
					CtrlManager.OpenWindow(WindowID.SignView, null);
					break;
				case "4":
					CtrlManager.OpenWindow(WindowID.TaskView, null);
					break;
				case "5":
					CtrlManager.OpenWindow(WindowID.RankView, null);
					break;
				case "6":
					CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
					break;
				}
			}
		}

		private void ShowPrice(bool isShow)
		{
			if (!isShow)
			{
				return;
			}
			this.priceController.SetData(GoodsSubject.Hero, this.heroNPC, 1, null, null, null);
		}

		private string PriceAll(string name)
		{
			this.list_goodsdata.Clear();
			this.list_goodsdata.AddRange(ModelManager.Instance.Get_ShopGoodsList());
			string result = null;
			this.goodsdata_hero.Clear();
			this.goodsdata_hero.AddRange(this.list_goodsdata.FindAll((GoodsData obj) => obj.Type == 1));
			this.hero_data.Clear();
			if (this.goodsdata_hero != null)
			{
				this.hero_data.AddRange(this.goodsdata_hero.FindAll((GoodsData obj) => obj.ElementId == name));
				if (this.hero_data == null || this.hero_data.Count == 0)
				{
					result = null;
				}
				else
				{
					result = Tools_ParsePrice.StringPrice(this.hero_data);
				}
			}
			return result;
		}
	}
}
