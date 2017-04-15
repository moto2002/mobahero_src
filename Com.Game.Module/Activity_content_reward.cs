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
	public class Activity_content_reward : Activity_contentBase
	{
		public UIGrid grid_reward;

		public Activity_rewardCell template_rewardCell;

		public Activity_tip tip;

		private object[] msgs;

		private bool bReposition;

		private bool bSortRequest;

		private bool bSortCoroutine;

		private bool bRefreshCoroutine;

		private List<string> listTask;

		private Dictionary<string, SysActivityTaskVo> dicTask;

		private Dictionary<string, Activity_rewardCell> dicComs;

		public override EActivityModuleType GetModuleType()
		{
			return EActivityModuleType.eTask;
		}

		private void Awake()
		{
			this.listTask = new List<string>();
			this.dicTask = new Dictionary<string, SysActivityTaskVo>();
			this.dicComs = new Dictionary<string, Activity_rewardCell>();
			this.msgs = new object[]
			{
				ClientC2V.Receive_GetTaskReward
			};
		}

		private void Update()
		{
			if (!this.bRefreshCoroutine && this.bSortRequest)
			{
				this.StartSort();
			}
		}

		private void OnEnable()
		{
			this.bReposition = false;
			this.bSortRequest = false;
			this.bSortCoroutine = false;
			this.bRefreshCoroutine = false;
			MobaMessageManagerTools.RegistMsg(this, this.msgs, true, "OnMsg_");
		}

		private void OnDisable()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, false, "OnMsg_");
		}

		[DebuggerHidden]
		public override IEnumerator RefreshUI(Func<IEnumerator> ieBreak = null)
		{
			Activity_content_reward.<RefreshUI>c__IteratorB9 <RefreshUI>c__IteratorB = new Activity_content_reward.<RefreshUI>c__IteratorB9();
			<RefreshUI>c__IteratorB.ieBreak = ieBreak;
			<RefreshUI>c__IteratorB.<$>ieBreak = ieBreak;
			<RefreshUI>c__IteratorB.<>f__this = this;
			return <RefreshUI>c__IteratorB;
		}

		[DebuggerHidden]
		private IEnumerator RefreshUI_tasks(Func<IEnumerator> ieBreak)
		{
			Activity_content_reward.<RefreshUI_tasks>c__IteratorBA <RefreshUI_tasks>c__IteratorBA = new Activity_content_reward.<RefreshUI_tasks>c__IteratorBA();
			<RefreshUI_tasks>c__IteratorBA.ieBreak = ieBreak;
			<RefreshUI_tasks>c__IteratorBA.<$>ieBreak = ieBreak;
			<RefreshUI_tasks>c__IteratorBA.<>f__this = this;
			return <RefreshUI_tasks>c__IteratorBA;
		}

		[DebuggerHidden]
		private IEnumerator GridReposition()
		{
			Activity_content_reward.<GridReposition>c__IteratorBB <GridReposition>c__IteratorBB = new Activity_content_reward.<GridReposition>c__IteratorBB();
			<GridReposition>c__IteratorBB.<>f__this = this;
			return <GridReposition>c__IteratorBB;
		}

		private int SortList(string a, string b)
		{
			int result;
			if (string.IsNullOrEmpty(a) || !this.dicTask.ContainsKey(a))
			{
				result = 1;
			}
			else if (string.IsNullOrEmpty(b) || !this.dicTask.ContainsKey(b))
			{
				result = -1;
			}
			else
			{
				SysActivityTaskVo sysActivityTaskVo = this.dicTask[a];
				SysActivityTaskVo sysActivityTaskVo2 = this.dicTask[b];
				ActivityTaskData activityTaskData = ModelManager.Instance.Get_Activity_taskData(sysActivityTaskVo.id);
				ActivityTaskData activityTaskData2 = ModelManager.Instance.Get_Activity_taskData(sysActivityTaskVo2.id);
				if (activityTaskData == null)
				{
					result = 1;
				}
				else if (activityTaskData2 == null)
				{
					result = -1;
				}
				else if (activityTaskData.taskstate != activityTaskData2.taskstate)
				{
					if (activityTaskData.taskstate == 1)
					{
						result = -1;
					}
					else if (activityTaskData.taskstate == 0)
					{
						if (activityTaskData2.taskstate == 1)
						{
							result = 1;
						}
						else
						{
							result = -1;
						}
					}
					else
					{
						result = 1;
					}
				}
				else
				{
					result = activityTaskData.taskid - activityTaskData2.taskid;
				}
			}
			return result;
		}

		[DebuggerHidden]
		private IEnumerator RefreshUI_sortList()
		{
			Activity_content_reward.<RefreshUI_sortList>c__IteratorBC <RefreshUI_sortList>c__IteratorBC = new Activity_content_reward.<RefreshUI_sortList>c__IteratorBC();
			<RefreshUI_sortList>c__IteratorBC.<>f__this = this;
			return <RefreshUI_sortList>c__IteratorBC;
		}

		private void OnShowRewardTip(RewardItemBase com, Vector3 v3, bool state)
		{
			this.tip.Show = state;
			if (state)
			{
				Transform transform = this.tip.transform.parent.transform;
				Vector3 v4 = v3 - transform.position;
				this.tip.Pos = transform.worldToLocalMatrix * v4;
				this.tip.Info = com;
			}
		}

		private void OnMsg_Receive_GetTaskReward(MobaMessage msg)
		{
			this.bSortRequest = true;
		}

		private void StartSort()
		{
			this.bSortRequest = false;
			if (this.bSortCoroutine)
			{
				base.StopCoroutine("RefreshUI_sortList");
			}
			this.bSortCoroutine = true;
			base.StartCoroutine("RefreshUI_sortList");
		}

		public bool NewbieGetFirstAwd()
		{
			if (this.listTask == null || this.listTask.Count < 1)
			{
				return false;
			}
			if (string.IsNullOrEmpty(this.listTask[0]))
			{
				return false;
			}
			Activity_rewardCell activity_rewardCell = null;
			return this.dicComs.TryGetValue(this.listTask[0], out activity_rewardCell) && activity_rewardCell != null && activity_rewardCell.NewbieGetAwd();
		}
	}
}
