using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class SelectHeroIcon : MonoBehaviour
{
	[SerializeField]
	private Renderer m_maskRenderer;

	[SerializeField]
	private UIWidget ThisItem;

	[SerializeField]
	private Transform m_Left;

	[SerializeField]
	private Transform m_Right;

	[SerializeField]
	private Transform m_BottomRight;

	[SerializeField]
	private Transform m_BlackBG;

	[SerializeField]
	private Transform m_Center;

	[SerializeField]
	private Transform m_Frame;

	private AnimationCurve m_ScaleCurve;

	private AnimationCurve m_AphaCurve;

	private AnimationCurve m_TopToBottom;

	private AnimationCurve m_CenterScale;

	private AnimationCurve m_MaskAlphCurve;

	private AnimationCurve m_MaskMove;

	private AnimationCurve m_ItemMove;

	private float m_fCoolDownEffectTotalTime = 1f;

	private float m_YAxisMoveTime = 0.1f;

	private float m_fMinValue;

	private float m_fMaxValue;

	private bool m_bIsInitCurves;

	private float positionMoveTimes = 7f;

	private float scaleChangeTimes = 5f;

	private bool Lock;

	private Vector3? recordVect3;

	private GameObject SelectHeroEffect;

	private GameObject recordEffect;

	public bool AnimationLock
	{
		get
		{
			return this.Lock;
		}
		set
		{
			this.Lock = value;
		}
	}

	private void CreateCurve_BigDiagonal()
	{
		Keyframe[] array = new Keyframe[]
		{
			new Keyframe(0f, this.m_fMinValue),
			new Keyframe(0.26f, this.m_fMaxValue)
		};
	}

	private void CreateCurve_SmallDiagonal()
	{
		Keyframe[] array = new Keyframe[]
		{
			new Keyframe(0f, this.m_fMinValue),
			new Keyframe(0.3f, this.m_fMinValue),
			new Keyframe(0.5f, this.m_fMaxValue),
			new Keyframe(0.500001f, this.m_fMinValue),
			new Keyframe(0.8f, this.m_fMinValue),
			new Keyframe(1f, this.m_fMaxValue)
		};
	}

	private void CreateCurve_BigLight()
	{
		Keyframe[] array = new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(0.2f, 0.79f),
			new Keyframe(0.26f, 0f)
		};
	}

	private void CreateCurve_SmallLight()
	{
		Keyframe[] array = new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(0.3f, 0f),
			new Keyframe(0.4f, 0.79f),
			new Keyframe(0.5f, 0f),
			new Keyframe(0.8f, 0f),
			new Keyframe(0.9f, 0.79f),
			new Keyframe(1f, 0f)
		};
	}

	private void CreateCurve_BaseLight()
	{
		Keyframe[] array = new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(0.2f, 0f),
			new Keyframe(0.3f, 0.88f),
			new Keyframe(0.43f, 0f),
			new Keyframe(0.63f, 0f),
			new Keyframe(0.73f, 0.88f),
			new Keyframe(0.83f, 0f)
		};
	}

	private void CreateCurve_Scale()
	{
		this.m_ScaleCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(0.2f, 0.9f),
			new Keyframe(0.85f, 1.25f),
			new Keyframe(1f, 1.2f)
		});
	}

	private void CreateCurve_Alph()
	{
		this.m_AphaCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		});
	}

	private void CreatCurve_MaskAlph()
	{
		this.m_MaskAlphCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(0.03f, 0.9f),
			new Keyframe(0.06f, 0.7f),
			new Keyframe(0.09f, 0.5f),
			new Keyframe(0.12f, 0.3f),
			new Keyframe(0.15f, 1f)
		});
	}

	private void CreatCurve_MaskMove()
	{
		this.m_MaskMove = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(0.09f, 0f),
			new Keyframe(0.12f, 0.2f),
			new Keyframe(0.4f, 1f)
		});
	}

	private void CreatCurve_ItemMove()
	{
		this.m_ItemMove = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0.2f),
			new Keyframe(0.03f, 0.45f),
			new Keyframe(0.06f, 0.7f),
			new Keyframe(0.09f, 0.9f),
			new Keyframe(0.15f, 1f)
		});
	}

	private void CreatCurve_CenterScale()
	{
		this.m_CenterScale = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(0.03f, 1.2f),
			new Keyframe(0.06f, 1.2f),
			new Keyframe(0.09f, 1.1f),
			new Keyframe(0.12f, 1f)
		});
	}

	private void Start()
	{
		this.m_fCoolDownEffectTotalTime = 1f;
		this.m_fMinValue = -0.5f;
		this.m_fMaxValue = 0.5f;
		this.InitCurves();
	}

	private void InitCurves()
	{
		if (!this.m_bIsInitCurves)
		{
			this.SelectHeroEffect = (Resources.Load("Prefab/Effects/UIEffect/Fx_uiboard_hero") as GameObject);
			this.CreateCurve_BaseLight();
			this.CreateCurve_BigDiagonal();
			this.CreateCurve_BigLight();
			this.CreateCurve_SmallDiagonal();
			this.CreateCurve_SmallLight();
			this.CreateCurve_Scale();
			this.CreateCurve_Alph();
			this.CreatCurve_MaskAlph();
			this.CreatCurve_MaskMove();
			this.CreatCurve_ItemMove();
			this.CreatCurve_CenterScale();
			this.m_bIsInitCurves = true;
		}
	}

	private void Awake()
	{
		this.InitCurves();
	}

	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	public void ShowCardUpOrDownAnimation(float position_y)
	{
	}

	public void ShowCardAnimation(Vector3 position)
	{
		this.InitCurves();
		base.StartCoroutine(this.ShowSelectHeroEffect());
		base.StartCoroutine(this.ShowCoolDownEffect());
		base.StartCoroutine(this.ShowCardPosition(position));
		base.StartCoroutine(this.ShowAnchor());
		base.StartCoroutine(this.ShowCenterScale());
	}

	public void ShowCardScale(bool type)
	{
		this.InitCurves();
		base.StartCoroutine(this.SetCardScale(type));
	}

	private void ShowEffect(bool isShow)
	{
		if (isShow)
		{
			if (this.recordEffect == null)
			{
				this.recordEffect = NGUITools.AddChild(base.gameObject, this.SelectHeroEffect);
			}
			this.recordEffect.transform.localPosition = new Vector3(30f, -5f, 0f);
			Animator[] componentsInChildren = base.GetComponentsInChildren<Animator>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].speed = 1f;
				componentsInChildren[i].Play(0, 0, 0f);
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator ShowCoolDownEffect()
	{
		SelectHeroIcon.<ShowCoolDownEffect>c__Iterator172 <ShowCoolDownEffect>c__Iterator = new SelectHeroIcon.<ShowCoolDownEffect>c__Iterator172();
		<ShowCoolDownEffect>c__Iterator.<>f__this = this;
		return <ShowCoolDownEffect>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator ShowCardPosition(Vector3 position)
	{
		SelectHeroIcon.<ShowCardPosition>c__Iterator173 <ShowCardPosition>c__Iterator = new SelectHeroIcon.<ShowCardPosition>c__Iterator173();
		<ShowCardPosition>c__Iterator.position = position;
		<ShowCardPosition>c__Iterator.<$>position = position;
		<ShowCardPosition>c__Iterator.<>f__this = this;
		return <ShowCardPosition>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator ShowCenterScale()
	{
		SelectHeroIcon.<ShowCenterScale>c__Iterator174 <ShowCenterScale>c__Iterator = new SelectHeroIcon.<ShowCenterScale>c__Iterator174();
		<ShowCenterScale>c__Iterator.<>f__this = this;
		return <ShowCenterScale>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator ShowSelectHeroEffect()
	{
		SelectHeroIcon.<ShowSelectHeroEffect>c__Iterator175 <ShowSelectHeroEffect>c__Iterator = new SelectHeroIcon.<ShowSelectHeroEffect>c__Iterator175();
		<ShowSelectHeroEffect>c__Iterator.<>f__this = this;
		return <ShowSelectHeroEffect>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator ShowAnchor()
	{
		SelectHeroIcon.<ShowAnchor>c__Iterator176 <ShowAnchor>c__Iterator = new SelectHeroIcon.<ShowAnchor>c__Iterator176();
		<ShowAnchor>c__Iterator.<>f__this = this;
		return <ShowAnchor>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator SetCardScale(bool type)
	{
		SelectHeroIcon.<SetCardScale>c__Iterator177 <SetCardScale>c__Iterator = new SelectHeroIcon.<SetCardScale>c__Iterator177();
		<SetCardScale>c__Iterator.type = type;
		<SetCardScale>c__Iterator.<$>type = type;
		<SetCardScale>c__Iterator.<>f__this = this;
		return <SetCardScale>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator SetY_AxisPosition(float position_y)
	{
		SelectHeroIcon.<SetY_AxisPosition>c__Iterator178 <SetY_AxisPosition>c__Iterator = new SelectHeroIcon.<SetY_AxisPosition>c__Iterator178();
		<SetY_AxisPosition>c__Iterator.position_y = position_y;
		<SetY_AxisPosition>c__Iterator.<$>position_y = position_y;
		<SetY_AxisPosition>c__Iterator.<>f__this = this;
		return <SetY_AxisPosition>c__Iterator;
	}

	private void SetCooldownMask(float time)
	{
		this.m_maskRenderer.material.SetFloat("amt1", this.m_MaskAlphCurve.Evaluate(time));
		this.m_maskRenderer.material.SetFloat("amt2", this.m_MaskMove.Evaluate(time));
	}

	private void SetItemPosition(float time, Vector3 position)
	{
		float num = this.m_ItemMove.Evaluate(time);
		base.transform.localPosition = new Vector3(position.x - (1f - num) * (float)this.ThisItem.width, position.y, position.z);
	}

	public void CancelChoise()
	{
		this.m_Left.transform.localScale = Vector3.one;
		this.m_Right.transform.localScale = Vector3.one;
		for (int i = 0; i < this.m_BottomRight.childCount; i++)
		{
			this.m_BottomRight.GetChild(i).transform.localScale = Vector3.one;
		}
		this.m_maskRenderer.material.SetFloat("amt1", 0f);
		this.m_maskRenderer.material.SetFloat("amt2", 0f);
		Vector3? vector = this.recordVect3;
		if (vector.HasValue)
		{
			base.transform.localPosition = new Vector3(this.recordVect3.Value.x, base.transform.localPosition.y, this.recordVect3.Value.z);
		}
		this.m_Center.transform.localScale = Vector3.one;
		this.m_Frame.transform.localScale = Vector3.one;
	}
}
