using System;
using UnityEngine;

public class UIEffectSort : MonoBehaviour
{
	public UIPanel panel;

	public UIWidget widgetInBack;

	[NonSerialized]
	private Renderer[] mRenderers;

	private void Awake()
	{
		this.mRenderers = base.gameObject.GetComponentsInChildren<Renderer>();
	}

	public void InitPanel(UIPanel p)
	{
		this.panel = p;
		if (this.panel != null)
		{
			this.panel.AddUISort(this);
		}
	}

	private void Start()
	{
		if (this.panel != null)
		{
			this.panel.AddUISort(this);
		}
	}

	public void UpdateSortUI()
	{
		if (this.widgetInBack != null && this.widgetInBack.drawCall != null && this.mRenderers != null)
		{
			int num = this.widgetInBack.drawCall.renderQueue + 1;
			for (int i = 0; i < this.mRenderers.Length; i++)
			{
				Material[] materials = this.mRenderers[i].materials;
				for (int j = 0; j < materials.Length; j++)
				{
					Material material = materials[j];
					if (material.renderQueue != num)
					{
						material.renderQueue = num;
					}
				}
			}
		}
	}

	private void OnDestroy()
	{
		if (this.panel != null)
		{
			this.panel.RemoveUISort(this);
		}
	}
}
