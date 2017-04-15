using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Fill")]
public class TweenFill : UITweener
{
	[Range(0f, 1f)]
	public float from = 1f;

	[Range(0f, 1f)]
	public float to = 1f;

	private UISprite m_Sprite;

	private UILabel m_Label;

	public UISprite cachedSprite
	{
		get
		{
			if (this.m_Sprite == null)
			{
				this.m_Sprite = base.GetComponent<UISprite>();
				if (this.m_Sprite == null)
				{
					this.m_Sprite = base.GetComponentInChildren<UISprite>();
				}
			}
			return this.m_Sprite;
		}
	}

	public UILabel cachedLabel
	{
		get
		{
			if (this.m_Label == null)
			{
				this.m_Label = base.GetComponent<UILabel>();
				if (this.m_Label == null)
				{
					this.m_Label = base.GetComponentInChildren<UILabel>();
				}
			}
			return this.m_Label;
		}
	}

	[Obsolete("Use 'value' instead")]
	public float fill
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
		}
	}

	public float value
	{
		get
		{
			if (this.cachedSprite != null)
			{
				return this.cachedSprite.fillAmount;
			}
			return 0f;
		}
		set
		{
			if (this.cachedSprite != null)
			{
				this.cachedSprite.fillAmount = value;
			}
		}
	}

	public string value2
	{
		get
		{
			return this.cachedLabel.text;
		}
		set
		{
			this.cachedLabel.text = value;
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		if (this.isPause)
		{
			return;
		}
		this.value = Mathf.Lerp(this.from, this.to, factor);
		float num = this.value * this.duration;
		if (this.cachedLabel != null)
		{
			if (num >= 1f)
			{
				this.value2 = (this.value * this.duration).ToString("#0");
			}
			else
			{
				this.value2 = (this.value * this.duration).ToString("#0.0");
				if (this.value2 == "0.0")
				{
					this.value2 = string.Empty;
				}
			}
		}
	}

	public static TweenFill Begin(GameObject go, float duration, float fill)
	{
		TweenFill tweenFill = UITweener.Begin<TweenFill>(go, duration);
		tweenFill.from = tweenFill.value;
		tweenFill.to = fill;
		tweenFill.isPause = false;
		if (duration <= 0f)
		{
			tweenFill.Sample(1f, true);
			tweenFill.enabled = false;
		}
		return tweenFill;
	}

	public void Pause()
	{
		this.isPause = true;
	}

	public void Resume()
	{
		this.isPause = false;
	}

	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}
}
