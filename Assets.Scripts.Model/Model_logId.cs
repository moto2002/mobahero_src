using ExitGames.Client.Photon;
using MobaProtocol;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Assets.Scripts.Model
{
	internal class Model_logId : ModelBase<long>
	{
		private List<MobaGameCode> listCode;

		public Model_logId()
		{
			base.Init(EModelType.Model_logId);
		}

		public override void RegisterMsgHandler()
		{
			this.listCode = new List<MobaGameCode>
			{
				MobaGameCode.UploadArenaAtc
			};
			foreach (MobaGameCode current in this.listCode)
			{
				MVC_MessageManager.AddListener_model(current, new MobaMessageFunc(this.OnGetMsg));
			}
		}

		public override void UnRegisterMsgHandler()
		{
			foreach (MobaGameCode current in this.listCode)
			{
				MVC_MessageManager.RemoveListener_model(current, new MobaMessageFunc(this.OnGetMsg));
			}
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			int num = 0;
			MobaMessageType mobaMessageType = MVC_MessageManager.ClientMsg_to_RawCode((ClientMsg)msg.ID, out num);
			if (this.listCode.Contains((MobaGameCode)num))
			{
				string name = "OnGetMsg_" + mobaMessageType.ToString() + "_" + ((MobaGameCode)num).ToString();
				MethodInfo method = base.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
				if (method != null)
				{
					object[] parameters = new object[]
					{
						msg
					};
					method.Invoke(this, parameters);
					base.TriggerListners();
				}
			}
		}

		private bool PreHandel(MobaMessage msg, out OperationResponse res)
		{
			res = null;
			if (msg == null)
			{
				return false;
			}
			res = (msg.Param as OperationResponse);
			return res != null;
		}

		private void OnGetMsg_GameCode_UploadArenaAtc(MobaMessage msg)
		{
			base.LastError = 505;
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					if (lastError != MobaErrorCode.MoneyShortage)
					{
						base.DebugMessage = "===>TryUsingSkillPointResponse" + operationResponse.OperationCode;
					}
					else
					{
						base.DebugMessage = "===>TryUsingSkillPointResponse->金币不足！";
					}
				}
				else
				{
					long num = (long)operationResponse.Parameters[181];
					base.Data = num;
					base.DebugMessage = "===>OnGetMsg_GameCode_UploadArenaAtc ...LogId" + operationResponse.OperationCode;
				}
			}
			base.Valid = (base.LastError == 0 && null != base.Data);
		}
	}
}
