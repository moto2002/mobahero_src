using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using GUIFramework;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class MailView : BaseView<MailView>
	{
		private UIGrid mailpanel_grid;

		private Transform emptyShow;

		private UILabel rd_title;

		private UILabel rd_content;

		private UILabel rd_addresser;

		private Transform rd_main;

		private UILabel rd_ul_gold;

		private UILabel rd_ul_dorgonGold;

		private UILabel rd_ul_diamond;

		private UILabel rd_ul_others;

		private GameObject pre_item;

		private GameObject clickObj;

		private CoroutineManager coroutineManager = new CoroutineManager();

		private List<MailData> mailDataList = new List<MailData>();

		private MailData _mailData;

		private bool isClickMailItem;

		private TweenAlpha m_AlphaController;

		public MailView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "MailView");
			this.WindowTitle = LanguageManager.Instance.GetStringById("MailUI_Title_Mail");
		}

		public override void Init()
		{
			base.Init();
			this.mailpanel_grid = this.transform.Find("Anchor/MailPanel/Grid").GetComponent<UIGrid>();
			this.emptyShow = this.transform.Find("Empty");
			this.m_AlphaController = this.transform.GetComponent<TweenAlpha>();
		}

		public override void HandleAfterOpenView()
		{
			this.m_AlphaController.Begin();
		}

		public override void HandleBeforeCloseView()
		{
		}

		public override void RegisterUpdateHandler()
		{
			MVC_MessageManager.AddListener_view(MobaGameCode.ReceiveMailAttachment, new MobaMessageFunc(this.ReceiveMailAttachmentCallback));
			this.RefreshUI();
		}

		public override void CancelUpdateHandler()
		{
			MVC_MessageManager.RemoveListener_view(MobaGameCode.ReceiveMailAttachment, new MobaMessageFunc(this.ReceiveMailAttachmentCallback));
			this.isClickMailItem = false;
		}

		public override void RefreshUI()
		{
			this.UpdateMailData();
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void UpdateMailData()
		{
			this.UpdateMailView();
		}

		private List<MailData> SortList(List<MailData> mailDataList)
		{
			if (mailDataList == null)
			{
				return new List<MailData>();
			}
			if (mailDataList.Count <= 1)
			{
				return mailDataList;
			}
			int num;
			for (int i = mailDataList.Count - 1; i > 0; i = num)
			{
				num = 0;
				for (int j = 0; j < i; j++)
				{
					if (mailDataList[j].Time.CompareTo(mailDataList[j + 1].Time) < 0)
					{
						num = j;
						MailData value = mailDataList[j];
						mailDataList[j] = mailDataList[j + 1];
						mailDataList[j + 1] = value;
					}
				}
			}
			return mailDataList;
		}

		public void UpdateMailView()
		{
			this.ClearMailPanel();
			GameObject prefab = base.LoadPrefabCache("MailPanelItem");
			this.mailDataList = ModelManager.Instance.Get_mailDataList_X();
			if (this.mailDataList == null || this.mailDataList.Count == 0)
			{
				this.emptyShow.gameObject.SetActive(true);
				return;
			}
			this.emptyShow.gameObject.SetActive(false);
			this.mailDataList = this.SortList(this.mailDataList);
			for (int i = 0; i < this.mailDataList.Count; i++)
			{
				GameObject gameObject = NGUITools.AddChild(this.mailpanel_grid.gameObject, prefab);
				gameObject.transform.Find("Time").GetComponent<UILabel>().text = this.mailDataList[i].Time.ToString("MM/dd/yyyy");
				if (this.mailDataList[i].TemplateId == 0)
				{
					string[] array = this.mailDataList[i].Mail_Param.Split(new char[]
					{
						'|'
					});
					gameObject.transform.Find("Title/Label").GetComponent<UILabel>().text = array[0];
					gameObject.transform.Find("Name").GetComponent<UILabel>().text = array[2];
				}
				else
				{
					SysMailInfoVo dataById = BaseDataMgr.instance.GetDataById<SysMailInfoVo>(this.mailDataList[i].TemplateId.ToString());
					gameObject.transform.Find("Title/Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById.mail_title);
					gameObject.transform.Find("Name").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById.addresser);
				}
				gameObject.name = this.mailDataList[i].Id.ToString();
				gameObject.AddComponent<UIEventListener>();
				UIEventListener expr_1E9 = UIEventListener.Get(gameObject.gameObject);
				expr_1E9.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_1E9.onClick, new UIEventListener.VoidDelegate(this.ShowInfoPanel));
				gameObject.transform.Find("ReadState").gameObject.SetActive(false);
				gameObject.transform.Find("UnReadState").gameObject.SetActive(true);
				gameObject.transform.Find("Title/Label").GetComponent<UILabel>().color = new Color(1f, 1f, 1f, 1f);
				gameObject.transform.Find("Icon/New").gameObject.SetActive(true);
				gameObject.transform.GetComponent<BoxCollider>().enabled = true;
			}
			this.mailpanel_grid.Reposition();
			this.mailpanel_grid.transform.parent.GetComponent<UIScrollView>().ResetPosition();
		}

		private void ClearMailPanel()
		{
			if (this.mailpanel_grid == null)
			{
				this.mailpanel_grid = this.transform.Find("Anchor/MailPanel/Grid").GetComponent<UIGrid>();
			}
			int childCount = this.mailpanel_grid.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				UnityEngine.Object.DestroyImmediate(this.mailpanel_grid.transform.GetChild(0).gameObject);
			}
		}

		[DebuggerHidden]
		private IEnumerator PanelReposition()
		{
			MailView.<PanelReposition>c__Iterator167 <PanelReposition>c__Iterator = new MailView.<PanelReposition>c__Iterator167();
			<PanelReposition>c__Iterator.<>f__this = this;
			return <PanelReposition>c__Iterator;
		}

		public void TryChangeState(GameObject obj)
		{
			long num = 0L;
			if (long.TryParse(obj.name, out num))
			{
				List<MailData> list = ModelManager.Instance.Get_mailDataList_X();
				if (list != null)
				{
					Singleton<MenuBottomBarView>.Instance.RemoveNews(10, obj.name);
				}
			}
			else
			{
				ClientLogger.Error("obj.name数据越界，值为" + obj.name);
			}
		}

		private void ShowInfoPanel(GameObject obj)
		{
			if (this.isClickMailItem)
			{
				return;
			}
			this.isClickMailItem = true;
			this.clickObj = obj;
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在打开邮件", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.ReceiveMailAttachment, param, new object[]
			{
				long.Parse(obj.name)
			});
			this._mailData = this.mailDataList.Find((MailData data) => data.Id == long.Parse(obj.name));
		}

		private void ReceiveMailAttachmentCallback(MobaMessage msg)
		{
			this.isClickMailItem = false;
			if (null == this.clickObj || this._mailData == null)
			{
				return;
			}
			CtrlManager.OpenWindow(WindowID.MailShowPanel, null);
			Singleton<MailShowPanel>.Instance.ShowInfoPanel(this._mailData, this.clickObj);
			this.TryChangeState(this.clickObj);
			MailData item2 = ModelManager.Instance.Get_mailDataList_X().Find((MailData item) => item.Id == this._mailData.Id);
			ModelManager.Instance.Get_mailDataList_X().Remove(item2);
			this.UpdateMailView();
		}

		public void GetRewardResult(List<RewardModel> rewardModelList)
		{
			if (rewardModelList == null)
			{
				return;
			}
			for (int i = 0; i < rewardModelList.Count; i++)
			{
				switch (rewardModelList[i].Type)
				{
				case 1:
					if (rewardModelList[i].Id == "1")
					{
						ModelManager.Instance.Get_userData_X().Money += (long)rewardModelList[i].Count;
					}
					else if (rewardModelList[i].Id == "2")
					{
						ModelManager.Instance.Get_userData_X().Diamonds += (long)rewardModelList[i].Count;
					}
					else if (rewardModelList[i].Id == "9")
					{
						ModelManager.Instance.Get_userData_X().SmallCap += rewardModelList[i].Count;
					}
					break;
				case 2:
					ModelManager.Instance.Get_userData_X().Exp += (long)rewardModelList[i].Count;
					CharacterDataMgr.instance.SaveNowUserLevel(ModelManager.Instance.Get_userData_X().Exp);
					break;
				case 6:
					SendMsgManager.Instance.SendMsg(MobaGameCode.GetDoubleCard, null, new object[0]);
					break;
				}
			}
		}
	}
}
