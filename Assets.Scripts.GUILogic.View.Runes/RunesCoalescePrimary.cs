using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.Runes
{
	public class RunesCoalescePrimary : MonoBehaviour
	{
		private Transform transPrimary;

		private Transform btnPurchase;

		private UISprite runePrimary;

		private UILabel countCurrPrimary;

		private UILabel descPrimary;

		private UILabel countLackPrimary;

		private object[] mgs;

		private int modelID_1;

		private int modelID_2;

		private int modelID_3;

		private int countRunes;

		private int nowLack;

		private int beforePurchase;

		private int afterPurchase;

		public int ModelID
		{
			get
			{
				return this.modelID_1;
			}
		}

		private int CountRunes
		{
			get
			{
				return this.countRunes;
			}
			set
			{
				this.countRunes = value;
				this.countCurrPrimary.text = "x" + this.countRunes.ToString();
			}
		}

		private void Awake()
		{
			this.mgs = new object[]
			{
				ClientV2C.coalesceviewOpenView,
				ClientV2C.coalesceviewAdjustMiddle,
				ClientV2C.coalesceviewAfterPurchase,
				ClientV2C.coalesceviewAfterCoalesce
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
		}

		private void Initialize()
		{
			this.transPrimary = base.transform.Find("Primary");
			this.btnPurchase = this.transPrimary.Find("PurchaseBtn/Sprite");
			this.runePrimary = this.transPrimary.Find("Rune").GetComponent<UISprite>();
			this.countCurrPrimary = this.transPrimary.Find("Count").GetComponent<UILabel>();
			this.descPrimary = this.transPrimary.Find("Description").GetComponent<UILabel>();
			this.countLackPrimary = this.transPrimary.Find("RunesLackCount").GetComponent<UILabel>();
			UIEventListener.Get(this.btnPurchase.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickPurchase);
		}

		private void OnMsg_coalesceviewOpenView(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				ModelIDs modelsData = default(ModelIDs);
				modelsData = (ModelIDs)msg.Param;
				this.SetModelsData(modelsData);
				this.countLackPrimary.gameObject.SetActive(false);
				this.SetSprite();
				this.SetDescription();
				this.RefreshOwned();
			}
		}

		private void OnMsg_coalesceviewAdjustMiddle(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				int target = (int)msg.Param;
				this.ReFreshLack(target, 2);
			}
		}

		private void OnMsg_coalesceviewAfterPurchase(MobaMessage msg)
		{
			if (msg != null)
			{
				this.ReFreshLack();
			}
		}

		private void OnMsg_coalesceviewAfterCoalesce(MobaMessage msg)
		{
			if (msg != null)
			{
				this.RefreshOwned();
				this.countLackPrimary.gameObject.SetActive(false);
			}
		}

		private void SetModelsData(ModelIDs mds)
		{
			this.modelID_1 = mds.modelID_1;
			this.modelID_2 = mds.modelID_2;
			this.modelID_3 = mds.modelID_3;
		}

		private void SetSprite()
		{
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(this.modelID_1.ToString());
			this.runePrimary.spriteName = dataById.icon;
		}

		private void SetDescription()
		{
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(this.modelID_1.ToString());
			string[] array = dataById.attribute.Split(new char[]
			{
				'|'
			});
			this.descPrimary.text = ((!(array[1].Substring(array[1].Length - 1, 1) == "%")) ? ("+" + float.Parse(array[1]).ToString(array[2]) + LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName)) : ("+" + array[1] + LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName)));
		}

		private void ClickPurchase(GameObject obj)
		{
			if (null != obj)
			{
				this.beforePurchase = this.countRunes;
				CtrlManager.OpenWindow(WindowID.PurchasePopupView, null);
				Singleton<PurchasePopupView>.Instance.onSuccess.Add(new Callback(this.AfterPurchase));
				Singleton<PurchasePopupView>.Instance.Show(GoodsSubject.Props, this.modelID_1.ToString(), 1, false);
			}
		}

		private void AfterPurchase()
		{
			List<EquipmentInfoData> list = ModelManager.Instance.Get_equipmentList_X();
			if (list != null && list.Count > 0)
			{
				EquipmentInfoData equipmentInfoData = list.Find((EquipmentInfoData obj) => obj.ModelId == this.modelID_1);
				this.CountRunes = ((equipmentInfoData != null) ? equipmentInfoData.Count : 0);
				this.afterPurchase = this.countRunes;
			}
			MobaMessageManagerTools.SendClientMsg(ClientV2C.coalesceviewAfterPurchase, null, false);
		}

		private void RefreshOwned()
		{
			List<EquipmentInfoData> list = ModelManager.Instance.Get_equipmentList_X();
			if (list != null && list.Count > 0)
			{
				EquipmentInfoData equipmentInfoData = list.Find((EquipmentInfoData obj) => obj.ModelId == this.modelID_1);
				this.CountRunes = ((equipmentInfoData != null) ? equipmentInfoData.Count : 0);
				this.beforePurchase = this.countRunes;
			}
		}

		private void ReFreshLack()
		{
			int num = this.afterPurchase - this.beforePurchase;
			if (num >= 0 && num < this.nowLack)
			{
				this.countLackPrimary.gameObject.SetActive(true);
				this.nowLack -= num;
				this.countLackPrimary.text = "缺少" + this.nowLack + "个1级符文";
				MobaMessageManagerTools.SendClientMsg(ClientV2C.coalesceviewChangeExpColor, true, false);
			}
			else
			{
				this.countLackPrimary.gameObject.SetActive(false);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.coalesceviewChangeExpColor, false, false);
			}
			this.afterPurchase = this.beforePurchase;
		}

		private void ReFreshLack(int target, int type)
		{
			int num = 0;
			if (type == 2)
			{
				num = target * 3;
			}
			if (type == 3)
			{
				num = target * 9;
			}
			this.countLackPrimary.gameObject.SetActive(num > this.countRunes);
			int num2 = num - this.countRunes;
			this.nowLack = num2;
			this.countLackPrimary.text = "*缺少" + num2 + "个1级符文*";
			MobaMessageManagerTools.SendClientMsg(ClientV2C.coalesceviewChangeExpColor, num > this.countRunes, false);
		}
	}
}
