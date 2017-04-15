using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace HomeState
{
	internal class HomeState_requestData : HomeStateBase
	{
		private const string LogTag = "Home";

		private const int WaitTime = 15;

		private bool bFinish;

		private float RecordTime;

		private int countdown_waitTime = 15;

		private List<MobaGameCode> listRequest = new List<MobaGameCode>();

		private List<MobaGameCode> listWait = new List<MobaGameCode>();

		private Task timerTask;

		private CoroutineManager m_CoroutineManager = new CoroutineManager();

		public HomeState_requestData() : base(HomeStateCode.HomeState_requestData, MobaPeerType.C2GateServer)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.RequestData();
		}

		public override void OnExit()
		{
			base.OnExit();
			this.m_CoroutineManager.StopAllCoroutine();
		}

		public override void OnUpdate(long delta)
		{
		}

		protected override void RegistHandler()
		{
			base.RegistHandler();
			this.listRequest.Add(MobaGameCode.GetHeroList);
			this.listRequest.Add(MobaGameCode.GetTaskList);
			this.listRequest.Add(MobaGameCode.ShowDailyTask);
			this.listRequest.Add(MobaGameCode.GetEquipmentList);
			this.listRequest.Add(MobaGameCode.GetMagicBottleInfo);
			this.listRequest.Add(MobaGameCode.GetDoubleCard);
			this.listRequest.Add(MobaGameCode.GetSignDay);
			this.listRequest.Add(MobaGameCode.GetActivityTask);
			this.listRequest.Add(MobaGameCode.GetHomeTotalRecord);
			this.listRequest.Add(MobaGameCode.GetSummSkinList);
			this.listRequest.Add(MobaGameCode.GetDiscountCard);
			this.listRequest.Add(MobaGameCode.GetShopNew);
			this.listRequest.Add(MobaGameCode.GetCurrencyCount);
			this.listRequest.Add(MobaGameCode.GetActivityTask);
			this.listRequest.Add(MobaGameCode.GetNoticeBoard);
			foreach (MobaGameCode current in this.listRequest)
			{
				MVC_MessageManager.AddListener_view(current, new MobaMessageFunc(this.OnMsg));
			}
		}

		protected override void UnregistHandler()
		{
			base.UnregistHandler();
			foreach (MobaGameCode current in this.listRequest)
			{
				MVC_MessageManager.RemoveListener_view(current, new MobaMessageFunc(this.OnMsg));
			}
		}

		private void RequestData()
		{
			this.RecordTime = Time.realtimeSinceStartup;
			this.countdown_waitTime = 15;
			this.listWait.Clear();
			foreach (MobaGameCode current in this.listRequest)
			{
				this.listWait.Add(current);
			}
			this.DoRequestData();
		}

		private void DoRequestData()
		{
			this.timerTask = this.m_CoroutineManager.StartCoroutine(this.Corountine_waitRequestData(), true);
			long num = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetHeroList, null, new object[]
			{
				num.ToString()
			});
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetEquipmentList, null, new object[0]);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetTaskList, null, new object[]
			{
				0
			});
			SendMsgManager.Instance.SendMsg(MobaGameCode.ShowDailyTask, null, new object[0]);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetMagicBottleInfo, null, new object[0]);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetDoubleCard, null, new object[0]);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetShopNew, null, new object[0]);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetSignDay, null, new object[0]);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetActivityTask, null, new object[0]);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetHomeTotalRecord, null, new object[0]);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetSummSkinList, null, new object[]
			{
				num
			});
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetDiscountCard, null, new object[0]);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetCurrencyCount, null, new object[]
			{
				0
			});
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetNoticeBoard, null, new object[0]);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetKdaMyHeroData, null, new object[0]);
		}

		[DebuggerHidden]
		private IEnumerator Corountine_waitRequestData()
		{
			HomeState_requestData.<Corountine_waitRequestData>c__Iterator45 <Corountine_waitRequestData>c__Iterator = new HomeState_requestData.<Corountine_waitRequestData>c__Iterator45();
			<Corountine_waitRequestData>c__Iterator.<>f__this = this;
			return <Corountine_waitRequestData>c__Iterator;
		}

		private void OnCompleteRequest(MobaGameCode code)
		{
			foreach (MobaGameCode current in this.listWait)
			{
				if (code == current)
				{
					this.countdown_waitTime = 15;
					this.listWait.Remove(current);
					break;
				}
			}
			if (this.listWait.Count == 0)
			{
				this.m_CoroutineManager.StopAllCoroutine();
				this.OnRequestDataFinish();
			}
		}

		private void OnRequestDataFinish()
		{
			this.RecordTime = Time.realtimeSinceStartup - this.RecordTime;
			if (GlobalSettings.isLoginByHoolaiSDK)
			{
				InitSDK.instance.SetExtData("1");
			}
			else if (GlobalSettings.isLoginByAnySDK)
			{
				InitSDK.instance.SetAnySDKExtData("1");
			}
			HomeManager.Instance.ChangeState(HomeStateCode.HomeState_menu);
		}

		private void OnMsg(MobaMessage msg)
		{
			string text = "On";
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num;
			MobaMessageType mobaMessageType = MVC_MessageManager.ClientMsg_to_RawCode((ClientMsg)msg.ID, out num);
			if (mobaMessageType == MobaMessageType.GameCode)
			{
				text += ((MobaGameCode)num).ToString();
			}
			Type type = base.GetType();
			MethodInfo method = type.GetMethod(text, BindingFlags.Instance | BindingFlags.NonPublic);
			if (method != null)
			{
				method.Invoke(this, new object[]
				{
					operationResponse
				});
			}
		}

		private void OnGetSignDay(OperationResponse res)
		{
			int num = (int)res.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				this.OnCompleteRequest(MobaGameCode.GetSignDay);
			}
		}

		private void OnGetHomeTotalRecord(OperationResponse res)
		{
			int num = (int)res.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				this.OnCompleteRequest(MobaGameCode.GetHomeTotalRecord);
			}
		}

		private void OnGetHeroList(OperationResponse res)
		{
			int num = (int)res.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				ClientLogger.Error("GetHeroList失败：" + res.DebugMessage);
				Singleton<TipView>.Instance.ShowViewSetText("英雄列表获取失败!", 1f);
			}
			else
			{
				CharacterDataMgr.instance.UpdateHerosData();
				this.OnCompleteRequest(MobaGameCode.GetHeroList);
			}
		}

		private void OnGetTaskList(OperationResponse res)
		{
			int num = (int)res.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				ClientLogger.Error("GetTaskList失败：" + res.DebugMessage);
				Singleton<TipView>.Instance.ShowViewSetText("任务列表获取失败!", 1f);
			}
			else
			{
				this.OnCompleteRequest(MobaGameCode.GetTaskList);
			}
		}

		private void OnShowDailyTask(OperationResponse res)
		{
			int num = (int)res.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				ClientLogger.Error("ShowDailyTask失败：" + res.DebugMessage);
				Singleton<TipView>.Instance.ShowViewSetText("日常列表获取失败!", 1f);
			}
			else
			{
				this.OnCompleteRequest(MobaGameCode.ShowDailyTask);
			}
		}

		private void OnGetEquipmentList(OperationResponse res)
		{
			int num = (int)res.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				ClientLogger.Error("GetEquipmentList失败：" + res.DebugMessage);
				Singleton<TipView>.Instance.ShowViewSetText("背包道具列表获取失败!", 1f);
			}
			else
			{
				CharacterDataMgr.instance.UpdateHerosData();
				this.OnCompleteRequest(MobaGameCode.GetEquipmentList);
			}
		}

		private void OnGetMagicBottleInfo(OperationResponse res)
		{
			int num = (int)res.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				this.OnCompleteRequest(MobaGameCode.GetMagicBottleInfo);
			}
		}

		private void OnGetDoubleCard(OperationResponse res)
		{
			int num = (int)res.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				this.OnCompleteRequest(MobaGameCode.GetDoubleCard);
			}
		}

		private void OnGetSummSkinList(OperationResponse res)
		{
			int num = (int)res.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				this.OnCompleteRequest(MobaGameCode.GetSummSkinList);
			}
		}

		private void OnGetShopNew(OperationResponse res)
		{
			int num = (int)res.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				this.OnCompleteRequest(MobaGameCode.GetShopNew);
			}
		}

		private void OnGetDiscountCard(OperationResponse res)
		{
			int num = (int)res.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				this.OnCompleteRequest(MobaGameCode.GetDiscountCard);
			}
		}

		private void OnGetCurrencyCount(OperationResponse res)
		{
			int num = (int)res.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				this.OnCompleteRequest(MobaGameCode.GetCurrencyCount);
			}
		}

		private void OnGetActivityTask(OperationResponse res)
		{
			int num = (int)res.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				this.OnCompleteRequest(MobaGameCode.GetActivityTask);
			}
		}

		private void OnGetNoticeBoard(OperationResponse res)
		{
			int num = (int)res.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				this.OnCompleteRequest(MobaGameCode.GetNoticeBoard);
			}
		}

		protected override void OnConnected_game(MobaMessage msg)
		{
			if (!this.bFinish && NetWorkHelper.Instance.IsGateAvailable)
			{
				this.DoRequestData();
			}
		}

		protected override void OnDisconnected_game(MobaMessage msg)
		{
		}
	}
}
