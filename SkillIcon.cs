using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class SkillIcon : MonoBehaviour
{
	public delegate void BoolDelegate(GameObject go, bool state);

	public const float TIME = 2.5f;

	[SerializeField]
	private Transform m_iconTrans;

	[SerializeField]
	private Renderer m_maskRenderer;

	private AnimationCurve m_pressCurve;

	private AnimationCurve m_bigDiagonalCurve;

	private AnimationCurve m_smallDiagonalCurve;

	private AnimationCurve m_baseLightCurve;

	private AnimationCurve m_bigLightCurve;

	private AnimationCurve m_smallLightCurve;

	private float m_fPressCurveTotalTime;

	private float m_fCoolDownEffectTotalTime;

	private float m_fMinValue;

	private float m_fMaxValue;

	private bool m_bCanPress;

	private bool m_bIsPress;

	private bool m_bIsInitCurves;

	private bool Lock;

	private float LongthTime;

	private bool press;

	public SkillIcon.BoolDelegate LongthPress;

	private CoroutineManager coroutineManager = new CoroutineManager();

	private Task task;

	public void SetLock(bool type)
	{
		this.Lock = type;
	}

	public new bool GetType()
	{
		return this.Lock;
	}

	private void Awake()
	{
		this.LongthTime = 2.5f;
	}

	private void Start()
	{
		this.m_fPressCurveTotalTime = 0f;
		this.m_fCoolDownEffectTotalTime = 1f;
		this.m_fMinValue = -0.5f;
		this.m_fMaxValue = 0.5f;
		this.InitCurves();
	}

	private void OnDisable()
	{
		base.StopCoroutine(this.CooldownEffect());
		base.StopAllCoroutines();
		this.SetCooldownMask(0f);
		this.m_iconTrans.localScale = Vector3.one;
	}

	private void InitCurves()
	{
		if (!this.m_bIsInitCurves)
		{
			this.CreateCurve_BaseLight();
			this.CreateCurve_BigDiagonal();
			this.CreateCurve_BigLight();
			this.CreateCurve_SmallDiagonal();
			this.CreateCurve_SmallLight();
			this.m_bIsInitCurves = true;
		}
	}

	private void CreateCurve_PressDown()
	{
		this.m_fPressCurveTotalTime = 0.1f;
		this.m_pressCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(0.033f, 0.96f),
			new Keyframe(0.1f, 0.75f)
		});
	}

	private void CreateCurve_PressUp()
	{
		this.m_fPressCurveTotalTime = 0.333f;
		this.m_pressCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, this.m_iconTrans.localScale.x),
			new Keyframe(0.067f, 1.38f),
			new Keyframe(0.1f, 1.34f),
			new Keyframe(0.2f, 1.05f),
			new Keyframe(0.333f, 1f)
		});
	}

	private void CreateCurve_BigDiagonal()
	{
		this.m_bigDiagonalCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, this.m_fMinValue),
			new Keyframe(0.26f, this.m_fMaxValue)
		});
	}

	private void CreateCurve_SmallDiagonal()
	{
		this.m_smallDiagonalCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, this.m_fMinValue),
			new Keyframe(0.3f, this.m_fMinValue),
			new Keyframe(0.5f, this.m_fMaxValue),
			new Keyframe(0.500001f, this.m_fMinValue),
			new Keyframe(0.8f, this.m_fMinValue),
			new Keyframe(1f, this.m_fMaxValue)
		});
	}

	private void CreateCurve_BigLight()
	{
		this.m_bigLightCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(0.2f, 0.79f),
			new Keyframe(0.26f, 0f)
		});
	}

	private void CreateCurve_SmallLight()
	{
		this.m_smallLightCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(0.3f, 0f),
			new Keyframe(0.4f, 0.79f),
			new Keyframe(0.5f, 0f),
			new Keyframe(0.8f, 0f),
			new Keyframe(0.9f, 0.79f),
			new Keyframe(1f, 0f)
		});
	}

	private void CreateCurve_BaseLight()
	{
		this.m_baseLightCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(0.2f, 0f),
			new Keyframe(0.3f, 0.88f),
			new Keyframe(0.43f, 0f),
			new Keyframe(0.63f, 0f),
			new Keyframe(0.73f, 0.88f),
			new Keyframe(0.83f, 0f)
		});
	}

	private void OnPress(bool isPress)
	{
	}

	private void Update()
	{
	}

	[DebuggerHidden]
	private IEnumerator PressEffect()
	{
		SkillIcon.<PressEffect>c__IteratorE8 <PressEffect>c__IteratorE = new SkillIcon.<PressEffect>c__IteratorE8();
		<PressEffect>c__IteratorE.<>f__this = this;
		return <PressEffect>c__IteratorE;
	}

	[DebuggerHidden]
	private IEnumerator CooldownEffect()
	{
		SkillIcon.<CooldownEffect>c__IteratorE9 <CooldownEffect>c__IteratorE = new SkillIcon.<CooldownEffect>c__IteratorE9();
		<CooldownEffect>c__IteratorE.<>f__this = this;
		return <CooldownEffect>c__IteratorE;
	}

	private void SetCooldownMask(float time)
	{
		if (this.m_maskRenderer == null || this.m_maskRenderer.material == null || this.m_bigDiagonalCurve == null)
		{
			return;
		}
		this.m_maskRenderer.material.SetTextureOffset("_DiagonalLineBig", Vector2.one * this.m_bigDiagonalCurve.Evaluate(time));
		this.m_maskRenderer.material.SetTextureOffset("_DiagonalLineSmall", Vector2.one * this.m_smallDiagonalCurve.Evaluate(time));
		this.m_maskRenderer.material.SetFloat("_ValueMain", this.m_baseLightCurve.Evaluate(time));
		this.m_maskRenderer.material.SetFloat("_ValueBig", this.m_bigLightCurve.Evaluate(time));
		this.m_maskRenderer.material.SetFloat("_ValueSmall", this.m_smallLightCurve.Evaluate(time));
	}

	public void SetCanPress(bool canPress, bool needShow = true)
	{
		if (this.m_bCanPress != canPress)
		{
			this.m_bIsPress = canPress;
			this.m_bCanPress = canPress;
			if (this.m_bCanPress && this.m_bIsInitCurves && needShow)
			{
				base.StopCoroutine(this.CooldownEffect());
				this.SetCooldownMask(0f);
				if (base.gameObject.activeInHierarchy)
				{
					base.StartCoroutine(this.CooldownEffect());
				}
			}
		}
	}

	public void Init()
	{
		base.StopAllCoroutines();
		if (this.m_bIsInitCurves)
		{
			this.SetCooldownMask(0f);
		}
		this.m_iconTrans.localScale = Vector3.one;
	}
}
