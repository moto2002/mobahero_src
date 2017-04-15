using Assets.Scripts.Model;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaProtocol;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class DailyView : BaseView<DailyView>
	{
		private Transform taskPanel;

		private TaskDataManager taskDataManager;

		private TaskDisplayManager taskDisplayManager;

		public CoroutineManager coroutineManager;

		public GameObject taskRewardItem;

		public DailyView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Home/DailyView");
			this.WindowTitle = "日常";
		}

		public override void Init()
		{
			base.Init();
			this.taskPanel = this.transform.Find("Anchor/TasklPanel");
			this.taskDataManager = new TaskDataManager();
			this.taskDisplayManager = new TaskDisplayManager();
			this.coroutineManager = new CoroutineManager();
		}

		public override void HandleAfterOpenView()
		{
		}

		public override void HandleBeforeCloseView()
		{
		}

		public override void RegisterUpdateHandler()
		{
			Singleton<MenuTopBarView>.Instance.DoOpenOtherViewAction(0.25f);
			MVC_MessageManager.AddListener_view(MobaGameCode.ShowDailyTask, new MobaMessageFunc(this.OnGetMsg_ShowDailyTask));
			MVC_MessageManager.AddListener_view(MobaGameCode.GetDailyTaskAward, new MobaMessageFunc(this.OnGetDailyTaskAward));
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在加载", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.ShowDailyTask, param, new object[0]);
		}

		private void OnGetMsg_ShowDailyTask(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			this.taskDisplayManager.ClearTaskList();
			this.taskDisplayManager.CreateOrUpdataDailyList(this.taskDataManager.GetDailyTasListData(), this.taskPanel);
		}

		public override void CancelUpdateHandler()
		{
			MVC_MessageManager.RemoveListener_view(MobaGameCode.ShowDailyTask, new MobaMessageFunc(this.OnGetMsg_ShowDailyTask));
			MVC_MessageManager.RemoveListener_view(MobaGameCode.GetDailyTaskAward, new MobaMessageFunc(this.OnGetDailyTaskAward));
			this.taskDisplayManager.ClearTaskList();
		}

		public override void RefreshUI()
		{
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void OnGetDailyTaskAward(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[123];
			Singleton<MenuBottomBarView>.Instance.RemoveNews(1, num.ToString());
			this.taskDisplayManager.PlayDuang(this.taskDataManager.GetTaskAwardData());
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在加载", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.ShowDailyTask, param, new object[0]);
		}

		public void NewbieGetDailyAwd()
		{
			if (this.taskDisplayManager != null)
			{
				this.taskDisplayManager.NewbieGetDailyAwd();
			}
		}
	}
}
