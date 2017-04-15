using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaMessageData;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BottleSystemView
{
	public class BottleViewBottomRight : MonoBehaviour
	{
		private const int AVERAGE = 3;

		private const int COLTOTAL = 10;

		private List<DropItemData> did = new List<DropItemData>();

		private int typeBottle;

		private int currlevel;

		private int lastRange;

		private int times;

		private int countLegend;

		private int countCollect;

		private bool isProceeding;

		private string descriptionLen = string.Empty;

		private string descriptianCol = string.Empty;

		private Transform buttonLegend;

		private UILabel textCountLegend;

		private UILabel textDesLegend;

		private UISprite spriteLegend;

		private UISprite legendDigitBg_1;

		private UISprite legendDigitBg_2;

		private Transform buttonCollect;

		private UILabel textCountCollect;

		private UILabel textDesCollect;

		private UISprite spriteCollect;

		private UISprite collectDigitBg_1;

		private UISprite collectDigitBg_2;

		private Transform legendGetFX;

		private Transform collectGetFX;

		private Dictionary<string, SysMagicbottleLevelVo> info;

		private int CountLegend
		{
			get
			{
				return this.countLegend;
			}
			set
			{
				this.countLegend = value;
				if (0 >= this.countLegend)
				{
					this.countLegend = 0;
				}
				this.RefreshCountUI();
			}
		}

		private int CountCollect
		{
			get
			{
				return this.countCollect;
			}
			set
			{
				this.countCollect = value;
				if (0 >= this.countCollect)
				{
					this.countCollect = 0;
				}
				this.RefreshCountUI();
			}
		}

		private void Awake()
		{
			this.Initialize();
		}

		private void OnEnable()
		{
			BottleViewCtrl.GetInstance().drawState = DrawState.Nothing;
			this.legendGetFX.gameObject.SetActive(false);
			this.collectGetFX.gameObject.SetActive(false);
			this.Register();
		}

		private void OnDisable()
		{
			this.Unregister();
			BottleViewCtrl.GetInstance().drawState = DrawState.Nothing;
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void Initialize()
		{
			this.buttonLegend = base.transform.Find("Legendary");
			this.textCountLegend = base.transform.Find("Legendary/DigitBg/Label").GetComponent<UILabel>();
			this.textDesLegend = base.transform.Find("Legendary/Title/Description").GetComponent<UILabel>();
			this.spriteLegend = base.transform.Find("Legendary/LegendSprite").GetComponent<UISprite>();
			this.legendDigitBg_1 = base.transform.Find("Legendary/DigitBg/DigitBg_1").GetComponent<UISprite>();
			this.legendDigitBg_2 = base.transform.Find("Legendary/DigitBg/DigitBg_2").GetComponent<UISprite>();
			this.legendGetFX = base.transform.Find("Legendary/Cs_get");
			this.buttonCollect = base.transform.Find("Collector");
			this.textCountCollect = base.transform.Find("Collector/DigitBg/Label").GetComponent<UILabel>();
			this.textDesCollect = base.transform.Find("Collector/Title/Description").GetComponent<UILabel>();
			this.spriteCollect = base.transform.Find("Collector/CollectorSprite").GetComponent<UISprite>();
			this.collectDigitBg_1 = base.transform.Find("Collector/DigitBg/DigitBg_1").GetComponent<UISprite>();
			this.collectDigitBg_2 = base.transform.Find("Collector/DigitBg/DigitBg_2").GetComponent<UISprite>();
			this.collectGetFX = base.transform.Find("Collector/Dc_get");
			UIEventListener.Get(this.buttonLegend.gameObject).onClick = new UIEventListener.VoidDelegate(this.OpenTreasure);
			UIEventListener.Get(this.buttonCollect.gameObject).onClick = new UIEventListener.VoidDelegate(this.OpenTreasure);
		}

		private void InitUI()
		{
			string str = "[00ff00]/" + 3 + "[-]";
			string str2 = string.Empty;
			string newValue = string.Empty;
			this.currlevel = ModelManager.Instance.Get_BottleData_Level();
			this.info = BaseDataMgr.instance.GetTypeDicByType<SysMagicbottleLevelVo>();
			int num = Tools_ParsePrice.MaxLevelCheck<SysMagicbottleLevelVo>(this.info, false);
			int num2;
			if (this.currlevel >= num)
			{
				SysMagicbottleLevelVo dataById = BaseDataMgr.instance.GetDataById<SysMagicbottleLevelVo>(num.ToString());
				int lastLevelDefference = dataById.lastLevelDefference;
				num2 = ((this.currlevel - num) / lastLevelDefference + 1) * lastLevelDefference + num;
			}
			else
			{
				num2 = Tools_ParsePrice.ParseCollectRange(this.currlevel, true);
			}
			this.countLegend = ModelManager.Instance.Get_BottleData_LegendCount();
			this.countCollect = ModelManager.Instance.Get_BottleData_CollectorCount();
			this.textCountLegend.text = this.countLegend.ToString();
			this.textCountCollect.text = this.countCollect.ToString();
			this.legendDigitBg_1.gameObject.SetActive(this.countLegend < 10);
			this.legendDigitBg_2.gameObject.SetActive(this.countLegend >= 10);
			this.collectDigitBg_1.gameObject.SetActive(this.countCollect < 10);
			this.collectDigitBg_2.gameObject.SetActive(this.countCollect >= 10);
			this.spriteLegend.color = ((this.countLegend != 0) ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f));
			this.spriteCollect.color = ((this.countCollect != 0) ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f));
			int num3 = (this.currlevel % 3 != 0) ? (this.currlevel % 3) : 3;
			str2 = "[ff0000]" + num3 + "[-]";
			newValue = "[00ff00]" + num2 + "[-]";
			string stringById = LanguageManager.Instance.GetStringById("MagicBottle_NomalOpenTips");
			string stringById2 = LanguageManager.Instance.GetStringById("MagicBottle_GoodOpenTips");
			this.descriptionLen = stringById.Replace("*", str2 + str);
			this.descriptianCol = stringById2.Replace("*", newValue);
			this.textDesLegend.text = this.descriptionLen;
			this.textDesCollect.text = this.descriptianCol;
		}

		private void Register()
		{
			MobaMessageManager.RegistMessage((ClientMsg)23051, new MobaMessageFunc(this.OnDraw));
			MobaMessageManager.RegistMessage((ClientMsg)23053, new MobaMessageFunc(this.OnOpenView));
			MobaMessageManager.RegistMessage((ClientMsg)21036, new MobaMessageFunc(this.ActionLevelUp));
			MobaMessageManager.RegistMessage((ClientMsg)25012, new MobaMessageFunc(this.OnTimeOut));
			MobaMessageManager.RegistMessage((ClientMsg)25062, new MobaMessageFunc(this.OnPeerConnected));
			MobaMessageManager.RegistMessage((ClientMsg)25061, new MobaMessageFunc(this.OnPeerDisconnected));
		}

		private void Unregister()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)23051, new MobaMessageFunc(this.OnDraw));
			MobaMessageManager.UnRegistMessage((ClientMsg)23053, new MobaMessageFunc(this.OnOpenView));
			MobaMessageManager.UnRegistMessage((ClientMsg)21036, new MobaMessageFunc(this.ActionLevelUp));
			MobaMessageManager.UnRegistMessage((ClientMsg)25012, new MobaMessageFunc(this.OnTimeOut));
			MobaMessageManager.UnRegistMessage((ClientMsg)25062, new MobaMessageFunc(this.OnPeerConnected));
			MobaMessageManager.UnRegistMessage((ClientMsg)25061, new MobaMessageFunc(this.OnPeerDisconnected));
		}

		private void OpenTreasure(GameObject obj)
		{
			if (null != obj)
			{
				if (BottleViewCtrl.GetInstance().drawState == DrawState.Drawing)
				{
					return;
				}
				if (obj.name.CompareTo("Legendary") == 0)
				{
					if (this.countLegend == 0)
					{
						if (BottleViewCtrl.GetInstance().drawState == DrawState.Nothing)
						{
							MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemOpenSecondaryPanel, LackState.lackLegend, false);
						}
						return;
					}
					if (BottleViewCtrl.GetInstance().drawState == DrawState.Nothing)
					{
						BottleViewCtrl.GetInstance().drawState = DrawState.Drawing;
						this.ConfirmOpenLegend(true);
					}
				}
				if (obj.name.CompareTo("Collector") == 0)
				{
					if (this.countCollect == 0)
					{
						if (BottleViewCtrl.GetInstance().drawState == DrawState.Nothing)
						{
							MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemOpenSecondaryPanel, LackState.lackCollect, false);
						}
						return;
					}
					if (BottleViewCtrl.GetInstance().drawState == DrawState.Nothing)
					{
						BottleViewCtrl.GetInstance().drawState = DrawState.Drawing;
						this.ConfirmOpenCollect(true);
					}
				}
			}
		}

		private void ConfirmOpenLegend(bool sure)
		{
			AudioMgr.Play("Play_UI_OpenCSP", null, false, false);
			if (sure)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemOpenLegend, null, false);
			}
		}

		private void ConfirmOpenCollect(bool sure)
		{
			AudioMgr.Play("Play_UI_OpenDCP", null, false, false);
			if (sure)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemOpenCollect, null, false);
			}
		}

		private void OnDraw(MobaMessage msg)
		{
			if (msg == null)
			{
				CtrlManager.ShowMsgBox("Null Exception!!!", "'msg' is null. Please check 'OnDraw' method. 'BottleViewBottomRight.cs'.", delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
				return;
			}
			List<object> list = (List<object>)msg.Param;
			this.typeBottle = (int)list[0];
			this.did.Clear();
			this.did.AddRange((List<DropItemData>)list[1]);
			this.times = 0;
			this.times++;
			CtrlManager.OpenWindow(WindowID.GetItemsView, null);
			if (this.did.Count == 0 || this.did == null)
			{
				return;
			}
			Tools_BottleDrop.ParseDetail(this.did);
			Tools_BottleDrop.SetModelData();
			MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemChangeBottleDraw, null, false);
			GetItemsView expr_A5 = Singleton<GetItemsView>.Instance;
			expr_A5.onFinish = (Callback)Delegate.Combine(expr_A5.onFinish, new Callback(this.RefreshDataUI));
			if (this.typeBottle != 0)
			{
				if (this.typeBottle == 1)
				{
					this.CountLegend--;
				}
				else
				{
					this.CountCollect--;
				}
			}
		}

		private void OnOpenView(MobaMessage msg)
		{
			if (msg != null)
			{
				MagicBottleData magicBottleData = (MagicBottleData)msg.Param;
				if (magicBottleData != null)
				{
					this.InitUI();
				}
			}
		}

		private void OnPeerConnected(MobaMessage msg)
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemOpenView, null, false);
			BottleViewCtrl.GetInstance().drawState = DrawState.Nothing;
		}

		private void OnPeerDisconnected(MobaMessage msg)
		{
			BottleViewCtrl.GetInstance().drawState = DrawState.Nothing;
		}

		private void ActionLevelUp(MobaMessage msg)
		{
			if (msg != null)
			{
				int num = (int)msg.Param;
				if (num != 0)
				{
					if (Tools_ParsePrice.IsCollectorBottleUp(this.currlevel, num))
					{
						this.CountCollect++;
						this.collectGetFX.gameObject.SetActive(false);
						this.collectGetFX.gameObject.SetActive(true);
						AudioMgr.Play("Play_DCP_Levelup", null, false, false);
					}
					this.currlevel = num;
					if (this.currlevel % 3 == 0)
					{
						this.CountLegend++;
						this.legendGetFX.gameObject.SetActive(false);
						this.legendGetFX.gameObject.SetActive(true);
						AudioMgr.Play("Play_CSP_Levelup", null, false, false);
					}
					this.RefreshDesUI();
				}
			}
		}

		private void OnTimeOut(MobaMessage msg)
		{
			if (msg != null)
			{
				MobaMessageType mobaMessageType = MobaMessageType.GameCode;
				int num = 161;
				MsgData_WaitServerResponsTimeout msgData_WaitServerResponsTimeout = (MsgData_WaitServerResponsTimeout)msg.Param;
				if (msgData_WaitServerResponsTimeout.MobaMsgType == mobaMessageType && msgData_WaitServerResponsTimeout.MsgID == num)
				{
					BottleViewCtrl.GetInstance().drawState = DrawState.Nothing;
				}
			}
		}

		private void RefreshDesUI()
		{
			if (this.info == null)
			{
				this.info = BaseDataMgr.instance.GetTypeDicByType<SysMagicbottleLevelVo>();
			}
			int num = Tools_ParsePrice.MaxLevelCheck<SysMagicbottleLevelVo>(this.info, false);
			int num2;
			if (this.currlevel >= num)
			{
				SysMagicbottleLevelVo dataById = BaseDataMgr.instance.GetDataById<SysMagicbottleLevelVo>(num.ToString());
				int lastLevelDefference = dataById.lastLevelDefference;
				num2 = ((this.currlevel - num) / lastLevelDefference + 1) * lastLevelDefference + num;
			}
			else
			{
				num2 = Tools_ParsePrice.ParseCollectRange(this.currlevel, true);
			}
			string str = "[00ff00]/" + 3 + "[-]";
			string str2 = string.Empty;
			string newValue = string.Empty;
			int num3 = (this.currlevel % 3 != 0) ? (this.currlevel % 3) : 3;
			str2 = "[ff0000]" + num3 + "[-]";
			newValue = "[00ff00]" + num2 + "[-]";
			string stringById = LanguageManager.Instance.GetStringById("MagicBottle_NomalOpenTips");
			string stringById2 = LanguageManager.Instance.GetStringById("MagicBottle_GoodOpenTips");
			this.descriptionLen = stringById.Replace("*", str2 + str);
			this.descriptianCol = stringById2.Replace("*", newValue);
			this.textDesLegend.text = this.descriptionLen;
			this.textDesCollect.text = this.descriptianCol;
		}

		private void RefreshCountUI()
		{
			this.textCountLegend.text = this.countLegend.ToString();
			this.textCountCollect.text = this.countCollect.ToString();
			this.legendDigitBg_1.gameObject.SetActive(this.countLegend < 10);
			this.legendDigitBg_2.gameObject.SetActive(this.countLegend >= 10);
			this.collectDigitBg_1.gameObject.SetActive(this.countCollect < 10);
			this.collectDigitBg_2.gameObject.SetActive(this.countCollect >= 10);
			this.spriteLegend.color = ((this.countLegend != 0) ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f));
			this.spriteCollect.color = ((this.countCollect != 0) ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f));
		}

		private void RefreshDataUI()
		{
			List<EquipmentInfoData> list = ModelManager.Instance.Get_equipmentList_X();
			EquipmentInfoData equipmentInfoData = new EquipmentInfoData();
			if (list != null)
			{
				equipmentInfoData = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId == 7777);
			}
			if (equipmentInfoData != null && equipmentInfoData.Count != 0)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemAddExpBallSuccess, equipmentInfoData.Count, false);
			}
			else
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemAddExpBallSuccess, 0, false);
			}
			BottleViewCtrl.GetInstance().drawState = DrawState.Nothing;
		}

		public void NewbieGetMagicBottleTaleAwd()
		{
			if (this.buttonLegend != null)
			{
				this.OpenTreasure(this.buttonLegend.gameObject);
			}
		}

		public bool NewbieIsHaveLengendBottle()
		{
			return this.CountLegend > 0;
		}
	}
}
