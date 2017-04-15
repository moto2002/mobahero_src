using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_gameStatus : ModelBase<GameStatus>
	{
		public Model_gameStatus()
		{
			base.Init(EModelType.Model_gameStatus);
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.LastError = 505;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				MobaFriendCode mobaFriendCode = MVC_MessageManager.NotifyModel_to_Friend((ClientMsg)msg.ID);
				MobaFriendCode mobaFriendCode2 = mobaFriendCode;
				if (mobaFriendCode2 == MobaFriendCode.Friend_GetFriendList)
				{
					this.OnGetMsg_GetFriendList(operationResponse);
				}
			}
			base.TriggerListners();
		}

		private void OnGetMsg_GetFriendList(OperationResponse res)
		{
			int num = (int)res.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				byte[] buffer = res.Parameters[27] as byte[];
				List<FriendData> list = SerializeHelper.Deserialize<List<FriendData>>(buffer);
				foreach (FriendData item in list)
				{
					sbyte status = item.Status;
					if (status == 1)
					{
						FriendData friendData = ModelManager.Instance.Get_FriendDataList_X().Find((FriendData obj) => obj.TargetId == item.TargetId);
						if (friendData != null)
						{
							friendData.GameStatus = item.GameStatus;
						}
						else
						{
							ModelManager.Instance.Get_FriendDataList_X().Add(item);
						}
					}
				}
			}
			base.Valid = (base.LastError == 0);
		}
	}
}
