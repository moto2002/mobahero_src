using System;
using UnityEngine;

public class RenderQueueModifier : MonoBehaviour
{
	public enum RenderType
	{
		FRONT,
		BACK
	}

	public UIWidget m_target;

	public RenderQueueModifier.RenderType m_type;

	private Renderer[] _renderers;

	private int _lastQueue;

	private void Start()
	{
		this._renderers = base.GetComponentsInChildren<Renderer>();
	}

	private void FixedUpdate()
	{
		if (this.m_target == null || this.m_target.drawCall == null)
		{
			return;
		}
		int num = this.m_target.drawCall.renderQueue;
		num += ((this.m_type != RenderQueueModifier.RenderType.FRONT) ? -1 : 1);
		if (this._lastQueue != num)
		{
			this._lastQueue = num;
			Renderer[] renderers = this._renderers;
			for (int i = 0; i < renderers.Length; i++)
			{
				Renderer renderer = renderers[i];
				renderer.material.renderQueue = this._lastQueue;
			}
		}
	}

	public void SetTargetRenderType(UIWidget widget, RenderQueueModifier.RenderType type)
	{
		this.m_target = widget;
		this.m_type = type;
	}
}
