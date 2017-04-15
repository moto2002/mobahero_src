using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaMessageData;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BottleSystemView
{
	public class BottleViewRightTop : MonoBehaviour
	{
		private int PER_EXPBALL;

		private string tip_continue;

		private bool isClick;

		private bool isStop;

		private float realTime;

		private int countCost;

		private long basicExp;

		private long deltaExp;

		private int expBall;

		private int currLevel;

		private long currExp;

		private long currMaxExp;

		private BottleSystemDigit bottlesystemdigit;

		private Coroutine long_coroutine;

		private Coroutine click_coroutine;

		private Transform buttonPurchaseExp;

		private Transform buttonUseExp;

		private Transform interrogation;

		private Transform rule;

		private UILabel textBottleExp;

		private UILabel textExpBallCount;

		private UIGrid levels;

		private TweenScale tween_scale;

		private UIProgressBar loadingBar;

		public int CurrLevel
		{
			get
			{
				return this.currLevel;
			}
			set
			{
				this.currLevel = value;
				this.ChangeLevelEffect();
				ModelManager.Instance.Get_BottleData().curlevel = this.currLevel;
				MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemCanLevelUp, this.CurrLevel, false);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemChangeBottleLevelUp, this.currLevel, false);
			}
		}

		private int Expball
		{
			get
			{
				return this.expBall;
			}
			set
			{
				this.expBall = value;
				if (this.expBall <= 0)
				{
					this.expBall = 0;
				}
				this.textExpBallCount.text = this.expBall.ToString();
			}
		}

		private void Awake()
		{
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
			MobaMessageManager.RegistMessage((ClientMsg)23052, new MobaMessageFunc(this.OnUseExp));
			MobaMessageManager.RegistMessage((ClientMsg)23053, new MobaMessageFunc(this.OnOpenView));
			MobaMessageManager.RegistMessage((ClientMsg)21040, new MobaMessageFunc(this.ActionAddExpBallSuccess));
			MobaMessageManager.RegistMessage((ClientMsg)25012, new MobaMessageFunc(this.OnTimeOut));
			MobaMessageManager.RegistMessage((ClientMsg)25062, new MobaMessageFunc(this.OnPeerConnected));
			MobaMessageManager.RegistMessage((ClientMsg)25061, new MobaMessageFunc(this.OnPeerDisconnected));
		}

		private void Unregister()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)23052, new MobaMessageFunc(this.OnUseExp));
			MobaMessageManager.UnRegistMessage((ClientMsg)23053, new MobaMessageFunc(this.OnOpenView));
			MobaMessageManager.UnRegistMessage((ClientMsg)21040, new MobaMessageFunc(this.ActionAddExpBallSuccess));
			MobaMessageManager.UnRegistMessage((ClientMsg)25012, new MobaMessageFunc(this.OnTimeOut));
			MobaMessageManager.UnRegistMessage((ClientMsg)25062, new MobaMessageFunc(this.OnPeerConnected));
			MobaMessageManager.UnRegistMessage((ClientMsg)25061, new MobaMessageFunc(this.OnPeerDisconnected));
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void Initialize()
		{
			this.buttonPurchaseExp = base.transform.Find("AddExp/AddBtn");
			this.buttonUseExp = base.transform.Find("LevelUp");
			this.interrogation = base.transform.Find("Title/Interrogation");
			this.rule = base.transform.Find("Rule");
			this.textBottleExp = base.transform.Find("Exp/Progress/Text").GetComponent<UILabel>();
			this.textExpBallCount = base.transform.Find("AddExp/Exp").GetComponent<UILabel>();
			this.levels = base.transform.Find("Levels").GetComponent<UIGrid>();
			this.tween_scale = this.levels.GetComponent<TweenScale>();
			this.loadingBar = base.transform.Find("Exp/Progress/LanBar").GetComponent<UIProgressBar>();
			this.bottlesystemdigit = Resources.Load<BottleSystemDigit>("Prefab/UI/Bottle/BottleSystemDigit");
			UIEventListener.Get(this.buttonPurchaseExp.gameObject).onClick = new UIEventListener.VoidDelegate(this.PurchaseExp);
			UIEventListener.Get(this.buttonUseExp.gameObject).onPress = new UIEventListener.BoolDelegate(this.AddExpPress);
			UIEventListener.Get(this.interrogation.gameObject).onClick = new UIEventListener.VoidDelegate(this.ShowRule);
			this.tip_continue = LanguageManager.Instance.GetStringById("Bottle_Continue");
			this.PER_EXPBALL = int.Parse(BaseDataMgr.instance.GetDataById<SysGameItemsVo>("7777").effect.Split(new char[]
			{
				'|'
			})[1]);
		}

		private void InitUI()
		{
			this.currLevel = ModelManager.Instance.Get_BottleData_Level();
			this.currExp = ModelManager.Instance.Get_BottleData_Exp();
			this.currMaxExp = (long)Model_BottleSystem.Exp_Max;
			this.loadingBar.value = (float)this.currExp / (float)this.currMaxExp;
			this.loadingBar.alpha = (float)((this.loadingBar.value <= 0f) ? 0 : 1);
			this.textBottleExp.text = this.currExp + "/" + this.currMaxExp;
			MagicBottleData magicBottleData = ModelManager.Instance.Get_BottleData();
			int expball = (magicBottleData != null) ? magicBottleData.expbottlecount : 0;
			this.Expball = expball;
			this.rule.gameObject.SetActive(false);
			string[] array = LanguageManager.Instance.GetStringById("Bottle_Describe").Split(new char[]
			{
				'|'
			});
			if (array == null)
			{
				return;
			}
			string str = "[ffcd04]" + array[0];
			string str2 = "[00e1c9]" + array[1];
			this.rule.GetChild(0).GetComponent<UILabel>().text = str + str2;
			GridHelper.FillGrid<BottleSystemDigit>(this.levels, this.bottlesystemdigit, 5, delegate(int idx, BottleSystemDigit comp)
			{
				comp.name = (5 - idx).ToString();
				comp.Init(this.CheckNum(this.currLevel), idx);
			});
			float num = 0f;
			for (int i = this.levels.transform.childCount - 1; i >= 0; i--)
			{
				if (this.levels.transform.GetChild(i).gameObject.activeInHierarchy)
				{
					this.levels.transform.GetChild(i).GetComponent<BottleSystemDigit>().GPU.lagtime = num;
					this.levels.transform.GetChild(i).GetComponent<_GPUParticleCtrl>().enabled = false;
					num += 0.2f;
				}
			}
			this.levels.Reposition();
		}

		private void ShowRule(GameObject obj)
		{
			if (null != obj)
			{
				this.rule.gameObject.SetActive(!this.rule.gameObject.activeInHierarchy);
			}
		}

		private void PurchaseExp(GameObject obj = null)
		{
			if (null != obj && BottleViewCtrl.GetInstance().drawState == DrawState.Nothing)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemAddExpBall, null, false);
			}
		}

		private void AddExpPress(GameObject obj, bool isPress)
		{
			EquipmentInfoData equipmentInfoData = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData eb) => eb.ModelId == 7777);
			if (!NetWorkHelper.Instance.IsGateAvailable)
			{
				Singleton<TipView>.Instance.ShowViewSetText("网络环境较差，请稍后再试", 1f);
			}
			else if (equipmentInfoData == null || equipmentInfoData.Count == 0)
			{
				this.PurchaseExp(obj);
			}
			else if (isPress)
			{
				if (BottleViewCtrl.GetInstance().drawState == DrawState.Nothing)
				{
					BottleViewCtrl.GetInstance().drawState = DrawState.Drawing;
					this.isStop = false;
					this.isClick = true;
					if (this.long_coroutine != null)
					{
						base.StopCoroutine("IsLongPress");
					}
					this.long_coroutine = base.StartCoroutine("IsLongPress");
				}
			}
			else
			{
				this.isStop = true;
				if (this.long_coroutine != null)
				{
					base.StopCoroutine("IsLongPress");
				}
				this.realTime = 0f;
				if (this.isClick)
				{
					if (this.click_coroutine != null)
					{
						return;
					}
					MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemChangeBottleOnce, 0.3f, false);
					this.UseExpBall(1);
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator IsLongPress()
		{
			BottleViewRightTop.<IsLongPress>c__Iterator119 <IsLongPress>c__Iterator = new BottleViewRightTop.<IsLongPress>c__Iterator119();
			<IsLongPress>c__Iterator.<>f__this = this;
			return <IsLongPress>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator ChangeExp()
		{
			BottleViewRightTop.<ChangeExp>c__Iterator11A <ChangeExp>c__Iterator11A = new BottleViewRightTop.<ChangeExp>c__Iterator11A();
			<ChangeExp>c__Iterator11A.<>f__this = this;
			return <ChangeExp>c__Iterator11A;
		}

		[DebuggerHidden]
		private IEnumerator ChangeLongPressExp()
		{
			BottleViewRightTop.<ChangeLongPressExp>c__Iterator11B <ChangeLongPressExp>c__Iterator11B = new BottleViewRightTop.<ChangeLongPressExp>c__Iterator11B();
			<ChangeLongPressExp>c__Iterator11B.<>f__this = this;
			return <ChangeLongPressExp>c__Iterator11B;
		}

		private void UseExpBall(int count)
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemUseExp, count, false);
		}

		private void OnUseExp(MobaMessage msg)
		{
			if (msg != null)
			{
				int num = (int)msg.Param;
				if (num == 1 && this.isClick)
				{
					this.Expball--;
					if (this.click_coroutine != null)
					{
						base.StopCoroutine("ChangeExp");
					}
					this.click_coroutine = base.StartCoroutine("ChangeExp");
				}
				if (num > 1)
				{
					BottleViewCtrl.GetInstance().drawState = DrawState.Nothing;
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
					this.levels.Reposition();
				}
			}
		}

		private void OnPeerConnected(MobaMessage msg)
		{
			this.isStop = false;
			MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemOpenView, null, false);
		}

		private void OnTimeOut(MobaMessage msg)
		{
			if (msg != null)
			{
				MobaMessageType mobaMessageType = MobaMessageType.GameCode;
				int num = 210;
				MsgData_WaitServerResponsTimeout msgData_WaitServerResponsTimeout = (MsgData_WaitServerResponsTimeout)msg.Param;
				if (msgData_WaitServerResponsTimeout.MobaMsgType == mobaMessageType && msgData_WaitServerResponsTimeout.MsgID == num)
				{
					MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemOpenView, null, false);
					BottleViewCtrl.GetInstance().drawState = DrawState.Nothing;
				}
			}
		}

		private void OnPeerDisconnected(MobaMessage msg)
		{
			this.isStop = true;
			base.StopAllCoroutines();
		}

		private void ActionAddExpBallSuccess(MobaMessage msg)
		{
			if (msg != null)
			{
				int expball = (int)msg.Param;
				this.Expball = expball;
			}
		}

		private void OnOpenLegend(MobaMessage msg)
		{
		}

		private void OnOpenCollect(MobaMessage msg)
		{
		}

		private List<int> CheckNum(int level)
		{
			return new List<int>
			{
				level % 10,
				level / 10 % 10,
				level / 100 % 10,
				level / 1000 % 10,
				level / 10000 % 10
			};
		}

		private void LevelUpThree()
		{
		}

		private void ChangeLevelEffect()
		{
			GridHelper.FillGrid<BottleSystemDigit>(this.levels, this.bottlesystemdigit, 5, delegate(int idx, BottleSystemDigit comp)
			{
				comp.CheckPosition(this.CheckNum(this.currLevel));
			});
			GridHelper.FillGrid<BottleSystemDigit>(this.levels, this.bottlesystemdigit, 5, delegate(int idx, BottleSystemDigit comp)
			{
				comp.CheckTween(this.CheckNum(this.currLevel));
			});
		}

		public void NewbieMagicBottleUseExp(bool isPress)
		{
			if (this.buttonUseExp != null)
			{
				EquipmentInfoData equipmentInfoData = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData eb) => eb.ModelId == 7777);
				if (equipmentInfoData == null || equipmentInfoData.Count == 0)
				{
					return;
				}
				this.AddExpPress(this.buttonUseExp.gameObject, isPress);
			}
		}
	}
}
