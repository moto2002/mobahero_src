using Com.Game.Data;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class ActivityData
	{
		public class ActivityNewState
		{
			public bool hasRewards;

			public bool notRead;

			public int type;

			public SysActivityVo activityVo;

			public Dictionary<int, ActivityData.TaskState> dicTaskState;
		}

		public class TaskState
		{
			public int taskID;

			public int taskState;

			public SysActivityTaskVo taskVo;
		}

		public class Rewards
		{
			public List<DropItemData> listDropItem;

			public List<HeroInfoData> listHero;

			public List<EquipmentInfoData> listEquip;

			public List<DropItemData> listRepeatItem;
		}

		public List<ActivityTaskData> listTask;

		public List<NoticeBoardData> listNotice;

		public ActivityData.Rewards rewards;

		public List<int> listHasReadActivity;

		public Dictionary<int, ActivityData.ActivityNewState> dicActivityNewState;

		public ActivityData()
		{
			this.rewards = new ActivityData.Rewards();
		}
	}
}
