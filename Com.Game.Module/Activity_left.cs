using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	public class Activity_left : MonoBehaviour
	{
		public UIGrid grid_activity;

		public Activity_activityMenu template_activity;

		public UIScrollView scrollView;

		private Dictionary<int, Activity_activityMenu> dicComs;

		private Dictionary<int, SysActivityVo> config;

		private Dictionary<int, NoticeBoardData> dicNoticeData;

		private List<int> listItemID;

		private int curTypeID;

		private int curLeftItemID;

		private bool valid = true;

		public Action OnCurActivityChange
		{
			get;
			set;
		}

		public EActivityType AType
		{
			get;
			set;
		}

		public int CurTypeID
		{
			get
			{
				return this.curTypeID;
			}
			set
			{
				this.curTypeID = value;
				this.UpdateItemIDList();
				this.RefreshUI_itemList();
			}
		}

		public int CurLeftItemIndex
		{
			set
			{
				if (this.listItemID != null && value >= 0 && value < this.listItemID.Count)
				{
					this.CurLeftItemID = this.listItemID[value];
				}
				else
				{
					this.CurLeftItemID = -2147483648;
				}
			}
		}

		public int CurLeftItemID
		{
			get
			{
				return this.curLeftItemID;
			}
			set
			{
				this.curLeftItemID = value;
				this.SendMsg_read();
				this.RefreshUI_curLeftItem();
			}
		}

		public object CurItemInfo
		{
			get
			{
				object result = null;
				if (this.AType == EActivityType.eActivity)
				{
					if (this.config.ContainsKey(this.CurLeftItemID))
					{
						result = this.config[this.CurLeftItemID];
					}
				}
				else if (this.dicNoticeData.ContainsKey(this.CurLeftItemID))
				{
					result = this.dicNoticeData[this.CurLeftItemID];
				}
				return result;
			}
		}

		private void Awake()
		{
			this.listItemID = new List<int>();
			this.dicComs = new Dictionary<int, Activity_activityMenu>();
			this.dicNoticeData = new Dictionary<int, NoticeBoardData>();
			Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysActivityVo>();
			this.config = new Dictionary<int, SysActivityVo>();
			DateTime serverCurrentTime = ToolsFacade.ServerCurrentTime;
			foreach (KeyValuePair<string, object> current in dicByType)
			{
				SysActivityVo sysActivityVo = current.Value as SysActivityVo;
				if (sysActivityVo != null)
				{
					DateTime dateTime = ActivityTools.GetDateTime(sysActivityVo.show_start_time, true);
					DateTime dateTime2 = ActivityTools.GetDateTime(sysActivityVo.show_end_time, false);
					if (string.IsNullOrEmpty(sysActivityVo.show_start_time) || !(serverCurrentTime < dateTime))
					{
						if (string.IsNullOrEmpty(sysActivityVo.show_end_time) || !(serverCurrentTime > dateTime2))
						{
							this.config.Add(int.Parse(current.Key), sysActivityVo);
						}
					}
				}
			}
			if (this.config == null)
			{
				this.valid = false;
				ClientLogger.Error("SysActivityVo 读取失败");
			}
		}

		private void Start()
		{
		}

		private void UpdateItemIDList()
		{
			if (this.AType == EActivityType.eActivity)
			{
				this.UpdateActivityIDList();
			}
			else
			{
				this.UpdateNoticeIDList();
			}
		}

		private void UpdateActivityIDList()
		{
			this.listItemID.Clear();
			int num = this.CurTypeID;
			Dictionary<int, SysActivityVo>.Enumerator enumerator = this.config.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, SysActivityVo> current = enumerator.Current;
				SysActivityVo value = current.Value;
				if (value != null)
				{
					if (value.activity_type_id == num)
					{
						this.listItemID.Add(value.id);
					}
				}
			}
			this.listItemID.Sort(new Comparison<int>(this.Comparision_SysActivity));
		}

		private void UpdateNoticeIDList()
		{
			this.listItemID.Clear();
			List<NoticeBoardData> list = ModelManager.Instance.Get_Activity_noticeList();
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.listItemID.Add(list[i].noticeid);
					if (!this.dicNoticeData.ContainsKey(list[i].noticeid))
					{
						this.dicNoticeData.Add(list[i].noticeid, list[i]);
					}
				}
			}
		}

		private void RefreshUI_itemList()
		{
			this.scrollView.UpdatePosition();
			this.scrollView.ResetPosition();
			List<int> list = this.listItemID;
			GridHelper.FillGrid<Activity_activityMenu>(this.grid_activity, this.template_activity, (list != null) ? list.Count : 0, delegate(int idx, Activity_activityMenu comp)
			{
				comp.AType = this.AType;
				if (this.AType == EActivityType.eActivity)
				{
					comp.Info = this.config[list[idx]];
				}
				else
				{
					comp.Info = this.dicNoticeData[list[idx]];
				}
				comp.OnLeftItemSelect = new Action<Activity_activityMenu>(this.OnSelectActivity);
				comp.RefreshUI();
				this.dicComs[list[idx]] = comp;
				comp.gameObject.SetActive(true);
			});
			this.RefreshUI_initList();
			this.grid_activity.Reposition();
		}

		private void RefreshUI_initList()
		{
			for (int i = 0; i < this.grid_activity.transform.childCount; i++)
			{
				Transform child = this.grid_activity.transform.GetChild(i);
				if (!(null == child))
				{
					if (!child.gameObject.activeSelf)
					{
						child.transform.localPosition = Vector3.zero;
					}
				}
			}
		}

		private void RefreshUI_curLeftItem()
		{
			if (this.dicComs.ContainsKey(this.CurLeftItemID))
			{
				this.dicComs[this.CurLeftItemID].Checked = true;
			}
		}

		private void SendMsg_read()
		{
			if (this.AType == EActivityType.eActivity)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.activity_readActivity, this.curLeftItemID, true);
			}
			else
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.activity_readNotice, this.curLeftItemID, true);
			}
		}

		private void OnSelectActivity(Activity_activityMenu com)
		{
			if (this.OnCurActivityChange != null && this.curLeftItemID != com.ID)
			{
				this.curLeftItemID = com.ID;
				this.SendMsg_read();
				this.OnCurActivityChange();
			}
		}

		private int Comparision_SysActivity(int a, int b)
		{
			SysActivityVo sysActivityVo = this.config[a];
			SysActivityVo sysActivityVo2 = this.config[b];
			int result;
			if (sysActivityVo.rank != sysActivityVo2.rank)
			{
				result = sysActivityVo.rank - sysActivityVo2.rank;
			}
			else
			{
				result = sysActivityVo.id - sysActivityVo2.id;
			}
			return result;
		}

		public void NewbieSelActByActivityId(int inActivityId)
		{
			Activity_activityMenu activity_activityMenu = null;
			if (this.dicComs != null && this.dicComs.TryGetValue(inActivityId, out activity_activityMenu) && activity_activityMenu != null && activity_activityMenu.toggle != null)
			{
				activity_activityMenu.Checked = true;
			}
		}
	}
}
