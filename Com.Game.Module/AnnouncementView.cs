using GUIFramework;
using MobaHeros;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Com.Game.Module
{
	public class AnnouncementView : BaseView<AnnouncementView>
	{
		private Transform makeSure;

		private Transform cancel;

		private Transform sure;

		private Transform content;

		private UIPanel panel;

		private UIScrollView scrollView;

		private Transform BlackBG;

		private UILabel label;

		private TweenAlpha m_AlphaController;

		private TweenAlpha m_AlphaController1;

		private bool isInit;

		private int tryTime;

		private CoroutineManager coroutineManager = new CoroutineManager();

		public AnnouncementView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Home/AnnouncementView");
		}

		public override void Init()
		{
			base.Init();
			this.makeSure = this.transform.Find("Anchor/MakeSure");
			this.content = this.makeSure.Find("ContentPanel");
			this.sure = this.makeSure.Find("Sure");
			this.panel = this.gameObject.GetComponent<UIPanel>();
			this.scrollView = this.content.GetComponent<UIScrollView>();
			this.BlackBG = this.makeSure.Find("BlackBG");
			this.label = this.content.Find("Content").GetComponent<UILabel>();
			this.panel.alpha = 0f;
			UIEventListener expr_BB = UIEventListener.Get(this.sure.gameObject);
			expr_BB.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_BB.onClick, new UIEventListener.VoidDelegate(this.ClickMakeSureSure));
			UIEventListener expr_EC = UIEventListener.Get(this.BlackBG.gameObject);
			expr_EC.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_EC.onClick, new UIEventListener.VoidDelegate(this.ClickMakeSureSure));
			this.m_AlphaController = this.transform.GetComponent<TweenAlpha>();
			this.m_AlphaController1 = this.content.GetComponentInChildren<TweenAlpha>();
		}

		public override void HandleAfterOpenView()
		{
			AutoTestController.InvokeTestLogic(AutoTestTag.Home, delegate
			{
				CtrlManager.CloseWindow(WindowID.AnnouncementView);
			}, 1f);
		}

		public override void HandleBeforeCloseView()
		{
		}

		public override void RegisterUpdateHandler()
		{
			this.InitLogTxt();
			this.UpdateTaskData();
			this.m_AlphaController.Begin();
		}

		public override void CancelUpdateHandler()
		{
		}

		public void InitLogTxt()
		{
			string[] array = GlobalSettings.AppVersion.Split(new char[]
			{
				'.'
			});
			string name = string.Concat(new string[]
			{
				"Log_",
				array[0],
				"_",
				array[1],
				"_",
				(int.Parse(array[2]) - this.tryTime).ToString(),
				".txt"
			});
			this.coroutineManager.StartCoroutine(this.StartCheckAndDownload(name), true);
		}

		[DebuggerHidden]
		private IEnumerator StartCheckAndDownload(string _name)
		{
			AnnouncementView.<StartCheckAndDownload>c__Iterator11C <StartCheckAndDownload>c__Iterator11C = new AnnouncementView.<StartCheckAndDownload>c__Iterator11C();
			<StartCheckAndDownload>c__Iterator11C._name = _name;
			<StartCheckAndDownload>c__Iterator11C.<$>_name = _name;
			<StartCheckAndDownload>c__Iterator11C.<>f__this = this;
			return <StartCheckAndDownload>c__Iterator11C;
		}

		private void SetLabelText(string logTxt)
		{
			if (logTxt[0] != '\0')
			{
				logTxt = logTxt.Remove(0, 1);
			}
			if (logTxt.Length > 10000)
			{
				logTxt = logTxt.Remove(10000, logTxt.Length - 10000);
				logTxt += "...\n其他内容请去官网查看详情。";
			}
			this.label.text = logTxt;
		}

		private void CreateORwriteConfigFile(string path, string name, string info)
		{
			FileInfo fileInfo = new FileInfo(path + "//" + name);
			StreamWriter streamWriter;
			if (!fileInfo.Exists)
			{
				streamWriter = fileInfo.CreateText();
			}
			else
			{
				streamWriter = fileInfo.AppendText();
			}
			streamWriter.WriteLine(info);
			streamWriter.Close();
			streamWriter.Dispose();
		}

		private string LoadFile(string path, string name)
		{
			StreamReader streamReader = null;
			try
			{
				streamReader = File.OpenText(path + "//" + name);
			}
			catch (Exception var_1_19)
			{
				return null;
			}
			string text = string.Empty;
			string str;
			while ((str = streamReader.ReadLine()) != null)
			{
				text = text + str + "\n";
			}
			streamReader.Close();
			streamReader.Dispose();
			return text;
		}

		public override void RefreshUI()
		{
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void UpdateTaskData()
		{
			this.scrollView.ResetPosition();
			this.m_AlphaController1.Begin();
		}

		[DebuggerHidden]
		private IEnumerator ShowLabel()
		{
			AnnouncementView.<ShowLabel>c__Iterator11D <ShowLabel>c__Iterator11D = new AnnouncementView.<ShowLabel>c__Iterator11D();
			<ShowLabel>c__Iterator11D.<>f__this = this;
			return <ShowLabel>c__Iterator11D;
		}

		private void ClickMakeSureSure(GameObject objct_1 = null)
		{
			CtrlManager.CloseWindow(WindowID.AnnouncementView);
		}
	}
}
