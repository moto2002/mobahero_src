using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CameraShake
{
	private Task m_shakeTask;

	private Transform m_shakeNode;

	private AnimationCurve m_animationCurveForShake;

	private float m_fDurationShake;

	public CameraShake(Transform shakeNode)
	{
		this.m_shakeNode = shakeNode;
		this.m_animationCurveForShake = new AnimationCurve();
	}

	public void Shake(float shakeStrength, int shakeCount, float duration, float dampingValue)
	{
		this.m_animationCurveForShake = this.CreateCurve_Shake(shakeStrength, shakeCount, duration, dampingValue);
		this.m_fDurationShake = duration;
		if (this.m_shakeTask != null)
		{
			this.m_shakeTask.Stop();
		}
		this.m_shakeTask = new Task(this.Shake_Coroutine(), true);
	}

	private AnimationCurve CreateCurve_Shake(float shakeStrength, int shakeCount, float duration, float dampingValue)
	{
		Keyframe[] array = new Keyframe[shakeCount];
		for (int i = 0; i < shakeCount; i++)
		{
			float time = duration / (float)shakeCount * (float)i;
			shakeStrength *= dampingValue;
			if (i == 0 || i == shakeCount - 1)
			{
				array[i] = new Keyframe(time, 0f);
			}
			else
			{
				shakeStrength = 0f - shakeStrength;
				array[i] = new Keyframe(time, shakeStrength);
			}
		}
		return new AnimationCurve(array);
	}

	[DebuggerHidden]
	private IEnumerator Shake_Coroutine()
	{
		CameraShake.<Shake_Coroutine>c__Iterator1A <Shake_Coroutine>c__Iterator1A = new CameraShake.<Shake_Coroutine>c__Iterator1A();
		<Shake_Coroutine>c__Iterator1A.<>f__this = this;
		return <Shake_Coroutine>c__Iterator1A;
	}
}
