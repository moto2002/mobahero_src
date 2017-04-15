using System;
using UnityEngine;

public class UIAnimationCurves : MonoBehaviour
{
	public static UIAnimationCurves AnimationCurves;

	public AnimationCurve offsetCurve_y = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.2f, 120f),
		new Keyframe(0.3f, 140f),
		new Keyframe(0.8f, 70f)
	});

	public AnimationCurve offsetCurve_x = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.8f, 130f)
	});

	public AnimationCurve alphaCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.05f, 1f),
		new Keyframe(0.3f, 1f),
		new Keyframe(0.5f, 0.6f),
		new Keyframe(0.7f, 0f),
		new Keyframe(0.8f, 0f)
	});

	public AnimationCurve scaleCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.05f, 2f),
		new Keyframe(0.15f, 1.5f),
		new Keyframe(0.3f, 1.2f),
		new Keyframe(0.5f, 0.8f),
		new Keyframe(0.8f, 0.4f)
	});

	private void Awake()
	{
		UIAnimationCurves.AnimationCurves = this;
	}
}
