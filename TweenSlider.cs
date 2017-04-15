using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Slider")]
public class TweenSlider : UITweener
{
	[Range(0f, 1f)]
	public float from = 1f;

	[Range(0f, 1f)]
	public float to = 1f;

	private UISlider m_Slider;

	public UISlider cachedSlider
	{
		get
		{
			if (this.m_Slider == null)
			{
				this.m_Slider = base.GetComponent<UISlider>();
				if (this.m_Slider == null)
				{
					this.m_Slider = base.GetComponentInChildren<UISlider>();
				}
			}
			return this.m_Slider;
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
			if (this.cachedSlider != null)
			{
				return this.cachedSlider.value;
			}
			return 0f;
		}
		set
		{
			if (this.cachedSlider != null)
			{
				this.cachedSlider.value = value;
			}
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = Mathf.Lerp(this.from, this.to, factor);
	}

	public static TweenSlider Begin(GameObject go, float duration, float fill)
	{
		TweenSlider tweenSlider = UITweener.Begin<TweenSlider>(go, duration);
		tweenSlider.from = tweenSlider.value;
		tweenSlider.to = fill;
		if (duration <= 0f)
		{
			tweenSlider.Sample(1f, true);
			tweenSlider.enabled = false;
		}
		return tweenSlider;
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
