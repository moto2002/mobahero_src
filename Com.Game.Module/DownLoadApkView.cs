using Assets.Scripts.Model;
using GUIFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	internal class DownLoadApkView : BaseView<DownLoadApkView>
	{
		private List<DownLoadAPKNewsItem> listItems;

		private Dictionary<int, DownLoadAPKNewsItem> items;

		private List<ILoginNewsItem> coms;

		private List<UIToggle> toggles;

		private UIGrid grid_news;

		private UIGrid grid_controls;

		private UICenterOnChild centerCom;

		private LoginNewsItem_texture template_pic;

		private LoginNewsItem_notice template_txt;

		private UIToggle template_toggle;

		private UIButton leftBtn;

		private UIButton rightBtn;

		private int curNewsKey;

		private bool press;

		private float interval;

		private Task autoPlayTask;

		private UIButton btn_update;

		public DownLoadApkView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Login/DownLoadAPK");
		}

		public override void Init()
		{
			base.Init();
			this.btn_update = this.transform.FindChild("Bottom/DownLoad").GetComponent<UIButton>();
			this.grid_news = this.transform.FindChild("Top/News/grid").GetComponent<UIGrid>();
			this.grid_controls = this.transform.FindChild("Top/controls/grid").GetComponent<UIGrid>();
			this.centerCom = this.grid_news.GetComponent<UICenterOnChild>();
			this.template_pic = this.transform.FindChild("Top/News/template_texture").GetComponent<LoginNewsItem_texture>();
			this.template_txt = this.transform.FindChild("Top/News/template_text").GetComponent<LoginNewsItem_notice>();
			this.template_toggle = this.transform.FindChild("Top/controls/controlItem").GetComponent<UIToggle>();
			this.leftBtn = this.transform.FindChild("Top/controls2/left").GetComponent<UIButton>();
			this.rightBtn = this.transform.FindChild("Top/controls2/right").GetComponent<UIButton>();
			UIEventListener.Get(this.btn_update.gameObject).onClick = new UIEventListener.VoidDelegate(this.Onclick_update);
			UIEventListener.Get(this.leftBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClick_leftBtn);
			UIEventListener.Get(this.rightBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClick_rightBtn);
			this.centerCom.onFinished = new SpringPanel.OnFinished(this.CenterChildFinish);
			this.items = new Dictionary<int, DownLoadAPKNewsItem>();
			this.listItems = new List<DownLoadAPKNewsItem>();
			this.coms = new List<ILoginNewsItem>();
			this.toggles = new List<UIToggle>();
			this.curNewsKey = -1;
			this.press = false;
			this.interval = 0f;
		}

		public override void RegisterUpdateHandler()
		{
		}

		public override void HandleAfterOpenView()
		{
			Task task = new Task(this.DownloadXML(), true);
			task.Finished += new Task.FinishedHandler(this.OnDownLoadXMLFinish);
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void HandleBeforeCloseView()
		{
			this.autoPlayTask.Stop();
			this.autoPlayTask = null;
		}

		public override void RefreshUI()
		{
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void Onclick_update(GameObject obj = null)
		{
			UniWebViewFacade.Instance.OpenUrl(ModelManager.Instance.Get_AppUpgradeUrl());
		}

		private void OnClick_leftBtn(GameObject obj)
		{
			if (this.curNewsKey > 0)
			{
				this.curNewsKey--;
				this.RefreshUI_curToggle();
				this.RefreshUI_leftRightBtn();
				this.RefreshUI_curNews();
				this.interval = 5f;
			}
		}

		private void OnClick_rightBtn(GameObject obj)
		{
			if (this.curNewsKey < this.items.Count - 1)
			{
				this.curNewsKey++;
				this.RefreshUI_curToggle();
				this.RefreshUI_leftRightBtn();
				this.RefreshUI_curNews();
				this.interval = 5f;
			}
		}

		private void OnClick_toggle(GameObject obj = null)
		{
			int num = -1;
			if (int.TryParse(obj.name, out num))
			{
				this.curNewsKey = num;
				this.RefreshUI_leftRightBtn();
				this.RefreshUI_curNews();
				this.interval = 5f;
			}
		}

		private void OnPress_item(GameObject go, bool state)
		{
			this.press = state;
			if (!state)
			{
				this.interval = 5f;
			}
		}

		private void CenterChildFinish()
		{
			GameObject centeredObject = this.centerCom.centeredObject;
			int num = -1;
			if (int.TryParse(centeredObject.name, out num) && this.curNewsKey != num)
			{
				this.curNewsKey = num;
				this.RefreshUI_leftRightBtn();
				this.RefreshUI_curToggle();
			}
		}

		private void OnDownLoadXMLFinish(bool manual)
		{
			this.RefreshUI_InitItems();
			this.RefreshUI_curToggle();
			this.RefreshUI_leftRightBtn();
		}

		private void RefreshUI_InitItems()
		{
			for (int i = 0; i < this.listItems.Count; i++)
			{
				DownLoadAPKNewsItem downLoadAPKNewsItem = this.listItems[i];
				if (downLoadAPKNewsItem != null)
				{
					if (this.curNewsKey < 0)
					{
						this.curNewsKey = i;
					}
					this.InitNewsItem(downLoadAPKNewsItem, i);
					this.InitControlItem(downLoadAPKNewsItem, i);
				}
			}
			this.grid_news.Reposition();
			this.grid_controls.Reposition();
		}

		private void InitControlItem(DownLoadAPKNewsItem info, int index)
		{
			GameObject gameObject = NGUITools.AddChild(this.grid_controls.gameObject, this.template_toggle.gameObject);
			gameObject.name = index.ToString();
			gameObject.SetActive(true);
			this.toggles.Add(gameObject.GetComponent<UIToggle>());
			UIEventListener.Get(gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClick_toggle);
		}

		private void InitNewsItem(DownLoadAPKNewsItem info, int index)
		{
			GameObject gameObject;
			ILoginNewsItem component;
			if (info.type == 1)
			{
				gameObject = NGUITools.AddChild(this.grid_news.gameObject, this.template_pic.gameObject);
				component = gameObject.GetComponent<LoginNewsItem_texture>();
			}
			else
			{
				gameObject = NGUITools.AddChild(this.grid_news.gameObject, this.template_txt.gameObject);
				component = gameObject.GetComponent<LoginNewsItem_notice>();
			}
			gameObject.name = index.ToString();
			component.Info = info;
			component.CheckResource();
			component.OnHandle = new Action<GameObject, bool>(this.OnPress_item);
			this.coms.Add(component);
			gameObject.SetActive(true);
		}

		private void RefreshUI_curNews()
		{
			if (this.curNewsKey >= 0 && this.curNewsKey < this.coms.Count)
			{
				this.centerCom.CenterOn(this.coms[this.curNewsKey].Obj.transform);
			}
		}

		private void RefreshUI_curToggle()
		{
			if (this.curNewsKey >= 0 && this.curNewsKey < this.toggles.Count)
			{
				UIToggle uIToggle = this.toggles[this.curNewsKey];
				uIToggle.value = true;
			}
		}

		private void RefreshUI_leftRightBtn()
		{
			if (this.curNewsKey >= 0 && this.curNewsKey < this.items.Count)
			{
				this.leftBtn.isEnabled = (this.curNewsKey > 0);
				this.rightBtn.isEnabled = (this.curNewsKey < this.items.Count - 1);
			}
		}

		[DebuggerHidden]
		private IEnumerator DownloadXML()
		{
			DownLoadApkView.<DownloadXML>c__Iterator15E <DownloadXML>c__Iterator15E = new DownLoadApkView.<DownloadXML>c__Iterator15E();
			<DownloadXML>c__Iterator15E.<>f__this = this;
			return <DownloadXML>c__Iterator15E;
		}

		[DebuggerHidden]
		private IEnumerator AutoPlay()
		{
			DownLoadApkView.<AutoPlay>c__Iterator15F <AutoPlay>c__Iterator15F = new DownLoadApkView.<AutoPlay>c__Iterator15F();
			<AutoPlay>c__Iterator15F.<>f__this = this;
			return <AutoPlay>c__Iterator15F;
		}
	}
}
