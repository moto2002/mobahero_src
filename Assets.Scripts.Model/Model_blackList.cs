using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_blackList : ModelBase<List<FriendData>>
	{
		public Model_blackList()
		{
			base.Init(EModelType.Model_blackList);
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaFriendCode.Friend_ModifyFriendStatus, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaFriendCode.Friend_ModifyFriendStatus, new MobaMessageFunc(this.OnGetMsg));
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
					this.OnGetMsg_TryModifyFriendStatus(operationResponse);
					break;
				}
			}
			base.TriggerListners();
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
					if (status == 2)
					{
						((List<FriendData>)base.Data).Add(current);
					}
				}
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>GetFriendList" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_TryModifyFriendStatus(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				byte b = (byte)res.Parameters[10];
				long targetSummid = (long)res.Parameters[102];
				byte b2 = b;
				if (b2 == 3)
				{
					((List<FriendData>)base.Data).RemoveAll((FriendData obj) => (int)obj.Status == 2 && obj.TargetId == targetSummid);
				}
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>TryModifyFriendStatus" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}
	}
}
