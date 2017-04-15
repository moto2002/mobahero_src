using Assets.Scripts.Model;
using Com.Game.Module;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.PropertyView
{
	public class PropViewSkin : MonoBehaviour
	{
		private Transform tranSkinPanel;

		private Transform tranSkinParent;

		private object[] mgs;

		private string heroNPC = string.Empty;

		private long heroID;

		private int skinID;

		private PropertyType toggleType;

		public string HeroNPC
		{
			get
			{
				return this.heroNPC;
			}
		}

		public long HeroID
		{
			get
			{
				return this.heroID;
			}
		}

		public int SkinID
		{
			get
			{
				return this.skinID;
			}
		}

		private void Awake()
		{
			this.mgs = new object[]
			{
				ClientV2C.sacriviewChangeHero,
				ClientV2C.propviewChangeToggle,
				ClientC2V.SkinWear,
				MobaGameCode.GetSummSkinList
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
			this.tranSkinPanel = base.transform.Find("SkinPanel");
			this.tranSkinParent = this.tranSkinPanel.Find("SkinParent");
			this.toggleType = PropertyType.Other;
		}

		private void OnMsg_propviewChangeToggle(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				PropertyType propertyType = (PropertyType)((int)msg.Param);
				this.toggleType = propertyType;
				this.ShowSkinPanel(this.heroNPC, this.toggleType == PropertyType.Info);
				this.tranSkinPanel.gameObject.SetActive(propertyType == PropertyType.Info);
			}
		}

		private void OnMsg_sacriviewChangeHero(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				string modelID = string.Empty;
				modelID = (string)msg.Param;
				this.heroNPC = modelID;
				HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(modelID);
				this.heroID = ((heroInfoData != null) ? heroInfoData.HeroId : 0L);
				this.skinID = ((heroInfoData != null) ? heroInfoData.CurrSkin : 0);
				this.ShowSkinPanel(this.heroNPC, this.toggleType == PropertyType.Info);
			}
		}

		private void OnMsg_SkinWear(MobaMessage msg)
		{
			if (msg.Param == null || null == this.tranSkinParent)
			{
				return;
			}
			int num = (int)msg.Param;
			if (this.skinID == num)
			{
				SkinPanel componentInChildren = this.tranSkinParent.gameObject.GetComponentInChildren<SkinPanel>();
				SkinPanel.IsWearSkin(this.heroID, this.skinID);
				componentInChildren.SetWearBtnState(this.heroID, this.skinID);
			}
		}

		private void OnMsg_GetSummSkinList(MobaMessage msg)
		{
			SkinPanel componentInChildren = this.tranSkinParent.gameObject.GetComponentInChildren<SkinPanel>();
			if (componentInChildren != null && !string.IsNullOrEmpty(this.heroNPC))
			{
				componentInChildren.RefreshUISkinPanel(this.heroNPC);
				componentInChildren.ReFreshPrice(this.heroID, this.skinID);
			}
			SkinPanel.IsWearSkin(this.heroID, this.skinID);
		}

		private void ShowSkinPanel(string heroName, bool isShow = true)
		{
			if (!isShow)
			{
				return;
			}
			SkinPanel[] componentsInChildren = this.tranSkinParent.gameObject.GetComponentsInChildren<SkinPanel>(true);
			SkinPanel skinPanel;
			if (componentsInChildren == null || componentsInChildren.Length == 0)
			{
				skinPanel = SkinPanel.genSkinPanel(this.tranSkinParent);
				skinPanel.transform.localScale = new Vector3(0.77f, 0.77f, 1f);
			}
			else
			{
				skinPanel = componentsInChildren[0];
			}
			skinPanel.setHeroName(heroName, this.heroID, this.skinID);
			skinPanel.onSkinChanged -= new Action<int>(this.OnSkinChanged);
			skinPanel.onSkinChanged += new Action<int>(this.OnSkinChanged);
			skinPanel.onBuyBtnEvent -= new Action<int>(this.OnBuySkinEvent);
			skinPanel.onBuyBtnEvent += new Action<int>(this.OnBuySkinEvent);
			skinPanel.onWearBtnEvent -= new Action<Transform, bool>(this.OnWearEvent);
			skinPanel.onWearBtnEvent += new Action<Transform, bool>(this.OnWearEvent);
			Transform transform = this.tranSkinParent.Find("skinPanel(Clone)/Warn");
			if (null != transform)
			{
				transform.gameObject.SetActive(false);
			}
		}

		private void SendWearMsg()
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewReplaceSkin, this.skinID, false);
		}

		private void SendSkinMsg()
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewBuySkinSuccess, null, false);
		}

		private void OnSkinChanged(int id)
		{
			if (id != this.skinID)
			{
				SkinPanel componentInChildren = this.tranSkinParent.gameObject.GetComponentInChildren<SkinPanel>();
				MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewChangeSkin, id, false);
				this.skinID = id;
				componentInChildren.SetWearBtnState(this.heroID, this.skinID);
				componentInChildren.SetBuyBtnState(this.skinID);
				componentInChildren.ReFreshPrice(this.heroID, this.skinID);
			}
		}

		private void OnBuySkinEvent(int id)
		{
			if (CharacterDataMgr.instance.OwenHeros.Contains(this.heroNPC))
			{
				CtrlManager.OpenWindow(WindowID.PurchasePopupView, null);
				Singleton<PurchasePopupView>.Instance.onSuccess.Add(new Callback(this.BuySkinCallBack));
				Singleton<PurchasePopupView>.Instance.Show(GoodsSubject.Skin, id.ToString(), 1, false);
			}
			else
			{
				string text = string.Empty;
				text = LanguageManager.Instance.GetStringById("HeroAltar_BeforeSkin");
				Singleton<TipView>.Instance.ShowViewSetText(text, 1f);
			}
		}

		private void OnWearEvent(Transform obj = null, bool iswear = false)
		{
			if (null != obj.gameObject)
			{
				obj.gameObject.SetActive(iswear);
			}
			this.SendWearMsg();
		}

		private void BuySkinCallBack()
		{
			Singleton<MenuTopBarView>.Instance.RefreshUI();
			this.SendSkinMsg();
			this.SendWearMsg();
		}
	}
}
