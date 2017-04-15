using ExitGames.Client.Photon;
using MobaProtocol;
using System;

namespace Assets.Scripts.Model
{
	internal class Model_TimeMask : ModelBase<TimeClass>
	{
		public Model_TimeMask()
		{
			base.Init(EModelType.Model_TimeMask);
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.ArenaAtcCheck, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.GetArenaState, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.ArenaAtcCheck, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetArenaState, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.LastError = 505;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				switch (MVC_MessageManager.NotifyModel_to_Game((ClientMsg)msg.ID))
				{
				case MobaGameCode.ArenaAtcCheck:
					this.OnGetMsg_GameCode_ArenaAtcCheck(operationResponse);
					break;
				case MobaGameCode.GetArenaState:
					this.OnGetMsg_GetArenaState(operationResponse);
					break;
				}
			}
			base.TriggerListners();
		}

		private void OnGetMsg_GameCode_ArenaAtcCheck(OperationResponse operationResponse)
		{
			base.LastError = (int)operationResponse.Parameters[1];
			MobaErrorCode lastError = (MobaErrorCode)base.LastError;
			if (lastError != MobaErrorCode.CDTimeError)
			{
				if (lastError != MobaErrorCode.DayCountError)
				{
					if (lastError != MobaErrorCode.Ok)
					{
						if (lastError != MobaErrorCode.EnergyShortage)
						{
							base.DebugMessage = "===>MobaGameClientPeer:TryArenaAtcCheckResponse" + operationResponse.OperationCode;
						}
						else
						{
							base.DebugMessage = "===>TryArenaAtcCheckResponse->体力不足，不允许执行挑战！";
						}
					}
					else
					{
						base.DebugMessage = "===>:TryArenaAtcCheckResponse->校验成功，允许执行挑战！";
						base.Data = new TimeClass();
						TimeClass timeClass = base.Data as TimeClass;
						timeClass.ArenaTimeSpan = DateTime.Now;
					}
				}
				else
				{
					base.DebugMessage = "===>MobaGameClientPeer:TryArenaAtcCheckResponse->次数不足，不允许执行挑战！";
				}
			}
			else
			{
				base.DebugMessage = "===>TryArenaAtcCheckResponse->CD时间未到，不允许执行挑战！";
			}
			base.DebugMessage = ((base.LastError != 0) ? "===>重置竞技场挑战CD时间响应操作失败" : "===>重置竞技场挑战CD时间响应操作成功");
		}

		private void OnGetMsg_GetArenaState(OperationResponse operationResponse)
		{
			base.LastError = (int)operationResponse.Parameters[1];
			MobaErrorCode lastError = (MobaErrorCode)base.LastError;
			if (lastError == MobaErrorCode.Ok)
			{
				base.Data = new TimeClass();
				TimeClass timeClass = base.Data as TimeClass;
				timeClass.ArenaTimeSpan = DateTime.Now;
			}
			base.DebugMessage = ((base.LastError != 0) ? "===>获取竞技场信息失败" : "==>获取竞技场信息成功");
		}
	}
}
