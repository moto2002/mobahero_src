using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class Activity_center : MonoBehaviour
	{
		public UIWidget dragSp;

		public BoxCollider dragBox;

		public UIScrollView uiScrolview;

		public UITable uiTable;

		public Activity_contentBase[] template_content;

		private Dictionary<EActivityModuleType, Activity_contentBase> dicTemplateContent;

		private bool bReiposition;

		private SysActivityVo curActivity;

		private NoticeBoardData curBoardData;

		private Coroutine coroutine;

		public EActivityType AType
		{
			get;
			set;
		}

		public object CurItemInfo
		{
			get
			{
				if (this.AType == EActivityType.eActivity)
				{
					return this.curActivity;
				}
				return this.curBoardData;
			}
			set
			{
				if (this.AType == EActivityType.eActivity)
				{
					this.curActivity = (value as SysActivityVo);
				}
				else
				{
					this.curBoardData = (value as NoticeBoardData);
				}
			}
		}

		private void Awake()
		{
			this.dicTemplateContent = new Dictionary<EActivityModuleType, Activity_contentBase>();
			for (int i = 0; i < this.template_content.Length; i++)
			{
				if (!this.dicTemplateContent.ContainsKey(this.template_content[i].GetModuleType()))
				{
					this.dicTemplateContent.Add(this.template_content[i].GetModuleType(), this.template_content[i]);
				}
			}
		}

		private void Start()
		{
		}

		private void OnEnable()
		{
		}

		private void OnDisable()
		{
			this.ResetTable();
		}

		public void RefreshUI_content()
		{
			if (this.coroutine != null)
			{
				base.StopCoroutine("RefreshUI");
			}
			this.coroutine = base.StartCoroutine("RefreshUI");
		}

		[DebuggerHidden]
		private IEnumerator RefreshUI()
		{
			Activity_center.<RefreshUI>c__IteratorAF <RefreshUI>c__IteratorAF = new Activity_center.<RefreshUI>c__IteratorAF();
			<RefreshUI>c__IteratorAF.<>f__this = this;
			return <RefreshUI>c__IteratorAF;
		}

		[DebuggerHidden]
		private IEnumerator AddModule(object info)
		{
			Activity_center.<AddModule>c__IteratorB0 <AddModule>c__IteratorB = new Activity_center.<AddModule>c__IteratorB0();
			<AddModule>c__IteratorB.info = info;
			<AddModule>c__IteratorB.<$>info = info;
			<AddModule>c__IteratorB.<>f__this = this;
			return <AddModule>c__IteratorB;
		}

		[DebuggerHidden]
		private IEnumerator RepositionTable()
		{
			Activity_center.<RepositionTable>c__IteratorB1 <RepositionTable>c__IteratorB = new Activity_center.<RepositionTable>c__IteratorB1();
			<RepositionTable>c__IteratorB.<>f__this = this;
			return <RepositionTable>c__IteratorB;
		}

		private void OnReposition()
		{
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(base.transform, this.uiTable.transform);
			this.dragBox.size = bounds.size;
			this.dragBox.center = new Vector3(bounds.size.x / 2f, -bounds.size.y / 2f);
			this.dragSp.SetDimensions((int)bounds.size.x, (int)bounds.size.y);
			this.bReiposition = false;
		}

		private void ResetTable()
		{
			this.uiScrolview.ResetPosition();
			for (int i = 0; i < this.uiTable.transform.childCount; i++)
			{
				Transform child = this.uiTable.transform.GetChild(i);
				child.gameObject.SetActive(false);
				UnityEngine.Object.Destroy(child.gameObject);
			}
		}

		private EActivityModuleType GetModuleType(object obj)
		{
			EActivityModuleType result = EActivityModuleType.eNone;
			if (this.AType == EActivityType.eActivity)
			{
				SysActivityModuleVo sysActivityModuleVo = obj as SysActivityModuleVo;
				if (sysActivityModuleVo != null)
				{
					result = (EActivityModuleType)sysActivityModuleVo.type;
				}
			}
			else
			{
				NoticeModuleData noticeModuleData = obj as NoticeModuleData;
				if (noticeModuleData != null)
				{
					result = noticeModuleData.type;
				}
			}
			return result;
		}

		private List<object> GetDataList()
		{
			if (this.AType == EActivityType.eActivity)
			{
				return this.GetDataList_activity();
			}
			return this.GetDataList_notice();
		}

		private object GetCurData()
		{
			if (this.AType == EActivityType.eActivity)
			{
				return this.curActivity;
			}
			return this.curBoardData;
		}

		private List<object> GetDataList_activity()
		{
			List<object> list = new List<object>();
			if (this.curActivity != null)
			{
				if (string.IsNullOrEmpty(this.curActivity.module))
				{
					ClientLogger.Error("SysActivityVo 配置错误 没有任何模块");
				}
				else
				{
					string[] array = this.curActivity.module.Split(new char[]
					{
						','
					});
					for (int i = 0; i < array.Length; i++)
					{
						SysActivityModuleVo dataById = BaseDataMgr.instance.GetDataById<SysActivityModuleVo>(array[i]);
						if (dataById != null)
						{
							list.Add(dataById);
						}
					}
				}
			}
			return list;
		}

		private List<object> GetDataList_notice()
		{
			List<object> list = new List<object>();
			if (this.curBoardData != null)
			{
				list.Add(new NoticeModuleData
				{
					data = this.curBoardData,
					type = EActivityModuleType.eTitle
				});
				list.Add(new NoticeModuleData
				{
					data = this.curBoardData,
					type = EActivityModuleType.eText
				});
				if (this.curBoardData.type != 0)
				{
					list.Add(new NoticeModuleData
					{
						data = this.curBoardData,
						type = EActivityModuleType.eGoto
					});
				}
			}
			return list;
		}

		public bool NewbieGetFirstAwd()
		{
			if (this.uiTable == null)
			{
				return false;
			}
			List<Transform> children = this.uiTable.children;
			if (children == null || children.Count < 1)
			{
				return false;
			}
			Activity_content_reward activity_content_reward = null;
			for (int i = 0; i < children.Count; i++)
			{
				Transform transform = children[i];
				if (transform != null)
				{
					activity_content_reward = transform.GetComponent<Activity_content_reward>();
					if (activity_content_reward != null)
					{
						break;
					}
				}
			}
			return activity_content_reward != null && activity_content_reward.NewbieGetFirstAwd();
		}
	}
}
