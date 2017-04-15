using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.Runes
{
	public class RunesCoalesceHigh : MonoBehaviour
	{
		private Transform transHigh;

		private Transform btnMinus;

		private Transform btnPlus;

		private UISprite btnCoalesceHigh;

		private UISprite runeHigh;

		private UILabel labelCurrHighCount;

		private UILabel labelHighDesc;

		private UILabel labelMoneyCost;

		private UILabel labelExpectCount;

		private Color alphaBtn = new Color(255f, 255f, 255f, 0.4862745f);

		private Color nonAlphaBtn = new Color(255f, 255f, 255f, 1f);

		private RuneSend runesend = default(RuneSend);

		private object[] mgs;

		private int modelID_1;

		private int modelID_2;

		private int modelID_3;

		private int countRunesOwn;

		private int countMoney;

		private int countRunesExp;

		private int cost1to2;

		private int cost2to3;

		private int moneyRequire;

		private bool isCanCoalesce;

		public int ModelID_3
		{
			get
			{
				return this.modelID_3;
			}
		}

		private int CountRunesOwn
		{
			get
			{
				return this.countRunesOwn;
			}
			set
			{
				this.countRunesOwn = value;
				this.labelCurrHighCount.text = "x" + this.countRunesOwn.ToString();
			}
		}

		private int CountMoney
		{
			get
			{
				return this.countMoney;
			}
			set
			{
				this.countMoney = value;
				this.labelMoneyCost.text = this.countMoney.ToString();
				UserData userData = ModelManager.Instance.Get_userData_X();
				this.labelMoneyCost.color = (((long)this.countMoney >= userData.Money) ? Color.red : Color.yellow);
			}
		}

		private int CountRunesExp
		{
			get
			{
				return this.countRunesExp;
			}
			set
			{
				this.countRunesExp = value;
				this.labelExpectCount.text = this.countRunesExp.ToString();
				if (this.countRunesExp == 0)
				{
					this.labelExpectCount.color = Color.yellow;
				}
				this.InitBtnState();
			}
		}

		private void Awake()
		{
			this.mgs = new object[]
			{
				ClientV2C.coalesceviewOpenView,
				ClientV2C.coalesceviewAdjustMiddle,
				ClientV2C.coalesceviewAfterPurchase,
				ClientV2C.coalesceviewAfterCoalesce,
				ClientV2C.coalesceviewChangeExpColor,
				ClientV2C.coalesceviewChangeBtnState
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
			this.transHigh = base.transform.Find("High");
			this.btnCoalesceHigh = this.transHigh.Find("CoalesceBtn").GetComponent<UISprite>();
			this.runeHigh = this.transHigh.Find("Rune").GetComponent<UISprite>();
			this.labelCurrHighCount = this.transHigh.Find("Count").GetComponent<UILabel>();
			this.labelHighDesc = this.transHigh.Find("Description").GetComponent<UILabel>();
			this.labelMoneyCost = this.transHigh.Find("CostHint/MoneyCost").GetComponent<UILabel>();
			this.btnMinus = this.transHigh.Find("CountAdjust/Minus");
			this.btnPlus = this.transHigh.Find("CountAdjust/Plus");
			this.labelExpectCount = this.transHigh.Find("CountAdjust/Label").GetComponent<UILabel>();
			UIEventListener.Get(this.btnCoalesceHigh.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickCoalesce);
			UIEventListener.Get(this.btnMinus.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickAdjust);
			UIEventListener.Get(this.btnPlus.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickAdjust);
		}

		private void OnMsg_coalesceviewOpenView(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				ModelIDs modelsData = default(ModelIDs);
				modelsData = (ModelIDs)msg.Param;
				this.SetModelsData(modelsData);
				this.SetPerCost();
				this.ResetSectionState();
				this.SetSprite();
				this.SetDescription();
				this.ReFreshOwned();
			}
		}

		private void OnMsg_coalesceviewAdjustMiddle(MobaMessage msg)
		{
			if (msg.Param == null)
			{
				this.ResetSectionState();
			}
		}

		private void OnMsg_coalesceviewAfterPurchase(MobaMessage msg)
		{
			if (msg != null)
			{
				RunesCtrl.GetInstance().runestate = RunesInlayState.Nothing;
				this.ReFreshOwned();
				this.CountRunesExp = this.countRunesExp;
			}
		}

		private void OnMsg_coalesceviewAfterCoalesce(MobaMessage msg)
		{
			if (msg != null)
			{
				this.ReFreshOwned();
				this.ResetSectionState();
				RunesCtrl.GetInstance().runestate = RunesInlayState.Nothing;
			}
		}

		private void OnMsg_coalesceviewChangeExpColor(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				bool flag = (bool)msg.Param;
				this.labelExpectCount.color = ((!flag || this.countRunesExp == 0) ? Color.yellow : Color.red);
			}
		}

		private void OnMsg_coalesceviewChangeBtnState(MobaMessage msg)
		{
			this.btnCoalesceHigh.alpha = 0.4862745f;
			this.btnCoalesceHigh.GetComponent<UIButton>().defaultColor = this.alphaBtn;
			this.btnCoalesceHigh.GetComponent<UIButton>().hover = this.alphaBtn;
			this.btnCoalesceHigh.GetComponent<UIButton>().pressed = this.alphaBtn;
			this.isCanCoalesce = false;
		}

		private void ResetSectionState()
		{
			this.CountRunesExp = 0;
			this.CountMoney = 0;
		}

		private void SetMoney(int count)
		{
			this.CountMoney = count * (this.cost1to2 + this.cost2to3);
		}

		private void SetModelsData(ModelIDs mds)
		{
			this.modelID_1 = mds.modelID_1;
			this.modelID_2 = mds.modelID_2;
			this.modelID_3 = mds.modelID_3;
		}

		private void SetSprite()
		{
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(this.modelID_3.ToString());
			this.runeHigh.spriteName = dataById.icon;
		}

		private void SetDescription()
		{
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(this.modelID_3.ToString());
			string[] array = dataById.attribute.Split(new char[]
			{
				'|'
			});
			this.labelHighDesc.text = ((!(array[1].Substring(array[1].Length - 1, 1) == "%")) ? ("+" + float.Parse(array[1]).ToString(array[2]) + LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName)) : ("+" + array[1] + LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName)));
		}

		private void SetPerCost()
		{
			if (this.modelID_1 != 0)
			{
				SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(this.modelID_1.ToString());
				if (dataById != null)
				{
					this.cost1to2 = dataById.consumption_money;
				}
			}
			if (this.modelID_2 != 0)
			{
				SysGameItemsVo dataById2 = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(this.modelID_2.ToString());
				if (dataById2 != null)
				{
					this.cost2to3 = dataById2.consumption_money;
				}
			}
		}

		private void ReFreshOwned()
		{
			List<EquipmentInfoData> list = ModelManager.Instance.Get_equipmentList_X();
			if (list != null)
			{
				EquipmentInfoData equipmentInfoData = list.Find((EquipmentInfoData obj) => obj.ModelId == this.modelID_3);
				this.CountRunesOwn = ((equipmentInfoData != null) ? equipmentInfoData.Count : 0);
			}
		}

		private void InitBtnState()
		{
			UserData userData = ModelManager.Instance.Get_userData_X();
			long money = userData.Money;
			bool flag = this.CheckCount();
			this.CountMoney = this.moneyRequire;
			if (flag && (long)this.moneyRequire <= money)
			{
				this.btnCoalesceHigh.alpha = 1f;
				this.btnCoalesceHigh.GetComponent<UIButton>().defaultColor = this.nonAlphaBtn;
				this.btnCoalesceHigh.GetComponent<UIButton>().hover = this.nonAlphaBtn;
				this.btnCoalesceHigh.GetComponent<UIButton>().pressed = this.nonAlphaBtn;
				this.isCanCoalesce = true;
			}
			else
			{
				this.btnCoalesceHigh.alpha = 0.4862745f;
				this.btnCoalesceHigh.GetComponent<UIButton>().defaultColor = this.alphaBtn;
				this.btnCoalesceHigh.GetComponent<UIButton>().hover = this.alphaBtn;
				this.btnCoalesceHigh.GetComponent<UIButton>().pressed = this.alphaBtn;
				this.isCanCoalesce = false;
			}
		}

		private bool CheckCount()
		{
			this.moneyRequire = 0;
			if (this.countRunesExp <= 0)
			{
				return false;
			}
			List<EquipmentInfoData> list = ModelManager.Instance.Get_equipmentList_X();
			EquipmentInfoData equipmentInfoData = list.Find((EquipmentInfoData obj) => obj.ModelId == this.modelID_1);
			int num = (equipmentInfoData != null) ? equipmentInfoData.Count : 0;
			equipmentInfoData = list.Find((EquipmentInfoData obj) => obj.ModelId == this.modelID_2);
			int num2 = (equipmentInfoData != null) ? equipmentInfoData.Count : 0;
			int num3 = this.countRunesExp * 3;
			if (num3 <= num2)
			{
				this.runesend.ModelID = this.modelID_3;
				this.runesend.Count1 = 0;
				this.runesend.Count2 = num3;
				this.moneyRequire = this.countRunesExp * this.cost2to3;
				return true;
			}
			int num4 = (num3 - num2) * 3;
			this.moneyRequire = (num3 - num2) * this.cost1to2 + this.countRunesExp * this.cost2to3;
			if (num4 <= num)
			{
				this.runesend.ModelID = this.modelID_3;
				this.runesend.Count1 = num4;
				this.runesend.Count2 = num2;
				return true;
			}
			return false;
		}

		private void SetPerCost(ModelIDs mds)
		{
			int num = mds.modelID_1;
			int num2 = mds.modelID_2;
			if (num != 0)
			{
				SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(num.ToString());
				if (dataById != null)
				{
					this.cost1to2 = dataById.consumption_money;
				}
			}
			if (num2 != 0)
			{
				SysGameItemsVo dataById2 = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(num2.ToString());
				if (dataById2 != null)
				{
					this.cost2to3 = dataById2.consumption_money;
				}
			}
		}

		private int ReturnInitCount(int modelid)
		{
			if (modelid == 0)
			{
				return 0;
			}
			int num = 0;
			List<EquipmentInfoData> list = ModelManager.Instance.Get_equipmentList_X();
			if (list != null)
			{
				EquipmentInfoData equipmentInfoData = list.Find((EquipmentInfoData obj) => obj.ModelId == modelid);
				int arg_58_0 = (equipmentInfoData != null) ? equipmentInfoData.Count : 0;
			}
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(modelid.ToString());
			bool flag = true;
			if (dataById != null)
			{
				flag = int.TryParse(dataById.consumption, out num);
			}
			num = ((flag && num != 0) ? ((int)Mathf.Pow((float)num, 2f)) : 9);
			return 0;
		}

		private int MoneyRequire()
		{
			return this.countRunesExp * (this.cost1to2 + this.cost2to3);
		}

		private void ClickCoalesce(GameObject obj)
		{
			if (null != obj && null != obj)
			{
				if (!this.isCanCoalesce)
				{
					Singleton<TipView>.Instance.ShowViewSetText("缺少一级符文或金钱条件不满足！！！", 1f);
					return;
				}
				if (RunesCtrl.GetInstance().runestate == RunesInlayState.Doing)
				{
					return;
				}
				RunesCtrl.GetInstance().runestate = RunesInlayState.Doing;
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在合成", false, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.Coalesce, param, new object[]
				{
					this.runesend.ModelID,
					this.runesend.Count1,
					this.runesend.Count2
				});
				RunesCtrl.GetInstance().modelID = this.modelID_3;
			}
		}

		private void ClickAdjust(GameObject obj)
		{
			if (null != obj)
			{
				string name = obj.name;
				if (name != null)
				{
					if (RunesCoalesceHigh.<>f__switch$map28 == null)
					{
						RunesCoalesceHigh.<>f__switch$map28 = new Dictionary<string, int>(2)
						{
							{
								"Minus",
								0
							},
							{
								"Plus",
								1
							}
						};
					}
					int num;
					if (RunesCoalesceHigh.<>f__switch$map28.TryGetValue(name, out num))
					{
						if (num != 0)
						{
							if (num == 1)
							{
								this.CountRunesExp++;
								if (this.countRunesExp > 99)
								{
									this.CountRunesExp = 0;
								}
							}
						}
						else
						{
							this.CountRunesExp--;
							if (this.countRunesExp < 0)
							{
								this.CountRunesExp = 99;
							}
						}
					}
				}
				MobaMessageManagerTools.SendClientMsg(ClientV2C.coalesceviewAdjustHigh, this.countRunesExp, false);
			}
		}
	}
}
