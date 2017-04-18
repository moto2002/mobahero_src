using Com.Game.Utils;
using GUIFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Com.Game.Module
{
    /// <summary>
    /// 控制管理器
    /// </summary>
	public static class CtrlManager
	{
        /// <summary>
        /// 所有窗口视图控制器列表
        /// </summary>
		private static Dictionary<WindowID, IView> mDicWindCtrls;
        /// <summary>
        /// 所有打开窗口视图控制器列表
        /// </summary>
		private static Dictionary<WindowID, IView> mDicOpenCtrls;
        /// <summary>
        /// 初始化窗口控制管理器
        /// </summary>
		public static void Init()
		{
			UIManager.Instance.OnOpened += new UIManager.OpenWindowEventHandler(CtrlManager.OnOpenWindow);
			UIManager.Instance.OnClosed += new UIManager.CloseWindowEventHandler(CtrlManager.OnCloseWindow);
			CtrlManager.mDicWindCtrls = new Dictionary<WindowID, IView>();
			CtrlManager.mDicOpenCtrls = new Dictionary<WindowID, IView>();
		}
        /// <summary>
        /// 清理窗口控制管理器
        /// </summary>
		public static void Clear()
		{
			UIManager.Instance.OnOpened -= new UIManager.OpenWindowEventHandler(CtrlManager.OnOpenWindow);
			UIManager.Instance.OnClosed -= new UIManager.CloseWindowEventHandler(CtrlManager.OnCloseWindow);
			CtrlManager.mDicWindCtrls.Clear();
			CtrlManager.mDicOpenCtrls.Clear();
		}

        /// <summary>
        /// 根据窗口ID获取指定窗口的视图控制器,如果没有，返回一个默认的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="winId"></param>
        /// <returns></returns>
		public static T GetCtrl<T>(WindowID winId) where T : IView
		{
			return (!CtrlManager.mDicWindCtrls.ContainsKey(winId)) ? default(T) : ((T)((object)CtrlManager.mDicWindCtrls[winId]));
		}
        /// <summary>
        /// 根据窗口ID获取指定窗口的视图控制器，如果没有，就返回null
        /// </summary>
        /// <param name="winId"></param>
        /// <returns></returns>
		public static IView GetCtrl(WindowID winId)
		{
			IView arg_23_0;
			if (CtrlManager.mDicWindCtrls.ContainsKey(winId))
			{
				IView view = CtrlManager.mDicWindCtrls[winId];
				arg_23_0 = view;
			}
			else
			{
				arg_23_0 = null;
			}
			return arg_23_0;
		}

        /// <summary>
        /// 添加指定窗口ID的视图控制器到控制管理器中
        /// </summary>
        /// <param name="winId"></param>
        /// <param name="view"></param>
		public static void AddCtrl(WindowID winId, IView view)
		{
			view.WinId = winId;
			CtrlManager.mDicWindCtrls[winId] = view;
		}

        /// <summary>
        /// 从控制器管理器中移除指定窗口ID对应的视图控制器
        /// </summary>
        /// <param name="winId"></param>
		public static void RemoveCtrl(WindowID winId)
		{
			if (CtrlManager.mDicWindCtrls.ContainsKey(winId))
			{
				CtrlManager.mDicWindCtrls.Remove(winId);
			}
			if (CtrlManager.mDicOpenCtrls.ContainsKey(winId))
			{
				CtrlManager.mDicOpenCtrls.Remove(winId);
			}
		}
        /// <summary>
        /// 返回前一个窗口
        /// </summary>
		public static void ReturnPreWindow()
		{
			UIManager.Instance.ShowPreWindow();
		}
        /// <summary>
        /// 预加载指定窗口
        /// </summary>
        /// <param name="winId"></param>
		public static void PreloadWindow(WindowID winId)
		{
			IView viewInstance = CtrlManager.GetViewInstance(winId);
			viewInstance.WinId = winId;
			UIManager.Instance.PreloadWindow(viewInstance.WinId.ToString(), viewInstance.WinResCfg);
		}
        /// <summary>
        /// 打开指定窗口，使用指定视图控制器
        /// </summary>
        /// <param name="winId"></param>
        /// <param name="ctrl"></param>
		public static void OpenWindow(WindowID winId, IView ctrl = null)
		{
			try
			{
				if (CtrlManager.mDicOpenCtrls.ContainsKey(winId))
				{
					if (winId == WindowID.TipView)
					{
						CtrlManager.OnOpenWindow(new OpenWindowEventArgs(true, winId.ToString(), CtrlManager.mDicOpenCtrls[winId].uiWindow));
					}
				}
				else
				{
					IView view = ctrl;
					if (view == null)
					{
						view = CtrlManager.GetViewInstance(winId);
					}
					view.WinId = winId;
					CtrlManager.mDicWindCtrls[winId] = view;
					UIManager.Instance.OpenWindow(view.WinId.ToString(), view.WinResCfg);
				}
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
			}
		}
        /// <summary>
        /// 关闭指定窗口
        /// </summary>
        /// <param name="winId"></param>
		public static void CloseWindow(WindowID winId)
		{
			if (!CtrlManager.mDicOpenCtrls.ContainsKey(winId))
			{
				return;
			}
			UIManager.Instance.CloseWindow(winId.ToString());
		}
        /// <summary>
        /// 判断指定窗口是否已打开
        /// </summary>
        /// <param name="winId"></param>
        /// <returns></returns>
		public static bool IsWindowOpen(WindowID winId)
		{
			return CtrlManager.mDicOpenCtrls != null && CtrlManager.mDicOpenCtrls.ContainsKey(winId);
		}
        /// <summary>
        /// 销毁所有窗口
        /// </summary>
		public static void DestroyAllWindows()
		{
			UIManager.Instance.DestroyAllWindows();
			CtrlManager.mDicWindCtrls.Clear();
			CtrlManager.mDicOpenCtrls.Clear();
		}
        /// <summary>
        /// 除排除列表中的窗口外所有窗口销毁(包括界面控制器)
        /// </summary>
        /// <param name="winIdsIgnored"></param>
		public static void DestroyAllWindowsExcept(params WindowID[] winIdsIgnored)
		{
			string[] winIdsIgnored2 = (from x in winIdsIgnored
			select x.ToString()).ToArray<string>();
			UIManager.Instance.DestroyAllWindowsExcept(winIdsIgnored2);
			HashSet<WindowID> hashSet = new HashSet<WindowID>(CtrlManager.mDicWindCtrls.Keys);
			hashSet.ExceptWith(winIdsIgnored);
			foreach (WindowID current in hashSet)
			{
				CtrlManager.mDicWindCtrls.Remove(current);
			}
			hashSet = new HashSet<WindowID>(CtrlManager.mDicOpenCtrls.Keys);
			hashSet.ExceptWith(winIdsIgnored);
			foreach (WindowID current2 in hashSet)
			{
				CtrlManager.mDicOpenCtrls.Remove(current2);
			}
		}
        /// <summary>
        /// 获取指定窗口id的窗口视图控制器实例
        /// </summary>
        /// <param name="winId"></param>
        /// <returns></returns>
		private static IView GetViewInstance(WindowID winId)
		{
			PropertyInfo property = UICtrlDefine.mDicCtrlType[winId].GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			return property.GetGetMethod().Invoke(null, null) as IView;
		}
        /// <summary>
        /// 打开窗口事件回调处理
        /// </summary>
        /// <param name="args"></param>
		private static void OnOpenWindow(OpenWindowEventArgs args)
		{
			WindowID key = (WindowID)((int)Enum.Parse(typeof(WindowID), args.WinName));
			if (args.IsSuccess)  //成功打开
			{
				IView view = CtrlManager.mDicWindCtrls[key];
				TUIWindow uiWindow = args.UiWindow;
				if (uiWindow.DataCfg.WinType == WindowType.Normal && CtrlManager.mDicOpenCtrls.ContainsKey(WindowID.MenuTopBarView))
				{
					if (uiWindow.DataCfg.IsOnset)
					{
						((MenuTopBarView)CtrlManager.mDicOpenCtrls[WindowID.MenuTopBarView]).ShowHUDView(true);
						((MenuTopBarView)CtrlManager.mDicOpenCtrls[WindowID.MenuTopBarView]).ShowBcakBtn(false, view.WindowTitle);
					}
					else if (!uiWindow.DataCfg.IsDelayShowBar)
					{
						((MenuTopBarView)CtrlManager.mDicOpenCtrls[WindowID.MenuTopBarView]).ShowBcakBtn(true, view.WindowTitle);
					}
					else
					{
						((MenuTopBarView)CtrlManager.mDicOpenCtrls[WindowID.MenuTopBarView]).DelayShowBcakBtn(true, view.WindowTitle);
					}
				}
				if (view.uiWindow == null)
				{
					view.uiWindow = uiWindow;
					view.Init();
				}
				view.IsOpened = true;
				view.RegisterUpdateHandler();
				view.HandleAfterOpenView();
				CtrlManager.mDicOpenCtrls[key] = view;
			}
			else //失败，就移除
			{
				CtrlManager.mDicWindCtrls.Remove(key);
			}
		}

		private static void OnCloseWindow(CloseWindowEventArgs args)
		{
			if (args.IsSuccess)
			{
				WindowID key = (WindowID)((int)Enum.Parse(typeof(WindowID), args.WinName));
				if (CtrlManager.mDicWindCtrls.ContainsKey(key))
				{
					IView view = CtrlManager.mDicWindCtrls[key];
					view.HandleBeforeCloseView();
					view.IsOpened = false;
					view.CancelUpdateHandler();
					if (args.IsRestart)
					{
						view.OnRestart();
					}
					if (args.IsDestroy)
					{
						view.Destroy();
						CtrlManager.mDicWindCtrls.Remove(key);
					}
					CtrlManager.mDicOpenCtrls.Remove(key);
				}
			}
			else
			{
				WindowID key2 = (WindowID)((int)Enum.Parse(typeof(WindowID), args.WinName));
				if (CtrlManager.mDicWindCtrls.ContainsKey(key2))
				{
					CtrlManager.mDicWindCtrls.Remove(key2);
				}
			}
		}

		public static void ShowMsgBox(string title, string content, Action<bool> callback, PopViewType type = PopViewType.PopTwoButton, string ok = "确定", string cancel = "取消", PopViewTask task = null)
		{
			MobaMessageManagerTools.SendClientMsg(ClientC2V.PopView_enqueue, new PopViewParam(title, content, callback, type, ok, cancel, task), false);
		}

		public static void ShowMsgBox(string title, string content, Action callback, PopViewType type = PopViewType.PopOneButton, string ok = "确定", string cancel = "取消", PopViewTask task = null)
		{
			MobaMessageManagerTools.SendClientMsg(ClientC2V.PopView_enqueue, new PopViewParam(title, content, callback, type, ok, cancel, task), false);
		}
	}
}
