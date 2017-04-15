using System;
using System.Collections.Generic;
using UnityEngine;

public class RenderQueueMgr : MonoBehaviour
{
	public GameObject[] fxGameObject;

	public UIPanel panel;

	private List<Material> listMaterial;

	private void Start()
	{
		this.listMaterial = new List<Material>();
		if (this.panel != null)
		{
			this.panel.addRenderQueueManage(this);
		}
		this.init();
	}

	private void init()
	{
		this.setCollectMaterials(base.gameObject);
		this.setSortMaterialRenderQueue(this.listMaterial);
	}

	public void refreshRenderQueue()
	{
		this.listMaterial.Clear();
		this.init();
	}

	private void setCollectMaterials(GameObject goChild)
	{
		Renderer renderer = goChild.transform.renderer;
		if (renderer != null)
		{
			Material[] materials = renderer.materials;
			if (materials != null)
			{
				this.listMaterial.AddRange(materials);
			}
		}
		int childCount = goChild.transform.childCount;
		if (childCount <= 0)
		{
			return;
		}
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = goChild.transform.GetChild(i).gameObject;
			this.setCollectMaterials(gameObject);
		}
	}

	private void setSortMaterialRenderQueue(List<Material> listMaterial)
	{
		if (listMaterial == null || listMaterial.Count <= 1)
		{
			return;
		}
		listMaterial.Sort(delegate(Material x, Material y)
		{
			if (x.renderQueue > y.renderQueue)
			{
				return 1;
			}
			if (x.renderQueue < y.renderQueue)
			{
				return -1;
			}
			if (x.renderQueue == y.renderQueue)
			{
				return 0;
			}
			return (x.GetInstanceID() >= y.GetInstanceID()) ? 1 : -1;
		});
	}

	public void setFxObjectRenderQueue(GameObject gm, int val)
	{
		Renderer[] componentsInChildren = gm.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Renderer renderer = array[i];
			if (renderer.GetComponent<UIRect>() != null)
			{
				renderer.gameObject.AddComponent<UIPanel>().startingRenderQueue = val;
			}
			else
			{
				renderer.material.renderQueue = val;
			}
		}
	}

	public bool setRenderQueueValue(int renderQueueValue)
	{
		int i = 0;
		if (this.fxGameObject != null && this.fxGameObject.Length != 0)
		{
			GameObject[] array = this.fxGameObject;
			for (int j = 0; j < array.Length; j++)
			{
				GameObject gm = array[j];
				this.setFxObjectRenderQueue(gm, renderQueueValue + i);
				i++;
			}
			return true;
		}
		int count;
		if (this.listMaterial != null && (count = this.listMaterial.Count) > 0)
		{
			for (i = 0; i < count; i++)
			{
				Material material = this.listMaterial[i];
				if (material != null)
				{
					material.renderQueue = renderQueueValue + i;
				}
			}
			return true;
		}
		return false;
	}

	public int getAddMaxRenderQueueValue()
	{
		if (this.listMaterial == null)
		{
			return 0;
		}
		return this.listMaterial.Count;
	}

	private void OnDestroy()
	{
		if (this.panel != null)
		{
			this.panel.removeRenderQueueManage(this);
		}
		this.panel = null;
		if (this.listMaterial != null)
		{
			this.listMaterial.Clear();
		}
		this.listMaterial = null;
	}
}
