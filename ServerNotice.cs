using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ServerNotice : MonoBehaviour
{
	private const float SPEED = 200f;

	private const float DELAY_TIME = 1.5f;

	public GameObject noticeRoot;

	public UILabel content;

	public TweenPosition tween;

	private CoroutineManager cMgr = new CoroutineManager();

	private CoroutineManager wordSlideMgr = new CoroutineManager();

	private Color32[] col = new Color32[]
	{
		Color.white,
		new Color32(2, 182, 207, 255)
	};

	private float panelSizeX = 810f;

	private float panelSoftnessX = 4f;

	private readonly float startX = -717f;

	private int staticWidth = 800;

	private void Awake()
	{
		if (this.noticeRoot == null && this.content == null && this.tween == null)
		{
			ClientLogger.Error("ServerNotice 指定组件为空。");
		}
	}

	private void OnDestroy()
	{
		this.cMgr.StopAllCoroutine();
		this.CallByNoticeVisible(false);
		this.wordSlideMgr.StopAllCoroutine();
	}

	private void OnEnable()
	{
		if (this.noticeRoot.activeInHierarchy)
		{
			this.noticeRoot.SetActive(false);
		}
		this.cMgr.StartCoroutine(this.TryGetMsg(), true);
	}

	private void OnDisable()
	{
		this.cMgr.StopAllCoroutine();
		this.CallByNoticeVisible(false);
		this.wordSlideMgr.StopAllCoroutine();
	}

	[DebuggerHidden]
	private IEnumerator TryGetMsg()
	{
		ServerNotice.<TryGetMsg>c__Iterator14D <TryGetMsg>c__Iterator14D = new ServerNotice.<TryGetMsg>c__Iterator14D();
		<TryGetMsg>c__Iterator14D.<>f__this = this;
		return <TryGetMsg>c__Iterator14D;
	}

	private void onGetMsg(string dataStr)
	{
		this.CallByNoticeVisible(true);
		this.wordSlideMgr.StopAllCoroutine();
		this.noticeRoot.SetActive(true);
		this.wordSlideMgr.StartCoroutine(this.Timer(8), true);
		this.content.text = dataStr;
		this.content.transform.localPosition = new Vector3(this.startX, 0f, 0f);
		this.content.color = this.col[0];
		if (this.content.width > this.staticWidth)
		{
			this.tween.from = this.tween.transform.localPosition;
			this.tween.to = new Vector3(this.startX - (float)this.content.width + this.panelSizeX - 50f, 0f, 0f);
			this.tween.duration = (float)this.content.width / 200f;
			this.wordSlideMgr.StartCoroutine(this.TweenerForOtherNotice(), true);
		}
	}

	private void onGetMsg(NotificationData data)
	{
		this.CallByNoticeVisible(true);
		this.wordSlideMgr.StopAllCoroutine();
		this.noticeRoot.SetActive(true);
		this.wordSlideMgr.StartCoroutine(this.Timer(data.timeVal), true);
		this.content.text = data.Content;
		this.content.transform.localPosition = new Vector3(this.startX, 0f, 0f);
		if (data.iparam <= this.col.Length - 1)
		{
			this.content.color = this.col[data.iparam];
		}
		else
		{
			this.content.color = this.col[0];
		}
		if (this.content.width > this.staticWidth)
		{
			this.tween.from = this.tween.transform.localPosition;
			this.tween.to = new Vector3(this.startX - (float)this.content.width + this.panelSizeX - 50f, 0f, 0f);
			this.tween.duration = (float)this.content.width / 200f;
			this.wordSlideMgr.StartCoroutine(this.Tweener(), true);
		}
	}

	private void CallByNoticeVisible(bool isShow)
	{
	}

	[DebuggerHidden]
	private IEnumerator Timer(int time)
	{
		ServerNotice.<Timer>c__Iterator14E <Timer>c__Iterator14E = new ServerNotice.<Timer>c__Iterator14E();
		<Timer>c__Iterator14E.time = time;
		<Timer>c__Iterator14E.<$>time = time;
		<Timer>c__Iterator14E.<>f__this = this;
		return <Timer>c__Iterator14E;
	}

	[DebuggerHidden]
	private IEnumerator Tweener()
	{
		ServerNotice.<Tweener>c__Iterator14F <Tweener>c__Iterator14F = new ServerNotice.<Tweener>c__Iterator14F();
		<Tweener>c__Iterator14F.<>f__this = this;
		return <Tweener>c__Iterator14F;
	}

	[DebuggerHidden]
	private IEnumerator TweenerForOtherNotice()
	{
		ServerNotice.<TweenerForOtherNotice>c__Iterator150 <TweenerForOtherNotice>c__Iterator = new ServerNotice.<TweenerForOtherNotice>c__Iterator150();
		<TweenerForOtherNotice>c__Iterator.<>f__this = this;
		return <TweenerForOtherNotice>c__Iterator;
	}
}
