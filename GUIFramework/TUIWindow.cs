using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace GUIFramework
{
    /// <summary>
    /// 窗口UI绑定脚本组件
    /// </summary>
	public class TUIWindow : MonoBehaviour
	{
		public WinDataCfg DataCfg;

		public WinShowCfg ShowCfg;

		private TweenScale twScale;

		private int minDepth = 1;

		private Coroutine coroutine;

		public Transform trans
		{
			get;
			private set;
		}

		public GameObject gameObj
		{
			get;
			private set;
		}

		public bool IsLocked
		{
			get;
			set;
		}

		public string WinName
		{
			get;
			set;
		}

		public int MinDepth
		{
			get
			{
				return this.minDepth;
			}
			set
			{
				this.minDepth = value;
			}
		}
        /// <summary>
        /// 退出UI时是否需要刷新
        /// </summary>
		public bool IsRefreshReturnSeq
		{
			get
			{
				return this.ShowCfg.ShowMode == WindowShowMode.NeedReturn || this.ShowCfg.ShowMode == WindowShowMode.HideOther;
			}
		}

		private void Awake()
		{
			this.trans = base.transform;
			this.gameObj = base.gameObject;
			if (!this.gameObj.GetComponent<TweenScale>())
			{
				this.twScale = this.gameObj.AddComponent<TweenScale>();
			}
		}
        /// <summary>
        /// 窗口显示
        /// </summary>
        /// <param name="OnComplete"></param>
		public void Show(EventDelegate.Callback OnComplete)
		{
			this.gameObj.SetActive(true);
			TweenScale tweenScale = TweenScale.Begin(this.gameObj, 0.25f, Vector3.one);
			tweenScale.from = Vector3.zero;
			tweenScale.SetOnFinished(OnComplete);
		}

        /// <summary>
        /// 窗口隐藏
        /// </summary>
        /// <param name="OnComplete"></param>
		public void Hide(EventDelegate.Callback OnComplete)
		{
			TweenScale tweenScale = TweenScale.Begin(this.gameObj, 0.25f, Vector3.zero);
			tweenScale.SetOnFinished(OnComplete);
		}
        /// <summary>
        /// 打开窗口
        /// </summary>
		public void OpenWindow()
		{
			if (this.coroutine != null)
			{
				base.StopCoroutine(this.coroutine);
			}
			this.gameObj.SetActive(true);
		}
        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="isDestroy">是否销毁</param>
		public void CloseWindow(bool isDestroy)
		{
			if (this.DataCfg.IsDelayClose && base.gameObject != null && base.gameObject.active)
			{
				this.coroutine = base.StartCoroutine(this.DoCloseWindow(isDestroy));//延迟关闭
			}
			else if (isDestroy)
			{
				NGUITools.Destroy(base.gameObject);
			}
			else
			{
				this.gameObj.SetActive(false);
			}
		}

		[DebuggerHidden]
		private IEnumerator DoCloseWindow(bool isDestroy)
		{
			TUIWindow.<DoCloseWindow>c__IteratorA1 <DoCloseWindow>c__IteratorA = new TUIWindow.<DoCloseWindow>c__IteratorA1();
			<DoCloseWindow>c__IteratorA.isDestroy = isDestroy;
			<DoCloseWindow>c__IteratorA.<$>isDestroy = isDestroy;
			<DoCloseWindow>c__IteratorA.<>f__this = this;
			return <DoCloseWindow>c__IteratorA;
		}
	}
}
