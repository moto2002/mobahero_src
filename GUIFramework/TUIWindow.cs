using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace GUIFramework
{
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

		public void Show(EventDelegate.Callback OnComplete)
		{
			this.gameObj.SetActive(true);
			TweenScale tweenScale = TweenScale.Begin(this.gameObj, 0.25f, Vector3.one);
			tweenScale.from = Vector3.zero;
			tweenScale.SetOnFinished(OnComplete);
		}

		public void Hide(EventDelegate.Callback OnComplete)
		{
			TweenScale tweenScale = TweenScale.Begin(this.gameObj, 0.25f, Vector3.zero);
			tweenScale.SetOnFinished(OnComplete);
		}

		public void OpenWindow()
		{
			if (this.coroutine != null)
			{
				base.StopCoroutine(this.coroutine);
			}
			this.gameObj.SetActive(true);
		}

		public void CloseWindow(bool isDestroy)
		{
			if (this.DataCfg.IsDelayClose && base.gameObject != null && base.gameObject.active)
			{
				this.coroutine = base.StartCoroutine(this.DoCloseWindow(isDestroy));
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
