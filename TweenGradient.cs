using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(UILabel))]
public class TweenGradient : MonoBehaviour
{
	[Range(0.05f, 10f), SerializeField]
	private float duration = 0.1f;

	[SerializeField]
	private Color[] gradientTopColArr = new Color[]
	{
		Color.white,
		Color.white
	};

	[SerializeField]
	private Color[] gradientBottomColArr = new Color[]
	{
		Color.white,
		Color.white
	};

	private UILabel theLabel;

	private Camera theUiCamera;

	private byte topColArrCounter;

	private byte bottomColArrCounter;

	private Color topColorLerpA;

	private Color topColorLerpB;

	private float topColorRefreshTime;

	private Color bottomColorLerpA;

	private Color bottomColorLerpB;

	private float bottomColorRefreshTime;

	private bool visibleFlag = true;

	private CoroutineManager cMgr = new CoroutineManager();

	private float UPDATE_DURATION = 0.03f;

	public float Duration
	{
		get
		{
			return this.duration;
		}
		set
		{
			this.duration = ((value < 0.05f || value > 10f) ? 0.05f : value);
		}
	}

	public Color[] GradientTopColorsArray
	{
		set
		{
			if (TweenGradient.IsColorArrayIllegal(value))
			{
				return;
			}
			if (TweenGradient.IsColorArrayIllegal(this.gradientBottomColArr))
			{
				return;
			}
			this.cMgr.StopAllCoroutine();
			this.ResetFields();
			this.gradientTopColArr = value;
		}
	}

	public Color[] GradientBottomColorsArray
	{
		set
		{
			if (TweenGradient.IsColorArrayIllegal(this.gradientTopColArr))
			{
				return;
			}
			if (TweenGradient.IsColorArrayIllegal(value))
			{
				return;
			}
			this.cMgr.StopAllCoroutine();
			this.ResetFields();
			this.gradientBottomColArr = value;
		}
	}

	private void OnAwake()
	{
		bool flag = GlobalSettings.Instance.QualitySetting.OldLevel == 4 || GlobalSettings.Instance.QualitySetting.OldLevel == 3 || GlobalSettings.Instance.QualitySetting.OldLevel == 2;
		this.UPDATE_DURATION = ((!flag) ? 0.04f : 0.025f);
	}

	private void OnEnable()
	{
		if (TweenGradient.IsColorArrayIllegal(this.gradientTopColArr))
		{
			return;
		}
		if (TweenGradient.IsColorArrayIllegal(this.gradientBottomColArr))
		{
			return;
		}
		this.theLabel = base.gameObject.GetComponent<UILabel>();
		this.theUiCamera = UICamera.currentCamera;
		this.ResetFields();
		base.StartCoroutine(this.UpdateVisibleState());
		this.ActivateTweenCoroutines();
	}

	private void OnDisable()
	{
		this.theLabel = null;
		this.cMgr.StopAllCoroutine();
		base.StopAllCoroutines();
	}

	private void ResetFields()
	{
		this.topColorRefreshTime = 0f;
		this.bottomColorRefreshTime = 0f;
		this.topColorLerpA = this.gradientTopColArr[0];
		this.topColorLerpB = this.gradientTopColArr[1];
		this.theLabel.gradientTop = this.gradientTopColArr[0];
		this.bottomColorLerpA = this.gradientBottomColArr[0];
		this.bottomColorLerpB = this.gradientBottomColArr[1];
		this.theLabel.gradientBottom = this.gradientBottomColArr[0];
		this.topColArrCounter = 1;
		this.bottomColArrCounter = 1;
	}

	private static bool IsColorArrayIllegal(Color[] _arrColors)
	{
		return _arrColors == null || _arrColors.Length <= 1;
	}

	private static Bounds GetWidgetBoundsInWorldPosition(UIWidget _theWidget)
	{
		Vector3[] worldCorners = _theWidget.worldCorners;
		Bounds result = new Bounds(worldCorners[0], Vector3.zero);
		for (int i = 1; i < 4; i++)
		{
			result.Encapsulate(worldCorners[i]);
		}
		return result;
	}

	private void CheckObjectVisible()
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(this.theUiCamera);
		bool flag = GeometryUtility.TestPlanesAABB(planes, TweenGradient.GetWidgetBoundsInWorldPosition(this.theLabel));
		if (this.visibleFlag == flag)
		{
			return;
		}
		this.visibleFlag = flag;
		if (this.visibleFlag)
		{
			this.cMgr.ResumeAllCoroutine();
		}
		else
		{
			this.cMgr.PauseAllCoroutine();
		}
	}

	[DebuggerHidden]
	private IEnumerator UpdateGradient()
	{
		TweenGradient.<UpdateGradient>c__IteratorA5 <UpdateGradient>c__IteratorA = new TweenGradient.<UpdateGradient>c__IteratorA5();
		<UpdateGradient>c__IteratorA.<>f__this = this;
		return <UpdateGradient>c__IteratorA;
	}

	[DebuggerHidden]
	private IEnumerator UpdateVisibleState()
	{
		TweenGradient.<UpdateVisibleState>c__IteratorA6 <UpdateVisibleState>c__IteratorA = new TweenGradient.<UpdateVisibleState>c__IteratorA6();
		<UpdateVisibleState>c__IteratorA.<>f__this = this;
		return <UpdateVisibleState>c__IteratorA;
	}

	[DebuggerHidden]
	private IEnumerator UpdateTopCounter()
	{
		TweenGradient.<UpdateTopCounter>c__IteratorA7 <UpdateTopCounter>c__IteratorA = new TweenGradient.<UpdateTopCounter>c__IteratorA7();
		<UpdateTopCounter>c__IteratorA.<>f__this = this;
		return <UpdateTopCounter>c__IteratorA;
	}

	[DebuggerHidden]
	private IEnumerator UpdateBottomCounter()
	{
		TweenGradient.<UpdateBottomCounter>c__IteratorA8 <UpdateBottomCounter>c__IteratorA = new TweenGradient.<UpdateBottomCounter>c__IteratorA8();
		<UpdateBottomCounter>c__IteratorA.<>f__this = this;
		return <UpdateBottomCounter>c__IteratorA;
	}

	[ContextMenu("Activate!")]
	public void ActivateTweenCoroutines()
	{
		if (!this.theLabel.applyGradient)
		{
			this.theLabel.applyGradient = true;
		}
		this.cMgr.StartCoroutine(this.UpdateGradient(), true);
		this.cMgr.StartCoroutine(this.UpdateTopCounter(), true);
		this.cMgr.StartCoroutine(this.UpdateBottomCounter(), true);
	}

	private void SetStyle(Color[] top, Color[] bottom, float dur)
	{
		this.duration = dur;
		if (Application.isPlaying)
		{
			if (this.theLabel == null)
			{
				this.theLabel = base.gameObject.GetComponent<UILabel>();
			}
			this.theLabel.applyGradient = false;
			this.GradientTopColorsArray = top;
			this.GradientBottomColorsArray = bottom;
		}
		else
		{
			this.gradientTopColArr = top;
			this.gradientBottomColArr = bottom;
		}
	}

	public void SetStyle(AllochroicPreset preset)
	{
		this.SetStyle(preset.topColors, preset.bottomColors, preset.duration);
	}

	[ContextMenu("Rainbow Style")]
	public void SetRainbowStyle()
	{
		BaseLabelStylePreset gradientPreset = LabelStylePresetFactory.GetGradientPreset(ELabelStyle.Season1_Rainbow);
		if (gradientPreset is AllochroicPreset)
		{
			this.SetStyle(gradientPreset as AllochroicPreset);
		}
	}
}
