using Assets.Scripts.Model;
using Com.Game.Data;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class Activity_title : MonoBehaviour
	{
		public UILabel lb_title;

		public UISprite sp_notice;

		public UIToggle toggle;

		private bool showNoticeIcon;

		private SysActivityTypeVo info;

		private object[] msgs;

		public Action<Activity_title> OnToggleSelect
		{
			get;
			set;
		}

		public EActivityType AType
		{
			get;
			private set;
		}

		public SysActivityTypeVo Info
		{
			get
			{
				return this.info;
			}
			set
			{
				if (value != this.info)
				{
					this.info = value;
					this.AType = (EActivityType)this.info.type;
				}
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

		public bool ShowNoticeIcon
		{
			get
			{
				return this.showNoticeIcon;
			}
			set
			{
				this.showNoticeIcon = value;
				this.RefreshUI_noticeIcon();
			}
		}

		private void Awake()
		{
			this.showNoticeIcon = false;
			this.toggle.onChange.Add(new EventDelegate(new EventDelegate.Callback(this.OnToggleChanged)));
			this.msgs = new object[]
			{
				ClientC2V.activity_updateActivityState,
				ClientC2V.activity_updateNoticeState,
				ClientC2V.activity_updateTaskState
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

		private void OnMsg_activity_updateActivityState(MobaMessage msg)
		{
			ActivityData.ActivityNewState activityNewState = msg.Param as ActivityData.ActivityNewState;
			if (this.AType == EActivityType.eActivity && this.info.id == activityNewState.type)
			{
				this.RefreshUI_noticeIcon();
			}
		}

		private void OnMsg_activity_updateNoticeState(MobaMessage msg)
		{
			int num = (int)msg.Param;
			if (this.AType == EActivityType.eNotice)
			{
				this.RefreshUI_noticeIcon();
			}
		}

		private void OnMsg_activity_updateTaskState(MobaMessage msg)
		{
			if (this.AType == EActivityType.eActivity)
			{
				ActivityData.ActivityNewState activityNewState = msg.Param as ActivityData.ActivityNewState;
				if (activityNewState != null && activityNewState.type == this.info.id)
				{
					this.RefreshUI_noticeIcon();
				}
			}
		}

		public void RefreshUI()
		{
			this.RefreshUI_noticeIcon();
			this.RefreshUI_title();
		}

		private void RefreshUI_noticeIcon()
		{
			bool active;
			if (this.AType == EActivityType.eActivity)
			{
				active = ModelManager.Instance.Get_Activity_newActivityStateByType(this.info.id);
			}
			else
			{
				active = ModelManager.Instance.Get_Activity_newNoticeState();
			}
			this.sp_notice.gameObject.SetActive(active);
		}

		private void RefreshUI_title()
		{
			this.lb_title.text = LanguageManager.Instance.GetStringById(this.Info.name);
		}

		private void OnToggleChanged()
		{
			if (this.toggle.value && this.OnToggleSelect != null)
			{
				this.OnToggleSelect(this);
			}
		}
	}
}
