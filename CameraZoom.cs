using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CameraZoom
{
	private Transform m_zoomNode;

	private AnimationCurve m_animationCurveForZoom;

	private float m_fDurationZoom;

	private bool m_bIsZoom;

	private Task m_zoomTask;

	public CameraZoom(Transform zoomNode)
	{
		this.m_zoomNode = zoomNode;
		this.m_animationCurveForZoom = new AnimationCurve();
	}

	public void Zoom(float finalDistance, float duration, bool easeInOut = true)
	{
		this.CreateCurve_Zoom(finalDistance, duration, easeInOut);
		this.m_fDurationZoom = duration;
		if (this.m_zoomTask != null)
		{
			this.m_zoomTask.Stop();
			this.m_zoomTask = null;
		}
		this.m_zoomTask = new Task(this.Zoom_Coroutine(), true);
		this.m_bIsZoom = true;
	}

	private void CreateCurve_Zoom(float finalDistance, float duration, bool easeInOut)
	{
		if (easeInOut)
		{
			this.m_animationCurveForZoom = AnimationCurve.EaseInOut(0f, this.m_zoomNode.localPosition.z, duration, finalDistance);
		}
		else
		{
			this.m_animationCurveForZoom = AnimationCurve.Linear(0f, this.m_zoomNode.localPosition.z, duration, finalDistance);
		}
	}

	[DebuggerHidden]
	private IEnumerator Zoom_Coroutine()
	{
		CameraZoom.<Zoom_Coroutine>c__Iterator1B <Zoom_Coroutine>c__Iterator1B = new CameraZoom.<Zoom_Coroutine>c__Iterator1B();
		<Zoom_Coroutine>c__Iterator1B.<>f__this = this;
		return <Zoom_Coroutine>c__Iterator1B;
	}
}
