using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public static class ModelTools_activity
	{
		public static ActivityData Get_Activity_Data(this ModelManager mmng)
		{
			ActivityData activityData = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_activity))
			{
				activityData = mmng.GetData<ActivityData>(EModelType.Model_activity);
			}
			return activityData ?? new ActivityData();
		}

		public static List<ActivityTaskData> Get_Activity_TaskList(this ModelManager mmng)
		{
			ActivityData activityData = mmng.Get_Activity_Data();
			return activityData.listTask;
		}

		public static ActivityTaskData Get_Activity_taskData(this ModelManager mmng, int taskID)
		{
			List<ActivityTaskData> list = mmng.Get_Activity_TaskList();
			ActivityTaskData result = null;
			if (list != null)
			{
				result = list.Find((ActivityTaskData obj) => obj.taskid == taskID);
			}
			return result;
		}

		public static List<NoticeBoardData> Get_Activity_noticeList(this ModelManager mmng)
		{
			ActivityData activityData = mmng.Get_Activity_Data();
			return activityData.listNotice;
		}

		public static ActivityData.Rewards Get_Activity_rewards(this ModelManager mmng)
		{
			ActivityData activityData = mmng.Get_Activity_Data();
			return activityData.rewards;
		}

		public static List<int> Get_Activity_hasReadActivity(this ModelManager mmng)
		{
			ActivityData activityData = mmng.Get_Activity_Data();
			return activityData.listHasReadActivity;
		}

		public static bool Get_Activity_newState(this ModelManager mmng, int activityID)
		{
			bool result = false;
			ActivityData activityData = mmng.Get_Activity_Data();
			Dictionary<int, ActivityData.ActivityNewState> dicActivityNewState = activityData.dicActivityNewState;
			if (dicActivityNewState != null && dicActivityNewState.ContainsKey(activityID))
			{
				ActivityData.ActivityNewState activityNewState = activityData.dicActivityNewState[activityID];
				result = (activityNewState.hasRewards || activityNewState.notRead);
			}
			return result;
		}

		public static bool Get_Activity_newActivityStateByType(this ModelManager mmng, int type)
		{
			bool result = false;
			ActivityData activityData = mmng.Get_Activity_Data();
			if (activityData.dicActivityNewState != null)
			{
				foreach (KeyValuePair<int, ActivityData.ActivityNewState> current in activityData.dicActivityNewState)
				{
					if (current.Value.type == type && (current.Value.hasRewards || current.Value.notRead))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public static bool Get_Activity_newNoticeState(this ModelManager mmng)
		{
			bool result = false;
			ActivityData activityData = mmng.Get_Activity_Data();
			if (activityData.listNotice != null)
			{
				foreach (NoticeBoardData current in activityData.listNotice)
				{
					if (!current.hasRead)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public static bool Get_Activity_HasRewards(this ModelManager mmng)
		{
			bool result = false;
			ActivityData activityData = mmng.Get_Activity_Data();
			if (activityData.dicActivityNewState != null)
			{
				foreach (KeyValuePair<int, ActivityData.ActivityNewState> current in activityData.dicActivityNewState)
				{
					if (current.Value.hasRewards)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
	}
}
