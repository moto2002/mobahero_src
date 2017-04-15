using Assets.Scripts.GUILogic.View.HomeChatView;
using Com.Game.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UITweenHelper : MonoBehaviour
{
	private enum TweenType
	{
		TweenScale,
		TweenAlpha
	}

	[Serializable]
	private class TweenData
	{
		[SerializeField]
		public UITweenHelper.TweenType type;

		[SerializeField]
		public Vector3 fromVector3;

		[SerializeField]
		public Vector3 toVector3;

		[Range(0f, 1f), SerializeField]
		public float fromFloat;

		[Range(0f, 1f), SerializeField]
		public float toFloat;

		[SerializeField]
		public UITweener.Style playStyle;

		[SerializeField]
		public AnimationCurve animationCurve;

		[SerializeField]
		public float duration;
	}

	[SerializeField]
	private UIWidget[] Components;

	[SerializeField]
	private UITweenHelper.TweenData[] AddTypes;

	private TweenAlpha[] tAlphaGroup;

	private TweenScale[] tScaleGroup;

	private bool isPlayed;

	private Dictionary<int, Action> OnTweenFinish;

	public float NextPlayDelay;

	private void Awake()
	{
		this.tAlphaGroup = new TweenAlpha[this.Components.Length];
		this.tScaleGroup = new TweenScale[this.Components.Length];
		this.OnTweenFinish = new Dictionary<int, Action>();
		for (int i = 0; i < this.Components.Length; i++)
		{
			for (int j = 0; j < this.AddTypes.Length; j++)
			{
				UITweenHelper.TweenType type = this.AddTypes[j].type;
				if (type != UITweenHelper.TweenType.TweenScale)
				{
					if (type == UITweenHelper.TweenType.TweenAlpha)
					{
						if (this.Components[i].GetComponent<TweenAlpha>() != null)
						{
							this.tAlphaGroup[i] = this.Components[i].GetComponent<TweenAlpha>();
						}
						else
						{
							this.tAlphaGroup[i] = this.Components[i].gameObject.AddComponent<TweenAlpha>();
						}
						this.tAlphaGroup[i].from = this.AddTypes[j].fromFloat;
						this.tAlphaGroup[i].to = this.AddTypes[j].toFloat;
						this.tAlphaGroup[i].style = this.AddTypes[j].playStyle;
						this.tAlphaGroup[i].animationCurve = this.AddTypes[j].animationCurve;
						this.tAlphaGroup[i].duration = this.AddTypes[j].duration;
					}
				}
				else
				{
					if (this.Components[i].GetComponent<TweenScale>() != null)
					{
						this.tScaleGroup[i] = this.Components[i].GetComponent<TweenScale>();
					}
					else
					{
						this.tScaleGroup[i] = this.Components[i].gameObject.AddComponent<TweenScale>();
					}
					this.tScaleGroup[i].from = this.AddTypes[j].fromVector3;
					this.tScaleGroup[i].to = this.AddTypes[j].toVector3;
					this.tScaleGroup[i].style = this.AddTypes[j].playStyle;
					this.tScaleGroup[i].animationCurve = this.AddTypes[j].animationCurve;
					this.tScaleGroup[i].duration = this.AddTypes[j].duration;
				}
			}
		}
		this.isPlayed = false;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Backslash))
		{
			if (null != Singleton<HomeChatview>.Instance.transform)
			{
				if (Singleton<HomeChatview>.Instance.gameObject.activeInHierarchy)
				{
					CtrlManager.CloseWindow(WindowID.HomeChatview);
				}
				else
				{
					CtrlManager.OpenWindow(WindowID.HomeChatview, null);
					MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewOpenView, ChitchatType.Hall, false);
					MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewFillChatHistory, false, false);
				}
			}
			else
			{
				CtrlManager.OpenWindow(WindowID.HomeChatview, null);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewOpenView, ChitchatType.Hall, false);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewFillChatHistory, true, false);
			}
		}
	}

	[ContextMenu("Reset")]
	public void Init()
	{
		if (this.tAlphaGroup == null || this.tAlphaGroup.Length == 0 || this.tScaleGroup == null || this.tScaleGroup.Length == 0)
		{
			this.Awake();
		}
	}

	[ContextMenu("Play")]
	public void Play()
	{
		this.Init();
		if (this.isPlayed)
		{
			for (int i = 0; i < this.Components.Length; i++)
			{
				if (this.tAlphaGroup[i] != null)
				{
					this.tAlphaGroup[i].ResetToBeginning();
				}
				if (this.tScaleGroup[i] != null)
				{
					this.tScaleGroup[i].ResetToBeginning();
				}
			}
			this.isPlayed = false;
		}
		for (int j = 0; j < this.Components.Length; j++)
		{
			if (this.tAlphaGroup[j] != null)
			{
				this.tAlphaGroup[j].delay = 0.05f * (float)j + this.NextPlayDelay;
				this.tAlphaGroup[j].PlayForward();
			}
			if (this.tScaleGroup[j] != null)
			{
				this.tScaleGroup[j].delay = 0.05f * (float)j + this.NextPlayDelay;
				this.tScaleGroup[j].PlayForward();
			}
			this.DoOnTweenFinish(j);
		}
		this.NextPlayDelay = 0f;
		this.isPlayed = true;
	}

	public void ExchangeWidget(int index, UIWidget comp)
	{
		if (index < this.Components.Length)
		{
			this.Components[index] = comp;
			this.Awake();
		}
	}

	private void DoOnTweenFinish(int index)
	{
		if (this.OnTweenFinish == null || !this.OnTweenFinish.ContainsKey(index))
		{
			return;
		}
		this.OnTweenFinish[index]();
		this.OnTweenFinish.Remove(index);
	}

	public void AddTweenFinishCallback(int index, Action callback)
	{
		if (index < 0 || index >= this.Components.Length)
		{
			return;
		}
		if (this.OnTweenFinish == null)
		{
			this.OnTweenFinish = new Dictionary<int, Action>();
		}
		if (this.OnTweenFinish.ContainsKey(index))
		{
			this.OnTweenFinish[index] = callback;
		}
		else
		{
			this.OnTweenFinish.Add(index, callback);
		}
	}
}
