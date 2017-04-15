using Assets.Scripts.Model;
using Com.Game.Module;
using MobaMessageData;
using MobaProtocol;
using MobaProtocol.Data;
using System;

namespace Assets.Scripts.GUILogic.View.BottleSystemView
{
	public class BottleViewCtrl
	{
		private static BottleViewCtrl instance;

		private static object obj_lock = new object();

		private bool isRequest;

		private CoroutineManager coroutiner;

		private DrawState state;

		public DrawState drawState
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}

		private BottleViewCtrl()
		{
		}

		public static BottleViewCtrl GetInstance()
		{
			if (BottleViewCtrl.instance == null)
			{
				object obj = BottleViewCtrl.obj_lock;
				lock (obj)
				{
					if (BottleViewCtrl.instance == null)
					{
						BottleViewCtrl.instance = new BottleViewCtrl();
						return BottleViewCtrl.instance;
					}
				}
			}
			return BottleViewCtrl.instance;
		}

		public void Init()
		{
			this.Register();
			this.coroutiner = new CoroutineManager();
		}

		public void UnInit()
		{
			this.UnRegister();
			this.drawState = DrawState.Nothing;
		}

		private void Register()
		{
			MobaMessageManager.RegistMessage((ClientMsg)21037, new MobaMessageFunc(this.ActionOpenLegend));
			MobaMessageManager.RegistMessage((ClientMsg)21038, new MobaMessageFunc(this.ActionOpenCollect));
			MobaMessageManager.RegistMessage((ClientMsg)21035, new MobaMessageFunc(this.ActionAddExpBall));
			MobaMessageManager.RegistMessage((ClientMsg)21034, new MobaMessageFunc(this.ActionUseExp));
			MobaMessageManager.RegistMessage((ClientMsg)21033, new MobaMessageFunc(this.ActionOpenView));
			MobaMessageManager.RegistMessage((ClientMsg)23054, new MobaMessageFunc(this.OnOpenView));
			MobaMessageManager.RegistMessage((ClientMsg)25012, new MobaMessageFunc(this.OnTimeOut));
		}

		private void UnRegister()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)21037, new MobaMessageFunc(this.ActionOpenLegend));
			MobaMessageManager.UnRegistMessage((ClientMsg)21038, new MobaMessageFunc(this.ActionOpenCollect));
			MobaMessageManager.UnRegistMessage((ClientMsg)21035, new MobaMessageFunc(this.ActionAddExpBall));
			MobaMessageManager.UnRegistMessage((ClientMsg)21034, new MobaMessageFunc(this.ActionUseExp));
			MobaMessageManager.UnRegistMessage((ClientMsg)21033, new MobaMessageFunc(this.ActionOpenView));
			MobaMessageManager.UnRegistMessage((ClientMsg)23054, new MobaMessageFunc(this.OnOpenView));
			MobaMessageManager.UnRegistMessage((ClientMsg)25012, new MobaMessageFunc(this.OnTimeOut));
		}

		private void ActionOpenView(MobaMessage msg)
		{
			this.isRequest = true;
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在更新数据...", false, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetMagicBottleInfo, param, new object[0]);
		}

		private void ActionOpenLegend(MobaMessage msg)
		{
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在开启...", false, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.DrawMagicBottleAward, param, new object[]
			{
				1
			});
		}

		private void ActionOpenCollect(MobaMessage msg)
		{
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在开启...", false, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.DrawMagicBottleAward, param, new object[]
			{
				2
			});
		}

		private void ActionAddExpBall(MobaMessage msg)
		{
			SendMsgManager.SendMsgParam sendMsgParam = new SendMsgManager.SendMsgParam(true, "正在打开商店页面...", true, 15f);
			CtrlManager.OpenWindow(WindowID.PurchasePopupView, null);
			Singleton<PurchasePopupView>.Instance.onSuccess.Add(new Callback(this.PurchaseSuccess));
			Singleton<PurchasePopupView>.Instance.Show(ETypicalCommodity.MagicExpBottle, false);
		}

		private void ActionUseExp(MobaMessage msg)
		{
			int num = 0;
			if (msg != null)
			{
				num = (int)msg.Param;
			}
			if (num != 0)
			{
				EquipmentInfoData equipmentInfoData = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId == 7777);
				if (equipmentInfoData == null)
				{
					CtrlManager.ShowMsgBox("NullException!", "没有道具可以被找到", delegate
					{
					}, PopViewType.PopOneButton, "确定", "取消", null);
				}
				else if (num != 0)
				{
					if (equipmentInfoData.Count - num >= 0)
					{
						SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在使用道具...", true, 15f);
						SendMsgManager.Instance.SendMsg(MobaGameCode.UseProps, param, new object[]
						{
							equipmentInfoData.EquipmentId,
							num,
							"0"
						});
					}
					if (equipmentInfoData.Count <= 0)
					{
						Singleton<TipView>.Instance.ShowViewSetText("小魔瓶经验球不足！", 1f);
					}
				}
			}
		}

		private void OnOpenView(MobaMessage msg)
		{
			if (msg != null && this.isRequest)
			{
				MagicBottleData param = (MagicBottleData)msg.Param;
				CtrlManager.OpenWindow(WindowID.BottleSystemView, null);
				MobaMessageManagerTools.SendClientMsg(ClientC2V.OpenBottleView, param, false);
				this.isRequest = false;
			}
		}

		private void PurchaseSuccess()
		{
			int count = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId == 7777).Count;
			MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemAddExpBallSuccess, count, false);
		}

		private void OnTimeOut(MobaMessage msg)
		{
			if (msg != null)
			{
				MobaMessageType mobaMessageType = MobaMessageType.GameCode;
				int num = 160;
				MsgData_WaitServerResponsTimeout msgData_WaitServerResponsTimeout = (MsgData_WaitServerResponsTimeout)msg.Param;
				if (msgData_WaitServerResponsTimeout.MobaMsgType == mobaMessageType && msgData_WaitServerResponsTimeout.MsgID == num)
				{
					Singleton<TipView>.Instance.ShowViewSetText("服务器正忙，请稍后再试", 1f);
				}
			}
		}
	}
}
