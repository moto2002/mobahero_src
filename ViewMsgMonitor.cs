using Assets.Scripts.Model;
using Assets.Scripts.Server;
using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

public class ViewMsgMonitor : IGlobalComServer
{
	private CoroutineManager m_CoroutineManager;

	private string oldNotificationDataContent = string.Empty;

	private object[] mgs;

	public void OnAwake()
	{
	}

	public void OnStart()
	{
		this.RegistHandler();
		this.m_CoroutineManager = new CoroutineManager();
	}

	public void OnUpdate()
	{
	}

	public void OnDestroy()
	{
		this.UnregistHandler();
	}

	public void Enable(bool b)
	{
	}

	public void OnRestart()
	{
	}

	public void OnApplicationQuit()
	{
	}

	public void OnApplicationFocus(bool isFocus)
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)25063, isFocus, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	public void OnApplicationPause(bool isPause)
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)25064, isPause, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	private void RegistHandler()
	{
		this.mgs = new object[]
		{
			MobaUserDataCode.UD_StatusChanged,
			MobaChatCode.Chat_NoticePrivateNewMsg,
			MobaUserDataCode.UD_ClientRichGiftUser
		};
		MVC_MessageManager.AddListener_view(MobaGameCode.CompleteTaskMessage, new MobaMessageFunc(this.OnGetMsg_CompleteTaskMessage));
		MVC_MessageManager.AddListener_view(MobaGameCode.SystemNotice, new MobaMessageFunc(this.OnGetMsg_SystemNotice));
		MVC_MessageManager.AddListener_view(MobaGameCode.GetMailList, new MobaMessageFunc(this.OnGetMailList));
		MVC_MessageManager.AddListener_view(MobaGameCode.GetChargeInfo, new MobaMessageFunc(this.OnGetChargeInfo));
		MVC_MessageManager.AddListener_view(MobaGameCode.GetCurrencyCount, new MobaMessageFunc(this.OnGetCurrencyCount));
		MVC_MessageManager.AddListener_view(MobaFriendCode.Friend_StatusChange, new MobaMessageFunc(this.OnGetMsg_Friend_StatusChange));
		MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_InviteJoinRoom, new MobaMessageFunc(this.OnGetMsg_Room_InviteJoinRoom));
		MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_NotifyErrorInfo, new MobaMessageFunc(this.OnGetMsg_Room_NotifyErrorInfo));
		Singleton<PvpRoomView>.Instance.RegisterCallBack();
		MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
	}

	private void UnregistHandler()
	{
		MVC_MessageManager.RemoveListener_view(MobaGameCode.CompleteTaskMessage, new MobaMessageFunc(this.OnGetMsg_CompleteTaskMessage));
		MVC_MessageManager.RemoveListener_view(MobaGameCode.SystemNotice, new MobaMessageFunc(this.OnGetMsg_SystemNotice));
		MVC_MessageManager.RemoveListener_view(MobaGameCode.GetMailList, new MobaMessageFunc(this.OnGetMailList));
		MVC_MessageManager.RemoveListener_view(MobaGameCode.GetChargeInfo, new MobaMessageFunc(this.OnGetChargeInfo));
		MVC_MessageManager.RemoveListener_view(MobaGameCode.GetCurrencyCount, new MobaMessageFunc(this.OnGetCurrencyCount));
		MVC_MessageManager.RemoveListener_view(MobaFriendCode.Friend_StatusChange, new MobaMessageFunc(this.OnGetMsg_Friend_StatusChange));
		MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_InviteJoinRoom, new MobaMessageFunc(this.OnGetMsg_Room_InviteJoinRoom));
		MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_NotifyErrorInfo, new MobaMessageFunc(this.OnGetMsg_Room_NotifyErrorInfo));
		Singleton<PvpRoomView>.Instance.CancelCallBack();
		MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
	}

	private void OnGetMsg_Room_NotifyErrorInfo(MobaMessage msg)
	{
		this.OnGetMsg_SystemNotice(msg);
	}

	private void OnGetMsg_Room_InviteJoinRoom(MobaMessage msg)
	{
		this.OnGetMsg_SystemNotice(msg);
	}

	private void OnGetMsg_Friend_StatusChange(MobaMessage msg)
	{
		OperationResponse operationResponse = msg.Param as OperationResponse;
		if (operationResponse == null)
		{
			return;
		}
		int num = (int)operationResponse.Parameters[1];
		MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
		if (mobaErrorCode != MobaErrorCode.Ok)
		{
			this.OnNotificationEvent(num, operationResponse.DebugMessage, null);
		}
		else
		{
			byte[] buffer = operationResponse.Parameters[40] as byte[];
			NotificationData notificationData = SerializeHelper.Deserialize<NotificationData>(buffer);
			if (notificationData.Type == 14 || notificationData.Type == 15)
			{
				string[] array = notificationData.Content.Split(new char[]
				{
					'|'
				});
				if (array.Length > 1)
				{
					string s = array[array.Length - 1];
					if (Singleton<FriendView>.Instance.newMessageList.Contains(ToolsFacade.Instance.GetUserIdBySummId(long.Parse(s))))
					{
						Singleton<FriendView>.Instance.newMessageList.Remove(ToolsFacade.Instance.GetUserIdBySummId(long.Parse(s)));
					}
				}
				Singleton<MenuView>.Instance.UpdateFriendNew();
			}
			this.OnNotificationEvent(num, operationResponse.DebugMessage, notificationData);
		}
	}

	private void OnGetMsg_SystemNotice(MobaMessage msg)
	{
		OperationResponse operationResponse = msg.Param as OperationResponse;
		if (operationResponse == null)
		{
			return;
		}
		int num = (int)operationResponse.Parameters[1];
		MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
		if (mobaErrorCode != MobaErrorCode.Ok)
		{
			this.OnNotificationEvent(num, operationResponse.DebugMessage, null);
		}
		else
		{
			byte[] buffer = operationResponse.Parameters[40] as byte[];
			NotificationData notificationData = SerializeHelper.Deserialize<NotificationData>(buffer);
			this.OnNotificationEvent(num, operationResponse.DebugMessage, notificationData);
		}
	}

	private void OnGetMsg_OnChangeGroupst(MobaMessage msg)
	{
		if (msg == null)
		{
			return;
		}
		OperationResponse operationResponse = msg.Param as OperationResponse;
		if (operationResponse == null)
		{
			return;
		}
		int num = (int)operationResponse.Parameters[1];
		MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
		if (mobaErrorCode != MobaErrorCode.Ok)
		{
			this.OnNotificationEvent(num, operationResponse.DebugMessage, null);
		}
		else
		{
			byte[] buffer = operationResponse.Parameters[40] as byte[];
			NotificationData notificationData = SerializeHelper.Deserialize<NotificationData>(buffer);
			this.OnNotificationEvent(num, operationResponse.DebugMessage, notificationData);
		}
	}

	private void OnGetMsg_CompleteTaskMessage(MobaMessage msg)
	{
		OperationResponse operationResponse = msg.Param as OperationResponse;
		if (operationResponse == null)
		{
			return;
		}
		int num = (int)operationResponse.Parameters[1];
		MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
		if (mobaErrorCode == MobaErrorCode.Ok)
		{
			int num2 = (int)operationResponse.Parameters[123];
			int num3 = (int)operationResponse.Parameters[252];
			int num4 = (int)operationResponse.Parameters[241];
			this.OnCompleteTaskMessage(num4, num2, num3);
			if (num4 == 1)
			{
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetTaskList, null, new object[]
				{
					num3
				});
			}
			else if (num4 == 3 && (num2 == 10101 || num2 == 10102))
			{
				Singleton<MenuView>.Instance.UpdateFirstPay();
			}
		}
	}

	public void OnCompleteTaskMessage(int type, int taskId, int AchieveId)
	{
		AchieveAll achieveAll = ModelManager.Instance.Get_AchieveAll_X();
		if (achieveAll == null)
		{
			return;
		}
		if (type == 1)
		{
			Singleton<MenuBottomBarView>.Instance.SetNews(2, taskId.ToString());
			DetailAchieveData detailAchieveData = achieveAll.detailAchieveDataList.Find((DetailAchieveData obj) => obj.taskid == taskId);
			detailAchieveData.isComplete = true;
		}
		else if (type == 2)
		{
			Singleton<MenuBottomBarView>.Instance.SetNews(1, taskId.ToString());
			DailyTaskData dailyTaskData = achieveAll.dailyTaskList.Find((DailyTaskData obj) => obj.taskid == taskId);
			dailyTaskData.isComplete = true;
		}
		if (GlobalSettings.isLoginByHoolaiSDK)
		{
			AnalyticsToolManager.Quest(taskId.ToString(), CharacterDataMgr.instance.GetUserLevel(ModelManager.Instance.Get_userData_X().Exp));
		}
	}

	private void OnGetMailList(MobaMessage msg)
	{
		if (msg == null)
		{
			return;
		}
		OperationResponse operationResponse = msg.Param as OperationResponse;
		if (operationResponse == null)
		{
			return;
		}
		int num = (int)operationResponse.Parameters[1];
		MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
		if (mobaErrorCode == MobaErrorCode.Ok)
		{
			Singleton<MenuBottomBarView>.Instance.CheckEmailNewsState();
			if (null != Singleton<MailView>.Instance.gameObject && Singleton<MailView>.Instance.gameObject.activeInHierarchy)
			{
				Singleton<MailView>.Instance.UpdateMailView();
			}
		}
	}

	private void OnGetChargeInfo(MobaMessage msg)
	{
		if (msg == null)
		{
			return;
		}
		OperationResponse operationResponse = msg.Param as OperationResponse;
		if (operationResponse == null)
		{
			return;
		}
		int num = (int)operationResponse.Parameters[1];
		MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
		if (mobaErrorCode == MobaErrorCode.Ok)
		{
			PayDimondData payDimondData = SerializeHelper.Deserialize<PayDimondData>(operationResponse.Parameters[84] as byte[]);
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(false, "刷新数据...", true, 15f);
			int num2 = 2;
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetCurrencyCount, param, new object[]
			{
				num2
			});
		}
	}

	private void OnGetCurrencyCount(MobaMessage msg)
	{
		if (msg == null)
		{
			return;
		}
		OperationResponse operationResponse = msg.Param as OperationResponse;
		if (operationResponse == null)
		{
			return;
		}
		int num = (int)operationResponse.Parameters[1];
		MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
		if (mobaErrorCode == MobaErrorCode.Ok)
		{
			int num2 = (int)operationResponse.Parameters[10];
			if (num2 == 0)
			{
				ModelManager.Instance.Get_userData_X().Money = (long)operationResponse.Parameters[61];
				ModelManager.Instance.Get_userData_X().Diamonds = (long)operationResponse.Parameters[62];
				ModelManager.Instance.Get_userData_X().SmallCap = (int)operationResponse.Parameters[101];
				ModelManager.Instance.Get_userData_X().Speaker = (int)operationResponse.Parameters[11];
			}
			else if (num2 == 1)
			{
				long money = (long)operationResponse.Parameters[101];
				ModelManager.Instance.Get_userData_X().Money = money;
			}
			else if (num2 == 2)
			{
				long diamonds = (long)operationResponse.Parameters[101];
				ModelManager.Instance.Get_userData_X().Diamonds = diamonds;
			}
			else if (num2 == 9)
			{
				int smallCap = (int)operationResponse.Parameters[101];
				ModelManager.Instance.Get_userData_X().SmallCap = smallCap;
			}
			if (Singleton<MenuTopBarView>.Instance.gameObject)
			{
				Singleton<MenuTopBarView>.Instance.RefreshUI();
			}
		}
	}

	private void OnMsg_UD_StatusChanged(MobaMessage msg)
	{
		OperationResponse operationResponse = msg.Param as OperationResponse;
		if (operationResponse == null)
		{
			return;
		}
		int num = (int)operationResponse.Parameters[1];
		MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
		if (mobaErrorCode != MobaErrorCode.Ok)
		{
			this.OnNotificationEvent(num, operationResponse.DebugMessage, null);
		}
		else
		{
			byte[] buffer = operationResponse.Parameters[40] as byte[];
			NotificationData notificationData = SerializeHelper.Deserialize<NotificationData>(buffer);
			this.OnNotificationEvent(num, operationResponse.DebugMessage, notificationData);
		}
	}

	private void OnMsg_Chat_NoticePrivateNewMsg(MobaMessage msg)
	{
		OperationResponse operationResponse = msg.Param as OperationResponse;
		if (operationResponse == null)
		{
			return;
		}
		string content = (string)operationResponse.Parameters[100];
		NotificationData notificationData = new NotificationData();
		notificationData.Type = 9;
		notificationData.Content = content;
		this.OnNotificationEvent(0, operationResponse.DebugMessage, notificationData);
	}

	private void OnMsg_UD_ClientRichGiftUser(MobaMessage msg)
	{
		Singleton<MenuView>.Instance.UpdateTuhaoHongBao();
	}

	private void OnNotificationEvent(int ret, string debugMessage, NotificationData notificationData)
	{
		if (notificationData == null || notificationData.Content == null)
		{
			return;
		}
		if (ret == 0)
		{
			this.oldNotificationDataContent = notificationData.Content;
			short type = notificationData.Type;
			switch (type + 5)
			{
			case 0:
				MobaMessageManager.ExecuteMsg(MobaMessageManager.GetMessage((ClientMsg)23066, notificationData, 0f));
				break;
			case 1:
				MobaMessageManager.DispatchMsg(MobaMessageManager.GetMessage((ClientMsg)25024, null, 0f));
				NetWorkHelper.Instance.Enable(false);
				CtrlManager.ShowMsgBox("服务器提示", notificationData.Content, new Action(GlobalObject.QuitApp), PopViewType.PopOneButton, "确定", "取消", null);
				break;
			case 2:
				NetWorkHelper.Instance.Enable(false);
				CtrlManager.ShowMsgBox("服务器维护", "请点击确认，退出游戏", new Action(GlobalObject.QuitApp), PopViewType.PopOneButton, "确定", "取消", null);
				break;
			case 3:
				CtrlManager.ShowMsgBox("服务器提示", notificationData.Content, null, PopViewType.PopOneButton, "确定", "取消", null);
				break;
			case 4:
				CtrlManager.ShowMsgBox("服务器重启提示", "服务器已经重启，客户端将在5秒后强制退出，退出后请尝试重新登入！", new Action(GlobalObject.ReStartGame), PopViewType.PopOneButton, "立刻退出", "取消", null);
				break;
			case 5:
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetCurrencyCount, null, new object[]
				{
					1
				});
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetCurrencyCount, null, new object[]
				{
					2
				});
				break;
			case 12:
				Singleton<FriendView>.Instance.UpdateNewApply(notificationData.Content);
				Singleton<PvpRoomView>.Instance.UpdateApplyState(notificationData.Content);
				Singleton<HomeChatview>.Instance.UpdateFriendList(true, string.Empty);
				break;
			case 14:
				Singleton<FriendView>.Instance.AddMessage(notificationData.Content);
				Singleton<HomeChatview>.Instance.AddFriendMessage(notificationData.Content);
				Singleton<MenuView>.Instance.SetNews(14, "0");
				break;
			case 15:
				Singleton<FriendView>.Instance.UpdateNewApply(notificationData.Content);
				Singleton<PvpRoomView>.Instance.UpdateApplyState(notificationData.Content);
				break;
			case 16:
				Singleton<PvpRoomView>.Instance.InviteGoToRoom(notificationData.Content);
				break;
			case 17:
				if (Singleton<PvpRoomView>.Instance.gameObject != null && Singleton<PvpRoomView>.Instance.gameObject.activeInHierarchy)
				{
					Singleton<PvpRoomView>.Instance.RefuseGoToRoom(notificationData.Content);
				}
				break;
			case 19:
				Singleton<FriendView>.Instance.UpdateApplyState(notificationData.Content, true);
				Singleton<PvpRoomView>.Instance.UpdateApplyState(notificationData.Content);
				if (Singleton<HomeChatview>.Instance.gameObject != null && Singleton<HomeChatview>.Instance.gameObject.activeInHierarchy)
				{
					Singleton<HomeChatview>.Instance.UnFriended();
				}
				Singleton<HomeChatview>.Instance.UpdateFriendList(false, notificationData.Content);
				break;
			case 20:
				Singleton<FriendView>.Instance.UpdateApplyState(notificationData.Content, true);
				Singleton<PvpRoomView>.Instance.UpdateApplyState(notificationData.Content);
				if (Singleton<HomeChatview>.Instance.gameObject != null && Singleton<HomeChatview>.Instance.gameObject.activeInHierarchy)
				{
					Singleton<HomeChatview>.Instance.UnFriended();
				}
				Singleton<HomeChatview>.Instance.UpdateFriendList(false, notificationData.Content);
				break;
			case 23:
			{
				if (Singleton<FriendView>.Instance.gameObject != null && Singleton<FriendView>.Instance.gameObject.activeInHierarchy)
				{
					Singleton<FriendView>.Instance.UpdateFriendState(notificationData.Content);
				}
				if (Singleton<PvpRoomView>.Instance.gameObject != null && Singleton<PvpRoomView>.Instance.gameObject.activeInHierarchy)
				{
					Singleton<PvpRoomView>.Instance.UpdateFriendState(notificationData.Content);
				}
				if (Singleton<HomeChatview>.Instance.gameObject != null)
				{
					Singleton<HomeChatview>.Instance.UpdateFriendState(notificationData.Content);
				}
				string[] stateStrs = notificationData.Content.Split(new char[]
				{
					'|'
				});
				if (stateStrs != null)
				{
					if (stateStrs.Length >= 3)
					{
						List<FriendData> list = ModelManager.Instance.Get_FriendDataList_X();
						if (list != null)
						{
							FriendData friendData = list.Find((FriendData _obj) => _obj.TargetId.ToString() == stateStrs[1]);
							if (friendData != null)
							{
								friendData.GameStatus = (sbyte)int.Parse(stateStrs[2]);
							}
						}
					}
					Singleton<MenuBottomBarView>.Instance.UpdateFriendNum();
				}
				break;
			}
			case 24:
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetMailList, null, new object[0]);
				break;
			case 30:
				Singleton<TipView>.Instance.ShowViewSetText("已经在房间中!", 1f);
				Singleton<PvpRoomView>.Instance.ErrorToLeve();
				break;
			case 31:
				Singleton<TipView>.Instance.ShowViewSetText("房间关闭!", 1f);
				break;
			case 32:
				Singleton<TipView>.Instance.ShowViewSetText("房间已经满员!", 1f);
				break;
			case 33:
				Singleton<TipView>.Instance.ShowViewSetText("房间已经不存在!", 1f);
				break;
			case 34:
				Singleton<TipView>.Instance.ShowViewSetText("不是房主无法操作!", 1f);
				break;
			case 35:
				Singleton<TipView>.Instance.ShowViewSetText("当前无法修改房间的类型!", 1f);
				break;
			case 36:
				Singleton<TipView>.Instance.ShowViewSetText("不在房间中!", 1f);
				break;
			}
		}
	}
}
