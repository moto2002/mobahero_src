using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class AnimateHpBar : MonoBehaviour
{
	[SerializeField]
	private UISlider HpSlider;

	[SerializeField]
	private UISprite OffSprite;

	[SerializeField]
	private float _threshold = 0.5f;

	[SerializeField]
	private float _targetValue;

	private bool _isSliding;

	private float _originWidth;

	private float AnimateValue
	{
		get
		{
			return (float)this.OffSprite.width / this._originWidth;
		}
		set
		{
			this.OffSprite.width = (int)(this._originWidth * value);
		}
	}

	public float value
	{
		get
		{
			return this.HpSlider.value;
		}
		set
		{
			float value2 = this.HpSlider.value;
			this.HpSlider.value = value;
			if (Mathf.Abs(value - value2) < 0.04f)
			{
				if (!this._isSliding)
				{
					this.AnimateValue = value;
				}
				return;
			}
			if (value > this.AnimateValue)
			{
				this.AnimateValue = value;
				this.StopAnimation();
				return;
			}
			this.StartAnimation();
		}
	}

	private void Awake()
	{
		this._originWidth = (float)this.OffSprite.width;
		this.OffSprite.width = 0;
	}

	private void StopAnimation()
	{
		if (this._isSliding)
		{
			base.StopCoroutine("AnimateHpBar_Coroutine");
			this._isSliding = false;
		}
	}

	private void StartAnimation()
	{
		if (!this._isSliding)
		{
			base.StartCoroutine("AnimateHpBar_Coroutine");
		}
	}

	[DebuggerHidden]
	private IEnumerator AnimateHpBar_Coroutine()
	{
		AnimateHpBar.<AnimateHpBar_Coroutine>c__IteratorC4 <AnimateHpBar_Coroutine>c__IteratorC = new AnimateHpBar.<AnimateHpBar_Coroutine>c__IteratorC4();
		<AnimateHpBar_Coroutine>c__IteratorC.<>f__this = this;
		return <AnimateHpBar_Coroutine>c__IteratorC;
	}
}
