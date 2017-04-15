using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_applyList : ModelBase<List<FriendData>>
	{
		public Model_applyList()
		{
			base.Init(EModelType.Model_applyList);
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.SystemNotice, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaFriendCode.Friend_ModifyFriendStatus, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.SystemNotice, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaFriendCode.Friend_ModifyFriendStatus, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.LastError = 505;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				switch (MVC_MessageManager.NotifyModel_to_Friend((ClientMsg)msg.ID))
				{
				case MobaFriendCode.Friend_GetFriendList:
					this.OnGetMsg_GetFriendList(operationResponse);
					break;
				case MobaFriendCode.Friend_ModifyFriendStatus:
					this.OnGetMsg_ModifyFriendStatus(operationResponse);
					break;
				}
			}
			base.TriggerListners();
		}

		private void OnGetMsg_SystemNotice(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				byte[] buffer = res.Parameters[40] as byte[];
				NotificationData notificationData = SerializeHelper.Deserialize<NotificationData>(buffer);
				short type = notificationData.Type;
				if (type == 8)
				{
					string[] notPassApplyArr = notificationData.Content.Split(new char[]
					{
						'|'
					});
					((List<FriendData>)base.Data).RemoveAll((FriendData obj) => obj.TargetId == long.Parse(notPassApplyArr[notPassApplyArr.Length - 1]));
				}
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>SystemNotice" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_ModifyFriendStatus(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				byte b = (byte)res.Parameters[10];
				long targetSummid = (long)res.Parameters[102];
				byte b2 = b;
				if (b2 == 2)
				{
					if (base.Data != null)
					{
						((List<FriendData>)base.Data).RemoveAll((FriendData obj) => (int)obj.Status == 0 && obj.TargetId == targetSummid);
					}
				}
				base.DebugMessage = "====>Ok " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>GetArenaEnemyInfo" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0 || base.LastError == 20105 || base.LastError == 20106);
		}

		private void OnGetMsg_GetFriendList(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				byte[] buffer = res.Parameters[27] as byte[];
				List<FriendData> list = SerializeHelper.Deserialize<List<FriendData>>(buffer);
				base.Data = new List<FriendData>();
				foreach (FriendData current in list)
				{
					sbyte status = current.Status;
					if (status == 3)
					{
						((List<FriendData>)base.Data).Add(current);
					}
				}
				base.DebugMessage = "====>OK" + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>GetFriendList" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}
	}
}
