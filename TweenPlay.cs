using AnimationOrTween;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Play"), ExecuteInEditMode]
public class TweenPlay : MonoBehaviour, IPlay
{
	public GameObject tweenTarget;

	public Direction playDirection = Direction.Forward;

	public bool includeChildren;

	public List<UITweener> mTweens = new List<UITweener>();

	private int index;

	private int count;

	public float delay;

	public List<EventDelegate> onFinished = new List<EventDelegate>();

	public UITweener.Style style
	{
		get;
		set;
	}

	public List<EventDelegate> OnEnd
	{
		get
		{
			return this.onFinished;
		}
	}

	public void Begin()
	{
		this.Play(this.playDirection);
	}

	private void Awake()
	{
		this.UpdateTween();
		this.ResetToBeginning();
	}

	private void Reset()
	{
		this.tweenTarget = base.gameObject;
	}

	public void Play(Direction playDirection)
	{
		this.playDirection = playDirection;
		this.UpdateTween();
		this.ResetToBeginning();
		this.Play();
	}

	public void PlayForward()
	{
		this.Play(Direction.Forward);
	}

	public void PlayReverse()
	{
		this.Play(Direction.Reverse);
	}

	private void Play()
	{
		if (this.mTweens == null || this.mTweens.Count == 0)
		{
			return;
		}
		if (this.playDirection == Direction.Toggle)
		{
			this.Toggle();
		}
		else
		{
			List<UITweener> tweenerByGroup = this.GetTweenerByGroup(this.mTweens[this.index].tweenGroup);
			this.count = tweenerByGroup.Count;
			if (this.playDirection == Direction.Forward)
			{
				this.index += this.count - 1;
			}
			else if (this.playDirection == Direction.Reverse)
			{
				this.index -= this.count - 1;
			}
			foreach (UITweener current in tweenerByGroup)
			{
				this.Play(current, this.playDirection);
			}
		}
	}

	public void Toggle()
	{
		List<UITweener> tweenerByGroup = this.GetTweenerByGroup(this.mTweens[this.index].tweenGroup);
		if (tweenerByGroup != null)
		{
			foreach (UITweener current in tweenerByGroup)
			{
				current.enabled = !current.enabled;
			}
		}
	}

	public void Pause()
	{
		List<UITweener> tweenerByGroup = this.GetTweenerByGroup(this.mTweens[this.index].tweenGroup);
		if (tweenerByGroup != null)
		{
			foreach (UITweener current in tweenerByGroup)
			{
				current.enabled = false;
			}
		}
	}

	public void Resume()
	{
		List<UITweener> tweenerByGroup = this.GetTweenerByGroup(this.mTweens[this.index].tweenGroup);
		if (tweenerByGroup != null)
		{
			foreach (UITweener current in tweenerByGroup)
			{
				current.enabled = true;
			}
		}
		this.Play();
	}

	private void Play(UITweener tw, Direction playDirection)
	{
		if (playDirection == Direction.Forward)
		{
			tw.Replay(true);
		}
		else
		{
			tw.Replay(false);
		}
	}

	public void UpdateTween()
	{
		GameObject gameObject = (!(this.tweenTarget == null)) ? this.tweenTarget : base.gameObject;
		this.mTweens.Clear();
		UITweener[] array = (!this.includeChildren) ? gameObject.GetComponents<UITweener>() : gameObject.GetComponentsInChildren<UITweener>();
		UITweener[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			UITweener uITweener = array2[i];
			if (uITweener.tweenGroup >= 0)
			{
				this.mTweens.Add(uITweener);
			}
		}
		this.mTweens.Sort(new Comparison<UITweener>(this.CompareByGropFunc));
		int j = 0;
		int num = this.mTweens.Count;
		while (j < num)
		{
			this.InitTween(j, new EventDelegate.Callback(this.PlayNext));
			j++;
		}
	}

	public void SetStartToCurrentValue()
	{
		foreach (UITweener current in this.mTweens)
		{
			current.SetStartToCurrentValue();
		}
	}

	public void SetEndToCurrentValue()
	{
		foreach (UITweener current in this.mTweens)
		{
			current.SetEndToCurrentValue();
		}
	}

	public void ResetToBeginning()
	{
		if (this.mTweens == null || this.mTweens.Count == 0)
		{
			return;
		}
		if (this.index >= this.mTweens.Count)
		{
			this.index = this.mTweens.Count - 1;
		}
		else if (this.index < 0)
		{
			this.index = 0;
		}
		List<UITweener> tweenerByGroup = this.GetTweenerByGroup(this.mTweens[this.index].tweenGroup);
		if (tweenerByGroup != null)
		{
			foreach (UITweener current in tweenerByGroup)
			{
				current.ResetToBeginning();
			}
		}
		if (this.playDirection == Direction.Forward)
		{
			this.index = 0;
		}
		else if (this.playDirection == Direction.Reverse)
		{
			this.index = this.mTweens.Count - 1;
		}
	}

	private int CompareByGropFunc(UITweener left, UITweener right)
	{
		if (left.tweenGroup < right.tweenGroup)
		{
			return -1;
		}
		if (left.tweenGroup > right.tweenGroup)
		{
			return 1;
		}
		return 0;
	}

	public List<UITweener> GetTweenerByGroup(int group)
	{
		List<UITweener> list = new List<UITweener>();
		foreach (UITweener current in this.mTweens)
		{
			if (current.tweenGroup == group)
			{
				list.Add(current);
			}
		}
		return list;
	}

	private void InitTween(int index, EventDelegate.Callback callBack)
	{
		if (index < this.mTweens.Count && index >= 0)
		{
			UITweener uITweener = this.mTweens[index];
			uITweener.ResetToBeginning();
			uITweener.style = UITweener.Style.Once;
			uITweener.enabled = false;
			EventDelegate.Add(uITweener.onFinished, callBack, false);
		}
	}

	private void PlayNext()
	{
		this.count--;
		if (this.count == 0)
		{
			if (this.playDirection == Direction.Forward)
			{
				this.index++;
				if (this.index >= this.mTweens.Count)
				{
					if (this.style == UITweener.Style.Once)
					{
						this.index = this.mTweens.Count - 1;
						EventDelegate.Execute(this.onFinished);
						return;
					}
					if (this.style == UITweener.Style.Loop)
					{
						this.index = 0;
					}
					else
					{
						this.index = this.mTweens.Count - 1;
						this.playDirection = Direction.Reverse;
					}
				}
			}
			else if (this.playDirection == Direction.Reverse)
			{
				this.index--;
				if (this.index < 0)
				{
					if (this.style == UITweener.Style.Once)
					{
						this.index = 0;
						EventDelegate.Execute(this.onFinished);
						return;
					}
					if (this.style == UITweener.Style.Loop)
					{
						this.index = this.mTweens.Count - 1;
					}
					else
					{
						this.index = 0;
						this.playDirection = Direction.Forward;
					}
				}
			}
			this.Play();
		}
	}

	public void AddTweenEvent(UITweener tw, EventDelegate.Callback callBack)
	{
		if (this.mTweens.Contains(tw))
		{
			EventDelegate.Add(tw.onFinished, callBack);
		}
	}
}
