using System;
using System.Collections.Generic;
using UnityEngine;

public class TransOptimizer : MonoBehaviour
{
	public float hideDelay = 5f;

	private List<Renderer> _renderers = new List<Renderer>();

	private static Shader _tranShader;

	private void Awake()
	{
		if (!TransOptimizer._tranShader)
		{
			TransOptimizer._tranShader = Shader.Find("MyShader/NoLight_transp");
		}
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
		Renderer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Renderer renderer = array[i];
			if (renderer && renderer.sharedMaterial.shader == TransOptimizer._tranShader)
			{
				this._renderers.Add(renderer);
			}
		}
	}

	private void OnEnable()
	{
		this.SetChildrenVisible(true);
		base.Invoke("HideChildren", this.hideDelay);
	}

	private void HideChildren()
	{
		this.SetChildrenVisible(false);
	}

	private void SetChildrenVisible(bool visible)
	{
		foreach (Renderer current in this._renderers)
		{
			if (current && current.gameObject.activeSelf != visible)
			{
				current.gameObject.SetActive(visible);
			}
		}
	}

	private void DoInit()
	{
		if (!TransOptimizer._tranShader)
		{
			TransOptimizer._tranShader = Shader.Find("MyShader/NoLight_transp");
		}
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
		Renderer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Renderer renderer = array[i];
			if (renderer && renderer.sharedMaterial.shader == TransOptimizer._tranShader)
			{
				AutoHide exists = renderer.gameObject.GetComponent<AutoHide>();
				if (!exists)
				{
					exists = renderer.gameObject.AddComponent<AutoHide>();
				}
			}
		}
	}
}
