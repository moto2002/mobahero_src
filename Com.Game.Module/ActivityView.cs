using GUIFramework;
using MobaMessageData;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class ActivityView : BaseView<ActivityView>
	{
		private Activity_top com_top;

		private Activity_left com_left;

		private Activity_center com_center;

		private object[] msgs;

		private bool bMsgTask;

		private bool bMsgNotice;

		private bool bDoRefresh;

		private bool bDoStartOpen;

		public ActivityView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Activity/ActivityView");
		}

		public override void Init()
		{
			base.Init();
			this.msgs = new object[]
			{
				ClientV2V.Activity_setCurActivity,
				ClientV2C.activity_close,
				ClientC2V.Receive_GetActivityTask,
				ClientC2V.Receive_GetNoticeBoard,
				ClientC2V.ShowActivityNotice,
				ClientNet.Connected_gate
			};
			this.com_top = this.gameObject.transform.FindChild("Top").GetComponent<Activity_top>();
			this.com_left = this.gameObject.transform.FindChild("Left").GetComponent<Activity_left>();
			this.com_center = this.gameObject.transform.FindChild("Content").GetComponent<Activity_center>();
			GameObject gameObject = this.gameObject.transform.FindChild("close").gameObject;
			UIEventListener.Get(gameObject).onClick = new UIEventListener.VoidDelegate(this.onClick_close);
			this.com_top.OnTopTitleChange = new Action(this.OnSelectTopItem);
			this.com_left.OnCurActivityChange = new Action(this.OnSelectLeftItem);
		}

		public override void RegisterUpdateHandler()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, true, "OnMsg_");
		}

		public override void CancelUpdateHandler()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, false, "OnMsg_");
		}

		public override void HandleAfterOpenView()
		{
			base.HandleAfterOpenView();
			this.bMsgTask = false;
			this.bMsgNotice = false;
			this.bDoRefresh = false;
			this.bDoStartOpen = false;
			MobaMessageManagerTools.SendClientMsg(ClientV2V.Activity_openView, null, false);
		}

		public override void HandleBeforeCloseView()
		{
			base.HandleBeforeCloseView();
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void onClick_close(GameObject go)
		{
			this.Close();
		}

		private void OnSelectLeftItem()
		{
			this.com_center.CurItemInfo = this.com_left.CurItemInfo;
			this.RefreshUI_center();
		}

		private void OnSelectTopItem()
		{
			this.com_left.AType = this.com_top.AType;
			this.com_left.CurTypeID = this.com_top.CurTitle;
			this.com_left.CurLeftItemIndex = 0;
			this.com_center.AType = this.com_top.AType;
			this.com_center.CurItemInfo = this.com_left.CurItemInfo;
			this.RefreshUI_center();
		}

		private void OnMsg_Activity_setCurActivity(MobaMessage msg)
		{
			MsgData_Activity_setCurActivity msgData_Activity_setCurActivity = msg.Param as MsgData_Activity_setCurActivity;
			this.com_top.CurTitle = msgData_Activity_setCurActivity.activity_typeID;
			this.com_left.AType = EActivityType.eActivity;
			this.com_left.CurTypeID = msgData_Activity_setCurActivity.activity_typeID;
			this.com_left.CurLeftItemID = msgData_Activity_setCurActivity.activity_id;
			this.com_center.AType = EActivityType.eActivity;
			this.com_center.CurItemInfo = this.com_left.CurItemInfo;
			this.bDoRefresh = true;
			this.RefreshUI_center();
		}

		private void OnMsg_ShowActivityNotice(MobaMessage msg)
		{
			this.bDoStartOpen = true;
			this.RefreshUI_center();
		}

		private void OnMsg_activity_close(MobaMessage msg)
		{
			this.Close();
		}

		private void OnMsg_Receive_GetActivityTask(MobaMessage msg)
		{
			this.bMsgTask = true;
			this.RefreshUI_center();
		}

		private void OnMsg_Receive_GetNoticeBoard(MobaMessage msg)
		{
			this.bMsgNotice = true;
			this.RefreshUI_center();
		}

		private void OnMsg_Connected_gate(MobaMessage msg)
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2V.Activity_openView, null, false);
			this.RefreshUI_center();
		}

		private void RefreshUI_center()
		{
			if (this.bMsgTask && this.bMsgNotice && (this.bDoRefresh || this.bDoStartOpen))
			{
				if (this.bDoStartOpen)
				{
					this.bDoStartOpen = false;
					this.bDoRefresh = true;
					this.ChooseFirstNotice();
				}
				this.com_center.RefreshUI_content();
			}
		}

		private void ChooseFirstNotice()
		{
			this.com_top.CurTitle = -1;
			this.com_left.AType = this.com_top.AType;
			this.com_left.CurTypeID = this.com_top.CurTitle;
			this.com_left.CurLeftItemIndex = 0;
			this.com_center.AType = this.com_top.AType;
			this.com_center.CurItemInfo = this.com_left.CurItemInfo;
		}

		private void Close()
		{
			CtrlManager.CloseWindow(WindowID.ActivityView);
			Singleton<MenuView>.Instance.CheckActivityState();
		}

		public void NewbieSelActivityByType(int inActivityTypeId)
		{
			if (this.com_top != null)
			{
				this.com_top.NewbieSelActivityByType(inActivityTypeId);
			}
		}

		public void NewbieSelActByActivityId(int inActivityId)
		{
			if (this.com_left != null)
			{
				this.com_left.NewbieSelActByActivityId(inActivityId);
			}
		}

		public bool NewbieGetNewbieActAwd()
		{
			return this.com_center != null && this.com_center.NewbieGetFirstAwd();
		}

		public bool NewbieGetLoginAwd()
		{
			return this.com_center != null && this.com_center.NewbieGetFirstAwd();
		}
	}
}
