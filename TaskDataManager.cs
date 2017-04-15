using Assets.Scripts.Model;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

public class TaskDataManager
{
	public List<DetailAchieveData> GetListData(string typeName)
	{
		AchieveAll achieveAll = ModelManager.Instance.Get_AchieveAll_X();
		return this.FixTaskIndex(achieveAll.detailAchieveDataList.FindAll((DetailAchieveData obj) => obj.achieveid.ToString() == typeName));
	}

	private List<DetailAchieveData> FixTaskIndex(List<DetailAchieveData> list)
	{
		List<DetailAchieveData> list2 = new List<DetailAchieveData>();
		list2.AddRange(list.FindAll((DetailAchieveData obj) => obj.isComplete && !obj.isGetAward));
		list2.AddRange(list.FindAll((DetailAchieveData obj) => !obj.isComplete && !obj.isGetAward));
		list2.AddRange(list.FindAll((DetailAchieveData obj) => obj.isComplete && obj.isGetAward));
		return list2;
	}

	public List<DailyTaskData> GetDailyTasListData()
	{
		AchieveAll achieveAll = ModelManager.Instance.Get_AchieveAll_X();
		return this.FixDailyIndex(achieveAll.dailyTaskList);
	}

	private List<DailyTaskData> FixDailyIndex(List<DailyTaskData> list)
	{
		List<DailyTaskData> list2 = new List<DailyTaskData>();
		list2.AddRange(list.FindAll((DailyTaskData obj) => obj.isComplete && !obj.isGetAward));
		list2.AddRange(list.FindAll((DailyTaskData obj) => !obj.isComplete && !obj.isGetAward));
		list2.AddRange(list.FindAll((DailyTaskData obj) => obj.isComplete && obj.isGetAward));
		return list2;
	}

	public TaskAward GetTaskAwardData()
	{
		AchieveAll achieveAll = ModelManager.Instance.Get_AchieveAll_X();
		return achieveAll.taskAward;
	}
}
