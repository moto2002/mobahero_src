using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Com.Game.Module
{
	public class Activity_top : MonoBehaviour
	{
		[SerializeField]
		private UIGrid grid_title;

		[SerializeField]
		private Activity_title template_title;

		private Dictionary<int, Activity_title> dicComs;

		private Dictionary<int, SysActivityTypeVo> config;

		private List<int> keys;

		private int curTitle = -2147483648;

		private bool valid = true;

		public Action OnTopTitleChange
		{
			get;
			set;
		}

		public EActivityType AType
		{
			get;
			private set;
		}

		public int CurTitle
		{
			get
			{
				return this.curTitle;
			}
			set
			{
				if (this.curTitle != value)
				{
					if (value == -1)
					{
						this.curTitle = this.SetFirstNoticeItem();
					}
					else if (this.config.ContainsKey(value))
					{
						this.curTitle = value;
						this.AType = (EActivityType)this.config[this.curTitle].type;
					}
					this.RefreshUI_curTitle();
				}
			}
		}

		private void Awake()
		{
			this.Init();
			this.RefreshUI_titleList();
		}

		private void Start()
		{
		}

		private void Init()
		{
			this.config = new Dictionary<int, SysActivityTypeVo>();
			Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysActivityTypeVo>();
			long exp = ModelManager.Instance.Get_userData_filed_X("Exp");
			int userLevel = CharacterDataMgr.instance.GetUserLevel(exp);
			foreach (KeyValuePair<string, object> current in dicByType)
			{
				SysActivityTypeVo sysActivityTypeVo = current.Value as SysActivityTypeVo;
				if (sysActivityTypeVo.level_limit_floor == 0 || userLevel >= sysActivityTypeVo.level_limit_floor)
				{
					if (sysActivityTypeVo.level_limit_top == 0 || userLevel <= sysActivityTypeVo.level_limit_top)
					{
						this.config.Add(int.Parse(current.Key), sysActivityTypeVo);
					}
				}
			}
			if (this.config != null)
			{
				this.keys = new List<int>(this.config.Keys);
				this.keys.Sort();
			}
			else
			{
				this.valid = false;
				ClientLogger.Error("SysActivityTypeVo 读取失败");
			}
			this.dicComs = new Dictionary<int, Activity_title>();
		}

		private int SetFirstNoticeItem()
		{
			int num = -2147483648;
			KeyValuePair<int, SysActivityTypeVo> keyValuePair = this.config.FirstOrDefault((KeyValuePair<int, SysActivityTypeVo> obj) => obj.Value.type == 2);
			if (keyValuePair.Value != null)
			{
				List<NoticeBoardData> list = ModelManager.Instance.Get_Activity_noticeList();
				if (list != null && list.Count > 0)
				{
					num = keyValuePair.Key;
					this.AType = EActivityType.eNotice;
				}
			}
			if (0 > num)
			{
				keyValuePair = this.config.FirstOrDefault<KeyValuePair<int, SysActivityTypeVo>>();
				if (keyValuePair.Value != null)
				{
					num = keyValuePair.Value.id;
					this.AType = (EActivityType)keyValuePair.Value.type;
				}
			}
			return num;
		}

		private void RefreshUI_curTitle()
		{
			if (this.dicComs.ContainsKey(this.curTitle))
			{
				this.dicComs[this.curTitle].Checked = true;
			}
		}

		private void RefreshUI_titleList()
		{
			List<int> list = this.keys;
			GridHelper.FillGrid<Activity_title>(this.grid_title, this.template_title, (list != null) ? list.Count : 0, delegate(int idx, Activity_title comp)
			{
				comp.Info = this.config[list[idx]];
				comp.RefreshUI();
				comp.OnToggleSelect = new Action<Activity_title>(this.OnSelectTitle);
				this.dicComs[list[idx]] = comp;
				comp.gameObject.SetActive(true);
			});
			this.grid_title.Reposition();
		}

		private void OnSelectTitle(Activity_title com)
		{
			if (this.OnTopTitleChange != null && this.curTitle != com.Info.id && this.config.ContainsKey(this.curTitle))
			{
				this.curTitle = com.Info.id;
				this.AType = com.AType;
				this.OnTopTitleChange();
			}
		}

		public void NewbieSelActivityByType(int inTypeId)
		{
			Activity_title activity_title = null;
			if (this.dicComs != null && this.dicComs.TryGetValue(inTypeId, out activity_title) && activity_title != null && activity_title.toggle != null)
			{
				activity_title.Checked = true;
			}
		}
	}
}
