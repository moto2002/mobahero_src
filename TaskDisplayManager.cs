using Assets.Scripts.Model;
using Com.Game.Module;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TaskDisplayManager
{
	private GameObject taskItemPre;

	private List<TaskItem> taskItemList;

	private UIScrollView m_ScrollView;

	private UIPanel m_Panel;

	private List<DetailAchieveData> m_taskData;

	private List<DailyTaskData> m_dailyData;

	private bool canClickGetReward = true;

	private float itemOffY = 200f;

	private int oldIndex = -1;

	private CoroutineManager cormgr = new CoroutineManager();

	private int listIndex;

	public void CreateOrUpdataAchieveList(List<DetailAchieveData> taskData, Transform parentTra)
	{
		if (taskData == null)
		{
			return;
		}
		this.m_taskData = taskData;
		this.m_dailyData = null;
		if (this.taskItemPre == null)
		{
			this.taskItemPre = (Resources.Load("Prefab/UI/Home/TaskItem") as GameObject);
		}
		this.m_ScrollView = parentTra.GetComponent<UIScrollView>();
		this.m_Panel = parentTra.GetComponent<UIPanel>();
		this.m_Panel.onClipMove = new UIPanel.OnClippingMoved(this.OnClipMove);
		if (this.taskItemList == null)
		{
			this.taskItemList = new List<TaskItem>();
		}
		this.CheckListCount(taskData, this.taskItemList);
		this.oldIndex = -1;
		this.UpdateItemList(0, true);
		this.m_ScrollView.ResetPosition();
	}

	public void CreateOrUpdataDailyList(List<DailyTaskData> dailyData, Transform parentTra)
	{
		if (dailyData == null)
		{
			return;
		}
		this.m_dailyData = dailyData;
		this.m_taskData = null;
		if (this.taskItemPre == null)
		{
			this.taskItemPre = (Resources.Load("Prefab/UI/Home/TaskItem") as GameObject);
		}
		this.m_ScrollView = parentTra.GetComponent<UIScrollView>();
		this.m_Panel = parentTra.GetComponent<UIPanel>();
		this.m_Panel.onClipMove = new UIPanel.OnClippingMoved(this.OnClipMove);
		if (this.taskItemList == null)
		{
			this.taskItemList = new List<TaskItem>();
		}
		this.CheckListCount(dailyData, this.taskItemList);
		this.oldIndex = -1;
		this.UpdateItemList(0, true);
		this.m_ScrollView.ResetPosition();
	}

	private void UpdateItemList(int index, bool resetOldIndex = false)
	{
		if (resetOldIndex)
		{
			this.oldIndex = -1;
		}
		if (this.oldIndex == index)
		{
			return;
		}
		int num = index;
		while (true)
		{
			if (this.m_taskData != null)
			{
				if (num >= this.m_taskData.Count)
				{
					break;
				}
			}
			else if (num >= this.m_dailyData.Count)
			{
				break;
			}
			if (num == index + 6)
			{
				break;
			}
			if (num >= this.taskItemList.Count)
			{
				GameObject gameObject = NGUITools.AddChild(this.m_Panel.gameObject, this.taskItemPre);
				gameObject.name = num.ToString();
				TaskItem taskItem = gameObject.GetComponent<TaskItem>();
				this.taskItemList.Add(taskItem);
				gameObject.transform.localPosition = new Vector3(0f, (float)(-(float)num) * this.itemOffY, 0f);
				if (this.m_taskData != null)
				{
					taskItem.InitData(this.m_taskData[num], null);
				}
				else
				{
					taskItem.InitData(null, this.m_dailyData[num]);
				}
				UIEventListener.Get(taskItem.getRewardBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickGetReward);
			}
			else
			{
				TaskItem taskItem = this.taskItemList[num];
				GameObject gameObject = taskItem.gameObject;
				if (this.oldIndex == -1)
				{
					if (this.m_taskData != null)
					{
						taskItem.InitData(this.m_taskData[num], null);
					}
					else
					{
						taskItem.InitData(null, this.m_dailyData[num]);
					}
				}
				if (!taskItem.gameObject.activeSelf)
				{
					taskItem.gameObject.SetActive(true);
					gameObject.transform.localPosition = new Vector3(0f, (float)(-(float)num) * this.itemOffY, 0f);
				}
			}
			num++;
		}
		this.oldIndex = index;
	}

	private void OnClickGetReward(GameObject go)
	{
		if (Singleton<GetItemsView>.Instance.IsOpen || !this.canClickGetReward)
		{
			return;
		}
		GetItemsView expr_20 = Singleton<GetItemsView>.Instance;
		expr_20.onFinish = (Callback)Delegate.Combine(expr_20.onFinish, new Callback(this.CanGetReward));
		SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在获得奖励", true, 15f);
		if (this.m_taskData != null)
		{
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetAchieveTaskAward, param, new object[]
			{
				this.m_taskData[int.Parse(go.transform.parent.parent.parent.name)].taskid
			});
			this.canClickGetReward = false;
		}
		else
		{
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetDailyTaskAward, param, new object[]
			{
				this.m_dailyData[int.Parse(go.transform.parent.parent.parent.name)].taskid
			});
			this.canClickGetReward = false;
		}
		this.cormgr.StartCoroutine(this.RevertCanClick(), true);
	}

	private void CheckListCount(List<DetailAchieveData> taskData, List<TaskItem> taskItemList)
	{
		if (taskData.Count < taskItemList.Count)
		{
			for (int i = taskData.Count; i < taskItemList.Count; i++)
			{
				taskItemList[i].gameObject.SetActive(false);
				taskItemList[i].transform.localPosition = Vector3.zero;
			}
		}
	}

	private void CheckListCount(List<DailyTaskData> taskData, List<TaskItem> taskItemList)
	{
		if (taskData.Count < taskItemList.Count)
		{
			for (int i = taskData.Count; i < taskItemList.Count; i++)
			{
				taskItemList[i].gameObject.SetActive(false);
				taskItemList[i].transform.localPosition = Vector3.zero;
			}
		}
	}

	private void OnClipMove(UIPanel panel)
	{
		if (this.m_taskData == null && this.m_dailyData == null)
		{
			return;
		}
		if (this.m_taskData != null && this.m_taskData.Count <= 6)
		{
			return;
		}
		if (this.m_dailyData != null && this.m_dailyData.Count <= 6)
		{
			return;
		}
		this.CheckListStateAndUpdate(panel.clipOffset.y);
	}

	private void CheckListStateAndUpdate(float offY)
	{
		this.UpdateItemList(this.CheckIndex(offY), false);
	}

	private int CheckIndex(float offY)
	{
		if (offY > -369f)
		{
			return 0;
		}
		int num = (int)((-369f - offY) / this.itemOffY);
		if (this.m_dailyData != null)
		{
			if (num > this.m_dailyData.Count - 6)
			{
				num = this.m_dailyData.Count - 6;
			}
		}
		else if (num > this.m_taskData.Count - 6)
		{
			num = this.m_taskData.Count - 6;
		}
		return num;
	}

	public void PlayDuang(TaskAward taskAward)
	{
		ToolsFacade.Instance.GetRewards_WriteInModels(taskAward.equipmentList, taskAward.heroList, taskAward.itemList, taskAward.repeatList, null);
	}

	[DebuggerHidden]
	private IEnumerator RevertCanClick()
	{
		TaskDisplayManager.<RevertCanClick>c__Iterator186 <RevertCanClick>c__Iterator = new TaskDisplayManager.<RevertCanClick>c__Iterator186();
		<RevertCanClick>c__Iterator.<>f__this = this;
		return <RevertCanClick>c__Iterator;
	}

	public void CanGetReward()
	{
		this.canClickGetReward = true;
		GetItemsView expr_0C = Singleton<GetItemsView>.Instance;
		expr_0C.onFinish = (Callback)Delegate.Remove(expr_0C.onFinish, new Callback(this.CanGetReward));
	}

	public void ClearTaskList()
	{
		if (this.taskItemList == null)
		{
			return;
		}
		for (int i = 0; i < this.taskItemList.Count; i++)
		{
			UnityEngine.Object.Destroy(this.taskItemList[i].gameObject);
		}
		this.taskItemList.Clear();
	}

	public void NewbieGetAchieveAwd()
	{
		this.NewbieItemGetAwd();
	}

	public void NewbieGetDailyAwd()
	{
		this.NewbieItemGetAwd();
	}

	private void NewbieItemGetAwd()
	{
		if (this.taskItemList == null || this.taskItemList.Count < 1)
		{
			return;
		}
		TaskItem taskItem = this.taskItemList[0];
		if (taskItem == null)
		{
			return;
		}
		if (taskItem.getRewardBtn == null)
		{
			return;
		}
		this.OnClickGetReward(taskItem.getRewardBtn.gameObject);
	}
}
