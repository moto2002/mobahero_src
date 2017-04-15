using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using MobaMessageData;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class DoubleCardCtrl : MonoBehaviour
	{
		private const int X = -310;

		private const int Y = -145;

		private UILabel ExpCount;

		private UILabel GoldCount;

		private UILabel contentTitle;

		private UILabel firstState;

		private UILabel secondState;

		private Transform transExp;

		private Transform transGold;

		private Transform transDetail;

		private Transform closeBG;

		private Task task_countDown;

		private CoroutineManager coroutine;

		private object[] mgs;

		private bool isExp;

		private bool isPress;

		private int expCountWin;

		private double expCountTime;

		private int goldCountWin;

		private double goldCountTime;

		private int expMultiple;

		private int goldMultiple;

		private float tempTime = 60f;

		private DateTime expTimeGotten;

		private DateTime goldTimeGotten;

		private int days;

		private int hours;

		private int mins;

		private TimeSpan expSpan;

		private TimeSpan goldSpan;

		public int ExpCountWin
		{
			get
			{
				return this.expCountWin;
			}
		}

		public double ExpCountTime
		{
			get
			{
				return this.expCountTime;
			}
		}

		public int GoldCountWin
		{
			get
			{
				return this.goldCountWin;
			}
		}

		public double GoldCountTime
		{
			get
			{
				return this.goldCountTime;
			}
		}

		private void Awake()
		{
			this.mgs = new object[]
			{
				ClientC2V.GetDoubleCard,
				ClientC2C.GateConnected,
				ClientC2C.GateDisconnected,
				ClientC2C.WaitServerResponseTimeOut
			};
			this.Initialize();
		}

		private void OnEnable()
		{
			this.Register();
			this.transDetail.gameObject.SetActive(false);
			this.tempTime = 60f;
			this.GetModelData();
			this.DisplayInfo();
			if (this.task_countDown != null)
			{
				this.coroutine.StopCoroutine(this.task_countDown);
			}
			this.task_countDown = this.coroutine.StartCoroutine(this.CountDown(), true);
		}

		private void OnDisable()
		{
			this.Unregister();
			if (this.coroutine != null)
			{
				this.coroutine.StopAllCoroutine();
			}
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
			this.transExp = base.transform.Find("DoubleExp");
			this.transGold = base.transform.Find("DoubleGold");
			this.transDetail = base.transform.Find("Details");
			this.closeBG = this.transDetail.Find("BG");
			this.ExpCount = this.transExp.Find("Count").GetComponent<UILabel>();
			this.GoldCount = this.transGold.Find("Count").GetComponent<UILabel>();
			this.contentTitle = this.transDetail.Find("Title").GetComponent<UILabel>();
			this.firstState = this.transDetail.Find("1stState").GetComponent<UILabel>();
			this.secondState = this.transDetail.Find("2ndState").GetComponent<UILabel>();
			this.coroutine = new CoroutineManager();
			UIEventListener.Get(this.transExp.gameObject).onClick = new UIEventListener.VoidDelegate(this.DragItem);
			UIEventListener.Get(this.transGold.gameObject).onClick = new UIEventListener.VoidDelegate(this.DragItem);
			UIEventListener.Get(this.closeBG.gameObject).onClick = new UIEventListener.VoidDelegate(this.CloseDetail);
		}

		private void OnMsg_GetDoubleCard(MobaMessage msg)
		{
			if (msg != null)
			{
				this.GetModelData();
				this.DisplayInfo();
			}
		}

		private void OnMsg_GateConnected(MobaMessage msg)
		{
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetDoubleCard, null, new object[0]);
		}

		private void OnMsg_GateDisconnected(MobaMessage msg)
		{
			if (this.coroutine != null)
			{
				this.coroutine.StopAllCoroutine();
			}
		}

		private void OnMsg_WaitServerResponseTimeOut(MobaMessage msg)
		{
			if (msg != null)
			{
				MobaMessageType mobaMessageType = MobaMessageType.GameCode;
				int num = 23049;
				MsgData_WaitServerResponsTimeout msgData_WaitServerResponsTimeout = (MsgData_WaitServerResponsTimeout)msg.Param;
				if (msgData_WaitServerResponsTimeout.MobaMsgType == mobaMessageType && msgData_WaitServerResponsTimeout.MsgID == num)
				{
					Singleton<TipView>.Instance.ShowViewSetText("获取数据失败！", 1f);
				}
			}
		}

		private void GetModelData()
		{
			this.expCountWin = 0;
			this.expCountTime = 0.0;
			this.goldCountWin = 0;
			this.goldCountTime = 0.0;
			this.expMultiple = 1;
			this.goldMultiple = 1;
			List<DoubleCardData> list = ModelManager.Instance.Get_All_DoubleCardData_X();
			List<DoubleCardData> list2 = new List<DoubleCardData>();
			List<DoubleCardData> list3 = new List<DoubleCardData>();
			if (list != null && list.Count != 0)
			{
				list2.AddRange(list.FindAll((DoubleCardData exps) => exps.type == 2));
				list3.AddRange(list.FindAll((DoubleCardData golds) => golds.type == 1));
			}
			if (list2 != null && list2.Count != 0)
			{
				SysGameBuffVo dataById = BaseDataMgr.instance.GetDataById<SysGameBuffVo>(list2[0].modelid.ToString());
				this.expMultiple += ((list2.Count <= 1) ? dataById.add_multiple : (list2.Count * dataById.add_multiple));
				for (int num = 0; num != list2.Count; num++)
				{
					int recordtype = list2[num].recordtype;
					if (recordtype != 1)
					{
						if (recordtype == 2)
						{
							this.expCountWin = list2[num].recordvalue;
						}
					}
					else
					{
						this.expCountTime = (double)list2[num].recordvalue;
						this.expTimeGotten = list2[num].timeget;
						this.expCountTime -= ToolsFacade.Instance.SpanFromNowToGet(this.expTimeGotten).TotalMinutes;
					}
				}
			}
			if (list3 != null && list3.Count != 0)
			{
				SysGameBuffVo dataById2 = BaseDataMgr.instance.GetDataById<SysGameBuffVo>(list3[0].modelid.ToString());
				this.goldMultiple += ((list3.Count <= 1) ? dataById2.add_multiple : (list3.Count * dataById2.add_multiple));
				for (int num2 = 0; num2 != list3.Count; num2++)
				{
					int recordtype2 = list3[num2].recordtype;
					if (recordtype2 != 1)
					{
						if (recordtype2 == 2)
						{
							this.goldCountWin = list3[num2].recordvalue;
						}
					}
					else
					{
						this.goldCountTime = (double)list3[num2].recordvalue;
						this.goldTimeGotten = list3[num2].timeget;
						this.goldCountTime -= ToolsFacade.Instance.SpanFromNowToGet(this.goldTimeGotten).TotalMinutes;
					}
				}
			}
		}

		private void DisplayInfo()
		{
			this.transExp.gameObject.SetActive(0 < this.expCountWin || 0.0 < this.expCountTime);
			this.transGold.gameObject.SetActive(0 < this.goldCountWin || 0.0 < this.goldCountTime);
			this.expMultiple = ((this.expCountWin <= 0 || this.expCountTime <= 0.0) ? 2 : 3);
			this.goldMultiple = ((this.goldCountWin <= 0 || this.goldCountTime <= 0.0) ? 2 : 3);
			this.ExpCount.text = this.expMultiple.ToString() + "multi";
			this.GoldCount.text = this.goldMultiple.ToString() + "multi";
			if (this.expCountTime <= 0.0)
			{
				ModelManager.Instance.RemoveCertainCardData(2, 1);
			}
			if (this.goldCountTime <= 0.0)
			{
				ModelManager.Instance.RemoveCertainCardData(1, 1);
			}
		}

		private void SetTxt()
		{
		}

		private void CloseDetail(GameObject obj)
		{
			if (null != obj)
			{
				this.isPress = false;
				if (null != this.transDetail)
				{
					this.transDetail.gameObject.SetActive(false);
				}
			}
		}

		private void DragItem(GameObject obj)
		{
			if (null != obj)
			{
				string name = obj.name;
				if (name != null)
				{
					if (DoubleCardCtrl.<>f__switch$map19 == null)
					{
						DoubleCardCtrl.<>f__switch$map19 = new Dictionary<string, int>(2)
						{
							{
								"DoubleExp",
								0
							},
							{
								"DoubleGold",
								1
							}
						};
					}
					int num;
					if (DoubleCardCtrl.<>f__switch$map19.TryGetValue(name, out num))
					{
						if (num != 0)
						{
							if (num == 1)
							{
								this.contentTitle.text = LanguageManager.Instance.GetStringById("Game_Buff_TipTitle_Gold");
								this.AllocateData(false);
								this.isExp = false;
							}
						}
						else
						{
							this.contentTitle.text = LanguageManager.Instance.GetStringById("Game_Buff_TipTitle_Exp");
							this.AllocateData(true);
							this.isExp = true;
						}
					}
				}
				this.isPress = true;
				this.transDetail.gameObject.SetActive(true);
			}
		}

		private void AllocateData(bool isExp)
		{
			if (isExp)
			{
				this.transDetail.localPosition = new Vector3(-310f, -145f, 0f);
				this.AllocateText(this.expCountTime, this.expCountWin);
			}
			else
			{
				this.transDetail.localPosition = new Vector3(-310f, -145f, 0f) + new Vector3(90f, 0f, 0f);
				this.AllocateText(this.goldCountTime, this.goldCountWin);
			}
		}

		private void StartProcedure()
		{
		}

		[DebuggerHidden]
		private IEnumerator CountDown()
		{
			DoubleCardCtrl.<CountDown>c__Iterator135 <CountDown>c__Iterator = new DoubleCardCtrl.<CountDown>c__Iterator135();
			<CountDown>c__Iterator.<>f__this = this;
			return <CountDown>c__Iterator;
		}

		private void TransferToTimeSpan(double value, ref string content)
		{
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, 0);
			timeSpan = TimeSpan.FromMinutes(value);
			this.days = timeSpan.Days;
			this.hours = timeSpan.Hours;
			this.mins = timeSpan.Minutes;
			if (0 < this.days)
			{
				content = content + this.days + "天";
			}
			if (0 < this.hours)
			{
				content = content + this.hours + "时";
			}
			if (0 < this.mins)
			{
				content = content + this.mins + "分";
			}
		}

		private void AllocateText(double countTime, int countWin)
		{
			string text;
			if (0.0 >= countTime || 0 >= countWin)
			{
				this.secondState.gameObject.SetActive(false);
				if (countTime <= 0.0)
				{
					text = LanguageManager.Instance.GetStringById("Game_Buff_TipType_Win") + countWin.ToString();
				}
				else
				{
					text = LanguageManager.Instance.GetStringById("Game_Buff_TipType_Time");
					this.TransferToTimeSpan(countTime, ref text);
				}
				this.firstState.text = text;
				return;
			}
			this.secondState.gameObject.SetActive(true);
			text = LanguageManager.Instance.GetStringById("Game_Buff_TipType_Time");
			this.TransferToTimeSpan(countTime, ref text);
			this.firstState.text = text;
			text = LanguageManager.Instance.GetStringById("Game_Buff_TipType_Win") + countWin.ToString();
			this.secondState.text = text;
		}
	}
}
