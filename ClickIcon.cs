using MobaHeros;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ClickIcon : MonoBehaviour
{
	public AnimationCurve curveLine;

	public AnimationCurve curveCircleChange = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1.6f),
		new Keyframe(0.05f, 1.7f),
		new Keyframe(0.15f, 0.8f),
		new Keyframe(0.2f, 1f)
	});

	public AnimationCurve curveAlphaChange = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.2f, 1f)
	});

	public ClickIconType prefabName = ClickIconType.TargetIcon;

	public int rotateSpeed = 180;

	public float rotateSpeedFactor = 1f;

	public float scaleChangeTime = 0.4f;

	public float arrowLiveTimes = 0.5f;

	public float voidCircleActiveTime = 0.2f;

	public GamePhaseState gamePhaseState = GamePhaseState.Phase1;

	public static ResourceHandle iconHandle1;

	public static ResourceHandle iconHandle2;

	private SpriteRenderer solidTex;

	private SpriteRenderer voidTex;

	private AnimationCurve ReturnCurveLine()
	{
		Keyframe[] array = new Keyframe[3];
		array[0] = new Keyframe(0f, 1.6f);
		array[1] = new Keyframe(0.11f, 0.8f);
		array[1].outTangent = 20f;
		array[2] = new Keyframe(0.16f, 1f);
		return new AnimationCurve(array);
	}

	private void Awake()
	{
		Transform transform = base.transform.Find("SolidCircle");
		if (transform != null)
		{
			this.solidTex = transform.GetComponent<SpriteRenderer>();
		}
		Transform transform2 = base.transform.Find("VoidCircle");
		if (transform2 != null)
		{
			this.voidTex = transform2.GetComponent<SpriteRenderer>();
		}
	}

	public static void Clear()
	{
		if (ClickIcon.iconHandle1 != null)
		{
			ClickIcon.iconHandle1.Release();
			ClickIcon.iconHandle1 = null;
		}
		if (ClickIcon.iconHandle2 != null)
		{
			ClickIcon.iconHandle2.Release();
			ClickIcon.iconHandle2 = null;
		}
	}

	public static ClickIcon Get(GameObject obj = null)
	{
		ClickIcon component = obj.GetComponent<ClickIcon>();
		if (component == null)
		{
			obj.AddComponent<ClickIcon>();
		}
		return obj.GetComponent<ClickIcon>();
	}

	private void OnDisable()
	{
		base.gameObject.transform.localScale = Vector3.one;
		base.StopAllCoroutines();
	}

	public void Inactive(bool isActive)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(isActive);
		}
		NGUITools.SetActiveChildren(base.gameObject, isActive);
	}

	public void Init(ClickIconType type = ClickIconType.MouseClick)
	{
		this.curveLine = this.ReturnCurveLine();
		this.prefabName = type;
		base.StopAllCoroutines();
		this.Inactive(true);
		switch (this.prefabName)
		{
		case ClickIconType.TargetIcon:
			if (base.gameObject.activeInHierarchy)
			{
				base.StartCoroutine("TargetIconCreate");
			}
			if (this.solidTex != null)
			{
				this.solidTex.enabled = true;
			}
			if (this.voidTex != null)
			{
				this.voidTex.enabled = true;
			}
			break;
		case ClickIconType.MainPlayerIcon:
			if (base.gameObject.activeInHierarchy)
			{
				base.StartCoroutine("MainPlayerIconCreate");
			}
			break;
		case ClickIconType.MouseClick:
			if (base.gameObject.activeInHierarchy)
			{
				Transform transform = base.transform.Find("VoidCircle");
				Transform transform2 = base.transform.Find("fourarrow 2");
				transform.transform.localScale = Vector3.one * 0.052f;
				transform.gameObject.SetActive(true);
				transform2.gameObject.SetActive(true);
				Animator component = base.transform.GetComponent<Animator>();
				component.Play("Anim_MouseClick", -1, 0f);
				component.Update(0f);
				component = transform2.GetComponent<Animator>();
				component.Play("Take 001", -1, 0f);
				component.Update(0f);
				base.StartCoroutine("MouseClickCreate");
			}
			break;
		}
	}

	public void Clean()
	{
		base.StopAllCoroutines();
		ClickIconType clickIconType = this.prefabName;
		if (clickIconType != ClickIconType.TargetIcon)
		{
			if (clickIconType == ClickIconType.MainPlayerIcon)
			{
				if (base.gameObject.activeInHierarchy)
				{
					base.StartCoroutine("MainPlayerIconClean");
				}
			}
		}
		else if (base.gameObject.activeInHierarchy)
		{
			base.StartCoroutine("TargetIconClean");
		}
	}

	[DebuggerHidden]
	private IEnumerator MainPlayerIconCreate()
	{
		ClickIcon.<MainPlayerIconCreate>c__Iterator2E <MainPlayerIconCreate>c__Iterator2E = new ClickIcon.<MainPlayerIconCreate>c__Iterator2E();
		<MainPlayerIconCreate>c__Iterator2E.<>f__this = this;
		return <MainPlayerIconCreate>c__Iterator2E;
	}

	[DebuggerHidden]
	private IEnumerator MainPlayerIconClean()
	{
		ClickIcon.<MainPlayerIconClean>c__Iterator2F <MainPlayerIconClean>c__Iterator2F = new ClickIcon.<MainPlayerIconClean>c__Iterator2F();
		<MainPlayerIconClean>c__Iterator2F.<>f__this = this;
		return <MainPlayerIconClean>c__Iterator2F;
	}

	[DebuggerHidden]
	private IEnumerator TargetIconCreate()
	{
		ClickIcon.<TargetIconCreate>c__Iterator30 <TargetIconCreate>c__Iterator = new ClickIcon.<TargetIconCreate>c__Iterator30();
		<TargetIconCreate>c__Iterator.<>f__this = this;
		return <TargetIconCreate>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator TargetIconClean()
	{
		ClickIcon.<TargetIconClean>c__Iterator31 <TargetIconClean>c__Iterator = new ClickIcon.<TargetIconClean>c__Iterator31();
		<TargetIconClean>c__Iterator.<>f__this = this;
		return <TargetIconClean>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator MouseClickCreate()
	{
		ClickIcon.<MouseClickCreate>c__Iterator32 <MouseClickCreate>c__Iterator = new ClickIcon.<MouseClickCreate>c__Iterator32();
		<MouseClickCreate>c__Iterator.<>f__this = this;
		return <MouseClickCreate>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator MouseClickClean()
	{
		ClickIcon.<MouseClickClean>c__Iterator33 <MouseClickClean>c__Iterator = new ClickIcon.<MouseClickClean>c__Iterator33();
		<MouseClickClean>c__Iterator.<>f__this = this;
		return <MouseClickClean>c__Iterator;
	}

	public int ComputeSpeed(GamePhaseState rect)
	{
		if (rect == this.gamePhaseState)
		{
			return this.rotateSpeed;
		}
		switch (rect)
		{
		case GamePhaseState.None:
			this.rotateSpeed = 180;
			this.gamePhaseState = GamePhaseState.None;
			break;
		case GamePhaseState.Phase1:
			this.rotateSpeed = 180;
			this.gamePhaseState = GamePhaseState.Phase1;
			break;
		case GamePhaseState.Phase2:
			this.rotateSpeed = 360;
			this.gamePhaseState = GamePhaseState.Phase2;
			break;
		case GamePhaseState.Phase3:
			this.rotateSpeed = 480;
			this.gamePhaseState = GamePhaseState.Phase3;
			break;
		default:
			this.rotateSpeed = 180;
			this.gamePhaseState = GamePhaseState.Phase1;
			break;
		}
		return this.rotateSpeed;
	}
}
