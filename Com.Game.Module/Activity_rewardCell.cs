using Assets.Scripts.Model;
using Com.Game.Data;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class Activity_rewardCell : MonoBehaviour
	{
		public Activity_rewardItem template_item;

		public UILabel lb_notice;

		public UIGrid grid_reward;

		public UIWidget[] widgets;

		public UISprite sp_finish;

		public UISprite sp_unfinish;

		public Activity_taskBtn taskBtn;

		private List<RewardItemBase> listRItems;

		private List<Activity_rewardItem> listComs;

		private bool bReposition;

		private object[] msgs;

		public Action<RewardItemBase, Vector3, bool> OnShowTips
		{
			get;
			set;
		}

		public SysActivityTaskVo Info
		{
			get;
			set;
		}

		public SysActivityVo TaskVo
		{
			get;
			set;
		}

		private void Awake()
		{
			this.listComs = new List<Activity_rewardItem>();
			this.msgs = new object[]
			{
				ClientC2V.Receive_GetTaskReward
			};
		}

		private void OnEnable()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, true, "OnMsg_");
		}

		private void OnDisable()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, false, "OnMsg_");
		}

		[DebuggerHidden]
		public IEnumerator RefreshUI(IEnumerator rewardRepos, Func<IEnumerator> tableRepos)
		{
			Activity_rewardCell.<RefreshUI>c__IteratorBF <RefreshUI>c__IteratorBF = new Activity_rewardCell.<RefreshUI>c__IteratorBF();
			<RefreshUI>c__IteratorBF.rewardRepos = rewardRepos;
			<RefreshUI>c__IteratorBF.tableRepos = tableRepos;
			<RefreshUI>c__IteratorBF.<$>rewardRepos = rewardRepos;
			<RefreshUI>c__IteratorBF.<$>tableRepos = tableRepos;
			<RefreshUI>c__IteratorBF.<>f__this = this;
			return <RefreshUI>c__IteratorBF;
		}

		[DebuggerHidden]
		private IEnumerator RefreshUI_rewards()
		{
			Activity_rewardCell.<RefreshUI_rewards>c__IteratorC0 <RefreshUI_rewards>c__IteratorC = new Activity_rewardCell.<RefreshUI_rewards>c__IteratorC0();
			<RefreshUI_rewards>c__IteratorC.<>f__this = this;
			return <RefreshUI_rewards>c__IteratorC;
		}

		[DebuggerHidden]
		private IEnumerator GridReposition()
		{
			Activity_rewardCell.<GridReposition>c__IteratorC1 <GridReposition>c__IteratorC = new Activity_rewardCell.<GridReposition>c__IteratorC1();
			<GridReposition>c__IteratorC.<>f__this = this;
			return <GridReposition>c__IteratorC;
		}

		private void RefreshUI_alpha(float alpha = 0.01f)
		{
			for (int i = 0; i < this.widgets.Length; i++)
			{
				this.widgets[i].alpha = alpha;
			}
		}

		private void RefreshUI_tweenAlpha(bool b)
		{
			for (int i = 0; i < this.widgets.Length; i++)
			{
				TweenAlpha component = this.widgets[i].gameObject.GetComponent<TweenAlpha>();
				if (null != component)
				{
					component.enabled = b;
				}
			}
		}

		private void OnMsg_Receive_GetTaskReward(MobaMessage msg)
		{
			ActivityTaskData activityTaskData = msg.Param as ActivityTaskData;
			if (this.Info != null && activityTaskData != null && this.Info.id == activityTaskData.taskid)
			{
				this.taskBtn.RefreshUI2();
				this.GetRewards();
			}
		}

		private void OnMouseOver(Activity_rewardItem com, bool state)
		{
			if (this.OnShowTips != null)
			{
				this.OnShowTips(com.Info, com.transform.position, state);
			}
		}

		private void GetRewards()
		{
			ActivityData.Rewards rewards = ModelManager.Instance.Get_Activity_rewards();
			if (rewards == null)
			{
				return;
			}
			ToolsFacade.Instance.GetRewards_WriteInModels(rewards.listEquip, rewards.listHero, rewards.listDropItem, rewards.listRepeatItem, null);
		}

		public bool NewbieGetAwd()
		{
			return !(this.taskBtn == null) && this.taskBtn.NewbieGetAwd();
		}
	}
}
