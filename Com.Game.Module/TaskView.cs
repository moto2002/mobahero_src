using Assets.Scripts.GUILogic.View.PropertyView;
using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	public class TaskView : BaseView<TaskView>
	{
		private const string summaryType = "1";

		private Transform leftPanel;

		private Transform leftChoose;

		private Transform taskSummary;

		private Transform rightPanel;

		private Transform taskPanel;

		private List<Transform> leftTras;

		private List<Transform> rightTras;

		private TaskDataManager taskDataManager;

		private TaskDisplayManager taskDisplayManager;

		public CoroutineManager coroutineManager;

		public GameObject taskRewardItem;

		private string oldLeftChoose = "1";

		private int searchModel;

		private List<string> pointId;

		public TaskView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "TaskView");
			this.WindowTitle = "成就";
		}

		public override void Init()
		{
			base.Init();
			this.leftPanel = this.transform.Find("Anchor/LeftAnchor/Panel");
			this.leftChoose = this.transform.Find("Anchor/LeftAnchor/Choose");
			this.taskSummary = this.transform.Find("Anchor/TaskSummary");
			this.rightPanel = this.taskSummary.Find("Panel");
			this.taskPanel = this.transform.Find("Anchor/TasklPanel");
			if (this.pointId == null)
			{
				this.pointId = new List<string>();
			}
			this.GetLeftTras();
			this.GetRightTras();
			this.CheckPoint();
			this.taskDataManager = new TaskDataManager();
			this.taskDisplayManager = new TaskDisplayManager();
			this.coroutineManager = new CoroutineManager();
		}

		private void GetLeftTras()
		{
			if (this.leftTras == null)
			{
				this.leftTras = new List<Transform>();
			}
			else
			{
				this.leftTras.Clear();
			}
			for (int i = 0; i < this.leftPanel.childCount; i++)
			{
				Transform child = this.leftPanel.GetChild(i);
				this.leftTras.Add(child);
				UIEventListener.Get(child.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickPanel);
			}
		}

		private void CheckPoint()
		{
			for (int i = 0; i < this.leftPanel.childCount; i++)
			{
				Transform child = this.leftPanel.GetChild(i);
				if (this.pointId.Contains(child.name))
				{
					child.Find("Warn").gameObject.SetActive(true);
				}
				else
				{
					child.Find("Warn").gameObject.SetActive(false);
				}
			}
			for (int j = 0; j < this.rightPanel.childCount; j++)
			{
				Transform child = this.rightPanel.GetChild(j);
				if (this.pointId.Contains(child.name))
				{
					child.Find("Warn").gameObject.SetActive(true);
				}
				else
				{
					child.Find("Warn").gameObject.SetActive(false);
				}
			}
		}

		private void GetRightTras()
		{
			if (this.rightTras == null)
			{
				this.rightTras = new List<Transform>();
			}
			else
			{
				this.rightTras.Clear();
			}
			for (int i = 0; i < this.rightPanel.childCount; i++)
			{
				Transform child = this.rightPanel.GetChild(i);
				if (this.pointId.Contains(child.name))
				{
					child.Find("Warn").gameObject.SetActive(true);
				}
				else
				{
					child.Find("Warn").gameObject.SetActive(false);
				}
				this.rightTras.Add(child);
				UIEventListener.Get(child.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickPanel);
			}
		}

		public void NewbieAutoSelBattleItem()
		{
			this.ClickPanel("201");
		}

		public void NewbieGetAchieveAwd()
		{
			if (this.taskDisplayManager != null)
			{
				this.taskDisplayManager.NewbieGetAchieveAwd();
			}
		}

		private void ClickPanel(GameObject obj)
		{
			this.ClickPanel(obj.name);
		}

		public void ClickPanel(string goName)
		{
			if (this.oldLeftChoose == goName)
			{
				return;
			}
			this.ChangeLeftChoose(goName);
			if (goName == "1")
			{
				this.searchModel = 1;
				this.OnGetTaskList(null);
			}
			else
			{
				this.searchModel = int.Parse(goName);
				this.OnGetTaskList(null);
			}
		}

		private void ShowList(string typeName)
		{
			if (this.taskSummary.gameObject.activeSelf)
			{
				this.taskSummary.gameObject.SetActive(false);
			}
			if (!this.taskPanel.gameObject.activeSelf)
			{
				this.taskPanel.gameObject.SetActive(true);
			}
			this.taskDisplayManager.ClearTaskList();
			this.taskDisplayManager.CreateOrUpdataAchieveList(this.taskDataManager.GetListData(typeName), this.taskPanel);
			if (this.taskDataManager.GetListData(typeName).Find((DetailAchieveData obj) => obj.isComplete && !obj.isGetAward) == null)
			{
				this.ShowPoint(typeName, false);
			}
			else
			{
				this.ShowPoint(typeName, true);
			}
		}

		private void ChangeLeftChoose(string name)
		{
			Transform transform;
			if (!(this.oldLeftChoose == string.Empty))
			{
				transform = this.leftPanel.Find(this.oldLeftChoose);
				transform.GetComponent<UISprite>().color = new Color(0f, 0.917647064f, 0.9764706f);
				transform.Find("Label").GetComponent<UILabel>().color = new Color(0f, 0.917647064f, 0.9764706f);
			}
			transform = this.leftPanel.Find(name);
			transform.GetComponent<UISprite>().color = new Color(0.996078432f, 0.831372559f, 0f);
			transform.Find("Label").GetComponent<UILabel>().color = new Color(0.996078432f, 0.831372559f, 0f);
			this.leftChoose.localPosition = new Vector3(this.leftChoose.localPosition.x, 344f + transform.localPosition.y, 0f);
			this.oldLeftChoose = name;
		}

		public override void HandleAfterOpenView()
		{
			if (this.oldLeftChoose != "1")
			{
				this.ChangeLeftChoose("1");
			}
		}

		public override void HandleBeforeCloseView()
		{
		}

		public override void RegisterUpdateHandler()
		{
			Singleton<MenuTopBarView>.Instance.DoOpenOtherViewAction(0.25f);
			MVC_MessageManager.AddListener_view(MobaGameCode.GetTaskList, new MobaMessageFunc(this.OnGetTaskList));
			MVC_MessageManager.AddListener_view(MobaGameCode.GetAchieveTaskAward, new MobaMessageFunc(this.OnGetAchieveTaskAward));
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在加载", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetTaskList, param, new object[]
			{
				0
			});
			this.searchModel = 1;
		}

		private void OnGetTaskList(MobaMessage msg = null)
		{
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
			}
			AchieveAll achieveAll = ModelManager.Instance.Get_AchieveAll_X();
			if (achieveAll == null)
			{
				return;
			}
			for (int j = 0; j < achieveAll.detailAchieveDataList.Count; j++)
			{
				if (achieveAll.detailAchieveDataList[j].isComplete && !achieveAll.detailAchieveDataList[j].isGetAward)
				{
					this.ShowPoint(achieveAll.detailAchieveDataList[j].achieveid.ToString(), true);
				}
			}
			if (this.searchModel.ToString() == "1")
			{
				TaskView.<OnGetTaskList>c__AnonStorey29E <OnGetTaskList>c__AnonStorey29E = new TaskView.<OnGetTaskList>c__AnonStorey29E();
				<OnGetTaskList>c__AnonStorey29E.totalAchieveDataList = ModelManager.Instance.Get_AchieveAll_X().totalAchieveDataList;
				int num = 0;
				int i;
				for (i = 0; i < <OnGetTaskList>c__AnonStorey29E.totalAchieveDataList.Count; i++)
				{
					Transform transform = this.rightTras.Find((Transform obj) => obj.name == <OnGetTaskList>c__AnonStorey29E.totalAchieveDataList[i].achieveId.ToString());
					SysAchievementVo dataById = BaseDataMgr.instance.GetDataById<SysAchievementVo>(<OnGetTaskList>c__AnonStorey29E.totalAchieveDataList[i].achieveId.ToString());
					string content = BaseDataMgr.instance.GetLanguageData(dataById.name).content;
					transform.Find("Name").GetComponent<UILabel>().text = content;
					transform.Find("PointContainer/PointLeft").GetComponent<UILabel>().text = <OnGetTaskList>c__AnonStorey29E.totalAchieveDataList[i].achievepoint + "/";
					transform.Find("PointContainer/PointRight").GetComponent<UILabel>().text = dataById.achievement_point.ToString();
					transform.Find("PointContainer").GetComponent<UICenterHelper>().Reposition();
					this.leftTras.Find((Transform obj) => obj.name == <OnGetTaskList>c__AnonStorey29E.totalAchieveDataList[i].achieveId.ToString()).Find("Label").GetComponent<UILabel>().text = content;
					num += <OnGetTaskList>c__AnonStorey29E.totalAchieveDataList[i].achievepoint;
				}
				this.taskSummary.Find("Title/PointLeft").GetComponent<UILabel>().text = num.ToString();
				this.taskSummary.Find("Title/PointRight").GetComponent<UILabel>().text = "/" + BaseDataMgr.instance.GetDataById<SysAchievementVo>("1").achievement_point;
				this.ShowTaskSummary();
				this.CheckPoint();
			}
			else
			{
				this.ShowList(this.searchModel.ToString());
			}
		}

		private void OnGetAchieveTaskAward(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[123];
			Singleton<MenuBottomBarView>.Instance.RemoveNews(2, num.ToString());
			this.taskDisplayManager.PlayDuang(this.taskDataManager.GetTaskAwardData());
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在加载", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetTaskList, param, new object[]
			{
				this.searchModel
			});
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetTaskList, param, new object[]
			{
				1
			});
		}

		public override void CancelUpdateHandler()
		{
			MVC_MessageManager.RemoveListener_view(MobaGameCode.GetTaskList, new MobaMessageFunc(this.OnGetTaskList));
			MVC_MessageManager.RemoveListener_view(MobaGameCode.GetAchieveTaskAward, new MobaMessageFunc(this.OnGetAchieveTaskAward));
			this.taskDisplayManager.ClearTaskList();
		}

		public override void RefreshUI()
		{
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void ShowTaskSummary()
		{
			if (!this.taskSummary.gameObject.activeSelf)
			{
				this.taskSummary.gameObject.SetActive(true);
			}
			if (this.taskPanel.gameObject.activeSelf)
			{
				this.taskPanel.gameObject.SetActive(false);
			}
		}

		public void TravelView(string name)
		{
			if (name != null)
			{
				if (TaskView.<>f__switch$map2B == null)
				{
					TaskView.<>f__switch$map2B = new Dictionary<string, int>(11)
					{
						{
							"1",
							0
						},
						{
							"2",
							0
						},
						{
							"3",
							0
						},
						{
							"4",
							0
						},
						{
							"5",
							0
						},
						{
							"6",
							0
						},
						{
							"7",
							0
						},
						{
							"8",
							0
						},
						{
							"9",
							0
						},
						{
							"11",
							0
						},
						{
							"10",
							1
						}
					};
				}
				int num;
				if (TaskView.<>f__switch$map2B.TryGetValue(name, out num))
				{
					if (num != 0)
					{
						if (num == 1)
						{
							if (this.GetIsCoalesce())
							{
								CtrlManager.OpenWindow(WindowID.RunesOverView, null);
								MobaMessageManagerTools.SendClientMsg(ClientV2C.runesviewInitToggle, string.Empty, false);
							}
							else if (CharacterDataMgr.instance.OwenHeros.Count > 0)
							{
								CtrlManager.OpenWindow(WindowID.PropertyView, null);
								MobaMessageManagerTools.SendClientMsg(ClientV2C.sacriviewChangeHero, CharacterDataMgr.instance.OwenHeros[0], false);
								MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewInitToggle, PropertyType.Rune, false);
							}
							else
							{
								Singleton<TipView>.Instance.ShowViewSetText("当前没有已拥有的英雄，无法完成此任务", 1f);
							}
						}
					}
					else
					{
						int i = int.Parse(name);
						GotoWindowTools.GotoWindow(i);
					}
				}
			}
		}

		public void ShowPoint(string AchieveId, bool isShow = true)
		{
			if (this.pointId == null)
			{
				this.pointId = new List<string>();
			}
			if (isShow)
			{
				if (!this.pointId.Contains(AchieveId))
				{
					this.pointId.Add(AchieveId);
				}
				if (Singleton<TaskView>.Instance.gameObject)
				{
					if (this.leftTras != null)
					{
						this.leftTras.Find((Transform obj) => obj.name == AchieveId).Find("Warn").gameObject.SetActive(true);
					}
					if (this.rightTras != null)
					{
						this.rightTras.Find((Transform obj) => obj.name == AchieveId).Find("Warn").gameObject.SetActive(true);
					}
				}
			}
			else
			{
				this.leftTras.Find((Transform obj) => obj.name == AchieveId).Find("Warn").gameObject.SetActive(false);
				this.rightTras.Find((Transform obj) => obj.name == AchieveId).Find("Warn").gameObject.SetActive(false);
				if (this.pointId.Contains(AchieveId))
				{
					this.pointId.Remove(AchieveId);
				}
			}
		}

		public bool GetIsCoalesce()
		{
			return !(this.oldLeftChoose == "204");
		}
	}
}
