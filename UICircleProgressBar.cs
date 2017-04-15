using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class UICircleProgressBar : MonoBehaviour
{
	public Color Color1From = new Color(1f, 1f, 1f, 1f);

	public Color Color1 = new Color(1f, 1f, 1f, 1f);

	[SerializeField]
	private UISprite Circle1;

	public Color ColorBg = new Color(1f, 1f, 1f, 1f);

	[SerializeField]
	private UISprite Circle1Bg;

	public Color Color2 = new Color(1f, 1f, 1f, 1f);

	[SerializeField]
	private UISprite Circle2;

	[SerializeField]
	private UISprite Circle2Light;

	public Color Color2Light = new Color(1f, 1f, 1f, 1f);

	[SerializeField]
	private float mDuration1;

	[SerializeField]
	private float mDuration2;

	public int animFPS = 30;

	public List<Callback> onLevelUp = new List<Callback>();

	private CoroutineManager cMgr = new CoroutineManager();

	private float value1;

	private float value2;

	private float valueSum;

	private bool levelUpFlag;

	public float value
	{
		get
		{
			return this.Circle1.fillAmount + this.Circle2.fillAmount;
		}
	}

	public float duration1
	{
		get
		{
			return this.mDuration1;
		}
		set
		{
			this.mDuration1 = value;
		}
	}

	public float duration2
	{
		get
		{
			return this.mDuration2;
		}
		set
		{
			this.mDuration2 = value;
		}
	}

	private void Awake()
	{
	}

	private void OnDestory()
	{
		this.CoroutineClear();
	}

	public void CoroutineClear()
	{
		this.cMgr.StopAllCoroutine();
	}

	public Vector3 GetHeadPoint()
	{
		float x = 0f;
		float y = 0f;
		if (this.Circle1 != null && this.Circle2 != null)
		{
			float f = this.value * 2f * 3.14159274f;
			float num = 0.957894742f * (float)(this.Circle1.width / 2);
			x = num * Mathf.Sin(f);
			y = num * Mathf.Cos(f);
		}
		return new Vector3(x, y, 0f);
	}

	public void Play(bool isCircle1 = true)
	{
		if (isCircle1)
		{
			this.cMgr.StartCoroutine(this.Tween1(1f, 0f), true);
		}
		else
		{
			this.cMgr.StartCoroutine(this.Tween1(0f, 1f), true);
		}
	}

	public void Play(float v1 = -1f, float v2 = -1f, float vs = -1f)
	{
		if (v1 != -1f)
		{
			this.value1 = v1;
		}
		if (v2 != -1f)
		{
			this.value2 = v2;
		}
		if (vs != -1f)
		{
			this.valueSum = vs;
		}
		if (this.valueSum == 0f)
		{
			return;
		}
		float num = this.value1 / this.valueSum;
		float num2;
		if (this.value2 >= this.valueSum - this.value1)
		{
			num2 = 1f - num;
			this.levelUpFlag = true;
			this.value2 = this.value2 + this.value1 - this.valueSum;
			this.value1 = 0f;
		}
		else
		{
			num2 = this.value2 / this.valueSum;
			this.levelUpFlag = false;
		}
		this.cMgr.StopAllCoroutine();
		if (this.Circle1 != null && this.Circle2 != null && this.Circle1Bg != null)
		{
			this.Circle1.color = this.Color1From;
			this.Circle1Bg.color = this.ColorBg;
			this.Circle2.color = this.Color2;
			if (this.Circle2Light != null)
			{
				this.Circle2Light.color = this.Color2Light;
			}
			if (num > 0.002f)
			{
				this.cMgr.StartCoroutine(this.Tween1(num, num2), true);
			}
			else if (num2 > 0.002f)
			{
				this.cMgr.StartCoroutine(this.Tween2(num2), true);
			}
		}
	}

	private void LevelUp()
	{
		this.cMgr.StopAllCoroutine();
		if (this.onLevelUp != null && this.onLevelUp.Count != 0)
		{
			foreach (Callback current in this.onLevelUp)
			{
				current();
			}
		}
		this.Circle2.transform.localRotation = Quaternion.Euler(Vector3.zero);
		if (this.Circle2Light != null)
		{
			this.Circle2Light.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 2f));
			this.Circle2Light.fillAmount = 0f;
		}
		this.Circle1.fillAmount = 0f;
		this.Circle1Bg.fillAmount = 0f;
		this.Circle2.fillAmount = 0f;
	}

	private float DecayFactor1(float x)
	{
		if (x == 0f || x == 1f)
		{
			return x;
		}
		float num = x * x;
		return 0.8732f * num * num - (0.8381f * x + 1.7431f) * num + 2.7125f * x;
	}

	[DebuggerHidden]
	private IEnumerator Tween1(float start, float move)
	{
		UICircleProgressBar.<Tween1>c__Iterator110 <Tween1>c__Iterator = new UICircleProgressBar.<Tween1>c__Iterator110();
		<Tween1>c__Iterator.start = start;
		<Tween1>c__Iterator.move = move;
		<Tween1>c__Iterator.<$>start = start;
		<Tween1>c__Iterator.<$>move = move;
		<Tween1>c__Iterator.<>f__this = this;
		return <Tween1>c__Iterator;
	}

	private float DecayFactor2(float x)
	{
		if (x == 0f || x == 1f)
		{
			return x;
		}
		float num = x * x;
		float num2 = num * x;
		float num3 = num2 * x;
		return -0.7391f * num3 + 3.1792f * num2 - 5.1248f * num + 3.6857f * x;
	}

	[DebuggerHidden]
	private IEnumerator Tween2(float move)
	{
		UICircleProgressBar.<Tween2>c__Iterator111 <Tween2>c__Iterator = new UICircleProgressBar.<Tween2>c__Iterator111();
		<Tween2>c__Iterator.move = move;
		<Tween2>c__Iterator.<$>move = move;
		<Tween2>c__Iterator.<>f__this = this;
		return <Tween2>c__Iterator;
	}
}
