using Assets.Scripts.Model;
using Com.Game.Data;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class Activity_activityMenu : MonoBehaviour
	{
		public UILabel lb_content;

		public UISprite sp_level;

		public UIToggle toggle;

		public UISprite sp_new;

		private SysActivityVo activityData;

		private NoticeBoardData noticeData;

		private object[] msgs;

		public Action<Activity_activityMenu> OnLeftItemSelect
		{
			get;
			set;
		}

		public EActivityType AType
		{
			get;
			set;
		}

		public object Info
		{
			get
			{
				if (this.AType == EActivityType.eActivity)
				{
					return this.activityData;
				}
				return this.noticeData;
			}
			set
			{
				if (this.AType == EActivityType.eActivity)
				{
					this.activityData = (value as SysActivityVo);
				}
				else
				{
					this.noticeData = (value as NoticeBoardData);
				}
			}
		}

		public int ID
		{
			get
			{
				if (this.AType == EActivityType.eActivity)
				{
					return this.activityData.id;
				}
				return this.noticeData.noticeid;
			}
		}

		public bool Checked
		{
			get
			{
				return this.toggle.value;
			}
			set
			{
				this.toggle.value = value;
			}
		}

		private void Awake()
		{
			this.toggle.onChange.Add(new EventDelegate(new EventDelegate.Callback(this.OnToggleChange)));
			this.msgs = new object[]
			{
				ClientC2V.activity_updateActivityState,
				ClientC2V.activity_updateNoticeState,
				ClientC2V.activity_updateTaskState
			};
		}

		private void Start()
		{
		}

		private void OnEnable()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, true, "OnMsg_");
		}

		private void OnDisable()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, false, "OnMsg_");
		}

		private void OnMsg_activity_updateActivityState(MobaMessage msg)
		{
			ActivityData.ActivityNewState activityNewState = msg.Param as ActivityData.ActivityNewState;
			if (this.AType == EActivityType.eActivity && this.activityData.id == activityNewState.activityVo.id)
			{
				this.RefreshUI_newIcon();
			}
		}

		private void OnMsg_activity_updateNoticeState(MobaMessage msg)
		{
			int num = (int)msg.Param;
			if (this.AType == EActivityType.eNotice && this.noticeData.noticeid == num)
			{
				this.RefreshUI_newIcon();
			}
		}

		private void OnMsg_activity_updateTaskState(MobaMessage msg)
		{
			if (this.AType == EActivityType.eActivity)
			{
				ActivityData.ActivityNewState activityNewState = msg.Param as ActivityData.ActivityNewState;
				if (activityNewState != null && activityNewState.activityVo.id == this.activityData.id)
				{
					this.RefreshUI_newIcon();
				}
			}
		}

		private void OnToggleChange()
		{
			if (this.toggle.value && this.OnLeftItemSelect != null)
			{
				this.OnLeftItemSelect(this);
			}
		}

		public void RefreshUI()
		{
			this.toggle.optionCanBeNone = true;
			this.toggle.startsActive = false;
			this.Checked = false;
			this.RefreshUI_content();
			this.RefreshUI_tag();
			this.toggle.optionCanBeNone = false;
			this.RefreshUI_newIcon();
		}

		private void RefreshUI_content()
		{
			if (this.Info == null)
			{
				return;
			}
			if (this.AType == EActivityType.eActivity)
			{
				this.lb_content.text = LanguageManager.Instance.GetStringById(this.activityData.name);
			}
			else
			{
				this.lb_content.text = this.noticeData.titleBtn;
			}
		}

		private void RefreshUI_newIcon()
		{
			if (this.AType == EActivityType.eActivity)
			{
				bool active = this.activityData != null && ModelManager.Instance.Get_Activity_newState(this.activityData.id);
				this.sp_new.gameObject.SetActive(active);
			}
			else
			{
				this.sp_new.gameObject.SetActive(this.noticeData != null && !this.noticeData.hasRead);
			}
		}

		private void RefreshUI_tag()
		{
			if (this.Info == null)
			{
				return;
			}
			string text = string.Empty;
			int num;
			if (this.AType == EActivityType.eActivity)
			{
				num = this.activityData.tag;
			}
			else
			{
				num = this.noticeData.label;
			}
			switch (num)
			{
			case 0:
				text = string.Empty;
				break;
			case 1:
				text = "Activity_tag_time_limit";
				break;
			case 2:
				text = "Activity_tag_hot";
				break;
			case 3:
				text = "Activity_tag_announcement";
				break;
			case 4:
				text = "Activity_tag_latest";
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.sp_level.gameObject.SetActive(true);
				this.sp_level.spriteName = text;
			}
			else
			{
				this.sp_level.gameObject.SetActive(false);
			}
		}
	}
}
