using Assets.Scripts.Model;
using Com.Game.Data;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class Activity_taskBtn : MonoBehaviour
	{
		public ActivityBtn[] activityBtns;

		private ActivityBtn.EActivityBtn curBtnState;

		private Dictionary<ActivityBtn.EActivityBtn, ActivityBtn> dicBtns;

		public SysActivityTaskVo taskVo
		{
			get;
			set;
		}

		public SysActivityVo activityVo
		{
			get;
			set;
		}

		private void Awake()
		{
			this.dicBtns = new Dictionary<ActivityBtn.EActivityBtn, ActivityBtn>();
			this.curBtnState = ActivityBtn.EActivityBtn.eNone;
			for (int i = 0; i < this.activityBtns.Length; i++)
			{
				if (!this.dicBtns.ContainsKey(this.activityBtns[i].eBtnType))
				{
					this.dicBtns.Add(this.activityBtns[i].eBtnType, this.activityBtns[i]);
				}
			}
		}

		[DebuggerHidden]
		public IEnumerator RefreshUI()
		{
			Activity_taskBtn.<RefreshUI>c__IteratorC3 <RefreshUI>c__IteratorC = new Activity_taskBtn.<RefreshUI>c__IteratorC3();
			<RefreshUI>c__IteratorC.<>f__this = this;
			return <RefreshUI>c__IteratorC;
		}

		public void RefreshUI2()
		{
			this.RefreshUI_btn();
		}

		private void RefreshUI_alpha(float alpha = 0.01f)
		{
			if (this.dicBtns.ContainsKey(this.curBtnState))
			{
				this.dicBtns[this.curBtnState].lb_text.alpha = alpha;
				this.dicBtns[this.curBtnState].sp_normal.alpha = alpha;
			}
		}

		private void RefreshUI_tween(bool b)
		{
			if (this.dicBtns.ContainsKey(this.curBtnState))
			{
				TweenAlpha[] tws = this.dicBtns[this.curBtnState].tws;
				for (int i = 0; i < tws.Length; i++)
				{
					tws[i].enabled = b;
				}
			}
		}

		private void RefreshUI_btn()
		{
			this.curBtnState = ActivityBtn.EActivityBtn.eNone;
			if (this.taskVo != null && this.activityVo != null)
			{
				DateTime dateTime = ActivityTools.GetDateTime(this.activityVo.start_time, true);
				DateTime dateTime2 = ActivityTools.GetDateTime(this.activityVo.end_time, false);
				DateTime serverCurrentTime = ToolsFacade.ServerCurrentTime;
				string text = null;
				if (serverCurrentTime < dateTime || serverCurrentTime > dateTime2)
				{
					this.curBtnState = ActivityBtn.EActivityBtn.eInactive;
					if (serverCurrentTime < dateTime)
					{
						text = LanguageManager.Instance.GetStringById("Activity_Task_Not_Start");
					}
					else
					{
						text = LanguageManager.Instance.GetStringById("Activity_Task_End");
					}
				}
				else
				{
					ActivityTaskData activityTaskData = ModelManager.Instance.Get_Activity_taskData(this.taskVo.id);
					if (activityTaskData != null)
					{
						if (activityTaskData.taskstate == 2)
						{
							this.curBtnState = ActivityBtn.EActivityBtn.eFinish;
						}
						else if (activityTaskData.taskstate == 1)
						{
							this.curBtnState = ActivityBtn.EActivityBtn.eReward;
						}
						else if (activityTaskData.taskstate == 0)
						{
							if (this.taskVo.travel_to != 0)
							{
								this.curBtnState = ActivityBtn.EActivityBtn.eGoto;
							}
							else
							{
								this.curBtnState = ActivityBtn.EActivityBtn.eNone;
							}
						}
					}
					else
					{
						this.curBtnState = ActivityBtn.EActivityBtn.eInactive;
					}
				}
				foreach (KeyValuePair<ActivityBtn.EActivityBtn, ActivityBtn> current in this.dicBtns)
				{
					if (current.Key == this.curBtnState)
					{
						if (!string.IsNullOrEmpty(text))
						{
							this.dicBtns[this.curBtnState].lb_text.text = text;
						}
						this.dicBtns[this.curBtnState].gameObject.SetActive(true);
						this.dicBtns[this.curBtnState].callback = new Action<ActivityBtn>(this.OnClick_ActivityBtn);
					}
					else
					{
						current.Value.gameObject.SetActive(false);
					}
				}
			}
		}

		private void OnClick_ActivityBtn(ActivityBtn btn)
		{
			if (btn.eBtnType == ActivityBtn.EActivityBtn.eGoto)
			{
				GotoWindowTools.GotoWindow(this.taskVo.travel_to);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.activity_close, null, false);
			}
			else if (btn.eBtnType != ActivityBtn.EActivityBtn.eFinish)
			{
				if (btn.eBtnType == ActivityBtn.EActivityBtn.eReward)
				{
					SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "领取奖励...", true, 15f);
					SendMsgManager.Instance.SendMsg(MobaGameCode.GetActivityAward, param, new object[]
					{
						this.taskVo.id.ToString()
					});
				}
				else if (btn.eBtnType == ActivityBtn.EActivityBtn.eInactive)
				{
				}
			}
		}

		public bool NewbieGetAwd()
		{
			if (this.dicBtns == null)
			{
				return false;
			}
			ActivityBtn activityBtn = null;
			if (!this.dicBtns.TryGetValue(ActivityBtn.EActivityBtn.eReward, out activityBtn))
			{
				return false;
			}
			if (activityBtn != null && this.taskVo != null)
			{
				this.OnClick_ActivityBtn(activityBtn);
				return true;
			}
			return false;
		}
	}
}
