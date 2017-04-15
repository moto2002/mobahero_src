using Com.Game.Utils;
using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_Friend : ModelBase<FriendModelData>
	{
		public Model_Friend()
		{
			base.Init(EModelType.Model_Friend);
			base.Data = new FriendModelData();
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaFriendCode.Friend_ApplyAddFriend, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaFriendCode.Friend_ModifyFriendStatus, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.GetUserInfoBySummId, new MobaMessageFunc(this.OnGetMsgGameCode));
			MVC_MessageManager.AddListener_model(MobaGameCode.GetFriendMessages, new MobaMessageFunc(this.OnGetMsgGameCode));
			MVC_MessageManager.AddListener_model(MobaGameCode.SystemNotice, new MobaMessageFunc(this.OnGetMsgGameCode));
			MVC_MessageManager.AddListener_model(MobaFriendCode.Friend_RaderFindFriend, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaFriendCode.Friend_ApplyAddFriend, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaFriendCode.Friend_ModifyFriendStatus, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetUserInfoBySummId, new MobaMessageFunc(this.OnGetMsgGameCode));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetFriendMessages, new MobaMessageFunc(this.OnGetMsgGameCode));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.SystemNotice, new MobaMessageFunc(this.OnGetMsgGameCode));
			MVC_MessageManager.RemoveListener_model(MobaFriendCode.Friend_RaderFindFriend, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.DebugMessage = ((base.LastError != 0) ? "===>好友内容取失败" : "===>好友内容获取成功");
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					MobaFriendCode mobaFriendCode = MVC_MessageManager.NotifyModel_to_Friend((ClientMsg)msg.ID);
					base.LastMsgType = msg.ID;
					switch (mobaFriendCode)
					{
					case MobaFriendCode.Friend_ApplyAddFriend:
						this.OnGetMsg_ApplyAddFriend(operationResponse);
						break;
					case MobaFriendCode.Friend_GetFriendList:
						this.OnGetMsg_FriendList(operationResponse);
						break;
					case MobaFriendCode.Friend_RaderFindFriend:
						this.OnGetMsg_RadarFindFriend(operationResponse);
						break;
					case MobaFriendCode.Friend_ModifyFriendStatus:
						this.OnGetMsg_ModifyFriendStatus(operationResponse);
						break;
					}
				}
			}
			base.Valid = ((base.LastError == 0 || base.LastError == 20109 || base.LastError == 20105 || base.LastError == 20106 || base.LastError == 20107 || base.LastError == 20104 || base.LastError == 20101) && null != base.Data);
			base.TriggerListners();
		}

		protected void OnGetMsgGameCode(MobaMessage msg)
		{
			base.DebugMessage = ((base.LastError != 0) ? "===>好友内容取失败" : "===>好友内容获取成功");
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					MobaGameCode mobaGameCode = MVC_MessageManager.NotifyModel_to_Game((ClientMsg)msg.ID);
					base.LastMsgType = msg.ID;
					MobaGameCode mobaGameCode2 = mobaGameCode;
					switch (mobaGameCode2)
					{
					case MobaGameCode.GetFriendMessages:
						this.OnGetMsg_GetFriendMessages(operationResponse);
						goto IL_9B;
					case MobaGameCode.SendPrivateMessage:
						IL_67:
						if (mobaGameCode2 != MobaGameCode.SystemNotice)
						{
							goto IL_9B;
						}
						this.OnGetMsg_SystemNotice(operationResponse);
						goto IL_9B;
					case MobaGameCode.GetUserInfoBySummId:
						this.OnGetMsg_GetUserInfoBySummId(operationResponse);
						goto IL_9B;
					}
					goto IL_67;
				}
			}
			IL_9B:
			base.Valid = ((base.LastError == 0 || base.LastError == 20109 || base.LastError == 20105 || base.LastError == 20106 || base.LastError == 20107 || base.LastError == 20104 || base.LastError == 20101) && null != base.Data);
			base.TriggerListners();
		}

		private void OnGetMsg_FriendList(OperationResponse operationResponse)
		{
			base.LastError = (int)operationResponse.Parameters[1];
			FriendModelData friendModelData = base.Data as FriendModelData;
			MobaErrorCode lastError = (MobaErrorCode)base.LastError;
			if (lastError != MobaErrorCode.Ok)
			{
				friendModelData.getFriendDataListOk = false;
			}
			else
			{
				friendModelData.getFriendDataListOk = true;
				byte[] buffer = operationResponse.Parameters[27] as byte[];
				List<FriendData> list = SerializeHelper.Deserialize<List<FriendData>>(buffer);
				friendModelData.friendDataList = new List<FriendData>();
				foreach (FriendData item in list)
				{
					sbyte status = item.Status;
					if (status == 1)
					{
						FriendData friendData = friendModelData.friendDataList.Find((FriendData obj) => obj.TargetId == item.TargetId);
						if (friendData != null)
						{
							friendData.Exp = item.Exp;
							friendData.GameStatus = item.GameStatus;
							friendData.Icon = item.Icon;
							friendData.LadderRank = item.LadderRank;
							friendData.LadderScore = item.LadderScore;
							friendData.MathWinNum = item.MathWinNum;
							friendData.PictureFrame = item.PictureFrame;
							friendData.Status = item.Status;
							friendData.SummId = item.SummId;
							friendData.TargetId = item.TargetId;
							friendData.TargetName = item.TargetName;
						}
						else
						{
							friendModelData.friendDataList.Add(item);
						}
					}
				}
				base.DebugMessage = "====>OK " + operationResponse.OperationCode;
			}
			base.Data = friendModelData;
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_ApplyAddFriend(OperationResponse operationResponse)
		{
			base.LastError = (int)operationResponse.Parameters[1];
			FriendModelData friendModelData = base.Data as FriendModelData;
			MobaErrorCode lastError = (MobaErrorCode)base.LastError;
			if (lastError != MobaErrorCode.Ok)
			{
				friendModelData.applyAddFriendOK = false;
			}
			else
			{
				friendModelData.applyAddFriendOK = true;
				base.DebugMessage = "====>OK " + operationResponse.OperationCode;
			}
			base.Data = friendModelData;
			base.Valid = (base.LastError == 0 || base.LastError == 20109 || base.LastError == 20105 || base.LastError == 20106 || base.LastError == 20107 || base.LastError == 20104 || base.LastError == 20101);
		}

		private void OnGetMsg_ModifyFriendStatus(OperationResponse operationResponse)
		{
			base.LastError = (int)operationResponse.Parameters[1];
			FriendModelData friendModelData = base.Data as FriendModelData;
			MobaErrorCode lastError = (MobaErrorCode)base.LastError;
			if (lastError != MobaErrorCode.Ok)
			{
				base.DebugMessage = "====>GetArenaDefTeam" + operationResponse.OperationCode;
				friendModelData.modifyFriendStatusOK = false;
			}
			else
			{
				friendModelData.modifyFriendStatusOK = true;
				byte b = (byte)operationResponse.Parameters[10];
				long targetSummid = (long)operationResponse.Parameters[102];
				friendModelData.friendState = new FriendState();
				friendModelData.friendState.opertype = b;
				friendModelData.friendState.targetSummid = targetSummid;
				byte b2 = b;
				if (b2 == 4)
				{
					friendModelData.friendDataList.RemoveAll((FriendData obj) => (int)obj.Status == 1 && obj.TargetId == targetSummid);
				}
				base.DebugMessage = "====>OK " + operationResponse.OperationCode;
			}
			base.Data = friendModelData;
			base.Valid = (base.LastError == 0 || base.LastError == 20105 || base.LastError == 20106);
		}

		private void OnGetMsg_GetUserInfoBySummId(OperationResponse operationResponse)
		{
			int num = (int)operationResponse.Parameters[1];
			FriendModelData friendModelData = base.Data as FriendModelData;
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				friendModelData.getUserInfoBySummIdOK = false;
				base.DebugMessage = "====>GetUserInfoBySummIdResponse" + operationResponse.OperationCode;
			}
			else
			{
				friendModelData.getUserInfoBySummIdOK = true;
				byte[] buffer = operationResponse.Parameters[86] as byte[];
				friendModelData.userInfo = SerializeHelper.Deserialize<FriendData>(buffer);
			}
			base.Data = friendModelData;
			base.Valid = true;
		}

		private void OnGetMsg_GetFriendMessages(OperationResponse operationResponse)
		{
			base.LastError = (int)operationResponse.Parameters[1];
			FriendModelData friendModelData = base.Data as FriendModelData;
			MobaErrorCode lastError = (MobaErrorCode)base.LastError;
			if (lastError != MobaErrorCode.Ok)
			{
				base.DebugMessage = "====>GetArenaDefTeam" + operationResponse.OperationCode;
				friendModelData.getFriendMessagesOK = false;
			}
			else
			{
				friendModelData.getFriendMessagesOK = true;
				byte[] buffer = operationResponse.Parameters[12] as byte[];
				List<NotificationData> list = SerializeHelper.Deserialize<List<NotificationData>>(buffer);
				friendModelData.notificationDataList = list;
				if (friendModelData.friendDataList != null && friendModelData.friendDataList.Count >= 1)
				{
					foreach (NotificationData current in list)
					{
						string[] subItem = current.Content.Split(new char[]
						{
							'|'
						});
						FriendData friendData = friendModelData.friendDataList.Find((FriendData obj) => obj.TargetId == (long)int.Parse(subItem[0]));
						if (friendData != null)
						{
							if (friendData.Messages == null)
							{
								friendData.Messages = new List<Messages>();
							}
							friendData.Messages.Add(new Messages
							{
								Time = subItem[subItem.Length - 1],
								Content = subItem[1]
							});
						}
					}
					base.DebugMessage = "====>OK " + operationResponse.OperationCode;
				}
			}
			base.Data = friendModelData;
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_GetFriendList(OperationResponse operationResponse)
		{
			base.LastError = (int)operationResponse.Parameters[1];
			if (base.LastError == 0)
			{
				byte[] buffer = operationResponse.Parameters[27] as byte[];
				List<FriendData> list = SerializeHelper.Deserialize<List<FriendData>>(buffer);
				FriendModelData friendModelData = base.Data as FriendModelData;
				friendModelData.friendDataList = new List<FriendData>();
				foreach (FriendData item in list)
				{
					sbyte status = item.Status;
					if (status == 1)
					{
						FriendData friendData = friendModelData.friendDataList.Find((FriendData obj) => obj.TargetId == item.TargetId);
						if (friendData != null)
						{
							friendData.Exp = item.Exp;
							friendData.GameStatus = item.GameStatus;
							friendData.Icon = item.Icon;
							friendData.LadderRank = item.LadderRank;
							friendData.LadderScore = item.LadderScore;
							friendData.MathWinNum = item.MathWinNum;
							friendData.PictureFrame = item.PictureFrame;
							friendData.Status = item.Status;
							friendData.SummId = item.SummId;
							friendData.TargetId = item.TargetId;
							friendData.TargetName = item.TargetName;
						}
						else
						{
							friendModelData.friendDataList.Add(item);
						}
					}
				}
				base.Data = friendModelData;
				base.DebugMessage = "====>OK " + operationResponse.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>GetArenaDefTeam" + operationResponse.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_SystemNotice(OperationResponse operationResponse)
		{
			if (operationResponse == null)
			{
				ClientLogger.Error("Model_Friend:OnGetMsg_SystemNotice:operationResponse=null");
				return;
			}
			if (operationResponse.Parameters == null || !operationResponse.Parameters.ContainsKey(1))
			{
				ClientLogger.Error("Model_Friend:OnGetMsg_SystemNotice:operationResponse.Parameters.ContainsKey((byte)MobaOpKey.ErrorCode)");
				return;
			}
			base.LastError = (int)operationResponse.Parameters[1];
			FriendModelData friendModelData = base.Data as FriendModelData;
			if (friendModelData == null)
			{
				ClientLogger.Error("Model_Friend:OnGetMsg_SystemNotice:data=null");
				return;
			}
			MobaErrorCode lastError = (MobaErrorCode)base.LastError;
			if (lastError != MobaErrorCode.Ok)
			{
				base.DebugMessage = "====>GetArenaDefTeam" + operationResponse.OperationCode;
			}
			else if (!operationResponse.Parameters.ContainsKey(40))
			{
				ClientLogger.Error("Model_Friend:OnGetMsg_SystemNotice:operationResponse[MobaOpKey.Notification]==null");
			}
			else
			{
				byte[] buffer = operationResponse.Parameters[40] as byte[];
				NotificationData notificationData = SerializeHelper.Deserialize<NotificationData>(buffer);
				if (notificationData == null)
				{
					ClientLogger.Error("Model_Friend:OnGetMsg_SystemNotice:deserialize failed");
				}
				else
				{
					short type = notificationData.Type;
					if (type != 14)
					{
						if (type != 15)
						{
							if (type == 9)
							{
								string[] subItem = notificationData.Content.Split(new char[]
								{
									'|'
								});
								FriendData friendData = friendModelData.friendDataList.Find((FriendData obj) => obj.TargetId == (long)int.Parse(subItem[0]));
								if (friendData != null)
								{
									if (friendData.Messages == null)
									{
										friendData.Messages = new List<Messages>();
									}
									friendData.Messages.Add(new Messages
									{
										Time = subItem[subItem.Length - 1],
										Content = subItem[1]
									});
								}
							}
						}
						else
						{
							string[] delsubItem = notificationData.Content.Split(new char[]
							{
								'|'
							});
							FriendData item = friendModelData.friendDataList.Find((FriendData obj) => obj.TargetId == (long)int.Parse(delsubItem[delsubItem.Length - 1]));
							friendModelData.friendDataList.Remove(item);
						}
					}
					else
					{
						string[] blacksubItem = notificationData.Content.Split(new char[]
						{
							'|'
						});
						friendModelData.friendDataList.RemoveAll((FriendData obj) => obj.TargetId == long.Parse(blacksubItem[1]));
					}
					base.DebugMessage = "====>OK " + operationResponse.OperationCode;
				}
			}
			base.Data = friendModelData;
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_RadarFindFriend(OperationResponse operationResponse)
		{
			int num = (int)operationResponse.Parameters[1];
			FriendModelData friendModelData = base.Data as FriendModelData;
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				base.DebugMessage = "====>RaderFriendList" + operationResponse.OperationCode;
			}
			else
			{
				byte[] buffer = operationResponse.Parameters[29] as byte[];
				friendModelData.radarFriendList = SerializeHelper.Deserialize<List<FriendData>>(buffer);
			}
			base.Data = friendModelData;
			base.Valid = (base.LastError == 0);
		}
	}
}
