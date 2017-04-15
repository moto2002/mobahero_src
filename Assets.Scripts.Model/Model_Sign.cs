using ExitGames.Client.Photon;
using MobaProtocol;
using System;

namespace Assets.Scripts.Model
{
	internal class Model_Sign : ModelBase<SignState>
	{
		public Model_Sign()
		{
			base.Init(EModelType.Model_Sign);
			base.Data = new SignState();
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.GetSignDay, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetSignDay, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					MobaGameCode mobaGameCode = MVC_MessageManager.NotifyModel_to_Game((ClientMsg)msg.ID);
					base.LastMsgType = (int)mobaGameCode;
					MobaGameCode mobaGameCode2 = mobaGameCode;
					if (mobaGameCode2 == MobaGameCode.GetSignDay)
					{
						this.OnGetMsg_GetSignDay(operationResponse);
					}
				}
			}
			base.TriggerListners();
		}

		private void OnGetMsg_GetSignDay(OperationResponse operationResponse)
		{
			base.LastError = (int)operationResponse.Parameters[1];
			SignState signState = base.Data as SignState;
			MobaErrorCode lastError = (MobaErrorCode)base.LastError;
			if (lastError == MobaErrorCode.Ok)
			{
				signState.isPass = Convert.ToInt32(operationResponse.Parameters[150]);
				signState.week = (int)Convert.ToInt16(operationResponse.Parameters[10]);
				signState.dataReceiveTime = ToolsFacade.ServerCurrentTime;
				base.DebugMessage = "====>OK " + operationResponse.OperationCode;
			}
			base.Data = signState;
			base.Valid = (base.LastError == 0);
		}
	}
}
