using Com.Game.Utils;
using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_mailList : ModelBase<MailModelData>
	{
		public Model_mailList()
		{
			base.Init(EModelType.Model_mailList);
			base.Data = new MailModelData();
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.GetMailList, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.ReceiveMailAttachment, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.ModifyEmailState, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetMailList, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.ReceiveMailAttachment, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.ModifyEmailState, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.DebugMessage = ((base.LastError != 0) ? "===>邮件列表获取失败" : "===>邮件列表获取成功");
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					switch (MVC_MessageManager.NotifyModel_to_Game((ClientMsg)msg.ID))
					{
					case MobaGameCode.ReceiveMailAttachment:
						this.OnGetMsg_ReceiveMailAttachment(operationResponse);
						base.LastMsgType = 80;
						break;
					case MobaGameCode.GetMailList:
						this.OnGetMsg_MailList(operationResponse);
						base.LastMsgType = 81;
						break;
					case MobaGameCode.ModifyEmailState:
						this.OnGetMsg_ModifyEmailState(operationResponse);
						base.LastMsgType = 83;
						break;
					}
				}
			}
			base.Valid = (base.LastError == 0 && null != base.Data);
			base.TriggerListners();
		}

		private void OnGetMsg_MailList(OperationResponse operationResponse)
		{
			base.LastError = (int)operationResponse.Parameters[1];
			if (base.LastError == 0)
			{
				MailModelData mailModelData = base.Data as MailModelData;
				if (mailModelData == null)
				{
					mailModelData = new MailModelData();
				}
				byte[] buffer = operationResponse.Parameters[134] as byte[];
				List<MailData> mailList = SerializeHelper.Deserialize<List<MailData>>(buffer);
				mailModelData.mailList = mailList;
				base.Data = mailModelData;
				base.DebugMessage = "获取邮件信息成功！";
			}
			else
			{
				base.DebugMessage = "===>获取邮件信息" + operationResponse.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_ReceiveMailAttachment(OperationResponse operationResponse)
		{
			base.LastError = (int)operationResponse.Parameters[1];
			if (base.LastError == 0)
			{
				MailModelData mailModelData = base.Data as MailModelData;
				long maiId = (long)operationResponse.Parameters[73];
				MailData item = mailModelData.mailList.Find((MailData obj) => obj.Id == maiId);
				mailModelData.mailList.Remove(item);
				base.Data = mailModelData;
				base.DebugMessage = "收到邮件信息！";
			}
			else
			{
				base.DebugMessage = "===>收到邮件信息" + operationResponse.OperationCode;
				ClientLogger.Error("ReceiveMailAttachment Error: " + base.LastError);
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetMailList, null, new object[0]);
			}
		}

		private void OnGetMsg_ModifyEmailState(OperationResponse operationResponse)
		{
			base.LastError = (int)operationResponse.Parameters[1];
			MailModelData mailModelData = base.Data as MailModelData;
			MobaErrorCode lastError = (MobaErrorCode)base.LastError;
			if (lastError != MobaErrorCode.Ok)
			{
				ClientLogger.Error("ModifyEmailState Error: " + base.LastError);
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetMailList, null, new object[0]);
			}
			else
			{
				long mailId = (long)operationResponse.Parameters[73];
				mailModelData.modifyMailId = mailId;
				mailModelData.mailList.ForEach(delegate(MailData obj)
				{
					if (obj.Id == mailId)
					{
						obj.IsRead = true;
					}
				});
				base.DebugMessage = "收到邮件信息！";
			}
		}
	}
}
