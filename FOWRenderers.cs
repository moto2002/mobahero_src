using System;
using UnityEngine;

[AddComponentMenu("Fog of War/Renderers")]
public class FOWRenderers : MonoBehaviour
{
	private Transform mTrans;

	private Renderer[] mRenderers;

	private float mNextUpdate;

	private bool mIsVisible = true;

	private bool mUpdate = true;

	public bool isVisible
	{
		get
		{
			return this.mIsVisible;
		}
	}

	public void Rebuild()
	{
		this.mUpdate = true;
	}

	private void Awake()
	{
		this.mTrans = base.transform;
	}

	private void LateUpdate()
	{
		if (this.mNextUpdate < Time.time)
		{
			this.UpdateNow();
		}
	}

	private void UpdateNow()
	{
		this.mNextUpdate = Time.time + 0.075f + UnityEngine.Random.value * 0.05f;
		if (FOWSystem.instance == null)
		{
			base.enabled = false;
			return;
		}
		if (this.mUpdate)
		{
			this.mRenderers = base.GetComponentsInChildren<Renderer>();
		}
		bool flag = FOWSystem.instance.IsVisible(this.mTrans.position);
		if (this.mUpdate || this.mIsVisible != flag)
		{
			this.mUpdate = false;
			this.mIsVisible = flag;
			int i = 0;
			int num = this.mRenderers.Length;
			while (i < num)
			{
				Renderer renderer = this.mRenderers[i];
				if (renderer)
				{
					renderer.enabled = this.mIsVisible;
				}
				else
				{
					this.mUpdate = true;
					this.mNextUpdate = Time.time;
				}
				i++;
			}
		}
	}
}
