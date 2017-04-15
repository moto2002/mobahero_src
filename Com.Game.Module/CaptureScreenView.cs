using Assets.Scripts.Model;
using GUIFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Com.Game.Module
{
	public class CaptureScreenView : BaseView<CaptureScreenView>
	{
		public class PicFileData
		{
			public string fileName = string.Empty;

			public string fullFilePath = string.Empty;

			public PicFileData(string _fileName, string _fullFilePath)
			{
				this.fileName = _fileName;
				this.fullFilePath = _fullFilePath;
			}
		}

		public class StringExtention
		{
			public static string[] SplitWithString(string sourceString, string splitString)
			{
				List<string> list = new List<string>();
				string item = string.Empty;
				while (sourceString.IndexOf(splitString) > -1)
				{
					item = sourceString.Substring(0, sourceString.IndexOf(splitString));
					sourceString = sourceString.Substring(sourceString.IndexOf(splitString) + splitString.Length);
					list.Add(item);
				}
				list.Add(sourceString);
				return list.ToArray();
			}
		}

		private Transform makeSure;

		private Transform cancel;

		private Transform sure;

		private Transform content;

		private UIPanel panel;

		private UIScrollView scrollView;

		private UIGrid grid;

		private Transform BlackBG;

		private UILabel label;

		private UITexture bigTexture;

		private Transform bigTextureBg;

		private TweenAlpha m_AlphaController;

		private TweenAlpha m_AlphaController1;

		private GameObject hideObject;

		private bool isInit;

		private int tryTime;

		private CoroutineManager coroutineManager = new CoroutineManager();

		private List<CaptureScreenView.PicFileData> fileArray = new List<CaptureScreenView.PicFileData>();

		private GameObject captureScreenItemPre;

		private List<CaptureScreenItem> captureScreenList = new List<CaptureScreenItem>();

		private string _path;

		private string roomId = string.Empty;

		public CaptureScreenView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Home/CaptureScreenView");
		}

		public override void Init()
		{
			base.Init();
			this.makeSure = this.transform.Find("Anchor/MakeSure");
			this.content = this.makeSure.Find("ContentPanel/Content");
			this.sure = this.makeSure.Find("Sure");
			this.panel = this.gameObject.GetComponent<UIPanel>();
			this.scrollView = this.content.parent.GetComponent<UIScrollView>();
			this.grid = this.content.GetComponent<UIGrid>();
			this.BlackBG = this.makeSure.Find("BlackBG");
			this.bigTextureBg = this.transform.Find("Anchor/MakeSure/ShowBigPic");
			this.bigTexture = this.bigTextureBg.transform.Find("Pic").GetComponent<UITexture>();
			this.panel.alpha = 0f;
			UIEventListener expr_EC = UIEventListener.Get(this.sure.gameObject);
			expr_EC.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_EC.onClick, new UIEventListener.VoidDelegate(this.ClickMakeSureSure));
			UIEventListener expr_11D = UIEventListener.Get(this.BlackBG.gameObject);
			expr_11D.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_11D.onClick, new UIEventListener.VoidDelegate(this.ClickMakeSureSure));
			UIEventListener expr_158 = UIEventListener.Get(this.bigTextureBg.Find("Bg").gameObject);
			expr_158.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_158.onClick, new UIEventListener.VoidDelegate(this.CloseBigTextureBg));
			this.m_AlphaController = this.transform.GetComponent<TweenAlpha>();
			this.m_AlphaController1 = this.content.GetComponentInChildren<TweenAlpha>();
		}

		public override void HandleAfterOpenView()
		{
		}

		public override void HandleBeforeCloseView()
		{
		}

		public override void RegisterUpdateHandler()
		{
			if (this.hideObject != null)
			{
				this.hideObject.gameObject.SetActive(false);
			}
			this.ShowPicList();
			this.m_AlphaController.Begin();
		}

		private void ShowPicList()
		{
			this._path = Application.persistentDataPath + "/MobaPic/" + ModelManager.Instance.Get_userData_X().SummonerId.ToString();
			if (this.roomId != string.Empty)
			{
				this._path = this._path + "/" + this.roomId;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(this._path);
			FileInfo[] array = new FileInfo[0];
			if (!directoryInfo.Exists)
			{
				Directory.CreateDirectory(this._path);
			}
			else
			{
				array = directoryInfo.GetFiles();
			}
			this.GetObjectNameToArray(this._path, "jpg");
			if (this.fileArray != null)
			{
				for (int i = 0; i < this.fileArray.Count; i++)
				{
					if (this.captureScreenItemPre == null)
					{
						this.captureScreenItemPre = (Resources.Load("Prefab/UI/Home/CaptureScreenItem") as GameObject);
					}
					CaptureScreenItem component = NGUITools.AddChild(this.content.gameObject, this.captureScreenItemPre).GetComponent<CaptureScreenItem>();
					this.captureScreenList.Add(component);
					component.picFileData = this.fileArray[i];
					string[] array2 = this.fileArray[i].fileName.Split(new char[]
					{
						'/'
					});
					component.picNameLabel.text = array2[array2.Length - 1];
					component.timeLabel.text = Directory.GetLastWriteTimeUtc(this.fileArray[i].fullFilePath).ToShortDateString();
					this.coroutineManager.StartCoroutine(this.LoadPic(component.picFileData.fullFilePath, component.pic), true);
					UIEventListener.Get(component.deleteBtn).onClick = new UIEventListener.VoidDelegate(this.DeletePic);
					UIEventListener.Get(component.shareBtn).onClick = new UIEventListener.VoidDelegate(this.ShareBtn);
					UIEventListener.Get(component.pic.gameObject).onClick = new UIEventListener.VoidDelegate(this.ShowBigPic);
				}
			}
			this.grid.Reposition();
			this.scrollView.ResetPosition();
		}

		[DebuggerHidden]
		private IEnumerator LoadPic(string path, UITexture tex)
		{
			CaptureScreenView.<LoadPic>c__Iterator11E <LoadPic>c__Iterator11E = new CaptureScreenView.<LoadPic>c__Iterator11E();
			<LoadPic>c__Iterator11E.path = path;
			<LoadPic>c__Iterator11E.tex = tex;
			<LoadPic>c__Iterator11E.<$>path = path;
			<LoadPic>c__Iterator11E.<$>tex = tex;
			return <LoadPic>c__Iterator11E;
		}

		private void DeletePic(GameObject _obj)
		{
			CaptureScreenItem component = _obj.transform.parent.GetComponent<CaptureScreenItem>();
			if (component != null)
			{
				UnityEngine.Object.DestroyImmediate(_obj.transform.parent.gameObject);
				this.captureScreenList.Remove(component);
				this.fileArray.Remove(component.picFileData);
				File.Delete(component.picFileData.fullFilePath);
				this.grid.Reposition();
				this.scrollView.ResetPosition();
			}
		}

		private void ShareBtn(GameObject _obj)
		{
			GameManager.Instance.DoShareSDK(1, new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), null);
		}

		private void ShowBigPic(GameObject pic)
		{
			this.bigTexture.mainTexture = pic.transform.parent.GetComponent<CaptureScreenItem>().pic.mainTexture;
			this.bigTextureBg.gameObject.SetActive(true);
		}

		private void CloseBigTextureBg(GameObject obj)
		{
			this.bigTextureBg.gameObject.SetActive(false);
		}

		private void GetObjectNameToArray(string path, string pattern)
		{
			try
			{
				string[] fileSystemEntries = Directory.GetFileSystemEntries(path);
				for (int i = 0; i < fileSystemEntries.Length; i++)
				{
					string text = fileSystemEntries[i];
					string[] array = CaptureScreenView.StringExtention.SplitWithString(text, path + "\\");
					if (array.Length <= 1 || !array[1].EndsWith(".meta"))
					{
						string[] array2;
						if (array.Length > 1)
						{
							array2 = CaptureScreenView.StringExtention.SplitWithString(array[1], ".");
						}
						else
						{
							array2 = CaptureScreenView.StringExtention.SplitWithString(array[0], ".");
						}
						if (array2.Length > 1)
						{
							this.fileArray.Add(new CaptureScreenView.PicFileData(array2[0], text));
						}
						else
						{
							this.GetObjectNameToArray(path + "/" + array2[0], "pattern");
						}
					}
				}
			}
			catch (DirectoryNotFoundException)
			{
			}
		}

		public void HideObject(GameObject obj)
		{
			this.hideObject = obj;
		}

		public override void CancelUpdateHandler()
		{
			if (this.hideObject != null)
			{
				this.hideObject.gameObject.SetActive(true);
				this.hideObject = null;
			}
			this.ClearPanel();
		}

		private void ClearPanel()
		{
			for (int i = 0; i < this.captureScreenList.Count; i++)
			{
				if (this.captureScreenList[i] != null)
				{
					UnityEngine.Object.Destroy(this.captureScreenList[i].gameObject);
				}
				this.captureScreenList[i] = null;
			}
			this.captureScreenList.Clear();
			this.fileArray.Clear();
			this.captureScreenItemPre = null;
			this.roomId = string.Empty;
		}

		public void SetRoomId(string roomID)
		{
			this.roomId = roomID;
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

		private void ClickMakeSureSure(GameObject objct_1 = null)
		{
			CtrlManager.CloseWindow(WindowID.CaptureScreenView);
		}
	}
}
