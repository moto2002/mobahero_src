using Com.Game.Module;
using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.PropertyView
{
	public class PropViewCollectionItemInfo : MonoBehaviour
	{
		private Transform item_Info;

		private Transform buttonItem;

		private UISprite sideBG;

		private UILabel quality;

		private UILabel itemName;

		private EffectItem currEffectItem;

		private object[] mgs;

		private void Awake()
		{
			this.mgs = new object[]
			{
				ClientV2C.propviewClickCollectionItem
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

		private void Register()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
		}

		private void Unregister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
		}

		private void Start()
		{
		}

		private void Update()
		{
			if (null == base.transform || null == this.item_Info)
			{
				Debug.LogError(base.transform + " " + this.item_Info);
			}
		}

		private void Initialize()
		{
			this.item_Info = base.transform.Find("ItemInfo");
			this.sideBG = this.item_Info.Find("SideBG").GetComponent<UISprite>();
			this.quality = this.item_Info.Find("Quality").GetComponent<UILabel>();
			this.itemName = this.item_Info.Find("ItemName").GetComponent<UILabel>();
			this.buttonItem = this.item_Info.Find("ButtonItem");
			UIEventListener.Get(this.buttonItem.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickButtonInfo);
		}

		private void OnMsg_propviewClickCollectionItem(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				EffectItem effectItem = (EffectItem)msg.Param;
				this.currEffectItem = effectItem;
				InfoItem component = this.item_Info.GetComponent<InfoItem>();
				component.Init(null != effectItem);
				component.CheckInfoState(effectItem);
			}
		}

		private void OnClickButtonInfo(GameObject obj)
		{
			if (SacrificialCtrl.GetInstance().collectionState == CollectionState.Sending)
			{
				Singleton<TipView>.Instance.ShowViewSetText("服务器正忙  请稍后点击!!!", 1f);
				return;
			}
			if (null != obj && SacrificialCtrl.GetInstance().collectionState == CollectionState.Nothing)
			{
				SacrificialCtrl.GetInstance().collectionState = CollectionState.Sending;
				AudioMgr.Play("Play_Menu_click", null, false, false);
				InfoItem component = obj.transform.parent.GetComponent<InfoItem>();
				ButtonType bT = component.BT;
				switch (bT)
				{
				case ButtonType.CapShop:
					CtrlManager.OpenWindow(WindowID.PurchasePopupView, null);
					Singleton<PurchasePopupView>.Instance.onSuccess.Add(new Callback(this.AfterBuying));
					Singleton<PurchasePopupView>.Instance.Show(GoodsSubject.Props, this.currEffectItem.ModelID.ToString(), 1, false);
					SacrificialCtrl.GetInstance().collectionState = CollectionState.Nothing;
					return;
				case ButtonType.MagicBottle:
					MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemOpenView, null, false);
					return;
				case (ButtonType)3:
					IL_90:
					if (bT == ButtonType.Wear)
					{
						MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewCollectionWearEffect, 1, false);
						return;
					}
					if (bT != ButtonType.Discharge)
					{
						return;
					}
					MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewCollectionWearEffect, 2, false);
					return;
				case ButtonType.Achievement:
					CtrlManager.OpenWindow(WindowID.AchievementView, null);
					return;
				case ButtonType.RankList:
					CtrlManager.OpenWindow(WindowID.RankView, null);
					return;
				case ButtonType.Battle:
					CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
					return;
				}
				goto IL_90;
			}
		}

		private void AfterBuying()
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewCollectionAfterBuying, null, false);
			SacrificialCtrl.GetInstance().collectionState = CollectionState.Nothing;
			InfoItem component = this.item_Info.GetComponent<InfoItem>();
			component.Init(false);
		}
	}
}
