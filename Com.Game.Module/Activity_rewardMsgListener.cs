using Assets.Scripts.Model;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class Activity_rewardMsgListener : MonoBehaviour
	{
		private int mTaskId;

		public Callback onSucceed;

		public int taskId
		{
			set
			{
				if (value > 0)
				{
					this.mTaskId = value;
				}
			}
		}

		private void OnEnable()
		{
			MobaMessageManager.RegistMessage((ClientMsg)23073, new MobaMessageFunc(this.GetRewards));
		}

		private void OnDisable()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)23073, new MobaMessageFunc(this.GetRewards));
		}

		private void GetRewards(MobaMessage msg)
		{
			ActivityTaskData activityTaskData = (ActivityTaskData)msg.Param;
			if (activityTaskData == null || activityTaskData.taskid != this.mTaskId)
			{
				return;
			}
			ActivityData.Rewards rewards = ModelManager.Instance.Get_Activity_rewards();
			if (rewards == null)
			{
				return;
			}
			ToolsFacade.Instance.GetRewards_WriteInModels(rewards.listEquip, rewards.listHero, rewards.listDropItem, rewards.listRepeatItem, this.onSucceed);
			this.mTaskId = 0;
		}
	}
}
