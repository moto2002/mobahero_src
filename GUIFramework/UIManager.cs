using Assets.Scripts.Server;
using Com.Game.Module;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GUIFramework
{
	public class UIManager : IGlobalComServer
	{
		private class CompareBaseWindow : IComparer<TUIWindow>
		{
			public int Compare(TUIWindow left, TUIWindow right)
			{
				return left.MinDepth - right.MinDepth;
			}
		}

		public delegate void OpenWindowEventHandler(OpenWindowEventArgs e);

		public delegate void CloseWindowEventHandler(CloseWindowEventArgs e);

		private Dictionary<string, TUIWindow> mDicAllWindows;

		private Dictionary<string, TUIWindow> mDicOpenWindows;

		private Stack<ReturnWinSeqData> mSepReturnWins;

		private TUIWindow mCurNormalWin;

		private TUIWindow mPreNormalWin;

		private Transform mBgRoot;

		private Transform mNormalRoot;

		private Transform mFixedRoot;

		private Transform mPopupRoot;

		private Transform mHintRoot;

		private Transform mTopRoot;

		private static UIManager mInstance;

		public event UIManager.OpenWindowEventHandler OnOpened
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.OnOpened = (UIManager.OpenWindowEventHandler)Delegate.Combine(this.OnOpened, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.OnOpened = (UIManager.OpenWindowEventHandler)Delegate.Remove(this.OnOpened, value);
			}
		}

		public event UIManager.CloseWindowEventHandler OnClosed
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.OnClosed = (UIManager.CloseWindowEventHandler)Delegate.Combine(this.OnClosed, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.OnClosed = (UIManager.CloseWindowEventHandler)Delegate.Remove(this.OnClosed, value);
			}
		}

		public static UIManager Instance
		{
			get
			{
				if (UIManager.mInstance == null)
				{
					throw new Exception("uninitialize UIManager");
				}
				return UIManager.mInstance;
			}
		}

		public void OnAwake()
		{
			UIManager.mInstance = this;
			this.Init();
		}

		public void OnStart()
		{
		}

		public void OnUpdate()
		{
		}

		public void OnDestroy()
		{
		}

		public void Enable(bool b)
		{
		}

		public void OnRestart()
		{
			this.DestroyAllWindows();
		}

		public void OnApplicationQuit()
		{
		}

		public void OnApplicationFocus(bool isFocus)
		{
		}

		public void OnApplicationPause(bool isPause)
		{
		}

		public void Init()
		{
			CtrlManager.Init();
			this.mDicAllWindows = new Dictionary<string, TUIWindow>();
			this.mDicOpenWindows = new Dictionary<string, TUIWindow>();
			this.mSepReturnWins = new Stack<ReturnWinSeqData>();
			if (this.mBgRoot == null)
			{
				this.mBgRoot = NGUITools.AddChild(ViewTree.home.transform.parent.gameObject).transform;
				this.mBgRoot.name = "BackgroundRoot";
			}
			if (this.mNormalRoot == null)
			{
				this.mNormalRoot = NGUITools.AddChild(ViewTree.home.transform.parent.gameObject).transform;
				this.mNormalRoot.name = "NormalRoot";
			}
			if (this.mFixedRoot == null)
			{
				this.mFixedRoot = NGUITools.AddChild(ViewTree.home.transform.parent.gameObject).transform;
				this.mFixedRoot.name = "FixedRoot";
			}
			if (this.mPopupRoot == null)
			{
				this.mPopupRoot = NGUITools.AddChild(ViewTree.home.transform.parent.gameObject).transform;
				this.mPopupRoot.name = "PopupRoot";
			}
			if (this.mHintRoot == null)
			{
				this.mHintRoot = NGUITools.AddChild(ViewTree.home.transform.parent.gameObject).transform;
				this.mHintRoot.name = "HintRoot";
			}
			if (this.mTopRoot == null)
			{
				this.mTopRoot = NGUITools.AddChild(ViewTree.home.transform.parent.gameObject).transform;
				this.mTopRoot.name = "TopRoot";
			}
		}

		public void ShowPreWindow()
		{
			if (this.mSepReturnWins.Count == 0)
			{
				CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
				CtrlManager.CloseWindow(WindowID.PvpRoomView);
				CtrlManager.CloseWindow(WindowID.ArenaModeView);
				CtrlManager.CloseWindow(WindowID.MenuView);
				CtrlManager.OpenWindow(WindowID.MenuView, null);
				CtrlManager.OpenWindow(WindowID.MenuBottomBarView, null);
				return;
			}
			this.DoShowPreWindow();
		}

		public void PreloadWindow(string winName, WinResurceCfg resCfg)
		{
			TUIWindow tUIWindow = this.ReadyToShowWin(winName, resCfg);
		}

		public void OpenWindow(string winName, WinResurceCfg resCfg)
		{
			TUIWindow tUIWindow = this.ReadyToShowWin(winName, resCfg);
			if (tUIWindow)
			{
				this.AdjustWinDepth(tUIWindow);
				this.RefreshReturnSeq(winName, tUIWindow);
				this.DoShowWindow(winName, tUIWindow);
				if (tUIWindow.DataCfg.WinType == WindowType.Normal)
				{
					this.mPreNormalWin = this.mCurNormalWin;
					this.mCurNormalWin = tUIWindow;
				}
				if (tUIWindow.DataCfg.IsOnset)
				{
					this.ClearReturnSeq();
				}
			}
		}

		public void CloseWindow(string winName)
		{
			if (this.mDicOpenWindows.ContainsKey(winName))
			{
				TUIWindow tUIWindow = this.mDicOpenWindows[winName];
				this.DoCloseWindow(winName, tUIWindow.DataCfg.IsDestroy);
			}
			else if (this.OnClosed != null)
			{
				this.OnClosed(new CloseWindowEventArgs(false, winName, false, false, null));
			}
		}

		public void DestroyAllWindows()
		{
			List<string> list = new List<string>(this.mDicAllWindows.Keys);
			TUIWindow tUIWindow = null;
			for (int i = 0; i < list.Count; i++)
			{
				if (this.mDicAllWindows.TryGetValue(list[i], out tUIWindow))
				{
					if (this.OnClosed != null)
					{
						this.OnClosed(new CloseWindowEventArgs(true, list[i], true, true, tUIWindow));
					}
					if (tUIWindow != null)
					{
						tUIWindow.CloseWindow(true);
					}
				}
			}
			this.mDicAllWindows.Clear();
			this.mDicOpenWindows.Clear();
			this.mSepReturnWins.Clear();
		}

		public void DestroyAllWindowsExcept(params string[] winIdsIgnored)
		{
			UIManager.<DestroyAllWindowsExcept>c__AnonStorey20E <DestroyAllWindowsExcept>c__AnonStorey20E = new UIManager.<DestroyAllWindowsExcept>c__AnonStorey20E();
			if (winIdsIgnored == null)
			{
				winIdsIgnored = new string[0];
			}
			<DestroyAllWindowsExcept>c__AnonStorey20E.winNames = new List<string>(this.mDicAllWindows.Keys);
			TUIWindow tUIWindow = null;
			int i;
			for (i = 0; i < <DestroyAllWindowsExcept>c__AnonStorey20E.winNames.Count; i++)
			{
				if (Array.FindIndex<string>(winIdsIgnored, (string x) => x == <DestroyAllWindowsExcept>c__AnonStorey20E.winNames[i]) < 0)
				{
					if (this.mDicAllWindows.TryGetValue(<DestroyAllWindowsExcept>c__AnonStorey20E.winNames[i], out tUIWindow))
					{
						if (this.OnClosed != null)
						{
							this.OnClosed(new CloseWindowEventArgs(true, <DestroyAllWindowsExcept>c__AnonStorey20E.winNames[i], true, false, tUIWindow));
						}
						if (tUIWindow != null)
						{
							tUIWindow.CloseWindow(true);
						}
						this.mDicAllWindows.Remove(<DestroyAllWindowsExcept>c__AnonStorey20E.winNames[i]);
						this.mDicOpenWindows.Remove(<DestroyAllWindowsExcept>c__AnonStorey20E.winNames[i]);
					}
				}
			}
			this.mSepReturnWins.Clear();
		}

		public void DestroyWindow(string winID)
		{
			if (this.mDicAllWindows.ContainsKey(winID))
			{
				UnityEngine.Object.Destroy(this.mDicAllWindows[winID].gameObj);
				this.mDicAllWindows.Remove(winID);
			}
			Resources.UnloadUnusedAssets();
		}

		private TUIWindow ReadyToShowWin(string winName, WinResurceCfg resCfg)
		{
			if (this.mDicOpenWindows.ContainsKey(winName))
			{
				return null;
			}
			if (this.mDicAllWindows.ContainsKey(winName))
			{
				if (this.mDicAllWindows[winName])
				{
					return this.mDicAllWindows[winName];
				}
				this.mDicAllWindows.Remove(winName);
			}
			GameObject gameObject;
			if (resCfg.IsLoadFromConfig)
			{
				gameObject = ResourceManager.Load<GameObject>(resCfg.Url, true, resCfg.IsAssetbundle, null, 0, false);
			}
			else
			{
				gameObject = (Resources.Load(resCfg.Url) as GameObject);
			}
			if (gameObject == null)
			{
				return null;
			}
			TUIWindow component = gameObject.GetComponent<TUIWindow>();
			if (component == null)
			{
				return null;
			}
			GameObject gameObject2 = NGUITools.AddChild(this.GetWindowRoot(component.DataCfg.WinType).gameObject, gameObject);
			component = gameObject2.GetComponent<TUIWindow>();
			component.WinName = winName;
			this.mDicAllWindows.Add(winName, component);
			return component;
		}

		private void DoShowWindow(string winName, TUIWindow uiWindow)
		{
			this.mDicOpenWindows[winName] = uiWindow;
			uiWindow.OpenWindow();
			if (this.OnOpened != null)
			{
				this.OnOpened(new OpenWindowEventArgs(true, winName, uiWindow));
			}
		}

		private void DoCloseWindow(string winName, bool isDestroy)
		{
			TUIWindow tUIWindow = this.mDicOpenWindows[winName];
			if (this.OnClosed != null)
			{
				this.OnClosed(new CloseWindowEventArgs(true, winName, isDestroy, false, tUIWindow));
			}
			if (tUIWindow == null)
			{
				return;
			}
			tUIWindow.CloseWindow(isDestroy);
			if (isDestroy && this.mDicAllWindows.ContainsKey(winName))
			{
				this.mDicAllWindows.Remove(winName);
			}
			this.mDicOpenWindows.Remove(winName);
		}

		public void HideAllOpenWindow(bool includeFixed)
		{
			if (!includeFixed)
			{
				List<string> list = new List<string>();
				foreach (KeyValuePair<string, TUIWindow> current in this.mDicOpenWindows)
				{
					if (current.Value.DataCfg.WinType != WindowType.Fixed)
					{
						list.Add(current.Key);
						current.Value.gameObj.SetActive(false);
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					this.OnClosed(new CloseWindowEventArgs(true, list[i], false, false, this.mDicOpenWindows[list[i]]));
					this.mDicOpenWindows.Remove(list[i]);
				}
			}
			else
			{
				foreach (KeyValuePair<string, TUIWindow> current2 in this.mDicOpenWindows)
				{
					this.OnClosed(new CloseWindowEventArgs(true, current2.Key, false, false, current2.Value));
					current2.Value.gameObj.SetActive(false);
				}
				this.mDicOpenWindows.Clear();
			}
		}

		private Transform GetWindowRoot(WindowType winType)
		{
			if (winType == WindowType.Background)
			{
				return this.mBgRoot;
			}
			if (winType == WindowType.Normal)
			{
				return this.mNormalRoot;
			}
			if (winType == WindowType.Fixed || winType == WindowType.SemiFixed)
			{
				return this.mFixedRoot;
			}
			if (winType == WindowType.Popup)
			{
				return this.mPopupRoot;
			}
			if (winType == WindowType.Hint)
			{
				return this.mHintRoot;
			}
			if (winType == WindowType.Highest)
			{
				return this.mTopRoot;
			}
			return this.mNormalRoot.parent;
		}

		private void AdjustWinDepth(TUIWindow win)
		{
			WindowType winType = win.DataCfg.WinType;
			int num = 1;
			if (winType == WindowType.Background)
			{
				num = Mathf.Clamp(UIUtils.GetMaxTargetDepth(this.mBgRoot.gameObject, false) + 1, 1, 2147483647);
			}
			if (winType == WindowType.Normal)
			{
				num = Mathf.Clamp(UIUtils.GetMaxTargetDepth(this.mNormalRoot.gameObject, false) + 1, 10, 2147483647);
			}
			else if (winType == WindowType.Fixed || winType == WindowType.SemiFixed)
			{
				num = Mathf.Clamp(UIUtils.GetMaxTargetDepth(this.mFixedRoot.gameObject, false) + 1, 100, 2147483647);
			}
			else if (winType == WindowType.Popup)
			{
				num = Mathf.Clamp(UIUtils.GetMaxTargetDepth(this.mPopupRoot.gameObject, false) + 1, 120, 2147483647);
			}
			else if (winType == WindowType.Hint)
			{
				num = Mathf.Clamp(UIUtils.GetMaxTargetDepth(this.mHintRoot.gameObject, false) + 1, 150, 2147483647);
			}
			else if (winType == WindowType.Highest)
			{
				num = Mathf.Clamp(UIUtils.GetMaxTargetDepth(this.mTopRoot.gameObject, false) + 1, 1500, 2147483647);
			}
			if (win.MinDepth != num)
			{
				UIUtils.SetTargetMinPanel(win.gameObj, num);
			}
			win.MinDepth = num;
		}

		private void RefreshReturnSeq(string winName, TUIWindow uiWin)
		{
			if (uiWin.IsRefreshReturnSeq)
			{
				bool flag = true;
				if (this.mCurNormalWin != null && this.mCurNormalWin.ShowCfg.ShowMode == WindowShowMode.UnneedReturn)
				{
					flag = false;
				}
				if (this.mDicOpenWindows.Count > 0 && flag)
				{
					List<string> list = new List<string>();
					List<string> list2 = new List<string>();
					List<TUIWindow> list3 = new List<TUIWindow>();
					ReturnWinSeqData returnWinSeqData = new ReturnWinSeqData();
					foreach (KeyValuePair<string, TUIWindow> current in this.mDicOpenWindows)
					{
						bool flag2 = true;
						if (uiWin.ShowCfg.ShowMode == WindowShowMode.NeedReturn || current.Value.DataCfg.WinType == WindowType.Fixed || current.Value.DataCfg.WinType == WindowType.Background || current.Value.DataCfg.WinType == WindowType.Hint || current.Value.DataCfg.WinType == WindowType.Highest)
						{
							flag2 = false;
						}
						if (flag2)
						{
							list.Add(current.Key);
						}
						if (current.Value.ShowCfg.IsAddToReturnSeq)
						{
							list3.Add(current.Value);
						}
					}
					for (int i = 0; i < list.Count; i++)
					{
						this.HideWindow(list[i]);
					}
					if (list3.Count > 0)
					{
						list3.Sort(new UIManager.CompareBaseWindow());
						for (int j = 0; j < list3.Count; j++)
						{
							string winName2 = list3[j].WinName;
							list2.Add(winName2);
						}
						returnWinSeqData.hideTargetWindow = uiWin;
						returnWinSeqData.lstReturnShowTargets = list2;
						this.mSepReturnWins.Push(returnWinSeqData);
					}
				}
			}
			else if (uiWin.ShowCfg.ShowMode == WindowShowMode.HideAll)
			{
				this.HideAllOpenWindow(true);
			}
		}

		private void ShowWindowForReturn(string winName)
		{
			this.mDicOpenWindows[winName] = this.mDicAllWindows[winName];
			this.mDicOpenWindows[winName].OpenWindow();
			if (this.OnOpened != null)
			{
				this.OnOpened(new OpenWindowEventArgs(true, winName, this.mDicOpenWindows[winName]));
			}
		}

		private void HideWindow(string winName)
		{
			TUIWindow exists;
			if (this.mDicOpenWindows.TryGetValue(winName, out exists) && exists)
			{
				if (this.OnClosed != null)
				{
					this.OnClosed(new CloseWindowEventArgs(true, winName, false, false, this.mDicOpenWindows[winName]));
				}
				this.mDicOpenWindows[winName].CloseWindow(false);
				this.mDicOpenWindows.Remove(winName);
			}
		}

		private void DoShowPreWindow()
		{
			if (this.mSepReturnWins.Count != 0)
			{
				ReturnWinSeqData returnWinSeqData = this.mSepReturnWins.Peek();
				if (returnWinSeqData != null)
				{
					string winName = returnWinSeqData.hideTargetWindow.WinName;
					if (returnWinSeqData.hideTargetWindow != null && this.mDicOpenWindows.ContainsKey(winName))
					{
						this.HideWindow(winName);
						if (returnWinSeqData.lstReturnShowTargets != null)
						{
							for (int i = 0; i < returnWinSeqData.lstReturnShowTargets.Count; i++)
							{
								string text = returnWinSeqData.lstReturnShowTargets[i];
								TUIWindow tUIWindow = this.mDicAllWindows[text];
								this.ShowWindowForReturn(text);
								if (tUIWindow.DataCfg.WinType == WindowType.Normal)
								{
									this.mPreNormalWin = this.mCurNormalWin;
									this.mCurNormalWin = this.mDicAllWindows[returnWinSeqData.lstReturnShowTargets[i]];
								}
							}
						}
						this.mSepReturnWins.Pop();
					}
				}
				return;
			}
			if (this.mCurNormalWin == null)
			{
				return;
			}
		}

		private void ClearReturnSeq()
		{
			if (this.mSepReturnWins != null)
			{
				this.mSepReturnWins.Clear();
			}
		}
	}
}
